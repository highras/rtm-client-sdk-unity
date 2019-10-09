using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace com.fpnn {

    public delegate void ServiceDelegate();

    public class FPManager {

        private class TimerTask {

            public object state;
            public long timestamp;
            public Action<object> callback;
        }

        private class ServiceLocker {

            public int Status = 0;
        }

        private class TimerLocker {

            public int Status = 0;
        }

        private static FPManager instance;
        private static object lock_obj = new object();

        public static FPManager Instance {
            get {
                if (instance == null) {
                    lock (lock_obj) {
                        if (instance == null) {
                            instance = new FPManager();
                        }
                    }
                }

                return instance;
            }
        }

        private FPManager() {}

        private Timer _threadTimer = null;
        private TimerLocker timer_locker = new TimerLocker();
        private List<EventDelegate> _secondCalls = new List<EventDelegate>();

        public void Init() {
            this.StartTaskTimer();
            this.StartTimerThread();
            this.StartServiceThread();
        }

        public void AddSecond(EventDelegate callback) {
            lock (timer_locker) {
                if (this._secondCalls.Count >= 50) {
                    ErrorRecorderHolder.recordError(new Exception("Seond Calls Limit!"));
                    return;
                }

                this._secondCalls.Add(callback);
            }

            this.StartTimerThread();
        }

        public void RemoveSecond(EventDelegate callback) {
            lock (timer_locker) {
                int index = this._secondCalls.IndexOf(callback);

                if (index != -1) {
                    this._secondCalls.RemoveAt(index);
                }
            }
        }

        public void StartTimerThread() {
            lock (timer_locker) {
                if (timer_locker.Status != 0) {
                    return;
                }

                timer_locker.Status = 1;

                if (this._threadTimer == null) {
                    try {
                        this._threadTimer = new Timer(new TimerCallback(OnSecond), null, 1000, 1000);
                    } catch (Exception ex) {
                        ErrorRecorderHolder.recordError(ex);
                    }
                }
            }
        }

        private void OnSecond(object state) {
            lock (timer_locker) {
                this.CallSecond(this._secondCalls);
            }
        }

        private void CallSecond(ICollection<EventDelegate> list) {
            foreach (EventDelegate service in list) {
                if (service != null) {
                    this.EventTask(service, new EventData("second", FPManager.Instance.GetMilliTimestamp()));
                }
            }
        }

        public void StopTimerThread() {
            lock (timer_locker) {
                timer_locker.Status = 0;

                if (this._threadTimer != null) {
                    try {
                        this._threadTimer.Dispose();
                        this._threadTimer = null;
                    } catch (Exception ex) {
                        ErrorRecorderHolder.recordError(ex);
                    }
                }

                this._secondCalls.Clear();
            }
        }


        private Thread _serviceThread = null;
        private ManualResetEvent _serviceEvent = new ManualResetEvent(false);

        private ServiceLocker service_locker = new ServiceLocker();

        private void StartServiceThread() {
            lock (service_locker) {
                if (service_locker.Status != 0) {
                    return;
                }

                service_locker.Status = 1;

                try {
                    this._serviceThread = new Thread(new ThreadStart(ServiceThread));

                    if (this._serviceThread.Name == null) {
                        this._serviceThread.Name = "FPNN-SERVICE";
                    }

                    this._serviceThread.Start();
                } catch (Exception ex) {
                    ErrorRecorderHolder.recordError(ex);
                }
            }
        }

        private void ServiceThread() {
            try {
                while (true) {
                    this._serviceEvent.WaitOne();
                    List<ServiceDelegate> list;

                    lock (service_locker) {
                        if (service_locker.Status == 0) {
                            return;
                        }

                        list = this._serviceCache;
                        this._serviceCache = new List<ServiceDelegate>();
                    }

                    this._serviceEvent.Reset();
                    this.CallService(list);
                }
            } catch (ThreadAbortException tex) {
            } catch (Exception ex) {
                ErrorRecorderHolder.recordError(ex);
            } finally {
                this.StopServiceThread();
            }
        }

        private void CallService(ICollection<ServiceDelegate> list) {
            foreach (ServiceDelegate service in list) {
                if (service != null) {
                    try {
                        service();
                    } catch (Exception ex) {
                        ErrorRecorderHolder.recordError(ex);
                    }
                }
            }
        }

        private void StopServiceThread() {
            lock (service_locker) {
                if (service_locker.Status == 1) {
                    try {
                        this._serviceEvent.Set();
                    } catch (Exception ex) {
                        ErrorRecorderHolder.recordError(ex);
                    }

                    service_locker.Status = 0;
                    this._serviceCache.Clear();
                }
            }
        }

        private List<ServiceDelegate> _serviceCache = new List<ServiceDelegate>();

        public void EventTask(EventDelegate callback, EventData evd) {
            this.AddService(() => {
                if (callback != null) {
                    callback(evd);
                }
            });
        }

        public void CallbackTask(CallbackDelegate callback, CallbackData cbd) {
            this.AddService(() => {
                if (callback != null) {
                    callback(cbd);
                }
            });
        }

        public void ExecTask(Action<object> taskAction, object state) {
            this.AddService(() => {
                if (taskAction != null) {
                    taskAction(state);
                }
            });
        }

        public void DelayTask(int milliSecond, Action<object> taskAction, object state) {
            if (milliSecond <= 0) {
                this.ExecTask(taskAction, state);
                return;
            }

            TimerTask task = new TimerTask();
            task.state = state;
            task.callback = taskAction;
            task.timestamp = this.GetMilliTimestamp() + milliSecond;
            this.AddTimerTask(task);
        }

        private void AddService(ServiceDelegate service) {
            this.StartServiceThread();

            lock (service_locker) {
                if (this._serviceCache.Count < 3000) {
                    this._serviceCache.Add(service);
                }

                if (this._serviceCache.Count == 2998) {
                    ErrorRecorderHolder.recordError(new Exception("Service Calls Limit!"));
                }
            }

            try {
                this._serviceEvent.Set();
            } catch (Exception ex) {
                ErrorRecorderHolder.recordError(ex);
            }
        }


        private Timer _taskTimer = null;
        private TimerLocker task_locker = new TimerLocker();
        private List<TimerTask> _timerTaskQueue = new List<TimerTask>();

        private void AddTimerTask(TimerTask task) {
            if (task == null) {
                return;
            }

            this.StartTaskTimer();

            lock (task_locker) {
                if (this._timerTaskQueue.Count < 3000) {
                    int index = this._timerTaskQueue.Count;

                    for (int i = 0; i < this._timerTaskQueue.Count; i++) {
                        if (task.timestamp < this._timerTaskQueue[i].timestamp) {
                            index = i;
                            break;
                        }
                    }

                    this._timerTaskQueue.Insert(index, task);

                    if (this._timerTaskQueue.Count == 2998) {
                        ErrorRecorderHolder.recordError(new Exception("TimerTask Calls Limit!"));
                    }

                    if (index == 0) {
                        this.TimerOneTask();
                    }
                }
            }
        }

        private void TimerOneTask() {
            if (this._timerTaskQueue.Count > 0) {
                TimerTask task = this._timerTaskQueue[0];
                int ts = Math.Max(0, Convert.ToInt32(task.timestamp - this.GetMilliTimestamp()));
                this._taskTimer.Change(ts, Timeout.Infinite);
            }
        }

        private void StartTaskTimer() {
            lock (task_locker) {
                if (task_locker.Status != 0) {
                    return;
                }

                task_locker.Status = 1;

                if (this._taskTimer == null) {
                    try {
                        this._taskTimer = new Timer(new TimerCallback(OnTask), null, Timeout.Infinite, Timeout.Infinite);
                    } catch (Exception ex) {
                        ErrorRecorderHolder.recordError(ex);
                    }
                }
            }
        }

        private void OnTask(object state) {
            lock (task_locker) {
                if (this._timerTaskQueue.Count > 0) {
                    TimerTask task = this._timerTaskQueue[0];

                    if (task != null) {
                        this.ExecTask(task.callback, task.state);
                    }

                    this._timerTaskQueue.RemoveAt(0);
                }

                this.TimerOneTask();
            }
        }

        public void StopTaskTimer() {
            lock (task_locker) {
                task_locker.Status = 0;

                if (this._taskTimer != null) {
                    try {
                        this._taskTimer.Dispose();
                        this._taskTimer = null;
                    } catch (Exception ex) {
                        ErrorRecorderHolder.recordError(ex);
                    }
                }
            }
        }


        public Int64 GetMilliTimestamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        public int GetTimestamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt32(ts.TotalSeconds);
        }
    }
}