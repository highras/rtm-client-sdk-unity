using System;
using System.Collections.Generic;
using com.fpnn.proto;

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

        private Dictionary<string, string> WantStringDictionary(Message message, string key)
        {
            Dictionary<string, string> rev = new Dictionary<string, string>();

            Dictionary<object, object> originalDict = (Dictionary<object, object>)message.Want(key);
            foreach (KeyValuePair<object, object> kvp in originalDict)
                rev.Add((string)Convert.ChangeType(kvp.Key, TypeCode.String), (string)Convert.ChangeType(kvp.Value, TypeCode.String));

            return rev;
        }

        internal static AudioInfo BuildAudioInfo(string json, common.ErrorRecorder errorRecorder)
        {
            try
            {
                Dictionary<string, object> jsonData = common.Json.ParseObject(json);

                AudioInfo audioInfo = new AudioInfo();

                if (jsonData.TryGetValue("sl", out object sourceLanguage))
                    audioInfo.sourceLanguage = (string)sourceLanguage;
                else
                    audioInfo.sourceLanguage = "";

                if (jsonData.TryGetValue("rl", out object recognizedLanguage))
                    audioInfo.recognizedLanguage = (string)recognizedLanguage;
                else
                    audioInfo.recognizedLanguage = "";

                if (jsonData.TryGetValue("du", out object duration))
                    audioInfo.duration = (int)Convert.ChangeType(duration, typeof(int));
                else
                    audioInfo.duration = 0;

                if (jsonData.TryGetValue("rt", out object recognizedText))
                    audioInfo.recognizedText = (string)recognizedText;
                else
                    audioInfo.recognizedText = "";

                return audioInfo;
            }
            catch (Exception e)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("BuildAudioInfo failed. Json: " + json, e);

                return null;
            }
        }
    }
}
