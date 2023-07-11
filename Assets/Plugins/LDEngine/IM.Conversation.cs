using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class IM
    {
        public bool GetConversation(Action<List<Conversation>, int> callback, ConversationType conversationType, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
        {
            if (conversationType == ConversationType.P2P)
            {
                return client.GetP2PConversationList((List<Conversation> conversationList, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(conversationList, errorCode);
                    });
                }, mTypes, startTime, timeout);
            }
            else if (conversationType == ConversationType.GROUP)
            {
                return client.GetGroupConversationList((List<Conversation> conversationList, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(conversationList, errorCode);
                    });
                }, mTypes, startTime, timeout);
            }
            else
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(null, rtm.ErrorCode.RTM_EC_INVALID_PARAMETER);
                });
                return false;
            }
        }

        public bool GetAllUnreadConversation(Action<List<Conversation>, List<Conversation>, int> callback, bool clear = true, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
        {
            return client.GetUnreadConversationList((List<Conversation> groupConversationList, List<Conversation> p2pConversationList, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(groupConversationList, p2pConversationList, errorCode);
                });
            }, clear, mTypes, startTime, timeout);
        }

        public bool GetUnreadConversation(Action<List<Conversation>, int> callback, ConversationType conversationType, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
        {
            if (conversationType == ConversationType.P2P)
            {
                return client.GetP2PUnreadConversationList((List<Conversation> conversationList, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(conversationList, errorCode);
                    });
                }, mTypes, startTime, timeout);
            }
            else if (conversationType == ConversationType.GROUP)
            {
                return client.GetGroupUnreadConversationList((List<Conversation> conversationList, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(conversationList, errorCode);
                    });
                }, mTypes, startTime, timeout);
            }
            else
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(null, rtm.ErrorCode.RTM_EC_INVALID_PARAMETER);
                });
                return false;
            }
        }

        public bool ClearUnread(DoneDelegate callback, int timeout = 0)
        {
            return client.ClearUnread((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, timeout);
        }

        public bool RemoveP2PConversation(DoneDelegate callback, long uid, bool oneway = false, int timeout = 0)
        {
            return client.RemoveSession((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, uid, oneway, timeout);
        }
	}
}

