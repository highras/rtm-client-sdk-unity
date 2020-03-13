using System;
using com.fpnn.proto;
namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //===========================[ Data Get ]=========================//
        //-- Action<value, errorCode>
        public bool DataGet(Action<string, int> callback, string key, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("dataget");
            quest.Param("key", key);

            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                string value = null;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    { value = answer.Get<string>("val", null); }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(value, errorCode);
            }, timeout);
        }

        public int DataGet(out string value, string key, int timeout = 0)
        {
            value = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("dataget");
            quest.Param("key", key);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                value = answer.Get<string>("val", null);
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Data Set ]=========================//
        public bool DataSet(DoneDelegate callback, string key, string value, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("dataset");
            quest.Param("key", key);
            quest.Param("val", value);

            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
        }

        public int DataSet(string key, string value, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("dataset");
            quest.Param("key", key);
            quest.Param("val", value);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Data Delete ]=========================//
        public bool DataDelete(DoneDelegate callback, string key, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("datadel");
            quest.Param("key", key);

            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
        }

        public int DataDelete(string key, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("datadel");
            quest.Param("key", key);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }
    }
}
