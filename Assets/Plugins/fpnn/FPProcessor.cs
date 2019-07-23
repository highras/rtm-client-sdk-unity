using System;
using System.Collections;
using System.Collections.Generic;

namespace com.fpnn {

    public delegate void ServiceDelegate();
    public delegate void AnswerDelegate(object payload, bool exception);

    public class FPProcessor {

        public interface IProcessor {

            void Service(FPData data, AnswerDelegate answer);
            void OnSecond(long timestamp);
            bool HasPushService(string name);
            FPEvent GetEvent();
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

            public FPEvent GetEvent() {

                return this._event;
            }

            public void OnSecond(long timestamp) {}
        }

        private IProcessor _processor;

        public FPEvent GetEvent() {

            if (this._processor != null) {

                return this._processor.GetEvent();
            }

            return null;
        }

        public void SetProcessor(IProcessor processor) {

            this._processor = processor;
        }

        private bool _serviceAble;
        private System.Threading.ManualResetEvent _serviceEvent = new System.Threading.ManualResetEvent(false);

        private void StartServiceThread() {

            lock(service_locker) {

                if (this._serviceAble) {

                    return;
                }

                this._serviceAble = true;
                this._serviceEvent.Reset();
            }

            FPProcessor self = this;

            ThreadPool.Instance.Execute((state) => {

                while (self._serviceAble) {

                    try {

                        self._serviceEvent.WaitOne();

                        List<ServiceDelegate> list;

                        lock(self.service_locker) {

                            list = self._serviceCache;
                            self._serviceCache = new List<ServiceDelegate>();

                            self._serviceEvent.Reset();
                        }

                        self.CallService(list);
                    } catch (System.Threading.ThreadAbortException tex) {
                    } catch (Exception e) {

                        ErrorRecorderHolder.recordError(e);
                    } finally {

                        self.StopServiceThread();
                    }
                }
            });
        }

        private void CallService(ICollection<ServiceDelegate> list) {

            foreach (ServiceDelegate service in list) {

                if (service != null) {

                    service();
                }
            }
        }

        private void StopServiceThread() {

            lock(service_locker) {

                this._serviceEvent.Set();
            }

            this._serviceAble = false;
        }

        private System.Object service_locker = new System.Object();
        private List<ServiceDelegate> _serviceCache = new List<ServiceDelegate>();

        public void Service(FPData data, AnswerDelegate answer) {

            if (this._processor == null) {

                this._processor = new BaseProcessor();
            }

            if (!this._processor.HasPushService(data.GetMethod())) {

                if (data.GetMethod() != "ping") {

                    return;
                }
            }

            if (!this._serviceAble) {

                this.StartServiceThread();
            } 

            FPProcessor self = this;

            lock(service_locker) {

                this._serviceCache.Add(() => {

                    self._processor.Service(data, answer);
                });

                if (this._serviceCache.Count >= 100) {

                    this._serviceCache.Clear();
                }

                this._serviceEvent.Set();
            }       
        }

        public void OnSecond(long timestamp) {

            if (this._processor != null) {

                this._processor.OnSecond(timestamp);
            }
        }

        public void Destroy() {

            this.StopServiceThread();
        }
    }
}