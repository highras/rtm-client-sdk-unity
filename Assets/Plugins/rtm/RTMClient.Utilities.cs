using System;
using System.Collections.Generic;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        internal static string GetTranslatedLanguage(TranslateLanguage language)
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
    }
}
