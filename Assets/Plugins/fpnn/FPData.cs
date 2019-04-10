using System;
using System.Collections;
using System.IO;
using System.Text;

namespace com.fpnn
{
    public enum FP_FLAG {
		FP_FLAG_MSGPACK = 0x80,
		FP_FLAG_JSON = 0x40,
		FP_FLAG_ZIP  = 0x20,
		FP_FLAG_ENCRYPT = 0x10,
	};

	public enum FP_MSG_TYPE {
		FP_MT_ONEWAY = 0,
		FP_MT_TWOWAY = 1,
		FP_MT_ANSWER = 2,
	};

	public enum READ_STEP {
		READ_HEADER_NO_ENCRYPTOR = 0,
		READ_LEFT_NO_ENCRYPTOR = 1,
		READ_HEADER_ENCRYPTOR = 2,
		READ_LEFT_ENCRYPTOR = 3,
	};

    public class FPData: ICloneable
    {
        public static UInt32 tcpMagic = FPCommon.ReadUI32 (
			new byte[4]{
				Convert.ToByte('F'), 
				Convert.ToByte('P'), 
				Convert.ToByte('N'), 
				Convert.ToByte('N')
			}, 0);
		
        public UInt32 magic = tcpMagic;
		public byte version = 1;
		public byte flag = 0;
		public byte mtype = 0;
		public byte ss = 0;
		public UInt32 psize = 0;
		public UInt32 seqNum = 0;
        public static UInt32 nextSeq = 1;
        public static object seqLock = new object();
        public string method = "";
		public byte[] payload;
        public UInt32 nextLength = 12;
		public bool isEncryptor = false;
		public READ_STEP step;

		public object Clone()
    	{
        	return this.MemberwiseClone();
    	}

        public string Str()
		{
			return "magic: " + magic + ", version: " + Convert.ToInt32(version) + ", flag: "
					+ Convert.ToInt32(flag) + ", mtype: " + Convert.ToInt32(ss) + ", psize: "
					+ psize + ", seqNum: " + seqNum
					+ ", payload: " + FPCommon.GetString(payload);                                          
		}

        public FPData()
        {
			Reset();
        }

		public FPData(bool encryptor)
        {
			isEncryptor = encryptor;
			Reset();
        }

        public UInt32 GenSeqNum()
        {
            UInt32 seq = NextSeqNum();
            SetSeqNum(seq);
            return seq;
        }

		public UInt32 GetSeqNum()
		{
			return seqNum;
		}

        public static UInt32 NextSeqNum()
		{
			lock (seqLock)
			{
				return nextSeq++;
			}
		}

        public void SetFlag(FP_FLAG f)
        {
            flag |= Convert.ToByte(f);
        }

        public void SetMType (FP_MSG_TYPE t)
		{
			mtype = Convert.ToByte (t);
		}

        public void SetMethod(string m)
        {
            method = m;
            SetSS(Convert.ToByte(method.Length));
        }

        public void SetSS (byte s)
		{
			ss = s;
		}

        public void SetSeqNum (UInt32 seq)
		{
			seqNum = seq;
		}

        public bool IsOneWay()
		{
			return mtype == Convert.ToByte(FP_MSG_TYPE.FP_MT_ONEWAY);
		}

		public bool IsTwoWay()
		{
			return mtype == Convert.ToByte(FP_MSG_TYPE.FP_MT_TWOWAY);
		}

		public bool IsQuest ()
		{
			return IsTwoWay() || IsOneWay();
		}

		public bool IsAnswer ()
		{
			return mtype == Convert.ToByte(FP_MSG_TYPE.FP_MT_ANSWER);
		}

        public void SetPayload(byte[] buffer)
        {
            payload = buffer;
            psize = (UInt32)buffer.Length;
        }

