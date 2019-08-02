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

            lock(service_locker) {

                if (service_locker.Status != 0) {

                    return;
                }

                service_locker.Status = 1;
                this._serviceEvent.Reset();

                this._serviceThread = new Thread(new ThreadStart(ServiceThread));

                if (this._serviceThread.Name == null) {

                    this._serviceThread.Name = "fpnn_push_thread";
                }

                this._serviceThread.Start();
            }
        }

        private void ServiceThread() {

            try {

                while (true) {

                    this._serviceEvent.WaitOne();

                    List<ServiceDelegate> list;

                    lock(service_locker) {

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

            lock(service_locker) {

                if (service_locker.Status == 0) {

                    this.StartServiceThread();
                }

                FPProcessor self = this;
                this._serviceCache.Add(() => {

                    lock (self_locker) {

                        self._processor.Service(data, answer);
                    }
                });

                if (this._serviceCache.Count >= 3000) {

                    this._serviceCache.Clear();
                    ErrorRecorderHolder.recordError(new Exception("Pushs Call Limit!"));
                }

                this._serviceEvent.Set();
            }       
        }

        public void OnSecond(long timestamp) {

            lock (self_locker) {

                if (this._processor != null) {

                    this._processor.OnSecond(timestamp);
                }
            }
        }

        public void Destroy() {

            this.StopServiceThread();
        }
    }
}