using System;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Fpnn.Connection;
using Fpnn.Protocol;

namespace com.fpnn.rtm
{
	public class RTMClient
	{
		public const string SDK_VERSION = "csharp-sdk 1.0";
		private delegate void ReadAnswerCallback(FPAReader reader);

		private bool autoReconnect = true;
		private TCPClient dispatch = null;
		private TCPClient gate = null;
		private string cluster;
		private int timeout = 0;
		private object midLock = new Object();
		private uint midSeq = 0;
		private System.Timers.Timer pingTimer = null;
		private int idleTimeoutSecond = 15;
		public long lastRecvPingTime = 0;

		private string publicKeyData = null;
		private int pid = -1;
		private long uid;
		private string token = null;
		private bool pushUnread = false;
		private RTMEventHandler processor = null;
		private RTMClosedCallback closedCallback = null;
		private RTMQuestCallback authCb = null;

		private int sendFileClientNumMaxLimit = 3;
		private object sendFileLock = new object();
		private int sendFileClientNum = 0;

		private object connectLock = new object(); 

		public RTMClient(string dispatchEndpoint, string cluster, bool autoReconnect = true)
		{
			this.dispatch = new TCPClient(dispatchEndpoint);
			this.cluster = cluster;
			this.autoReconnect = autoReconnect;
		}

		public RTMClient(string dispatchHost, int dispatchPort, string cluster, bool autoReconnect = true)
		{
			this.dispatch = new TCPClient(dispatchHost, dispatchPort);
			this.cluster = cluster;
			this.autoReconnect = autoReconnect;
		}

		~RTMClient()
		{
			this.stop();
		}

		public int questTimeout()
		{
			return this.timeout;
		}

		public void setQuestTimeout(int questTimeout)
		{
			this.timeout = questTimeout;
		}

		public int idleTimeout()
		{
			return idleTimeoutSecond;
		}

		public void setidleTimeout(int seconds)
		{
			if (seconds <= 0)
				throw new Exception("ping interval must be greater than 0");
			
			this.idleTimeoutSecond = seconds;
		}

		public void setClosedCallback(RTMClosedCallback cb)
		{
			this.closedCallback = cb;
		}

		public void enableEncryptor(string publicKeyData)
		{
			this.publicKeyData = publicKeyData;
		}

		public void setEventHandler(RTMEventHandler processor)
		{
			this.processor = processor;
		}

		private void startPing()
		{
			if (this.pingTimer == null)
			{
				this.pingTimer = new System.Timers.Timer();
				this.pingTimer.Elapsed += new System.Timers.ElapsedEventHandler(checkPing);
				this.pingTimer.Interval = this.idleTimeoutSecond * 1000 / 2;
				this.pingTimer.Start();
				this.pingTimer.Enabled = true;
			}
		}

		private void checkPing(object source, ElapsedEventArgs e)
		{
			long maxLiveInterval = this.idleTimeoutSecond * 1000;
			if (PackCommon.getMilliTimestamp() - this.lastRecvPingTime > maxLiveInterval)
			{
				this.gate.close();

				if (this.autoReconnect)
				{
					try
					{
						this.gate.reconnect();
						this.sendAuth(true);
					}
					catch (Exception)
					{
						if (this.closedCallback != null)
							this.closedCallback.RTMClosed(true);
					}
				}
				else
				{
					if (this.closedCallback != null)
						this.closedCallback.RTMClosed(true);
				}
			}
		}

		private long genMid()
		{
			lock (this.midLock)
			{
				TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
				return (Convert.ToInt64(ts.TotalSeconds) << 32) + (this.midSeq++ & 0xffffff);
			}
		}

		public void close()
		{
			this.stop();
		}

		private void stop()
		{
			if (this.gate != null)
				this.gate.close();

			if (this.pingTimer != null)
				this.pingTimer.Stop();
		}

		private string getGateEndpoint()
		{
			if (this.publicKeyData != null)
				this.dispatch.enableEncryptor(this.publicKeyData);
			
			this.dispatch.connect();

			FPQWriter qw = new FPQWriter(2, "which");
			if (this.cluster == "")
				qw.param("what", "rtmGated");
			else
				qw.param("what", "rtmGated@" + this.cluster);
			qw.param("addrType", "ipv4");

			FPAReader reader = this.dispatch.sendQuestSync(qw.take());

			if (reader.isError())
				throw new RTMException(reader.errorCode(), reader.errorException(), reader.errorRaiser());
			else 
			{
				try
				{
					string endpoint = reader.want<string>("endpoint");

					if (endpoint == "")
						throw new RTMException("get rtmGated endpoint error");
					
					return endpoint;
				}
				catch (Exception e)
				{
					throw new RTMException("get rtmGated endpoint error", e);
				}
			}
		}

		private void connectToGate(string endpoint)
		{
			if (this.gate != null)
				this.gate.close();
			
			this.gate = new TCPClient(endpoint, false);
			this.gate.setQuestTimeout(this.timeout);

			if (this.publicKeyData != null)
				this.gate.enableEncryptor(this.publicKeyData);

			this.gate.setConnectionConnectedCallback(delegate () {
				
			});

			this.gate.setConnectionWillCloseCallback(delegate (bool causedByError)
			{
				if (causedByError)
				{
					if (this.autoReconnect)
					{
						try
						{
							this.gate.reconnect();
							this.sendAuth(true);
						}
						catch (Exception)
						{
							if (this.closedCallback != null)
								this.closedCallback.RTMClosed(causedByError);
						}
					}
					else
					{
						if (this.closedCallback != null)
							this.closedCallback.RTMClosed(causedByError);
					}
				}
			});

			if (this.processor != null)
				this.gate.setProcessor(new RTMPushDispatcher(this.processor, this));

			this.gate.connect();
		}

		private void connect()
		{
			lock (connectLock)
			{
				string endpoint = this.getGateEndpoint();
				this.connectToGate(endpoint);
			}
		}

