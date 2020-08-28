using System;
using System.Collections.Generic;
using System.Threading;
using com.fpnn.common;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    //-- Obsolete in v.2.2.0
    [Obsolete("Interface IRTMQuestProcessor is deprecated, please use class RTMQuestProcessor instead.")]
    public interface IRTMQuestProcessor
    {
        void SessionClosed(int ClosedByErrorCode);        //-- com.fpnn.ErrorCode & com.fpnn.rtm.ErrorCode

        void Kickout();
        void KickoutRoom(long roomId);

        //-- message for string format
        void PushMessage(long fromUid, long toUid, byte mtype, long mid, string message, string attrs, long mtime);
        void PushGroupMessage(long fromUid, long groupId, byte mtype, long mid, string message, string attrs, long mtime);
        void PushRoomMessage(long fromUid, long roomId, byte mtype, long mid, string message, string attrs, long mtime);
        void PushBroadcastMessage(long fromUid, byte mtype, long mid, string message, string attrs, long mtime);

        //-- message for binary format
        void PushMessage(long fromUid, long toUid, byte mtype, long mid, byte[] message, string attrs, long mtime);
        void PushGroupMessage(long fromUid, long groupId, byte mtype, long mid, byte[] message, string attrs, long mtime);
        void PushRoomMessage(long fromUid, long roomId, byte mtype, long mid, byte[] message, string attrs, long mtime);
        void PushBroadcastMessage(long fromUid, byte mtype, long mid, byte[] message, string attrs, long mtime);

        void PushChat(long fromUid, long toUid, long mid, TranslatedMessage message, string attrs, long mtime);
        void PushGroupChat(long fromUid, long groupId, long mid, TranslatedMessage message, string attrs, long mtime);
        void PushRoomChat(long fromUid, long roomId, long mid, TranslatedMessage message, string attrs, long mtime);
        void PushBroadcastChat(long fromUid, long mid, TranslatedMessage message, string attrs, long mtime);

        void PushAudio(long fromUid, long toUid, long mid, byte[] message, string attrs, long mtime);
        void PushGroupAudio(long fromUid, long groupId, long mid, byte[] message, string attrs, long mtime);
        void PushRoomAudio(long fromUid, long roomId, long mid, byte[] message, string attrs, long mtime);
        void PushBroadcastAudio(long fromUid, long mid, byte[] message, string attrs, long mtime);

        void PushCmd(long fromUid, long toUid, long mid, string message, string attrs, long mtime);
        void PushGroupCmd(long fromUid, long groupId, long mid, string message, string attrs, long mtime);
        void PushRoomCmd(long fromUid, long roomId, long mid, string message, string attrs, long mtime);
        void PushBroadcastCmd(long fromUid, long mid, string message, string attrs, long mtime);

        void PushFile(long fromUid, long toUid, byte mtype, long mid, string message, string attrs, long mtime);
        void PushGroupFile(long fromUid, long groupId, byte mtype, long mid, string message, string attrs, long mtime);
        void PushRoomFile(long fromUid, long roomId, byte mtype, long mid, string message, string attrs, long mtime);
        void PushBroadcastFile(long fromUid, byte mtype, long mid, string message, string attrs, long mtime);
    }

    //-- Obsolete in v.2.2.0
    [Obsolete("RTMMasterProcessorV1 is deprecated, please use RTMMasterProcessor instead.")]
    internal class RTMMasterProcessorV1: IRTMMasterProcessor
    {
        private IRTMQuestProcessor questProcessor;
        private DuplicatedMessageFilter duplicatedFilter;
        private ErrorRecorder errorRecorder;
        private Int64 connectionId;
        private Int64 lastPingTime;
        private Dictionary<string, QuestProcessDelegate> methodMap;

        public RTMMasterProcessorV1()
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

        public void SetProcessor(IRTMQuestProcessor processor)
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
            RTMControlCenter.UnregisterSession(connectionId);
            return true;
        }

        public void ReloginCompleted(bool successful, bool retryAgain, int errorCode, int retriedCount)
        {
            //-- Do nothing.
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
        private TranslatedMessage ProcessChatMessage(Quest quest)
        {
            TranslatedMessage tm = new TranslatedMessage();

            try
            {
                Dictionary<object, object> msg = quest.Want<Dictionary<object, object>>("msg");
                if (msg.TryGetValue("source", out object source))
                {
                    tm.source = (string)source;
                }
                else
                    tm.source = string.Empty;

                if (msg.TryGetValue("target", out object target))
                {
                    tm.target = (string)target;
                }
                else
                    tm.target = string.Empty;

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

            byte mtype = quest.Want<byte>("mtype");
            string attrs = quest.Want<string>("attrs");
            long mtime = quest.Want<long>("mtime");

            if (mtype == (byte)MessageType.Chat)
            {
                TranslatedMessage tm = ProcessChatMessage(quest);
                if (tm != null)
                    questProcessor.PushChat(from, to, mid, tm, attrs, mtime);

                return null;
            }

            MessageInfo messageInfo = BuildMessageInfo(quest);
            if (mtype == (byte)MessageType.Audio)
            {
                byte[] audioData = messageInfo.binaryData;

                if (!messageInfo.isBinary)
                    audioData = RTMClient.ConvertStringToByteArray(messageInfo.message);

                questProcessor.PushAudio(from, to, mid, audioData, attrs, mtime);
                return null;
            }

            if (mtype == (byte)MessageType.Cmd)
            {
                questProcessor.PushCmd(from, to, mid, messageInfo.message, attrs, mtime);
            }
            else if (mtype >= 40 && mtype <= 50)
            {
                questProcessor.PushFile(from, to, mtype, mid, messageInfo.message, attrs, mtime);
            }
            else
            {
                if (messageInfo.isBinary)
                    questProcessor.PushMessage(from, to, mtype, mid, messageInfo.binaryData, attrs, mtime);
                else
                    questProcessor.PushMessage(from, to, mtype, mid, messageInfo.message, attrs, mtime);
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

            byte mtype = quest.Want<byte>("mtype");
            string attrs = quest.Want<string>("attrs");
            long mtime = quest.Want<long>("mtime");

            if (mtype == (byte)MessageType.Chat)
            {
                TranslatedMessage tm = ProcessChatMessage(quest);
                if (tm != null)
                    questProcessor.PushGroupChat(from, groupId, mid, tm, attrs, mtime);

                return null;
            }

            MessageInfo messageInfo = BuildMessageInfo(quest);
            if (mtype == (byte)MessageType.Audio)
            {
                byte[] audioData = messageInfo.binaryData;

                if (!messageInfo.isBinary)
                    audioData = RTMClient.ConvertStringToByteArray(messageInfo.message);

                questProcessor.PushGroupAudio(from, groupId, mid, audioData, attrs, mtime);
                return null;
            }

            if (mtype == (byte)MessageType.Cmd)
            {
                questProcessor.PushGroupCmd(from, groupId, mid, messageInfo.message, attrs, mtime);
            }
            else if (mtype >= 40 && mtype <= 50)
            {
                questProcessor.PushGroupFile(from, groupId, mtype, mid, messageInfo.message, attrs, mtime);
            }
            else
            {
                if (messageInfo.isBinary)
                    questProcessor.PushGroupMessage(from, groupId, mtype, mid, messageInfo.binaryData, attrs, mtime);
                else
                    questProcessor.PushGroupMessage(from, groupId, mtype, mid, messageInfo.message, attrs, mtime);
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

            byte mtype = quest.Want<byte>("mtype");
            string attrs = quest.Want<string>("attrs");
            long mtime = quest.Want<long>("mtime");

            if (mtype == (byte)MessageType.Chat)
            {
                TranslatedMessage tm = ProcessChatMessage(quest);
                if (tm != null)
                    questProcessor.PushRoomChat(from, roomId, mid, tm, attrs, mtime);

                return null;
            }

            MessageInfo messageInfo = BuildMessageInfo(quest);
            if (mtype == (byte)MessageType.Audio)
            {
                byte[] audioData = messageInfo.binaryData;

                if (!messageInfo.isBinary)
                    audioData = RTMClient.ConvertStringToByteArray(messageInfo.message);

                questProcessor.PushRoomAudio(from, roomId, mid, audioData, attrs, mtime);
                return null;
            }

            if (mtype == (byte)MessageType.Cmd)
            {
                questProcessor.PushRoomCmd(from, roomId, mid, messageInfo.message, attrs, mtime);
            }
            else if (mtype >= 40 && mtype <= 50)
            {
                questProcessor.PushRoomFile(from, roomId, mtype, mid, messageInfo.message, attrs, mtime);
            }
            else
            {
                if (messageInfo.isBinary)
                    questProcessor.PushRoomMessage(from, roomId, mtype, mid, messageInfo.binaryData, attrs, mtime);
                else
                    questProcessor.PushRoomMessage(from, roomId, mtype, mid, messageInfo.message, attrs, mtime);
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

            byte mtype = quest.Want<byte>("mtype");
            string attrs = quest.Want<string>("attrs");
            long mtime = quest.Want<long>("mtime");

            if (mtype == (byte)MessageType.Chat)
            {
                TranslatedMessage tm = ProcessChatMessage(quest);
                if (tm != null)
                    questProcessor.PushBroadcastChat(from, mid, tm, attrs, mtime);

                return null;
            }

            MessageInfo messageInfo = BuildMessageInfo(quest);
            if (mtype == (byte)MessageType.Audio)
            {
                byte[] audioData = messageInfo.binaryData;

                if (!messageInfo.isBinary)
                    audioData = RTMClient.ConvertStringToByteArray(messageInfo.message);

                questProcessor.PushBroadcastAudio(from, mid, audioData, attrs, mtime);
                return null;
            }

            if (mtype == (byte)MessageType.Cmd)
            {
                questProcessor.PushBroadcastCmd(from, mid, messageInfo.message, attrs, mtime);
            }
            else if (mtype >= 40 && mtype <= 50)
            {
                questProcessor.PushBroadcastFile(from, mtype, mid, messageInfo.message, attrs, mtime);
            }
            else
            {
                if (messageInfo.isBinary)
                    questProcessor.PushBroadcastMessage(from, mtype, mid, messageInfo.binaryData, attrs, mtime);
                else
                    questProcessor.PushBroadcastMessage(from, mtype, mid, messageInfo.message, attrs, mtime);
            }

            return null;
        }
    }
}
