using System;
using System.Collections.Generic;
using System.Threading;

namespace com.fpnn.common
{
    /*
     * Close() is not necessary when using background thread.
     * 
     * If using ErrorRecoder, please call TaskThreadPool.SetDefaultErrorRecorder()
     *      before creating any instance, or call SetErrorRecorder() for each instance
     *      before WakeUp()s are called.
     */
    public class TaskThreadPool
    {
        public interface ITask
        {
            void Run();
        }

        class ActionTask: ITask
        {
            Action action;

            public ActionTask(Action act)
            {
                action = act;
            }
            public void Run()
            {
                action();
            }
        }

        //----------------[ fields ]-----------------------//
        private readonly int perfectCount;
        private readonly int maxCount;
        private readonly int maxQueueLength;

        private static ErrorRecorder defaultErrorRecorder;

        private class TaskThreadPoolCore
        {
            public bool backgroundThread;
            public int normalThreadCount;
            public int busyThreadCount;
            public int tempThreadCount;

            public bool stopped;
            public Semaphore semaphore;
            public Semaphore quitSemaphore;
            public Queue<ITask> taskQueue;

            public readonly int tempThreadLatencySeconds;
            public ErrorRecorder errorRecorder;

            public TaskThreadPoolCore(int tempLatencySeconds, bool usingBackgroundThread)
            {
                backgroundThread = usingBackgroundThread;
                normalThreadCount = 0;
                busyThreadCount = 0;
                tempThreadCount = 0;

                stopped = false;
                semaphore = new Semaphore(0, Int32.MaxValue);
                taskQueue = new Queue<ITask>();

                tempThreadLatencySeconds = tempLatencySeconds;
                errorRecorder = defaultErrorRecorder;
            }
        }

        private readonly TaskThreadPoolCore core;

        //----------------[ Constructor ]-----------------------//

        public TaskThreadPool(int initThreadCount, int perfectThreadCount, int maxThreadCount, int maxQueueLengthLimitation = 0, int tempLatencySeconds = 60, bool usingBackGroundThread = true)
        {
            if (initThreadCount < 0)
                throw new ArgumentException("Param initThreadCount is less than Zero.", nameof(initThreadCount));

            if (maxThreadCount <= 0)
                throw new ArgumentException("Param maxThreadCount is less than or equal to Zero.", nameof(maxThreadCount));

            if (perfectThreadCount < initThreadCount)
                throw new ArgumentOutOfRangeException(nameof(perfectThreadCount), "Param perfectThreadCount is less than initThreadCount");

            if (maxThreadCount < perfectThreadCount)
                throw new ArgumentOutOfRangeException(nameof(maxThreadCount), "Param maxThreadCount is less than perfectThreadCount");

            perfectCount = perfectThreadCount;
            maxCount = maxThreadCount;
            maxQueueLength = maxQueueLengthLimitation;

            core = new TaskThreadPoolCore(tempLatencySeconds, usingBackGroundThread);

            for (int i = 0; i < initThreadCount; i++)
            {
                var thread = new Thread(Worker)
                {
                    Name = "FPNN.ThreadPool.NormalWorker",
                    IsBackground = core.backgroundThread
                };
                thread.Start(core);
                core.normalThreadCount++;           //-- Unneed lock in there.
            }
        }

        public static void SetDefaultErrorRecorder(ErrorRecorder er)
        {
            defaultErrorRecorder = er;
        }

        public void SetErrorRecorder(ErrorRecorder er)
        {
            core.errorRecorder = er;
        }

        private void Append(bool normalThread)
        {
            try
            {
                Thread thread;
                if (normalThread)
                {
                    thread = new Thread(Worker)
                    {
                        Name = "FPNN.ThreadPool.NormalWorker"
                    };
                }
                else
                {
                    thread = new Thread(TempWorker)
                    {
                        Name = "FPNN.ThreadPool.TempWorker"
                    };
                }

                thread.IsBackground = core.backgroundThread;
                thread.Start(core);
            }
            catch (Exception e)
            {
                lock (core)
                {
                    if (normalThread)
                        core.normalThreadCount--;
                    else
                        core.tempThreadCount--;
                }
                throw e;
            }
        }

