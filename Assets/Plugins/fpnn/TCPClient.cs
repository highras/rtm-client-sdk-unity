using System;
using System.Net;
using System.Threading;
using com.fpnn.proto;

/*
 * TCPConnection 需要在连接事件完成后，抛弃连接事件代理，避免长时间持有client的引用，导致无法释放资源
 * TCPClient 需要在状态转换后，判断是否放弃当前的TCPConnection对象，避免资源持续相互引用，无法释放
 */

namespace com.fpnn
{
    /*
     * Connection events.
     */
    public delegate void ConnectionConnectedDelegate(Int64 connectionId, string endpoint, bool connected);
    public delegate void ConnectionCloseDelegate(Int64 connectionId, string endpoint, bool causedByError);

    /*
     * Process server pushed quests and return answers if necessary.
     */
    public delegate Answer QuestProcessDelegate(Int64 connectionId, string endpoint, Quest quest);

    public interface IQuestProcessor
    {
        QuestProcessDelegate GetQuestProcessDelegate(string method);
    }

    /*
     * Recommend that call Close() method when client instance is no longer used.
     * If this method haven't been called, the client instance will be existed until which connection is broken.
     */
    public class TCPClient
    {
        public enum ClientStatus
        {
            Closed,
            Connecting,
            Connected
        }

        //----------------[ fields ]-----------------------//

        private object interLocker;
        private readonly DnsEndPoint dnsEndpoint;
        public volatile int ConnectTimeout;
        public volatile int QuestTimeout;
        public volatile bool AutoConnect;

        private ClientStatus status;
        private ManualResetEvent syncConnectingEvent;
        private TCPConnection connection;
        private ConnectionConnectedDelegate connectConnectedDelegate;
        private ConnectionCloseDelegate connectionCloseDelegate;
        private IQuestProcessor questProcessor;

        private common.ErrorRecorder errorRecorder;

        //----------------[ Constructor ]-----------------------//
        public TCPClient(string host, int port, bool autoConnect = true)
        {
            interLocker = new object();
            dnsEndpoint = new DnsEndPoint(host, port);
            ConnectTimeout = 0;
            QuestTimeout = 0;
            AutoConnect = autoConnect;
            status = ClientStatus.Closed;
            syncConnectingEvent = new ManualResetEvent(false);

            errorRecorder = ClientEngine.errorRecorder;
        }

        public static TCPClient Create(string host, int port, bool autoConnect = true)
        {
            return new TCPClient(host, port, autoConnect);
        }

        public static TCPClient Create(string endpoint, bool autoConnect = true)
        {
            int idx = endpoint.LastIndexOf(':');
            if (idx == -1)
                throw new ArgumentException("Invalid endpoint: " + endpoint);

            string host = endpoint.Substring(0, idx);
            string portString = endpoint.Substring(idx + 1);
            int port = Convert.ToInt32(portString, 10);

            return new TCPClient(host, port, autoConnect);
        }

        //----------------[ Properties methods ]-----------------------//

        public string Endpoint()
        {
            return dnsEndpoint.ToString();
        }

        public ClientStatus Status()
        {
            lock (interLocker)
            {
                return status;
            }
        }

        public bool IsConnected()
        {
            lock (interLocker)
            {
                return status == ClientStatus.Connected;
            }
        }

        //----------------[ Configure Operations ]-----------------------//
        public void SetConnectionConnectedDelegate(ConnectionConnectedDelegate ccd)
        {
            lock (interLocker)
            {
                connectConnectedDelegate = ccd;
            }
        }

        public void SetConnectionCloseDelegate(ConnectionCloseDelegate cwcd)
        {
            lock (interLocker)
            {
                connectionCloseDelegate = cwcd;
            }
        }

        public void SetErrorRecorder(common.ErrorRecorder recorder)
        {
            lock (interLocker)
            {
                errorRecorder = recorder;
            }
        }

        public void SetQuestProcessor(IQuestProcessor processor)
        {
            lock (interLocker)
            {
                questProcessor = processor;
            }
        }

        //----------------[ Internal Configure Operations ]-----------------------//
        private void SetClientStatus(TCPConnection conn, ClientStatus newStatus)
        {
            lock (interLocker)
            {
                if (conn == connection)
                {
                    status = newStatus;
                    if (status == ClientStatus.Closed)
                        conn = null;
                }
            }
        }

