using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace com.fpnn {

    public delegate void OnDataDelegate(NetworkStream stream);

    public class FPSocket {

        private int _port;
        private string _host;
        private int _timeout;
        private OnDataDelegate _onData;

        private FPEvent _event;
        private TcpClient _socket;
        private NetworkStream _stream;

        private bool _isIPv6 = false;
        private bool _isClosed = true;
        private bool _isConnecting = false;
        private List<byte> _sendQueue = new List<byte>();
        private ManualResetEvent _sendEvent = new ManualResetEvent(false);
        private object _lock_obj = new object();

        public FPSocket(OnDataDelegate onData, string host, int port, int timeout) {

            this._host = host;
            this._port = port;
            this._timeout = timeout;
            this._onData = onData;

            this._event = new FPEvent();
        }

        public void Open() {

            if (String.IsNullOrEmpty(this._host)) {

                this.Close(new Exception("Cannot open null host"));
                return;
            }
            
            if (this._port <= 0) {

                this.OnError(new Exception("Cannot open without port"));
                return;
            }

            if (this._socket != null && (this.IsOpen() || this.IsConnecting())) {

                this.OnError(new Exception("has been connect!"));
                return;
            }

            this._isClosed = false;

            FPSocket self = this;

            com.fpnn.ThreadPool.Instance.Execute((state) => {

                try {

                    IPHostEntry hostEntry = Dns.GetHostEntry(self._host);
                    IPAddress ipaddr = hostEntry.AddressList[0];

                    if (ipaddr.AddressFamily != AddressFamily.InterNetworkV6) {

                        self._socket = new TcpClient(AddressFamily.InterNetwork);
                    } else {

                        self._isIPv6 = true;
                        self._socket = new TcpClient(AddressFamily.InterNetworkV6);
                    }

                    IAsyncResult result = self._socket.BeginConnect(ipaddr, self._port, null, null);

                    lock(self._lock_obj) {

                        self._isConnecting = true;
                    }

                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)self._timeout));

                    if (!success) {

                        lock(self._lock_obj) {

                            self._isConnecting = false;
                        }

                        self.Close(new Exception("Connect Timeout"));
                        return;
                    }

                    self._socket.EndConnect(result);
                    self._stream = self._socket.GetStream();

                    lock(self._lock_obj) {

                        self._isConnecting = false;
                    }

                    self.StartSendThread();
                    self.OnRead(self._stream);

                    self.OnConnect();
                } catch (Exception ex) {

                    lock(self._lock_obj) {

                        self._isConnecting = false;
                    }

                    self.Close(ex);
                }
            });
        }

        public bool IsIPv6() {

            return this._isIPv6;
        }

        public bool IsOpen() {

            if (this._socket != null) {

                return this._socket.Connected;
            }

            return false;
        }

        public bool IsConnecting() {

            return this._isConnecting;
        }

        public void Close(Exception ex) {

            if (!this._isClosed) {

                lock(this._sendQueue) {

                    this._sendEvent.Set();
                }

                if (this._socket != null) {

                    this._socket.Close(); 
                    this._socket = null;
                }

                this.OnClose(ex);
            }
        }

        public void Write(byte[] buffer) {

            lock(this._sendQueue) {

                for (int i = 0; i < buffer.Length; i++) {

                    this._sendQueue.Add(buffer[i]);
                }

                this._sendEvent.Set();
            }
        }

        public void Destroy() {

            this._isIPv6 = false;
            
            this._onData = null;
            this._event.RemoveListener();

            this.Close(null);
        }

        public FPEvent GetEvent() {

            return this._event;
        }

        public string GetHost() {

            return this._host;
        }

        public int GetPort() {

            return this._port;
        }

        public int GetTimeout() {

            return this._timeout;
        }

        private void OnConnect() {

            this._event.FireEvent(new EventData("connect"));
        }

        private void OnClose(Exception ex) {

            if (!this._isClosed) {

                this._isClosed = true;

                if (ex != null) {

                    this.OnError(ex);
                }

                this._event.FireEvent(new EventData("close"));
            }
        }

        private void OnRead(NetworkStream stream) {

            this._onData(stream);
        }

        private void OnError(Exception ex) {

            this._event.FireEvent(new EventData("error", ex));
        }

        private void StartSendThread() {

            FPSocket self = this;

            com.fpnn.ThreadPool.Instance.Execute((state) => { 

                self.OnWrite();
            });
        }

        private void OnWrite() {

            this._sendEvent.WaitOne();

            if (this._isClosed) {

                return;
            }

            byte[] buffer = new byte[0];

            lock(this._sendQueue) {

                buffer = this._sendQueue.ToArray();

                this._sendQueue.Clear();
                this._sendEvent.Reset();
            }

            this.WriteSocket(buffer, OnWrite);
        }

        private void WriteSocket(byte[] buffer, Action calllback) {

            FPSocket self = this;

            try {

                this._stream.BeginWrite(buffer, 0, buffer.Length, (ar) => {

                    try {

                        self._stream.EndWrite(ar);

                        if (calllback != null) {

                            calllback();
                        }
                    } catch (Exception ex) {

                        self.Close(ex);
                    }
                }, null);
            } catch (Exception ex) {

                this.Close(ex);
            }
        }
    }
}