		public void procException(RTMQuestCallback cb, string method, FPAReader reader)
		{
			if (cb == null)
				return;
			
			MethodInfo mi = cb.GetType().GetMethod(method + "Exception");
			if (mi != null)
			{
				mi.Invoke(cb, new object[] { new RTMException(reader.errorCode(), reader.errorException(), reader.errorRaiser()) });
			}
		}

		public void procException(RTMQuestCallback cb, string method, string msg)
		{
			if (cb == null)
				return;
			
			MethodInfo mi = cb.GetType().GetMethod(method + "Exception");
			if (mi != null)
			{
				mi.Invoke(cb, new object[] { new RTMException(msg) });
			}
		}

		public void procCallback(RTMQuestCallback cb, string method, object[] args)
		{
			if (cb == null)
				return;
			
			MethodInfo mi = cb.GetType().GetMethod(method + "Callback");
			if (mi != null)
			{
				mi.Invoke(cb, args);
			}
		}

		private void sendQuest(FPQuest quest, string method, RTMQuestCallback cb)
		{
			if (this.gate == null)
			{
				this.procException(cb, method, "can not send quest before auth");
			}
			else
			{
				this.gate.sendQuest(quest, delegate (FPAReader reader)
				{
					if (reader.isError())
					{
						this.procException(cb, method, reader);
					}
					else
					{
						this.procCallback(cb, method, new object[] { });
					}
				});
			}
		}

		private void sendQuest(FPQuest quest, string method, RTMQuestCallback cb, ReadAnswerCallback readCb)
		{
			if (this.gate == null)
			{
				this.procException(cb, method, "can not send quest before auth");
			}
			else
			{
				this.gate.sendQuest(quest, delegate (FPAReader reader)
				{
					if (reader.isError())
					{
						this.procException(cb, method, reader);
					}
					else
					{
						readCb(reader);
					}
				});
			}
		}

		private FPAReader sendQuest(FPQuest quest)
		{
			if(this.gate == null)
			{
				throw new RTMException("can not send quest before auth");
			}
			FPAReader reader = this.gate.sendQuestSync(quest);
			if (reader.isError())
			{
				throw new RTMException(reader.errorCode(), reader.errorException(), reader.errorRaiser());
			}
			return reader;
		}

		private void sendAuth(bool isReconnect)
		{
			FPQWriter qw = new FPQWriter(5, "auth");
			qw.param("pid", this.pid);
			qw.param("uid", this.uid);
			qw.param("token", this.token);
			qw.param("version", SDK_VERSION);
			qw.param("unread", this.pushUnread);

			this.gate.sendQuest(qw.take(), delegate (FPAReader reader)
			{
				if (reader.isError())
				{
					if (this.authCb != null)
						this.procException(this.authCb, "auth", reader);
				}
				else
				{
					bool ok = reader.want<bool>("ok");

					if (ok)
					{
						this.lastRecvPingTime = PackCommon.getMilliTimestamp();
						this.startPing();
					}
					else
					{
						string endpoint = reader.get<string>("gate", "");
						if (endpoint != "")
						{
							lock (connectLock)
							{
								this.connectToGate(endpoint);
							}
							this.sendAuth(isReconnect);
							return;
						}
					}

					if (this.authCb != null)
						this.procCallback(this.authCb, "auth", new object[] { ok, isReconnect });
				}
			});
		}

		private void sendAuthSync(bool isReconnect)
		{
			FPQWriter qw = new FPQWriter(5, "auth");
			qw.param("pid", this.pid);
			qw.param("uid", this.uid);
			qw.param("token", this.token);
			qw.param("version", SDK_VERSION);
			qw.param("unread", this.pushUnread);

			FPAReader reader = this.sendQuest(qw.take());

			if (reader.isError())
			{
				if (this.authCb != null)
					this.procException(this.authCb, "auth", reader);
			}
			else
			{
				bool ok = reader.want<bool>("ok");

				if (ok)
				{
					this.lastRecvPingTime = PackCommon.getMilliTimestamp();
					this.startPing();
				}
				else
				{
					string endpoint = reader.get<string>("gate", "");
					if (endpoint != "")
					{
						lock (connectLock)
						{
							this.connectToGate(endpoint);
						}
						this.sendAuthSync(isReconnect);
						return;
					}
				}

				if (this.authCb != null)
					this.procCallback(this.authCb, "auth", new object[] { ok, isReconnect });
			}
		}

		public void setAuthCallback(RTMQuestCallback cb)
		{
			this.authCb = cb;
		}

		public void auth(int pid, long uid, string token, bool pushUnread, RTMQuestCallback cb)
		{
			this.pid = pid;
			this.uid = uid;
			this.token = token;
			this.pushUnread = pushUnread;
			this.authCb = cb;

			this.connect();

			this.sendAuth(false);
		}

		public void auth(int pid, long uid, string token, bool pushUnread)
		{
			this.pid = pid;
			this.uid = uid;
			this.token = token;
			this.pushUnread = pushUnread;

			this.connect();

			this.sendAuthSync(false);
		}

		public void sendMessage(long to, byte mtype, string msg, string attrs, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(5, "sendmsg");
			qw.param("to", to);
			qw.param("mid", this.genMid());
			qw.param("mtype", mtype);
			qw.param("msg", msg);
			qw.param("attrs", attrs);
			this.sendQuest(qw.take(), "sendMessage", cb);
		}

		public void sendMessage(long to, byte mtype, string msg, string attrs)
		{
			FPQWriter qw = new FPQWriter(5, "sendmsg");
			qw.param("to", to);
			qw.param("mid", this.genMid());
			qw.param("mtype", mtype);
			qw.param("msg", msg);
			qw.param("attrs", attrs);
			this.sendQuest(qw.take());
		}

