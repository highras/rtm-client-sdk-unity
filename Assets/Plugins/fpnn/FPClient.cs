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

            this._sock = new FPSocket((stream) => {

                self.OnData(stream);
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

        public void Close() {

            lock (self_locker) {

                if (this._isClose) {
                    
                    return;
                }

                this.SocketClose(null);
            }
        }

        public void Close(Exception ex) {

            lock (self_locker) {

                if (this._isClose) {
                    
                    return;
                }

                this.SocketClose(ex);
            }
        }

        private void SocketClose(Exception ex){

            this._isClose = true;

            if (this._eventDelegate != null) {

                FPManager.Instance.RemoveSecond(this._eventDelegate);
                this._eventDelegate = null;
            }

            this._psr.Destroy();
            this._sock.Close(ex);
        }

        private void Destroy() {

            this.Client_Connect = null;
            this.Client_Close = null;
            this.Client_Error = null;
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

        private void OnData(NetworkStream stream) {

            this.ReadHead(stream);
        }

        private void ReadHead(NetworkStream stream) {

            int rlen = 0;
            byte[] buffer = new byte[FPConfig.READ_BUFFER_LEN];

            this.ReadBytes(stream, null, buffer, rlen, BuildHead);
        }

        private void BuildHead(NetworkStream stream, FPData peek, byte[] buffer) {

            peek = this._cyr.PeekHead(buffer);

            if (peek == null) {

                this._sock.Close(new Exception("worng package head!"));
                return;
            }

            this.ReadBody(stream, peek);
        }

        private void ReadBody(NetworkStream stream, FPData peek) {

            int diff = peek.GetPkgLen() - peek.Bytes.Length;

            if (diff <= 0) {

                this.BuildBody(stream, peek, null);
                return;
            }

            int rlen = 0;
            byte[] buffer = new byte[diff];

            this.ReadBytes(stream, peek, buffer, rlen, BuildBody);
        }

        private void BuildBody(NetworkStream stream, FPData peek, byte[] buffer) {

            if (buffer != null) {

                List<byte> lb = new List<byte>(peek.Bytes);

                lb.AddRange(buffer);
                peek.Bytes = lb.ToArray();

                lb.Clear();
            }

            this.BuildData(stream, peek);
        }

        private void BuildData(NetworkStream stream, FPData peek) {

            peek.Bytes = this._cyr.DeCode(peek.Bytes);
            FPData data = this._cyr.PeekHead(peek);

            if (!this._pkg.DeCode(data)) {

                this._sock.Close(new Exception("worng package body!"));
                return;
            }

            if (this._pkg.IsAnswer(data)) {

                this.ExecCallback(data);
            }

            if (this._pkg.IsQuest(data)) {

                this.PushService(data);
            }

            this.ReadHead(stream);
        }

        private void ReadBytes(NetworkStream stream, FPData peek, byte[] buffer, int rlen, Action<NetworkStream, FPData, byte[]> calllback) {

            if (rlen < buffer.Length) {

                FPClient self = this;

                this._sock.ReadSocket(stream, buffer, rlen, (buf, len) => {

                    self.ReadBytes(stream, peek, buf, len, calllback);
                });

                return;
            } 

            if (calllback != null) {

                calllback(stream, peek, buffer);
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

            this._sock.OnSecond(timestamp);
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
