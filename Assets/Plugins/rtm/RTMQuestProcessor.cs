using System;
using System.Collections.Generic;
using System.Threading;
using com.fpnn.common;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public class RTMQuestProcessor
    {
        //----------------[ System Events ]-----------------//
        public virtual void SessionClosed(int ClosedByErrorCode) { }    //-- ErrorCode: com.fpnn.ErrorCode & com.fpnn.rtm.ErrorCode

        //-- Return true for starting relogin, false for stopping relogin.
        public virtual bool ReloginWillStart(int lastErrorCode, int retriedCount) { return true; }
        public virtual void ReloginCompleted(bool successful, bool retryAgain, int errorCode, int retriedCount) { }

        public virtual void Kickout() { }
        public virtual void KickoutRoom(long roomId) { }

        //----------------[ Message Interfaces ]-----------------//
        //-- Messages
        public virtual void PushMessage(RTMMessage message) { }
        public virtual void PushGroupMessage(RTMMessage message) { }
        public virtual void PushRoomMessage(RTMMessage message) { }
        public virtual void PushBroadcastMessage(RTMMessage message) { }

        //-- Chat
        public virtual void PushChat(RTMMessage message) { }
        public virtual void PushGroupChat(RTMMessage message) { }
        public virtual void PushRoomChat(RTMMessage message) { }
        public virtual void PushBroadcastChat(RTMMessage message) { }

        //-- Audio
        public virtual void PushAudio(RTMMessage message) { }
        public virtual void PushGroupAudio(RTMMessage message) { }
        public virtual void PushRoomAudio(RTMMessage message) { }
        public virtual void PushBroadcastAudio(RTMMessage message) { }

        //-- Cmd
        public virtual void PushCmd(RTMMessage message) { }
        public virtual void PushGroupCmd(RTMMessage message) { }
        public virtual void PushRoomCmd(RTMMessage message) { }
        public virtual void PushBroadcastCmd(RTMMessage message) { }

        //-- Files
        public virtual void PushFile(RTMMessage message) { }
        public virtual void PushGroupFile(RTMMessage message) { }
        public virtual void PushRoomFile(RTMMessage message) { }
        public virtual void PushBroadcastFile(RTMMessage message) { }
    }

    internal class RTMMasterProcessor: IRTMMasterProcessor
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
                questProcessor.SessionClosed(ClosedByErrorCode);

            RTMControlCenter.UnregisterSession(connectionId);
        }

        public bool ReloginWillStart(int lastErrorCode, int retriedCount)
        {
            bool startRelogin = true;
            if (questProcessor != null)
                startRelogin = questProcessor.ReloginWillStart(lastErrorCode, retriedCount);

            if (startRelogin)       //-- if startRelogin == false, will call SessionClosed(), the UnregisterSession() will be called in SessionClosed().
                RTMControlCenter.UnregisterSession(connectionId);

            return startRelogin;
        }

        public void ReloginCompleted(bool successful, bool retryAgain, int errorCode, int retriedCount)
        {
            if (questProcessor != null)
                questProcessor.ReloginCompleted(successful, retryAgain, errorCode, retriedCount);
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
            RTMControlCenter.CloseSession(connectionId);

            if (questProcessor != null)
                questProcessor.Kickout();

            return null;
        }

        public Answer KickoutRoom(Int64 connectionId, string endpoint, Quest quest)
        {
            if (questProcessor != null)
            {
                long roomId = quest.Want<Int64>("rid");
                questProcessor.KickoutRoom(roomId);
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
            }
            else
            {
                MessageInfo messageInfo = BuildMessageInfo(quest);
                if (messageInfo.isBinary)
                {
                    rtmMessage.binaryMessage = messageInfo.binaryData;
                }
                else if (rtmMessage.messageType == (byte)MessageType.Audio)
                {
                    rtmMessage.audioInfo = RTMClient.BuildAudioInfo(messageInfo.message, errorRecorder);

                    if (rtmMessage.audioInfo != null)
                        rtmMessage.stringMessage = rtmMessage.audioInfo.recognizedText;
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

            if (duplicatedFilter.CheckP2PMessage(from, mid) == false)
                return null;

            RTMMessage rtmMessage = BuildRTMMessage(quest, from, to, mid);

            if (rtmMessage.messageType == (byte)MessageType.Chat)
            {
                if (rtmMessage.translatedInfo != null)
                    questProcessor.PushChat(rtmMessage);
            }
            else if (rtmMessage.messageType == (byte)MessageType.Audio)
            {
                if (rtmMessage.audioInfo != null)
                    questProcessor.PushAudio(rtmMessage);
            }
            else if (rtmMessage.messageType == (byte)MessageType.Cmd)
            {
                questProcessor.PushCmd(rtmMessage);
            }
            else if (rtmMessage.messageType >= 40 && rtmMessage.messageType <= 50)
            {
                questProcessor.PushFile(rtmMessage);
            }
            else
            {
                questProcessor.PushMessage(rtmMessage);
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
                    questProcessor.PushGroupChat(rtmMessage);
            }
            else if (rtmMessage.messageType == (byte)MessageType.Audio)
            {
                if (rtmMessage.audioInfo != null)
                    questProcessor.PushGroupAudio(rtmMessage);
            }
            else if (rtmMessage.messageType == (byte)MessageType.Cmd)
            {
                questProcessor.PushGroupCmd(rtmMessage);
            }
            else if (rtmMessage.messageType >= 40 && rtmMessage.messageType <= 50)
            {
                questProcessor.PushGroupFile(rtmMessage);
            }
            else
            {
                questProcessor.PushGroupMessage(rtmMessage);
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
                    questProcessor.PushRoomChat(rtmMessage);
            }
            else if (rtmMessage.messageType == (byte)MessageType.Audio)
            {
                if (rtmMessage.audioInfo != null)
                    questProcessor.PushRoomAudio(rtmMessage);
            }
            else if (rtmMessage.messageType == (byte)MessageType.Cmd)
            {
                questProcessor.PushRoomCmd(rtmMessage);
            }
            else if (rtmMessage.messageType >= 40 && rtmMessage.messageType <= 50)
            {
                questProcessor.PushRoomFile(rtmMessage);
            }
            else
            {
                questProcessor.PushRoomMessage(rtmMessage);
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
                    questProcessor.PushBroadcastChat(rtmMessage);
            }
            else if (rtmMessage.messageType == (byte)MessageType.Audio)
            {
                if (rtmMessage.audioInfo != null)
                    questProcessor.PushBroadcastAudio(rtmMessage);
            }
            else if (rtmMessage.messageType == (byte)MessageType.Cmd)
            {
                questProcessor.PushBroadcastCmd(rtmMessage);
            }
            else if (rtmMessage.messageType >= 40 && rtmMessage.messageType <= 50)
            {
                questProcessor.PushBroadcastFile(rtmMessage);
            }
            else
            {
                questProcessor.PushBroadcastMessage(rtmMessage);
            }

            return null;
        }
    }
}