		public void sendMessages(List<long> tos, byte mtype, string msg, string attrs, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(5, "sendmsgs");
			qw.param("tos", tos);
			qw.param("mid", this.genMid());
			qw.param("mtype", mtype);
			qw.param("msg", msg);
			qw.param("attrs", attrs);
			this.sendQuest(qw.take(), "sendMessages", cb);
		}

		public void sendMessages(List<long> tos, byte mtype, string msg, string attrs)
		{
			FPQWriter qw = new FPQWriter(5, "sendmsgs");
			qw.param("tos", tos);
			qw.param("mid", this.genMid());
			qw.param("mtype", mtype);
			qw.param("msg", msg);
			qw.param("attrs", attrs);
			this.sendQuest(qw.take());
		}

		public void sendGroupMessage(long gid, byte mtype, string msg, string attrs, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(5, "sendgroupmsg");
			qw.param("gid", gid);
			qw.param("mid", this.genMid());
			qw.param("mtype", mtype);
			qw.param("msg", msg);
			qw.param("attrs", attrs);
			this.sendQuest(qw.take(), "sendGroupMessage", cb);
		}

		public void sendGroupMessage(long gid, byte mtype, string msg, string attrs)
		{
			FPQWriter qw = new FPQWriter(5, "sendgroupmsg");
			qw.param("gid", gid);
			qw.param("mid", this.genMid());
			qw.param("mtype", mtype);
			qw.param("msg", msg);
			qw.param("attrs", attrs);
			this.sendQuest(qw.take());
		}

		public void sendRoomMessage(long rid, byte mtype, string msg, string attrs, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(5, "sendroommsg");
			qw.param("rid", rid);
			qw.param("mid", this.genMid());
			qw.param("mtype", mtype);
			qw.param("msg", msg);
			qw.param("attrs", attrs);
			this.sendQuest(qw.take(), "sendRoomMessage", cb);
		}

		public void sendRoomMessage(long rid, byte mtype, string msg, string attrs)
		{
			FPQWriter qw = new FPQWriter(5, "sendroommsg");
			qw.param("rid", rid);
			qw.param("mid", this.genMid());
			qw.param("mtype", mtype);
			qw.param("msg", msg);
			qw.param("attrs", attrs);
			this.sendQuest(qw.take());
		}

		public void addVariables(Dictionary<string, string> var, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "addvariables");
			qw.param("var", var);
			this.sendQuest(qw.take(), "addVariables", cb);
		}

		public void addVariables(Dictionary<string, string> var)
		{
			FPQWriter qw = new FPQWriter(1, "addvariables");
			qw.param("var", var);
			this.sendQuest(qw.take());
		}

