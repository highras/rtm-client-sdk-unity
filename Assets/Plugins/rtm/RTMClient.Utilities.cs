using System;
using System.Collections.Generic;
using com.fpnn.proto;
using com.fpnn.common;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        public static string GetTranslatedLanguage(TranslateLanguage language)
        {
            if (language == TranslateLanguage.None)
                return "";

            if (language == TranslateLanguage.zh_cn)
                return "zh-CN";

            if (language == TranslateLanguage.zh_tw)
                return "zh-TW";

            return language.ToString("G");
        }

        private HashSet<long> WantLongHashSet(Message message, string key)
        {
            HashSet<long> rev = new HashSet<long>();

            List<object> originalList = (List<object>)message.Want(key);
            foreach (object obj in originalList)
                rev.Add((long)Convert.ChangeType(obj, TypeCode.Int64));

            return rev;
        }

        private List<long> WantLongList(Message message, string key)
        {
            List<long> rev = new List<long>();

            List<object> originalList = (List<object>)message.Want(key);
            foreach (object obj in originalList)
                rev.Add((long)Convert.ChangeType(obj, TypeCode.Int64));

            return rev;
        }

        private List<int> GetIntList(Message message, string key)
        {
            List<int> rev = new List<int>();

            List<object> originalList = (List<object>)message.Get(key);
            if (originalList == null)
                return null;

            foreach (object obj in originalList)
                rev.Add((int)Convert.ChangeType(obj, TypeCode.Int32));

            return rev;
        }

        private List<string> GetStringList(Message message, string key)
        {
            List<string> rev = new List<string>();

            List<object> originalList = (List<object>)message.Get(key);
            if (originalList == null)
                return null;

            foreach (object obj in originalList)
                rev.Add((string)Convert.ChangeType(obj, TypeCode.String));

            return rev;
        }

        [System.Obsolete("WantStringDictionary will be deprecated, except another function using it.")]
        private Dictionary<string, string> WantStringDictionary(Message message, string key)
        {
            Dictionary<string, string> rev = new Dictionary<string, string>();

            Dictionary<object, object> originalDict = (Dictionary<object, object>)message.Want(key);
            foreach (KeyValuePair<object, object> kvp in originalDict)
                rev.Add((string)Convert.ChangeType(kvp.Key, TypeCode.String), (string)Convert.ChangeType(kvp.Value, TypeCode.String));

            return rev;
        }

        private Dictionary<long, string> WantLongStringDictionary(Message message, string key)
        {
            Dictionary<long, string> rev = new Dictionary<long, string>();

            Dictionary<object, object> originalDict = (Dictionary<object, object>)message.Want(key);
            foreach (KeyValuePair<object, object> kvp in originalDict)
                rev.Add((long)Convert.ChangeType(kvp.Key, TypeCode.Int64), (string)Convert.ChangeType(kvp.Value, TypeCode.String));

            return rev;
        }

        private Dictionary<long, int> WantLongIntDictionary(Message message, string key)
        {
            Dictionary<long, int> rev = new Dictionary<long, int>();

            Dictionary<object, object> originalDict = (Dictionary<object, object>)message.Want(key);
            foreach (KeyValuePair<object, object> kvp in originalDict)
                rev.Add((long)Convert.ChangeType(kvp.Key, TypeCode.Int64), (int)Convert.ChangeType(kvp.Value, TypeCode.Int32));

            return rev;
        }

        internal static void ParseFileMessage(BaseMessage baseMessage, ErrorRecorder errorRecorder)
        {
            try
            {
                Dictionary<string, object> infoDict = Json.ParseObject(baseMessage.stringMessage);
                if (infoDict != null)
                {
                    if (infoDict.TryGetValue("url", out object urlText))
                        baseMessage.fileInfo.url = (string)urlText;

                    if (infoDict.TryGetValue("size", out object sizeInt))
                        baseMessage.fileInfo.size = (Int32)Convert.ChangeType(sizeInt, TypeCode.Int32);

                    if (baseMessage.messageType == (byte)MessageType.ImageFile)
                    {
                        if (infoDict.TryGetValue("surl", out object surlText))
                            baseMessage.fileInfo.surl = (string)surlText;
                    }

                    baseMessage.stringMessage = null;
                }
            }
            catch (JsonException e)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Parse file msg error. Full msg: " + baseMessage.stringMessage, e);
            }
        }

        internal static void ParseFileAttrs(BaseMessage baseMessage, ErrorRecorder errorRecorder)
        {
            try
            {
                Dictionary<string, object> attrsDict = Json.ParseObject(baseMessage.attrs);
                if (attrsDict != null)
                {
                    if (attrsDict.TryGetValue("rtm", out object rtmAttrs))
                    {
                        Dictionary<string, object> rtmAttrsDict = (Dictionary<string, object>)rtmAttrs;
                        if (rtmAttrsDict.TryGetValue("type", out object typeText))
                        {
                            string typeStr = (string)typeText;
                            if (typeStr.Equals("audiomsg"))
                                baseMessage.fileInfo.isRTMAudio = true;
                        }

                        if (baseMessage.fileInfo.isRTMAudio)
                        {
                            if (rtmAttrsDict.TryGetValue("lang", out object languageText))
                                baseMessage.fileInfo.language = (string)languageText;

                            if (rtmAttrsDict.TryGetValue("duration", out object durationInt))
                                baseMessage.fileInfo.duration = (Int32)Convert.ChangeType(durationInt, TypeCode.Int32);
                        }
                    }

                    if (attrsDict.TryGetValue("custom", out object attrsInfo))
                    {
                        try
                        {
                            Dictionary<string, object> userAttrsDict = (Dictionary<string, object>)attrsInfo;
                            baseMessage.attrs = Json.ToString(userAttrsDict);
                            return;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                string userAttrs = (string)attrsInfo;
                                baseMessage.attrs = userAttrs;
                                return;
                            }
                            catch (Exception ex)
                            {
                                if (errorRecorder != null)
                                    errorRecorder.RecordError("Convert user attrs to string type for file attrs error. Full attrs: " + baseMessage.attrs, ex);
                            }
                        }
                    }
                }
            }
            catch (JsonException e)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Parse file attrs error. Full attrs: " + baseMessage.attrs, e);
            }
        }

        internal static void BuildFileInfo(BaseMessage baseMessage, ErrorRecorder errorRecorder)
        {
            baseMessage.fileInfo = new FileInfo();
            ParseFileMessage(baseMessage, errorRecorder);
            ParseFileAttrs(baseMessage, errorRecorder);
        }
    }
}
