using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace com.fpnn.proto
{
    public class Quest : Message
    {
        private static readonly byte[] FPNNBasicHeader = { 0x46, 0x50, 0x4e, 0x4e, 0x1, 0x80 };
        private static readonly byte[] fakePayloadLength = { 0, 0, 0, 0 };

        private UInt32 seqNum;
        private bool isOneWay;
        private String method;

        private static class SeqNumGenerator
        {
            static private object interLocker;
            static private int count;

            static SeqNumGenerator()
            {
                interLocker = new object();
                count = (int)(GetCurrentMilliseconds() % 1000000);
            }

            static public Int64 GetCurrentMilliseconds()
            {
                DateTime originDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan span = DateTime.Now - originDateTime;
                return (Int64)Math.Floor(span.TotalMilliseconds);
            }

            static public int Gen()
            {
                lock (interLocker)
                {
                    return ++count;
                }
            }
        }

        public Quest(string method): this(method, false)
        {
        }

        public Quest(string method, bool isOneWay)
        {
            this.method = method;
            this.isOneWay = isOneWay;
            seqNum = 0;
        }

        public Quest(string method, bool isOneWay, UInt32 seqNum, Dictionary<object, object> payload)
            : base(payload)
        {
            this.method = method;
            this.isOneWay = isOneWay;
            this.seqNum = seqNum;
        }

        public UInt32 SeqNum()
        {
            return seqNum;
        }

        public String Method()
        {
            return method;
        }

        public bool IsOneWay()
        {
            return isOneWay;
        }

        public bool IsTwoWay()
        {
            return !isOneWay;
        }

        private int BuildFPNNPackage(Stream stream)
        {
            UTF8Encoding utf8Encoding = new UTF8Encoding();     //-- NO BOM.
            byte[] methodData = utf8Encoding.GetBytes(method);

            if (seqNum == 0)
                seqNum = (UInt32)SeqNumGenerator.Gen();

            stream.Write(FPNNBasicHeader, 0, 6);
            if (isOneWay)
                stream.WriteByte(0x0);
            else
                stream.WriteByte(0x1);

            stream.WriteByte((byte)methodData.Length);

            //-- payload size
            stream.Write(fakePayloadLength, 0, 4);

            //-- seq num
            if (isOneWay == false)
            {
                byte[] seqBuffer = BitConverter.GetBytes(seqNum);

                if (BitConverter.IsLittleEndian == false)
                    Array.Reverse(seqBuffer);

                stream.Write(seqBuffer, 0, 4);
            }

            //-- method
            stream.Write(methodData, 0, methodData.Length);

            return methodData.Length;
        }

        new public byte[] Raw()
        {
            byte[] rawData;
            int methodUTF8Length;

            using (MemoryStream stream = new MemoryStream())
            {
                methodUTF8Length = BuildFPNNPackage(stream);
                base.Raw(stream);
                rawData = stream.ToArray();
            }

            Int32 payloadLength = rawData.Length - 12 - methodUTF8Length;
            if (!isOneWay)
                payloadLength -= 4;

            byte[] payloadLengthBuffer = BitConverter.GetBytes(payloadLength);

            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(payloadLengthBuffer);

            Array.Copy(payloadLengthBuffer, 0, rawData, 8, 4);

            return rawData;
        }
    }
}
