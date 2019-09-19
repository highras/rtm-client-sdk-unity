using System;
using System.IO;
using System.Text;
using System.Collections;

// using UnityEngine;

namespace com.fpnn {

    public class FPPackage {

        public string GetKeyCallback(FPData data) {

            string seq = "0";

            if (data != null) {

                seq = Convert.ToString(data.GetSeq());
            }

            StringBuilder sb = new StringBuilder(10);

            sb.Append("FPNN_");
            sb.Append(seq);

            return sb.ToString();
        }

        public bool IsHttp(FPData data) {

            if (data != null) {

                return BytesCompare(FPConfig.HTTP_MAGIC, data.GetMagic());
            }

            return false;
        }

        public bool IsTcp(FPData data) {

            if (data != null) {

                return BytesCompare(FPConfig.TCP_MAGIC, data.GetMagic());
            }

            return false;
        }

        public bool IsMsgPack(FPData data) {

            if (data != null) {

                return 1 == data.GetFlag();
            }

            return false;
        }

        public bool IsJson(FPData data) {

            if (data != null) {

                return 0 == data.GetFlag();
            }

            return false;
        }

        public bool IsOneWay(FPData data) {

            if (data != null) {

                return 0 == data.GetMtype();
            }

            return false;
        }

        public bool IsTwoWay(FPData data) {

            if (data != null) {

                return 1 == data.GetMtype();
            }

            return false;
        }

        public bool IsQuest(FPData data) {

            return this.IsTwoWay(data) || this.IsOneWay(data);
        }

        public bool IsAnswer(FPData data) {

            if (data != null) {

                return 2 == data.GetMtype();
            }

            return false;
        }

        public bool IsSupportPack(FPData data) {

            return this.IsMsgPack(data) != this.IsJson(data);
        }

        public bool CheckVersion(FPData data) {

            if (data == null) {

                return false;
            }

            if (data.GetVersion() < 0) {

                return false;
            }

            if (data.GetVersion() >= FPConfig.FPNN_VERSION.Length) {

                return false;
            }

            return true;
        }

