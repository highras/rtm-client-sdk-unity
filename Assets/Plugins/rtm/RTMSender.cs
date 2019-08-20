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

            lock(service_locker) {

                if (service_locker.Status != 0) {

                    return;
                }

                service_locker.Status = 1;
                this._serviceEvent.Reset();

                this._serviceThread = new Thread(new ThreadStart(ServiceThread));

                if (this._serviceThread.Name == null) {

                    this._serviceThread.Name = "rtm_sender_thread";
                }

                this._serviceThread.IsBackground = true;
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

        public void AddQuest(FPClient client, FPData data, IDictionary<string, object> payload, CallbackDelegate callback, int timeout) {

            this.StartServiceThread();

            lock(service_locker) {

                if (this._serviceCache.Count < 3000) {

                    this._serviceCache.Add(() => {

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

                if (this._serviceCache.Count == 2998) {

                    ErrorRecorderHolder.recordError(new Exception("Quest Calls Limit!"));
                }

                this._serviceEvent.Set();
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
        }
    }
}