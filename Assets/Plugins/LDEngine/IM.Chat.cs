using System.Collections;
using System.Collections.Generic;
using com.fpnn.common;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class IM
    {
        public bool SendChatMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, string message, string attrs = "", int timeout = 0)
        {
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendChat((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, message, attrs, null, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupChat((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, message, attrs, null, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            {
                return client.SendRoomChat((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, message, attrs, null, timeout);
            }
            else
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(0, rtm.ErrorCode.RTM_EC_INVALID_PARAMETER);
                });
                return false;
            }
        }

        static List<byte> chatMtype = new List<byte>(){(byte)MessageType.Chat};

        public bool GetHitoryChatMessage(HistoryMessageDelegate callback, MessageCategory messageCategory, long id, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.GetP2PMessage((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, id, desc, count, beginMsec, endMsec, lastId, chatMtype, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.GetGroupMessage((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, id, desc, count, beginMsec, endMsec, lastId, chatMtype, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            {
                return client.GetRoomMessage((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, id, desc, count, beginMsec, endMsec, lastId, chatMtype, timeout);
            }
            else if (messageCategory == MessageCategory.BroadcastMessage)
            {
                return client.GetBroadcastMessage((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, desc, count, beginMsec, endMsec, lastId, chatMtype, timeout);
            }
            else
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(0, 0, 0, 0, null, rtm.ErrorCode.RTM_EC_INVALID_PARAMETER);
                });
                return false;
            }
        }

        public bool GetHitoryChatMessageByMessageId(HistoryMessageDelegate callback, MessageCategory messageCategory, long id, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, List<byte> mtypes = null, int timeout = 0)
        {
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.GetP2PMessageByMessageId((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, id, desc, count, messageId, beginMsec, endMsec, chatMtype, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.GetGroupMessageByMessageId((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, id, desc, count, messageId, beginMsec, endMsec, chatMtype, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            {
                return client.GetRoomMessageByMessageId((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, id, desc, count, messageId, beginMsec, endMsec, chatMtype, timeout);
            }
            else if (messageCategory == MessageCategory.BroadcastMessage)
            {
                return client.GetBroadcastMessageByMessageId((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, desc, count, messageId, beginMsec, endMsec, chatMtype, timeout);
            }
            else
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(0, 0, 0, 0, null, rtm.ErrorCode.RTM_EC_INVALID_PARAMETER);
                });
                return false;
            }
        }
	}
}

