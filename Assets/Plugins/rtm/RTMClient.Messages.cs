using System;
using System.Collections.Generic;
using System.Text;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //===========================[ Sending String Messages ]=========================//
        /**
         * mtype MUST large than 50, else this interface will return false.
         */
        public bool SendMessage(MessageIdDelegate callback, long uid, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendMesage interface require mtype large than 50, current mtype is " + mtype);

                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, ErrorCode.RTM_EC_INVALID_MTYPE);
                    });

                return false;
            }

            return InternalSendMessage(uid, mtype, message, attrs, callback, timeout);
        }

        public int SendMessage(out long messageId, long uid, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            messageId = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendMesage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendMessage(out messageId, uid, mtype, message, attrs, timeout);
        }


        public bool SendGroupMessage(MessageIdDelegate callback, long groupId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendGroupMessage interface require mtype large than 50, current mtype is " + mtype);

                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, ErrorCode.RTM_EC_INVALID_MTYPE);
                    });

                return false;
            }

            return InternalSendGroupMessage(groupId, mtype, message, attrs, callback, timeout);
        }

        public int SendGroupMessage(out long messageId, long groupId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            messageId = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendGroupMessage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendGroupMessage(out messageId, groupId, mtype, message, attrs, timeout);
        }


        public bool SendRoomMessage(MessageIdDelegate callback, long roomId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendRoomMessage interface require mtype large than 50, current mtype is " + mtype);

                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, ErrorCode.RTM_EC_INVALID_MTYPE);
                    });

                return false;
            }

            return InternalSendRoomMessage(roomId, mtype, message, attrs, callback, timeout);
        }

        public int SendRoomMessage(out long messageId, long roomId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            messageId = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendRoomMessage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendRoomMessage(out messageId, roomId, mtype, message, attrs, timeout);
        }

        //===========================[ Sending Binary Messages ]=========================//
        /**
         * mtype MUST large than 50, else this interface will return false.
         */
        public bool SendMessage(MessageIdDelegate callback, long uid, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendMesage interface require mtype large than 50, current mtype is " + mtype);

                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, ErrorCode.RTM_EC_INVALID_MTYPE);
                    });

                return false;
            }

            return InternalSendMessage(uid, mtype, message, attrs, callback, timeout);
        }

        public int SendMessage(out long messageId, long uid, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            messageId = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendMesage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendMessage(out messageId, uid, mtype, message, attrs, timeout);
        }


        public bool SendGroupMessage(MessageIdDelegate callback, long groupId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendGroupMessage interface require mtype large than 50, current mtype is " + mtype);

                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, ErrorCode.RTM_EC_INVALID_MTYPE);
                    });

                return false;
            }

            return InternalSendGroupMessage(groupId, mtype, message, attrs, callback, timeout);
        }

        public int SendGroupMessage(out long messageId, long groupId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            messageId = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendGroupMessage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendGroupMessage(out messageId, groupId, mtype, message, attrs, timeout);
        }


        public bool SendRoomMessage(MessageIdDelegate callback, long roomId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendRoomMessage interface require mtype large than 50, current mtype is " + mtype);

                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, ErrorCode.RTM_EC_INVALID_MTYPE);
                    });

                return false;
            }

            return InternalSendRoomMessage(roomId, mtype, message, attrs, callback, timeout);
        }

        public int SendRoomMessage(out long messageId, long roomId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            messageId = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendRoomMessage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendRoomMessage(out messageId, roomId, mtype, message, attrs, timeout);
        }

        //===========================[ Messages Utilities ]=========================//
        internal static bool CheckBinaryType(object obj)
        {
            string typeFullName = obj.GetType().FullName;
            int idx = typeFullName.IndexOf('`');
            if (idx != -1)
            {
                typeFullName = typeFullName.Substring(0, idx);
            }

            return typeFullName == "System.Byte[]";
        }

        /*
        internal static byte[] ConvertStringToByteArray(string data)
        {
            //-- Please refer com.fpnn.msgpack.MsgPacker::UnpackString(...)

            UTF8Encoding utf8Encoding = new UTF8Encoding(false, true);     //-- NO BOM.
            return utf8Encoding.GetBytes(data);
        }
        */

        //===========================[ History Messages Utilities ]=========================//
        private HistoryMessageResult BuildHistoryMessageResult(long toId, Answer answer)
        {
            HistoryMessageResult result = new HistoryMessageResult();
            result.count = answer.Want<int>("num");
            result.lastCursorId = answer.Want<long>("lastid");
            result.beginMsec = answer.Want<long>("begin");
            result.endMsec = answer.Want<long>("end");
            result.messages = new List<HistoryMessage>();

            List<object> messages = (List<object>)answer.Want("msgs");
            foreach (List<object> items in messages)
            {
                bool deleted = (bool)Convert.ChangeType(items[4], TypeCode.Boolean);
                if (deleted)
                    continue;

                HistoryMessage message = new HistoryMessage();
                message.cursorId = (long)Convert.ChangeType(items[0], TypeCode.Int64);
                message.fromUid = (long)Convert.ChangeType(items[1], TypeCode.Int64);
                message.toId = toId;
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

                result.messages.Add(message);
            }
            result.count = result.messages.Count;
            return result;
        }

        private void AdjustHistoryMessageResultForP2PMessage(long selfUid, long peerUid, HistoryMessageResult result)
        {
            foreach (HistoryMessage hm in result.messages)
            {
                if (hm.fromUid == 1)
                {
                    hm.fromUid = selfUid;
                    hm.toId = peerUid;
                }
                else if (hm.fromUid == 2)
                {
                    hm.fromUid = peerUid;
                    hm.toId = selfUid;
                }
            }
        }

        //===========================[ History Messages ]=========================//
        //-------------[ Group History Messages ]---------------------//
        public bool GetGroupMessage(HistoryMessageDelegate callback, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, 0, 0, 0, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getgroupmsg");
            quest.Param("gid", groupId);
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    HistoryMessageResult result;
                    try
                    {
                        result = BuildHistoryMessageResult(groupId, answer);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                        result = new HistoryMessageResult();
                    }
                    callback(result.count, result.lastCursorId, result.beginMsec, result.endMsec, result.messages, errorCode);
                }
                else
                    callback(0, 0, 0, 0, null, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, 0, 0, 0, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetGroupMessage(out HistoryMessageResult result, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            result = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupmsg");
            quest.Param("gid", groupId);
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result = BuildHistoryMessageResult(groupId, answer);
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //-------------[ Room History Messages ]---------------------//
        public bool GetRoomMessage(HistoryMessageDelegate callback, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, 0, 0, 0, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getroommsg");
            quest.Param("rid", roomId);
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    HistoryMessageResult result;
                    try
                    {
                        result = BuildHistoryMessageResult(roomId, answer);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                        result = new HistoryMessageResult();
                    }
                    callback(result.count, result.lastCursorId, result.beginMsec, result.endMsec, result.messages, errorCode);
                }
                else
                    callback(0, 0, 0, 0, null, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, 0, 0, 0, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetRoomMessage(out HistoryMessageResult result, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            result = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getroommsg");
            quest.Param("rid", roomId);
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result = BuildHistoryMessageResult(roomId, answer);
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //-------------[ Broadcast History Messages ]---------------------//
        public bool GetBroadcastMessage(HistoryMessageDelegate callback, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, 0, 0, 0, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getbroadcastmsg");
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    HistoryMessageResult result;
                    try
                    {
                        result = BuildHistoryMessageResult(0, answer);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                        result = new HistoryMessageResult();
                    }
                    callback(result.count, result.lastCursorId, result.beginMsec, result.endMsec, result.messages, errorCode);
                }
                else
                    callback(0, 0, 0, 0, null, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, 0, 0, 0, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetBroadcastMessage(out HistoryMessageResult result, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            result = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getbroadcastmsg");
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result = BuildHistoryMessageResult(0, answer);
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //-------------[ P2P History Messages ]---------------------//
        public bool GetP2PMessage(HistoryMessageDelegate callback, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, 0, 0, 0, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getp2pmsg");
            quest.Param("ouid", peerUid);
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    HistoryMessageResult result;
                    try
                    {
                        result = BuildHistoryMessageResult(0, answer);
                        AdjustHistoryMessageResultForP2PMessage(uid, peerUid, result);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                        result = new HistoryMessageResult();
                    }
                    callback(result.count, result.lastCursorId, result.beginMsec, result.endMsec, result.messages, errorCode);
                }
                else
                    callback(0, 0, 0, 0, null, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, 0, 0, 0, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetP2PMessage(out HistoryMessageResult result, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            result = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getp2pmsg");
            quest.Param("ouid", peerUid);
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                result = BuildHistoryMessageResult(0, answer);
                AdjustHistoryMessageResultForP2PMessage(uid, peerUid, result);
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //-------------[ Delete Messages ]---------------------//
        public bool DeleteMessage(DoneDelegate callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return DeleteMessage(callback, fromUid, toId, messageId, (byte)messageCategory, timeout);
        }

        //-- toId: peer uid, or groupId, or roomId
        //-- type: 1: p2p, 2: group; 3: room
        //-- Obsolete in v.2.2.0, will change public to internal.
        internal bool DeleteMessage(DoneDelegate callback, long fromUid, long toId, long messageId, int type, int timeout = 0)
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

            Quest quest = new Quest("delmsg");
            quest.Param("from", fromUid);
            quest.Param("mid", messageId);
            quest.Param("xid", toId);
            quest.Param("type", type);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int DeleteMessage(long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return DeleteMessage(fromUid, toId, messageId, (byte)messageCategory, timeout);
        }
        //-- Obsolete in v.2.2.0, will change public to internal.
        internal int DeleteMessage(long fromUid, long toId, long messageId, int type, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("delmsg");
            quest.Param("from", fromUid);
            quest.Param("mid", messageId);
            quest.Param("xid", toId);
            quest.Param("type", type);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //-------------[ Get Messages ]---------------------//
        private RetrievedMessage BuildRetrievedMessage(Answer answer)
        {
            RetrievedMessage message = new RetrievedMessage();
            message.cursorId = answer.Want<long>("id");
            message.messageType = answer.Want<byte>("mtype");
            message.attrs = answer.Want<string>("attrs");
            message.modifiedTime = answer.Want<long>("mtime");

            object originalMessage = answer.Want("msg");

            if (!CheckBinaryType(originalMessage))
                message.stringMessage = (string)Convert.ChangeType(originalMessage, TypeCode.String);
            else
                message.binaryMessage = (byte[])originalMessage;

            if (message.messageType >= 40 && message.messageType <= 50)
                RTMClient.BuildFileInfo(message, errorRecorder);

            return message;
        }

        public bool GetMessage(Action<RetrievedMessage, int> callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return GetMessage(callback, fromUid, toId, messageId, (byte)messageCategory, timeout);
        }

        //-- toId: peer uid, or groupId, or roomId
        //-- type: 1: p2p, 2: group; 3: room
        internal bool GetMessage(Action<RetrievedMessage, int> callback, long fromUid, long toId, long messageId, int type, int timeout = 0)
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

            Quest quest = new Quest("getmsg");
            quest.Param("from", fromUid);
            quest.Param("mid", messageId);
            quest.Param("xid", toId);
            quest.Param("type", type);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    RetrievedMessage retrievedMessage = null;
                    try
                    {
                        retrievedMessage = BuildRetrievedMessage(answer);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                    callback(retrievedMessage, errorCode);
                }
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetMessage(out RetrievedMessage retrievedMessage, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return GetMessage(out retrievedMessage, fromUid, toId, messageId, (byte)messageCategory, timeout);
        }
        internal int GetMessage(out RetrievedMessage retrievedMessage, long fromUid, long toId, long messageId, int type, int timeout = 0)
        {
            retrievedMessage = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getmsg");
            quest.Param("from", fromUid);
            quest.Param("mid", messageId);
            quest.Param("xid", toId);
            quest.Param("type", type);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                retrievedMessage = BuildRetrievedMessage(answer);
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }
    }
}
