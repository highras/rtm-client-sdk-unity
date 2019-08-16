using System;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;

using UnityEngine;

namespace com.fpnn {

    public class FPClient {

        public Action<EventData> Client_Connect;
        public Action<EventData> Client_Close;
        public Action<EventData> Client_Error;

        private int _seq = 0;
        private bool _isClose = false;

        private FPSocket _sock;
        private FPData _peekData = null;

        private int _readLen = 0;
        private byte[] _buffer = null;

        private EventDelegate _eventDelegate;

        private FPPackage _pkg;
        private FPEncryptor _cyr;
        private FPProcessor _psr;
        private FPCallback _callback;

        public FPClient(string endpoint, int connectionTimeout) {

            String[] ipport = endpoint.Split(':');
            this.Init(ipport[0], Convert.ToInt32(ipport[1]), connectionTimeout);
        }

        public FPClient(string host, int port, int connectionTimeout) {

            this.Init(host, port, connectionTimeout);
        }

        protected void Init(string host, int port, int connectionTimeout) {

            this._pkg = new FPPackage();
            this._cyr = new FPEncryptor(_pkg);
            this._psr = new FPProcessor();
            this._callback = new FPCallback();

            if (connectionTimeout <= 0) {

                connectionTimeout = 30 * 1000;
            }

            FPClient self = this;
            this._eventDelegate = (evd) => {

                self.OnSecond(evd.GetTimestamp());
            };

            FPManager.Instance.AddSecond(this._eventDelegate);

            this._sock = new FPSocket((stream, socket_locker) => {

                self.OnData(stream, socket_locker);
            }, host, port, connectionTimeout);

            this._sock.Socket_Connect = this.OnConnect;
            this._sock.Socket_Close = this.OnClose;
            this._sock.Socket_Error = this.OnError; 
        }

        public FPProcessor GetProcessor() {

            return this._psr;
        }

        public FPPackage GetPackage() {

            return this._pkg;
        }

        public FPSocket GetSock() {

            return this._sock;
        }

        private object self_locker = new object();

        public void Connect() {

            lock (self_locker) {

                if (this._isClose) {

                    return;
                }

                this._sock.Open();
            }
        }

        private void SocketClose(Exception e){

            FPClient self = this;

            ThreadPool.QueueUserWorkItem(new WaitCallback((state) => { 

                try {

                    self._sock.Close(e);
                } catch (ThreadAbortException tex) {
                } catch (Exception ex) {

                    ErrorRecorderHolder.recordError(ex);
                } 
            }));
        }

        public void Close() {

            lock (self_locker) {

                if (this._isClose) {

                    return;
                }

                this._isClose = true;
                this.SocketClose(null);
            } 
        }

        public void Close(Exception ex) {

            lock (self_locker) {

                if (this._isClose) {
                    
                    return;
                }

                this._isClose = true;
                this.SocketClose(ex);
            } 
        }

        private void Destroy() {

            lock (self_locker) {

                if (this._eventDelegate != null) {

                    FPManager.Instance.RemoveSecond(this._eventDelegate);
                    this._eventDelegate = null;
                }

                this._psr.Destroy();

                this.Client_Connect = null;
                this.Client_Close = null;
                this.Client_Error = null;
            }
        }

        public void SendQuest(FPData data) {

            this.SendQuest(data, null, 0);
        }

        public void SendQuest(FPData data, CallbackDelegate callback) {

            this.SendQuest(data, callback, 0);
        }

        public void SendQuest(FPData data, CallbackDelegate callback, int timeout) {

            if (data.GetSeq() == 0) {

                data.SetSeq(this.AddSeq());
            }

            byte[] buf = this._pkg.EnCode(data);
            buf = this._cyr.EnCode(buf);

            if (callback != null) {

                this._callback.AddCallback(this._pkg.GetKeyCallback(data), callback, timeout);
            }

            if (buf != null) {

                this._sock.Write(buf);
            }
        }

        public CallbackData SendQuest(FPData data, int timeout) {

            //TODO
            return null;
        }

        public void SendNotify(FPData data) {

            if (data.GetMtype() != 0x0) {

                data.SetMtype(0x0);
            }

            byte[] buf = this._pkg.EnCode(data);
            buf = this._cyr.EnCode(buf);

            if (buf != null) {

                this._sock.Write(buf);
            }
        }

        public bool IsIPv6() {

            return this._sock.IsIPv6();
        }

        public bool IsOpen() {

            return this._sock.IsOpen();
        }

        public bool HasConnect() {

            return this._sock.IsOpen() || this._sock.IsConnecting();
        }

        private void OnConnect(EventData evd) {

            try {

                if (this.Client_Connect != null) {

                    this.Client_Connect(evd);
                }
            } catch (Exception ex) {

                ErrorRecorderHolder.recordError(ex);
            }
        }

        private void OnClose(EventData evd) {

            lock (self_locker) {

                this._seq = 0;
                this._readLen = 0;
                this._peekData = null;

                if (this._buffer != null) {

                    Array.Clear(this._buffer, 0, this._buffer.Length);
                    this._buffer = null;
                }

                this._callback.RemoveCallback();
                this._cyr.Clear();
            }

            try {

                if (this.Client_Close != null) {

                    this.Client_Close(new EventData("close"));
                }
            } catch (Exception ex) {
                
                ErrorRecorderHolder.recordError(ex);
            }

            this.Destroy();
        }

