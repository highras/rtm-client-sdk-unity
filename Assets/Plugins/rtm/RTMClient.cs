using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using GameDevWare.Serialization;
using com.fpnn;

using UnityEngine;

namespace com.rtm {

    public class RTMClient {

        private static class MidGenerator {

            static private long Count = 0;
            static private StringBuilder sb = new StringBuilder(20);
            static private object lock_obj = new object();

            static public long Gen() {

                lock (lock_obj) {

                    long c = 0;

                    if (++Count >= 999) {

                        Count = 0;
                    }

                    c = Count;

                    sb.Clear();
                    sb.Append(FPManager.Instance.GetMilliTimestamp());

                    if (c < 100) {

                        sb.Append("0");
                    }

                    if (c < 10) {

                        sb.Append("0");
                    }

                    sb.Append(c);
                    return Convert.ToInt64(sb.ToString());
                }
            }
        }

        private class DelayConnLocker {

            public int Status = 0;
        }

        private class ReconnLocker {

            public int Status = 0;
        }

        private FPEvent _event = new FPEvent();

        public FPEvent GetEvent() {

            return this._event;
        }

        private string _dispatch;
        private int _pid;
        private long _uid;
        private string _token;
        private string _version;
        private IDictionary<string, string> _attrs;
        private bool _reconnect;
        private int _timeout;

        private bool _isClose;
        private string _endpoint;
        private string _switchGate;

        private RTMSender _sender;
        private RTMProcessor _processor;
        private EventDelegate _eventDelegate;

        private BaseClient _baseClient;
        private DispatchClient _dispatchClient;

        /**
         * @param {string}                      dispatch
         * @param {int}                         pid
         * @param {long}                        uid
         * @param {string}                      token
         * @param {string}                      version
         * @param {IDictionary(string,string)}  attrs
         * @param {bool}                        reconnect
         * @param {int}                         timeout
         */
        public RTMClient(string dispatch, int pid, long uid, string token, string version, IDictionary<string, string> attrs, bool reconnect, int timeout) {

            Debug.Log("Hello RTM! rtm@" + RTMConfig.VERSION + ", fpnn@" + FPConfig.VERSION);

            this._dispatch = dispatch;
            this._pid = pid;
            this._uid = uid;
            this._token = token;
            this._version = version;
            this._attrs = new Dictionary<string, string>(attrs);
            this._reconnect = reconnect;
            this._timeout = timeout;

            this.InitProcessor();
        }

        private void InitProcessor() {

            RTMClient self = this;

            this._sender = new RTMSender();
            this._processor = new RTMProcessor(this._event);

            this._processor.AddPushService(RTMConfig.KICKOUT, (data) => {

                lock (self_locker) {

                    self._isClose = true;
                    self._baseClient.Close();
                }
            });

            this._eventDelegate = (evd) => {

                long lastPingTimestamp = 0;
                long timestamp = evd.GetTimestamp();

                if (self._processor != null) {

                    lastPingTimestamp = self._processor.GetPingTimestamp();
                }

                if (lastPingTimestamp > 0 && self._baseClient != null && self._baseClient.IsOpen()) {

                    if (timestamp - lastPingTimestamp > RTMConfig.RECV_PING_TIMEOUT) {

                        self._baseClient.Close(new Exception("ping timeout"));
                    }
                }

                self.DelayConnect(timestamp);
            };

            FPManager.Instance.AddSecond(this._eventDelegate);
            ErrorRecorderHolder.setInstance(new RTMErrorRecorder());
        }

        public RTMProcessor GetProcessor() {

            return this._processor;
        }

        public FPPackage GetPackage() {

            if (this._baseClient != null) {

                return this._baseClient.GetPackage();
            }

            return null;
        }

        public void SendQuest(FPData data, IDictionary<string, object> payload, CallbackDelegate callback, int timeout) {

            if (this._sender != null && this._baseClient != null) {

                this._sender.AddQuest(this._baseClient, data, payload, this._baseClient.QuestCallback(callback), timeout);
            }
        }

        private object self_locker = new object();

        public void Destroy() {

            this.Close();

            lock (delayconn_locker) {

                delayconn_locker.Status = 0;

                this._reconnCount = 0;
                this._lastConnectTime = 0;
            }

            lock (self_locker) {

                if (this._baseClient != null) {

                    this._baseClient.Destroy();
                    this._baseClient = null;
                }

                if (this._dispatchClient != null) {

                    this._dispatchClient.Destroy();
                    this._dispatchClient = null;
                }

                if (this._sender != null) {

                    this._sender.Destroy();
                    this._sender = null;
                }

                if (this._processor != null) {

                    this._processor.Destroy();
                    this._processor = null;
                }

                this._event.RemoveListener();

                if (this._eventDelegate != null) {

                    FPManager.Instance.RemoveSecond(this._eventDelegate);
                    this._eventDelegate = null;
                }
            }
        }

