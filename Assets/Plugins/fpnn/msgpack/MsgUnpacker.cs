using System;
using System.Text;
using System.Collections.Generic;

namespace com.fpnn.msgpack
{
    public static class MsgUnpacker
    {
        public static Dictionary<Object, Object> Unpack(byte[] binary)
        {
            return Unpack(binary, 0);
        }

        public static Dictionary<Object, Object> Unpack(byte[] binary, int offset, int length = 0)
        {
            SAXInfo info = new SAXInfo(binary)
            {
                currIdx = offset
            };

            int endIndx = info.binary.Length;
            if (length > 0)
                endIndx = Math.Min(offset + length, info.binary.Length);

            try
            {
                while (endIndx > info.currIdx)
                {
                    UnpackNext(info);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidDataException("Incomplete msgPack binary data.", e);
            }

            if (info.GetRootObject() is Dictionary<Object, Object> dict)
                return dict;

            throw new InvalidDataException("MsgPack binary is not dictionary.");
        }

        public static Object Unpack(byte[] binary, int offset, out int endOffset)
        {
            if (binary == null || binary.Length == 0)
                throw new InvalidDataException("MsgPack binary is empty.");

            SAXInfo info = new SAXInfo(binary)
            {
                currIdx = offset
            };

            try
            {
                UnpackNext(info);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidDataException("Incomplete msgPack binary data.", e);
            }

            endOffset = info.currIdx;

            return info.GetRootObject();
        }

        //------------[ Private Methods ]-----------//

        private class ContainerInfo
        {
            private Dictionary<object, object> dict;
            private List<object> list;
            private object key;
            private bool wantKey;

            public ContainerInfo(bool isDictionary, int capacity)
            {
                if (isDictionary)
                {
                    dict = new Dictionary<object, object>(capacity);
                    wantKey = true;
                }
                else
                    list = new List<object>(capacity);
            }

            public Object GetContainer()
            {
                if (dict != null)
                {
                    if (wantKey == false)
                        throw new InsufficientException("Invalid msgPack binary: dictionary lost value.");
                    return dict;
                }
                else
                    return list;
            }

            public void Add(Object obj)
            {
                if (dict != null)
                {
                    if (wantKey)
                    {
                        key = obj;
                        wantKey = false;
                    }
                    else
                    {
                        dict.Add(key, obj);
                        wantKey = true;
                        key = null;
                    }
                }
                else
                {
                    list.Add(obj);
                }
            }
        }

        private class SAXInfo
        {
            private Stack<ContainerInfo> containerStack;
            private object rootbject;
            //-- public fields
            public readonly byte[] binary;
            public int currIdx;

            public SAXInfo(byte[] binary)
            {
                containerStack = new Stack<ContainerInfo>();
                this.binary = binary;
                currIdx = 0;
            }

            public void Add(object obj)
            {
                if (containerStack.Count > 0)
                {
                    containerStack.Peek().Add(obj);
                }
                else if (rootbject == null)
                {
                    rootbject = obj;
                }
                else
                    throw new InvalidDataException("MsgPack binary has parallel roots.");
            }

            public void AddContainer(ContainerInfo container)
            {
                Add(container.GetContainer());
                containerStack.Push(container);
            }

            public void PopContainer()
            {
                containerStack.Pop();
            }

            public Object GetRootObject()
            {
                if (containerStack.Count > 0)
                    throw new InsufficientException("Invalid msgPack binary: insufficient binary data.");

                return rootbject;
            }
        }

        private delegate void UnpackItem(SAXInfo info);

        private static readonly Dictionary<byte, UnpackItem> _unpackDict;

        static MsgUnpacker()
        {
            _unpackDict = new Dictionary<byte, UnpackItem>
            {
                { 0xc0, UnpackNil },
                // { 0xc1, UnpackData },    //-- (never used) in msgpack SPEC: https://github.com/msgpack/msgpack/blob/master/spec.md
                { 0xc2, UnpackFalse },
                { 0xc3, UnpackTrue },

                { 0xc4, UnpackBin8 },
                { 0xc5, UnpackBin16 },
                { 0xc6, UnpackBin32 },
                { 0xc7, UnpackExt8 },

                { 0xc8, UnpackExt16 },
                { 0xc9, UnpackExt32 },
                { 0xca, UnpackFloat32 },
                { 0xcb, UnpackFloat64 },

                { 0xcc, UnpackUInt8 },
                { 0xcd, UnpackUInt16 },
                { 0xce, UnpackUInt32 },
                { 0xcf, UnpackUInt64 },

                { 0xd0, UnpackInt8 },
                { 0xd1, UnpackInt16 },
                { 0xd2, UnpackInt32 },
                { 0xd3, UnpackInt64 },

                { 0xd4, UnpackFixExt1 },
                { 0xd5, UnpackFixExt2 },
                { 0xd6, UnpackFixExt4 },
                { 0xd7, UnpackFixExt8 },

                { 0xd8, UnpackFixExt16 },
                { 0xd9, UnpackStr8 },
                { 0xda, UnpackStr16 },
                { 0xdb, UnpackStr32 },

                { 0xdc, UnpackArray16 },
                { 0xdd, UnpackArray32 },
                { 0xde, UnpackMap16 },
                { 0xdf, UnpackMap32 },
            };
        }

        private static void UnpackNext(SAXInfo info)
        {
            byte sign = info.binary[info.currIdx];
            info.currIdx++;

            if (_unpackDict.TryGetValue(sign, out UnpackItem unpacker))
            {
                unpacker(info);
            }
            else if (sign <= 0x7f)
            {
                UnpackPositiveFixInt(info);
            }
            else if (sign <= 0x8f)
            {
                UnpackFixMap(info);
            }
            else if (sign <= 0x9f)
            {
                UnpackFixArray(info);
            }
            else if (sign <= 0xbf)
            {
                UnpackFixStr(info);
            }
            else if (sign <= 0xff)
            {
                UnpackNegativeFixInt(info);
            }
            else
            {
                throw new UnrecognizedDataException("Msgpack first byte: 0x" + sign.ToString("X2"));
            }
        }

        //------------[ Delegate Methods ]-----------//

        private static void UnpackNil(SAXInfo info)
        {
            info.Add(null);
        }

        private static void UnpackFalse(SAXInfo info)
        {
            info.Add(false);
        }

        private static void UnpackTrue(SAXInfo info)
        {
            info.Add(true);
        }

        private static void UnpackBin8(SAXInfo info)
        {
            byte length = info.binary[info.currIdx];
            info.currIdx++;

            byte[] data = new byte[length];
            Array.Copy(info.binary, info.currIdx, data, 0, length);
            info.currIdx += length;

            info.Add(data);
        }

        private static void UnpackBin16(SAXInfo info)
        {
            ushort length;

            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[2];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 2);
                Array.Reverse(lengthBuffer);

                length = BitConverter.ToUInt16(lengthBuffer, 0);
            }
            else
            {
                length = BitConverter.ToUInt16(info.binary, info.currIdx);
            }
            info.currIdx += 2;