        public bool Wakeup(ITask task)
        {
            if (task == null)
                return false;

            bool needAppend = false;
            bool appendNormalThread = false;
            lock (core)
            {
                if (core.stopped)
                    return false;

                if (maxQueueLength > 0 && core.taskQueue.Count >= maxQueueLength)
                    return false;

                core.taskQueue.Enqueue(task);

                if (core.busyThreadCount + core.taskQueue.Count >= core.normalThreadCount + core.tempThreadCount)
                {
                    if (core.normalThreadCount < perfectCount)
                    {
                        needAppend = true;
                        appendNormalThread = true;
                        core.normalThreadCount++;
                    }
                    else if (core.normalThreadCount + core.tempThreadCount < maxCount)
                    {
                        needAppend = true;
                        appendNormalThread = false;
                        core.tempThreadCount++;
                    }
                }   
            }
            if (needAppend)
            {
                Append(appendNormalThread);
            }

            core.semaphore.Release();

            return true;
        }

        public bool Wakeup(Action action)
        {
            ActionTask task = new ActionTask(action);
            return Wakeup(task);
        }

        //-- Please call this function in locked status.
        private static void ExitWorker(bool normal, TaskThreadPoolCore core)
        {
            if (normal)
                core.normalThreadCount--;
            else
                core.tempThreadCount--;

            if (core.normalThreadCount == 0 && core.tempThreadCount == 0 && core.stopped && core.backgroundThread == false)
                core.quitSemaphore.Release();
        }

        private static void Worker(Object obj)
        {
            TaskThreadPoolCore core = (TaskThreadPoolCore)obj;

            while (true)
            {
                ITask task = null;
                core.semaphore.WaitOne();
                lock (core)
                {
                    if (core.taskQueue.Count > 0)
                    {
                        task = core.taskQueue.Dequeue();
                        core.busyThreadCount++;
                    }
                    else if (core.stopped)
                    {
                        ExitWorker(true, core);
                        return;
                    }
                    else
                        continue;
                }

                try
                {
                    task.Run();
                }
                catch (Exception e)
                {
                    if (core.errorRecorder != null)
                        core.errorRecorder.RecordError(e);
                }
                finally
                {
                    lock(core)
                    {
                        core.busyThreadCount--;
                    }
                }
            }
        }

        private static void TempWorker(Object obj)
        {
            TaskThreadPoolCore core = (TaskThreadPoolCore)obj;
            int latencySeconds = core.tempThreadLatencySeconds;
            DateTime idleTime = DateTime.Now;

            while (true)
            {
                ITask task = null;
                core.semaphore.WaitOne(latencySeconds * 1000);
                lock (core)
                {
                    if (core.taskQueue.Count > 0)
                    {
                        task = core.taskQueue.Dequeue();
                        core.busyThreadCount++;
                    }
                    else if (core.stopped)
                    {
                        ExitWorker(false, core);
                        return;
                    }
                    else
                    {
                        TimeSpan duration = DateTime.Now - idleTime;
                        latencySeconds -= Convert.ToInt32(duration.TotalSeconds);

                        if (latencySeconds <= 0)
                        {
                            ExitWorker(false, core);
                            return;
                        }
                        else
                            continue;
                    }
                }

                try
                {
                    task.Run();
                }
                catch (Exception e)
                {
                    if (core.errorRecorder != null)
                        core.errorRecorder.RecordError(e);
                }
                finally
                {
                    lock (core)
                    {
                        core.busyThreadCount--;
                    }
                }

                idleTime = DateTime.Now;
            }
        }

        public void Close(bool dropAllTasks = false)      //-- Synchronous method.
        {
            bool active = true;

            lock (core)
            {
                if (core.stopped)
                    return;

                if (dropAllTasks)
                    core.taskQueue.Clear();

                if (core.backgroundThread || (core.normalThreadCount == 0 && core.tempThreadCount == 0))
                    active = false;
                else
                    core.quitSemaphore = new Semaphore(0, 1);

                core.stopped = true;
            }

            core.semaphore.Release(maxCount);

            if (active)
            {
                core.quitSemaphore.WaitOne();
                core.quitSemaphore.Close();
            }

            core.semaphore.Close();
        }

        ~TaskThreadPool()
        {
            Close();
        }
    }
}
