using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace com.fpnn {

    public delegate void AnswerDelegate(object payload, bool exception);

    public class FPProcessor {

        public interface IProcessor {

            void Service(FPData data, AnswerDelegate answer);
            void OnSecond(long timestamp);
            bool HasPushService(string name);
        }

        private class ServiceLocker {

            public int Status = 0;
        }

        private class BaseProcessor:IProcessor {

            private FPEvent _event = new FPEvent();

            public void Service(FPData data, AnswerDelegate answer) {

                // TODO 
                if (data.GetFlag() == 0) {}
                if (data.GetFlag() == 1) {}
            }

            public bool HasPushService(string name) {

                return false;
            }

            public void OnSecond(long timestamp) {}
        }

        private bool _destroyed;
        private IProcessor _processor;
        private object self_locker = new object();

        public void SetProcessor(IProcessor processor) {

            lock (self_locker) {

                this._processor = processor;
            }
        }

        private Thread _serviceThread = null;
        private ManualResetEvent _serviceEvent = new ManualResetEvent(false);

        private ServiceLocker service_locker = new ServiceLocker();

        private void StartServiceThread() {

            lock (self_locker) {

                if (this._destroyed) {

                    return;
                }
            }

            lock (service_locker) {

                if (service_locker.Status != 0) {

                    return;
                }

                service_locker.Status = 1;

                try {

                    this._serviceThread = new Thread(new ThreadStart(ServiceThread));

                    if (this._serviceThread.Name == null) {

                        this._serviceThread.Name = "FPNN-PUSH";
                    }

                    this._serviceThread.Start();
                } catch(Exception ex) {

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

                try {

                    if (service != null) {

                        service();
                    }
                } catch(Exception ex) {

                    ErrorRecorderHolder.recordError(ex);
                }
            }
        }

        private void StopServiceThread() {

            lock (service_locker) {

                if (service_locker.Status != 0) {

                    service_locker.Status = 0;

                    try {

                        this._serviceEvent.Set();
                    } catch(Exception ex) {

                        ErrorRecorderHolder.recordError(ex);
                    }
                }
            }
        }

        private List<ServiceDelegate> _serviceCache = new List<ServiceDelegate>();

        public void Service(FPData data, AnswerDelegate answer) {

            lock (self_locker) {

                if (this._processor == null) {

                    this._processor = new BaseProcessor();
                }

                if (!this._processor.HasPushService(data.GetMethod())) {

                    if (data.GetMethod() != "ping") {

                        return;
                    }
                }
            }

            FPProcessor self = this;

            this.AddService(() => {

                lock (self_locker) {

                    self._processor.Service(data, answer);
                }
            });
        }

        private void AddService(ServiceDelegate service) {

            lock (self_locker) {

                if (this._destroyed) {

                    return;
                }
            }

            this.StartServiceThread();

            lock (service_locker) {

                if (this._serviceCache.Count < 3000) {

                    this._serviceCache.Add(service);
                } 

                if (this._serviceCache.Count == 2998) {

                    ErrorRecorderHolder.recordError(new Exception("Push Calls Limit!"));
                }
            } 

            try {

                this._serviceEvent.Set();
            } catch(Exception ex) {

                ErrorRecorderHolder.recordError(ex);
            }
        }

        public void OnSecond(long timestamp) {

            try {

                lock (self_locker) {

                    if (this._processor != null) {

                        this._processor.OnSecond(timestamp);
                    }
                }
            } catch(Exception ex) {

                ErrorRecorderHolder.recordError(ex);
            }
        }

        public void Destroy() {

            lock (self_locker) {

                if (this._destroyed) {

                    return;
                }

                this._destroyed = true;
            }

            this.StopServiceThread();

            lock (service_locker) {

                this._serviceCache.Clear();
            }
        }
    }
}