            byte[] data = new byte[length];
            Array.Copy(info.binary, info.currIdx, data, 0, length);
            info.currIdx += length;

            info.Add(data);
        }

        private static void UnpackBin32(SAXInfo info)
        {
            UInt32 length;

            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[4];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 4);
                Array.Reverse(lengthBuffer);

                length = BitConverter.ToUInt32(lengthBuffer, 0);
            }
            else
            {
                length = BitConverter.ToUInt32(info.binary, info.currIdx);
            }
            info.currIdx += 4;


            byte[] data = new byte[length];
            Array.Copy(info.binary, info.currIdx, data, 0, length);
            info.currIdx += (int)length;

            info.Add(data);
        }

        private static void UnpackExt8(SAXInfo info)
        {
            byte length = info.binary[info.currIdx];
            sbyte type = (sbyte)info.binary[info.currIdx + 1];

            if (length != 12 || type != -1)
                throw new UnsupportedTypeException("Unsupported msgPack ext8 format. type " + type + ", data length: " + length);

            info.currIdx += 2;

            byte[] secondsBuffer = new byte[8];
            byte[] nanosecondsBuffer = new byte[4];

            Array.Copy(info.binary, info.currIdx, nanosecondsBuffer, 0, 4);
            Array.Copy(info.binary, info.currIdx + 4, secondsBuffer, 0, 8);
            info.currIdx += 12;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(secondsBuffer);
                Array.Reverse(nanosecondsBuffer);
            }

            UInt32 nanoseconds = BitConverter.ToUInt32(nanosecondsBuffer, 0);
            Int64 seconds = BitConverter.ToInt64(secondsBuffer, 0);

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan offset = new TimeSpan(seconds * 1000 * 1000 * 10 + nanoseconds / 100);

            info.Add(origin.Add(offset));
        }

        private static void UnpackExt16(SAXInfo info)
        {
            sbyte type = (sbyte)info.binary[info.currIdx + 2];
            throw new UnsupportedTypeException("Unsupported msgPack ext16 format. type " + type);
        }

        private static void UnpackExt32(SAXInfo info)
        {
            sbyte type = (sbyte)info.binary[info.currIdx + 4];
            throw new UnsupportedTypeException("Unsupported msgPack ext32 format. type " + type);
        }

        private static void UnpackFloat32(SAXInfo info)
        {
            //-- float in C# memory already is IEEE 754

            float value;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[4];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 4);
                Array.Reverse(lengthBuffer);

                value = BitConverter.ToSingle(lengthBuffer, 0);
            }
            else
            {
                value = BitConverter.ToSingle(info.binary, info.currIdx);
            }
            info.currIdx += 4;

            info.Add(value);
        }

        private static void UnpackFloat64(SAXInfo info)
        {
            //-- float in C# memory already is IEEE 754

            double value;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[8];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 8);
                Array.Reverse(lengthBuffer);

                value = BitConverter.ToDouble(lengthBuffer, 0);
            }
            else
            {
                value = BitConverter.ToDouble(info.binary, info.currIdx);
            }
            info.currIdx += 8;

            info.Add(value);
        }

        private static void UnpackUInt8(SAXInfo info)
        {
            byte value = info.binary[info.currIdx];
            info.currIdx++;

            info.Add(value);
        }

        private static void UnpackUInt16(SAXInfo info)
        {
            ushort value;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[2];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 2);
                Array.Reverse(lengthBuffer);

                value = BitConverter.ToUInt16(lengthBuffer, 0);
            }
            else
            {
                value = BitConverter.ToUInt16(info.binary, info.currIdx);
            }
            info.currIdx += 2;

            info.Add(value);
        }

        private static void UnpackUInt32(SAXInfo info)
        {
            UInt32 value;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[4];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 4);
                Array.Reverse(lengthBuffer);

                value = BitConverter.ToUInt32(lengthBuffer, 0);
            }
            else
            {
                value = BitConverter.ToUInt32(info.binary, info.currIdx);
            }
            info.currIdx += 4;

            info.Add(value);
        }

        private static void UnpackUInt64(SAXInfo info)
        {
            UInt64 value;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[8];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 8);
                Array.Reverse(lengthBuffer);

                value = BitConverter.ToUInt64(lengthBuffer, 0);
            }
            else
            {
                value = BitConverter.ToUInt64(info.binary, info.currIdx);
            }
            info.currIdx += 8;

            info.Add(value);
        }

        private static void UnpackInt8(SAXInfo info)
        {
            sbyte value = (sbyte)info.binary[info.currIdx];
            info.currIdx++;

            info.Add(value);
        }

        private static void UnpackInt16(SAXInfo info)
        {
            short value;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[2];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 2);
                Array.Reverse(lengthBuffer);

                value = BitConverter.ToInt16(lengthBuffer, 0);
            }
            else
            {
                value = BitConverter.ToInt16(info.binary, info.currIdx);
            }
            info.currIdx += 2;

            info.Add(value);
        }

        private static void UnpackInt32(SAXInfo info)
        {
            Int32 value;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[4];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 4);
                Array.Reverse(lengthBuffer);

                value = BitConverter.ToInt32(lengthBuffer, 0);
            }
            else
            {
                value = BitConverter.ToInt32(info.binary, info.currIdx);
            }
            info.currIdx += 4;

            info.Add(value);
        }

        private static void UnpackInt64(SAXInfo info)
        {
            Int64 value;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[8];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 8);
                Array.Reverse(lengthBuffer);

                value = BitConverter.ToInt64(lengthBuffer, 0);
            }
            else
            {
                value = BitConverter.ToInt64(info.binary, info.currIdx);
            }
            info.currIdx += 8;

            info.Add(value);
        }

        private static void UnpackFixExt1(SAXInfo info)
        {
            sbyte type = (sbyte)info.binary[info.currIdx];
            throw new UnsupportedTypeException("Unsupported msgPack fix ext 1 format. type " + type);
        }
        private static void UnpackFixExt2(SAXInfo info)
        {
            sbyte type = (sbyte)info.binary[info.currIdx];
            throw new UnsupportedTypeException("Unsupported msgPack fix ext 2 format. type " + type);
        }
        private static void UnpackFixExt4(SAXInfo info)
        {
            sbyte type = (sbyte)info.binary[info.currIdx];
            if (type != -1)
                throw new UnsupportedTypeException("Unsupported msgPack fix ext 4 format. type " + type);

            info.currIdx++;


            UInt32 seconds;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[4];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 4);
                Array.Reverse(lengthBuffer);

                seconds = BitConverter.ToUInt32(lengthBuffer, 0);
            }
            else
            {
                seconds = BitConverter.ToUInt32(info.binary, info.currIdx);
            }
            info.currIdx += 4;

            
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan offset = new TimeSpan(seconds * 1000 * 1000 * 10);

            info.Add(origin.Add(offset));
        }
        private static void UnpackFixExt8(SAXInfo info)
        {
            sbyte type = (sbyte)info.binary[info.currIdx];
            if (type != -1)
                throw new UnsupportedTypeException("Unsupported msgPack fix ext 8 format. type " + type);

            info.currIdx++;


            UInt64 timeValue;
            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[8];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 8);
                Array.Reverse(lengthBuffer);

                timeValue = BitConverter.ToUInt64(lengthBuffer, 0);
            }
            else
            {
                timeValue = BitConverter.ToUInt64(info.binary, info.currIdx);
            }
            info.currIdx += 8;


            UInt32 nanoseconds = (UInt32)(timeValue >> 34);
            Int64 seconds = (Int64)(timeValue & 0x00000003FFFFFFFF);
            
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan offset = new TimeSpan(seconds * 1000 * 1000 * 10 + nanoseconds / 100);

            info.Add(origin.Add(offset));
        }
        private static void UnpackFixExt16(SAXInfo info)
        {
            sbyte type = (sbyte)info.binary[info.currIdx];
            throw new UnsupportedTypeException("Unsupported msgPack fix ext 16 format. type " + type);
        }

        private static void UnpackString(SAXInfo info, int length)
        {
            UTF8Encoding utf8Encoding = new UTF8Encoding(false, true);     //-- NO BOM.
            try
            {
                string str = utf8Encoding.GetString(info.binary, info.currIdx, length);
                info.currIdx += length;
                info.Add(str);
                return;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw ex;
            }
            catch (DecoderFallbackException)
            {
                //-- Do nothing, through the cache block.
            }
            catch (ArgumentException)
            {
                //-- Do nothing, through the cache block.
            }

            byte[] data = new byte[length];
            Array.Copy(info.binary, info.currIdx, data, 0, length);
            info.currIdx += length;
            info.Add(data);
        }
        private static void UnpackStr8(SAXInfo info)
        {
            byte length = info.binary[info.currIdx];
            info.currIdx++;

            UnpackString(info, length);
        }
        private static void UnpackStr16(SAXInfo info)
        {
            ushort length;

            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[2];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 2);
                Array.Reverse(lengthBuffer);

                length = BitConverter.ToUInt16(lengthBuffer, 0);
            }
            else
            {
                length = BitConverter.ToUInt16(info.binary, info.currIdx);
            }
            info.currIdx += 2;

            UnpackString(info, length);
        }
        private static void UnpackStr32(SAXInfo info)
        {
            UInt32 length;

            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[4];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 4);
                Array.Reverse(lengthBuffer);

                length = BitConverter.ToUInt32(lengthBuffer, 0);
            }
            else
            {
                length = BitConverter.ToUInt32(info.binary, info.currIdx);
            }
            info.currIdx += 4;

            UnpackString(info, (int)length);
        }
        private static void UnpackArray16(SAXInfo info)
        {
            ushort length;

            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[2];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 2);
                Array.Reverse(lengthBuffer);

                length = BitConverter.ToUInt16(lengthBuffer, 0);
            }
            else
            {
                length = BitConverter.ToUInt16(info.binary, info.currIdx);
            }
            info.currIdx += 2;

            ContainerInfo container = new ContainerInfo(false, (int)length);
            info.AddContainer(container);

            for (ushort i = 0; i < length; i++)
                UnpackNext(info);

            info.PopContainer();
        }
        private static void UnpackArray32(SAXInfo info)
        {
            UInt32 length;

            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[4];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 4);
                Array.Reverse(lengthBuffer);

                length = BitConverter.ToUInt32(lengthBuffer, 0);
            }
            else
            {
                length = BitConverter.ToUInt32(info.binary, info.currIdx);
            }
            info.currIdx += 4;

            ContainerInfo container = new ContainerInfo(false, (int)length);
            info.AddContainer(container);

            for (UInt32 i = 0; i < length; i++)
                UnpackNext(info);

            info.PopContainer();
        }

        private static void UnpackMap16(SAXInfo info)
        {
            ushort length;

            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[2];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 2);
                Array.Reverse(lengthBuffer);

                length = BitConverter.ToUInt16(lengthBuffer, 0);
            }
            else
            {
                length = BitConverter.ToUInt16(info.binary, info.currIdx);
            }
            info.currIdx += 2;

            ContainerInfo container = new ContainerInfo(true, (int)length);
            info.AddContainer(container);

            for (UInt32 i = 0; i < length * 2; i++)
                UnpackNext(info);

            info.PopContainer();
        }
        private static void UnpackMap32(SAXInfo info)
        {
            UInt32 length;

            if (BitConverter.IsLittleEndian)
            {
                byte[] lengthBuffer = new byte[4];
                Array.Copy(info.binary, info.currIdx, lengthBuffer, 0, 4);
                Array.Reverse(lengthBuffer);

                length = BitConverter.ToUInt32(lengthBuffer, 0);
            }
            else
            {
                length = BitConverter.ToUInt32(info.binary, info.currIdx);
            }
            info.currIdx += 4;

            ContainerInfo container = new ContainerInfo(true, (int)length);
            info.AddContainer(container);

            for (UInt32 i = 0; i < length * 2; i++)
                UnpackNext(info);

            info.PopContainer();
        }
        private static void UnpackPositiveFixInt(SAXInfo info)
        {
            sbyte value = (sbyte)info.binary[info.currIdx - 1];
            info.Add(value);
        }
        private static void UnpackFixMap(SAXInfo info)
        {
            byte length = info.binary[info.currIdx - 1];
            length &= 0x0F;

            ContainerInfo container = new ContainerInfo(true, (int)length);
            info.AddContainer(container);

            for (ushort i = 0; i < length * 2; i++)
                UnpackNext(info);

            info.PopContainer();
        }
        private static void UnpackFixArray(SAXInfo info)
        {
            byte length = info.binary[info.currIdx - 1];
            length &= 0x0F;

            ContainerInfo container = new ContainerInfo(false, (int)length);
            info.AddContainer(container);

            for (byte i = 0; i < length; i++)
                UnpackNext(info);

            info.PopContainer();
        }
        private static void UnpackFixStr(SAXInfo info)
        {
            byte length = info.binary[info.currIdx - 1];
            length &= 0x1F;

            UnpackString(info, length);
        }
        private static void UnpackNegativeFixInt(SAXInfo info)
        {
            byte value = info.binary[info.currIdx - 1];
            info.Add(value);
        }
    }
}
