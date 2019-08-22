using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace com.fpnn {

    public delegate void OnDataDelegate(NetworkStream stream, FPSocket.SocketLocker socket_locker);

    public class FPSocket {

        public class SocketLocker {

            public int Count = 0;
            public int Status = 0;
        }

        private class ConnectingLocker {

            public int Status = 0;
            public long timestamp = 0;
        }

        public Action<EventData> Socket_Connect;
        public Action<EventData> Socket_Close;
        public Action<EventData> Socket_Error;

        private int _port;
        private string _host;
        private int _timeout;
        private OnDataDelegate _onData;

        private TcpClient _socket;
        private NetworkStream _stream;

        private bool _isIPv6 = false;

        private List<byte> _sendQueue = new List<byte>();
        private ManualResetEvent _sendEvent = new ManualResetEvent(false);

        private SocketLocker socket_locker = new SocketLocker();
        private ConnectingLocker conn_locker = new ConnectingLocker();

        public FPSocket(OnDataDelegate onData, string host, int port, int timeout) {

            this._host = host;
            this._port = port;
            this._timeout = timeout;
            this._onData = onData;
        }

        public void Open() {

            if (String.IsNullOrEmpty(this._host)) {

                this.OnError(new Exception("Cannot open null host"));
                return;
            }
            
            if (this._port <= 0) {

                this.OnError(new Exception("Cannot open without port"));
                return;
            }

            lock (socket_locker) {

                if (this._socket != null) {

                    return;
                }

                socket_locker.Count = 0;
                socket_locker.Status = 0;
            }

            lock (conn_locker) {

                if (conn_locker.Status != 0) {

                    return;
                }
            }

            FPSocket self = this;

            FPManager.Instance.AsyncTask(() => {

                lock (conn_locker) {

                    if (conn_locker.Status != 0) {

                        return;
                    }

                    conn_locker.Status = 1;
                    conn_locker.timestamp = FPManager.Instance.GetMilliTimestamp();
                }

                try {

                    lock (socket_locker) {

                        if (self._socket != null) {

                            return;
                        }

                        IPHostEntry hostEntry = Dns.GetHostEntry(self._host);
                        IPAddress ipaddr = hostEntry.AddressList[0];

                        if (ipaddr.AddressFamily != AddressFamily.InterNetworkV6) {

                            self._socket = new TcpClient(AddressFamily.InterNetwork);
                        } else {

                            self._isIPv6 = true;
                            self._socket = new TcpClient(AddressFamily.InterNetworkV6);
                        }

                        self._socket.BeginConnect(ipaddr, self._port, new AsyncCallback(ConnectCallback), null);
                    }
                } catch (Exception ex) {

                    lock (conn_locker) {

                        conn_locker.Status = 0;
                    }

                    self.Close(ex);
                } 
            });
        }

        private void ConnectCallback(IAsyncResult ar) {

            try {

                bool isClose = false;

                lock (socket_locker) {

                    this._socket.EndConnect(ar);
                    this._stream = this._socket.GetStream();

                    isClose = socket_locker.Status != 0;
                }

                lock (conn_locker) {

                    conn_locker.Status = 0;
                }

                if (isClose) {

                    return;
                }

                this.OnConnect();

                this.OnRead(this._stream, socket_locker);
                this.OnWrite(this._stream);
            } catch (Exception ex) {

                lock (conn_locker) {

                    conn_locker.Status = 0;
                }

                this.Close(ex);
            }
        }

        public bool IsIPv6() {

            lock (socket_locker) {

                return this._isIPv6;
            }
        }

        public bool IsOpen() {

            lock (socket_locker) {

                if (this._socket != null) {

                    return this._socket.Connected;
                }

                return false;
            }
        }

        public bool IsConnecting() {

            lock (conn_locker) {

                return conn_locker.Status == 1;
            }
        }

        public void OnSecond(long timestamp) {

            bool timeout = false;

            lock (conn_locker) {

                if (conn_locker.Status == 1) {

                    if (timestamp - conn_locker.timestamp >= this._timeout) {

                        timeout = true;
                        conn_locker.Status = 0;
                    } 
                }
            }

            if (timeout) {

                this.Close(new Exception("Connect Timeout"));
            }
        }

        public void Close(Exception ex) {

            try {

                bool firstClose = false;

                lock (socket_locker) {

                    if (socket_locker.Status == 0) {

                        firstClose = true;
                        socket_locker.Status = 1;

                        if (ex != null) {

                            this.OnError(ex);
                        }

                        this._sendEvent.Set();
                    }

                    this.TryClose();
                }

                if (firstClose) {

                    FPSocket self = this;

                    FPManager.Instance.DelayTask(200, () => {

                        lock (socket_locker) {

                            if (socket_locker.Status != 3) {

                                self.SocketClose();
                            }
                        }
                    });
                }
            } catch (Exception e) {

                ErrorRecorderHolder.recordError(e);
            }
        }

        private void TryClose() {

            if (socket_locker.Status == 3) {

                return;
            }

            if (socket_locker.Count != 0) {

                return;
            }

            this.SocketClose();
        }

        private void SocketClose() {

            socket_locker.Status = 3;

            if (this._stream != null) {

                this._stream.Close();
                this._stream = null;
            }

            if (this._socket != null) {

                this._socket.Close();
                this._socket = null;
            }

            this._sendEvent.Close();
            this.OnClose();
        }

        private void OnClose() {

            try {

                if (this.Socket_Close != null) {

                    this.Socket_Close(new EventData("close"));
                }
            } catch (Exception ex) {
                
                ErrorRecorderHolder.recordError(ex);
            }

            this.Destroy();
        }

        private void Destroy() {

            this._onData = null;

            this.Socket_Connect = null;
            this.Socket_Close = null;
            this.Socket_Error = null;
        }

        public void Write(byte[] buffer) {

            lock (this._sendQueue) {

                for (int i = 0; i < buffer.Length; i++) {

                    this._sendQueue.Add(buffer[i]);
                }
            }

            this._sendEvent.Set();
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

            try {

                if (this.Socket_Connect != null) {

                    this.Socket_Connect(new EventData("connect"));
                }
            } catch (Exception ex) {

                ErrorRecorderHolder.recordError(ex);
            }
        }

        private void OnRead(NetworkStream stream, SocketLocker socket_locker) {

            this._onData(stream, socket_locker);
        }

        private void OnError(Exception ex) {

            try {

                if (this.Socket_Error != null) {

                    this.Socket_Error(new EventData("error", ex));
                }
            } catch (Exception e) {
                
                ErrorRecorderHolder.recordError(e);
            }
        }

        private void OnWrite(NetworkStream stream) {

            this._sendEvent.WaitOne();

            byte[] buffer = new byte[0];

            lock (this._sendQueue) {

                buffer = this._sendQueue.ToArray();
                this._sendQueue.Clear();
            }

            this.WriteSocket(stream, buffer, OnWrite);
        }

        private void WriteSocket(NetworkStream stream, byte[] buffer, Action<NetworkStream> calllback) {

            lock (socket_locker) {

                if (socket_locker.Status == 0) {

                    this._sendEvent.Reset();
                }

                socket_locker.Count++;
            }

            try {

                FPSocket self = this;

                stream.BeginWrite(buffer, 0, buffer.Length, (ar) => {

                    try {

                        try {

                            stream.EndWrite(ar);
                        } catch (Exception ex) {

                            self.Close(ex);
                        }

                        lock (socket_locker) {

                            socket_locker.Count--;
                        }

                        if (calllback != null) {

                            calllback(stream);
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