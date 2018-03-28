using System;
using System.IO;
using System.Text;

namespace Fpnn.Protocol
{
	public class FPQuest : FPMessage
	{
		public string method;
		private bool isOnewayQuest;

		public FPQuest (string method, bool oneway = false, FP_Pack_Type ptype = FP_Pack_Type.FP_PACK_MSGPACK)
		{
			_create (method, oneway, ptype);
		}

		public FPQuest (FPMessage msg)
			: base(msg.magic, msg.version, msg.flag, msg.mtype, msg.ss, msg.psize, msg.seqNum, msg.payload)
		{
			if (!msg.isSupportPack())
			{
				throw new System.Exception("Create quest from raw, But Not Json OR Msgpack");
			}
			if (!msg.isSupportProto())
			{
				throw new System.Exception("Create quest from raw, Not TCP OR HTTP");
			}
			setMethod(PackCommon.getStringByOffset(payload, 0, ss));
		}

		public void setMethod (string m)
		{
			method = m;
		}

		private void _create (string method, bool oneway = false, FP_Pack_Type ptype = FP_Pack_Type.FP_PACK_MSGPACK)
		{
			if (ptype == FP_Pack_Type.FP_PACK_MSGPACK) {
				setFlag (FP_FLAG.FP_FLAG_MSGPACK);
			} else if (ptype == FP_Pack_Type.FP_PACK_JSON) {
				setFlag (FP_FLAG.FP_FLAG_JSON);
			} else {
				throw new System.Exception ("Create Quest for unknow ptype");
			}
			setMType (oneway ? FPMessageType.FP_MT_ONEWAY : FPMessageType.FP_MT_TWOWAY);
			setSS (Convert.ToByte(method.Length));
			setMethod (method);
			this.isOnewayQuest = oneway;
		}

		public void genSeqNum()
		{
			if (!this.isOnewayQuest)
			{
				setSeqNum(FPMessage.nextSeqNum());
			}
		}

		public byte[] raw ()
		{
			MemoryStream m = new MemoryStream();
			ASCIIEncoding encoder = new ASCIIEncoding();

			byte[] magicBytes = PackCommon.getUI32Byte (magic);
			m.Write (magicBytes, 0, magicBytes.Length);
			m.WriteByte (version);
			m.WriteByte (flag);
			m.WriteByte (mtype);
			m.WriteByte (ss);
			byte[] psizeBytes = BitConverter.GetBytes (psize);
			m.Write (psizeBytes, 0, psizeBytes.Length);
			if (isTwoWay ()) {
				byte[] seqNumBytes = BitConverter.GetBytes (seqNum);
				m.Write (seqNumBytes, 0, seqNumBytes.Length);
			}
			m.Write(encoder.GetBytes(method), 0, method.Length);
			m.Write(payload, 0, payload.Length);

			return PackCommon.streamToBytes(m);
		}
	}
}