		public void setPushName(string pushname, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "setpushname");
			qw.param("pushname", pushname);
			this.sendQuest(qw.take(), "setPushName", cb);
		}

		public void setPushName(string pushname)
		{
			FPQWriter qw = new FPQWriter(1, "setpushname");
			qw.param("pushname", pushname);
			this.sendQuest(qw.take());
		}

		public void getPushName(RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(0, "getpushname");
			this.sendQuest(qw.take(), "getPushName", cb, delegate (FPAReader reader)
			{
				try
				{
					string pushname = reader.want<string>("pushname");
					this.procCallback(cb, "getPushName", new object[] { pushname });
				}
				catch (Exception)
				{
					this.procException(cb, "getPushName", "read answer error");
				}

			});
		}

		public string getPushName()
		{
			FPQWriter qw = new FPQWriter(0, "getpushname");
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				string pushname = reader.want<string>("pushname");
				return pushname;
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void setGeo(double lat, double lng, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(2, "setgeo");
			qw.param("lat", lat);
			qw.param("lng", lng);
			this.sendQuest(qw.take(), "setGeo", cb);
		}

		public void setGeo(double lat, double lng)
		{
			FPQWriter qw = new FPQWriter(2, "setgeo");
			qw.param("lat", lat);
			qw.param("lng", lng);
			this.sendQuest(qw.take());
		}

		public void getGeo(RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(0, "getgeo");
			this.sendQuest(qw.take(), "getGeo", cb, delegate (FPAReader reader)
			{
				try
				{
					double latRead = reader.want<double>("lat");
					double lngRead = reader.want<double>("lng");
					this.procCallback(cb, "getGeo", new object[] { latRead, lngRead });
				}
				catch (Exception)
				{
					this.procException(cb, "getGeo", "read answer error");
				}

			});
		}

		public void getGeo(out double lat, out double lng)
		{
			FPQWriter qw = new FPQWriter(0, "getgeo");
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				lat = reader.want<double>("lat");
				lng = reader.want<double>("lng");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void getGeos(List<long> uids, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "getgeos");
			qw.param("uids", uids);
			this.sendQuest(qw.take(), "getGeos", cb, delegate (FPAReader reader)
			{
				try
				{
					List<List<string>> geos = reader.want<List<List<string>>>("geos");
					this.procCallback(cb, "getGeos", new object[] { geos });
				}
				catch (Exception)
				{
					this.procException(cb, "getGeos", "read answer error");
				}

			});
		}

		public List<List<string>> getGeos(List<long> uids)
		{
			FPQWriter qw = new FPQWriter(1, "getgeos");
			qw.param("uids", uids);
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				return reader.want<List<List<string>>>("geos");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void addFriends(List<long> friends, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "addfriends");
			qw.param("friends", friends);
			this.sendQuest(qw.take(), "addFriends", cb);
		}

		public void addFriends(List<long> friends)
		{
			FPQWriter qw = new FPQWriter(1, "addfriends");
			qw.param("friends", friends);
			this.sendQuest(qw.take());
		}

		public void deleteFriends(List<long> friends, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "delfriends");
			qw.param("friends", friends);
			this.sendQuest(qw.take(), "deleteFriends", cb);
		}

		public void deleteFriends(List<long> friends)
		{
			FPQWriter qw = new FPQWriter(1, "delfriends");
			qw.param("friends", friends);
			this.sendQuest(qw.take());
		}

		public void getFriends(RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(0, "getfriends");
			this.sendQuest(qw.take(), "getFriends", cb, delegate (FPAReader reader)
			{
				try
				{
					List<long> uids = reader.want<List<long>>("uids");
					this.procCallback(cb, "getFriends", new object[] { uids });
				}
				catch (Exception)
				{
					this.procException(cb, "getFriends", "read answer error");
				}
			});
		}

		public List<long> getFriends()
		{
			FPQWriter qw = new FPQWriter(0, "getfriends");
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				return reader.want<List<long>>("uids");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void addGroupMembers(long gid, List<long> uids, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(2, "addgroupmembers");
			qw.param("gid", gid);
			qw.param("uids", uids);
			this.sendQuest(qw.take(), "addGroupMembers", cb);
		}

		public void addGroupMembers(long gid, List<long> uids)
		{
			FPQWriter qw = new FPQWriter(2, "addgroupmembers");
			qw.param("gid", gid);
			qw.param("uids", uids);
			this.sendQuest(qw.take());
		}

		public void deleteGroupMembers(long gid, List<long> uids, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(2, "delgroupmembers");
			qw.param("gid", gid);
			qw.param("uids", uids);
			this.sendQuest(qw.take(), "deleteGroupMembers", cb);
		}

		public void deleteGroupMembers(long gid, List<long> uids)
		{
			FPQWriter qw = new FPQWriter(2, "delgroupmembers");
			qw.param("gid", gid);
			qw.param("uids", uids);
			this.sendQuest(qw.take());
		}

		public void getGroupMembers(long gid, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "getgroupmembers");
			qw.param("gid", gid);
			this.sendQuest(qw.take(), "getGroupMembers", cb, delegate (FPAReader reader)
			{
				try
				{
					List<long> uids = reader.want<List<long>>("uids");
					this.procCallback(cb, "getGroupMembers", new object[] { uids });
				}
				catch (Exception)
				{
					this.procException(cb, "getGroupMembers", "read answer error");
				}
			});
		}

		public List<long> getGroupMembers(long gid)
		{
			FPQWriter qw = new FPQWriter(1, "getgroupmembers");
			qw.param("gid", gid);
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				return reader.want<List<long>>("uids");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void getUserGroups(RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(0, "getusergroups");
			this.sendQuest(qw.take(), "getUserGroups", cb, delegate (FPAReader reader)
			{
				try
				{
					List<long> gids = reader.want<List<long>>("gids");
					this.procCallback(cb, "getUserGroups", new object[] { gids });
				}
				catch (Exception)
				{
					this.procException(cb, "getUserGroups", "read answer error");
				}
			});
		}

		public List<long> getUserGroups()
		{
			FPQWriter qw = new FPQWriter(0, "getusergroups");
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				return reader.want<List<long>>("gids");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void enterRoom(long rid, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "enterroom");
			qw.param("rid", rid);
			this.sendQuest(qw.take(), "enterRoom", cb);
		}

		public void enterRoom(long rid)
		{
			FPQWriter qw = new FPQWriter(1, "enterroom");
			qw.param("rid", rid);
			this.sendQuest(qw.take());
		}

		public void leaveRoom(long rid, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "leaveroom");
			qw.param("rid", rid);
			this.sendQuest(qw.take(), "leaveRoom", cb);
		}

		public void leaveRoom(long rid)
		{
			FPQWriter qw = new FPQWriter(1, "leaveroom");
			qw.param("rid", rid);
			this.sendQuest(qw.take());
		}

		public void getUserRooms(RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(0, "getuserrooms");
			this.sendQuest(qw.take(), "getuserrooms", cb, delegate (FPAReader reader)
			{
				try
				{
					List<long> rooms = reader.want<List<long>>("rooms");
					this.procCallback(cb, "getUserRooms", new object[] { rooms });
				}
				catch (Exception)
				{
					this.procException(cb, "getUserRooms", "read answer error");
				}
			});
		}

		public List<long> getUserRooms()
		{
			FPQWriter qw = new FPQWriter(0, "getuserrooms");
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				return reader.want<List<long>>("rooms");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void getOnlineUsers(List<long> uids, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "getonlineusers");
			qw.param("uids", uids);
			this.sendQuest(qw.take(), "getOnlineUsers", cb, delegate (FPAReader reader)
			{
				try
				{
					List<long> uidsRead = reader.want<List<long>>("uids");
					this.procCallback(cb, "getOnlineUsers", new object[] { uidsRead });
				}
				catch (Exception)
				{
					this.procException(cb, "getOnlineUsers", "read answer error");
				}
			});
		}

		public List<long> getOnlineUsers(List<long> uids)
		{
			FPQWriter qw = new FPQWriter(1, "getonlineusers");
			qw.param("uids", uids);
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				return reader.want<List<long>>("uids");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void getGroupMessage(long gid, short num, bool desc, ushort page, long localmid, long localid, List<byte> mtypes, RTMQuestCallback cb)
		{
			if (mtypes == null)
				mtypes = new List<byte>();
			
			FPQWriter qw = new FPQWriter(7, "getgroupmsg");
			qw.param("gid", gid);
			qw.param("num", num);
			qw.param("desc", desc);
			qw.param("page", page);
			qw.param("localmid", localmid);
			qw.param("localid", localid);
			qw.param("mtypes", mtypes);
			this.sendQuest(qw.take(), "getGroupMessage", cb, delegate (FPAReader reader)
			{
				try
				{
					short numRead = reader.want<short>("num");
					long maxid = reader.want<long>("maxid");
					List<GroupMsg> msgs = reader.want<List<GroupMsg>>("msgs");
					this.procCallback(cb, "getGroupMessage", new object[] { numRead, maxid, msgs });
				}
				catch (Exception)
				{
					this.procException(cb, "getGroupMessage", "read answer error");
				}
			});
		}

		public void getGroupMessage(long gid, short num, bool desc, ushort page, long localmid, long localid, List<byte> mtypes, out short numRead, out long maxid, out List<GroupMsg> msgs)
		{
			if (mtypes == null)
				mtypes = new List<byte>();

			FPQWriter qw = new FPQWriter(7, "getgroupmsg");
			qw.param("gid", gid);
			qw.param("num", num);
			qw.param("desc", desc);
			qw.param("page", page);
			qw.param("localmid", localmid);
			qw.param("localid", localid);
			qw.param("mtypes", mtypes);
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				numRead = reader.want<short>("num");
				maxid = reader.want<long>("maxid");
				msgs = reader.want<List<GroupMsg>>("msgs");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void getRoomMessage(long rid, short num, bool desc, ushort page, long localmid, long localid, List<byte> mtypes, RTMQuestCallback cb)
		{
			if (mtypes == null)
				mtypes = new List<byte>();
			
			FPQWriter qw = new FPQWriter(7, "getroommsg");
			qw.param("rid", rid);
			qw.param("num", num);
			qw.param("desc", desc);
			qw.param("page", page);
			qw.param("localmid", localmid);
			qw.param("localid", localid);
			qw.param("mtypes", mtypes);
			this.sendQuest(qw.take(), "getRoomMessage", cb, delegate (FPAReader reader)
			{
				try
				{
					short numRead = reader.want<short>("num");
					long maxid = reader.want<long>("maxid");
					List<RoomMsg> msgs = reader.want<List<RoomMsg>>("msgs");
					this.procCallback(cb, "getRoomMessage", new object[] { numRead, maxid, msgs });
				}
				catch (Exception)
				{
					this.procException(cb, "getRoomMessage", "read answer error");
				}
			});
		}

		public void getRoomMessage(long rid, short num, bool desc, ushort page, long localmid, long localid, List<byte> mtypes, out short numRead, out long maxid, out List<RoomMsg> msgs)
		{
			if (mtypes == null)
				mtypes = new List<byte>();

			FPQWriter qw = new FPQWriter(7, "getroommsg");
			qw.param("rid", rid);
			qw.param("num", num);
			qw.param("desc", desc);
			qw.param("page", page);
			qw.param("localmid", localmid);
			qw.param("localid", localid);
			qw.param("mtypes", mtypes);
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				numRead = reader.want<short>("num");
				maxid = reader.want<long>("maxid");
				msgs = reader.want<List<RoomMsg>>("msgs");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void getBroadcastMessage(short num, bool desc, ushort page, long localmid, long localid, List<byte> mtypes, RTMQuestCallback cb)
		{
			if (mtypes == null)
				mtypes = new List<byte>();
			
			FPQWriter qw = new FPQWriter(6, "getbroadcastmsg");
			qw.param("num", num);
			qw.param("desc", desc);
			qw.param("page", page);
			qw.param("localmid", localmid);
			qw.param("localid", localid);
			qw.param("mtypes", mtypes);
			this.sendQuest(qw.take(), "getBroadcastMessage", cb, delegate (FPAReader reader)
			{
				try
				{
					short numRead = reader.want<short>("num");
					long maxid = reader.want<long>("maxid");
					List<BroadcastMsg> msgs = reader.want<List<BroadcastMsg>>("msgs");
					this.procCallback(cb, "getBroadcastMessage", new object[] { numRead, maxid, msgs });
				}
				catch (Exception)
				{
					this.procException(cb, "getBroadcastMessage", "read answer error");
				}
			});
		}

		public void getBroadcastMessage(short num, bool desc, ushort page, long localmid, long localid, List<byte> mtypes, out short numRead, out long maxid, out List<BroadcastMsg> msgs)
		{
			if (mtypes == null)
				mtypes = new List<byte>();

			FPQWriter qw = new FPQWriter(6, "getbroadcastmsg");
			qw.param("num", num);
			qw.param("desc", desc);
			qw.param("page", page);
			qw.param("localmid", localmid);
			qw.param("localid", localid);
			qw.param("mtypes", mtypes);
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				numRead = reader.want<short>("num");
				maxid = reader.want<long>("maxid");
				msgs = reader.want<List<BroadcastMsg>>("msgs");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void getP2PMessage(long from, short num, byte direction, bool desc, ushort page, long localmid, long localid, List<byte> mtypes, RTMQuestCallback cb)
		{
			if (mtypes == null)
				mtypes = new List<byte>();
			
			FPQWriter qw = new FPQWriter(8, "getp2pmsg");
			qw.param("from", from);
			qw.param("num", num);
			qw.param("direction", direction);
			qw.param("desc", desc);
			qw.param("page", page);
			qw.param("localmid", localmid);
			qw.param("localid", localid);
			qw.param("mtypes", mtypes);
			this.sendQuest(qw.take(), "getP2PMessage", cb, delegate (FPAReader reader)
			{
				try
				{
					short numRead = reader.want<short>("num");
					long maxid = reader.want<long>("maxid");
					List<P2PMsg> msgs = reader.want<List<P2PMsg>>("msgs");
					this.procCallback(cb, "getP2PMessage", new object[] { numRead, maxid, msgs });
				}
				catch (Exception)
				{
					this.procException(cb, "getP2PMessage", "read answer error");
				}
			});
		}

		public void getP2PMessage(long from, short num, byte direction, bool desc, ushort page, long localmid, long localid, List<byte> mtypes, out short numRead, out long maxid, out List<P2PMsg> msgs)
		{
			if (mtypes == null)
				mtypes = new List<byte>();

			FPQWriter qw = new FPQWriter(8, "getp2pmsg");
			qw.param("from", from);
			qw.param("num", num);
			qw.param("direction", direction);
			qw.param("desc", desc);
			qw.param("page", page);
			qw.param("localmid", localmid);
			qw.param("localid", localid);
			qw.param("mtypes", mtypes);
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				numRead = reader.want<short>("num");
				maxid = reader.want<long>("maxid");
				msgs = reader.want<List<P2PMsg>>("msgs");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		public void addDevice(string ptype, string dtype, string token, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(3, "adddevice");
			qw.param("ptype", ptype);
			qw.param("dtype", dtype);
			qw.param("token", token);
			this.sendQuest(qw.take(), "addDevice", cb);
		}

		public void addDevice(string ptype, string dtype, string token)
		{
			FPQWriter qw = new FPQWriter(3, "adddevice");
			qw.param("ptype", ptype);
			qw.param("dtype", dtype);
			qw.param("token", token);
			this.sendQuest(qw.take());
		}

		public void setLang(string lang, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(1, "setlang");
			qw.param("lang", lang);
			this.sendQuest(qw.take(), "setLang", cb);
		}

		public void setLang(string lang)
		{
			FPQWriter qw = new FPQWriter(1, "setlang");
			qw.param("lang", lang);
			this.sendQuest(qw.take());
		}

		public void translate(string text, string src, string dst, RTMQuestCallback cb)
		{
			FPQWriter qw = new FPQWriter(3, "translate");
			qw.param("text", text);
			qw.param("src", src);
			qw.param("dst", dst);
			this.sendQuest(qw.take(), "translate", cb, delegate (FPAReader reader)
			{
				try
				{
					string stext = reader.want<string>("stext");
					string srcRead = reader.want<string>("src");
					string dtext = reader.want<string>("dtext");
					string dstRead = reader.want<string>("dst");
					this.procCallback(cb, "translate", new object[] { stext, srcRead, dtext, dstRead });
				}
				catch (Exception)
				{
					this.procException(cb, "translate", "read answer error");
				}
			});
		}

		public void translate(string text, string src, string dst, out string stext, out string srcRead, out string dtext, out string dstRead)
		{
			FPQWriter qw = new FPQWriter(3, "translate");
			qw.param("text", text);
			qw.param("src", src);
			qw.param("dst", dst);
			FPAReader reader = this.sendQuest(qw.take());
			try
			{
				stext = reader.want<string>("stext");
				srcRead = reader.want<string>("src");
				dtext = reader.want<string>("dtext");
				dstRead = reader.want<string>("dst");
			}
			catch (Exception)
			{
				throw new RTMException("read answer error");
			}
		}

		private void sendFileQuest(string endpoint, FPQuest quest, string method, RTMQuestCallback cb)
		{
			TCPClient client = new TCPClient(endpoint);
			client.connect();

			client.sendQuest(quest, delegate (FPAReader reader)
			{
				if (reader.isError())
				{
					this.procException(cb, method, reader);

					lock (this.sendFileLock)
					{
						this.sendFileClientNum--;
					}

					client.close();
				}
				else
				{
					this.procCallback(cb, method, new object[] { });

					lock (this.sendFileLock)
					{
						this.sendFileClientNum--;
					}

					client.close();
				}
			});
		}

		private void sendFileQuest(string endpoint, FPQuest quest)
		{
			TCPClient client = new TCPClient(endpoint);
			client.connect();

			FPAReader reader = client.sendQuestSync(quest);

			if (reader.isError())
			{
				lock (this.sendFileLock)
				{
					this.sendFileClientNum--;
				}

				client.close();

				throw new RTMException(reader.errorCode(), reader.errorException(), reader.errorRaiser());
			}
			else
			{
				lock (this.sendFileLock)
				{
					this.sendFileClientNum--;
				}

				client.close();
			}
		}

		private string toHex(byte[] bytes)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);

			for (int i = 0; i < bytes.Length; i++)
				result.Append(bytes[i].ToString("x2"));

			return result.ToString();
		}

		private string buildAttrs(string token, byte[] content, string name, string ext)
		{
			MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] md5Binary = md5.ComputeHash(content);
			string md5Hex = toHex(md5Binary) + ":" + token;

			md5 = System.Security.Cryptography.MD5.Create();
			md5Binary = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(md5Hex));
			md5Hex = toHex(md5Binary);

			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			sb.Append("\"sign\":\"").Append(md5Hex).Append("\"");

			if (ext != null && ext.Length > 0)
				sb.Append(", \"ext\":\"").Append(ext).Append("\"");

			if (name != null && name.Length > 0)
				sb.Append(", \"filename\":\"").Append(name).Append("\"");

			sb.Append("}");

			return sb.ToString();
		}

		public void sendFile(byte mtype, long to, byte[] fileContent, string fileName, string fileExtension, RTMQuestCallback cb)
		{
			if (this.gate == null)
			{
				this.procException(cb, "sendFile", "can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					this.procException(cb, "sendFile", "you can only send " + this.sendFileClientNumMaxLimit + " files one time");
					return;
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(2, "filetoken");
			qw.param("cmd", "sendfile");
			qw.param("to", to);

			this.gate.sendQuest(qw.take(), delegate (FPAReader reader)
			{
				if (reader.isError())
				{
					this.procException(cb, "sendFile", reader);
					lock (this.sendFileLock)
					{
						this.sendFileClientNum--;
					}
				}
				else
				{
					string tokenRead = reader.want<string>("token");
					string endpoint = reader.want<string>("endpoint");
					FPQWriter qwSend = new FPQWriter(8, "sendfile");
					qwSend.param("pid", this.pid);
					qwSend.param("token", tokenRead);
					qwSend.param("mtype", mtype);
					qwSend.param("from", this.uid);
					qwSend.param("to", to);
					qwSend.param("mid", this.genMid());
					qwSend.param("file", fileContent);
					qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
					this.sendFileQuest(endpoint, qwSend.take(), "sendFile", cb);
				}
			});
		}

		public void sendFile(byte mtype, long to, byte[] fileContent, string fileName, string fileExtension)
		{
			if (this.gate == null)
			{
				throw new RTMException("can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					throw new RTMException("you can only send " + this.sendFileClientNumMaxLimit + " files one time");
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(2, "filetoken");
			qw.param("cmd", "sendfile");
			qw.param("to", to);

			FPAReader reader = this.gate.sendQuestSync(qw.take());

			if (reader.isError())
			{
				lock (this.sendFileLock)
				{
					this.sendFileClientNum--;
				}
				throw new RTMException(reader.errorCode(), reader.errorException(), reader.errorRaiser());
			}
			else
			{
				string tokenRead = reader.want<string>("token");
				string endpoint = reader.want<string>("endpoint");
				FPQWriter qwSend = new FPQWriter(8, "sendfile");
				qwSend.param("pid", this.pid);
				qwSend.param("token", tokenRead);
				qwSend.param("mtype", mtype);
				qwSend.param("from", this.uid);
				qwSend.param("to", to);
				qwSend.param("mid", this.genMid());
				qwSend.param("file", fileContent);
				qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
				this.sendFileQuest(endpoint, qwSend.take());
			}
		}

		public void sendFiles(byte mtype, List<long> tos, byte[] fileContent, string fileName, string fileExtension, RTMQuestCallback cb)
		{
			if (this.gate == null)
			{
				this.procException(cb, "sendFiles", "can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					this.procException(cb, "sendFiles", "you can only send " + this.sendFileClientNumMaxLimit + " files one time");
					return;
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(2, "filetoken");
			qw.param("cmd", "sendfiles");
			qw.param("tos", tos);

			this.gate.sendQuest(qw.take(), delegate (FPAReader reader)
			{
				if (reader.isError())
				{
					this.procException(cb, "sendFiles", reader);
					lock (this.sendFileLock)
					{
						this.sendFileClientNum--;
					}
				}
				else
				{
					string tokenRead = reader.want<string>("token");
					string endpoint = reader.want<string>("endpoint");
					FPQWriter qwSend = new FPQWriter(8, "sendfiles");
					qwSend.param("pid", this.pid);
					qwSend.param("token", tokenRead);
					qwSend.param("mtype", mtype);
					qwSend.param("from", this.uid);
					qwSend.param("tos", tos);
					qwSend.param("mid", this.genMid());
					qwSend.param("file", fileContent);
					qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
					this.sendFileQuest(endpoint, qw.take(), "sendFiles", cb);
				}
			});
		}

		public void sendFiles(byte mtype, List<long> tos, byte[] fileContent, string fileName, string fileExtension)
		{
			if (this.gate == null)
			{
				throw new RTMException("can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					throw new RTMException("you can only send " + this.sendFileClientNumMaxLimit + " files one time");
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(2, "filetoken");
			qw.param("cmd", "sendfiles");
			qw.param("tos", tos);

			FPAReader reader = this.gate.sendQuestSync(qw.take());

			if (reader.isError())
			{
				lock (this.sendFileLock)
				{
					this.sendFileClientNum--;
				}
				throw new RTMException(reader.errorCode(), reader.errorException(), reader.errorRaiser());
			}
			else
			{
				string tokenRead = reader.want<string>("token");
				string endpoint = reader.want<string>("endpoint");
				FPQWriter qwSend = new FPQWriter(8, "sendfiles");
				qwSend.param("pid", this.pid);
				qwSend.param("token", tokenRead);
				qwSend.param("mtype", mtype);
				qwSend.param("from", this.uid);
				qwSend.param("tos", tos);
				qwSend.param("mid", this.genMid());
				qwSend.param("file", fileContent);
				qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
				this.sendFileQuest(endpoint, qwSend.take());
			}
		}

		public void sendGroupFile(byte mtype, long gid, byte[] fileContent, string fileName, string fileExtension, RTMQuestCallback cb)
		{
			if (this.gate == null)
			{
				this.procException(cb, "sendGroupFile", "can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					this.procException(cb, "sendGroupFile", "you can only send " + this.sendFileClientNumMaxLimit + " files one time");
					return;
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(2, "filetoken");
			qw.param("cmd", "sendgroupfile");
			qw.param("gid", gid);

			this.gate.sendQuest(qw.take(), delegate (FPAReader reader)
			{
				if (reader.isError())
				{
					this.procException(cb, "sendGroupFile", reader);
					lock (this.sendFileLock)
					{
						this.sendFileClientNum--;
					}
				}
				else
				{
					string tokenRead = reader.want<string>("token");
					string endpoint = reader.want<string>("endpoint");
					FPQWriter qwSend = new FPQWriter(8, "sendgroupfile");
					qwSend.param("pid", this.pid);
					qwSend.param("token", tokenRead);
					qwSend.param("mtype", mtype);
					qwSend.param("from", this.uid);
					qwSend.param("gid", gid);
					qwSend.param("mid", this.genMid());
					qwSend.param("file", fileContent);
					qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
					this.sendFileQuest(endpoint, qwSend.take(), "sendGroupFile", cb);
				}
			});
		}

		public void sendGroupFile(byte mtype, long gid, byte[] fileContent, string fileName, string fileExtension)
		{
			if (this.gate == null)
			{
				throw new RTMException("can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					throw new RTMException("you can only send " + this.sendFileClientNumMaxLimit + " files one time");
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(2, "filetoken");
			qw.param("cmd", "sendgroupfile");
			qw.param("gid", gid);

			FPAReader reader = this.gate.sendQuestSync(qw.take());

			if (reader.isError())
			{
				lock (this.sendFileLock)
				{
					this.sendFileClientNum--;
				}
				throw new RTMException(reader.errorCode(), reader.errorException(), reader.errorRaiser());
			}
			else
			{
				string tokenRead = reader.want<string>("token");
				string endpoint = reader.want<string>("endpoint");
				FPQWriter qwSend = new FPQWriter(8, "sendgroupfile");
				qwSend.param("pid", this.pid);
				qwSend.param("token", tokenRead);
				qwSend.param("mtype", mtype);
				qwSend.param("from", this.uid);
				qwSend.param("gid", gid);
				qwSend.param("mid", this.genMid());
				qwSend.param("file", fileContent);
				qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
				this.sendFileQuest(endpoint, qwSend.take());
			}
		}

		public void sendRoomFile(byte mtype, long rid, byte[] fileContent, string fileName, string fileExtension, RTMQuestCallback cb)
		{
			if (this.gate == null)
			{
				this.procException(cb, "sendRoomFile", "can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					this.procException(cb, "sendRoomFile", "you can only send " + this.sendFileClientNumMaxLimit + " files one time");
					return;
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(2, "filetoken");
			qw.param("cmd", "sendroomfile");
			qw.param("rid", rid);

			this.gate.sendQuest(qw.take(), delegate (FPAReader reader)
			{
				if (reader.isError())
				{
					this.procException(cb, "sendRoomFile", reader);
					lock (this.sendFileLock)
					{
						this.sendFileClientNum--;
					}
				}
				else
				{
					string tokenRead = reader.want<string>("token");
					string endpoint = reader.want<string>("endpoint");
					FPQWriter qwSend = new FPQWriter(8, "sendroomfile");
					qwSend.param("pid", this.pid);
					qwSend.param("token", tokenRead);
					qwSend.param("mtype", mtype);
					qwSend.param("from", this.uid);
					qwSend.param("rid", rid);
					qwSend.param("mid", this.genMid());
					qwSend.param("file", fileContent);
					qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
					this.sendFileQuest(endpoint, qwSend.take(), "sendRoomFile", cb);
				}
			});
		}

		public void sendRoomFile(byte mtype, long rid, byte[] fileContent, string fileName, string fileExtension)
		{
			if (this.gate == null)
			{
				throw new RTMException("can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					throw new RTMException("you can only send " + this.sendFileClientNumMaxLimit + " files one time");
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(2, "filetoken");
			qw.param("cmd", "sendroomfile");
			qw.param("rid", rid);

			FPAReader reader = this.gate.sendQuestSync(qw.take());

			if (reader.isError())
			{
				lock (this.sendFileLock)
				{
					this.sendFileClientNum--;
				}
				throw new RTMException(reader.errorCode(), reader.errorException(), reader.errorRaiser());
			}
			else
			{
				string tokenRead = reader.want<string>("token");
				string endpoint = reader.want<string>("endpoint");
				FPQWriter qwSend = new FPQWriter(8, "sendroomfile");
				qwSend.param("pid", this.pid);
				qwSend.param("token", tokenRead);
				qwSend.param("mtype", mtype);
				qwSend.param("from", this.uid);
				qwSend.param("rid", rid);
				qwSend.param("mid", this.genMid());
				qwSend.param("file", fileContent);
				qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
				this.sendFileQuest(endpoint, qwSend.take());
			}
		}

		public void broadcastFile(byte mtype, long gid, byte[] fileContent, string fileName, string fileExtension, RTMQuestCallback cb)
		{
			if (this.gate == null)
			{
				this.procException(cb, "broadcastFile", "can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					this.procException(cb, "broadcastFile", "you can only send " + this.sendFileClientNumMaxLimit + " files one time");
					return;
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(1, "filetoken");
			qw.param("cmd", "broadcastfile");

			this.gate.sendQuest(qw.take(), delegate (FPAReader reader)
			{
				if (reader.isError())
				{
					this.procException(cb, "broadcastFile", reader);
					lock (this.sendFileLock)
					{
						this.sendFileClientNum--;
					}
				}
				else
				{
					string tokenRead = reader.want<string>("token");
					string endpoint = reader.want<string>("endpoint");
					FPQWriter qwSend = new FPQWriter(7, "broadcastfile");
					qwSend.param("pid", this.pid);
					qwSend.param("token", tokenRead);
					qwSend.param("mtype", mtype);
					qwSend.param("from", this.uid);
					qwSend.param("mid", this.genMid());
					qwSend.param("file", fileContent);
					qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
					this.sendFileQuest(endpoint, qwSend.take(), "broadcastFile", cb);
				}
			});
		}

		public void broadcastFile(byte mtype, long gid, byte[] fileContent, string fileName, string fileExtension)
		{
			if (this.gate == null)
			{
				throw new RTMException("can not send quest before auth");
			}

			lock (this.sendFileLock)
			{
				if (this.sendFileClientNum >= this.sendFileClientNumMaxLimit)
				{
					throw new RTMException("you can only send " + this.sendFileClientNumMaxLimit + " files one time");
				}
				else
					this.sendFileClientNum++;
			}

			FPQWriter qw = new FPQWriter(1, "filetoken");
			qw.param("cmd", "broadcastfile");

			FPAReader reader = this.gate.sendQuestSync(qw.take());

			if (reader.isError())
			{
				lock (this.sendFileLock)
				{
					this.sendFileClientNum--;
				}
				throw new RTMException(reader.errorCode(), reader.errorException(), reader.errorRaiser());
			}
			else
			{
				string tokenRead = reader.want<string>("token");
				string endpoint = reader.want<string>("endpoint");
				FPQWriter qwSend = new FPQWriter(7, "broadcastfile");
				qwSend.param("pid", this.pid);
				qwSend.param("token", tokenRead);
				qwSend.param("mtype", mtype);
				qwSend.param("from", this.uid);
				qwSend.param("mid", this.genMid());
				qwSend.param("file", fileContent);
				qwSend.param("attrs", this.buildAttrs(tokenRead, fileContent, fileName, fileExtension));
				this.sendFileQuest(endpoint, qwSend.take());
			}
		}

	}
}
