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
            FPEvent GetEvent();
        }

        private class BaseProcessor:IProcessor {

            private FPEvent _event = new FPEvent();

            public void Service(FPData data, AnswerDelegate answer) {

                // TODO 
                
                if (data.GetFlag() == 0) {

                }

                if (data.GetFlag() == 1) {

                }
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
        private System.Threading.ManualResetEvent _serviceEvent = new System.Threading.ManualResetEvent(true);

        private void StartServiceThread() {

            if (this._serviceAble) {

                return;
            }

            FPProcessor self = this;
            this._serviceAble = true;

            ThreadPool.Instance.Execute((state) => {

                System.Threading.Thread.CurrentThread.Name = "fpnn_service_thread";

                try {

                    while (self._serviceAble) {

                        self._serviceEvent.WaitOne();

                        int count = 0;
                        ServiceDelegate service = null;

                        lock(service_locker) {

                            if (self._serviceCache.Count > 0) {

                                service = self._serviceCache[0];
                                self._serviceCache.RemoveAt(0);

                            }

                            count = self._serviceCache.Count;
                        }

                        if (service != null) {

                            service();
                        }

                        if (count == 0) {

                            self._serviceEvent.Reset();
                        }
                    }
                } catch (System.Threading.ThreadAbortException tex) {
                } catch (Exception e) {

                    ErrorRecorderHolder.recordError(e);
                }

                System.Threading.Thread.CurrentThread.Name = null;
            });
        }

        private void StopServiceThread() {

            this._serviceEvent.Reset();
            this._serviceAble = false;
        }

        private System.Object service_locker = new System.Object();
        private List<ServiceDelegate> _serviceCache = new List<ServiceDelegate>();

        public void Service(FPData data, AnswerDelegate answer) {

            if (this._processor == null) {

                this._processor = new BaseProcessor();
            }

            FPProcessor self = this;

            lock(service_locker) {

                this._serviceCache.Add(() => {

                    self._processor.Service(data, answer);
                });

                if (this._serviceCache.Count >= 100) {

                    this._serviceCache.Clear();
                }
            }            

            if (!this._serviceAble) {

                this.StartServiceThread();
            } else {

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
            this._serviceCache.Clear();
        }
    }
}