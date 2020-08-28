using System;
using System.Text;
using System.Collections.Generic;

namespace com.fpnn.common
{
    public static class Json
    {
        public static object Parse(string json)
        {
            JsonParser parser = JsonParser.Create(json);
            return parser.Parse();
        }

        public static Dictionary<string, object> ParseObject(string json)
        {
            JsonParser parser = JsonParser.Create(json);
            return parser.ParseObject();
        }

        //-- Placeholder: Maybe implemented in futuer
        //-- public static string ToString(object obj) { return ""; }
    }

    //============================[ Exception ]============================//
    public class JsonException : Exception
    {
        public JsonException(string message) : base(message) { }
        public JsonException(String message, Exception ex) : base(message, ex) { }
    }

    public class InvalidJsonException : JsonException
    {
        public InvalidJsonException(string message) : base(message) { }
        public InvalidJsonException(string message, Exception ex) : base(message, ex) { }
    }

    public class JsonTypeException : JsonException
    {
        public JsonTypeException(string message) : base(message) { }
    }

    //============================[ Json Parser ]============================//

    internal enum JsonElementType
    {
        Empty,
        Null,
        Boolean,
        String,
        Int64,
        UInt64,
        Double,
        Array,
        Dictionary,
    }

    internal delegate void JsonParseSignDelegate(JsonParser parser);

    internal class JsonParser
    {
        class JsonElement
        {
            public object element;
            public JsonElementType type;
        }

        static private readonly Dictionary<char, JsonParseSignDelegate> signDelegateMap;
        static private readonly Dictionary<char, ushort> hexTable;
        static private readonly HashSet<char> numericalChars;

        private readonly string json;
        private int idx;
        private string key;
        private bool wantKey;
        private bool wantValue;
        private bool wantComma;
        private bool wantSemicolon;

        private readonly Stack<JsonElement> elementStack;

        private object parseResult;
        private JsonElementType resultType;

        static JsonParser()
        {
            signDelegateMap = new Dictionary<char, JsonParseSignDelegate>
            {
                { '{', EnterObject },
                { '}', ExitObject },
                { '[', EnterArray },
                { ']', ExitArray },
                { ',', ProcessComma },
                { ':', ProcessSemicolon },
                { '"', ProcessString },
            };

            hexTable = new Dictionary<char, ushort>
            {
                { '0', 0 },
                { '1', 1 },
                { '2', 2 },
                { '3', 3 },
                { '4', 4 },
                { '5', 5 },
                { '6', 6 },
                { '7', 7 },
                { '8', 8 },
                { '9', 9 },

                { 'a', 10 },
                { 'b', 11 },
                { 'c', 12 },
                { 'd', 13 },
                { 'e', 14 },
                { 'f', 15 },

                { 'A', 10 },
                { 'B', 11 },
                { 'C', 12 },
                { 'D', 13 },
                { 'E', 14 },
                { 'F', 15 },
            };

            numericalChars = new HashSet<char>
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                '+', '-', '.', 'E', 'e'
            };
        }

        private static void EnterObject(JsonParser parser)
        {
            parser.EnterObject();
        }
        private static void ExitObject(JsonParser parser)
        {
            parser.ExitObject();
        }
        private static void EnterArray(JsonParser parser)
        {
            parser.EnterArray();
        }
        private static void ExitArray(JsonParser parser)
        {
            parser.ExitArray();
        }
        private static void ProcessComma(JsonParser parser)
        {
            parser.ProcessComma();
        }
        private static void ProcessSemicolon(JsonParser parser)
        {
            parser.ProcessSemicolon();
        }
        private static void ProcessString(JsonParser parser)
        {
            parser.ProcessString();
        }

        public static JsonParser Create(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidJsonException("Json Parser: content error. Null or empty content.");

            JsonParser parser;

            try
            {
                string normalizedString = json;
                if (!json.IsNormalized())
                    normalizedString = json.Normalize();

                parser = new JsonParser(normalizedString);
            }
            catch (ArgumentException e)
            {
                throw new InvalidJsonException("Json Parser: content error. Include invalid Unicode.", e);
            }

            return parser;
        }

        //---------------------[ Instance Parts ]--------------------------//
        private JsonParser(string jsonString)
        {
            json = jsonString;
            idx = 0;

            elementStack = new Stack<JsonElement>();
        }

        private void Reset()
        {
            key = string.Empty;

            wantKey = false;
            wantValue = false;
            wantComma = false;
            wantSemicolon = false;

            elementStack.Clear();

            parseResult = null;
            resultType = JsonElementType.Empty;
        }

        private void ParseCore()
        {
            Reset();

            while (idx < json.Length && parseResult == null)
            {
                if (Char.IsWhiteSpace(json[idx]))
                {
                    idx++;
                    continue;
                }

                if (signDelegateMap.TryGetValue(json[idx], out JsonParseSignDelegate method))
                    method(this);
                else
                    GeneralProcess();
            }
        }

