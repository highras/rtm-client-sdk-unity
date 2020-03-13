using System;
using System.Collections.Generic;
using System.Text;
using com.fpnn.proto;
using com.fpnn.msgpack;
namespace com.fpnn
{
    internal class StandardReceiver : ReceiverBase
    {
        private const int FPNNHeaderLength = 12;

        private bool receivingHeader;
        private byte[] headerBuffer;
        private byte[] bodyBuffer;

        private int payloadLength;
        private bool isAnswer;
        private bool isTwowayQuest;

        public StandardReceiver()
        {
            headerBuffer = new byte[FPNNHeaderLength];

            ChangeToReceiveHeader();
        }

        private void ChangeToReceiveHeader()
        {
            receivingHeader = true;
            buffer = headerBuffer;
            offset = 0;
            requireLength = FPNNHeaderLength;
        }

        private void ProcessHeader()
        {
            if ((headerBuffer[0] != 0x46)
                || (headerBuffer[1] != 0x50)
                || (headerBuffer[2] != 0x4e)
                || (headerBuffer[3] != 0x4e))
            {
                throw new ReceiverErrorMessageException("Package is not FPNN package, magic code mismatched.");
            }

            if (headerBuffer[5] != 0x80)
                throw new ReceiverErrorMessageException("Package is not encoded by msgpack.");

            if (BitConverter.IsLittleEndian)
            {
                payloadLength = BitConverter.ToInt32(headerBuffer, 8);
            }
            else
            {
                byte[] lengthBuffer = new byte[4];
                Array.Copy(headerBuffer, 8, lengthBuffer, 0, 4);
                Array.Reverse(lengthBuffer);

                payloadLength = BitConverter.ToInt32(lengthBuffer, 0);
            }

            if (payloadLength < 1 || payloadLength > ClientEngine.maxPayloadSize)
                throw new ReceiverErrorMessageException("Received invalid package, package payload length: " + payloadLength);

            byte mtype = headerBuffer[6];
            if (mtype == 2)
            {
                isAnswer = true;
                requireLength = payloadLength + 4;
            }
            else if (mtype == 1)
            {
                isAnswer = false;
                isTwowayQuest = true;
                requireLength = payloadLength + 4 + headerBuffer[7];
            }
            else if (mtype == 0)
            {
                isAnswer = false;
                isTwowayQuest = false;
                requireLength = payloadLength + headerBuffer[7];
            }
            else
                throw new ReceiverErrorMessageException("Received invalid package, mtype is " + mtype);

            //-- Change to receive payload.

            if (bodyBuffer == null)
                bodyBuffer = new byte[requireLength];
            else if (bodyBuffer.Length < requireLength)
                bodyBuffer = new byte[requireLength];

            receivingHeader = false;
            buffer = bodyBuffer;
            offset = 0;
        }

        private UInt32 FetchSeqNum()
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(bodyBuffer, 0);
            }
            else
            {
                byte[] seqNumBuffer = new byte[4];
                Array.Copy(bodyBuffer, 0, seqNumBuffer, 0, 4);
                Array.Reverse(seqNumBuffer);

                return BitConverter.ToUInt32(seqNumBuffer, 0);
            }
        }

        private Answer BuildAnswer()
        {
            Dictionary<Object, Object> payload = MsgUnpacker.Unpack(bodyBuffer, 4, requireLength - 4);
            bool isErrorAnswer = (headerBuffer[7] != 0);

            return new Answer(FetchSeqNum(), isErrorAnswer, payload);
        }

        private Quest BuildQuest()
        {
            string method;
            UInt32 seqNum;
            Dictionary<Object, Object> payload;

            UTF8Encoding utf8Encoding = new UTF8Encoding(false, true);     //-- NO BOM.

            if (isTwowayQuest)
            {
                seqNum = FetchSeqNum();
                method = utf8Encoding.GetString(bodyBuffer, 4, headerBuffer[7]);
                payload = MsgUnpacker.Unpack(bodyBuffer, 4 + headerBuffer[7], requireLength - 4 - headerBuffer[7]);
            }
            else
            {
                seqNum = 0;
                method = utf8Encoding.GetString(bodyBuffer, 0, headerBuffer[7]);
                payload = MsgUnpacker.Unpack(bodyBuffer, headerBuffer[7], requireLength - headerBuffer[7]);
            }

            return new Quest(method, !isTwowayQuest, seqNum, payload);
        }

        public override void Done(out Quest quest, out Answer answer)
        {
            quest = null;
            answer = null;

            if (receivingHeader)
            {
                if (offset == requireLength)
                    ProcessHeader();
                
                return;
            }

            if (offset != requireLength)
                return;

            if (isAnswer)
                answer = BuildAnswer();
            else
                quest = BuildQuest();

            //-- Change to receive header.

            ChangeToReceiveHeader();
        }
    }
}
