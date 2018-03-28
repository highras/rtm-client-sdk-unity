using System;
using System.IO;
using System.Text;

namespace Fpnn.Protocol
{
	public class FPReply : FPMessage
	{
		public FPReply(UInt32 seqNum, FP_Pack_Type ptype = FP_Pack_Type.FP_PACK_MSGPACK)
			: base()
		{
			_create (seqNum, ptype);
		}

		private void _create (UInt32 seqNum, FP_Pack_Type ptype = FP_Pack_Type.FP_PACK_MSGPACK)
		{
			if (ptype == FP_Pack_Type.FP_PACK_MSGPACK) {
				setFlag (FP_FLAG.FP_FLAG_MSGPACK);
			} else if (ptype == FP_Pack_Type.FP_PACK_JSON) {
				setFlag (FP_FLAG.FP_FLAG_JSON);
			} else {
				throw new System.Exception ("Create Reply for unknow ptype");
			}
			setMType (FPMessageType.FP_MT_ANSWER);
			setSS (0);
			setSeqNum (seqNum);
		}

		public byte[] raw ()
		{
			MemoryStream m = new MemoryStream();
			byte[] magicBytes = PackCommon.getUI32Byte (magic);
			m.Write (magicBytes, 0, magicBytes.Length);
			m.WriteByte (version);
			m.WriteByte (flag);
			m.WriteByte (mtype);
			m.WriteByte (ss);

			byte[] RAW_PSIZE = {0x01, 0x00, 0x00, 0x00};
			m.Write (RAW_PSIZE, 0, RAW_PSIZE.Length);
			byte[] seqNumBytes = BitConverter.GetBytes (seqNum);
			m.Write (seqNumBytes, 0, seqNumBytes.Length);
			byte[] RAW_PAYLOAD = {0x80};
			m.Write(RAW_PAYLOAD, 0, RAW_PAYLOAD.Length);
			return PackCommon.streamToBytes(m);
		}
	}
}

