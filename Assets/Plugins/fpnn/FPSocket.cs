using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace com.fpnn
{

    public delegate void ConnectedCallbackDelegate();
    public delegate void ClosedCallbackDelegate();
    public delegate void ErrorCallbackDelegate(Exception e);
    public delegate void OnReadDelegate(FPData data);

    public class FPSocket
    {
        TcpClient Client { get; set; }  = null;
        NetworkStream Stream { get; set; }
        string Host { get; set; }
        int Port { get; set; }
        int Timeout { get; set; }
        public ConnectedCallbackDelegate ConnectedCallback { get; set; } = null;
        public ClosedCallbackDelegate ClosedCallback { get; set; } = null;
        public ErrorCallbackDelegate ErrorCallback  { get; set; } = null;
        public OnReadDelegate OnRead { get; set; } = null;

        private Thread WriteThread = null;
        private bool IsDisposed = false;
        private ArrayList SendQueue = new ArrayList();
        private ManualResetEvent SendEvent = new ManualResetEvent(false);
        private FPData ReadData;
        private bool IsClosedCallbackRun = false;
        private bool IsConnecting = false;
        private object ConnectingLock = new object();

        public FPSocket(string host, int port, int timeout, bool isEncryptor = false)
        {
            Host = host;
            Port = port;
            Timeout = timeout;
            ReadData = new FPData(isEncryptor);
            Client = new TcpClient();
        }

        public void Open()
        {
            if (String.IsNullOrEmpty(Host)) {
                if (ErrorCallback != null)
				    ErrorCallback(new Exception("Cannot open null host"));
                return;
			}
			
			if (Port <= 0) {
                if (ErrorCallback != null)
				    ErrorCallback(new Exception("Cannot open without port"));
                return;
			}

            ThreadPool.QueueUserWorkItem(new WaitCallback(o => {
                IAsyncResult result;
                try
                {
                    result = Client.BeginConnect(Host, Port, null, null);
                    lock(ConnectingLock)
                    {
                        IsConnecting = true;
                    }
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)this.Timeout));
                    if (!success)
                    {
                        lock(ConnectingLock)
                        {
                            IsConnecting = false;
                        }
                        Client.Close();
                        if (ErrorCallback != null)
                            ErrorCallback(new Exception("Connect Timeout"));
                        return;
                    }
                    Client.EndConnect(result);
                    Stream = Client.GetStream();

                    lock(ConnectingLock)
                    {
                        IsConnecting = false;
                    }

                    StartWriteThread(); 
                    ListenForRead();

                    if (ConnectedCallback != null)
                        ConnectedCallback();                    
                }
                catch (Exception e)
                {
                    lock(ConnectingLock)
                    {
                        IsConnecting = false;
                    }
                    if (Client != null)
                        Client.Close();
                    if (ErrorCallback != null)
                        ErrorCallback(e);
                }
            }));
        }

        public bool IsOpen()
        {
            return Client.Connected;
        }

        public bool HasConnect()
        {
            return IsOpen() || IsConnecting;
        }

        public void Send(byte[] buffer)
        {
            lock(SendQueue)
            {
                SendQueue.Add(buffer);
                SendEvent.Set();
            }
        }

        private void ListenForRead()
        {
            try
            {
                byte[] buffer = new byte[ReadData.nextLength];
                Stream.BeginRead(buffer, 0, (int)ReadData.nextLength, (ar) =>
                {
                    try
                    {
                        int readBytes = Stream.EndRead(ar);

                        if (readBytes == 0)
                        {
                            Dispose();
                            RunClosedCallback();
                        }
                        else
                        {
                            if (ReadData.Decode(buffer))
                            {
                                if (OnRead != null)
                                {
                                    FPData dataClone = (FPData)ReadData.Clone();
                                    OnRead(dataClone);
                                }
                                    
                                ReadData.Reset();
                            }
                            ListenForRead();
                        }
                    }
                    catch (Exception)
                    {
                        Dispose();
                        RunClosedCallback();
                    }
                }, null);
            } 
            catch (Exception)
            {
                Dispose();
                RunClosedCallback();
            }
        }

        private void StartWriteThread()
        {
            WriteThread = new Thread(new ThreadStart(ListenForWrite));
            WriteThread.Start();
        }

        private void RunClosedCallback()
        {
            lock(ClosedCallback)
            {
                if (!IsClosedCallbackRun)
                {
                    IsClosedCallbackRun = true;
                    if (ClosedCallback != null)
                        ClosedCallback();
                }
            }
        }

        private void ListenForWrite()
        {
            SendEvent.WaitOne();
            
            if (IsDisposed)
                return;

            ArrayList tmpQueue = new ArrayList();
            lock(SendQueue)
            {
                for (int i = 0; i < SendQueue.Count; i++)
                    for (int j = 0; j < ((byte[])SendQueue[i]).Length; j++)
                        tmpQueue.Add(((byte[])SendQueue[i])[j]);
                SendQueue.Clear();
                SendEvent.Reset();
            }

            byte[] buffer = FPCommon.GetBytes(tmpQueue);

            try
            {
                if (buffer != null && buffer.Length > 0)
                {
                    Stream.BeginWrite(buffer, 0, buffer.Length, (ar) =>
                    {
                        try
                        {
                            Stream.EndWrite(ar);
                            ListenForWrite();
                        }
                        catch (Exception e)
                        {
                            if (ErrorCallback != null)
                                ErrorCallback(new Exception("send error(01): " + e.Message));

                            Dispose();
                            RunClosedCallback();
                        }
                    }, null);
                }
                else
                {
                    ListenForWrite();
                }
            } 
            catch (Exception e)
            {
                if (ErrorCallback != null)
                    ErrorCallback(new Exception("send error(02): " + e.Message));
                Dispose();
                RunClosedCallback();
            }
        }

        public void Dispose()
        {
            IsDisposed = true;
            lock(SendQueue)
            {
                SendEvent.Set();
            }
            if (Client != null)
            {
                Client.Close();
                Client = null;
            }
                
            if (WriteThread != null)
            {
                WriteThread.Join(); 
                WriteThread = null;
            }
        }
    }
}