        /**
         * @param {string}  endpoint
         */
        public void Login(string endpoint) {

            lock (self_locker) {

                this._endpoint = endpoint;
                this._isClose = false;

                if (!string.IsNullOrEmpty(this._endpoint)) {

                    this.ConnectRTMGate(this._timeout);
                    return;
                }

                RTMClient self = this;

                if (this._dispatchClient == null) {

                    this._dispatchClient = new DispatchClient(this._dispatch, this._timeout);

                    this._dispatchClient.Client_Close = (evd) => {

                        // Debug.Log("[DispatchClient] closed!");
                        bool reconn = false;

                        lock (self_locker) {

                            if (self._dispatchClient != null) {

                                self._dispatchClient.Destroy();
                                self._dispatchClient = null;
                            }

                            reconn = string.IsNullOrEmpty(self._endpoint);
                        }

                        if (reconn) {

                            self.Reconnect();
                        }
                    };

                    this._dispatchClient.Client_Connect = (evd) => {

                        // Debug.Log("[DispatchClient] connected!");
                        IDictionary<string, object> payload = new Dictionary<string, object>();

                        payload.Add("pid", self._pid);
                        payload.Add("uid", self._uid);
                        payload.Add("what", "rtmGated");
                        payload.Add("addrType", self._dispatchClient.IsIPv6() ? "ipv6" : "ipv4");
                        payload.Add("version", self._version);

                        self._dispatchClient.Which(self._sender, payload, self._timeout, (cbd) => {

                            IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                            lock (self_locker) {

                                if (dict != null) {

                                    self._endpoint = Convert.ToString(dict["endpoint"]);
                                }

                                if (self._dispatchClient != null) {

                                    self._dispatchClient.Close(cbd.GetException());
                                }
                            }

                            self.Login(self._endpoint);
                        });
                    };

                    this._dispatchClient.Connect();
                }
            }
        }

        /**
         *
         * rtmGate (2)
         *
         * @param {long}                    to
         * @param {byte}                    mtype
         * @param {string}                  msg
         * @param {string}                  attrs
         * @param {long}                    mid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary(mtime:long)} payload 
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void SendMessage(long to, byte mtype, String msg, String attrs, long mid, int timeout, CallbackDelegate callback) {

            if (mid == 0) {

                mid = MidGenerator.Gen();
            }

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("to", to);
            payload.Add("mid", mid);
            payload.Add("mtype", mtype);
            payload.Add("msg", msg);
            payload.Add("attrs", attrs);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("sendmsg");

            this.SendQuest(data, payload, (cbd) => {

                cbd.SetMid(mid);

                if (callback != null) {

                    callback(cbd);
                }
            }, timeout);
        }

        /**
         *
         * rtmGate (3)
         *
         * @param {long}                    gid
         * @param {byte}                    mtype
         * @param {string}                  msg
         * @param {string}                  attrs
         * @param {long}                    mid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary(mtime:long)} payload 
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void SendGroupMessage(long gid, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback) {

            if (mid == 0) {

                mid = MidGenerator.Gen();
            }

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);
            payload.Add("mid", mid);
            payload.Add("mtype", mtype);
            payload.Add("msg", msg);
            payload.Add("attrs", attrs);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("sendgroupmsg");

            this.SendQuest(data, payload, (cbd) => {

                cbd.SetMid(mid);

                if (callback != null) {

                    callback(cbd);
                }
            }, timeout);
        }

        /**
         *
         * rtmGate (4)
         *
         * @param {long}                    rid
         * @param {byte}                    mtype
         * @param {string}                  msg
         * @param {string}                  attrs
         * @param {long}                    mid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary(mtime:long)} payload 
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void SendRoomMessage(long rid, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback) {

            if (mid == 0) {

                mid = MidGenerator.Gen();
            }

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("rid", rid);
            payload.Add("mid", mid);
            payload.Add("mtype", mtype);
            payload.Add("msg", msg);
            payload.Add("attrs", attrs);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("sendroommsg");

            this.SendQuest(data, payload, (cbd) => {

                cbd.SetMid(mid);

                if (callback != null) {

                    callback(cbd);
                }
            }, timeout);
        }

        /**
         *
         * rtmGate (5)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {long}                    mid
         * @param {Exception}               exception
         * @param {IDictionary(p2p:IDictionary(String,int),group:IDictionary(String,int))} payload
         * </CallbackData>
         */
        public void GetUnreadMessage(int timeout, CallbackDelegate callback) {

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getunreadmsg");

            this.SendQuest(data, new Dictionary<string, object>(), callback, timeout);
        }

