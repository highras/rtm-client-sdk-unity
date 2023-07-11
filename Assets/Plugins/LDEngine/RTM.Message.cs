using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class RTM
    {
        public bool SendBasicMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendMessage((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, mtype, message, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupMessage((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, mtype, message, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            {
                return client.SendGroupMessage((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, mtype, message, attrs, timeout);
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

        public bool SendBinaryMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendMessage((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, mtype, message, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupMessage((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, mtype, message, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            {
                return client.SendGroupMessage((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, mtype, message, attrs, timeout);
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

        public bool GetHitoryBasicMessage(HistoryMessageDelegate callback, MessageCategory messageCategory, long id, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.GetP2PMessage((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, id, desc, count, beginMsec, endMsec, lastId, mtypes, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.GetGroupMessage((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, id, desc, count, beginMsec, endMsec, lastId, mtypes, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            {
                return client.GetRoomMessage((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, id, desc, count, beginMsec, endMsec, lastId, mtypes, timeout);
            }
            else if (messageCategory == MessageCategory.BroadcastMessage)
            {
                return client.GetBroadcastMessage((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(count, lastCursorId, beginMsec, endMsec, messages, errorCode);
                    });
                }, desc, count, beginMsec, endMsec, lastId, mtypes, timeout);
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