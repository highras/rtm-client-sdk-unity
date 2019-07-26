using System;
using System.Collections.Generic;
using System.Net.Sockets;

using UnityEngine;

namespace com.fpnn {

    public class FPClient {

        private int _seq = 0;
        private int _timeout = 30 * 1000;
        private bool _isClose;
        private bool _autoReconnect;

        private FPSocket _sock;
        private FPData _peekData = null;

        private int _readBytes = 0;
        private byte[] _buffer = null;

        private EventDelegate _eventDelegate;

        private FPPackage _pkg;
        private FPEncryptor _cyr;
        private FPEvent _event;
        private FPProcessor _psr;
        private FPCallback _callback;

        private object _lock_obj = new object();

        public FPClient(string endpoint, bool autoReconnect, int connectionTimeout) {

            String[] ipport = endpoint.Split(':');
            this.Init(ipport[0], Convert.ToInt32(ipport[1]), autoReconnect, connectionTimeout);
        }

        public FPClient(string host, int port, bool autoReconnect, int connectionTimeout) {

            this.Init(host, port, autoReconnect, connectionTimeout);
        }

        protected void Init(string host, int port, bool autoReconnect, int connectionTimeout) {

            this._pkg = new FPPackage();
            this._cyr = new FPEncryptor(_pkg);
            this._event = new FPEvent();
            this._psr = new FPProcessor();
            this._callback = new FPCallback();

            if (connectionTimeout > 0) {

                this._timeout = connectionTimeout;
            }

            this._autoReconnect = autoReconnect;

            FPClient self = this;

            this._eventDelegate = (evd) => {

                self.OnSecond(evd.GetTimestamp());
            };

            ThreadPool.Instance.Event.AddListener("second", this._eventDelegate);

            this._sock = new FPSocket((stream) => {

                self.OnData(stream);
            }, host, port, this._timeout);

            this._sock.GetEvent().AddListener("connect", (evd) => {

                self.OnConnect();
            });

            this._sock.GetEvent().AddListener("close", (evd) => {

                self.OnClose();
            });

            this._sock.GetEvent().AddListener("error", (evd) => {

                self.OnError(evd.GetException());
            });
        }

        public FPEvent GetEvent() {

            return this._event;
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

        public void Connect() {

            this._isClose = false;
            this._sock.Open();
        }

        public void Close() {

            this._isClose = true;
            this._sock.Close(null);
        }

        public void Close(Exception ex) {

            this._isClose = true;
            this._sock.Close(ex);
        }

        public void Destroy() {

            if (this._eventDelegate != null) {

                ThreadPool.Instance.Event.RemoveListener("second", this._eventDelegate);
                this._eventDelegate = null;
            }

            this._autoReconnect = false;

            this._event.RemoveListener();

            this._psr.Destroy();
            this._sock.Destroy();

            this.OnClose();
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

        private void OnConnect() {

            this._event.FireEvent(new EventData("connect"));
        }

        private void OnClose() {

            this._seq = 0;
            this._readBytes = 0;
            this._peekData = null;

            if (this._buffer != null) {

                Array.Clear(this._buffer, 0, this._buffer.Length);
                this._buffer = null;
            }

            this._callback.RemoveCallback();
            this._cyr.Clear();

            this._event.FireEvent(new EventData("close", !this._isClose && this._autoReconnect));

            if (this._autoReconnect) {

                this.Reconnect();
            }
        }

        private void Reconnect() {

            if (this._isClose) {

                return;
            }

            if (this.HasConnect()) {

                return;
            }

            this.Connect();
        }

        private void OnData(NetworkStream stream) {

            this.ReadHead(stream);
        }

        private void ReadHead(NetworkStream stream) {

            if (this._buffer == null) {

                this._readBytes = 0;
                this._buffer = new byte[FPConfig.READ_BUFFER_LEN];
            }

            if (this._readBytes < this._buffer.Length) {

                this.ReadSocket(stream, ReadHead);
                return;
            }

            this.BuildHead(stream);
        }

        private void BuildHead(NetworkStream stream) {

            if (this._peekData == null) {

                this._peekData = this._cyr.PeekHead(this._buffer);

                Array.Clear(this._buffer, 0, this._buffer.Length);
                this._buffer = null;
            }

            if (this._peekData == null) {

                this._sock.Close(new Exception("worng package head!"));
                return;
            }

            this.ReadBody(stream);
        }

        private void ReadBody(NetworkStream stream) {

            if (this._buffer == null) {

                int diff = this._peekData.GetPkgLen() - this._peekData.Bytes.Length;

                if (diff > 0) {

                    this._readBytes = 0;
                    this._buffer = new byte[diff];
                }
            }

            if (this._readBytes < this._buffer.Length) {

                this.ReadSocket(stream, ReadBody);
                return;
            }

            this.BuildBody(stream);
        }

        private void BuildBody(NetworkStream stream) {

            if (this._buffer != null) {

                List<byte> lb = new List<byte>(this._peekData.Bytes);

                lb.AddRange(this._buffer);

                Array.Clear(this._buffer, 0, this._buffer.Length);
                this._buffer = null;

                this._peekData.Bytes = lb.ToArray();

                lb.Clear();
            }

            this.BuildData(stream);
        }

        private void BuildData(NetworkStream stream) {

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
            this.OnData(stream);
        }

        private void ReadSocket(NetworkStream stream, Action<NetworkStream> calllback) {

            FPClient self = this;

            try {

                int len = this._buffer.Length - this._readBytes;

                stream.BeginRead(this._buffer, this._readBytes, len, (ar) => {

                    try {

                        int readBytes = stream.EndRead(ar);

                        if (readBytes == 0) {

                            self._sock.Close(null);
                        }else {

                            self._readBytes += readBytes;

                            if (calllback != null) {

                                calllback(stream);
                            }
                        }

                    } catch (Exception ex) {

                        self._sock.Close(ex);
                    }
                }, null);
            } catch (Exception ex) {

                self._sock.Close(ex);
            }
        }

        private void OnError(Exception ex) {

            this._event.FireEvent(new EventData("error", ex));
        }

        private void OnSecond(long timestamp) {

            this._psr.OnSecond(timestamp);
            this._callback.OnSecond(timestamp);
        }

        private void PushService(FPData quest) {

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

        private void ExecCallback(FPData answer) {

            string key = this._pkg.GetKeyCallback(answer);

            if (!String.IsNullOrEmpty(key)) {

                this._callback.ExecCallback(key, answer);
            }
        }

        private int AddSeq() {

            lock(this._lock_obj) {

                this._seq++;
            }

            return this._seq;
        }
    }
}
