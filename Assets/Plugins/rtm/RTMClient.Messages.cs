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
        public bool SendMessage(ActTimeDelegate callback, long uid, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendMesage interface require mtype large than 50, current mtype is " + mtype);

                return false;
            }

            return InternalSendMessage(uid, mtype, message, attrs, callback, timeout);
        }

        public int SendMessage(out long mtime, long uid, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            mtime = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendMesage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendMessage(out mtime, uid, mtype, message, attrs, timeout);
        }


        public bool SendGroupMessage(ActTimeDelegate callback, long groupId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendGroupMessage interface require mtype large than 50, current mtype is " + mtype);

                return false;
            }

            return InternalSendGroupMessage(groupId, mtype, message, attrs, callback, timeout);
        }

        public int SendGroupMessage(out long mtime, long groupId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            mtime = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendGroupMessage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendGroupMessage(out mtime, groupId, mtype, message, attrs, timeout);
        }


        public bool SendRoomMessage(ActTimeDelegate callback, long roomId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendRoomMessage interface require mtype large than 50, current mtype is " + mtype);

                return false;
            }

            return InternalSendRoomMessage(roomId, mtype, message, attrs, callback, timeout);
        }

        public int SendRoomMessage(out long mtime, long roomId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            mtime = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendRoomMessage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendRoomMessage(out mtime, roomId, mtype, message, attrs, timeout);
        }

        //===========================[ Sending Binary Messages ]=========================//
        /**
         * mtype MUST large than 50, else this interface will return false.
         */
        public bool SendMessage(ActTimeDelegate callback, long uid, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendMesage interface require mtype large than 50, current mtype is " + mtype);

                return false;
            }

            return InternalSendMessage(uid, mtype, message, attrs, callback, timeout);
        }

        public int SendMessage(out long mtime, long uid, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            mtime = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendMesage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendMessage(out mtime, uid, mtype, message, attrs, timeout);
        }


        public bool SendGroupMessage(ActTimeDelegate callback, long groupId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendGroupMessage interface require mtype large than 50, current mtype is " + mtype);

                return false;
            }

            return InternalSendGroupMessage(groupId, mtype, message, attrs, callback, timeout);
        }

        public int SendGroupMessage(out long mtime, long groupId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            mtime = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendGroupMessage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendGroupMessage(out mtime, groupId, mtype, message, attrs, timeout);
        }


        public bool SendRoomMessage(ActTimeDelegate callback, long roomId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendRoomMessage interface require mtype large than 50, current mtype is " + mtype);

                return false;
            }

            return InternalSendRoomMessage(roomId, mtype, message, attrs, callback, timeout);
        }

        public int SendRoomMessage(out long mtime, long roomId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            mtime = 0;

            if (mtype <= 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("SendRoomMessage interface require mtype large than 50, current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            return InternalSendRoomMessage(out mtime, roomId, mtype, message, attrs, timeout);
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

        internal static byte[] ConvertStringToByteArray(string data)
        {
            //-- Please refer com.fpnn.msgpack.MsgPacker::UnpackString(...)

            UTF8Encoding utf8Encoding = new UTF8Encoding(false, true);     //-- NO BOM.
            return utf8Encoding.GetBytes(data);
        }

        //===========================[ History Messages Utilities ]=========================//
        private HistoryMessageResult BuildHistoryMessageResult(Answer answer)
        {
            HistoryMessageResult result = new HistoryMessageResult();
            result.count = answer.Want<int>("num");
            result.lastId = answer.Want<long>("lastid");
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
                message.id = (long)Convert.ChangeType(items[0], TypeCode.Int64);
                message.fromUid = (long)Convert.ChangeType(items[1], TypeCode.Int64);
                message.mtype = (byte)Convert.ChangeType(items[2], TypeCode.Byte);
                message.mid = (long)Convert.ChangeType(items[3], TypeCode.Int64);

                if (message.mtype != MessageMType_Audio)
                {

                    if (CheckBinaryType(items[5]))
                        message.binaryMessage = (byte[])items[5];
                    else
                        message.binaryMessage = ConvertStringToByteArray((string)Convert.ChangeType(items[5], TypeCode.String));
                }
                else
                {
                    if (!CheckBinaryType(items[5]))
                        message.stringMessage = (string)Convert.ChangeType(items[5], TypeCode.String);
                    else
                        message.binaryMessage = (byte[])items[5];
                }

                message.attrs = (string)Convert.ChangeType(items[6], TypeCode.String);
                message.mtime = (long)Convert.ChangeType(items[7], TypeCode.Int64);

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
                    hm.fromUid = selfUid;
                else if (hm.fromUid == 2)
                    hm.fromUid = peerUid;
            }
        }

        //===========================[ History Messages ]=========================//
        //-------------[ Group History Messages ]---------------------//
        public bool GetGroupMessage(HistoryMessageDelegate callback, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("getgroupmsg");
            quest.Param("gid", groupId);
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);
            
            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    HistoryMessageResult result;
                    try
                    {
                        result = BuildHistoryMessageResult(answer);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                        result = new HistoryMessageResult();
                    }
                    callback(result.count, result.lastId, result.beginMsec, result.endMsec, result.messages, errorCode);
                }
                
                callback(0, 0, 0, 0, null, errorCode);
            }, timeout);
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
                result = BuildHistoryMessageResult(answer);
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
                return false;

            Quest quest = new Quest("getroommsg");
            quest.Param("rid", roomId);
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    HistoryMessageResult result;
                    try
                    {
                        result = BuildHistoryMessageResult(answer);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                        result = new HistoryMessageResult();
                    }
                    callback(result.count, result.lastId, result.beginMsec, result.endMsec, result.messages, errorCode);
                }

                callback(0, 0, 0, 0, null, errorCode);
            }, timeout);
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
                result = BuildHistoryMessageResult(answer);
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
                return false;

            Quest quest = new Quest("getbroadcastmsg");
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    HistoryMessageResult result;
                    try
                    {
                        result = BuildHistoryMessageResult(answer);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                        result = new HistoryMessageResult();
                    }
                    callback(result.count, result.lastId, result.beginMsec, result.endMsec, result.messages, errorCode);
                }

                callback(0, 0, 0, 0, null, errorCode);
            }, timeout);
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
                result = BuildHistoryMessageResult(answer);
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
                return false;

            Quest quest = new Quest("getp2pmsg");
            quest.Param("ouid", peerUid);
            quest.Param("desc", desc);
            quest.Param("num", count);

            quest.Param("begin", beginMsec);
            quest.Param("end", endMsec);
            quest.Param("lastid", lastId);

            if (mtypes != null)
                quest.Param("mtypes", mtypes);

            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    HistoryMessageResult result;
                    try
                    {
                        result = BuildHistoryMessageResult(answer);
                        AdjustHistoryMessageResultForP2PMessage(uid, peerUid, result);
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                        result = new HistoryMessageResult();
                    }
                    callback(result.count, result.lastId, result.beginMsec, result.endMsec, result.messages, errorCode);
                }

                callback(0, 0, 0, 0, null, errorCode);
            }, timeout);
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
                result = BuildHistoryMessageResult(answer);
                AdjustHistoryMessageResultForP2PMessage(uid, peerUid, result);
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //-------------[ Delete Messages ]---------------------//
        //-- xid: peer uid, or groupId, or roomId
        //-- type: 1: p2p, 2: group; 3: room
        public bool DeleteMessage(DoneDelegate callback, long xid, long mid, int type, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("delmsg");
            quest.Param("mid", mid);
            quest.Param("xid", xid);
            quest.Param("type", type);

            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
        }

        public int DeleteMessage(long xid, long mid, int type, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("delmsg");
            quest.Param("mid", mid);
            quest.Param("xid", xid);
            quest.Param("type", type);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //-------------[ Get Messages ]---------------------//
        private RetrievedMessage BuildRetrievedMessage(Answer answer)
        {
            RetrievedMessage message = new RetrievedMessage();
            message.id = answer.Want<long>("id");
            message.mtype = answer.Want<byte>("mtype");
            message.attrs = answer.Want<string>("attrs");
            message.mtime = answer.Want<long>("mtime");

            object originalMessage = answer.Want("msg");

            if (message.mtype != MessageMType_Audio)
            {

                if (CheckBinaryType(originalMessage))
                    message.binaryMessage = (byte[])originalMessage;
                else
                    message.binaryMessage = ConvertStringToByteArray((string)Convert.ChangeType(originalMessage, TypeCode.String));
            }
            else
            {
                if (!CheckBinaryType(originalMessage))
                    message.stringMessage = (string)Convert.ChangeType(originalMessage, TypeCode.String);
                else
                    message.binaryMessage = (byte[])originalMessage;
            }

            return message;
        }

        //-- xid: peer uid, or groupId, or roomId
        //-- type: 1: p2p, 2: group; 3: room
        public bool GetMessage(Action<RetrievedMessage, int> callback, long xid, long mid, int type, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("getmsg");
            quest.Param("mid", mid);
            quest.Param("xid", xid);
            quest.Param("type", type);

            return client.SendQuest(quest, (Answer answer, int errorCode) => {
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
        }

        public int GetMessage(out RetrievedMessage retrievedMessage, long xid, long mid, int type, int timeout = 0)
        {
            retrievedMessage = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getmsg");
            quest.Param("mid", mid);
            quest.Param("xid", xid);
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