        private void ConfigConnectedDelegate(TCPConnection conn, ConnectionConnectedDelegate cb, ManualResetEvent finishEvent)
        {
            conn.SetConnectedDelegate((Int64 connectionId, string endpoint, bool connected) =>
            {
                if (cb != null)
                    try
                    {
                        cb(connectionId, endpoint, connected);
                    }
                    catch (Exception ex)
                    {
                        if (errorRecorder != null)
                            errorRecorder.RecordError("Connected event exception. Remote endpoint: " + endpoint + ".", ex);
                    }

                SetClientStatus(conn, connected ? ClientStatus.Connected : ClientStatus.Closed);

                finishEvent.Set();
            });
        }

        private void ConfigWillCloseDelegate(TCPConnection conn, ConnectionCloseDelegate cb)
        {
            conn.SetCloseDelegate((Int64 connectionId, string endpoint, bool causedByError) =>
            {
                SetClientStatus(conn, ClientStatus.Closed);

                cb?.Invoke(connectionId, endpoint, causedByError);
            });
        }

        //----------------[ Connect Operations ]-----------------------//
        private void RealConnect()
        {
            TCPConnection conn;
            lock (interLocker)
            {
                if (status != ClientStatus.Closed)
                    return;

                connection = new TCPConnection(dnsEndpoint);

                ConfigConnectedDelegate(connection, connectConnectedDelegate, syncConnectingEvent);

                ConfigWillCloseDelegate(connection, connectionCloseDelegate);

                if (questProcessor != null)
                    connection.SetQuestProcessor(questProcessor);

                if (errorRecorder != null)
                    connection.SetErrorRecorder(errorRecorder);

                status = ClientStatus.Connecting;
                syncConnectingEvent.Reset();

                conn = connection;
            }

            conn.AsyncConnect(ConnectTimeout);
        }

        public void AsyncConnect()
        {
            RealConnect();
        }

        public bool SyncConnect()
        {
            RealConnect();
            syncConnectingEvent.WaitOne();

            lock (interLocker)
            {
                return (status == ClientStatus.Connected);
            }
        }

        public void AsyncReconnect()
        {
            Close();
            AsyncConnect();
        }

        public bool SyncReconnect()
        {
            Close();
            return SyncConnect();
        }

        public void Close()
        {
            TCPConnection conn;
            lock (interLocker)
            {
                conn = connection;
                connection = null;
                status = ClientStatus.Closed;
                syncConnectingEvent.Set();          //-- If some threads are waiting for sync connecting finished.
            }

            if (conn != null)
                conn.Close();
        }

        //----------------[ Operations ]-----------------------//
        public bool SendQuest(Quest quest, IAnswerCallback callback, int timeout = 0)
        {
            if (AutoConnect)
                AsyncConnect();     //-- Auto check and reconnect if necessary.

            TCPConnection conn = null;

            lock (interLocker)
            {
                conn = connection;
            }

            if (conn != null)
            {
                if (timeout == 0)
                    timeout = QuestTimeout;

                if (timeout == 0)
                    timeout = ClientEngine.globalQuestTimeoutSeconds;

                conn.SendQuest(quest, callback, timeout);
                return true;
            }
            else
                return false;
        }

        public bool SendQuest(Quest quest, AnswerDelegate callback, int timeout = 0)
        {
            AnswerDelegateCallback cb = new AnswerDelegateCallback(callback);
            return SendQuest(quest, cb, timeout);
        }

        public Answer SendQuest(Quest quest, int timeout = 0)
        {
            if (quest.IsOneWay())
            {
                SendQuest(quest, (IAnswerCallback)null, timeout);
                return null;
            }

            SyncAnswerCallback callback = new SyncAnswerCallback(quest);
            if (SendQuest(quest, callback, timeout))
            {
                return callback.GetAnswer();
            }

            Answer answer = new Answer(quest);
            answer.FillErrorCode(ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
            return answer;
        }

        public void SendAnswer(Answer answer)
        {
            TCPConnection conn = null;

            lock (interLocker)
            {
                conn = connection;
            }

            if (conn != null)
                conn.SendAnswer(answer);
        }
    }
}
