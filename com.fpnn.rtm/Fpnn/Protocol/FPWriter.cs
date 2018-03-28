using System;
using System.IO;
using MsgPack;

namespace Fpnn.Protocol
{
	public class FPWriter
	{

		protected Packer packer;
		protected MemoryStream stream = new MemoryStream();

		public void param<T> (T v)
		{
			packer.Pack<T> (v);
		}

		public void paramBinary (byte[] v, int size)
		{
			packer.PackBinaryHeader (size);
			packer.PackBinary (v);
		}

		public void paramNull ()
		{
			packer.PackNull ();
		}

		public void paramArray (string k, int size)
		{
			packer.Pack (k);
			packer.PackArrayHeader (size);
		}

		public void paramMap (string k, int size)
		{
			packer.Pack (k);
			packer.PackMapHeader (size);
		}

		public void param<T> (string k, T v)
		{
			packer.Pack(k);
			packer.Pack<T>(v);
		}

		public void paramBinary (string k, byte[] v, int size)
		{
			packer.Pack (k);
			packer.PackBinaryHeader (size);
			packer.PackBinary (v);
		}

		public void paramNull (string k)
		{
			packer.Pack (k);
			packer.PackNull ();
		}

		public FPWriter (int size)
		{
			packer = Packer.Create(stream);
			packer.PackMapHeader (size);
		}

		// only support pack a map
		public FPWriter ()
		{	
		}
	}

	public class FPQWriter : FPWriter
	{
		FPQuest quest;

		public FPQWriter (int size, string method, bool oneway = false, FP_Pack_Type ptype = FP_Pack_Type.FP_PACK_MSGPACK)
		: base(size) {
			quest = new FPQuest (method, oneway, ptype);
		}

		// only support pack a map
		public FPQWriter (string method, bool oneway = false, FP_Pack_Type ptype = FP_Pack_Type.FP_PACK_MSGPACK)
		: base() {
			quest = new FPQuest (method, oneway, ptype);
		}

		public FPQuest take ()
		{
			byte[] payload = stream.ToArray ();
			quest.setPayload (payload);
			quest.setPayloadSize (Convert.ToUInt32(payload.Length));
			quest.setCTime (DateTime.Now.Millisecond);
			return quest;
		}

	}

	public class FPAWriter : FPWriter
	{
		FPAnswer answer;

	}
}

