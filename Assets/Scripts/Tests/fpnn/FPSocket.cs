using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace com.fpnn {

    public delegate void OnDataDelegate(NetworkStream stream);

    public class FPSocket {

        private enum SocketStatus {
            Normal,
            SocketWillClosing,
            ScoketClosed
        }

        private enum ConnectingStatus
        {
            NoAction,
            WillConnect,
            Connecting
        }

        private class SocketLocker {
            public int Count = 0;
            public SocketStatus Status = SocketStatus.Normal;
        }

        private class ConnectingLocker {
            public ConnectingStatus Status = ConnectingStatus.NoAction;
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

        //-- For RUM sending bg event.
        private bool _delayCloseTriggered = false;
        private int _closeDelayForAppleMobileDeviceInBsckground = 0;
        public void SetCloseDelayForAppleMobileDeviceInBackground(int delayMilliseconds) { _closeDelayForAppleMobileDeviceInBsckground = delayMilliseconds; }

        private static volatile bool _appleMobileDeviceInBackground = false;
        public static void AppleMobileDeviceSwitchToBackground(bool background) { _appleMobileDeviceInBackground = background; }

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
            if (string.IsNullOrEmpty(this._host)) {
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
                socket_locker.Status = SocketStatus.Normal;
            }

            lock (conn_locker) {
                if (conn_locker.Status != ConnectingStatus.NoAction) {
                    return;
                }

                conn_locker.Status = ConnectingStatus.WillConnect;
                conn_locker.timestamp = FPManager.Instance.GetMilliTimestamp();
            }

            FPManager.Instance.AsyncTask(AsyncConnect, null);
        }

        private void AsyncConnect(object state) {
            lock (conn_locker) {
                if (conn_locker.Status != ConnectingStatus.WillConnect) {
                    return;
                }

                conn_locker.Status = ConnectingStatus.Connecting;
            }

            try {
                lock (socket_locker) {
                    if (this._socket != null) {
                        return;
                    }

                    IPHostEntry hostEntry = Dns.GetHostEntry(this._host);
                    IPAddress ipaddr = hostEntry.AddressList[0];

                    if (ipaddr.AddressFamily != AddressFamily.InterNetworkV6) {
                        this._socket = new TcpClient(AddressFamily.InterNetwork);
                    } else {
                        this._isIPv6 = true;
                        this._socket = new TcpClient(AddressFamily.InterNetworkV6);
                    }

                    this._socket.BeginConnect(ipaddr, this._port, new AsyncCallback(ConnectCallback), null);
                }
            } catch (Exception ex) {
                lock (conn_locker) {
                    conn_locker.Status = ConnectingStatus.NoAction;
                }

                this.Close(ex);
            }
        }

        private void ConnectCallback(IAsyncResult ar) {
            try {
                bool isClose = false;

                lock (socket_locker) {
                    this._socket.EndConnect(ar);
                    this._stream = this._socket.GetStream();
                    isClose = (socket_locker.Status != SocketStatus.Normal);
                }

                lock (conn_locker) {
                    conn_locker.Status = ConnectingStatus.NoAction;
                }

                if (isClose) {
                    this.DelayClose(null);
                    return;
                }

                this.OnConnect();
                this.OnRead(this._stream);
                this.OnWrite(this._stream);
            } catch (Exception ex) {
                lock (conn_locker) {
                    conn_locker.Status = ConnectingStatus.NoAction;
                }

                this.Close(ex);
            }
        }

        public bool IsIPv6() {
            lock (socket_locker) {
                return this._isIPv6;
            }
        }

        public bool IsConnected() {
            lock (socket_locker) {
                if (this._socket != null) {
                    return this._socket.Connected;
                }

                return false;
            }
        }

        public bool IsConnecting() {
            lock (conn_locker) {
                return conn_locker.Status != ConnectingStatus.NoAction;
            }
        }

        public void OnSecond(long timestamp) {
            if (this._timeout <= 0) {
                return;
            }

            bool timeout = false;
            lock (conn_locker) {
                if (conn_locker.Status != ConnectingStatus.NoAction) {
                    if (timestamp - conn_locker.timestamp >= this._timeout) {
                        timeout = true;
                        conn_locker.Status = ConnectingStatus.NoAction;
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
                    if (socket_locker.Status == SocketStatus.Normal) {
                        firstClose = true;
                        socket_locker.Status = SocketStatus.SocketWillClosing;

                        if (ex != null) {
                            this.OnError(ex);
                        }

                        try {
                            this._sendEvent.Set();
                        } catch (Exception e) {
                            ErrorRecorderHolder.recordError(e);
                        }

                        if (this.IsConnecting()) {
                            return;
                        }
                    }
                    this.TryClose();
                }

                if (firstClose) {
                    FPManager.Instance.DelayTask(200, DelayClose, null);
                }
            } catch (Exception e) {
                ErrorRecorderHolder.recordError(e);
            }
        }

        private void BackgroundDelayClose(object _)
        {
            if (!_appleMobileDeviceInBackground)
            {
                lock (socket_locker) { _delayCloseTriggered = false; }
                return;
            }

            Close(null);
        }

        private void DelayClose(object state) {
            lock (socket_locker) {
                if (socket_locker.Count > 0) {
                    FPManager.Instance.DelayTask(80, DelayClose, null);
                    return;
                }

                if (socket_locker.Status != SocketStatus.ScoketClosed) {
                    this.SocketClose();
                }
            }
        }

        private void TryClose() {
            if (socket_locker.Status == SocketStatus.ScoketClosed) {
                return;
            }

            if (socket_locker.Count != 0) {
                return;
            }

            try {
                this.SocketClose();
            } catch (Exception ex) {
                ErrorRecorderHolder.recordError(ex);
            }
        }

        private void SocketClose() {
            if (this._stream != null) {
                this._stream.Close();
                this._stream = null;
            }

            if (this._socket != null) {
                this._socket.Close();
                this._socket = null;
            }

            try {
                this._sendEvent.Close();
            } catch (Exception ex) {
                ErrorRecorderHolder.recordError(ex);
            }

            socket_locker.Status = SocketStatus.ScoketClosed;
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

        private Object self_locker = new Object();

        public void Write(byte[] buffer) {
            if (buffer == null || buffer.Length <= 0) {
                return;
            }

            lock (socket_locker) {
                if (socket_locker.Status != SocketStatus.Normal) {
                    return;
                }
            }

            lock (self_locker) {
                for (int i = 0; i < buffer.Length; i++) {
                    this._sendQueue.Add(buffer[i]);
                }
            }

            try {
                this._sendEvent.Set();
            } catch (Exception ex) {
                ErrorRecorderHolder.recordError(ex);
            }
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

        private void OnRead(NetworkStream stream) {
            if (this._onData != null) {
                this._onData(stream);
            }
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
            try {
                if (!this._sendEvent.SafeWaitHandle.IsClosed) {
                    this._sendEvent.WaitOne();
                }
            } catch (Exception ex) {
                ErrorRecorderHolder.recordError(ex);
            }

            byte[] buffer = new byte[0];

            lock (self_locker) {
                buffer = this._sendQueue.ToArray();
                this._sendQueue.Clear();
            }

            this.WriteSocket(stream, buffer, OnWrite);
        }

        private void WriteSocket(NetworkStream stream, byte[] buffer, Action<NetworkStream> calllback) {
            bool needTriggerDelayTask = false;
            lock (socket_locker) {
                if (this._socket == null) {
                    return;
                }

                if (!this._socket.Connected) {
                    this.Close(null);
                    return;
                }

                if (_appleMobileDeviceInBackground)
                {
                    if (_closeDelayForAppleMobileDeviceInBsckground == 0)
                    {
                        this.Close(null);
                        return;
                    }

                    if (!_delayCloseTriggered)
                    {
                        needTriggerDelayTask = true;
                        _delayCloseTriggered = true;
                    }
                }

                    if (socket_locker.Status == SocketStatus.Normal) {
                    try {
                        this._sendEvent.Reset();
                    } catch (Exception ex) {
                        ErrorRecorderHolder.recordError(ex);
                    }
                }
                socket_locker.Count++;
            }

            if (needTriggerDelayTask)
                FPManager.Instance.DelayTask(_closeDelayForAppleMobileDeviceInBsckground, BackgroundDelayClose, null);

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

        public void ReadSocket(NetworkStream stream, byte[] buffer, int rlen, Action<byte[], int> calllback) {
            bool needTriggerDelayTask = false;

            lock (socket_locker) {
                if (this._socket == null) {
                    return;
                }

                if (!this._socket.Connected) {
                    this.Close(null);
                    return;
                }

                if (_appleMobileDeviceInBackground)
                {
                    if (_closeDelayForAppleMobileDeviceInBsckground == 0)
                    {
                        this.Close(null);
                        return;
                    }

                    if (!_delayCloseTriggered)
                    {
                        needTriggerDelayTask = true;
                        _delayCloseTriggered = true;
                    }
                }

                socket_locker.Count++;
            }

            if (needTriggerDelayTask)
                FPManager.Instance.DelayTask(_closeDelayForAppleMobileDeviceInBsckground, BackgroundDelayClose, null);

            try {
                FPSocket self = this;
                stream.BeginRead(buffer, rlen, buffer.Length - rlen, (ar) => {
                    try {
                        int len = 0;

                        try {
                            len = stream.EndRead(ar);
                        } catch (Exception ex) {
                            self.Close(ex);
                        }

                        lock (socket_locker) {
                            socket_locker.Count--;
                        }

                        if (len == 0) {
                            self.Close(null);
                        } else {
                            rlen += len;

                            if (calllback != null) {
                                calllback(buffer, rlen);
                            }
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
