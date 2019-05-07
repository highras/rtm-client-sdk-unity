using System;
using System.IO;
using System.Text;
using System.Collections;

// using UnityEngine;

namespace com.fpnn {

    public class FPPackage {

        public string GetKeyCallback(FPData data) {

            StringBuilder sb = new StringBuilder(10);

            sb.Append("FPNN_");
            sb.Append(Convert.ToString(data.GetSeq()));

            return sb.ToString();
        }

        public bool IsHttp(FPData data) {

            return BytesCompare_Base64(FPConfig.HTTP_MAGIC, data.GetMagic());
        }

        public bool IsTcp(FPData data) {

            return BytesCompare_Base64(FPConfig.TCP_MAGIC, data.GetMagic());
        }

        public bool IsMsgPack(FPData data) {

            return 1 == data.GetFlag();
        }

        public bool IsJson(FPData data) {

            return 0 == data.GetFlag();
        }

        public bool IsOneWay(FPData data) {

            return 0 == data.GetMtype();
        }

        public bool IsTwoWay(FPData data) {

            return 1 == data.GetMtype();
        }

        public bool IsQuest(FPData data) {

            return this.IsTwoWay(data) || this.IsOneWay(data);
        }

        public bool IsAnswer(FPData data) {

            return 2 == data.GetMtype();
        }

        public bool IsSupportPack(FPData data) {

            return this.IsMsgPack(data) != this.IsJson(data);
        }

        public bool CheckVersion(FPData data) {

            if (data.GetVersion() < 0) {

                return false;
            }

            if (data.GetVersion() >= FPConfig.FPNN_VERSION.Length) {

                return false;
            }

            return true;
        }

        public FPData PeekHead(byte[] bytes) {

            if (bytes.Length < 12) {

                return null;
            }

            FPData peek = new FPData();

            peek.SetMagic(this.GetByteArrayRange(bytes, 0, 3));
            peek.SetVersion(Array.IndexOf(FPConfig.FPNN_VERSION, bytes[4]));

            if (bytes[5] == FPConfig.FP_FLAG[0]) {

                peek.SetFlag(0);
            }

            if (bytes[5] == FPConfig.FP_FLAG[1]) {

                peek.SetFlag(1);
            }

            peek.SetMtype(Array.IndexOf(FPConfig.FP_MESSAGE_TYPE, bytes[6]));
            peek.SetSS(bytes[7]);
            peek.SetPsize((int)BitConverter.ToUInt32(this.GetByteArrayRange(bytes, 8, 11), 0));

            return peek;
        }

        public bool DeCode(FPData data) {

            byte[] bytes = this.GetByteArrayRange(data.Bytes, 12, data.Bytes.Length - 1);

            if (this.IsOneWay(data)) {

                return this.DeCodeOneWay(bytes, data);
            }

            if (this.IsTwoWay(data)) {

                return this.DeCodeTwoWay(bytes, data);
            }

            if (this.IsAnswer(data)) {

                return this.DeCodeAnswer(bytes, data);
            }

            return false;
        }

        private bool DeCodeOneWay(byte[] bytes, FPData data) {

            if (bytes.Length != data.GetSS() + data.GetPsize()) {

                return false;
            }

            data.SetMethod(this.GetString(this.GetByteArrayRange(bytes, 0, data.GetSS() - 1)));

            if (this.IsJson(data)) {

                data.SetPayload(this.GetString(this.GetByteArrayRange(bytes, data.GetSS(), bytes.Length - 1)));
            }

            if (this.IsMsgPack(data)) {

                data.SetPayload(this.GetByteArrayRange(bytes, data.GetSS(), bytes.Length - 1));
            }

            return true;
        }

        private bool DeCodeTwoWay(byte[] bytes, FPData data) {

            if (bytes.Length != 4 + data.GetSS() + data.GetPsize()) {

                return false;
            }

            data.SetSeq((int)BitConverter.ToUInt32(this.GetByteArrayRange(bytes, 0, 3), 0));
            data.SetMethod(this.GetString(this.GetByteArrayRange(bytes, 4, data.GetSS() + 4 - 1)));

            if (this.IsJson(data)) {

                data.SetPayload(this.GetString(this.GetByteArrayRange(bytes, 4 + data.GetSS(), bytes.Length - 1)));
            }

            if (this.IsMsgPack(data)) {

                data.SetPayload(this.GetByteArrayRange(bytes, 4 + data.GetSS(), bytes.Length - 1));
            }

            return true;
        }

        private bool DeCodeAnswer(byte[] bytes, FPData data) {

            if (bytes.Length != 4 + data.GetPsize()) {

                return false;
            }

            data.SetSeq((int)BitConverter.ToUInt32(this.GetByteArrayRange(bytes, 0, 3), 0));

            if (this.IsJson(data)) {

                data.SetPayload(this.GetString(this.GetByteArrayRange(bytes, 4, bytes.Length - 1)));
            }

            if (this.IsMsgPack(data)) {

                data.SetPayload(this.GetByteArrayRange(bytes, 4, bytes.Length - 1));
            }

            return true;
        }

