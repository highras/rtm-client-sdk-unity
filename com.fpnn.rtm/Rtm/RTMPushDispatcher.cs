using System;
using System.Reflection;
using System.Collections.Generic;
using Fpnn.Connection;
using Fpnn.Protocol;
namespace com.fpnn.rtm
{
	public class RTMPushDispatcher: FpnnClientProcessor
	{
		RTMEventHandler processor;
		RTMPushCache midCache;
		RTMClient client;

		public RTMPushDispatcher(RTMEventHandler processor, RTMClient client)
		{
			this.processor = processor;
			this.client = client;
			this.midCache = new RTMPushCache(1000); // cache mid num
		}

		private void procMethod(string method, object[] args)
		{
			MethodInfo mi = this.processor.GetType().GetMethod(method);
			if (mi != null)
			{
				mi.Invoke(this.processor, args);
			}
		}

		public void ping(FPQReader reader)
		{
			this.client.lastRecvPingTime = PackCommon.getMilliTimestamp();
		}

		public void bye(FPQReader reader)
		{
			try
			{
				this.procMethod("bye", new object[] { });
			} 
			catch (Exception e) 
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void kickout(FPQReader reader)
		{
			try
			{
				this.procMethod("kickout", new object[] { });
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void pushmsg(FPQReader reader)
		{
			try 
			{
				long from = reader.want<long>("from");
				byte mtype = reader.want<byte>("mtype");
				byte ftype = reader.want<byte>("ftype");
				long mid = reader.want<long>("mid");
				string msg = reader.want<string>("msg");
				string attrs = reader.want<string>("attrs");

				if (!this.midCache.containsKey(mid, from))
					this.procMethod("pushMessage", new object[] { from, mtype,ftype,mid, msg, attrs });
			} 
			catch (Exception e) 
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void pushgroupmsg(FPQReader reader)
		{
			try
			{
				long gid = reader.want<long>("gid");
				long from = reader.want<long>("from");
				byte mtype = reader.want<byte>("mtype");
				byte ftype = reader.want<byte>("ftype");
				long mid = reader.want<long>("mid");
				string msg = reader.want<string>("msg");
				string attrs = reader.want<string>("attrs");

				if (!this.midCache.containsKey(mid, from))
					this.procMethod("pushGroupMessage", new object[] { gid, from, mtype, ftype, mid, msg, attrs });
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void pushroommsg(FPQReader reader)
		{
			try
			{
				long rid = reader.want<long>("rid");
				long from = reader.want<long>("from");
				byte mtype = reader.want<byte>("mtype");
				byte ftype = reader.want<byte>("ftype");
				long mid = reader.want<long>("mid");
				string msg = reader.want<string>("msg");
				string attrs = reader.want<string>("attrs");

				if (!this.midCache.containsKey(mid, from))
					this.procMethod("pushRoomMessage", new object[] { rid, from, mtype, ftype, mid, msg, attrs });
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void pushbroadcastmsg(FPQReader reader)
		{
			try
			{
				long from = reader.want<long>("from");
				byte mtype = reader.want<byte>("mtype");
				byte ftype = reader.want<byte>("ftype");
				long mid = reader.want<long>("mid");
				string msg = reader.want<string>("msg");
				string attrs = reader.want<string>("attrs");

				if (!this.midCache.containsKey(mid, from))
					this.procMethod("pushBroadcastMessage", new object[] { from, mtype, ftype, mid, msg, attrs });
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void transmsg(FPQReader reader)
		{
			try
			{
				long from = reader.want<long>("from");
				long mid = reader.want<long>("mid");
				long omid = reader.want<long>("omid");
				string msg = reader.want<string>("msg");

				if (!this.midCache.containsKey(mid, from))
					this.procMethod("transMessage", new object[] { from, mid, omid, msg });
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void transgroupmsg(FPQReader reader)
		{
			try
			{
				long gid = reader.want<long>("gid");
				long from = reader.want<long>("from");
				long mid = reader.want<long>("mid");
				long omid = reader.want<long>("omid");
				string msg = reader.want<string>("msg");

				if (!this.midCache.containsKey(mid, from))
					this.procMethod("transGroupMessage", new object[] { gid, from, mid, omid, msg });
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void transroommsg(FPQReader reader)
		{
			try
			{
				long rid = reader.want<long>("rid");
				long from = reader.want<long>("from");
				long mid = reader.want<long>("mid");
				long omid = reader.want<long>("omid");
				string msg = reader.want<string>("msg");

				if (!this.midCache.containsKey(mid, from))
					this.procMethod("transRoomMessage", new object[] { rid, from, mid, omid, msg });
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void transbroadcastmsg(FPQReader reader)
		{
			try
			{
				long from = reader.want<long>("from");
				long mid = reader.want<long>("mid");
				long omid = reader.want<long>("omid");
				string msg = reader.want<string>("msg");

				if (!this.midCache.containsKey(mid, from))
					this.procMethod("transBroadcastMessage", new object[] { from, mid, omid, msg });
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		public void pushunread(FPQReader reader)
		{
			try
			{
				List<long> p2p = reader.want<List<long>>("p2p");
				List<long> group = reader.want<List<long>>("group");
				bool bc = reader.want<bool>("bc");
				this.procMethod("pushUnread", new object[] { p2p, group, bc });
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

	}
}