        /**
         *
         * rtmGate (6)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void CleanUnreadMessage(int timeout, CallbackDelegate callback) {

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("cleanunreadmsg");

            this.SendQuest(data, new Dictionary<string, object>(), callback, timeout);
        }

         /**
         *
         * rtmGate (7)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {long}                    mid
         * @param {Exception}               exception
         * @param {IDictionary(p2p:Map(String,long),IDictionary:Map(String,long))}    payload
         * </CallbackData>
         */
        public void GetSession(int timeout, CallbackDelegate callback) {

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getsession");

            this.SendQuest(data, new Dictionary<string, object>(), callback, timeout);
        }

        /**
         *
         * rtmGate (8)
         *
         * @param {long}                    gid
         * @param {bool}                    desc
         * @param {int}                     num
         * @param {long}                    begin
         * @param {long}                    end
         * @param {long}                    lastid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(GroupMsg))} payload
         * </CallbackData>
         *
         * <GroupMsg>
         * @param {long}                    id
         * @param {long}                    from
         * @param {byte}                    mtype
         * @param {long}                    mid
         * @param {bool}                    deleted
         * @param {string}                  msg
         * @param {string}                  attrs
         * @param {long}                    mtime
         * </GroupMsg>
         */
        public void GetGroupMessage(long gid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);
            payload.Add("desc", desc);
            payload.Add("num", num);

            if (begin > 0) {

                payload.Add("begin", begin);
            }

            if (end > 0) {

                payload.Add("end", end);
            }

            if (lastid > 0) {

                payload.Add("lastid", lastid);
            }

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getgroupmsg");