        public byte[] EnCode(FPData data) {

            if (this.IsOneWay(data)) {

                return this.EnCodeOneway(data);
            }

            if (this.IsTwoWay(data)) {

                return this.EnCodeTwoway(data);
            }

            if (this.IsAnswer(data)) {

                return this.EnCodeAnswer(data);
            }

            return null;
        }

        private byte[] EnCodeOneway(FPData data) {

            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            MemoryStream ms = this.BuildHeader(data, 12 + data.GetSS() + data.GetPsize());

            ms.WriteByte((byte)data.GetSS());

            byte[] psizeBytes = BitConverter.GetBytes(data.GetPsize());
            ms.Write(psizeBytes, 0, psizeBytes.Length);

            byte[] methodBytes = encoder.GetBytes(data.GetMethod());
            ms.Write(methodBytes, 0, methodBytes.Length);

            byte[] payloadBytes = null;

            if (this.IsJson(data)) {

                payloadBytes = encoder.GetBytes(data.JsonPayload());
            }

            if (this.IsMsgPack(data)) {

                payloadBytes = data.MsgpackPayload();
            }

            ms.Write(payloadBytes, 0, payloadBytes.Length);

            return this.StreamToBytes(ms);
        }

        private byte[] EnCodeTwoway(FPData data) {

            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            MemoryStream ms = this.BuildHeader(data, 16 + data.GetSS() + data.GetPsize());

            ms.WriteByte((byte)data.GetSS());

            byte[] psizeBytes = BitConverter.GetBytes(data.GetPsize());
            ms.Write(psizeBytes, 0, psizeBytes.Length);

            byte[] seqBytes = BitConverter.GetBytes (data.GetSeq());
            ms.Write(seqBytes, 0, seqBytes.Length);

            byte[] methodBytes = encoder.GetBytes(data.GetMethod());
            ms.Write(methodBytes, 0, methodBytes.Length);

            byte[] payloadBytes = null;

            if (this.IsJson(data)) {

                payloadBytes = encoder.GetBytes(data.JsonPayload());
            }

            if (this.IsMsgPack(data)) {

                payloadBytes = data.MsgpackPayload();
            }

            ms.Write(payloadBytes, 0, payloadBytes.Length);

            return this.StreamToBytes(ms);
        }

        private byte[] EnCodeAnswer(FPData data) {

            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            MemoryStream ms = this.BuildHeader(data, 16 + data.GetPsize());

            ms.WriteByte((byte)data.GetSS());

            byte[] psizeBytes = BitConverter.GetBytes(data.GetPsize());
            ms.Write(psizeBytes, 0, psizeBytes.Length);

            byte[] seqBytes = BitConverter.GetBytes (data.GetSeq());
            ms.Write(seqBytes, 0, seqBytes.Length);

            byte[] payloadBytes = null;

            if (this.IsJson(data)) {

                payloadBytes = encoder.GetBytes(data.JsonPayload());
            }

            if (this.IsMsgPack(data)) {

                payloadBytes = data.MsgpackPayload();
            }

            ms.Write(payloadBytes, 0, payloadBytes.Length);

            return this.StreamToBytes(ms);
        }


        private MemoryStream BuildHeader(FPData data, int size) {

            MemoryStream ms = new MemoryStream();

            if (this.IsHttp(data)) {

                ms.Write(FPConfig.HTTP_MAGIC, 0, 4);
            }

            if (this.IsTcp(data)) {

                ms.Write(FPConfig.TCP_MAGIC, 0, 4);
            }

            ms.WriteByte(FPConfig.FPNN_VERSION[data.GetVersion()]);

            if (this.IsJson(data)) {

                ms.WriteByte(FPConfig.FP_FLAG[data.GetFlag()]);
            }

            if (this.IsMsgPack(data)) {

                ms.WriteByte(FPConfig.FP_FLAG[data.GetFlag()]);
            }

            ms.WriteByte(FPConfig.FP_MESSAGE_TYPE[data.GetMtype()]);

            return ms;
        }

        public UInt32 ReadUI32 (byte[] ui32in) {

            return (UInt32)(((ui32in [0] & 0xff) << 24) | ((ui32in [1] & 0xff) << 16) | ((ui32in [2] & 0xff) << 8) | ((ui32in [3] & 0xff)));
        }
        
        public UInt32 ReadUI32 (byte[] ui32in, int off) {

            return (UInt32)(((ui32in [off] & 0xff) << 24) | ((ui32in [off + 1] & 0xff) << 16) | ((ui32in [off + 2] & 0xff) << 8) | ((ui32in [off + 3] & 0xff)));
        }

        public string GetString(byte[] bytes) {

            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public byte[] GetByteArrayRange(byte[] arr, int start, int end) {

            byte[] arrNew = new byte[end - start + 1];

            int j = 0;

            for (int i = start; i <= end; i++) {

                arrNew[j++] = arr[i];
            }

            return arrNew;
        }

        public byte[] StreamToBytes(MemoryStream stream) {

            return ((MemoryStream)stream).ToArray();                
        }

        public bool BytesCompare_Base64(byte[] b1, byte[] b2) {

            if (b1 == null || b2 == null) {

                return false;
            }

            if(b1.Length != b2.Length) {

                return false;
            }

            return string.Compare(Convert.ToBase64String(b1), Convert.ToBase64String(b2), false) == 0 ? true : false;
        }
    }
}