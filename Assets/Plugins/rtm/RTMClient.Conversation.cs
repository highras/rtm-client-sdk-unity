using System;
using System.Collections.Generic;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        private List<Conversation> BuildP2PConversationList(List<long> conversations, List<int> unreads, List<object> messages)
        {
            List<Conversation> conversationList = new List<Conversation>();
            int i = 0;
            foreach (List<object> items in messages)
            {
                bool deleted = (bool)Convert.ChangeType(items[4], TypeCode.Boolean);
                if (deleted)
                {
                    i += 1;
                    continue;
                }

                HistoryMessage message = new HistoryMessage();
                message.cursorId = (long)Convert.ChangeType(items[0], TypeCode.Int64);
                if (message.cursorId != 0)
                {
                    long direction = (long)Convert.ChangeType(items[1], TypeCode.Int64);
                    if (direction == 1)
                    {
                        message.fromUid = Uid;
                        message.toId = conversations[i];
                    }
                    else
                    {
                        message.fromUid = conversations[i];
                        message.toId = Uid;
                    }
                    message.messageType = (byte)Convert.ChangeType(items[2], TypeCode.Byte);
                    message.messageId = (long)Convert.ChangeType(items[3], TypeCode.Int64);

                    if (!CheckBinaryType(items[5]))
                        message.stringMessage = (string)Convert.ChangeType(items[5], TypeCode.String);
                    else
                        message.binaryMessage = (byte[])items[5];

                    message.attrs = (string)Convert.ChangeType(items[6], TypeCode.String);
                    message.modifiedTime = (long)Convert.ChangeType(items[7], TypeCode.Int64);

                    if (message.messageType >= 40 && message.messageType <= 50)
                        RTMClient.BuildFileInfo(message, errorRecorder);
                }

                Conversation conversation = new Conversation();
                conversation.id = conversations[i];
                conversation.conversationType = ConversationType.P2P;
                conversation.unreadCount = unreads[i];
                conversation.lastMessage = message;
                conversationList.Add(conversation);
                i += 1;
            }
            return conversationList;
        }

        public bool GetP2PConversationList(Action<List<Conversation>, int> callback, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
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

            Quest quest = new Quest("getp2pconversationlist");
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                List<Conversation> conversationList = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> conversations = WantLongList(answer, "conversations");
                        List<int> unreads = GetIntList(answer, "unreads");
                        List<object> messages = (List<object>)answer.Want("msgs");
                        conversationList = BuildP2PConversationList(conversations, unreads, messages);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(conversationList, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetP2PConversationList(out List<Conversation> conversationList, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
        { 
            conversationList = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getp2pconversationlist");
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                List<long> conversations = WantLongList(answer, "conversations");
                List<int> unreads = GetIntList(answer, "unreads");
                List<object> messages = (List<object>)answer.Want("msgs");
                conversationList = BuildP2PConversationList(conversations, unreads, messages);

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        public bool GetP2PUnreadConversationList(Action<List<Conversation>, int> callback, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
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

            Quest quest = new Quest("getp2punreadconversationlist");
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                List<Conversation> conversationList = null;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> conversations = WantLongList(answer, "conversations");
                        List<int> unreads = GetIntList(answer, "unreads");
                        List<object> messages = (List<object>)answer.Want("msgs");
                        conversationList = BuildP2PConversationList(conversations, unreads, messages);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(conversationList, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetP2PUnreadConversationList(out List<Conversation> conversationList, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
        { 
            conversationList = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getp2punreadconversationlist");
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                List<long> conversations = WantLongList(answer, "conversations");
                List<int> unreads = GetIntList(answer, "unreads");
                List<object> messages = (List<object>)answer.Want("msgs");
                conversationList = BuildP2PConversationList(conversations, unreads, messages);

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        private List<Conversation> BuildGroupConversationList(List<long> conversations, List<int> unreads, List<object> messages)
        {
            List<Conversation> conversationList = new List<Conversation>();
            int i = 0;
            foreach (List<object> items in messages)
            {
                bool deleted = (bool)Convert.ChangeType(items[4], TypeCode.Boolean);
                if (deleted)
                {
                    i += 1;
                    continue;
                }

                HistoryMessage message = new HistoryMessage();
                message.cursorId = (long)Convert.ChangeType(items[0], TypeCode.Int64);
                if (message.cursorId != 0)
                {
                    message.fromUid = (long)Convert.ChangeType(items[1], TypeCode.Int64);
                    message.toId = conversations[i];
                    message.messageType = (byte)Convert.ChangeType(items[2], TypeCode.Byte);
                    message.messageId = (long)Convert.ChangeType(items[3], TypeCode.Int64);

                    if (!CheckBinaryType(items[5]))
                        message.stringMessage = (string)Convert.ChangeType(items[5], TypeCode.String);
                    else
                        message.binaryMessage = (byte[])items[5];

                    message.attrs = (string)Convert.ChangeType(items[6], TypeCode.String);
                    message.modifiedTime = (long)Convert.ChangeType(items[7], TypeCode.Int64);

                    if (message.messageType >= 40 && message.messageType <= 50)
                        RTMClient.BuildFileInfo(message, errorRecorder);
                }

                Conversation conversation = new Conversation();
                conversation.id = conversations[i];
                conversation.conversationType = ConversationType.GROUP;
                conversation.unreadCount = unreads[i];
                conversation.lastMessage = message;
                conversationList.Add(conversation);
                i += 1;
            }
            return conversationList;
        }

        public bool GetGroupConversationList(Action<List<Conversation>, int> callback, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
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

            Quest quest = new Quest("getgroupconversationlist");
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                List<Conversation> conversationList = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> conversations = WantLongList(answer, "conversations");
                        List<int> unreads = GetIntList(answer, "unreads");
                        List<object> messages = (List<object>)answer.Want("msgs");
                        conversationList = BuildGroupConversationList(conversations, unreads, messages);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(conversationList, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetGroupConversationList(out List<Conversation> conversationList, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
        { 
            conversationList = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupconversationlist");
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                List<long> conversations = WantLongList(answer, "conversations");
                List<int> unreads = GetIntList(answer, "unreads");
                List<object> messages = (List<object>)answer.Want("msgs");
                conversationList = BuildGroupConversationList(conversations, unreads, messages);

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        public bool GetGroupUnreadConversationList(Action<List<Conversation>, int> callback, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
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

            Quest quest = new Quest("getgroupunreadconversationlist");
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                List<Conversation> conversationList = null;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> conversations = WantLongList(answer, "conversations");
                        List<int> unreads = GetIntList(answer, "unreads");
                        List<object> messages = (List<object>)answer.Want("msgs");
                        conversationList = BuildGroupConversationList(conversations, unreads, messages);
                     }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(conversationList, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetGroupUnreadConversationList(out List<Conversation> conversationList, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
        { 
            conversationList = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupunreadconversationlist");
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                List<long> conversations = WantLongList(answer, "conversations");
                List<int> unreads = GetIntList(answer, "unreads");
                List<object> messages = (List<object>)answer.Want("msgs");
                conversationList = BuildGroupConversationList(conversations, unreads, messages);

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        public bool GetUnreadConversationList(Action<List<Conversation>, List<Conversation>, int> callback, bool clear = true, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
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

            Quest quest = new Quest("getunreadconversationlist");
            quest.Param("clear", clear);
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                List<Conversation> groupConversationList = null;
                List<Conversation> p2pConversationList = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> groupConversations = WantLongList(answer, "groupConversations");
                        List<int> groupUnreads = GetIntList(answer, "groupUnreads");
                        List<object> groupMsgs = (List<object>)answer.Want("groupMsgs");
                        List<long> p2pConversations = WantLongList(answer, "p2pConversations");
                        List<int> p2pUnreads = GetIntList(answer, "p2pUnreads");
                        List<object> p2pMsgs = (List<object>)answer.Want("p2pMsgs");
 
                        groupConversationList = BuildGroupConversationList(groupConversations, groupUnreads, groupMsgs);
                        p2pConversationList = BuildP2PConversationList(p2pConversations, p2pUnreads, p2pMsgs);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(groupConversationList, p2pConversationList, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetUnreadConversationList(out List<Conversation> groupConversationList, out List<Conversation> p2pConversationList, bool clear = true, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0)
        { 
            groupConversationList = null;
            p2pConversationList = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getunreadconversationlist");
            quest.Param("clear", clear);
            if (mTypes != null)
                quest.Param("mtypes", mTypes);
            if (startTime != 0)
                quest.Param("mtime", startTime);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                List<long> groupConversations = WantLongList(answer, "groupConversations");
                List<int> groupUnreads = GetIntList(answer, "groupUnreads");
                List<object> groupMsgs = (List<object>)answer.Want("groupMsgs");
                List<long> p2pConversations = WantLongList(answer, "p2pConversations");
                List<int> p2pUnreads = GetIntList(answer, "p2pUnreads");
                List<object> p2pMsgs = (List<object>)answer.Want("p2pMsgs");

                groupConversationList = BuildGroupConversationList(groupConversations, groupUnreads, groupMsgs);
                p2pConversationList = BuildP2PConversationList(p2pConversations, p2pUnreads, p2pMsgs);

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }
    }
}