        private void OnData(NetworkStream stream, FPSocket.SocketLocker socket_locker) {

            this.ReadHead(stream, socket_locker);
        }

        private void ReadHead(NetworkStream stream, FPSocket.SocketLocker socket_locker) {

            lock (self_locker) {

                if (this._buffer == null) {

                    this._readLen = 0;
                    this._buffer = new byte[FPConfig.READ_BUFFER_LEN];
                }

                if (this._readLen < this._buffer.Length) {

                    this.ReadSocket(stream, socket_locker, ReadHead);
                    return;
                }
            }

            this.BuildHead(stream, socket_locker);
        }

        private void BuildHead(NetworkStream stream, FPSocket.SocketLocker socket_locker) {

            lock (self_locker) {

                if (this._peekData == null) {

                    this._peekData = this._cyr.PeekHead(this._buffer);

                    Array.Clear(this._buffer, 0, this._buffer.Length);
                    this._buffer = null;
                }

                if (this._peekData == null) {

                    this._sock.Close(new Exception("worng package head!"));
                    return;
                }
            } 

            this.ReadBody(stream, socket_locker);
        }

        private void ReadBody(NetworkStream stream, FPSocket.SocketLocker socket_locker) {

            lock (self_locker) {

                if (this._buffer == null) {

                    int diff = this._peekData.GetPkgLen() - this._peekData.Bytes.Length;

                    if (diff > 0) {

                        this._readLen = 0;
                        this._buffer = new byte[diff];
                    }
                }

                if (this._readLen < this._buffer.Length) {

                    this.ReadSocket(stream, socket_locker, ReadBody);
                    return;
                }
            }

            this.BuildBody(stream, socket_locker);
        }

        private void BuildBody(NetworkStream stream, FPSocket.SocketLocker socket_locker) {

            lock (self_locker) {

                if (this._buffer != null) {

                    List<byte> lb = new List<byte>(this._peekData.Bytes);

                    lb.AddRange(this._buffer);

                    Array.Clear(this._buffer, 0, this._buffer.Length);
                    this._buffer = null;

                    this._peekData.Bytes = lb.ToArray();

                    lb.Clear();
                }
            }

            this.BuildData(stream, socket_locker);
        }

        private void BuildData(NetworkStream stream, FPSocket.SocketLocker socket_locker) {

            lock (self_locker) {

                this._peekData.Bytes = this._cyr.DeCode(this._peekData.Bytes);
                this._peekData = this._cyr.PeekHead(this._peekData);

                if (!this._pkg.DeCode(this._peekData)) {

                    this._sock.Close(new Exception("worng package body!"));
                    return;
                }

                if (this._pkg.IsAnswer(this._peekData)) {

                    this.ExecCallback(this._peekData);
                }

                if (this._pkg.IsQuest(this._peekData)) {

                    this.PushService(this._peekData);
                }

                this._peekData = null;
            }

            this.OnData(stream, socket_locker);
        }

        private void ReadSocket(NetworkStream stream, FPSocket.SocketLocker socket_locker, Action<NetworkStream, FPSocket.SocketLocker> calllback) {

            lock (socket_locker) {

                socket_locker.Count++;
            }

            try {

                FPClient self = this;

                lock (self_locker) {

                    int len = this._buffer.Length - this._readLen;

                    stream.BeginRead(this._buffer, this._readLen, len, (ar) => {

                        try {

                            int rlen = 0;

                            try {

                                rlen = stream.EndRead(ar);
                            } catch (Exception ex) {

                                self._sock.Close(ex);
                            }

                            lock (socket_locker) {

                                socket_locker.Count--;
                            }

                            if (rlen == 0) {

                                self._sock.Close(null);
                            } else {

                                lock (self_locker) {

                                    self._readLen += rlen;
                                }

                                if (calllback != null) {

                                    calllback(stream, socket_locker);
                                }
                            }
                        } catch (Exception ex) {

                            self._sock.Close(ex);
                        }
                    }, null);
                }
            } catch (Exception ex) {

                this._sock.Close(ex);
            }
        }

        private void OnError(EventData evd) {

            try {

                if (this.Client_Error != null) {

                    this.Client_Error(evd);
                }
            } catch (Exception ex) {
                
                ErrorRecorderHolder.recordError(ex);
            }
        }

        private void OnSecond(long timestamp) {

            this._psr.OnSecond(timestamp);
            this._callback.OnSecond(timestamp);
        }

        private void PushService(FPData quest) {

            lock (self_locker) {

                FPClient self = this;

                this._psr.Service(quest, (payload, exception) => {

                    FPData data = new FPData();

                    data.SetFlag(quest.GetFlag());
                    data.SetMtype(0x2);
                    data.SetSeq(quest.GetSeq());
                    data.SetSS(exception ? 1 : 0);

                    if (quest.GetFlag() == 0) {

                        data.SetPayload(Convert.ToString(payload));
                    }

                    if (quest.GetFlag() == 1) {

                        data.SetPayload((byte[])payload);
                    }

                    self.SendQuest(data);
                });
            }
        }

        private void ExecCallback(FPData answer) {

            lock (self_locker) {

                string key = this._pkg.GetKeyCallback(answer);

                if (!String.IsNullOrEmpty(key)) {

                    this._callback.ExecCallback(key, answer);
                }
            }
        }

        private object seq_locker = new object();

        private int AddSeq() {

            lock (seq_locker) {

                return ++this._seq;
            }
        }
    }
}
