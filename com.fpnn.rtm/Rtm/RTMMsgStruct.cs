using System;
using MsgPack;
using MsgPack.Serialization;

namespace com.fpnn.rtm
{
	public class GroupMsg
	{	
		[MessagePackMember(0)]
		public long id;

		[MessagePackMember(1)]
		public long from;

		[MessagePackMember(2)]
		public byte mtype;

		[MessagePackMember(3)]
		public byte ftype;

		[MessagePackMember(4)]
		public long mid;

		[MessagePackMember(5)]
		public string msg;

		[MessagePackMember(6)]
		public string attrs;

		[MessagePackMember(7)]
		public int mtime;
	}

	public class RoomMsg
	{
		[MessagePackMember(0)]
		public long id;

		[MessagePackMember(1)]
		public long from;

		[MessagePackMember(2)]
		public byte mtype;

		[MessagePackMember(3)]
		public byte ftype;

		[MessagePackMember(4)]
		public long mid;

		[MessagePackMember(5)]
		public string msg;

		[MessagePackMember(6)]
		public string attrs;

		[MessagePackMember(7)]
		public int mtime;
	}

	public class BroadcastMsg
	{
		[MessagePackMember(0)]
		public long id;

		[MessagePackMember(1)]
		public long from;

		[MessagePackMember(2)]
		public byte mtype;

		[MessagePackMember(3)]
		public byte ftype;

		[MessagePackMember(4)]
		public long mid;

		[MessagePackMember(5)]
		public string msg;

		[MessagePackMember(6)]
		public string attrs;

		[MessagePackMember(7)]
		public int mtime;
	}

	public class P2PMsg
	{
		[MessagePackMember(0)]
		public long id;

		[MessagePackMember(1)]
		public long other_uid;

		[MessagePackMember(2)]
		public byte direction;

		[MessagePackMember(3)]
		public byte mtype;

		[MessagePackMember(4)]
		public byte ftype;

		[MessagePackMember(5)]
		public long mid;

		[MessagePackMember(6)]
		public string msg;

		[MessagePackMember(7)]
		public string attrs;

		[MessagePackMember(8)]
		public int mtime;
	}
}

