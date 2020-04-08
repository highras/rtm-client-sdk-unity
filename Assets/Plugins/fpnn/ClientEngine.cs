using System;
using System.Collections.Generic;
using System.Threading;
namespace com.fpnn
{
    public static partial class ClientEngine
    {
        private static volatile bool inited;
        private static volatile bool stopped;
        private static object interLocker;
        private static Thread routineThread;
        private static Semaphore quitSemaphore;
        private static bool forbiddenRegisterConnection;        //-- Unity iOS only.
        private static Dictionary<TCPConnection, Int64> connectingConnections;
        private static HashSet<TCPConnection> allConnections;
        private static common.TaskThreadPool taskPool;

        internal static DateTime originDateTime;
        internal static int globalConnectTimeoutSeconds;
        internal static int globalQuestTimeoutSeconds;
        internal static int maxPayloadSize;
        internal static common.ErrorRecorder errorRecorder;

        static partial void PlatformInit();     //-- In lock (interLocker) {...}
        static partial void PlatformUninit();

        static ClientEngine()
        {
            inited = false;
            interLocker = new object();
        }

        /*
         * Is NOT necessary, just uniform the interfaces with Unity version.
         */
        public static void Init()
        {
            Init(null);
        }

        /*
         * Customized Init.
         */
        public static void Init(Config config)
        {
            if (inited)
                return;

            lock (interLocker)
            {
                if (inited)
                    return;

                if (config == null)
                    config = new Config();

                //---------------------

                stopped = false;
                forbiddenRegisterConnection = false;
                connectingConnections = new Dictionary<TCPConnection, long>();
                allConnections = new HashSet<TCPConnection>();
                originDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

                globalConnectTimeoutSeconds = config.globalConnectTimeoutSeconds;
                globalQuestTimeoutSeconds = config.globalQuestTimeoutSeconds;
                maxPayloadSize = config.maxPayloadSize;
                errorRecorder = config.errorRecorder;

                taskPool = new common.TaskThreadPool(config.taskThreadPoolConfig.initThreadCount,
                    config.taskThreadPoolConfig.perfectThreadCount,
                    config.taskThreadPoolConfig.maxThreadCount,
                    config.taskThreadPoolConfig.maxQueueLengthLimitation,
                    config.taskThreadPoolConfig.tempLatencySeconds,
#if UNITY_2017_1_OR_NEWER
                    false
#else
                    true
#endif
                    );

                taskPool.SetErrorRecorder(config.errorRecorder);

                routineThread = new Thread(RoutineFunc)
                {
                    Name = "FPNN.ClientEngine.RoutineThread",
                    IsBackground = true
                };
                routineThread.Start();

                //---------------------

                PlatformInit();

                inited = true;
            }
        }

        private static void CheckInitStatus()
        {
            Init(null);
        }

        private static void RoutineFunc()
        {
            while (!stopped)
            {
                Thread.Sleep(1000);

                Int64 currentSeconds = GetCurrentSeconds();
                HashSet<TCPConnection> checkingConnections = new HashSet<TCPConnection>();
                HashSet<TCPConnection> connectingTimeoutedConnections = new HashSet<TCPConnection>();
                lock (interLocker)
                {
                    foreach (TCPConnection conn in allConnections)
                        checkingConnections.Add(conn);

                    foreach (KeyValuePair<TCPConnection, Int64> kvp in connectingConnections)
                    {
                        if (kvp.Value <= currentSeconds)
                            connectingTimeoutedConnections.Add(kvp.Key);
                    }
                }

                foreach (TCPConnection conn in connectingTimeoutedConnections)
                {
                    conn.Close();
                }

                currentSeconds = GetCurrentSeconds();
                foreach (TCPConnection conn in checkingConnections)
                {
                    conn.CleanTimeoutedCallbacks(currentSeconds);
                }
            }

            StopAllConnections();
            quitSemaphore.Release();
        }

        /*
         * Only for Unity on iOS devices when apps is going to background.
         */
        internal static void StopAllConnections()
        {
            if (inited == false)
                return;

            CheckInitStatus();

            HashSet<TCPConnection> currentConnections = new HashSet<TCPConnection>();
            lock (interLocker)
            {
                foreach (TCPConnection conn in allConnections)
                    currentConnections.Add(conn);
            }

            foreach (TCPConnection conn in currentConnections)
            {
                conn.Close();
            }
        }

        /*
         * Only for Unity on iOS devices when apps is going to background.
         */
        internal static void ChangeForbiddenRegisterConnection(bool forbidden)
        {
            lock (interLocker)
            {
                forbiddenRegisterConnection = forbidden;
            }
        }

        internal static bool RegisterConnectingConnection(TCPConnection conn, int connectTimeout)
        {
            CheckInitStatus();
            lock (interLocker)
            {
                if (forbiddenRegisterConnection)    //-- Unity iOS only.
                    return false;

                if (connectTimeout <= 0)
                    connectTimeout = globalQuestTimeoutSeconds;

                connectingConnections.Add(conn, GetCurrentSeconds() + connectTimeout);
                allConnections.Add(conn);
            }
            
            return true;
        }

        internal static bool RegisterConnectedConnection(TCPConnection conn)
        {
            CheckInitStatus();
            lock (interLocker)
            {
                if (forbiddenRegisterConnection)    //-- Unity iOS only.
                    return false;

                connectingConnections.Remove(conn);
                allConnections.Add(conn);
            }

            return true;
        }

        internal static void UnregisterConnection(TCPConnection conn)
        {
            CheckInitStatus();
            lock (interLocker)
            {
                connectingConnections.Remove(conn);
                allConnections.Remove(conn);
            }
        }

        internal static bool RunTask(common.TaskThreadPool.ITask task)
        {
            CheckInitStatus();
            return taskPool.Wakeup(task);
        }

        internal static bool RunTask(Action action)
        {
            CheckInitStatus();
            return taskPool.Wakeup(action);
        }

        public static Int64 GetCurrentSeconds()
        {
            TimeSpan span = DateTime.UtcNow - originDateTime;
            return (Int64)Math.Floor(span.TotalSeconds);
        }

        public static Int64 GetCurrentMilliseconds()
        {
            TimeSpan span = DateTime.UtcNow - originDateTime;
            return (Int64)Math.Floor(span.TotalMilliseconds);
        }

        public static Int64 GetCurrentMicroseconds()
        {
            TimeSpan span = DateTime.UtcNow - originDateTime;
            return (Int64)Math.Floor(span.TotalMilliseconds * 1000);
        }

        public static void Close()
        {
            lock (interLocker)
            {
                if (stopped)
                    return;

                quitSemaphore = new Semaphore(0, 1);
                stopped = true;
            }
            quitSemaphore.WaitOne();
            quitSemaphore.Close();
            quitSemaphore = null;

            PlatformUninit();
            taskPool.Close();
        }
    }
}
