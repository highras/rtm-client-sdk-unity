using System;
using System.IO;
using System.Collections.Generic;

namespace com.fpnn.proto
{
    public class Answer : Message
    {
        private static readonly byte[] FPNNBasicAnswerHeader = { 0x46, 0x50, 0x4e, 0x4e, 0x1, 0x80, 0x2 };
        private static readonly byte[] fakePayloadLength = { 0, 0, 0, 0 };

        private bool errorAnswer;
        private UInt32 seqNum;

        public Answer(Quest quest): this(quest.SeqNum())
        {
        }

        public Answer(UInt32 seqNum)
        {
            errorAnswer = false;
            this.seqNum = seqNum;
        }

        public Answer(UInt32 seqNum, bool error, Dictionary<object, object> payload)
            : base(payload)
        {
            errorAnswer = error;
            this.seqNum = seqNum;
        }

        public UInt32 SeqNum()
        {
            return seqNum;
        }

        public bool IsException()
        {
            return errorAnswer;
        }

        public int ErrorCode()
        {
            return (int)Get("code", 0);
        }

        public string Ex()
        {
            return (string)Get("ex", "");
        }

        public void FillErrorCode(int code)
        {
            errorAnswer = true;
            Param("code", code);
        }

        public void FillErrorInfo(int code, string ex)
        {
            errorAnswer = true;
            Param("code", code);
            Param("ex", ex);
        }

        private void BuildFPNNPackage(Stream stream)
        {
            stream.Write(FPNNBasicAnswerHeader, 0, 7);
            if (errorAnswer)
                stream.WriteByte(0x1);
            else
                stream.WriteByte(0x0);

            //-- payload size
            stream.Write(fakePayloadLength, 0, 4);

            //-- seq num
            byte[] seqBuffer = BitConverter.GetBytes(seqNum);

            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(seqBuffer);

            stream.Write(seqBuffer, 0, 4);
        }


        new public byte[] Raw()
        {
            byte[] rawData;
            using (MemoryStream stream = new MemoryStream())
            {
                BuildFPNNPackage(stream);
                base.Raw(stream);
                rawData = stream.ToArray();
            }

            Int32 payloadLength = rawData.Length - 16;  //-- 16: Answer heander length.

            byte[] payloadLengthBuffer = BitConverter.GetBytes(payloadLength);

            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(payloadLengthBuffer);

            Array.Copy(payloadLengthBuffer, 0, rawData, 8, 4);

            return rawData;
        }
    }
}
