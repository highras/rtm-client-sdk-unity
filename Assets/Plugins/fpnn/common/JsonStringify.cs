using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace com.fpnn.common
{
    internal static class JsonStringEscaper
    {
        public delegate void CharEscape(StringBuilder sb, char c);

        private static readonly Dictionary<char, CharEscape> _charEscapeDict;

        static JsonStringEscaper()
        {
            _charEscapeDict = new Dictionary<char, CharEscape>
            {
                { '\\', Slash },
                { '"', QuotationMarks },
                { '\b', SpecialChars },
                { '\f', SpecialChars },
                { '\n', SpecialChars },
                { '\r', SpecialChars },
                { '\t', SpecialChars },
            };
        }

        static public void Escape(StringBuilder sb, string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (_charEscapeDict.TryGetValue(c, out CharEscape escapeFunc))
                {
                    escapeFunc(sb, c);
                }
                else
                {
                    ushort value = Convert.ToUInt16(c);

                    if (value > 0x1f && value < 0x7f)       //-- ASCII visible chars
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append("\\u");
                        sb.Append(value.ToString("x4"));
                    }
                }
            }
        }

        static private void SpecialChars(StringBuilder sb, char c)
        {
            switch (c)
            {
                case '\b': sb.Append("\\b"); break;
                case '\f': sb.Append("\\f"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
            }
        }

        static private void Slash(StringBuilder sb, char c)
        {
            sb.Append("\\\\");
        }

        static private void QuotationMarks(StringBuilder sb, char c)
        {
            sb.Append("\\\"");
        }
    }

    //============================[ Exception ]============================//

    public class UnsupportedTypeException : JsonException
    {
        public UnsupportedTypeException(string message) : base(message) { }

        static public UnsupportedTypeException Create(Object obj)
        {
            string typeFullName = obj.GetType().FullName;
            return new UnsupportedTypeException("FPJson unsupported type: " + typeFullName);
        }

        static public UnsupportedTypeException DictionaryKey(Object obj)
        {
            string typeFullName = obj.GetType().FullName;
            return new UnsupportedTypeException("FPJson unsupported object key type: " + typeFullName);
        }
    }

    //============================[ Json Stringify ]============================//

    internal class JsonStringify
    {
        public delegate void Serialize(JsonStringify js, object obj);

        private static readonly Dictionary<string, Serialize> _serializeDict;

        static JsonStringify()
        {
            _serializeDict = new Dictionary<string, Serialize>
            {
                { "System.Boolean", SerializeBoolean },

                { "System.Decimal", SerializeDecimal },
                { "System.Double", SerializeDouble },
                { "System.Single", SerializeFloat },

                { "System.SByte", SerializeInteger },
                { "System.Int16", SerializeInteger },
                { "System.Int32", SerializeInteger },
                { "System.Int64", SerializeInteger },

                { "System.Byte", SerializeUInteger },
                { "System.Char", SerializeUInteger },
                { "System.UInt16", SerializeUInteger },
                { "System.UInt32", SerializeUInteger },
                { "System.UInt64", SerializeUInteger },

                { "System.String", SerializeString },
                { "System.Tuple", SerializeTuple },
                { "System.DateTime", SerializeTimestamp }
            };
        }

        //-- Instance Methods

        private StringBuilder sb;

        public JsonStringify()
        {
            sb = new StringBuilder();
        }

        public string Stringify(object obj)
        {
            if (obj == null)
            {
                SerializeNull(this);
                return sb.ToString();
            }

            string typeFullName = obj.GetType().FullName;
            int idx = typeFullName.IndexOf('`');
            if (idx != -1)
            {
                typeFullName = typeFullName.Substring(0, idx);
            }

            if (_serializeDict.TryGetValue(typeFullName, out Serialize serializer))
            {
                serializer(this, obj);
            }
            else if (obj is IEnumerable)
            {
                SerializeIEnumerable(this, obj);
            }
            else
            {
                throw UnsupportedTypeException.Create(obj);
            }

            return sb.ToString();
        }

        static private void SerializeBoolean(JsonStringify js, object obj)
        {
            Boolean v = (bool)obj;
            js.sb.Append(v);
        }

        static private void SerializeDecimal(JsonStringify js, object obj)
        {
            Decimal dec = (Decimal)obj;
            js.sb.Append(dec);
        }

        static private void SerializeDouble(JsonStringify js, object obj)
        {
            double value = (double)obj;
            js.sb.Append(value);
        }

        static private void SerializeFloat(JsonStringify js, object obj)
        {
            float value = (float)obj;
            js.sb.Append(value);
        }

        static private void SerializeInteger(JsonStringify js, object obj)
        {
            Int64 value = (Int64)Convert.ChangeType(obj, TypeCode.Int64);
            js.sb.Append(value);
        }

        static private void SerializeUInteger(JsonStringify js, object obj)
        {
            UInt64 value = (UInt64)Convert.ChangeType(obj, TypeCode.UInt64);
            js.sb.Append(value);
        }

        static private void SerializeString(JsonStringify js, object obj)
        {
            string str = (string)obj;

            js.sb.Append("\"");
            JsonStringEscaper.Escape(js.sb, str);
            js.sb.Append("\"");
        }

        static private void SerializeTuple(JsonStringify js, object obj)
        {
            Type objType = obj.GetType();
            var props = objType.GetProperties();

            bool isFirst = true;
            js.sb.Append("[");

            foreach (System.Reflection.PropertyInfo prop in props)
            {
                if (isFirst)
                    isFirst = false;
                else
                    js.sb.Append(",");

                object o = prop.GetValue(obj, null);
                js.Stringify(o);
            }

            js.sb.Append("]");
        }
        static private void SerializeTimestamp(JsonStringify js, object obj)
        {
            DateTime userDate = (DateTime)obj;
            var univDateTime = userDate.ToUniversalTime();

            js.sb.Append("\"");
            js.sb.Append(univDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            js.sb.Append("\"");
        }

        static private void SerializeNull(JsonStringify js)
        {
            js.sb.Append("null");
        }

        static private void SerializeArray(JsonStringify js, object obj)
        {
            IEnumerable ie = (IEnumerable)obj;
            IEnumerator it = ie.GetEnumerator();
            it.Reset();

            bool isFirst = true;
            js.sb.Append("[");

            while (it.MoveNext())
            {
                if (isFirst)
                    isFirst = false;
                else
                    js.sb.Append(",");

                object o = it.Current;
                js.Stringify(o);
            }

            js.sb.Append("]");
        }

        static private void SerializeDictionary(JsonStringify js, object obj)
        {
            IEnumerable ie = (IEnumerable)obj;
            IEnumerator it = ie.GetEnumerator();
            it.Reset();

            bool isFirst = true;
            js.sb.Append("{");

            IDictionaryEnumerator id = (IDictionaryEnumerator)it;
            while (it.MoveNext())
            {
                if (isFirst)
                    isFirst = false;
                else
                    js.sb.Append(",");

                object k = id.Key;
                if (Type.GetTypeCode(k.GetType()) != TypeCode.String)
                    throw UnsupportedTypeException.DictionaryKey(k);

                js.Stringify(k);

                js.sb.Append(":");

                object v = id.Value;
                js.Stringify(v);
            }

            js.sb.Append("}");
        }

        static private void SerializeIEnumerable(JsonStringify js, object obj)
        {
            IEnumerable ie = (IEnumerable)obj;
            IEnumerator it = ie.GetEnumerator();
            it.Reset();

            if (it is IDictionaryEnumerator)
                SerializeDictionary(js, obj);
            else
                SerializeArray(js, obj);
        }
    }
}
