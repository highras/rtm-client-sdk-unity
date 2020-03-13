using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using com.fpnn.proto;

namespace com.fpnn
{
    // Reference: https://github.com/gaochundong/Cowboy/blob/master/Cowboy/Cowboy.Sockets/Tcp/Client/EAP/TcpSocketSaeaClient.cs
    // Reference: https://answers.unity.com/questions/812618/how-to-use-socketasynceventargs-in-unity3d.html
    // Reference: https://blog.csdn.net/fuemocheng/article/details/78241405
    // Reference: https://www.cnblogs.com/MRRAOBX/articles/3908026.html
    // Reference: https://docs.microsoft.com/zh-cn/dotnet/api/system.net.sockets.socketasynceventargs?view=netframework-4.6

    internal class TCPConnection
    {
        private const int MaxRecursionDeepOfReceiveFunction = 10;
        private const int MaxRecursionDeepOfSendFunction = 10;

        private class AnswerCallbackUnit
        {
            public IAnswerCallback callback;
            public UInt32 seqNum;
            public Int64 timeoutTime;
        }

        //----------------[ fields ]-----------------------//
        private object interLocker;
        private readonly EndPoint endpoint;
        private TCPClient.ClientStatus status;
        private volatile bool beginClosing;
        private volatile bool requireClose;
        private bool connectingCanBeCannelled;

        private Socket socket;
        private SocketAsyncEventArgs receiveAsyncEventArgs;
        private SocketAsyncEventArgs sendAsyncEventArgs;
        private int recursionDeepOfReceiveFunction;
        private int recursionDeepOfSendFunction;

        private Int64 connectionId;
        private ConnectionConnectedDelegate connectConnectedDelegate;
        private ConnectionCloseDelegate connectionCloseDelegate;
        private IQuestProcessor questProcessor;

        private int currSendOffset;
        private byte[] currSendBuffer;
        private Queue<byte[]> sendQueue;
        private ReceiverBase receiver;

        private Dictionary<Int64, HashSet<AnswerCallbackUnit>> callbackTimeoutMap;
        private Dictionary<UInt32, AnswerCallbackUnit> callbackSeqNumMap;

        private common.ErrorRecorder errorRecorder;

        //----------------[ Constructor ]-----------------------//

        public TCPConnection(EndPoint endpoint)
        {
            interLocker = new object();
            this.endpoint = endpoint;
            status = TCPClient.ClientStatus.Closed;
            beginClosing = false;
            requireClose = false;
            connectingCanBeCannelled = false;

            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            receiveAsyncEventArgs = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };
            receiveAsyncEventArgs.Completed += IO_Completed;

            currSendOffset = 0;
            currSendBuffer = null;
            sendQueue = new Queue<byte[]>();
            callbackTimeoutMap = new Dictionary<Int64, HashSet<AnswerCallbackUnit>>();
            callbackSeqNumMap = new Dictionary<UInt32, AnswerCallbackUnit>();
        }

        //----------------[ Configure Operations ]-----------------------//

        public void SetConnectedDelegate(ConnectionConnectedDelegate cb)
        {
            connectConnectedDelegate = cb;
        }

        public void SetCloseDelegate(ConnectionCloseDelegate cb)
        {
            connectionCloseDelegate = cb;
        }

        public void SetQuestProcessor(IQuestProcessor questProcessor)
        {
            this.questProcessor = questProcessor;
        }

        public void SetErrorRecorder(common.ErrorRecorder er)
        {
            errorRecorder = er;
        }

        //----------------[ Properties methods ]-----------------------//
        public TCPClient.ClientStatus Status()
        {
            lock (interLocker)
            {
                return status;
            }
        }

        //----------------[ I/O Operations ]-----------------------//

