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

        //===========================[ Kickout ]=========================//
        public bool Kickout(DoneDelegate callback, string endpoint, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("kickout");
            quest.Param("ce", endpoint);

            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
        }

        public int Kickout(string endpoint, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("kickout");
            quest.Param("ce", endpoint);
            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Add Attributes ]=========================//
        public bool AddAttributes(DoneDelegate callback, Dictionary<string, string> attrs, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("addattrs");
            quest.Param("attrs", attrs);

            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
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
        private List<Dictionary<string, string>> ConvertGetAttributesAnswer(Answer answer)
        {
            List<Dictionary<string, string>> attributes = new List<Dictionary<string, string>>();
            List<object> attrsList = answer.Want<List<object>>("attrs");

            foreach (object obj in attrsList)
            {
                Dictionary<object, object> originalDict = (Dictionary<object, object>)obj;
                Dictionary<string, string> attrDict = new Dictionary<string, string>();

                foreach (KeyValuePair<object, object> kvp in originalDict)
                {
                    attrDict.Add((string)kvp.Key, (string)kvp.Value);
                }
                if (attrDict.Count > 0)
                    attributes.Add(attrDict);
            }

            return attributes;
        }

        //-- Action<attributes, errorCode>
        public bool GetAttributes(Action<List<Dictionary<string, string>>, int> callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("getattrs");
            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                List < Dictionary<string, string> > result = null;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    { result = ConvertGetAttributesAnswer(answer); }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(result, errorCode);
            }, timeout);
        }

        public int GetAttributes(out List<Dictionary<string, string>> attributes, int timeout = 0)
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
                attributes = ConvertGetAttributesAnswer(answer);
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
                return false;

            Quest quest = new Quest("adddebuglog");
            quest.Param("msg", message);
            quest.Param("attrs", attrs);

            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
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
                return false;

            Quest quest = new Quest("adddevice");
            quest.Param("apptype", appType);
            quest.Param("devicetoken", deviceToken);

            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
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
                return false;

            Quest quest = new Quest("removedevice");
            quest.Param("devicetoken", deviceToken);

            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
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
    }
}
