using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Collections;
using System.Reflection;

namespace com.fpnn
{
    public class FPClient
    {
        string Host { get; set; }
        int Port { get; set; }
        bool AutoReconnect { get; set; }
        int ConnectionTimeout { get; set; }
        CallbackDictionary CbContainer { get; set; }
        FPSocket Socket = null;
        bool IsConnected = false;
        bool IsClose = false;
        public ConnectedCallbackDelegate ConnectedCallback { get; set; } = null;
        public ClosedCallbackDelegate ClosedCallback { get; set; } = null;
        public ErrorCallbackDelegate ErrorCallback  { get; set; } = null;
        public FPProcessor Processor  { get; set; }


        public FPClient(string hostport) 
			: this(hostport.Split(':')[0], Int32.Parse(hostport.Split(':')[1]), true, 5000) 
        {
        }

        public FPClient(string hostport, bool autoReconnect)
			: this(hostport.Split(':')[0], Int32.Parse(hostport.Split(':')[1]), autoReconnect, 5000)
		{
		}

        public FPClient(string hostport, bool autoReconnect, int connectionTimeout)
            : this(hostport.Split(':')[0], Int32.Parse(hostport.Split(':')[1]), autoReconnect, connectionTimeout)
        {
        }

        public FPClient(string host, int port, bool autoReconnect = true, int connectionTimeout = 5000)
        {
            Host = host;
            Port = port;
            AutoReconnect = autoReconnect;
            ConnectionTimeout = connectionTimeout;
            Processor = new FPProcessor();
            Init();
        }

        public bool IsOpen()
        {
            return Socket.IsOpen();
        }

        public bool HasConnect()
        {
            return Socket.HasConnect();
        }

        private void Init()
        {
            CbContainer = new CallbackDictionary();
            InitSocket();
        }

        private void InitSocket()
        {
            Socket = new FPSocket(Host, Port, ConnectionTimeout);
            Socket.ClosedCallback = delegate()
            {
                if (ClosedCallback != null)
                {
                    try
                    {
                        ClosedCallback();
                    }
                    catch (Exception e)
                    {
                        ErrorRecorderHolder.recordError(e);
                    }
                }

                if (!IsClose && AutoReconnect)
                {
                    Socket = null;
                    Reconnect();
                    ErrorRecorderHolder.recordError("Reconnected");
                }
            };
            Socket.ConnectedCallback = delegate()
            {
                if (ConnectedCallback != null)
                {
                    try
                    {
                        ConnectedCallback();
                    }
                    catch (Exception e)
                    {
                        ErrorRecorderHolder.recordError(e);
                    }
                }                    
            };
            Socket.ErrorCallback = delegate (Exception e)
            {
                if (ErrorCallback != null)
                {
                    try
                    {
                        ErrorCallback(e);
                    }
                    catch (Exception ex)
                    {
                        ErrorRecorderHolder.recordError(ex);
                    }
                }
            };
            Socket.OnRead = delegate (FPData data)
            {
                if (data.IsAnswer())
                {
                    FPClientCallback cb = CbContainer.Get(data.GetSeqNum());
                    if (cb != null)
                    {
                        CallbackData cbd = new CallbackData(data);
                        if (cb.syncSignal == null) {
                            try
                            {
                                ThreadPool.QueueUserWorkItem( (state) =>
                                {
                                    cb.callback(cbd);
                                });
                            }
                            catch (Exception e)
                            {
                                ErrorRecorderHolder.recordError(e);
                            }
                        } else {
                            cb.syncCbd = cbd;
                            cb.syncSignal.Set();
                        }
                    }
                }
                else if (data.IsQuest()) 
                {
                    Socket.Send(FPData.QuestAnswerRaw(data.GetSeqNum(), data.flag));
                    Processor.FireEvent(data.method, data);
                }
            };
        }

        public void Reconnect()
        {
            lock(CbContainer)
            {
                CbContainer.FlushAllException(new Exception("Connection Broken"));
                if (Socket != null)
                {
                    Socket.Dispose();
                    Socket = null;
                }
                InitSocket();
                Socket.Open();
            }
            IsConnected = true;
        }

        public void Connect() 
        {
            Socket = new FPSocket(Host, Port, ConnectionTimeout);
            InitSocket();
            Socket.Open();
            IsConnected = true;
        }

        public void Close()
        {
            IsClose = true;
            lock(CbContainer)
            {
                CbContainer.FlushAllException(new Exception("Connection Broken"));
                if (Socket != null)
                {
                    Socket.Dispose();
                    Socket = null;
                }    
            }
        }
        
        public void SetQuestTimeout(int seconds)
		{
		    CbContainer.SetTimeout(seconds);
		}

        public void SendQuest(FPData data, FPCallback cb, int timeoutMilliSeconds = 0)
        {
            if (data.IsOneWay ()) {
				Send(data, null);
			} else {
				FPClientDelegate cbDelegate = new FPClientDelegate(cb);
				cbDelegate.timeoutMilliSeconds = timeoutMilliSeconds;
				Send(data, cbDelegate);
			}
        }

        public void SendQuest(FPData data, FPClientCallback cb = null, int timeoutMilliSeconds = 0)
		{
			cb.timeoutMilliSeconds = timeoutMilliSeconds;
			Send (data, cb);
		}

        public CallbackData SendQuest(FPData data, int timeoutMilliSeconds)
		{
			FPClientCallback cb = new FPClientCallback();
			cb.timeoutMilliSeconds = timeoutMilliSeconds;
			cb.syncSignal = new ManualResetEvent(false);
			cb.syncSignal.Reset();
			Send(data, cb);

			cb.syncSignal.WaitOne();

			if (cb.syncCbd != null)
                return cb.syncCbd;
            else
                throw new Exception("Send Quest Sync Error");
		}

        private UInt32 Send(FPData data, FPClientCallback cb = null)
        {
            if (!IsConnected)
                Connect();
            
            UInt32 seqNum = data.GenSeqNum();

            byte[] buffer = data.Raw();

            // TODO Encryptor

            if (cb != null)
				CbContainer.Put(seqNum, cb);

            Socket.Send(buffer);

            return seqNum;
        }
    }
}