        public FPData PeekHead(byte[] bytes) {

            if (bytes != null && bytes.Length >= 12) {

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

            return null;
        }

        public bool DeCode(FPData data) {

            if (data != null) {

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
            }

            return false;
        }

        private bool DeCodeOneWay(byte[] bytes, FPData data) {

            if (bytes != null && (bytes.Length == data.GetSS() + data.GetPsize())) {

                data.SetMethod(this.GetString(this.GetByteArrayRange(bytes, 0, data.GetSS() - 1)));

                if (this.IsJson(data)) {

                    data.SetPayload(this.GetString(this.GetByteArrayRange(bytes, data.GetSS(), bytes.Length - 1)));
                    return true;
                }

                if (this.IsMsgPack(data)) {

                    data.SetPayload(this.GetByteArrayRange(bytes, data.GetSS(), bytes.Length - 1));
                    return true;
                }
            }

            return false;
        }

        private bool DeCodeTwoWay(byte[] bytes, FPData data) {

            if (bytes != null && (bytes.Length == 4 + data.GetSS() + data.GetPsize())) {

                data.SetSeq((int)BitConverter.ToUInt32(this.GetByteArrayRange(bytes, 0, 3), 0));
                data.SetMethod(this.GetString(this.GetByteArrayRange(bytes, 4, data.GetSS() + 4 - 1)));

                if (this.IsJson(data)) {

                    data.SetPayload(this.GetString(this.GetByteArrayRange(bytes, 4 + data.GetSS(), bytes.Length - 1)));
                    return true;
                }

                if (this.IsMsgPack(data)) {

                    data.SetPayload(this.GetByteArrayRange(bytes, 4 + data.GetSS(), bytes.Length - 1));
                    return true;
                }
            }

            return false;
        }

        private bool DeCodeAnswer(byte[] bytes, FPData data) {

            if (bytes != null && (bytes.Length == 4 + data.GetPsize())) {

                data.SetSeq((int)BitConverter.ToUInt32(this.GetByteArrayRange(bytes, 0, 3), 0));

                if (this.IsJson(data)) {

                    data.SetPayload(this.GetString(this.GetByteArrayRange(bytes, 4, bytes.Length - 1)));
                    return true;
                }

                if (this.IsMsgPack(data)) {

                    data.SetPayload(this.GetByteArrayRange(bytes, 4, bytes.Length - 1));
                    return true;
                }
            }

            return false;
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

            if (data != null) {

                System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                MemoryStream ms = this.BuildHeader(data, 12 + data.GetSS() + data.GetPsize());

                ms.WriteByte((byte)data.GetSS());

                byte[] psizeBytes = BitConverter.GetBytes(data.GetPsize());
                ms.Write(psizeBytes, 0, psizeBytes.Length);

                string method = (data.GetMethod() != null) ? data.GetMethod() : "";
                byte[] methodBytes = encoder.GetBytes(method);
                ms.Write(methodBytes, 0, methodBytes.Length);

                byte[] payloadBytes = null;

                if (this.IsJson(data)) {

                    string json = (data.JsonPayload() != null) ? data.JsonPayload() : "";
                    payloadBytes = encoder.GetBytes(json);
                }

                if (this.IsMsgPack(data)) {

                    payloadBytes = data.MsgpackPayload();
                }

                if (payloadBytes != null) {

                    ms.Write(payloadBytes, 0, payloadBytes.Length);
                }

                return this.StreamToBytes(ms);
            }

            return null;
        }

        private byte[] EnCodeTwoway(FPData data) {

            if (data != null) {

                System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                MemoryStream ms = this.BuildHeader(data, 16 + data.GetSS() + data.GetPsize());

                ms.WriteByte((byte)data.GetSS());

                byte[] psizeBytes = BitConverter.GetBytes(data.GetPsize());
                ms.Write(psizeBytes, 0, psizeBytes.Length);

                byte[] seqBytes = BitConverter.GetBytes(data.GetSeq());
                ms.Write(seqBytes, 0, seqBytes.Length);

                string method = (data.GetMethod() != null) ? data.GetMethod() : "";
                byte[] methodBytes = encoder.GetBytes(method);
                ms.Write(methodBytes, 0, methodBytes.Length);

                byte[] payloadBytes = null;

                if (this.IsJson(data)) {

                    string json = (data.JsonPayload() != null) ? data.JsonPayload() : "";
                    payloadBytes = encoder.GetBytes(json);
                }

                if (this.IsMsgPack(data)) {

                    payloadBytes = data.MsgpackPayload();
                }

                if (payloadBytes != null) {

                    ms.Write(payloadBytes, 0, payloadBytes.Length);
                }

                return this.StreamToBytes(ms);
            }

            return null;
        }

        private byte[] EnCodeAnswer(FPData data) {

            if (data != null) {

                System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                MemoryStream ms = this.BuildHeader(data, 16 + data.GetPsize());

                ms.WriteByte((byte)data.GetSS());

                byte[] psizeBytes = BitConverter.GetBytes(data.GetPsize());
                ms.Write(psizeBytes, 0, psizeBytes.Length);

                byte[] seqBytes = BitConverter.GetBytes(data.GetSeq());
                ms.Write(seqBytes, 0, seqBytes.Length);

                byte[] payloadBytes = null;

                if (this.IsJson(data)) {

                    string json = (data.JsonPayload() != null) ? data.JsonPayload() : "";
                    payloadBytes = encoder.GetBytes(json);
                }

                if (this.IsMsgPack(data)) {

                    payloadBytes = data.MsgpackPayload();
                }

                if (payloadBytes != null) {

                    ms.Write(payloadBytes, 0, payloadBytes.Length);
                }

                return this.StreamToBytes(ms);
            }

            return null;
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

        private UInt32 ReadUI32 (byte[] ui32in) {

            return (UInt32)(((ui32in [0] & 0xff) << 24) | ((ui32in [1] & 0xff) << 16) | ((ui32in [2] & 0xff) << 8) | ((ui32in [3] & 0xff)));
        }
        
        private UInt32 ReadUI32 (byte[] ui32in, int off) {

            return (UInt32)(((ui32in [off] & 0xff) << 24) | ((ui32in [off + 1] & 0xff) << 16) | ((ui32in [off + 2] & 0xff) << 8) | ((ui32in [off + 3] & 0xff)));
        }

        private string GetString(byte[] bytes) {

            string str = null;

            try {

                str = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            } catch (Exception ex) {

                ErrorRecorderHolder.recordError(ex);
            }

            return str;
        }

        public byte[] GetByteArrayRange(byte[] arr, int start, int end) {

            if (arr == null || arr.Length == 0) {

                return arr;
            }

            int len = end - start + 1;

            if (len <= 0) {

                return new byte[0];
            }

            byte[] subarr = new byte[len];

            try {

                Array.Copy(arr, start, subarr, 0, len);
            } catch (Exception ex) {

                subarr = new byte[0];
                ErrorRecorderHolder.recordError(ex);
            }

            return subarr;
        }

        private byte[] StreamToBytes(MemoryStream stream) {

            return ((MemoryStream)stream).ToArray();                
        }

        private bool BytesCompare(byte[] b1, byte[] b2) {

            if (b1 == null || b2 == null) {

                return false;
            }

            if(b1.Length != b2.Length) {

                return false;
            }

            for (var i = 0; i < b1.Length; i++) {

               if (b1[i] != b2[i]) {

                   return false;
               }
            }

            return true;
        }
    }
}