		public bool Decode(byte[] buffer)
		{
			if (step == READ_STEP.READ_HEADER_NO_ENCRYPTOR)
			{
				magic = FPCommon.ReadUI32(FPCommon.GetByteArrayRange(buffer, 0, 3));
				version = buffer[4];
				flag = buffer[5];
				mtype = buffer[6];
				ss = buffer[7];
				psize = BitConverter.ToUInt32(FPCommon.GetByteArrayRange(buffer, 8, 11), 0);
				
				if (mtype == 0)
					nextLength = ss + psize;
				else if (mtype == 1)
					nextLength = ss + psize + 4;
				else if (mtype == 2)
					nextLength = psize + 4;
				step = READ_STEP.READ_LEFT_NO_ENCRYPTOR;
				return false;
			}
			if (step == READ_STEP.READ_LEFT_NO_ENCRYPTOR)
			{
				if (mtype == 0)
				{
					method = FPCommon.GetStringByOffset(buffer, 0, ss);
					payload = FPCommon.GetByteArrayRange(buffer, ss, buffer.Length - 1);
				}
				if (mtype == 1)
				{
					seqNum = BitConverter.ToUInt32(FPCommon.GetByteArrayRange(buffer, 0, 3), 0);
					method = FPCommon.GetStringByOffset(buffer, 4, ss);
					payload = FPCommon.GetByteArrayRange(buffer, 4 + ss, buffer.Length - 1);
				}
				if (mtype == 2)
				{
					seqNum = BitConverter.ToUInt32(FPCommon.GetByteArrayRange(buffer, 0, 3), 0);
					payload = FPCommon.GetByteArrayRange(buffer, 4, buffer.Length - 1);
				}
				step = READ_STEP.READ_HEADER_NO_ENCRYPTOR;
				return true;
			}

			// TODO Encryptor
			return false;
		}

        public void Reset()
        {
            magic = tcpMagic;
            version = 1;
            flag = 0;
            mtype = 0;
            ss = 0;
            psize = 0;
            seqNum = 0;
            method = "";
			if (isEncryptor)
			{
				nextLength = 4;
				step = READ_STEP.READ_HEADER_ENCRYPTOR;
			}
			else
			{
            	nextLength = 12;
				step = READ_STEP.READ_HEADER_NO_ENCRYPTOR;
			}
        }

        public byte[] Raw()
        {
            MemoryStream m = new MemoryStream();
			ASCIIEncoding encoder = new ASCIIEncoding();

			byte[] magicBytes = FPCommon.GetUI32Byte(magic);
			m.Write(magicBytes, 0, magicBytes.Length);
			m.WriteByte (version);
			m.WriteByte (flag);
			m.WriteByte (mtype);
			m.WriteByte (ss);
			byte[] psizeBytes = BitConverter.GetBytes (psize);
			m.Write (psizeBytes, 0, psizeBytes.Length);
			if (IsTwoWay()) {
				byte[] seqNumBytes = BitConverter.GetBytes (seqNum);
				m.Write(seqNumBytes, 0, seqNumBytes.Length);
			}
			m.Write(encoder.GetBytes(method), 0, method.Length);
			m.Write(payload, 0, payload.Length);

			return FPCommon.StreamToBytes(m);
        }

		public static byte[] QuestAnswerRaw(UInt32 seqNum, byte flag)
		{
			MemoryStream m = new MemoryStream();
			byte[] magicBytes = FPCommon.GetUI32Byte(tcpMagic);
			m.Write(magicBytes, 0, magicBytes.Length);
			m.WriteByte(1);
			m.WriteByte(flag);
			m.WriteByte(Convert.ToByte(FP_MSG_TYPE.FP_MT_ANSWER));
			m.WriteByte(0);

			byte[] RAW_PSIZE = {0x01, 0x00, 0x00, 0x00};
			m.Write(RAW_PSIZE, 0, RAW_PSIZE.Length);
			byte[] seqNumBytes = BitConverter.GetBytes(seqNum);
			m.Write(seqNumBytes, 0, seqNumBytes.Length);
			byte[] RAW_PAYLOAD = {0x80};
			m.Write(RAW_PAYLOAD, 0, RAW_PAYLOAD.Length);
			return FPCommon.StreamToBytes(m);
		}

    }
}