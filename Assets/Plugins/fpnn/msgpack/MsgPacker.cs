using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace com.fpnn.msgpack
{
    public static class MsgPacker
    {
        public delegate void PackItem(Stream stream, Object obj);

        private static readonly Dictionary<string, PackItem> _packDict;

        static MsgPacker()
        {
            _packDict = new Dictionary<string, PackItem>
            {
                { "System.Boolean", PackBoolean },

                { "System.Decimal", PackDecimal },
                { "System.Double", PackDouble },
                { "System.Single", PackFloat },

                { "System.SByte", PackInteger },
                { "System.Int16", PackInteger },
                { "System.Int32", PackInteger },
                { "System.Int64", PackInteger },

                { "System.Byte", PackUInteger },
                { "System.Char", PackUInteger },
                { "System.UInt16", PackUInteger },
                { "System.UInt32", PackUInteger },
                { "System.UInt64", PackUInteger },

                { "System.String", PackString },
                { "System.Tuple", PackTuple },
                { "System.DateTime", PackTimestamp },
                { "System.Byte[]", PackBinary }
            };
        }

        /*
         * May throw exception in UnsupportedTypeException type.
         */
        static public void Pack(Stream stream, Object obj)
        {
            if (obj == null)
            {
                PackNull(stream);
                return;
            }

            string typeFullName = obj.GetType().FullName;
            int idx = typeFullName.IndexOf('`');
            if (idx != -1)
            {
                typeFullName = typeFullName.Substring(0, idx);
            }

            if (_packDict.TryGetValue(typeFullName, out PackItem packer))
            {
                packer(stream, obj);
            }
            else if (obj is IEnumerable)
            {
                PackIEnumerable(stream, obj);
            }
            else
            {
                throw UnsupportedTypeException.Create(obj);
            }
        }

        static public void PackNull(Stream stream)
        {
            byte sign = 0xc0;
            stream.WriteByte(sign);
        }

        static public void PackBoolean(Stream stream, Object obj)
        {
            Boolean v = (bool)obj;
            if (v)
            {
                byte sign = 0xc3;
                stream.WriteByte(sign);
            }
            else
            {
                byte sign = 0xc2;
                stream.WriteByte(sign);
            }
        }

        static public void PackInteger(Stream stream, Object obj)
        {
            Int64 value = (Int64)Convert.ChangeType(obj, TypeCode.Int64);

            if (value >= 0)
            {
                PackUInteger(stream, obj);
                return;
            }

            if ((0xFFFFFFFFFFFFFFE0 & (UInt64)value) == 0xFFFFFFFFFFFFFFE0)
            {
                sbyte sbyteValue = (sbyte)value;
                stream.WriteByte((byte)sbyteValue);
            }
            else if ((0xFFFFFFFFFFFFFF80 & (UInt64)value) == 0xFFFFFFFFFFFFFF80)
            {
                sbyte sbyteValue = (sbyte)value;

                byte sign = 0xd0;
                stream.WriteByte(sign);
                stream.WriteByte((byte)sbyteValue);
            }
            else if ((0xFFFFFFFFFFFF8000 & (UInt64)value) == 0xFFFFFFFFFFFF8000)
            {
                short shortValue = (short)value;
                byte[] lenBuffer = BitConverter.GetBytes(shortValue);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xd1;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 2);
            }
            else if ((0xFFFFFFFF80000000 & (UInt64)value) == 0xFFFFFFFF80000000)
            {
                Int32 int32Value = (Int32)value;
                byte[] lenBuffer = BitConverter.GetBytes(int32Value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xd2;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 4);
            }
            else
            {
                Int64 int64Value = (Int64)value;
                byte[] lenBuffer = BitConverter.GetBytes(int64Value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xd3;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 8);
            }
        }

        static public void PackUInteger(Stream stream, Object obj)
        {
            UInt64 value = (UInt64)Convert.ChangeType(obj, TypeCode.UInt64);

            if (value <= 127)
            {
                byte byteValue = (byte)value;
                stream.WriteByte(byteValue);
            }
            else if (value <= 0xFF)
            {
                byte sign = 0xcc;
                stream.WriteByte(sign);

                byte byteValue = (byte)value;
                stream.WriteByte(byteValue);
            }
            else if (value <= 0xFFFF)
            {
                ushort shortValue = (ushort)value;
                byte[] lenBuffer = BitConverter.GetBytes(shortValue);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xcd;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 2);
            }
            else if (value <= 0xFFFFFFFF)
            {
                UInt32 uint32Value = (UInt32)value;
                byte[] lenBuffer = BitConverter.GetBytes(uint32Value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xce;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 4);
            }
            else
            {
                UInt64 uint64Value = (UInt64)value;
                byte[] lenBuffer = BitConverter.GetBytes(uint64Value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xcf;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 8);
            }
        }

        static public void PackFloat(Stream stream, Object obj)
        {
            //-- float in C# memory already is IEEE 754

            float value = (float)obj;
            byte[] byteArray = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArray);

            byte sign = 0xca;
            stream.WriteByte(sign);
            stream.Write(byteArray, 0, 4);
        }

        static public void PackDouble(Stream stream, Object obj)
        {
            //-- double in C# memory already is IEEE 754

            double value = (double)obj;
            byte[] byteArray = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArray);

            byte sign = 0xcb;
            stream.WriteByte(sign);
            stream.Write(byteArray, 0, 8);
        }

        static public void PackDecimal(Stream stream, Object obj)
        {
            Decimal dec = (Decimal)obj;
            double value = Decimal.ToDouble(dec);

            PackDouble(stream, value);
        }

        static public void PackString(Stream stream, Object obj)
        {
            string stringValue = (string)obj;

            UTF8Encoding utf8Encoding = new UTF8Encoding();     //-- NO BOM.
            byte[] rawData = utf8Encoding.GetBytes(stringValue);

            if (rawData.Length <= 31)
            {
                byte sign = (byte)(rawData.Length | 0xA0);
                stream.WriteByte(sign);
            }
            else if (rawData.Length <= 255)
            {
                byte sign = 0xd9;
                stream.WriteByte(sign);

                byte lengthByte = (byte)rawData.Length;
                stream.WriteByte(lengthByte);
            }
            else if (rawData.Length <= 65535)
            {
                byte sign = 0xda;
                stream.WriteByte(sign);

                ushort shortValue = (ushort)rawData.Length;
                byte[] lenBuffer = BitConverter.GetBytes(shortValue);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                stream.Write(lenBuffer, 0, 2);
            }
            else
            {
                byte sign = 0xdb;
                stream.WriteByte(sign);

                UInt32 uint32Value = (UInt32)rawData.Length;
                byte[] lenBuffer = BitConverter.GetBytes(uint32Value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                stream.Write(lenBuffer, 0, 4);
            }

            stream.Write(rawData, 0, rawData.Length);
        }

        static public void PackBinary(Stream stream, Object obj)
        {
            byte[] binaryData = (byte[])obj;

            if (binaryData.Length <= 255)
            {
                byte sign = 0xc4;
                stream.WriteByte(sign);

                byte lengthByte = (byte)binaryData.Length;
                stream.WriteByte(lengthByte);
            }
            else if (binaryData.Length <= 65535)
            {
                byte sign = 0xc5;
                stream.WriteByte(sign);

                ushort shortValue = (ushort)binaryData.Length;
                byte[] lenBuffer = BitConverter.GetBytes(shortValue);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                stream.Write(lenBuffer, 0, 2);
            }
            else
            {
                byte sign = 0xc6;
                stream.WriteByte(sign);

                UInt32 uint32Value = (UInt32)binaryData.Length;
                byte[] lenBuffer = BitConverter.GetBytes(uint32Value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                stream.Write(lenBuffer, 0, 4);
            }

            stream.Write(binaryData, 0, binaryData.Length);
        }

        static public void PackArray(Stream stream, Object obj)
        {
            IEnumerable ie = (IEnumerable)obj;
            IEnumerator it = ie.GetEnumerator();
            it.Reset();

            int count = 0;

            while (it.MoveNext())
            {
                count++;
            }

            if (count <= 15)
            {
                byte sign = (byte)(count | 0x90);
                stream.WriteByte(sign);
            }
            else if (count <= 65535)
            {
                ushort shortValue = (ushort)count;
                byte[] lenBuffer = BitConverter.GetBytes(shortValue);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xdc;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 2);
            }
            else
            {
                UInt32 uint32Value = (UInt32)count;
                byte[] lenBuffer = BitConverter.GetBytes(uint32Value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xdd;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 4);
            }

            it.Reset();

            while (it.MoveNext())
            {
                Object o = it.Current;
                Pack(stream, o);
            }
        }

        static public void PackDictionary(Stream stream, Object obj)
        {
            IEnumerable ie = (IEnumerable)obj;
            IEnumerator it = ie.GetEnumerator();
            it.Reset();

            int count = 0;

            while (it.MoveNext())
            {
                count++;
            }

            if (count <= 15)
            {
                byte sign = (byte)(count | 0x80);
                stream.WriteByte(sign);
            }
            else if (count <= 65535)
            {
                ushort shortValue = (ushort)count;
                byte[] lenBuffer = BitConverter.GetBytes(shortValue);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xde;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 2);
            }
            else
            {
                UInt32 uint32Value = (UInt32)count;
                byte[] lenBuffer = BitConverter.GetBytes(uint32Value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xdf;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 4);
            }

            it.Reset();
            IDictionaryEnumerator id = (IDictionaryEnumerator)it;
            while (it.MoveNext())
            {
                Object k = id.Key;
                Pack(stream, k);

                Object v = id.Value;
                Pack(stream, v);
            }
        }

        static public void PackTimestamp(Stream stream, Object obj)
        {
            DateTime userDate = (DateTime)obj;
            DateTime utcUserDate = userDate.ToUniversalTime();
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            TimeSpan diff = utcUserDate - origin;
            Int64 seconds = (Int64)Math.Floor(diff.TotalSeconds);
            Int64 milliseconds = (Int64)Math.Floor(diff.TotalMilliseconds);
            Int64 nonaseconds = (milliseconds - seconds * 1000) * 1000 * 1000;

            if (nonaseconds == 0 && ((UInt64)seconds >> 32) == 0)
            {
                UInt32 secondsValue = (UInt32)seconds;
                byte[] lenBuffer = BitConverter.GetBytes(secondsValue);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xd6;
                stream.WriteByte(sign);
                sbyte type = -1;
                stream.WriteByte((byte)type);
                stream.Write(lenBuffer, 0, 4);
            }
            else if (((UInt64)seconds >> 34) == 0)
            {
                UInt64 rawData = ((UInt64)nonaseconds << 34) | (UInt64)seconds;
                byte[] byteArray = BitConverter.GetBytes(rawData);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(byteArray);

                byte sign = 0xd7;
                stream.WriteByte(sign);
                sbyte type = -1;
                stream.WriteByte((byte)type);
                stream.Write(byteArray, 0, 8);
            }
            else
            {
                UInt32 nanos = (UInt32)nonaseconds;
                byte[] secondsBytes = BitConverter.GetBytes(seconds);
                byte[] nanosBytes = BitConverter.GetBytes(nanos);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(secondsBytes);
                    Array.Reverse(nanosBytes);
                }

                byte sign = 0xc7;
                stream.WriteByte(sign);
                sign = 12;
                stream.WriteByte(sign);
                sbyte type = -1;
                stream.WriteByte((byte)type);
                stream.Write(nanosBytes, 0, 4);
                stream.Write(secondsBytes, 0, 8);
            }
        }

        static public void PackTuple(Stream stream, Object obj)
        {
            Type objType = obj.GetType();
            var props = objType.GetProperties();
            int count = props.Length;

            if (count <= 15)
            {
                byte sign = (byte)(count | 0x90);
                stream.WriteByte(sign);
            }
            else if (count <= 65535)
            {
                ushort shortValue = (ushort)count;
                byte[] lenBuffer = BitConverter.GetBytes(shortValue);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xdc;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 2);
            }
            else
            {
                UInt32 uint32Value = (UInt32)count;
                byte[] lenBuffer = BitConverter.GetBytes(uint32Value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBuffer);

                byte sign = 0xdd;
                stream.WriteByte(sign);
                stream.Write(lenBuffer, 0, 4);
            }

            foreach (System.Reflection.PropertyInfo prop in props)
            {
                Object subObj = prop.GetValue(obj, null);
                Pack(stream, obj);
            }
        }

        static public void PackIEnumerable(Stream stream, Object obj)
        {
            IEnumerable ie = (IEnumerable)obj;
            IEnumerator it = ie.GetEnumerator();
            it.Reset();

            if (it is IDictionaryEnumerator)
                PackDictionary(stream, obj);
            else
                PackArray(stream, obj);
        }
    }
}