        public object Parse()
        {
            ParseCore();
            return parseResult;
        }


        public Dictionary<string, object> ParseObject()
        {
            ParseCore();
            if (resultType == JsonElementType.Dictionary)
                return (Dictionary<string, object>)parseResult;

            throw new JsonTypeException("Json Parser: want the Dictionary type, but the parsed type is " + resultType.ToString("G") + ".");
        }

        private void FillKey(string value)
        {
            key = value;

            wantKey = false;
            wantValue = false;
            wantComma = false;
            wantSemicolon = true;
        }

        private void FillValue(object obj, JsonElementType type)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)elementStack.Peek().element;
            try
            {
                dict.Add(key, obj);
            }
            catch (ArgumentException e)
            {
                throw new InvalidJsonException("Json Parser: reduplicated key: \"" + key + "\", offset " + idx, e);
            }

            wantKey = false;
            wantValue = false;
            wantComma = true;
            wantSemicolon = false;
        }

        private void InsertNode(object obj, JsonElementType type)
        {
            int stackCount = elementStack.Count;
            if (stackCount > 0)
            {
                if (wantValue)
                    FillValue(obj, type);
                else
                {
                    List<object> list = (List<object>)elementStack.Peek().element;
                    list.Add(obj);

                    wantComma = true;
                }
            }

            if (type == JsonElementType.Array || type == JsonElementType.Dictionary)
            {
                elementStack.Push(new JsonElement()
                {
                    element = obj,
                    type = type
                });
            }
            else if (stackCount == 0)
            {
                parseResult = obj;
                resultType = type;
            }
        }

        private void FinishNode()
        {
            JsonElement element = elementStack.Pop();

            if (elementStack.Count == 0)
            {
                parseResult = element.element;
                resultType = element.type;
            }
            else
            {
                wantKey = false;
                wantValue = false;
                wantComma = true;
                wantSemicolon = false;
            }
        }

        private void ProcessSlash(ref StringBuilder stringBuilder)
        {
            idx++;
            if (idx >= json.Length)
                throw new InvalidJsonException("Json Parser: content error, json truncated after '\\'.");

            switch (json[idx])
            {
                case '"':
                    stringBuilder.Append('"');
                    break;

                case '\\':
                    stringBuilder.Append('\\');
                    break;

                case '/':
                    stringBuilder.Append('/');
                    break;

                case 'b':
                    stringBuilder.Append('\b');
                    break;

                case 'f':
                    stringBuilder.Append('\f');
                    break;

                case 'n':
                    stringBuilder.Append('\n');
                    break;

                case 'r':
                    stringBuilder.Append('\r');
                    break;

                case 't':
                    stringBuilder.Append('\t');
                    break;

                case 'u':
                    if (idx + 5 >= json.Length)
                        throw new InvalidJsonException("Json Parser: content error, json truncated after '\\u'.");

                    idx++;
                    ushort value = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        if (hexTable.TryGetValue(json[idx+i], out ushort v))
                        {
                            value <<= 4;
                            value += v;
                        }
                        else
                            throw new InvalidJsonException("Json Parser: content error, invalid hex number for '\\u'. Offset " + idx);
                    }

                    try
                    {
                        char c = Convert.ToChar(value);
                        stringBuilder.Append(c);
                    }
                    catch (OverflowException e)
                    {
                        throw new InvalidJsonException("Json Parser: content error, invalid unicode value for '\\u'. Offset " + idx, e);
                    }
                    
                    idx += 4;
                    return;
            }

            idx++;
        }

        private string FetchString()
        {
            idx++;
            int startIdx = idx;
            StringBuilder stringBuilder = new StringBuilder();

            while (idx < json.Length)
            {
                if (json[idx] == '\\')
                {
                    int count = idx - startIdx;
                    if (count > 0)
                        stringBuilder.Append(json, startIdx, count);

                    ProcessSlash(ref stringBuilder);
                    startIdx = idx;
                    continue;
                }

                if (json[idx] == '"')
                {
                    int count = idx - startIdx;
                    if (count > 0)
                        stringBuilder.Append(json, startIdx, count);

                    idx++;
                    return stringBuilder.ToString();
                }

                idx++;
            }

            throw new InvalidJsonException("Json Parser: content error, json truncated in string value.");
        }

        private void EnterObject()
        {
            if (wantKey || wantSemicolon || wantComma)
                throw new InvalidJsonException("Json Parser: content error, '{' at improper place. Offset " + idx);

            InsertNode(new Dictionary<string, object>(), JsonElementType.Dictionary);

            wantKey = true;
            wantValue = false;
            wantComma = false;
            wantSemicolon = false;

            idx++;
        }

        private void ExitObject()
        {
            if (wantValue || wantSemicolon || elementStack.Count == 0 || elementStack.Peek().type != JsonElementType.Dictionary)
                throw new InvalidJsonException("Json Parser: content error, '}' at improper place. Offset " + idx);

            idx++;
            FinishNode();
        }

        private void EnterArray()
        {
            if (wantKey || wantSemicolon || wantComma)
                throw new InvalidJsonException("Json Parser: content error, '[' at improper place. Offset " + idx);

            InsertNode(new List<object>(), JsonElementType.Array);

            wantKey = false;
            wantValue = false;
            wantComma = false;
            wantSemicolon = false;

            idx++;
        }

        private void ExitArray()
        {
            if (wantValue || wantSemicolon || elementStack.Count == 0 || elementStack.Peek().type != JsonElementType.Array)
                throw new InvalidJsonException("Json Parser: content error, ']' at improper place. Offset " + idx);

            idx++;
            FinishNode();
        }

        private void ProcessComma()
        {
            if (!wantComma)
                throw new InvalidJsonException("Json Parser: content error. ',' at error position. Offset " + idx);

            wantComma = false;
            if (elementStack.Peek().type == JsonElementType.Dictionary)
                wantKey = true;

            idx++;
        }

        private void ProcessSemicolon()
        {
            if (!wantSemicolon)
                throw new InvalidJsonException("Json Parser: content error. ':' at error position. Offset " + idx);

            wantSemicolon = false;
            wantValue = true;
            idx++;
        }

        private void ProcessString()
        {
            if (wantSemicolon || wantComma)
                throw new InvalidJsonException("Json Parser: content error. Require ':' or ','. Offset " + idx);

            string value = FetchString();

            if (wantKey)
                FillKey(value);
            else
                InsertNode(value, JsonElementType.String);
        }

        private void GeneralProcess()
        {
            if (wantKey || wantSemicolon || wantComma)
                throw new InvalidJsonException("Json Parser: content error. Require '\"' or ':' or ','. Offset " + idx);

            if (idx + 3 < json.Length)
            {
                string tmp = json.Substring(idx, 4);
                if (string.Equals(tmp, "true", StringComparison.OrdinalIgnoreCase))
                {
                    idx += 4;
                    InsertNode(true, JsonElementType.Boolean);
                    return;
                }
                if (string.Equals(tmp, "null", StringComparison.OrdinalIgnoreCase))
                {
                    idx += 4;
                    InsertNode(null, JsonElementType.Null);
                    return;
                }
            }

            if (idx + 4 < json.Length)
            {
                string tmp = json.Substring(idx, 5);
                if (string.Equals(tmp, "false", StringComparison.OrdinalIgnoreCase))
                {
                    idx += 5;
                    InsertNode(false, JsonElementType.Boolean);
                    return;
                }
            }

            ProcessNumber();
        }

        private void ProcessNumber()
        {
            int dot = 0;
            int e = 0;
            int startIndex = idx;

            while (idx < json.Length && numericalChars.Contains(json[idx]))
            {
                if (json[idx] == '.')
                    dot++;
                else if (json[idx] == 'e' || json[idx] == 'E')
                    e++;

                idx++;
            }

            if (startIndex == idx)
            {
                if (idx + 7 < json.Length)
                {
                    string tmp = json.Substring(idx, 8);
                    if (string.Equals(tmp, "Infinity", StringComparison.OrdinalIgnoreCase))
                    {
                        idx += 8;
                        InsertNode(Double.PositiveInfinity, JsonElementType.Double);
                        return;
                    }
                }

                if (idx + 2 < json.Length)
                {
                    string tmp = json.Substring(idx, 3);
                    if (string.Equals(tmp, "nan", StringComparison.OrdinalIgnoreCase))
                    {
                        idx += 3;
                        InsertNode(Double.NaN, JsonElementType.Double);
                        return;
                    }
                    if (string.Equals(tmp, "inf", StringComparison.OrdinalIgnoreCase))
                    {
                        idx += 3;
                        InsertNode(Double.PositiveInfinity, JsonElementType.Double);
                        return;
                    }
                }

                throw new InvalidJsonException("Json Parser: content error. Unparseable content. Offset " + idx);
            }

            if (e > 1 || dot > 1)
                throw new InvalidJsonException("Json Parser: content error. Unparseable content, invalid number. Offset " + idx);

            string numberString = json.Substring(startIndex, idx - startIndex);

            if (e > 0 || dot > 0)
            {
                try
                {
                    double value = Convert.ToDouble(numberString);
                    InsertNode(value, JsonElementType.Double);
                    return;
                }
                catch (Exception ex)
                {
                    throw new InvalidJsonException("Json Parser: content error. Invalid number. Offset " + idx, ex);
                }
            }

            try
            {
                try
                {
                    long value = Convert.ToInt64(numberString);
                    InsertNode(value, JsonElementType.Int64);
                    return;
                }
                catch (OverflowException)
                {
                    UInt64 value = Convert.ToUInt64(numberString);
                    InsertNode(value, JsonElementType.UInt64);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidJsonException("Json Parser: content error. Invalid number. Offset " + idx, ex);
            }
        }
    }
}