        public void AsyncConnect(int connectTimeout)
        {
            lock (interLocker)
            {
                if (status != TCPClient.ClientStatus.Closed)
                    return;

                status = TCPClient.ClientStatus.Connecting;  
            }

            if (ClientEngine.RegisterConnectingConnection(this, connectTimeout) == false)
            {
                lock (interLocker)
                {
                    status = TCPClient.ClientStatus.Closed;
                    beginClosing = true;
                }

                CallConnectionConnectedDelegate(0, false, "Connecting cannel event exception. Remote endpoint: " + endpoint + ".");
                connectionCloseDelegate = null;
                socket.Close();
                return;
            }

            connectionId = 0;
            receiver = new StandardReceiver();
            sendAsyncEventArgs = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };
            sendAsyncEventArgs.Completed += IO_Completed;

            recursionDeepOfReceiveFunction = 0;
            recursionDeepOfSendFunction = 0;

            try
            {
                if (!socket.ConnectAsync(receiveAsyncEventArgs))      //-- Synchronous
                    ConnectCompleted(socket, receiveAsyncEventArgs);
                else
                {
                    lock (interLocker)
                    {
                        if (requireClose)
                            CannelConnecting();
                        else
                            connectingCanBeCannelled = true;
                    }
                }
            }
            catch (SocketException e)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Connect to " + endpoint + " failed.", e);

