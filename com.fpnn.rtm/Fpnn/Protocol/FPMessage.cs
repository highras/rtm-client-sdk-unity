using System;

namespace Fpnn.Protocol
{
	public enum FP_FLAG {
		FP_FLAG_MSGPACK = 0x80,
		FP_FLAG_JSON = 0x40,
		FP_FLAG_ZIP  = 0x20,
		FP_FLAG_ENCRYPT = 0x10,
	};
	
	public enum FP_Pack_Type {
		FP_PACK_MSGPACK = 0,
		FP_PACK_JSON = 1,
	};

	public enum FPMessageType {
		FP_MT_ONEWAY = 0,
		FP_MT_TWOWAY = 1,
		FP_MT_ANSWER = 2,
	};

	public class FPMessage
	{
		public static UInt32 tcpMagic = PackCommon.readUI32 (
			new byte[4]{
				Convert.ToByte('F'), 
				Convert.ToByte('P'), 
				Convert.ToByte('N'), 
				Convert.ToByte('N')
			}, 0);
		
		public static UInt32 httpMagic = PackCommon.readUI32 (
			new byte[4]{
				Convert.ToByte('P'), 
				Convert.ToByte('O'), 
				Convert.ToByte('S'), 
				Convert.ToByte('T')
			}, 0);
		
		public UInt32 magic = tcpMagic;
		public byte version = 1;
		public byte flag = 0;
		public byte mtype = 0;
		public byte ss = 0;
		public UInt32 psize = 0;
		public Int64 ctime = 0;
		public UInt32 seqNum = 0;
		public byte[] payload;

		public static UInt32 nextSeq = 1;
		public static object seqLock = new object();
		public static UInt32 nextSeqNum ()
		{
			lock (seqLock)
			{
				return nextSeq++;
			}
		}

		public string str()
		{
			return "magic: " + magic + ", version: " + Convert.ToInt32(version) + ", flag: "
					+ Convert.ToInt32(flag) + ", mtype: " + Convert.ToInt32(ss) + ", psize: "
					+ psize + ", ctime: " + ctime + ", seqNum: " + seqNum
					+ ", payload: " + PackCommon.getString(payload);                                          
		}

		
		public FPMessage ()
		{
		}

		public FPMessage(UInt32 magic, byte version, byte flag, byte mtype, byte ss, UInt32 psize, UInt32 seq, byte[] payload)
		{
			setMagic(magic);
			setVersion(version);
			setFlag((FP_FLAG)flag);
			setMType((FPMessageType)mtype);
			setSS(ss);
			setPayloadSize(psize);
			setSeqNum(seq);
			setPayload(payload);
			setCTime(PackCommon.getMilliTimestamp());
		}

		public void setMagic (UInt32 m)
		{
			magic = m;
		}

		public void setVersion (byte v)  
		{ 
			version = v;
		}

		public void setFlag (FP_FLAG f)
		{ 
			flag |= Convert.ToByte(f); 
		}

		public void setMType (FPMessageType t)
		{
			mtype = Convert.ToByte (t);
		}

		public void setSS (byte s)
		{
			ss = s;
		}

		public void setPayloadSize (UInt32 size)
		{
			psize = size;
		}

		public void setSeqNum (UInt32 seq)
		{
			seqNum = seq;
		}

		public void setPayload (byte[] p)
		{
			payload = p;
		}

		public void setCTime (Int64 c)
		{
			ctime = c;
		}
		
		public bool isHTTP ()
		{
			return magic == httpMagic;
		}

		public bool isTCP ()
		{
			return magic == tcpMagic;
		}
		
		public bool isMsgPack ()
		{
			return (flag & Convert.ToByte(FP_FLAG.FP_FLAG_MSGPACK)) 
					== Convert.ToByte(FP_FLAG.FP_FLAG_MSGPACK);
		}

		public bool isJson ()
		{
			return (flag & Convert.ToByte(FP_FLAG.FP_FLAG_JSON)) 
				== Convert.ToByte(FP_FLAG.FP_FLAG_JSON);
		}

		public bool isZip () 
		{
			return (flag & Convert.ToByte(FP_FLAG.FP_FLAG_ZIP)) 
				== Convert.ToByte(FP_FLAG.FP_FLAG_ZIP);
		}

		public bool isEncrypt () 
		{
			return (flag & Convert.ToByte(FP_FLAG.FP_FLAG_ENCRYPT)) 
				== Convert.ToByte(FP_FLAG.FP_FLAG_ENCRYPT);
		}

		public bool isOneWay ()
		{
			return mtype == Convert.ToByte(FPMessageType.FP_MT_ONEWAY);
		}

		public bool isTwoWay ()
		{
			return mtype == Convert.ToByte(FPMessageType.FP_MT_TWOWAY);
		}

		public bool isQuest ()
		{
			return isTwoWay() || isOneWay();
		}

		public bool isAnswer ()
		{
			return mtype == Convert.ToByte(FPMessageType.FP_MT_ANSWER);
		}

		public bool isSupportPack ()
		{
			return isMsgPack() != isJson();
		}

		public bool isSupportProto ()
		{
			return isTCP() || isHTTP();
		}

		public UInt32 bodyLen ()
		{
			UInt32 len = psize;
			if (isTwoWay ()) {
				len += ss;
				len += sizeof(UInt32); // seq
			} else if (isAnswer ()) {
				len += sizeof(UInt32); // seq
			} else if (isOneWay ()) {
				len += ss;
			}
			return len;
		}
	}
}