            this.SendQuest(data, payload, (cbd) => {

                if (callback == null) {

                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<object> ol = (List<object>)dict["msgs"];
                    List<IDictionary<string, object>> nl = new List<IDictionary<string, object>>();

                    foreach (List<object> items in ol) {

                        IDictionary<string, object> map = new Dictionary<string, object>();

                        map.Add("id", items[0]);
                        map.Add("from", items[1]);
                        map.Add("mtype", items[2]);
                        map.Add("mid", items[3]);
                        map.Add("deleted", items[4]);
                        map.Add("msg", items[5]);
                        map.Add("attrs", items[6]);
                        map.Add("mtime", items[7]);

                        nl.Add(map);
                    }

                    dict["msgs"] = nl;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (9)
         *
         * @param {long}                    rid
         * @param {bool}                    desc
         * @param {int}                     num
         * @param {long}                    begin
         * @param {long}                    end
         * @param {long}                    lastid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(RoomMsg))} payload
         * </CallbackData>
         *
         * <RoomMsg>
         * @param {long}                    id
         * @param {long}                    from
         * @param {byte}                    mtype
         * @param {long}                    mid
         * @param {bool}                    deleted
         * @param {string}                  msg
         * @param {string}                  attrs
         * @param {long}                    mtime
         * </RoomMsg>
         */
        public void GetRoomMessage(long rid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("rid", rid);
            payload.Add("desc", desc);
            payload.Add("num", num);

            if (begin > 0) {

                payload.Add("begin", begin);
            }

            if (end > 0) {

                payload.Add("end", end);
            }

            if (lastid > 0) {

                payload.Add("lastid", lastid);
            }

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getroommsg");

            this.SendQuest(data, payload, (cbd) => {

                if (callback == null) {

                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<object> ol = (List<object>)dict["msgs"];
                    List<IDictionary<string, object>> nl = new List<IDictionary<string, object>>();

                    foreach (List<object> items in ol) {

                        IDictionary<string, object> map = new Dictionary<string, object>();

                        map.Add("id", items[0]);
                        map.Add("from", items[1]);
                        map.Add("mtype", items[2]);
                        map.Add("mid", items[3]);
                        map.Add("deleted", items[4]);
                        map.Add("msg", items[5]);
                        map.Add("attrs", items[6]);
                        map.Add("mtime", items[7]);

                        nl.Add(map);
                    }

                    dict["msgs"] = nl;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (10)
         *
         * @param {bool}                    desc
         * @param {int}                     num
         * @param {long}                    begin
         * @param {long}                    end
         * @param {long}                    lastid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(BroadcastMsg))} payload
         * </CallbackData>
         *
         * <BroadcastMsg>
         * @param {long}                    id
         * @param {long}                    from
         * @param {byte}                    mtype
         * @param {long}                    mid
         * @param {bool}                    deleted
         * @param {string}                  msg
         * @param {string}                  attrs
         * @param {long}                    mtime
         * </BroadcastMsg>
         */
        public void GetBroadcastMessage(bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("desc", desc);
            payload.Add("num", num);

            if (begin > 0) {

                payload.Add("begin", begin);
            }

            if (end > 0) {

                payload.Add("end", end);
            }

            if (lastid > 0) {

                payload.Add("lastid", lastid);
            }

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getbroadcastmsg");

            this.SendQuest(data, payload, (cbd) => {

                if (callback == null) {

                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<object> ol = (List<object>)dict["msgs"];
                    List<IDictionary<string, object>> nl = new List<IDictionary<string, object>>();

                    foreach (List<object> items in ol) {

                        IDictionary<string, object> map = new Dictionary<string, object>();

                        map.Add("id", items[0]);
                        map.Add("from", items[1]);
                        map.Add("mtype", items[2]);
                        map.Add("mid", items[3]);
                        map.Add("deleted", items[4]);
                        map.Add("msg", items[5]);
                        map.Add("attrs", items[6]);
                        map.Add("mtime", items[7]);

                        nl.Add(map);
                    }

                    dict["msgs"] = nl;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (11)
         *
         * @param {long}                    ouid
         * @param {bool}                    desc
         * @param {int}                     num
         * @param {long}                    begin
         * @param {long}                    end
         * @param {long}                    lastid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(P2PMsg))} payload
         * </CallbackData>
         *
         * <P2PMsg>
         * @param {long}                    id
         * @param {byte}                    direction
         * @param {byte}                    mtype
         * @param {long}                    mid
         * @param {bool}                    deleted
         * @param {string}                  msg
         * @param {string}                  attrs
         * @param {long}                    mtime
         * </P2PMsg>
         */
        public void GetP2PMessage(long ouid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("ouid", ouid);
            payload.Add("desc", desc);
            payload.Add("num", num);

            if (begin > 0) {

                payload.Add("begin", begin);
            }

            if (end > 0) {

                payload.Add("end", end);
            }

            if (lastid > 0) {

                payload.Add("lastid", lastid);
            }

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getp2pmsg");

            this.SendQuest(data, payload, (cbd) => {

                if (callback == null) {

                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<object> ol = (List<object>)dict["msgs"];
                    List<IDictionary<string, object>> nl = new List<IDictionary<string, object>>();

                    foreach (List<object> items in ol) {

                        IDictionary<string, object> map = new Dictionary<string, object>();

                        map.Add("id", items[0]);
                        map.Add("direction", items[1]);
                        map.Add("mtype", items[2]);
                        map.Add("mid", items[3]);
                        map.Add("deleted", items[4]);
                        map.Add("msg", items[5]);
                        map.Add("attrs", items[6]);
                        map.Add("mtime", items[7]);

                        nl.Add(map);
                    }

                    dict["msgs"] = nl;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (12)
         *
         * @param {string}                  cmd
         * @param {List(long)}              tos
         * @param {long}                    to
         * @param {long}                    rid
         * @param {long}                    gid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(token:string,endpoint:string)}   payload
         * </CallbackData>
         */
        public void FileToken(string cmd, List<long> tos, long to, long rid, long gid, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("cmd", cmd);

            if (tos != null && tos.Count > 0) {

                payload.Add("tos", tos);
            }

            if (to > 0) {

                payload.Add("to", to);
            }

            if (rid > 0) {

                payload.Add("rid", rid);
            }

            if (gid > 0) {

                payload.Add("gid", gid);
            }

            this.Filetoken(payload, callback, timeout);
        }

        /**
         * rtmGate (13)
         */
        public void Close() {

            lock (self_locker) {

                this._isClose = true;
            }

            IDictionary<string, object> payload = new Dictionary<string, object>();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("bye");

            RTMClient self = this;

            this.SendQuest(data, payload, (cbd) => {

                lock (self_locker) {

                    self._baseClient.Close();
                }
            }, 0);
        }

        /**
         *
         * rtmGate (14)
         *
         * @param {IDictionary(string,string)}      attrs
         * @param {int}                             timeout
         * @param {CallbackDelegate}                callback
         *
         * @callback
         * @param {CallbackData}                    cbd
         *
         * <CallbackData>
         * @param {Exception}                       exception
         * @param {IDictionary}                     payload
         * </CallbackData>
         */
        public void AddAttrs(IDictionary<string, string> attrs, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("attrs", attrs);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("addattrs");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (15)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(attrs:List(IDictionary<string, string>))}    payload
         * </CallbackData>
         *
         * <IDictionary<string, string>>
         * @param {string}                  ce
         * @param {string}                  login
         * @param {string}                  my
         * </IDictionary<string, string>>
         */
        public void GetAttrs(int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getattrs");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (16)
         *
         * @param {string}                  msg
         * @param {string}                  attrs
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void AddDebugLog(string msg, string attrs, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("msg", msg);
            payload.Add("attrs", attrs);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("adddebuglog");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (17)
         *
         * @param {string}                  apptype
         * @param {string}                  devicetoken
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void AddDevice(string apptype, string devicetoken, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("apptype", apptype);
            payload.Add("devicetoken", devicetoken);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("adddevice");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (18)
         *
         * @param {string}                  devicetoken
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void RemoveDevice(string devicetoken, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("devicetoken", devicetoken);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("removedevice");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (19)
         *
         * @param {string}                  targetLanguage
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void SetTranslationLanguage(string targetLanguage, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("lang", targetLanguage);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("setlang");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (20)
         *
         * @param {string}                  originalMessage
         * @param {string}                  originalLanguage
         * @param {string}                  targetLanguage
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(stext:string,src:string,dtext:string,dst:string)}    payload
         * </CallbackData>
         */
        public void Translate(string originalMessage, string originalLanguage, string targetLanguage, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("text", originalMessage);
            payload.Add("dst", targetLanguage);

            if (!string.IsNullOrEmpty(originalLanguage)) {

                payload.Add("src", originalLanguage);
            }

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("translate");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (21)
         *
         * @param {List(long)}              friends
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void AddFriends(List<long> friends, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("friends", friends);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("addfriends");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (22)
         *
         * @param {List(long)}              friends
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void DeleteFriends(List<long> friends, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("friends", friends);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("delfriends");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (23)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {List(long)}              payload
         * </CallbackData>
         */
        public void GetFriends(int timeout, CallbackDelegate callback) {

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getfriends");

            this.SendQuest(data, new Dictionary<string, object>(), (cbd) => {

                if (callback == null) {

                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<object> ids = (List<object>)dict["uids"];
                    callback(new CallbackData(ids));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (24)
         *
         * @param {long}                    gid
         * @param {List(long)}              uids
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void AddGroupMembers(long gid, List<long> uids, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);
            payload.Add("uids", uids);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("addgroupmembers");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (25)
         *
         * @param {long}                    gid
         * @param {List(long)}              uids
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void DeleteGroupMembers(long gid, List<long> uids, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);
            payload.Add("uids", uids);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("delgroupmembers");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (26)
         *
         * @param {long}                    gid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {List(long)}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void GetGroupMembers(long gid, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getgroupmembers");

            this.SendQuest(data, payload, (cbd) => {

                if (callback == null) {

                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<object> ids = (List<object>)dict["uids"];
                    callback(new CallbackData(ids));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (27)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {List(long)}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void GetUserGroups(int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getusergroups");

            this.SendQuest(data, payload, (cbd) => {

                if (callback == null) {

                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<object> ids = (List<object>)dict["gids"];
                    callback(new CallbackData(ids));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (28)
         *
         * @param {long}                    rid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void EnterRoom(long rid, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("rid", rid);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("enterroom");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (29)
         *
         * @param {long}                    rid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void LeaveRoom(long rid, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("rid", rid);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("leaveroom");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (30)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {List(long)}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void GetUserRooms(int timeout, CallbackDelegate callback) {

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getuserrooms");

            this.SendQuest(data, new Dictionary<string, object>(), (cbd) => {

                if (callback == null) {

                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<object> ids = (List<object>)dict["rooms"];
                    callback(new CallbackData(ids));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (31)
         *
         * @param {List(long)}              uids
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {List(long)}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void GetOnlineUsers(List<long> uids, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("uids", uids);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getonlineusers");

            this.SendQuest(data, payload, (cbd) => {

                if (callback == null) {

                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<object> ids = (List<object>)dict["uids"];
                    callback(new CallbackData(ids));
                    return;
                }

                callback(cbd);
            }, timeout);            
        }

        /**
         *
         * rtmGate (32)
         *
         * @param {long}                    mid
         * @param {long}                    xid
         * @param {byte}                    type
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void DeleteMessage(long mid, long xid, byte type, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("mid", mid);
            payload.Add("xid", xid);
            payload.Add("type", type);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("delmsg");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (33)
         *
         * @param {string}                  ce
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void Kickout(string ce, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("ce", ce);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("kickout");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (34)
         *
         * @param {string}                  key
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(val:String)} payload
         * </CallbackData>
         */
        public void DBGet(string key, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("key", key);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("dbget");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * rtmGate (35)
         *
         * @param {string}                  key
         * @param {string}                  value
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary}             payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void DBSet(string key, string value, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("key", key);
            payload.Add("val", value);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("dbset");

            this.SendQuest(data, payload, callback, timeout);
        }

        /**
         *
         * fileGate (1)
         *
         * @param {byte}                    mtype
         * @param {long}                    to
         * @param {byte[]}                  fileBytes
         * @param {long}                    mid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary(mtime:long)} payload
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void SendFile(byte mtype, long to, byte[] fileBytes, long mid, int timeout, CallbackDelegate callback) {

            if (fileBytes == null || fileBytes.Length <= 0) {

                this.GetEvent().FireEvent(new EventData("error", new Exception("empty file bytes!")));
                return;
            }

            Hashtable ops = new Hashtable();

            ops.Add("cmd", "sendfile");
            ops.Add("to", to);
            ops.Add("mtype", mtype);
            ops.Add("file", fileBytes);

            this.FileSendProcess(ops, mid, timeout, callback);
        }

        /**
         *
         * fileGate (3)
         *
         * @param {byte}                    mtype
         * @param {long}                    gid
         * @param {byte[]}                  fileBytes
         * @param {long}                    mid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary(mtime:long)} payload
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void SendGroupFile(byte mtype, long gid, byte[] fileBytes, long mid, int timeout, CallbackDelegate callback) {

            if (fileBytes == null || fileBytes.Length <= 0) {

                this.GetEvent().FireEvent(new EventData("error", new Exception("empty file bytes!")));
                return;
            }

            Hashtable ops = new Hashtable();

            ops.Add("cmd", "sendgroupfile");
            ops.Add("gid", gid);
            ops.Add("mtype", mtype);
            ops.Add("file", fileBytes);

            this.FileSendProcess(ops, mid, timeout, callback);
        }

        /**
         *
         * fileGate (4)
         *
         * @param {byte}                    mtype
         * @param {long}                    rid
         * @param {byte[]}                  fileBytes
         * @param {long}                    mid
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {IDictionary(mtime:long)} payload
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void SendRoomFile(byte mtype, long rid, byte[] fileBytes, long mid, int timeout, CallbackDelegate callback) {

            if (fileBytes == null || fileBytes.Length <= 0) {

                this.GetEvent().FireEvent(new EventData("error", new Exception("empty file bytes!")));
                return;
            }

            Hashtable ops = new Hashtable();

            ops.Add("cmd", "sendroomfile");
            ops.Add("rid", rid);
            ops.Add("mtype", mtype);
            ops.Add("file", fileBytes);

            this.FileSendProcess(ops, mid, timeout, callback);
        }

        /**
         *
         * rtmGate (1)
         *
         */
        private void Auth(int timeout) {

            RTMClient self = this;
            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("pid", this._pid);
            payload.Add("uid", this._uid);
            payload.Add("token", this._token);
            payload.Add("version", this._version);
            payload.Add("attrs", this._attrs);

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("auth");

            this.SendQuest(data, payload, (cbd) => {

                Exception exception = cbd.GetException();

                if (exception != null) {

                    lock (self_locker) {

                        if (self._baseClient != null) {

                            self._baseClient.Close(exception);
                        }
                    }

                    return;
                }

                object obj = cbd.GetPayload();

                if (obj != null) {

                    IDictionary<string, object> dict = (IDictionary<string, object>)obj;

                    bool ok = Convert.ToBoolean(dict["ok"]);

                    if (ok) {

                        if (self._processor != null) {

                            self._processor.InitPingTimestamp();
                        }

                        lock (delayconn_locker) {

                            self._reconnCount = 0;
                        }

                        string endpoint = null;

                        lock (self_locker) {

                            endpoint = self._endpoint;
                        }

                        self.GetEvent().FireEvent(new EventData("login", endpoint));
                        return;
                    }

                    if (dict.ContainsKey("gate")) {

                        string gate = Convert.ToString(dict["gate"]);

                        if (!string.IsNullOrEmpty(gate)) {

                            lock (self_locker) {

                                self._switchGate = gate;

                                if (self._baseClient != null) {

                                    self._baseClient.Close();
                                }
                            }

                            return;
                        }
                    }

                    if (!ok) {

                        lock (self_locker) {

                            self._isClose = true;
                        }

                        self.GetEvent().FireEvent(new EventData("login", new Exception("token error!")));
                        return;
                    }
                }

                lock (self_locker) {

                    if (self._baseClient != null) {

                        self._baseClient.Close(new Exception("auth error!"));
                    }
                }
            }, timeout);
        }

        private void ConnectRTMGate(int timeout) {

            RTMClient self = this;

            if (this._baseClient == null) {

                this._baseClient = new BaseClient(this._endpoint, timeout);

                this._baseClient.Client_Connect = (evd) => {

                    self.Auth(timeout);
                };

                this._baseClient.Client_Close = (evd) => {

                    bool retry = false;

                    lock (self_locker) {

                        if (self._baseClient != null) {

                            self._baseClient.Destroy();
                            self._baseClient = null;
                        }

                        self._endpoint = self._switchGate;
                        self._switchGate = null;

                        retry = !self._isClose && self._reconnect;
                    }

                    self.GetEvent().FireEvent(new EventData("close", retry));
                    self.Reconnect();
                };

                this._baseClient.Client_Error = (evd) => {

                    self.GetEvent().FireEvent(new EventData("error", evd.GetException()));
                };

                this._baseClient.GetProcessor().SetProcessor(this._processor);
                this._baseClient.Connect();
            }
        }

        private void FileSendProcess(Hashtable ops, long mid, int timeout, CallbackDelegate callback) {

            IDictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("cmd", ops["cmd"]);

            if (ops.Contains("tos")) {

                payload.Add("tos", ops["tos"]);
            }

            if (ops.Contains("to")) {

                payload.Add("to", ops["to"]);
            }

            if (ops.Contains("rid")) {

                payload.Add("rid", ops["rid"]);
            }

            if (ops.Contains("gid")) {

                payload.Add("gid", ops["gid"]);
            }

            RTMClient self = this;

            this.Filetoken(payload, (cbd) => {

                Exception exception = cbd.GetException();

                if (exception != null) {

                    self.GetEvent().FireEvent(new EventData("error", exception));
                    return;
                }

                object obj = cbd.GetPayload();

                if (obj != null) {

                    IDictionary<string, object> dict = (IDictionary<string, object>)obj;

                    string token = Convert.ToString(dict["token"]);
                    string endpoint = Convert.ToString(dict["endpoint"]);

                    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(endpoint)) {

                        self.GetEvent().FireEvent(new EventData("error", new Exception("file token error!")));
                        return;
                    }

                    FileClient fileClient = new FileClient(endpoint, timeout);

                    dict = new Dictionary<string, object>();

                    dict.Add("pid", self._pid);
                    dict.Add("mtype", ops["mtype"]);
                    dict.Add("mid", mid != 0 ? mid : MidGenerator.Gen());
                    dict.Add("from", self._uid);

                    if (ops.Contains("tos")) {

                        dict.Add("tos", ops["tos"]);
                    }

                    if (ops.Contains("to")) {

                        dict.Add("to", ops["to"]);
                    }

                    if (ops.Contains("rid")) {

                        dict.Add("rid", ops["rid"]);
                    }

                    if (ops.Contains("gid")) {

                        dict.Add("gid", ops["gid"]);
                    }

                    fileClient.Send(self._sender, Convert.ToString(ops["cmd"]), (byte[])ops["file"], token, dict, timeout, callback);
                }
            }, timeout);
        }

        private void Filetoken(IDictionary<string, object> payload, CallbackDelegate callback, int timeout) {

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("filetoken");

            this.SendQuest(data, payload, callback, timeout);
        }

        private int _reconnCount = 0;

        private void Reconnect() {

            if (!this._reconnect) {

                return;
            }

            lock (self_locker) {

                if (this._isClose) {

                    return;
                }
            }

            if (this._processor != null) {

                this._processor.ClearPingTimestamp();
            }

            int count = 0;

            lock (delayconn_locker) {

                this._reconnCount++;
                count = this._reconnCount;
            }

            if (count <= RTMConfig.RECONN_COUNT_ONCE) {

                this.Login(this._endpoint);
                return;
            }

            lock (delayconn_locker) {

                delayconn_locker.Status = 1;
                this._lastConnectTime = FPManager.Instance.GetMilliTimestamp();
            }
        }

        private long _lastConnectTime = 0;
        private DelayConnLocker delayconn_locker = new DelayConnLocker();

        private void DelayConnect(long timestamp) {

            lock (delayconn_locker) {

                if (delayconn_locker.Status == 0) {

                    return;
                }

                if (timestamp - this._lastConnectTime < RTMConfig.CONNCT_INTERVAL) {

                    return;
                }

                delayconn_locker.Status = 0;

                this._reconnCount = 0;
            }

            this.Login(this._endpoint);
        }

        private class DispatchClient:BaseClient {

            public DispatchClient(string endpoint, int timeout):base(endpoint, timeout) {}
            public DispatchClient(string host, int port, int timeout):base(host, port, timeout) {}

            public override void AddListener() {

                base.Client_Error = (evd) => {

                    ErrorRecorderHolder.recordError(evd.GetException());
                };
            }

            public void Which(RTMSender sender, IDictionary<string, object> payload, int timeout, CallbackDelegate callback) {

                if (sender != null) {

                    FPData data = new FPData();
                    data.SetFlag(0x1);
                    data.SetMtype(0x1);
                    data.SetMethod("which");

                    sender.AddQuest(this, data, payload, this.QuestCallback(callback), timeout);
                }
            }
        }

        private class FileClient:BaseClient {

            public FileClient(string endpoint, int timeout):base(endpoint, timeout) {}
            public FileClient(string host, int port, int timeout):base(host, port, timeout) {}

            public override void AddListener() {

                // base.Client_Connect = (evd) => {

                //     Debug.Log("[FileClient] connected!");
                // };

                // base.Client_Close = (evd) => {

                //     Debug.Log("[FileClient] closed!");
                // };

                base.Client_Error = (evd) => {

                    ErrorRecorderHolder.recordError(evd.GetException());
                };
            }

            public void Send(RTMSender sender, string method, byte[] fileBytes, string token, IDictionary<string, object> payload, int timeout, CallbackDelegate callback) {

                if (sender != null) {

                    String fileMd5 = base.CalcMd5(fileBytes, false);
                    String sign = base.CalcMd5(fileMd5 + ":" + token, false);

                    if (string.IsNullOrEmpty(sign)) {

                        ErrorRecorderHolder.recordError(new Exception("wrong sign!"));
                        return;
                    }

                    if (!base.HasConnect()) {

                        base.Connect();
                    }

                    IDictionary<string, string> attrs = new Dictionary<string, string>();
                    attrs.Add("sign", sign);

                    payload.Add("token", token);
                    payload.Add("file", fileBytes);
                    payload.Add("attrs", Json.SerializeToString(attrs));

                    long mid = (long)Convert.ToInt64(payload["mid"]);

                    FPData data = new FPData();
                    data.SetFlag(0x1);
                    data.SetMtype(0x1);
                    data.SetMethod(method);

                    FileClient self = this;

                    sender.AddQuest(this, data, payload, this.QuestCallback((cbd) => {

                        cbd.SetMid(mid);
                        self.Destroy();

                        if (callback != null) {

                            callback(cbd);
                        }
                    }), timeout);
                }
            }
        }

        private class BaseClient:FPClient {

            public BaseClient(string endpoint, int timeout):base(endpoint, timeout) {

                this.AddListener();
            }

            public BaseClient(string host, int port, int timeout):base(host, port, timeout) {

                this.AddListener();
            }

            public virtual void AddListener() {}

            public string CalcMd5(string str, bool upper) {

                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str);
                return CalcMd5(inputBytes, upper);
            }

            public string CalcMd5(byte[] bytes, bool upper) {

                MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] hash = md5.ComputeHash(bytes);
                
                string f = "x2";

                if (upper) {

                    f = "X2";
                }

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++) {

                    sb.Append(hash[i].ToString(f));
                }

                return sb.ToString();
            }

            private void CheckFPCallback(CallbackData cbd) {

                bool isAnswerException = false;
                FPData data = cbd.GetData();
                IDictionary<string, object> payload = null;

                if (data != null) {

                    if (data.GetFlag() == 0) {

                        try {

                            payload = Json.Deserialize<IDictionary<string, object>>(data.JsonPayload());
                        }catch(Exception ex) {

                            ErrorRecorderHolder.recordError(ex);
                        }
                    }

                    if (data.GetFlag() == 1) {

                        try {

                            using (MemoryStream inputStream = new MemoryStream(data.MsgpackPayload())) {

                                payload = MsgPack.Deserialize<IDictionary<string, object>>(inputStream);
                            }
                        } catch(Exception ex) {

                            ErrorRecorderHolder.recordError(ex);
                        }
                    }

                    if (base.GetPackage().IsAnswer(data)) {

                        isAnswerException = data.GetSS() != 0;
                    }
                }

                cbd.CheckException(isAnswerException, payload);
            }

            public CallbackDelegate QuestCallback(CallbackDelegate callback) {

                BaseClient self = this;

                return (cbd) => {

                    if (callback == null) {

                        return;
                    }

                    self.CheckFPCallback(cbd);
                    callback(cbd);
                };
            }
        }

        private class RTMErrorRecorder:ErrorRecorder {

            public override void recordError(Exception e) {
            
                // Debug
                Debug.LogError(e);
            }
        }
    }
}