                CloseWhenConnectingError();
            }
            catch (ObjectDisposedException e)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Connect to " + endpoint + " failed.", e);

                CloseWhenConnectingError();
            }
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    recursionDeepOfReceiveFunction = 0;
                    ReceiveCompleted(sender, e);
                    break;

                case SocketAsyncOperation.Send:
                    recursionDeepOfSendFunction = 0;
                    SendCompleted(sender, e);
                    break;

                case SocketAsyncOperation.Connect:
                    ConnectCompleted(sender, e);
                    break;

                case SocketAsyncOperation.Disconnect:
                    FinallyShutdownClose();
                    break;

                default:
                    {
                        string info;
                        if (e == receiveAsyncEventArgs)
                            info = "receiveAsyncEventArgs.";
                        else if (e == sendAsyncEventArgs)
                            info = "sendAsyncEventArgs.";
                        else
                            info = "closeAsyncEventArgs or other.";

                        CloseByException("IO_Completed exception. LastOperation is " + e.LastOperation
                            + ". Error SocketAsyncEventArgs is " + info, null, false);
                    }
                    break;
            }
        }

        private void ConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                if (errorRecorder != null && !requireClose)
                    errorRecorder.RecordError("Connect to " + endpoint + " failed. Due to SocketError: " + e.SocketError);

                CloseWhenConnectingError();
                return;
            }

            if (requireClose)
            {
                CallConnectionConnectedDelegate(0, false, "Connecting cannel event exception. Remote endpoint: " + endpoint + ".");
                connectionCloseDelegate = null;
                Close();
                return;
            }
            
            receiveAsyncEventArgs.SetBuffer(receiver.buffer, receiver.offset, receiver.requireLength - receiver.offset);
            connectionId = socket.Handle.ToInt64();

            CallConnectionConnectedDelegate(connectionId, true, "Connected event exception. Remote endpoint: " + endpoint + ".");

            lock (interLocker)
            {
                status = TCPClient.ClientStatus.Connected;
            }

            if (requireClose)
            {
                Close();
                return;
            }

            if (ClientEngine.RegisterConnectedConnection(this) == false)
            {
                Close();
                return;
            }

            CheckSending();

            try
            {
                if (!socket.ReceiveAsync(receiveAsyncEventArgs))
                    ReceiveCompleted(socket, receiveAsyncEventArgs);
            }
            catch (ObjectDisposedException ex)
            {
                CloseByException("Receive data from " + endpoint + " exception. Connection is broken.", ex, true);
            }
            catch (SocketException ex)
            {
                CloseByException("Receive data from " + endpoint + " exception. Access socket is error.", ex, false);
            }
        }

        private void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success && !beginClosing)
            {
                CloseByException("Receive data from " + endpoint + " failed. Due to SocketError: " + e.SocketError, null, false);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                Close();
                return;
            }

            receiver.offset += e.BytesTransferred;
            if (receiver.offset == receiver.requireLength)
            {
                Quest quest;
                Answer answer;

                try
                {
                    receiver.Done(out quest, out answer);
                }
                catch (ReceiverErrorMessageException ex)
                {
                    CloseByException("Processing received data from " + endpoint + " error: " + ex.Message + ". Connection will be closed.", null, false);
                    return;
                }
                catch (Exception ex)
                {
                    CloseByException("Processing received data from " + endpoint + " exception. Connection will be closed.", ex, false);
                    return;
                }
                

                if (answer != null)
                    DealAnswer(answer);
                else if (quest != null)
                    DealQuest(quest);
            }

            if (beginClosing)
                return;

            receiveAsyncEventArgs.SetBuffer(receiver.buffer, receiver.offset, receiver.requireLength - receiver.offset);

            try
            {
                if (!socket.ReceiveAsync(receiveAsyncEventArgs))
                {
                    recursionDeepOfReceiveFunction += 1;
                    if (recursionDeepOfReceiveFunction <= MaxRecursionDeepOfReceiveFunction)
                    {
                        ReceiveCompleted(socket, receiveAsyncEventArgs);
                    }
                    else
                    {
                        recursionDeepOfReceiveFunction = 0;
                        ClientEngine.RunTask(() => {
                            ReceiveCompleted(socket, receiveAsyncEventArgs);
                        });
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                CloseByException("Receive data from " + endpoint + " exception. Connection is broken.", ex, true);
            }
            catch (SocketException ex)
            {
                CloseByException("Receive data from " + endpoint + " exception. Access socket is error.", ex, false);
            }
        }

        private void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success && !beginClosing)
            {
                CloseByException("Send data to " + endpoint + " failed. Due to SocketError: " + e.SocketError, null, false);
                return;
            }

            currSendOffset += e.BytesTransferred;
            if (currSendOffset == currSendBuffer.Length)
            {
                currSendOffset = 0;

                lock (interLocker)
                {
                    if (sendQueue.Count > 0)
                    {
                        currSendBuffer = sendQueue.Dequeue();
                    }
                    else
                    {
                        currSendBuffer = null;
                        return;
                    }
                }
            }

            if (beginClosing)
                return;

            sendAsyncEventArgs.SetBuffer(currSendBuffer, currSendOffset, currSendBuffer.Length - currSendOffset);

            try
            {
                if (!socket.SendAsync(sendAsyncEventArgs))
                {
                    recursionDeepOfSendFunction += 1;
                    if (recursionDeepOfSendFunction <= MaxRecursionDeepOfSendFunction)
                    {
                        SendCompleted(socket, sendAsyncEventArgs);
                    }
                    else
                    {
                        recursionDeepOfSendFunction = 0;
                        ClientEngine.RunTask(() => {
                            SendCompleted(socket, sendAsyncEventArgs);
                        });
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                CloseByException("Send data to " + endpoint + " exception. Connection is broken.", ex, true);
            }
            catch (SocketException ex)
            {
                CloseByException("Send data to " + endpoint + " exception. Access socket is error.", ex, false);
            }
        }

        private void CheckSending()
        {
            bool startSending = false;

            lock (interLocker)
            {
                if (currSendBuffer != null)
                    return;

                if (sendQueue.Count == 0)
                    return;

                startSending = true;
                currSendBuffer = sendQueue.Dequeue();
            }

            if (!startSending)
                return;

            if (beginClosing)
                return;

            currSendOffset = 0;
            sendAsyncEventArgs.SetBuffer(currSendBuffer, 0, currSendBuffer.Length);
            try
            {
                if (!socket.SendAsync(sendAsyncEventArgs))
                    SendCompleted(socket, sendAsyncEventArgs);
            }
            catch (ObjectDisposedException ex)
            {
                CloseByException("Send data to " + endpoint + " exception. Connection is broken.", ex, true);
            }
            catch (SocketException ex)
            {
                CloseByException("Send data to " + endpoint + " exception. Access socket is error.", ex, false);
            }
        }

        //----------------[ Closing Operations ]-----------------------//

        public void Close()
        {
            Close(false);
        }

        private void Close(bool socketDisposed)
        {
            if (beginClosing)
                return;

            lock (interLocker)
            {
                if (status == TCPClient.ClientStatus.Closed)
                    return;

                if (status == TCPClient.ClientStatus.Connecting)
                {
                    requireClose = true;

                    if (connectingCanBeCannelled)
                        CannelConnecting();

                    return;
                }

                status = TCPClient.ClientStatus.Closed;
                beginClosing = true;
            }

            InternalClose(true, false, socketDisposed, false);
        }

        private void CloseByException(string message, Exception ex, bool socketDisposed)
        {
            if (errorRecorder != null)
            {
                if (ex != null)
                    errorRecorder.RecordError(message, ex);
                else
                    errorRecorder.RecordError(message);
            }

            InternalClose(true, true, socketDisposed, true);
        }

        private void InternalClose(bool callCloseEvent, bool causedByError, bool socketDisposed, bool checkReClosing)
        {
            if (checkReClosing)
            {
                lock (interLocker)
                {
                    if (status == TCPClient.ClientStatus.Closed)
                        return;

                    status = TCPClient.ClientStatus.Closed;
                }
            }

            CleanForClose(callCloseEvent, causedByError);

            if (socketDisposed)
            {
                socket.Close();
                return;
            }

            SocketAsyncEventArgs closeAsyncEventArgs = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };
            closeAsyncEventArgs.Completed += IO_Completed;

            try
            {
                if (!socket.DisconnectAsync(closeAsyncEventArgs))
                    FinallyShutdownClose();

                return;
            }
            catch (SocketException e)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Internal closing failed. SocketException means accessing socket is failed.", e);

                socket.Close();
            }
            catch (ObjectDisposedException)
            {
                socket.Close();
            }
        }

        private void CleanForClose(bool callCloseEvent, bool causedByError)
        {
            ClientEngine.UnregisterConnection(this);

            ClearAllCallback(ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED);

            if (callCloseEvent && connectionCloseDelegate != null)
            {
                try
                {
                    connectionCloseDelegate(connectionId, endpoint.ToString(), causedByError);
                }
                catch (Exception ex)
                {
                    if (errorRecorder != null)
                        errorRecorder.RecordError("Close event exception. Remote endpoint: " + endpoint + ".", ex);
                }
            }
            connectionCloseDelegate = null;
        }

        private void CannelConnecting()
        {
            try
            {
                Socket.CancelConnectAsync(receiveAsyncEventArgs);
            }
            catch (Exception ex)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Cannel connecting exception. Remote endpoint: " + endpoint + ".", ex);
            }
        }

        private void CloseWhenConnectingError()
        {
            ClientEngine.UnregisterConnection(this);

            lock (interLocker)
            {
                if (status == TCPClient.ClientStatus.Closed)
                    return;

                status = TCPClient.ClientStatus.Closed;
                beginClosing = true;
            }

            CallConnectionConnectedDelegate(0, false, "Connecting failed event exception. Remote endpoint: " + endpoint + ".");
            connectionCloseDelegate = null;

            ClearAllCallback(ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);

            socket.Close();
        }

        private void FinallyShutdownClose()
        {
            try
            {
                if (socket.Connected)
                    socket.Shutdown(SocketShutdown.Both);
            }
            catch (ObjectDisposedException)
            { /* Do nothing. */ }
            catch (Exception ex)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Exception when socket.Shutdown() action.", ex);
            }
            finally
            {
                socket.Close();
            }
        }

        //----------------[ callbacks Functions ]-----------------------//

        private void CallConnectionConnectedDelegate(Int64 connectionId, bool connected, string exceptionMessage)
        {
            if (connectConnectedDelegate != null)
            {
                try
                {
                    connectConnectedDelegate(connectionId, endpoint.ToString(), connected);
                }
                catch (Exception ex)
                {
                    if (errorRecorder != null)
                        errorRecorder.RecordError(exceptionMessage, ex);
                }

                connectConnectedDelegate = null;
            }
        }

        static void RunCallback(IAnswerCallback callback, int errorCode)
        {
            ClientEngine.RunTask(() => {
                callback.OnException(null, errorCode);
            });
        }

        public void CleanTimeoutedCallbacks(Int64 currentSeconds)
        {
            HashSet<IAnswerCallback> callbacks = new HashSet<IAnswerCallback>();
            Dictionary<Int64, HashSet<AnswerCallbackUnit>> timeoutedDict = new Dictionary<Int64, HashSet<AnswerCallbackUnit>>();

            lock (interLocker)
            {
                foreach (KeyValuePair<Int64, HashSet<AnswerCallbackUnit>> kvp in callbackTimeoutMap)
                {
                    if (kvp.Key <= currentSeconds)
                        timeoutedDict.Add(kvp.Key, kvp.Value);
                }

                foreach (KeyValuePair<Int64, HashSet<AnswerCallbackUnit>> kvp in timeoutedDict)
                {
                    callbackTimeoutMap.Remove(kvp.Key);

                    foreach(AnswerCallbackUnit unit in kvp.Value)
                    {
                        callbackSeqNumMap.Remove(unit.seqNum);
                        callbacks.Add(unit.callback);
                    }
                }
            }

            foreach (IAnswerCallback callback in callbacks)
                RunCallback(callback, ErrorCode.FPNN_EC_CORE_TIMEOUT);
        }

        private void ClearAllCallback(int errorCode)
        {
            Dictionary<UInt32, AnswerCallbackUnit> oldCallbackDict = new Dictionary<UInt32, AnswerCallbackUnit>();

            lock (interLocker)
            {
                Dictionary<UInt32, AnswerCallbackUnit> tmp = callbackSeqNumMap;
                callbackSeqNumMap = oldCallbackDict;
                oldCallbackDict = tmp;

                callbackTimeoutMap.Clear();
            }

            foreach (KeyValuePair<UInt32, AnswerCallbackUnit> kvp in oldCallbackDict)
                RunCallback(kvp.Value.callback, errorCode);
        }

        //----------------[ Quest & Answer Processing ]-----------------------//

        private void RunQuestProcessor(Quest quest, QuestProcessDelegate process)
        {
            TCPConnection conn = this;

            ClientEngine.RunTask(() => {
                
                Answer answer = null;
                bool asyncAnswered = false;
                AdvancedAnswerInfo.Reset(conn, quest);

                try
                {
                    answer = process(connectionId, endpoint.ToString(),  quest);
                }
                catch (Exception ex)
                {
                    if (errorRecorder != null)
                        errorRecorder.RecordError("Run quest process for method: " + quest.Method(), ex);
                }
                finally
                {
                    asyncAnswered = AdvancedAnswerInfo.Answered();
                }

                if (quest.IsTwoWay() && !asyncAnswered)
                {
                    if (answer == null)
                    {
                        answer = new Answer(quest);
                        answer.FillErrorInfo(ErrorCode.FPNN_EC_CORE_UNKNOWN_ERROR, "Two way quest " + quest.Method() + " lose an answer.");
                    }
                    SendAnswer(answer);
                }
                else
                {
                    if (answer != null)
                        if (errorRecorder != null)
                        {
                            if (quest.IsOneWay())
                                errorRecorder.RecordError("Answer created for one way quest: " + quest.Method());
                            else
                                errorRecorder.RecordError("Answer created reduplicated for two way quest: " + quest.Method());
                        }
                }
            });
        }

        private void DealQuest(Quest quest)
        {
            if (questProcessor != null)
            {
                QuestProcessDelegate process = questProcessor.GetQuestProcessDelegate(quest.Method());
                if (process != null)
                {
                    RunQuestProcessor(quest, process);
                }
                else
                {
                    if (quest.IsTwoWay())
                    {
                        Answer answer = new Answer(quest);
                        answer.FillErrorInfo(ErrorCode.FPNN_EC_CORE_UNKNOWN_METHOD, "This method is not supported by client.");
                        SendAnswer(answer);
                    }
                }
            }
            else
            {
                if (quest.IsTwoWay())
                {
                    Answer answer = new Answer(quest);
                    answer.FillErrorInfo(ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE, "Client without quest processor.");
                    SendAnswer(answer);
                }
            }
        }

        private void DealAnswer(Answer answer)
        {
            AnswerCallbackUnit unit = null;
            UInt32 seq = answer.SeqNum();
            lock (interLocker)
            {
                if (callbackSeqNumMap.TryGetValue(seq, out unit))
                {
                    callbackSeqNumMap.Remove(seq);

                    if (callbackTimeoutMap.TryGetValue(unit.timeoutTime, out HashSet<AnswerCallbackUnit> cbSet))
                    {
                        cbSet.Remove(unit);
                        if (cbSet.Count == 0)
                            callbackTimeoutMap.Remove(unit.timeoutTime);
                    }
                }
            }

            if (unit != null)
            {
                ClientEngine.RunTask(() => {
                    unit.callback.OnAnswer(answer);
                });
            }
        }

        //private void sendQuest(Quest quest, AnswerCallback callback, int timeoutInSeconds, boolean keyExchangedQuest)

        public void SendQuest(Quest quest, IAnswerCallback callback, int timeoutInSeconds)
        {
            if (quest == null)
            {
                if (callback != null)
                    RunCallback(callback, ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE);

                return;
            }

            bool isClosed;
            byte[] raw;
            try
            {
                raw = quest.Raw();
            }
            catch (Exception ex)
            {
                if (callback != null)
                    RunCallback(callback, ErrorCode.FPNN_EC_PROTO_UNKNOWN_ERROR);

                if (errorRecorder != null)
                    errorRecorder.RecordError("Send quest cannelled. Quest.Raw() exception.", ex);

                return;
            }

            lock (interLocker)
            {
                isClosed = (status == TCPClient.ClientStatus.Closed);
                if (!isClosed)
                {
                    sendQueue.Enqueue(raw);

                    if (callback != null)
                    {
                        if (timeoutInSeconds == 0)
                            timeoutInSeconds = ClientEngine.globalQuestTimeoutSeconds;

                        TimeSpan span = DateTime.Now - ClientEngine.originDateTime;
                        Int64 seconds = (Int64)Math.Floor(span.TotalSeconds) + timeoutInSeconds;

                        AnswerCallbackUnit unit = new AnswerCallbackUnit();
                        unit.callback = callback;
                        unit.seqNum = quest.SeqNum();
                        unit.timeoutTime = seconds;

                        callbackSeqNumMap.Add(quest.SeqNum(), unit);

                        if (callbackTimeoutMap.TryGetValue(seconds, out HashSet<AnswerCallbackUnit> cbSet))
                        {
                            cbSet.Add(unit);
                        }
                        else
                        {
                            cbSet = new HashSet<AnswerCallbackUnit>();
                            cbSet.Add(unit);
                            callbackTimeoutMap.Add(seconds, cbSet);
                        }
                    }

                    if (status == TCPClient.ClientStatus.Connecting)
                        return;
                }
            }

            if (!isClosed)
                CheckSending();
            else
            {
                if (callback != null)
                    RunCallback(callback, ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);

                if (errorRecorder != null)
                    errorRecorder.RecordError("Send Quest " + quest.Method() + " on closed connection.");
            }
        }

        public void SendAnswer(Answer answer)
        {
            bool checkSending = true;
            byte[] raw;

            try
            {
                raw = answer.Raw();
            }
            catch (Exception ex)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Send answer cannelled. Answer.Raw() exception.", ex);

                return;
            }

            lock (interLocker)
            {
                if (status == TCPClient.ClientStatus.Connected)
                {
                    sendQueue.Enqueue(raw);
                }
                else
                    checkSending = false;
            }

            if (checkSending)
                CheckSending();
            else
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Send answer on closed connection.");
            }
        }
    }
}
