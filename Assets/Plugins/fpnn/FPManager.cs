using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace com.fpnn {

    public delegate void ServiceDelegate();

    public class FPManager {

        private class ServiceLocker {

            public int Status = 0;
        }

        private class TimerLocker {

            public int Status = 0;
        }

        private static FPManager instance;
        private static object lock_obj = new object();

        public static FPManager Instance {

            get{

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

        public FPManager() {}

        private Timer _threadTimer = null;
        private TimerLocker timer_locker = new TimerLocker();
        private List<EventDelegate> _secondCalls = new List<EventDelegate>();

        public void AddSecond(EventDelegate callback) {

            this.StartTimerThread();

            lock(timer_locker) {

                if (this._secondCalls.Count >= 50) {

                    ErrorRecorderHolder.recordError(new Exception("Seond Calls Limit!"));
                    return;
                }

                this._secondCalls.Add(callback);
            }       
        }

        public void RemoveSecond(EventDelegate callback) {

            lock(timer_locker) {

                int index = this._secondCalls.IndexOf(callback);

                if (index != -1) {

                    this._secondCalls.RemoveAt(index);
                }

                if (this._secondCalls.Count == 0) {

                    this.StopTimerThread();
                }
            }
        }

        private void StartTimerThread() {

            lock(timer_locker) {

                if (timer_locker.Status != 0) {

                    return;
                }

                timer_locker.Status = 1;

                if (this._threadTimer == null) {

                    this._threadTimer = new Timer(new TimerCallback(OnSecond), null, 0, 1000);
                }
            }
        }

        private void OnSecond(object state) {

            lock(timer_locker) {

                this.CallSecond(this._secondCalls);
            }
        }

        private void CallSecond(ICollection<EventDelegate> list) {

            foreach (EventDelegate service in list) {

                if (service != null) {

                    try {

                        service(new EventData("second", FPManager.Instance.GetMilliTimestamp()));
                    } catch(Exception ex) {

                        ErrorRecorderHolder.recordError(ex);
                    }
                }
            }
        }

        private void StopTimerThread() {

            lock(timer_locker) {

                timer_locker.Status = 0;

                if (this._threadTimer != null) {

                    this._threadTimer.Dispose();
                    this._threadTimer = null;
                }
            }
        }

        private Thread _serviceThread = null;
        private ManualResetEvent _serviceEvent = new ManualResetEvent(false);

        private ServiceLocker service_locker = new ServiceLocker();

        private void StartServiceThread() {

            lock(service_locker) {

                if (service_locker.Status != 0) {

                    return;
                }

                service_locker.Status = 1;
                this._serviceEvent.Reset();

                this._serviceThread = new Thread(new ThreadStart(ServiceThread));

                if (this._serviceThread.Name == null) {

                    this._serviceThread.Name = "fpnn_service_thread";
                }

                this._serviceThread.Start();
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

                        this._serviceEvent.Reset();
                    }

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
                    } catch(Exception ex) {

                        ErrorRecorderHolder.recordError(ex);
                    }
                }
            }
        }

        private void StopServiceThread() {

            lock(service_locker) {

                service_locker.Status = 0;
                this._serviceEvent.Set();
            }
        }

        private List<ServiceDelegate> _serviceCache = new List<ServiceDelegate>();

        public void AddEventCall(EventDelegate callback, EventData evd) {

            this.StartServiceThread();

            lock(service_locker) {

                if (this._serviceCache.Count < 3000) {

                    this._serviceCache.Add(() => {

                        if (callback != null) {

                            callback(evd);
                        }
                    });
                } 

                if (this._serviceCache.Count == 2998) {

                    ErrorRecorderHolder.recordError(new Exception("Event Calls Limit!"));
                }

                this._serviceEvent.Set();
            }       
        }

        public void AddBackCall(CallbackDelegate callback, CallbackData cbd) {

            this.StartServiceThread();

            lock(service_locker) {

                if (this._serviceCache.Count < 3000) {

                    this._serviceCache.Add(() => {

                        if (callback != null) {

                            callback(cbd);
                        }
                    });
                } 

                if (this._serviceCache.Count == 2998) {

                    ErrorRecorderHolder.recordError(new Exception("Back Calls Limit!"));
                }

                this._serviceEvent.Set();
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