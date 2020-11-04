using System;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //======================[ string message version ]================================//
        private bool InternalSendMessage(long uid, byte mtype, string message, string attrs, MessageIdDelegate callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            long mid = MidGenerator.Gen();

            Quest quest = new Quest("sendmsg");
            quest.Param("to", uid);
            quest.Param("mid", mid);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                //long mtime = 0;
                //if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                //    mtime = answer.Want<long>("mtime");

                callback(mid, errorCode);

            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        private int InternalSendMessage(out long messageId, long uid, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                messageId = 0;
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
            }

            messageId = MidGenerator.Gen();

            Quest quest = new Quest("sendmsg");
            quest.Param("to", uid);
            quest.Param("mid", messageId);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            //mtime = answer.Want<long>("mtime");
            return fpnn.ErrorCode.FPNN_EC_OK;
        }


        private bool InternalSendGroupMessage(long groupId, byte mtype, string message, string attrs, MessageIdDelegate callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            long mid = MidGenerator.Gen();

            Quest quest = new Quest("sendgroupmsg");
            quest.Param("gid", groupId);
            quest.Param("mid", mid);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                //long mtime = 0;
                //if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                //    mtime = answer.Want<long>("mtime");

                callback(mid, errorCode);

            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        private int InternalSendGroupMessage(out long messageId, long groupId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                messageId = 0;
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
            }

            messageId = MidGenerator.Gen();

            Quest quest = new Quest("sendgroupmsg");
            quest.Param("gid", groupId);
            quest.Param("mid", messageId);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            //mtime = answer.Want<long>("mtime");
            return fpnn.ErrorCode.FPNN_EC_OK;
        }


        private bool InternalSendRoomMessage(long roomId, byte mtype, string message, string attrs, MessageIdDelegate callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            long mid = MidGenerator.Gen();

            Quest quest = new Quest("sendroommsg");
            quest.Param("rid", roomId);
            quest.Param("mid", mid);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                //long mtime = 0;
                //if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                //    mtime = answer.Want<long>("mtime");

                callback(mid, errorCode);

            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        private int InternalSendRoomMessage(out long messageId, long roomId, byte mtype, string message, string attrs = "", int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                messageId = 0;
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
            }

            messageId = MidGenerator.Gen();

            Quest quest = new Quest("sendroommsg");
            quest.Param("rid", roomId);
            quest.Param("mid", messageId);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            //mtime = answer.Want<long>("mtime");
            return fpnn.ErrorCode.FPNN_EC_OK;
        }

        //======================[ binary message version ]================================//
        private bool InternalSendMessage(long uid, byte mtype, byte[] message, string attrs, MessageIdDelegate callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            long mid = MidGenerator.Gen();

            Quest quest = new Quest("sendmsg");
            quest.Param("to", uid);
            quest.Param("mid", mid);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                //long mtime = 0;
                //if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                //    mtime = answer.Want<long>("mtime");

                callback(mid, errorCode);

            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        private int InternalSendMessage(out long messageId, long uid, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                messageId = 0;
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
            }

            messageId = MidGenerator.Gen();

            Quest quest = new Quest("sendmsg");
            quest.Param("to", uid);
            quest.Param("mid", messageId);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            //mtime = answer.Want<long>("mtime");
            return fpnn.ErrorCode.FPNN_EC_OK;
        }


        private bool InternalSendGroupMessage(long groupId, byte mtype, byte[] message, string attrs, MessageIdDelegate callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            long mid = MidGenerator.Gen();

            Quest quest = new Quest("sendgroupmsg");
            quest.Param("gid", groupId);
            quest.Param("mid", mid);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                //long mtime = 0;
                //if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                //    mtime = answer.Want<long>("mtime");

                callback(mid, errorCode);

            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        private int InternalSendGroupMessage(out long messageId, long groupId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                messageId = 0;
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
            }

            messageId = MidGenerator.Gen();

            Quest quest = new Quest("sendgroupmsg");
            quest.Param("gid", groupId);
            quest.Param("mid", messageId);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            //mtime = answer.Want<long>("mtime");
            return fpnn.ErrorCode.FPNN_EC_OK;
        }


        private bool InternalSendRoomMessage(long roomId, byte mtype, byte[] message, string attrs, MessageIdDelegate callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            long mid = MidGenerator.Gen();

            Quest quest = new Quest("sendroommsg");
            quest.Param("rid", roomId);
            quest.Param("mid", mid);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {
                //long mtime = 0;
                //if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                //    mtime = answer.Want<long>("mtime");

                callback(mid, errorCode);

            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        private int InternalSendRoomMessage(out long messageId, long roomId, byte mtype, byte[] message, string attrs = "", int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                messageId = 0;
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
            }

            messageId = MidGenerator.Gen();

            Quest quest = new Quest("sendroommsg");
            quest.Param("rid", roomId);
            quest.Param("mid", messageId);
            quest.Param("mtype", mtype);
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            //mtime = answer.Want<long>("mtime");
            return fpnn.ErrorCode.FPNN_EC_OK;
        }
    }
}
