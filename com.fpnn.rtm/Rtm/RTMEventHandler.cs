using System;
using System.Collections.Generic;
namespace com.fpnn.rtm
{
	public class RTMEventHandler
	{
		public virtual void bye() { }
		public virtual void kickout() { }
		public virtual void pushMessage(long from, byte mtype, byte ftype, long mid, string msg, string attrs) { }
		public virtual void pushGroupMessage(long gid, long from, byte mtype, byte ftype, long mid, string msg, string attrs) { }
		public virtual void pushRoomMessage(long rid, long from, byte mtype, byte ftype, long mid, string msg, string attrs) { }
		public virtual void pushBroadcastMessage(long from, byte mtype, byte ftype, long mid, string msg, string attrs) { }
		public virtual void transMessage(long from, long mid, long omid, string msg) { }
		public virtual void transGroupMessage(long gid, long from, long mid, long omid, string msg) { }
		public virtual void transRoomMessage(long rid, long from, long mid, long omid, string msg) { }
		public virtual void transBroadcastMessage(long from, long mid, long omid, string msg) { }
		public virtual void pushUnread(List<long> p2p, List<long> group, bool haveBroadcast) { }
	}
}
