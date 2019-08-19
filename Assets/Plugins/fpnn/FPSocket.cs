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

        public FPSocket(OnDataDelegate onData, string host, int port, int timeout) {

            this._host = host;
            this._port = port;
            this._timeout = timeout;
            this._onData = onData;
        }

        private ConnectingLocker conn_locker = new ConnectingLocker();

        public void Open() {

            if (String.IsNullOrEmpty(this._host)) {

                this.OnError(new Exception("Cannot open null host"));
                return;
            }
            
            if (this._port <= 0) {

                this.OnError(new Exception("Cannot open without port"));
                return;
            }

            lock (conn_locker) {

                if (conn_locker.Status == 1) {

                    return;
                }
            }

            lock (socket_locker) {

                if (this._socket != null) {

                    return;
                }

                socket_locker.Count = 0;
                socket_locker.Status = 0;
            }

            FPSocket self = this;

            ThreadPool.QueueUserWorkItem(new WaitCallback((state) => {

                lock (conn_locker) {

                    if (conn_locker.Status == 1) {

                        return;
                    }

                    conn_locker.Status = 1;
                }

                try {

                    var success = false;
                    IAsyncResult result = null;

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

                        result = self._socket.BeginConnect(ipaddr, self._port, null, null);
                        success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)self._timeout));
                    }

                    if (!success) {

                        lock (conn_locker) {

                            conn_locker.Status = 0;
                        }

                        self.Close(new Exception("Connect Timeout"));
                        return;
                    } 

                    lock (socket_locker) {

                        self._socket.EndConnect(result);
                        self._stream = self._socket.GetStream();
                    }

                    lock (conn_locker) {

                        conn_locker.Status = 0;
                    }

                    self.StartSendThread();
                    self.OnRead(self._stream, socket_locker);

                    self.OnConnect();
                } catch (Exception ex) {

                    lock (conn_locker) {

                        conn_locker.Status = 0;
                    }

                    self.Close(ex);
                } 
            }));
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

        public void Close(Exception ex) {

            bool firstClose = false;

            lock (socket_locker) {

                if (socket_locker.Status == 0) {

                    socket_locker.Status = 1;

                    if (ex != null) {

                        this.OnError(ex);
                    }

                    firstClose = true;
                }

                this.TryClose();
            }

            if (firstClose) {

                Thread.Sleep(200);

                lock (socket_locker) {

                    if (socket_locker.Status != 3) {

                        this.SocketClose();
                    }
                }
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

            lock(this._sendQueue) {

                this._sendEvent.Set();
            }

            if (this._stream != null) {

                this._stream.Close();
                this._stream = null;
            }

            if (this._socket != null) {

                this._socket.Close();
                this._socket = null;
            }

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

        public void Write(byte[] buffer) {

            lock(this._sendQueue) {

                for (int i = 0; i < buffer.Length; i++) {

                    this._sendQueue.Add(buffer[i]);
                }

                this._sendEvent.Set();
            }
        }

        private void Destroy() {

            this._onData = null;

            this.Socket_Connect = null;
            this.Socket_Close = null;
            this.Socket_Error = null;
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

        private void StartSendThread() {

            FPSocket self = this;

            ThreadPool.QueueUserWorkItem(new WaitCallback((state) => { 

                try {

                    self.OnWrite(self._stream);
                } catch (ThreadAbortException tex) {
                } catch (Exception ex) {

                    self.Close(ex);
                } 
            }));
        }

        private void OnWrite(NetworkStream stream) {

            this._sendEvent.WaitOne();

            byte[] buffer = new byte[0];

            lock(this._sendQueue) {

                buffer = this._sendQueue.ToArray();

                this._sendQueue.Clear();
                this._sendEvent.Reset();
            }

            this.WriteSocket(stream, buffer, OnWrite);
        }

        private void WriteSocket(NetworkStream stream, byte[] buffer, Action<NetworkStream> calllback) {

            lock (socket_locker) {

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