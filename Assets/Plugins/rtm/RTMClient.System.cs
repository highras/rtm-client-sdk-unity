using System;
using System.Collections.Generic;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        public void Bye(bool async = true)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                Close();
                return;
            }

            lock (interLocker)
            {
                if (autoReloginInfo != null)
                    autoReloginInfo.Disable();
            }

            Quest quest =  new Quest("bye");
            if (async)
            {
                bool success = client.SendQuest(quest, (Answer answer, int errorCode) => { Close(); });
                if (!success)
                    Close();
            }
            else
            {
                client.SendQuest(quest);
                Close();
            }
        }

        //===========================[ Add Attributes ]=========================//
        public bool AddAttributes(DoneDelegate callback, Dictionary<string, string> attrs, int timeout = 0)
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

            Quest quest = new Quest("addattrs");
            quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int AddAttributes(Dictionary<string, string> attrs, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("addattrs");
            quest.Param("attrs", attrs);
            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Get Attributes ]=========================//
        //-- Action<attributes, errorCode>
        public bool GetAttributes(Action<Dictionary<string, string>, int> callback, int timeout = 0)
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

            Quest quest = new Quest("getattrs");
            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                Dictionary<string, string> result = null;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        result = WantStringDictionary(answer, "attrs");
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
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetAttributes(out Dictionary<string, string> attributes, int timeout = 0)
        {
            attributes = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getattrs");
            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                attributes = WantStringDictionary(answer, "attrs");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Add Debug Log ]=========================//
        public bool AddDebugLog(DoneDelegate callback, string message, string attrs, int timeout = 0)
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

            Quest quest = new Quest("adddebuglog");
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int AddDebugLog(string message, string attrs, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("adddebuglog");
            quest.Param("msg", message);
            quest.Param("attrs", attrs);
            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Add Device ]=========================//
        public bool AddDevice(DoneDelegate callback, string appType, string deviceToken, int timeout = 0)
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

            Quest quest = new Quest("adddevice");
            quest.Param("apptype", appType);
            quest.Param("devicetoken", deviceToken);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int AddDevice(string appType, string deviceToken, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("adddevice");
            quest.Param("apptype", appType);
            quest.Param("devicetoken", deviceToken);
            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Remove Device ]=========================//
        public bool RemoveDevice(DoneDelegate callback, string deviceToken, int timeout = 0)
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

            Quest quest = new Quest("removedevice");
            quest.Param("devicetoken", deviceToken);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int RemoveDevice(string deviceToken, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("removedevice");
            quest.Param("devicetoken", deviceToken);
            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Add Device Push Option ]=========================//
        public bool AddDevicePushOption(DoneDelegate callback, MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            byte type = 99;
            switch (messageCategory)
            {
                case MessageCategory.P2PMessage:
                    type = 0; break;
                case MessageCategory.GroupMessage:
                    type = 1; break;
            }

            return AddDevicePushOption(callback, type, targetId,  mTypes, timeout);
        }

        internal bool AddDevicePushOption(DoneDelegate callback, byte type, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            if (type > 1)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(ErrorCode.RTM_EC_INVALID_PARAMETER);
                    });

                return false;
            }

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

            Quest quest = new Quest("addoption");
            quest.Param("type", type);
            quest.Param("xid", targetId);

            if (mTypes != null)
                quest.Param("mtypes", mTypes);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int AddDevicePushOption(MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            byte type = 99;
            switch (messageCategory)
            {
                case MessageCategory.P2PMessage:
                    type = 0; break;
                case MessageCategory.GroupMessage:
                    type = 1; break;
            }

            return AddDevicePushOption(type, targetId, mTypes, timeout);
        }

        internal int AddDevicePushOption(byte type, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            if (type > 1)
                return ErrorCode.RTM_EC_INVALID_PARAMETER;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("addoption");
            quest.Param("type", type);
            quest.Param("xid", targetId);

            if (mTypes != null)
                quest.Param("mtypes", mTypes);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Remove Device Push Option ]=========================//
        public bool RemoveDevicePushOption(DoneDelegate callback, MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            byte type = 99;
            switch (messageCategory)
            {
                case MessageCategory.P2PMessage:
                    type = 0; break;
                case MessageCategory.GroupMessage:
                    type = 1; break;
            }

            return RemoveDevicePushOption(callback, type, targetId, mTypes, timeout);
        }

        internal bool RemoveDevicePushOption(DoneDelegate callback, byte type, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            if (type > 1)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(ErrorCode.RTM_EC_INVALID_PARAMETER);
                    });

                return false;
            }

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

            Quest quest = new Quest("removeoption");
            quest.Param("type", type);
            quest.Param("xid", targetId);

            if (mTypes != null)
                quest.Param("mtypes", mTypes);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int RemoveDevicePushOption(MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            byte type = 99;
            switch (messageCategory)
            {
                case MessageCategory.P2PMessage:
                    type = 0; break;
                case MessageCategory.GroupMessage:
                    type = 1; break;
            }

            return RemoveDevicePushOption(type, targetId, mTypes, timeout);
        }

        internal int RemoveDevicePushOption(byte type, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            if (type > 1)
                return ErrorCode.RTM_EC_INVALID_PARAMETER;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("removeoption");
            quest.Param("type", type);
            quest.Param("xid", targetId);

            if (mTypes != null)
                quest.Param("mtypes", mTypes);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Get Device Push Option ]=========================//
        //-- Utilities functions
        private Dictionary<long, HashSet<byte>> WantLongByteHashSetDictionary(Message message, string key)
        {
            Dictionary <long, HashSet<byte>> rev = new Dictionary<long, HashSet<byte>>();

            Dictionary<object, object> originalDict = (Dictionary<object, object>)message.Want(key);
            foreach (KeyValuePair<object, object> kvp in originalDict)
            {
                List<object> originalList = (List<object>)(kvp.Value);
                HashSet<byte> resultSet = new HashSet<byte>();

                foreach (object obj in originalList)
                {
                    resultSet.Add((byte)Convert.ChangeType(obj, TypeCode.Byte));
                }
                
                rev.Add((long)Convert.ChangeType(kvp.Key, TypeCode.Int64), resultSet);
            }

            return rev;
        }

        //-- Action<Dictionary<p2p_uid，HashSet<mType>>, Dictionary<groupId, HashSet<mType>>, errorCode>
        public bool GetDevicePushOption(Action<Dictionary<long, HashSet<byte>>, Dictionary<long, HashSet<byte>>, int> callback, int timeout = 0)
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

            Quest quest = new Quest("getoption");
            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                Dictionary<long, HashSet<byte>> p2pDictionary = null;
                Dictionary<long, HashSet<byte>> groupDictionary = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        p2pDictionary = WantLongByteHashSetDictionary(answer, "p2p");
                        groupDictionary = WantLongByteHashSetDictionary(answer, "group");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(p2pDictionary, groupDictionary, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetDevicePushOption(out Dictionary<long, HashSet<byte>> p2pDictionary, out Dictionary<long, HashSet<byte>> groupDictionary, int timeout = 0)
        {
            p2pDictionary = null;
            groupDictionary = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getoption");
            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                p2pDictionary = WantLongByteHashSetDictionary(answer, "p2p");
                groupDictionary = WantLongByteHashSetDictionary(answer, "group");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }
    }
}
