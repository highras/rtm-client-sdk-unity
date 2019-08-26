using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using GameDevWare.Serialization;
using com.fpnn;

namespace com.rtm {

    public class RTMSender {

        private class ServiceLocker {

            public int Status = 0;
        }

        private bool _destroyed;
        private object self_locker = new object();

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

                        this._serviceThread.Name = "RTM-SENDER";
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

        public void AddQuest(FPClient client, FPData data, IDictionary<string, object> payload, CallbackDelegate callback, int timeout) {

            this.AddService(() => {

                if (client != null) {

                    byte[] bytes;

                    using (MemoryStream outputStream = new MemoryStream()) {

                        MsgPack.Serialize(payload, outputStream);
                        outputStream.Seek(0, SeekOrigin.Begin);

                        bytes = outputStream.ToArray();
                    }

                    data.SetPayload(bytes);
                    client.SendQuest(data, callback, timeout);
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

                    ErrorRecorderHolder.recordError(new Exception("Quest Calls Limit!"));
                }
            } 
            
            try {

                this._serviceEvent.Set();
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