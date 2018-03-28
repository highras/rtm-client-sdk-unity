using System;

namespace Fpnn.Protocol
{
	public enum FP_ST {
		FP_ST_OK = 0,
		FP_ST_ERROR = 1,
		FP_ST_HTTP_OK = 200,
		FP_ST_HTTP_ERROR = 500,
	};

	public class FPAnswer : FPMessage
	{
		public FP_ST status = FP_ST.FP_ST_OK;

		public FPAnswer (UInt32 magic, byte version, byte flag, byte mtype, byte ss, UInt32 psize, UInt32 seq, byte[] payload)
			: base(magic, version, flag, mtype, ss, psize, seq, payload)
		{
			if (!isSupportPack ()) {
				throw new System.Exception ("Create answer from raw, But Not Json OR Msgpack");
			}
			if (!isSupportProto ()) {
				throw new System.Exception ("Create answer from raw, Not TCP OR HTTP");
			}
			if (isAnswer () && ss == 0) {
				setStatus (FP_ST.FP_ST_OK);
			} else {
				setStatus (FP_ST.FP_ST_ERROR);
			}
		}

		public FPAnswer(FPMessage msg) 
			: base(msg.magic, msg.version, msg.flag, msg.mtype, msg.ss, msg.psize, msg.seqNum, msg.payload)
		{
			if (!msg.isSupportPack())
			{
				throw new System.Exception("Create answer from raw, But Not Json OR Msgpack");
			}
			if (!msg.isSupportProto())
			{
				throw new System.Exception("Create answer from raw, Not TCP OR HTTP");
			}
			if (msg.isAnswer() && ss == 0)
			{
				setStatus(FP_ST.FP_ST_OK);
			}
			else {
				setStatus(FP_ST.FP_ST_ERROR);
			}
		}

		public void setStatus (FP_ST st)
		{
			status = st;
		}
	}
}

