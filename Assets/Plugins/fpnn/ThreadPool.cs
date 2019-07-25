using System;

namespace com.fpnn {

    public class ThreadPool {

        public interface IThreadPool {

            void Execute(Action<object> action);
        }

        private class BaseThreadPool:IThreadPool {

            public void Execute(Action<object> action) {

                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(action));
            }
        }

        private static ThreadPool instance;
        private static object lock_obj = new object();

        public static ThreadPool Instance {

            get{

                if (instance == null) {

                    lock (lock_obj) {

                        if (instance == null) {

                            instance = new ThreadPool();
                        }
                    }
                }

                return instance;
            }
        }

        private bool _hasTimerThread;
        private IThreadPool _threadPool = null;
        private System.Threading.Timer _threadTimer = null;

        public FPEvent Event = new FPEvent();

        public void SetPool(IThreadPool value) {

            lock(lock_obj) {

                if (this._threadPool == null) {

                    this._threadPool = value;
                }
            }
        }

        public IThreadPool GetThreadPool() {

            return this._threadPool;
        }

        public void Execute(Action<object> action) {

            if (this._threadPool == null) {

                this.SetPool(new BaseThreadPool());
            }

            this._threadPool.Execute(action);
        }

        public void StartTimerThread() {

            lock(lock_obj) {

                if (!this._hasTimerThread) {

                    this._hasTimerThread = true;
                    this._threadTimer = new System.Threading.Timer(new System.Threading.TimerCallback(OnSecond), null, 0, 1000);
                }
            }
        }

        private void OnSecond(object state) {

            this.Event.FireEvent(new EventData("second", this.GetMilliTimestamp()));
        }

        public void StopTimerThread() {

            lock(lock_obj) {

                if (this._hasTimerThread) {

                    this._hasTimerThread = false;
                    this._threadTimer.Dispose();
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