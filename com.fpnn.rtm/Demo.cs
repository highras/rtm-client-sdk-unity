using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace com.fpnn.rtm
{
	public class Demo
	{
		public class MyEventHandler : RTMEventHandler
		{
			public override void bye()
			{
				Console.WriteLine("bye");
			}

			public override void kickout()
			{
				Console.WriteLine("kickout");
			}

			public override void pushMessage(long from, byte mtype, byte ftype, long mid, string msg, string attrs) 
			{
				Console.WriteLine("pushMsg from: " + from + " msg: " + msg);
			}

			public override void pushGroupMessage(long gid, long from, byte mtype, byte ftype, long mid, string msg, string attrs) { 
				Console.WriteLine("pushGroupMsg from: " + from + " msg: " + msg);
			}

			public override void pushRoomMessage(long rid, long from, byte mtype, byte ftype, long mid, string msg, string attrs) { 
				Console.WriteLine("pushRoomMsg from: " + from + " msg: " + msg);
			}

			public override void pushBroadcastMessage(long from, byte mtype, byte ftype, long mid, string msg, string attrs) {
				Console.WriteLine("pushBroadcastMsg from: " + from + " msg: " + msg);
			}

			public override void transMessage(long from, long mid, long omid, string msg) { 
				Console.WriteLine("transMsg from: " + from + " msg: " + msg);
			}

			public override void transGroupMessage(long gid, long from, long mid, long omid, string msg) { 
				Console.WriteLine("transGroupMsg from: " + from + " msg: " + msg);
			}

			public override void transRoomMessage(long rid, long from, long mid, long omid, string msg) { 
				Console.WriteLine("transRoomMsg from: " + from + " msg: " + msg);
			}

			public override void transBroadcastMessage(long from, long mid, long omid, string msg) { 
				Console.WriteLine("transBroadcastMsg from: " + from + " msg: " + msg);
			}

			public override void pushUnread(List<long> p2p, List<long> group, bool haveBroadcast) {
				Console.WriteLine("pushUnread");
			}

		}

		public class MyQuestCallback : RTMQuestCallback
		{
			RTMClient client;

			public MyQuestCallback(RTMClient client)
			{
				this.client = client;
			}

			public override void authCallback(bool success, bool isReconnect) {
				
				Console.WriteLine("auth: " + success + " reconnect: " + isReconnect);

				if (success)
				{
					client.sendMessage(2, 51, "test msg", "test attrs", this);
				}
			}
			public override void authException(RTMException e) { 
				Console.WriteLine("auth error: " + e.str());
			}

			public override void sendMessageCallback()
			{
				Console.WriteLine("send msg ok");
			}
			public override void sendMessageException(RTMException e)
			{
				Console.WriteLine("send msg error: " + e.str());
			}
		}

		public class MyClosedCallback : RTMClosedCallback
		{
			public override void RTMClosed(bool causedByError)
			{
				Console.WriteLine("close: " + causedByError);
			}
		}

		public static void Main(string[] args)
		{
			RTMClient client = new RTMClient("117.50.4.158:13325", "", true);
			
			client.setEventHandler(new MyEventHandler());
			client.setClosedCallback(new MyClosedCallback());

			int pid = 1000008;
			long uid = 1;
			string token = "E9DB6F6F8075B9591B1F015C5D902583";

			client.auth(pid, uid, token, true, new MyQuestCallback(client));

			//client.setAuthCallback(new MyQuestCallback(client));
			//client.auth(pid, uid, token, true);

			while (true)
				System.Threading.Thread.Sleep(1000);
		}
	}
}

