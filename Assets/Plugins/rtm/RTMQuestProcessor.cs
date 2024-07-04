using System;
using System.Collections.Generic;
using System.Threading;
using com.fpnn.common;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public delegate void SessionClosedDelegate(int ClosedByErrorCode);
    public delegate bool ReloginWillStartDelegate(int lastErrorCode, int retriedCount);
    public delegate void ReloginCompletedDelegate(bool successful, bool retryAgain, int errorCode, int retriedCount);
    public delegate void ClientStatusChangedDelegate(RTMClient.ClientStatus status);
    public delegate void KickOutDelegate();
    public delegate void KickoutRoomDelegate(long roomId);
    public delegate void PushMessageDelegate(RTMMessage message);

    public class RTMQuestProcessor
    {
        //----------------[ System Events ]-----------------//
        public virtual void SessionClosed(int ClosedByErrorCode) { }    //-- ErrorCode: com.fpnn.ErrorCode & com.fpnn.rtm.ErrorCode
        public SessionClosedDelegate SessionClosedCallback;

        //-- Return true for starting relogin, false for stopping relogin.
        public virtual bool ReloginWillStart(int lastErrorCode, int retriedCount) { return true; }
        public ReloginWillStartDelegate ReloginWillStartCallback;

        public virtual void ReloginCompleted(bool successful, bool retryAgain, int errorCode, int retriedCount) { }
        public ReloginCompletedDelegate ReloginCompletedCallback;

        public virtual void ClientStatusChanged(RTMClient.ClientStatus status){}
        public ClientStatusChangedDelegate ClientStatusChangedCallback;

        public virtual void Kickout() { }
        public KickOutDelegate KickoutCallback;

        public virtual void KickoutRoom(long roomId) { }
        public KickoutRoomDelegate KickoutRoomCallback;

        //----------------[ Message Interfaces ]-----------------//
        //-- Messages
        public virtual void PushMessage(RTMMessage message) { }
        public PushMessageDelegate PushMessageCallback;

        public virtual void PushGroupMessage(RTMMessage message) { }
        public PushMessageDelegate PushGroupMessageCallback;

        public virtual void PushRoomMessage(RTMMessage message) { }
        public PushMessageDelegate PushRoomMessageCallback;

        public virtual void PushBroadcastMessage(RTMMessage message) { }
        public PushMessageDelegate PushBroadcastMessageCallback;

        //-- Chat
        public virtual void PushChat(RTMMessage message) { }
        public PushMessageDelegate PushChatCallback;

        public virtual void PushGroupChat(RTMMessage message) { }
        public PushMessageDelegate PushGroupChatCallback;

        public virtual void PushRoomChat(RTMMessage message) { }
        public PushMessageDelegate PushRoomChatCallback;

        public virtual void PushBroadcastChat(RTMMessage message) { }
        public PushMessageDelegate PushBroadcastChatCallback;

        //-- Cmd
        public virtual void PushCmd(RTMMessage message) { }
        public PushMessageDelegate PushCmdCallback;

        public virtual void PushGroupCmd(RTMMessage message) { }
        public PushMessageDelegate PushGroupCmdCallback;

        public virtual void PushRoomCmd(RTMMessage message) { }
        public PushMessageDelegate PushRoomCmdCallback;

        public virtual void PushBroadcastCmd(RTMMessage message) { }
        public PushMessageDelegate PushBroadcastCmdCallback;

        //-- Files
        public virtual void PushFile(RTMMessage message) { }
        public PushMessageDelegate PushFileCallback;

        public virtual void PushGroupFile(RTMMessage message) { }
        public PushMessageDelegate PushGroupFileCallback;

        public virtual void PushRoomFile(RTMMessage message) { }
        public PushMessageDelegate PushRoomFileCallback;

        public virtual void PushBroadcastFile(RTMMessage message) { }
        public PushMessageDelegate PushBroadcastFileCallback;
    }

    public class RTMMasterProcessor: IRTMMasterProcessor
    {
        private RTMQuestProcessor questProcessor;
        private DuplicatedMessageFilter duplicatedFilter;
        private ErrorRecorder errorRecorder;
        private Int64 connectionId;
        private Int64 lastPingTime;
        private readonly Dictionary<string, QuestProcessDelegate> methodMap;

        public RTMMasterProcessor()
        {
            duplicatedFilter = new DuplicatedMessageFilter();
            lastPingTime = 0;

            methodMap = new Dictionary<string, QuestProcessDelegate> {
                { "ping", Ping },

                { "kickout", Kickout },
                { "kickoutroom", KickoutRoom },

                { "pushmsg", PushMessage },
                { "pushgroupmsg", PushGroupMessage },
                { "pushroommsg", PushRoomMessage },
                { "pushbroadcastmsg", PushBroadcastMessage },
            };
        }

        public void SetProcessor(RTMQuestProcessor processor)
        {
            questProcessor = processor;
        }

        public void SetErrorRecorder(ErrorRecorder recorder)
        {
            errorRecorder = recorder;
        }

        public void SetConnectionId(Int64 connId)
        {
            connectionId = connId;
            Interlocked.Exchange(ref lastPingTime, 0);
        }

        public void BeginCheckPingInterval()
        {
            Int64 now = ClientEngine.GetCurrentSeconds();
            Interlocked.Exchange(ref lastPingTime, now);
        }

        public bool ConnectionIsAlive()
        {
            Int64 lastPingSec = Interlocked.Read(ref lastPingTime);

            if (lastPingSec == 0 || ClientEngine.GetCurrentSeconds() - lastPingSec < RTMConfig.lostConnectionAfterLastPingInSeconds)
                return true;
            else
                return false;
        }

        public QuestProcessDelegate GetQuestProcessDelegate(string method)
        {
            if (methodMap.TryGetValue(method, out QuestProcessDelegate process))
            {
                return process;
            }

            return null;
        }

        public void SessionClosed(int ClosedByErrorCode)
        {
            if (questProcessor != null)
            { 
                questProcessor.SessionClosed(ClosedByErrorCode);
                RTMControlCenter.callbackQueue.PostAction(() => { 
                    questProcessor.SessionClosedCallback?.Invoke(ClosedByErrorCode);
                });
            }

            RTMControlCenter.UnregisterSession(connectionId);
        }

        public bool ReloginWillStart(int lastErrorCode, int retriedCount)
        {
            bool startRelogin = true;
            if (questProcessor != null)
            {
                if (questProcessor.ReloginWillStartCallback == null)
                    startRelogin = questProcessor.ReloginWillStart(lastErrorCode, retriedCount);
                else
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        questProcessor.ReloginWillStartCallback?.Invoke(lastErrorCode, retriedCount);
                    });
                }
            }

            if (startRelogin)       //-- if startRelogin == false, will call SessionClosed(), the UnregisterSession() will be called in SessionClosed().
                RTMControlCenter.UnregisterSession(connectionId);

            return startRelogin;
        }

        public void ReloginCompleted(bool successful, bool retryAgain, int errorCode, int retriedCount)
        {
            if (questProcessor != null)
            { 
                questProcessor.ReloginCompleted(successful, retryAgain, errorCode, retriedCount);
                RTMControlCenter.callbackQueue.PostAction(() => 
                {
                    questProcessor.ReloginCompletedCallback?.Invoke(successful, retryAgain, errorCode, retriedCount);
                });
            }
        }

        public void ClientStatusChanged(RTMClient.ClientStatus status)
        {
            if (questProcessor != null)
            { 
                questProcessor.ClientStatusChanged(status);
                RTMControlCenter.callbackQueue.PostAction(() => 
                {
                    questProcessor.ClientStatusChangedCallback?.Invoke(status);
                });
            }    
        }

        //----------------------[ RTM Operations ]-------------------//
        public Answer Ping(Int64 connectionId, string endpoint, Quest quest)
        {
            AdvanceAnswer.SendAnswer(new Answer(quest));

            Int64 now = ClientEngine.GetCurrentSeconds();
            Interlocked.Exchange(ref lastPingTime, now);

            return null;
        }

        public Answer Kickout(Int64 connectionId, string endpoint, Quest quest)
        {
            bool closed = RTMControlCenter.GetClientStatus(connectionId) == RTMClient.ClientStatus.Closed;
            RTMControlCenter.CloseSession(connectionId);

            if (questProcessor != null && closed == false)
            { 
                questProcessor.Kickout();
                RTMControlCenter.callbackQueue.PostAction(() => 
                {
                    questProcessor.KickoutCallback?.Invoke();
                });
            }

            return null;
        }

        public Answer KickoutRoom(Int64 connectionId, string endpoint, Quest quest)
        {
            if (questProcessor != null)
            {
                long roomId = quest.Want<Int64>("rid");
                questProcessor.KickoutRoom(roomId);
                RTMControlCenter.callbackQueue.PostAction(() => 
                {
                    questProcessor.KickoutRoomCallback?.Invoke(roomId);
                });
            }

            return null;
        }

        //----------------------[ RTM Messagess Utilities ]-------------------//
        private TranslatedInfo ProcessChatMessage(Quest quest)
        {
            TranslatedInfo tm = new TranslatedInfo();

            try
            {
                Dictionary<object, object> msg = quest.Want<Dictionary<object, object>>("msg");
                if (msg.TryGetValue("source", out object source))
                {
                    tm.sourceLanguage = (string)source;
                }
                else
                    tm.sourceLanguage = string.Empty;

                if (msg.TryGetValue("target", out object target))
                {
                    tm.targetLanguage = (string)target;
                }
                else
                    tm.targetLanguage = string.Empty;

                if (msg.TryGetValue("sourceText", out object sourceText))
                {
                    tm.sourceText = (string)sourceText;
                }
                else
                    tm.sourceText = string.Empty;

                if (msg.TryGetValue("targetText", out object targetText))
                {
                    tm.targetText = (string)targetText;
                }
                else
                    tm.targetText = string.Empty;

                return tm;
            }
            catch (InvalidCastException e)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("ProcessChatMessage failed.", e);

                return null;
            }
        }

        private class MessageInfo
        {
            public bool isBinary;
            public byte[] binaryData;
            public string message;
        }

        private MessageInfo BuildMessageInfo(Quest quest)
        {
            MessageInfo info = new MessageInfo();

            object message = quest.Want("msg");
            info.isBinary = RTMClient.CheckBinaryType(message);
            if (info.isBinary)
                info.binaryData = (byte[])message;
            else
                info.message = (string)message;

            return info;
        }

        private RTMMessage BuildRTMMessage(Quest quest, long from, long to, long mid)
        {
            RTMMessage rtmMessage = new RTMMessage
            {
                fromUid = from,
                toId = to,
                messageId = mid,
                messageType = quest.Want<byte>("mtype"),
                attrs = quest.Want<string>("attrs"),
                modifiedTime = quest.Want<long>("mtime")
            };

            if (rtmMessage.messageType == (byte)MessageType.Chat)
            {
                rtmMessage.translatedInfo = ProcessChatMessage(quest);
                if (rtmMessage.translatedInfo != null)
                {
                    if (rtmMessage.translatedInfo.targetText.Length > 0)
                        rtmMessage.stringMessage = rtmMessage.translatedInfo.targetText;
                    else
                        rtmMessage.stringMessage = rtmMessage.translatedInfo.sourceText;
                }
            }
            else if (rtmMessage.messageType == (byte)MessageType.Cmd)
            {
                rtmMessage.stringMessage = quest.Want<string>("msg");
            }
            else if (rtmMessage.messageType >= 40 && rtmMessage.messageType <= 50)
            {
                rtmMessage.stringMessage = quest.Want<string>("msg");
                RTMClient.BuildFileInfo(rtmMessage, errorRecorder);
            }
            else
            {
                MessageInfo messageInfo = BuildMessageInfo(quest);
                if (messageInfo.isBinary)
                {
                    rtmMessage.binaryMessage = messageInfo.binaryData;
                }
                else
                    rtmMessage.stringMessage = messageInfo.message;
            }

            return rtmMessage;
        }

        //----------------------[ RTM Messagess ]-------------------//
        public Answer PushMessage(Int64 connectionId, string endpoint, Quest quest)
        {
            AdvanceAnswer.SendAnswer(new Answer(quest));

            if (questProcessor == null)
                return null;

            long from = quest.Want<long>("from");
            long to = quest.Want<long>("to");
            long mid = quest.Want<long>("mid");

            if (duplicatedFilter.CheckP2PMessage(from, mid, to) == false)
                return null;

            RTMMessage rtmMessage = BuildRTMMessage(quest, from, to, mid);

            if (rtmMessage.messageType == (byte)MessageType.Chat)
            {
                if (rtmMessage.translatedInfo != null)
                { 
                    questProcessor.PushChat(rtmMessage);
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        questProcessor.PushChatCallback?.Invoke(rtmMessage);
                    });
                }
            }
            else if (rtmMessage.messageType == (byte)MessageType.Cmd)
            {
                questProcessor.PushCmd(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushCmdCallback?.Invoke(rtmMessage);
                });
            }
            else if (rtmMessage.messageType >= 40 && rtmMessage.messageType <= 50)
            {
                questProcessor.PushFile(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushFileCallback?.Invoke(rtmMessage);
                });
            }
            else
            {
                questProcessor.PushMessage(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushMessageCallback?.Invoke(rtmMessage);
                });
            }

            return null;
        }

        public Answer PushGroupMessage(Int64 connectionId, string endpoint, Quest quest)
        {
            AdvanceAnswer.SendAnswer(new Answer(quest));

            if (questProcessor == null)
                return null;

            long groupId = quest.Want<long>("gid");
            long from = quest.Want<long>("from");
            long mid = quest.Want<long>("mid");

            if (duplicatedFilter.CheckGroupMessage(groupId, from, mid) == false)
                return null;

            RTMMessage rtmMessage = BuildRTMMessage(quest, from, groupId, mid);

            if (rtmMessage.messageType == (byte)MessageType.Chat)
            {
                if (rtmMessage.translatedInfo != null)
                { 
                    questProcessor.PushGroupChat(rtmMessage);
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        questProcessor.PushGroupChatCallback?.Invoke(rtmMessage);
                    });
                }
            }
            else if (rtmMessage.messageType == (byte)MessageType.Cmd)
            {
                questProcessor.PushGroupCmd(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushGroupCmdCallback?.Invoke(rtmMessage);
                });
            }
            else if (rtmMessage.messageType >= 40 && rtmMessage.messageType <= 50)
            {
                questProcessor.PushGroupFile(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushGroupFileCallback?.Invoke(rtmMessage);
                });
            }
            else
            {
                questProcessor.PushGroupMessage(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushGroupMessageCallback?.Invoke(rtmMessage);
                });
            }

            return null;
        }

        public Answer PushRoomMessage(Int64 connectionId, string endpoint, Quest quest)
        {
            AdvanceAnswer.SendAnswer(new Answer(quest));

            if (questProcessor == null)
                return null;

            long from = quest.Want<long>("from");
            long roomId = quest.Want<long>("rid");
            long mid = quest.Want<long>("mid");

            if (duplicatedFilter.CheckRoomMessage(roomId, from, mid) == false)
                return null;

            RTMMessage rtmMessage = BuildRTMMessage(quest, from, roomId, mid);

            if (rtmMessage.messageType == (byte)MessageType.Chat)
            {
                if (rtmMessage.translatedInfo != null)
                { 
                    questProcessor.PushRoomChat(rtmMessage);
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        questProcessor.PushRoomChatCallback?.Invoke(rtmMessage);
                    });
                }
            }
            else if (rtmMessage.messageType == (byte)MessageType.Cmd)
            {
                questProcessor.PushRoomCmd(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushRoomCmdCallback?.Invoke(rtmMessage);
                });
            }
            else if (rtmMessage.messageType >= 40 && rtmMessage.messageType <= 50)
            {
                questProcessor.PushRoomFile(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushRoomFileCallback?.Invoke(rtmMessage);
                });
            }
            else
            {
                questProcessor.PushRoomMessage(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushRoomMessageCallback?.Invoke(rtmMessage);
                });
            }

            return null;
        }

        public Answer PushBroadcastMessage(Int64 connectionId, string endpoint, Quest quest)
        {
            AdvanceAnswer.SendAnswer(new Answer(quest));

            if (questProcessor == null)
                return null;

            long from = quest.Want<long>("from");
            long mid = quest.Want<long>("mid");

            if (duplicatedFilter.CheckBroadcastMessage(from, mid) == false)
                return null;

            RTMMessage rtmMessage = BuildRTMMessage(quest, from, 0, mid);

            if (rtmMessage.messageType == (byte)MessageType.Chat)
            {
                if (rtmMessage.translatedInfo != null)
                { 
                    questProcessor.PushBroadcastChat(rtmMessage);
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        questProcessor.PushBroadcastChatCallback?.Invoke(rtmMessage);
                    });
                }
            }
            else if (rtmMessage.messageType == (byte)MessageType.Cmd)
            {
                questProcessor.PushBroadcastCmd(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushBroadcastCmdCallback?.Invoke(rtmMessage);
                });
            }
            else if (rtmMessage.messageType >= 40 && rtmMessage.messageType <= 50)
            {
                questProcessor.PushBroadcastFile(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushBroadcastFileCallback?.Invoke(rtmMessage);
                });
            }
            else
            {
                questProcessor.PushBroadcastMessage(rtmMessage);
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    questProcessor.PushBroadcastMessageCallback?.Invoke(rtmMessage);
                });
            }

            return null;
        }
    }
}
