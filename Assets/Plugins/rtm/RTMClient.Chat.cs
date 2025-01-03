﻿using System;
using System.Collections.Generic;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //===========================[ Sending Chat ]=========================//
        public bool SendChat(MessageIdDelegate callback, long uid, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendMessage(uid, (byte)MessageType.Chat, message, attrs, callback, 0, null, checkParams, timeout);
        }

        public int SendChat(out long messageId, long uid, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendMessage(out messageId, out _, uid, (byte)MessageType.Chat, message, attrs, 0, null, checkParams, timeout);
        }

        public bool SendChat(SendMessageDelegate callback, long uid, string message, string attrs = "", string checkParams = null, int timeout = 0)
        { 
            return InternalSendMessage(uid, (byte)MessageType.Chat, message, attrs, callback, 0, null, checkParams, timeout);
        }

        public int SendChat(out long messageId, out long mtime, long uid, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendMessage(out messageId, out mtime, uid, (byte)MessageType.Chat, message, attrs, 0, null, checkParams, timeout);
        }

        public bool SendGroupChat(MessageIdDelegate callback, long groupId, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, (byte)MessageType.Chat, message, attrs, callback, 0, null, checkParams, timeout);
        }

        public int SendGroupChat(out long messageId, long groupId, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendGroupMessage(out messageId, out _, groupId, (byte)MessageType.Chat, message, attrs, 0, null, checkParams, timeout);
        }

        public bool SendGroupChat(SendMessageDelegate callback, long groupId, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, (byte)MessageType.Chat, message, attrs, callback, 0, null, checkParams, timeout);
        }

        public int SendGroupChat(out long messageId, out long mtime, long groupId, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendGroupMessage(out messageId, out mtime, groupId, (byte)MessageType.Chat, message, attrs, 0, null, checkParams, timeout);
        }

        public bool SendRoomChat(MessageIdDelegate callback, long roomId, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, (byte)MessageType.Chat, message, attrs, callback, 0, null, checkParams, timeout);
        }

        public int SendRoomChat(out long messageId, long roomId, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendRoomMessage(out messageId, out _, roomId, (byte)MessageType.Chat, message, attrs, 0, null, checkParams, timeout);
        }

        public bool SendRoomChat(SendMessageDelegate callback, long roomId, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, (byte)MessageType.Chat, message, attrs, callback, 0, null, checkParams, timeout);
        }

        public int SendRoomChat(out long messageId, out long mtime, long roomId, string message, string attrs = "", string checkParams = null, int timeout = 0)
        {
            return InternalSendRoomMessage(out messageId, out mtime, roomId, (byte)MessageType.Chat, message, attrs, 0, null, checkParams, timeout);
        }
        //===========================[ Sending Cmd ]=========================//
        public bool SendCmd(MessageIdDelegate callback, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(uid, (byte)MessageType.Cmd, message, attrs, callback, 0, null, null, timeout);
        }

        public int SendCmd(out long messageId, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(out messageId, out _, uid, (byte)MessageType.Cmd, message, attrs, 0, null, null, timeout);
        }

        public bool SendCmd(SendMessageDelegate callback, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(uid, (byte)MessageType.Cmd, message, attrs, callback, 0, null, null, timeout);
        }

        public int SendCmd(out long messageId, out long mtime, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(out messageId, out mtime, uid, (byte)MessageType.Cmd, message, attrs, 0, null, null, timeout);
        }

        public bool SendGroupCmd(MessageIdDelegate callback, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, (byte)MessageType.Cmd, message, attrs, callback, 0, null, null, timeout);
        }

        public int SendGroupCmd(out long messageId, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(out messageId, out _, groupId, (byte)MessageType.Cmd, message, attrs, 0, null, null, timeout);
        }

        public bool SendGroupCmd(SendMessageDelegate callback, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, (byte)MessageType.Cmd, message, attrs, callback, 0, null, null, timeout);
        }

        public int SendGroupCmd(out long messageId, out long mtime, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(out messageId, out mtime, groupId, (byte)MessageType.Cmd, message, attrs, 0, null, null, timeout);
        }

        public bool SendRoomCmd(MessageIdDelegate callback, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, (byte)MessageType.Cmd, message, attrs, callback, 0, null, null, timeout);
        }

        public int SendRoomCmd(out long messageId, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(out messageId, out _, roomId, (byte)MessageType.Cmd, message, attrs, 0, null, null, timeout);
        }

        public bool SendRoomCmd(SendMessageDelegate callback, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, (byte)MessageType.Cmd, message, attrs, callback, 0, null, null, timeout);
        }

        public int SendRoomCmd(out long messageId, out long mtime, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(out messageId, out mtime, roomId, (byte)MessageType.Cmd, message, attrs, 0, null, null, timeout);
        }
        //===========================[ History Chat (Chat & Cmd & Audio) ]=========================//
        private static readonly List<byte> chatMTypes = new List<byte>
        {
            (byte)MessageType.Chat,
            (byte)MessageType.Cmd,
            (byte)MessageType.ImageFile,
            (byte)MessageType.AudioFile,
            (byte)MessageType.VideoFile,
            (byte)MessageType.NormalFile,
        };

        public bool GetGroupChat(HistoryMessageDelegate callback, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetGroupMessage(callback, groupId, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }

        public int GetGroupChat(out HistoryMessageResult result, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetGroupMessage(out result, groupId, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }
        
        public bool GetGroupChatCount(Action<int, int> callback, long groupId, long beginMsec, long endMsec, int timeout = 0)
        {
            return GetGroupMessageCount(callback, groupId, beginMsec, endMsec, chatMTypes, timeout);
        }
        
        public int GetGroupChatCount(out int count, long groupId, long beginMsec, long endMsec, int timeout = 0)
        {
            return GetGroupMessageCount(out count, groupId, beginMsec, endMsec, chatMTypes, timeout);
        }

        public bool GetGroupChatByMessageId(HistoryMessageDelegate callback, long groupId, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, int timeout = 0)
        {
            return GetGroupMessageByMessageId(callback, groupId, desc, count, messageId, beginMsec, endMsec, chatMTypes, timeout);
        }

        public int GetGroupChatByMessageId(out HistoryMessageResult result, long groupId, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, int timeout = 0)
        {
            return GetGroupMessageByMessageId(out result, groupId, desc, count, messageId, beginMsec, endMsec, chatMTypes, timeout);
        }

        public bool GetRoomChat(HistoryMessageDelegate callback, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetRoomMessage(callback, roomId, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }

        public int GetRoomChat(out HistoryMessageResult result, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetRoomMessage(out result, roomId, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }
        
        public bool GetRoomChatCount(Action<int, int> callback, long roomId, long beginMsec, long endMsec, int timeout = 0)
        {
            return GetRoomMessageCount(callback, roomId, beginMsec, endMsec, chatMTypes, timeout);
        }
        
        public int GetRoomChatCount(out int count, long roomId, long beginMsec, long endMsec, int timeout = 0)
        {
            return GetRoomMessageCount(out count, roomId, beginMsec, endMsec, chatMTypes, timeout);
        }

        public bool GetRoomChatByMessageId(HistoryMessageDelegate callback, long roomId, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, int timeout = 0)
        {
            return GetRoomMessageByMessageId(callback, roomId, desc, count, messageId, beginMsec, endMsec, chatMTypes, timeout);
        }

        public int GetRoomChatByMessageId(out HistoryMessageResult result, long roomId, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, int timeout = 0)
        {
            return GetRoomMessageByMessageId(out result, roomId, desc, count, messageId, beginMsec, endMsec, chatMTypes, timeout);
        }

        public bool GetBroadcastChat(HistoryMessageDelegate callback, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetBroadcastMessage(callback, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }

        public int GetBroadcastChat(out HistoryMessageResult result, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetBroadcastMessage(out result, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }
        
        public bool GetBroadcastChatCount(Action<int, int> callback, long beginMsec, long endMsec, int timeout = 0)
        {
            return GetBroadcastMessageCount(callback, beginMsec, endMsec, chatMTypes, timeout);
        }
        
        public int GetBroadcastChatCount(out int count, long beginMsec, long endMsec, int timeout = 0)
        {
            return GetBroadcastMessageCount(out count, beginMsec, endMsec, chatMTypes, timeout);
        }

        public bool GetBroadcastChatByMessageId(HistoryMessageDelegate callback, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, int timeout = 0)
        {
            return GetBroadcastMessageByMessageId(callback, desc, count, messageId, beginMsec, endMsec, chatMTypes, timeout);
        }

        public int GetBroadcastChatByMessageId(out HistoryMessageResult result, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, int timeout = 0)
        {
            return GetBroadcastMessageByMessageId(out result, desc, count, messageId, beginMsec, endMsec, chatMTypes, timeout);
        }

        public bool GetP2PChat(HistoryMessageDelegate callback, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetP2PMessage(callback, peerUid, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }

        public int GetP2PChat(out HistoryMessageResult result, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetP2PMessage(out result, peerUid, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }
        
        public bool GetP2PChatCount(Action<int, int> callback, long peerUid, long beginMsec, long endMsec, int timeout = 0)
        {
            return GetP2PMessageCount(callback, peerUid, beginMsec, endMsec, chatMTypes, timeout);
        }
        
        public int GetP2PChatCount(out int count, long peerUid, long beginMsec, long endMsec, int timeout = 0)
        {
            return GetP2PMessageCount(out count, peerUid, beginMsec, endMsec, chatMTypes, timeout);
        }

        public bool GetP2PChatByMessageId(HistoryMessageDelegate callback, long peerUid, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, int timeout = 0)
        {
            return GetP2PMessageByMessageId(callback, peerUid, desc, count, messageId, beginMsec, endMsec, chatMTypes, timeout);
        }

        public int GetP2PChatByMessageId(out HistoryMessageResult result, long peerUid, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, int timeout = 0)
        {
            return GetP2PMessageByMessageId(out result, peerUid, desc, count, messageId, beginMsec, endMsec, chatMTypes, timeout);
        }
        //===========================[ Unread Chat ]=========================//
        //-- Action<List<p2p_uid>, List<groupId>, errorCode>
        public bool GetUnread(Action<List<long>, List<long>, int> callback, bool clear = false, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getunread");
            quest.Param("clear", clear);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                List<long> p2pList = null;
                List<long> groupList = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        p2pList = WantLongList(answer, "p2p");
                        groupList = WantLongList(answer, "group");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(p2pList, groupList, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetUnread(out List<long> p2pList, out List<long> groupList, bool clear = false, int timeout = 0)
        {
            p2pList = null;
            groupList = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getunread");
            quest.Param("clear", clear);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                p2pList = WantLongList(answer, "p2p");
                groupList = WantLongList(answer, "group");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        public bool GetUnread(Action<List<long>, List<long>, long, int> callback, bool clear = false, bool gettime = false,  int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, null, 0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getunread");
            quest.Param("clear", clear);
            quest.Param("gettime", gettime);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                List<long> p2pList = null;
                List<long> groupList = null;
                long logoutTime = 0;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        p2pList = WantLongList(answer, "p2p");
                        groupList = WantLongList(answer, "group");
                        logoutTime = answer.Get<long>("logouttime", 0);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(p2pList, groupList, logoutTime, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, 0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetUnread(out List<long> p2pList, out List<long> groupList, out long logoutTime, bool clear = false, bool gettime = false, int timeout = 0)
        {
            p2pList = null;
            groupList = null;
            logoutTime = 0;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getunread");
            quest.Param("clear", clear);
            quest.Param("gettime", gettime);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                p2pList = WantLongList(answer, "p2p");
                groupList = WantLongList(answer, "group");
                logoutTime = answer.Get<long>("logouttime", 0);
                
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Clear Unread ]=========================//
        public bool ClearUnread(DoneDelegate callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("cleanunread");
            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int ClearUnread(int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("cleanunread");
            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Get P2P Unread ]=========================//
        //-- Action<Dictionary<peerUid, unreadCount>, errorCode>
        public bool GetP2PUnread(Action<Dictionary<long, int>, int> callback, HashSet<long> uids, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetP2PUnread(callback, uids, 0, mTypes, timeout);
        }

        public bool GetP2PUnread(Action<Dictionary<long, int>, int> callback, HashSet<long> uids, long startTime, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetP2PUnread((Dictionary<long, int> unreadDictionary, Dictionary<long, long> _, int errorCode) => { callback(unreadDictionary, errorCode); }, uids, startTime, mTypes, timeout);
        }

        //-- Action<Dictionary<peerUid, unreadCount>, Dictionary<peerUid, lastUnreadTimestamp>, errorCode>
        public bool GetP2PUnread(Action<Dictionary<long, int>, Dictionary<long, long>, int> callback, HashSet<long> uids, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetP2PUnread(callback, uids, 0, mTypes, timeout);
        }

        public bool GetP2PUnread(Action<Dictionary<long, int>, Dictionary<long, long>, int> callback, HashSet<long> uids, long startTime, HashSet<byte> mTypes = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getp2punread");
            quest.Param("uids", uids);

            if (startTime > 0)
                quest.Param("mtime", startTime);

            if (mTypes != null && mTypes.Count > 0)
                quest.Param("mtypes", mTypes);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                Dictionary<long, int> unreadDictionary = null;
                Dictionary<long, long> lastUnreadTimestampDictionary = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        unreadDictionary = WantLongIntDictionary(answer, "p2p");
                        lastUnreadTimestampDictionary = WantLongLongDictionary(answer, "ltime");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }

                callback(unreadDictionary, lastUnreadTimestampDictionary, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetP2PUnread(out Dictionary<long, int> unreadDictionary, HashSet<long> uids, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetP2PUnread(out unreadDictionary, uids, 0, mTypes, timeout);
        }

        public int GetP2PUnread(out Dictionary<long, int> unreadDictionary, HashSet<long> uids, long startTime, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetP2PUnread(out unreadDictionary, out _, uids, startTime, mTypes, timeout);
        }

        public int GetP2PUnread(out Dictionary<long, int> unreadDictionary, out Dictionary<long, long> lastUnreadTimestampDictionary, HashSet<long> uids, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetP2PUnread(out unreadDictionary, out lastUnreadTimestampDictionary, uids, 0, mTypes, timeout);
        }

        public int GetP2PUnread(out Dictionary<long, int> unreadDictionary, out Dictionary<long, long> lastUnreadTimestampDictionary, HashSet<long> uids, long startTime, HashSet<byte> mTypes = null, int timeout = 0)
        {
            unreadDictionary = null;
            lastUnreadTimestampDictionary = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getp2punread");
            quest.Param("uids", uids);

            if (startTime > 0)
                quest.Param("mtime", startTime);

            if (mTypes != null && mTypes.Count > 0)
                quest.Param("mtypes", mTypes);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                unreadDictionary = WantLongIntDictionary(answer, "p2p");
                lastUnreadTimestampDictionary = WantLongLongDictionary(answer, "ltime");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Get Group Unread ]=========================//
        //-- Action<Dictionary<groupId, unreadCount>, errorCode>
        public bool GetGroupUnread(Action<Dictionary<long, int>, int> callback, HashSet<long> groupIds, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetGroupUnread(callback, groupIds, 0, mTypes, timeout);
        }

        public bool GetGroupUnread(Action<Dictionary<long, int>, int> callback, HashSet<long> groupIds, long startTime, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetGroupUnread((Dictionary<long, int> unreadDictionary, Dictionary<long, long> _, int errorCode) => { callback(unreadDictionary, errorCode); }, groupIds, startTime, mTypes, timeout);
        }

        //-- Action<Dictionary<groupId, unreadCount>, Dictionary<groupId, lastUnreadTimestamp>, errorCode>
        public bool GetGroupUnread(Action<Dictionary<long, int>, Dictionary<long, long>, int> callback, HashSet<long> groupIds, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetGroupUnread(callback, groupIds, 0, mTypes, timeout);
        }

        public bool GetGroupUnread(Action<Dictionary<long, int>, Dictionary<long, long>, int> callback, HashSet<long> groupIds, long startTime, HashSet<byte> mTypes = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getgroupunread");
            quest.Param("gids", groupIds);

            if (startTime > 0)
                quest.Param("mtime", startTime);

            if (mTypes != null && mTypes.Count > 0)
                quest.Param("mtypes", mTypes);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                Dictionary<long, int> unreadDictionary = null;
                Dictionary<long, long> lastUnreadTimestampDictionary = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        unreadDictionary = WantLongIntDictionary(answer, "group");
                        lastUnreadTimestampDictionary = WantLongLongDictionary(answer, "ltime");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }

                callback(unreadDictionary, lastUnreadTimestampDictionary, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetGroupUnread(out Dictionary<long, int> unreadDictionary, HashSet<long> groupIds, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetGroupUnread(out unreadDictionary, groupIds, 0, mTypes);
        }

        public int GetGroupUnread(out Dictionary<long, int> unreadDictionary, HashSet<long> groupIds, long startTime, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetGroupUnread(out unreadDictionary, out _, groupIds, startTime, mTypes, timeout);
        }

        public int GetGroupUnread(out Dictionary<long, int> unreadDictionary, out Dictionary<long, long> lastUnreadTimestampDictionary, HashSet<long> groupIds, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return GetGroupUnread(out unreadDictionary, out lastUnreadTimestampDictionary, groupIds, 0, mTypes, timeout);
        }

        public int GetGroupUnread(out Dictionary<long, int> unreadDictionary, out Dictionary<long, long> lastUnreadTimestampDictionary, HashSet<long> groupIds, long startTime, HashSet<byte> mTypes = null, int timeout = 0)
        {
            unreadDictionary = null;
            lastUnreadTimestampDictionary = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupunread");
            quest.Param("gids", groupIds);

            if (startTime > 0)
                quest.Param("mtime", startTime);

            if (mTypes != null && mTypes.Count > 0)
                quest.Param("mtypes", mTypes);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                unreadDictionary = WantLongIntDictionary(answer, "group");
                lastUnreadTimestampDictionary = WantLongLongDictionary(answer, "ltime");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Get Session ]=========================//
        //-- Action<List<p2p_uid>, List<groupId>, errorCode>
        public bool GetSession(Action<List<long>, List<long>, int> callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getsession");
            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                List<long> p2pList = null;
                List<long> groupList = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        p2pList = WantLongList(answer, "p2p");
                        groupList = WantLongList(answer, "group");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(p2pList, groupList, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetSession(out List<long> p2pList, out List<long> groupList, int timeout = 0)
        {
            p2pList = null;
            groupList = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getsession");
            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                p2pList = WantLongList(answer, "p2p");
                groupList = WantLongList(answer, "group");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Remove Session ]=========================//
        public bool RemoveSession(DoneDelegate callback, long toUid, bool oneway = false, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("removesession");
            quest.Param("to", toUid);
            quest.Param("oneway", oneway);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int RemoveSession(long toUid, bool oneway = false, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("removesession");
            quest.Param("to", toUid);
            quest.Param("oneway", oneway);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Delete Chat ]=========================//
        //-- toId: peer uid, or groupId, or roomId
        public bool DeleteChat(DoneDelegate callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return DeleteMessage(callback, fromUid, toId, messageId, (byte)messageCategory, timeout);
        }

        public int DeleteChat(long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return DeleteMessage(fromUid, toId, messageId, (byte)messageCategory, timeout);
        }

        //===========================[ Get Chat ]=========================//
        //-- toId: peer uid, or groupId, or roomId
        public bool GetChat(Action<RetrievedMessage, int> callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return GetMessage(callback, fromUid, toId, messageId, (byte)messageCategory, timeout);
        }

        public int GetChat(out RetrievedMessage retrievedMessage, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return GetMessage(out retrievedMessage, fromUid, toId, messageId, (byte)messageCategory, timeout);
        }

        //===========================[ Set Translated Languag ]=========================//
        public bool SetTranslatedLanguage(DoneDelegate callback, TranslateLanguage targetLanguage, int timeout = 0)
        {
            return SetTranslatedLanguage(callback, GetTranslatedLanguage(targetLanguage), timeout);
        }
        private bool SetTranslatedLanguage(DoneDelegate callback, string targetLanguage, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("setlang");
            quest.Param("lang", targetLanguage);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int SetTranslatedLanguage(TranslateLanguage targetLanguage, int timeout = 0)
        {
            return SetTranslatedLanguage(GetTranslatedLanguage(targetLanguage), timeout);
        }
        private int SetTranslatedLanguage(string targetLanguage, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("setlang");
            quest.Param("lang", targetLanguage);
            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Translate ]=========================//
        public enum TranslateType
        {
            Chat,
            Mail
        }

        public enum ProfanityType
        {
            Off,
            Stop,
            Censor
        }

        //-- Action<TranslatedInfo, errorCode>
        [System.Obsolete("Translate with TranslateLanguage sourceLanguage is deprecated, please using Translate with string sourceLanguage instead.")]
        public bool Translate(Action<TranslatedInfo, int> callback, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            return Translate(callback, text, GetTranslatedLanguage(destinationLanguage),
                GetTranslatedLanguage(sourceLanguage), type, profanity, timeout);
        }

        //-- Action<TranslatedInfo, errorCode>
        public bool Translate(Action<TranslatedInfo, int> callback, string text,
            string destinationLanguage, string sourceLanguage = "",
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("translate");
            quest.Param("text", text);
            quest.Param("dst", destinationLanguage);

            if (sourceLanguage.Length > 0)
                quest.Param("src", sourceLanguage);

            if (type == TranslateType.Mail)
                quest.Param("type", "mail");
            else
                quest.Param("type", "chat");

            switch (profanity)
            {
                case ProfanityType.Stop: quest.Param("profanity", "stop"); break;
                case ProfanityType.Censor: quest.Param("profanity", "censor"); break;
                case ProfanityType.Off: quest.Param("profanity", "off"); break;
            }

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                TranslatedInfo tm = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        tm = new TranslatedInfo();
                        tm.sourceLanguage = answer.Want<string>("source");
                        tm.targetLanguage = answer.Want<string>("target");
                        tm.sourceText = answer.Want<string>("sourceText");
                        tm.targetText = answer.Want<string>("targetText");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(tm, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        [System.Obsolete("Translate with TranslateLanguage sourceLanguage is deprecated, please using Translate with string sourceLanguage instead.")]
        public int Translate(out TranslatedInfo translatedinfo, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            return Translate(out translatedinfo, text, GetTranslatedLanguage(destinationLanguage),
                GetTranslatedLanguage(sourceLanguage), type, profanity, timeout);
        }

        public int Translate(out TranslatedInfo translatedinfo, string text,
            string destinationLanguage, string sourceLanguage = "",
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            translatedinfo = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("translate");
            quest.Param("text", text);
            quest.Param("dst", destinationLanguage);

            if (sourceLanguage.Length > 0)
                quest.Param("src", sourceLanguage);

            if (type == TranslateType.Mail)
                quest.Param("type", "mail");
            else
                quest.Param("type", "chat");

            switch (profanity)
            {
                case ProfanityType.Stop: quest.Param("profanity", "stop"); break;
                case ProfanityType.Censor: quest.Param("profanity", "censor"); break;
                case ProfanityType.Off: quest.Param("profanity", "off"); break;
            }

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                translatedinfo = new TranslatedInfo
                {
                    sourceLanguage = answer.Want<string>("source"),
                    targetLanguage = answer.Want<string>("target"),
                    sourceText = answer.Want<string>("sourceText"),
                    targetText = answer.Want<string>("targetText")
                };

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Profanity ]=========================//
        //-- Action<string text, List<string> classification, errorCode>
        [System.Obsolete("Profanity is deprecated, please use TCheck instead.")]
        public bool Profanity(Action<string, List<string>, int> callback, string text, bool classify = false, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(string.Empty, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("profanity");
            quest.Param("text", text);
            quest.Param("classify", classify);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                string resultText = "";
                List<string> classification = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        resultText = answer.Want<string>("text");
                        classification = GetStringList(answer, "classification");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(resultText, classification, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(string.Empty, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        [System.Obsolete("Profanity is deprecated, please use TCheck instead.")]
        public int Profanity(out string resultText, out List<string> classification, string text, bool classify = false, int timeout = 0)
        {
            resultText = "";
            classification = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("profanity");
            quest.Param("text", text);
            quest.Param("classify", classify);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                resultText = answer.Want<string>("text");
                classification = GetStringList(answer, "classification");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ SpeechToText ]=========================//
        //-------- url version ----------//
        //-- Action<string text, string language, errorCode>
        //public bool SpeechToText(Action<string, string, int> callback, string audioUrl, string language, string codec = null, int sampleRate = 0, int timeout = 120)
        //{
        //    TCPClient client = GetCoreClient();
        //    if (client == null)
        //    {
        //        if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
        //            ClientEngine.RunTask(() =>
        //            {
        //                callback(string.Empty, string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
        //            });

        //        return false;
        //    }

        //    Quest quest = new Quest("speech2text");
        //    quest.Param("audio", audioUrl);
        //    quest.Param("type", 1);
        //    quest.Param("lang", language);

        //    if (codec != null && codec.Length > 0)
        //        quest.Param("codec", codec);

        //    if (sampleRate > 0)
        //        quest.Param("srate", sampleRate);

        //    bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

        //        string text = "";
        //        string resultLanguage = "";

        //        if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
        //        {
        //            try
        //            {
        //                text = answer.Want<string>("text");
        //                resultLanguage = answer.Want<string>("lang");
        //            }
        //            catch (Exception)
        //            {
        //                errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
        //            }
        //        }
        //        callback(text, resultLanguage, errorCode);
        //    }, timeout);

        //    if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
        //        ClientEngine.RunTask(() =>
        //        {
        //            callback(string.Empty, string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
        //        });

        //    return asyncStarted;
        //}

        //public int SpeechToText(out string resultText, out string resultLanguage, string audioUrl, string language, string codec = null, int sampleRate = 0, int timeout = 120)
        //{
        //    resultText = "";
        //    resultLanguage = "";

        //    TCPClient client = GetCoreClient();
        //    if (client == null)
        //        return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

        //    Quest quest = new Quest("speech2text");
        //    quest.Param("audio", audioUrl);
        //    quest.Param("type", 1);
        //    quest.Param("lang", language);

        //    if (codec != null && codec.Length > 0)
        //        quest.Param("codec", codec);

        //    if (sampleRate > 0)
        //        quest.Param("srate", sampleRate);

        //    Answer answer = client.SendQuest(quest, timeout);

        //    if (answer.IsException())
        //        return answer.ErrorCode();

        //    try
        //    {
        //        resultText = answer.Want<string>("text");
        //        resultLanguage = answer.Want<string>("lang");

        //        return fpnn.ErrorCode.FPNN_EC_OK;
        //    }
        //    catch (Exception)
        //    {
        //        return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
        //    }
        //}

        //-------- binary version ----------//

        public bool SpeechToText(Action<string, string, int> callback, byte[] audioBinaryContent, string language, string codec = null, int sampleRate = 0, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(string.Empty, string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("speech2text");
            quest.Param("audio", audioBinaryContent);
            quest.Param("type", 2);
            quest.Param("lang", language);

            if (codec != null && codec.Length > 0)
                quest.Param("codec", codec);

            if (sampleRate > 0)
                quest.Param("srate", sampleRate);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                string text = "";
                string resultLanguage = "";

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        text = answer.Want<string>("text");
                        resultLanguage = answer.Want<string>("lang");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(text, resultLanguage, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(string.Empty, string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int SpeechToText(out string resultText, out string resultLanguage, byte[] audioBinaryContent, string language, string codec = null, int sampleRate = 0, int timeout = 120)
        {
            resultText = "";
            resultLanguage = "";

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("speech2text");
            quest.Param("audio", audioBinaryContent);
            quest.Param("type", 2);
            quest.Param("lang", language);

            if (codec != null && codec.Length > 0)
                quest.Param("codec", codec);

            if (sampleRate > 0)
                quest.Param("srate", sampleRate);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                resultText = answer.Want<string>("text");
                resultLanguage = answer.Want<string>("lang");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ SpeechTranslate ]=========================//
        public bool SpeechTranslate(Action<TranslatedInfo, int> callback, byte[] audioBinaryContent, string speechLanguage, string textLanguage = null, string codec = null, int sampleRate = 0, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("speechtranslation");
            quest.Param("audio", audioBinaryContent);
            quest.Param("speechLanguageCode", speechLanguage);
            if (textLanguage != null && textLanguage.Length > 0)
                quest.Param("textLanguageCode", textLanguage);

            if (codec != null && codec.Length > 0)
                quest.Param("codec", codec);

            if (sampleRate > 0)
                quest.Param("srate", sampleRate);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                TranslatedInfo translatedInfo = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        translatedInfo = new TranslatedInfo();
                        translatedInfo.sourceLanguage = answer.Want<string>("source");
                        translatedInfo.targetLanguage = answer.Want<string>("target");
                        translatedInfo.sourceText = answer.Want<string>("sourceText");
                        translatedInfo.targetText = answer.Want<string>("targetText");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(translatedInfo, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int SpeechTranslate(out TranslatedInfo translatedInfo, byte[] audioBinaryContent, string speechLanguage, string textLanguage = null, string codec = null, int sampleRate = 0, int timeout = 120)
        {
            translatedInfo = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("speechtranslation");
            quest.Param("audio", audioBinaryContent);
            quest.Param("speechLanguageCode", speechLanguage);
            if (textLanguage != null && textLanguage.Length > 0)
                quest.Param("textLanguageCode", textLanguage);

            if (codec != null && codec.Length > 0)
                quest.Param("codec", codec);

            if (sampleRate > 0)
                quest.Param("srate", sampleRate);

            if (codec != null && codec.Length > 0)
                quest.Param("codec", codec);

            if (sampleRate > 0)
                quest.Param("srate", sampleRate);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                translatedInfo = new TranslatedInfo();
                translatedInfo.sourceLanguage = answer.Want<string>("source");
                translatedInfo.targetLanguage = answer.Want<string>("target");
                translatedInfo.sourceText = answer.Want<string>("sourceText");
                translatedInfo.targetText = answer.Want<string>("targetText");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ TextCheck ]=========================//
        //-- Action<TextCheckResult result, errorCode>
        public bool TextCheck(Action<TextCheckResult, int> callback, string text, string strategyId = null, string checkParams = null, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(new TextCheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            if (strategyId != null && checkParams != null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(new TextCheckResult(), com.fpnn.rtm.ErrorCode.RTM_EC_INVALID_PARAMETER);
                    });

                return false;
            }

            Quest quest = new Quest("tcheck");
            quest.Param("text", text);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);
            if (checkParams != null)
                quest.Param("checkParams", checkParams);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                TextCheckResult result = new TextCheckResult();

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        result.result = answer.Want<int>("result");
                        result.text = answer.Get<string>("text", null);
                        result.tags = GetIntList(answer, "tags");
                        result.wlist = GetStringList(answer, "wlist");
                        result.language = answer.Get<string>("language", null);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(result, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(new TextCheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int TextCheck(out TextCheckResult result, string text, string strategyId = null, string checkParams = null, int timeout = 120)
        {
            result = new TextCheckResult();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            if (strategyId != null && checkParams != null)
                return com.fpnn.rtm.ErrorCode.RTM_EC_INVALID_PARAMETER;

            Quest quest = new Quest("tcheck");
            quest.Param("text", text);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);
            if (checkParams != null)
                quest.Param("checkParams", checkParams);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result.result = answer.Want<int>("result");
                result.text = answer.Get<string>("text", null);
                result.tags = GetIntList(answer, "tags");
                result.wlist = GetStringList(answer, "wlist");
                result.language = answer.Get<string>("language", null);

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ ImageCheck ]=========================//
        //-------- url version ----------//
        public bool ImageCheck(Action<CheckResult, int> callback, string imageUrl, string strategyId = null, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("icheck");
            quest.Param("image", imageUrl);
            quest.Param("type", 1);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);


            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                CheckResult result = new CheckResult();

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        result.result = answer.Want<int>("result");
                        result.tags = GetIntList(answer, "tags");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(result, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int ImageCheck(out CheckResult result, string imageUrl, string strategyId = null, int timeout = 120)
        {
            result = new CheckResult();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("icheck");
            quest.Param("image", imageUrl);
            quest.Param("type", 1);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);


            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result.result = answer.Want<int>("result");
                result.tags = GetIntList(answer, "tags");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //-------- binary version ----------//
        public bool ImageCheck(Action<CheckResult, int> callback, byte[] imageContent, string strategyId = null, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("icheck");
            quest.Param("image", imageContent);
            quest.Param("type", 2);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);


            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                CheckResult result = new CheckResult();

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        result.result = answer.Want<int>("result");
                        result.tags = GetIntList(answer, "tags");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(result, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int ImageCheck(out CheckResult result, byte[] imageContent, string strategyId = null, int timeout = 120)
        {
            result = new CheckResult();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("icheck");
            quest.Param("image", imageContent);
            quest.Param("type", 2);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);


            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result.result = answer.Want<int>("result");
                result.tags = GetIntList(answer, "tags");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ AudioCheck ]=========================//

        //-------- url version ----------//
        public bool AudioCheck(Action<CheckResult, int> callback, string audioUrl, string language, string codec = null, int sampleRate = 0, string strategyId = null, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("acheck");
            quest.Param("audio", audioUrl);
            quest.Param("type", 1);
            quest.Param("lang", language);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);

            if (codec != null && codec.Length > 0)
                quest.Param("codec", codec);

            if (sampleRate > 0)
                quest.Param("srate", sampleRate);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                CheckResult result = new CheckResult();

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        result.result = answer.Want<int>("result");
                        result.tags = GetIntList(answer, "tags");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(result, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int AudioCheck(out CheckResult result, string audioUrl, string language, string codec = null, int sampleRate = 0, string strategyId = null, int timeout = 120)
        {
            result = new CheckResult();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("acheck");
            quest.Param("audio", audioUrl);
            quest.Param("type", 1);
            quest.Param("lang", language);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);

            if (codec != null && codec.Length > 0)
                quest.Param("codec", codec);

            if (sampleRate > 0)
                quest.Param("srate", sampleRate);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result.result = answer.Want<int>("result");
                result.tags = GetIntList(answer, "tags");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //-------- binary version ----------//
        public bool AudioCheck(Action<CheckResult, int> callback, byte[] audioContent, string language, string codec = null, int sampleRate = 0, string strategyId = null, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("acheck");
            quest.Param("audio", audioContent);
            quest.Param("type", 2);
            quest.Param("lang", language);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);

            if (codec != null && codec.Length > 0)
                quest.Param("codec", codec);

            if (sampleRate > 0)
                quest.Param("srate", sampleRate);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                CheckResult result = new CheckResult();

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        result.result = answer.Want<int>("result");
                        result.tags = GetIntList(answer, "tags");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(result, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int AudioCheck(out CheckResult result, byte[] audioContent, string language, string codec = null, int sampleRate = 0, string strategyId = null, int timeout = 120)
        {
            result = new CheckResult();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("acheck");
            quest.Param("audio", audioContent);
            quest.Param("type", 2);
            quest.Param("lang", language);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);

            if (codec != null && codec.Length > 0)
                quest.Param("codec", codec);

            if (sampleRate > 0)
                quest.Param("srate", sampleRate);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result.result = answer.Want<int>("result");
                result.tags = GetIntList(answer, "tags");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ VideoCheck ]=========================//

        //-------- url version ----------//
        public bool VideoCheck(Action<CheckResult, int> callback, string videoUrl, string videoName, string strategyId = null, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("vcheck");
            quest.Param("video", videoUrl);
            quest.Param("type", 1);
            quest.Param("videoName", videoName);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);


            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                CheckResult result = new CheckResult();

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        result.result = answer.Want<int>("result");
                        result.tags = GetIntList(answer, "tags");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(result, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int VideoCheck(out CheckResult result, string videoUrl, string videoName, string strategyId = null, int timeout = 120)
        {
            result = new CheckResult();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("vcheck");
            quest.Param("video", videoUrl);
            quest.Param("type", 1);
            quest.Param("videoName", videoName);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result.result = answer.Want<int>("result");
                result.tags = GetIntList(answer, "tags");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //-------- binary version ----------//
        public bool VideoCheck(Action<CheckResult, int> callback, byte[] videoContent, string videoName, string strategyId = null, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("vcheck");
            quest.Param("video", videoContent);
            quest.Param("type", 2);
            quest.Param("videoName", videoName);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);


            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                CheckResult result = new CheckResult();

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        result.result = answer.Want<int>("result");
                        result.tags = GetIntList(answer, "tags");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(result, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(new CheckResult(), fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int VideoCheck(out CheckResult result, byte[] videoContent, string videoName, string strategyId = null, int timeout = 120)
        {
            result = new CheckResult();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("vcheck");
            quest.Param("video", videoContent);
            quest.Param("type", 2);
            quest.Param("videoName", videoName);
            if (strategyId != null)
                quest.Param("strategyId", strategyId);


            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result.result = answer.Want<int>("result");
                result.tags = GetIntList(answer, "tags");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }
    }
}