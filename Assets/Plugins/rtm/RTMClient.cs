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
            static private System.Object Lock = new System.Object();

            static public long Gen() {

                long c = 0;

                lock(Lock) {

                    if (++Count >= 999) {

                        Count = 0;
                    }

                    c = Count;

                    string strFix = Convert.ToString(c);

                    if (c < 100) {

                        strFix = "0" + strFix;
                    }

                    if (c < 10) {

                        strFix = "0" + strFix;
                    }

                    return Convert.ToInt64(Convert.ToString(ThreadPool.Instance.GetMilliTimestamp()) + strFix);
                }
            }
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
        private Dictionary<String, String> _attrs;
        private bool _reconnect;
        private int _timeout;

        private bool _startTimerThread;

        private bool _ipv6;
        private bool _isClose;
        private string _endpoint;

        private RTMProcessor _processor;

        private BaseClient _baseClient;
        private DispatchClient _dispatchClient;

        /**
         * @param {string}                      dispatch
         * @param {int}                         pid
         * @param {long}                        uid
         * @param {string}                      token
         * @param {string}                      version
         * @param {Dictionary(string,string)}   attrs
         * @param {bool}                        reconnect
         * @param {int}                         timeout
         * @param {bool}                        startTimerThread
         */
        public RTMClient(string dispatch, int pid, long uid, string token, string version, Dictionary<string, string> attrs, bool reconnect, int timeout, bool startTimerThread) {

            this._dispatch = dispatch;
            this._pid = pid;
            this._uid = uid;
            this._token = token;
            this._version = version;
            this._attrs = attrs;
            this._reconnect = reconnect;
            this._timeout = timeout;
            this._startTimerThread = startTimerThread;

            this.InitProcessor();
        }

        private void InitProcessor() {

            RTMClient self = this;

            this._processor = new RTMProcessor(this._event);
            this._processor.GetEvent().AddListener(RTMConfig.SERVER_PUSH.kickOut, (evd) => {

                self._isClose = true;
                self._baseClient.Close();
            });
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

        public void SendQuest(FPData data, CallbackDelegate callback, int timeout) {

            if (this._baseClient != null) {

                this._baseClient.SendQuest(data, this._baseClient.QuestCallback(callback), timeout);
            }
        }

        public CallbackData SendQuest(FPData data, int timeout) {

            if (this._baseClient != null) {

                return this._baseClient.SendQuest(data, timeout);
            }

            return null;
        }

        public void Destroy() {

            this.Close();

            if (this._baseClient != null) {

                this._baseClient.Destroy();
                this._baseClient = null;
            }

            if (this._dispatchClient != null) {

                this._dispatchClient.Destroy();
                this._dispatchClient = null;
            }

            this._event.RemoveListener();
        }

        /**
         * @param {string}  endpoint
         * @param {bool}    ipv6
         */
        public void Login(string endpoint, bool ipv6) {

            this._endpoint = endpoint;
            this._ipv6 = ipv6;
            this._isClose = false;

            if (!string.IsNullOrEmpty(this._endpoint)) {

                this.ConnectRTMGate(this._timeout);
                return;
            }

            RTMClient self = this;

            if (this._dispatchClient == null) {

                this._dispatchClient = new DispatchClient(this._dispatch, this._timeout, this._startTimerThread);

                this._dispatchClient.GetEvent().AddListener("close", (evd) => {

                    Debug.Log("[DispatchClient] closed!");

                    if (self._dispatchClient != null) {

                        self._dispatchClient.Destroy();
                        self._dispatchClient = null;
                    }

                    if (string.IsNullOrEmpty(self._endpoint)) {

                        self.GetEvent().FireEvent(new EventData("error", new Exception("dispatch client close with err!")));
                        self.Reconnect();
                    }
                });
            }

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("pid", this._pid);
            payload.Add("uid", this._uid);
            payload.Add("what", "rtmGated");
            payload.Add("addrType", this._ipv6 ? "ipv6" : "ipv4");
            payload.Add("version", this._version);

            this._dispatchClient.Which(payload, this._timeout, (cbd) => {

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    self.Login(Convert.ToString(dict["endpoint"]), self._ipv6);
                }

                Exception ex = cbd.GetException();

                if (ex != null) {

                    self.GetEvent().FireEvent(new EventData("error", ex));
                }
            });
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
         * @param {Dictionary(mtime:long)}  payload 
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void SendMessage(long to, byte mtype, String msg, String attrs, long mid, int timeout, CallbackDelegate callback) {

            if (mid == 0) {

                mid = MidGenerator.Gen();
            }

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("to", to);
            payload.Add("mid", mid);
            payload.Add("mtype", mtype);
            payload.Add("msg", msg);
            payload.Add("attrs", attrs);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("sendmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

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
         * @param {Dictionary(mtime:long)}  payload 
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void SendGroupMessage(long gid, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback) {

            if (mid == 0) {

                mid = MidGenerator.Gen();
            }

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);
            payload.Add("mid", mid);
            payload.Add("mtype", mtype);
            payload.Add("msg", msg);
            payload.Add("attrs", attrs);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("sendgroupmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

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
         * @param {Dictionary(mtime:long)}  payload 
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void SendRoomMessage(long rid, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback) {

            if (mid == 0) {

                mid = MidGenerator.Gen();
            }

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("rid", rid);
            payload.Add("mid", mid);
            payload.Add("mtype", mtype);
            payload.Add("msg", msg);
            payload.Add("attrs", attrs);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("sendroommsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

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
         * @param {Dictionary(p2p:Dictionary(String,int),group:Dictionary(String,int))} payload
         * </CallbackData>
         */
        public void GetUnreadMessage(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getunreadmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * @param {long}                    mid
         * </CallbackData>
         */
        public void CleanUnreadMessage(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("cleanunreadmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary(p2p:Map(String,long),Dictionary:Map(String,long))}    payload
         * </CallbackData>
         */
        public void GetSession(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getsession");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary(num:int,lastid:long,begin:long,end:long,msgs:List(GroupMsg))} payload
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

            Dictionary<string, object> payload = new Dictionary<string, object>();

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

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getgroupmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

                if (callback == null) {

                    return;
                }

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<ArrayList> ol = (List<ArrayList>)dict["msgs"];
                    List<Hashtable> nl = new List<Hashtable>();

                    foreach (ArrayList items in ol) {

                        Hashtable map = new Hashtable();

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
         * @param {Dictionary(num:int,lastid:long,begin:long,end:long,msgs:List(RoomMsg))} payload
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

            Dictionary<string, object> payload = new Dictionary<string, object>();

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

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getroommsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

                if (callback == null) {

                    return;
                }

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<ArrayList> ol = (List<ArrayList>)dict["msgs"];
                    List<Hashtable> nl = new List<Hashtable>();

                    foreach (ArrayList items in ol) {

                        Hashtable map = new Hashtable();

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
         * @param {Dictionary(num:int,lastid:long,begin:long,end:long,msgs:List(BroadcastMsg))} payload
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

            Dictionary<string, object> payload = new Dictionary<string, object>();

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

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getbroadcastmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

                if (callback == null) {

                    return;
                }

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<ArrayList> ol = (List<ArrayList>)dict["msgs"];
                    List<Hashtable> nl = new List<Hashtable>();

                    foreach (ArrayList items in ol) {

                        Hashtable map = new Hashtable();

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
         * @param {Dictionary(num:int,lastid:long,begin:long,end:long,msgs:List(P2PMsg))} payload
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

            Dictionary<string, object> payload = new Dictionary<string, object>();

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

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getp2pmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

                if (callback == null) {

                    return;
                }

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<ArrayList> ol = (List<ArrayList>)dict["msgs"];
                    List<Hashtable> nl = new List<Hashtable>();

                    foreach (ArrayList items in ol) {

                        Hashtable map = new Hashtable();

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
         * @param {Dictionary(token:string,endpoint:string)}   payload
         * </CallbackData>
         */
        public void FileToken(string cmd, List<long> tos, long to, long rid, long gid, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

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

            this._isClose = true;

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("bye");
            data.SetPayload(bytes);

            RTMClient self = this;

            this.SendQuest(data, (cbd) => {

                self._baseClient.Close();
            }, 0);
        }

        /**
         *
         * rtmGate (14)
         *
         * @param {Dictionary(string,string)}       attrs
         * @param {int}                             timeout
         * @param {CallbackDelegate}                callback
         *
         * @callback
         * @param {CallbackData}                    cbd
         *
         * <CallbackData>
         * @param {Exception}                       exception
         * @param {Dictionary}                      payload
         * </CallbackData>
         */
        public void AddAttrs(Dictionary<string, string> attrs, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("attrs", attrs);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("addattrs");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary(attrs:List(Hashtable))}    payload
         * </CallbackData>
         *
         * <Hashtable>
         * @param {string}                  ce
         * @param {string}                  login
         * @param {string}                  my
         * </Hashtable>
         */
        public void GetAttrs(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getattrs");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void AddDebugLog(string msg, string attrs, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("msg", msg);
            payload.Add("attrs", attrs);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("adddebuglog");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void AddDevice(string apptype, string devicetoken, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("apptype", apptype);
            payload.Add("devicetoken", devicetoken);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("adddevice");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void RemoveDevice(string devicetoken, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("devicetoken", devicetoken);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("removedevice");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void SetTranslationLanguage(string targetLanguage, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("lang", targetLanguage);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("setlang");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary(stext:string,src:string,dtext:string,dst:string)}    payload
         * </CallbackData>
         */
        public void Translate(string originalMessage, string originalLanguage, string targetLanguage, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("text", originalMessage);
            payload.Add("dst", targetLanguage);

            if (!string.IsNullOrEmpty(originalLanguage)) {

                payload.Add("src", originalLanguage);
            }

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("translate");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void AddFriends(List<long> friends, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("friends", friends);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("addfriends");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void DeleteFriends(List<long> friends, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("friends", friends);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("delfriends");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getfriends");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

                if (callback == null) {

                    return;
                }

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<long> ids = (List<long>)dict["uids"];
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void AddGroupMembers(long gid, List<long> uids, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);
            payload.Add("uids", uids);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("addgroupmembers");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void DeleteGroupMembers(long gid, List<long> uids, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);
            payload.Add("uids", uids);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("delgroupmembers");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getgroupmembers");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

                if (callback == null) {

                    return;
                }

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<long> ids = (List<long>)dict["uids"];
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

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getusergroups");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

                if (callback == null) {

                    return;
                }

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<long> ids = (List<long>)dict["gids"];
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void EnterRoom(long rid, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("rid", rid);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("enterroom");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void LeaveRoom(long rid, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("rid", rid);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("leaveroom");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getuserrooms");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

                if (callback == null) {

                    return;
                }

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<long> ids = (List<long>)dict["rooms"];
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

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("uids", uids);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("getonlineusers");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);

            this.SendQuest(data, (cbd) => {

                if (callback == null) {

                    return;
                }

                Dictionary<string, object> dict = (Dictionary<string, object>)cbd.GetPayload();

                if (dict != null) {

                    List<long> ids = (List<long>)dict["uids"];
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void DeleteMessage(long mid, long xid, byte type, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("mid", mid);
            payload.Add("xid", xid);
            payload.Add("type", type);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("delmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void Kickout(string ce, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("ce", ce);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("kickout");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary(val:String)}  payload
         * </CallbackData>
         */
        public void DBGet(string key, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("key", key);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("dbget");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
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
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </CallbackData>
         */
        public void DBSet(string key, string value, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("key", key);
            payload.Add("val", value);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("dbset");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
        }

        /**
         *
         * rtmGate (1)
         *
         */
        private void Auth(int timeout) {

            RTMClient self = this;
            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("pid", this._pid);
            payload.Add("uid", this._uid);
            payload.Add("token", this._token);
            payload.Add("version", this._version);
            payload.Add("attrs", this._attrs);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("auth");
            data.SetPayload(bytes);

            this.SendQuest(data, (cbd) => {

                Exception exception = cbd.GetException();

                if (exception != null) {

                    self.GetEvent().FireEvent(new EventData("error", exception));
                    self.Reconnect();
                    return;
                }

                object obj = cbd.GetPayload();

                if (obj != null) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)obj;

                    bool ok = Convert.ToBoolean(dict["ok"]);

                    if (ok) {

                        self.GetEvent().FireEvent(new EventData("login", self._endpoint));
                        return;
                    }

                    if (dict.ContainsKey("gate")) {

                        string gate = Convert.ToString(dict["gate"]);

                        if (!string.IsNullOrEmpty(gate)) {

                            self._endpoint = gate;
                            self.Reconnect();
                            return;
                        }
                    }

                    if (!ok) {

                        self.GetEvent().FireEvent(new EventData("login", new Exception("token error!")));
                        return;
                    }
                }

                self.GetEvent().FireEvent(new EventData("error", new Exception("auth error!")));
            }, timeout);
        }

        private void ConnectRTMGate(int timeout) {

            if (this._baseClient != null) {

                this._baseClient.Destroy();
            }

            RTMClient self = this;

            this._baseClient = new BaseClient(this._endpoint, false, timeout, this._startTimerThread);

            this._baseClient.GetEvent().AddListener("connect", (evd) => {

                self.Auth(timeout);
            });

            this._baseClient.GetEvent().AddListener("close", (evd) => {

                self.GetEvent().FireEvent(new EventData("close", !self._isClose && self._reconnect));

                self._endpoint = null;
                self.Reconnect();
            });

            this._baseClient.GetEvent().AddListener("error", (evd) => {

                self.GetEvent().FireEvent(new EventData("error", evd.GetException()));
            });

            this._baseClient.GetProcessor().SetProcessor(this._processor);
            this._baseClient.EnableConnect();
        }

        private void FileSendProcess(Hashtable ops, long mid, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

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

                    Dictionary<string, object> dict = (Dictionary<string, object>)obj;

                    string token = Convert.ToString(dict["token"]);
                    string endpoint = Convert.ToString(dict["endpoint"]);

                    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(endpoint)) {

                        self.GetEvent().FireEvent(new EventData("error", new Exception("file token error!")));
                        return;
                    }

                    FileClient fileClient = new FileClient(endpoint, timeout, false);

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

                    fileClient.Send(Convert.ToString(ops["cmd"]), (byte[])ops["file"], token, dict, timeout, callback);
                }
            }, timeout);
        }

        private void Filetoken(Dictionary<string, object> payload, CallbackDelegate callback, int timeout) {

            MemoryStream msgpackStream = new MemoryStream();

            MsgPack.Serialize(payload, msgpackStream);
            msgpackStream.Position = 0; 
            
            byte[] bytes = msgpackStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod("filetoken");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);
        }

        private void Reconnect() {

            if (!this._reconnect) {

                return;
            }

            if (this._isClose) {

                return;
            }

            this.Login(this._endpoint, this._ipv6);
        }

        private class DispatchClient:BaseClient {

            public DispatchClient(string endpoint, int timeout, bool startTimerThread):base(endpoint, false, timeout, startTimerThread) {}
            public DispatchClient(string host, int port, int timeout, bool startTimerThread):base(host, port, false, timeout, startTimerThread) {}

        	public override void AddListener() {

                base.GetEvent().AddListener("connect", (evd) => {

                    Debug.Log("[DispatchClient] connected!");
                });

                base.GetEvent().AddListener("error", (evd) => {

                    Debug.Log(evd.GetException().Message);
                });
        	}

            public void Which(Dictionary<string, object> payload, int timeout, CallbackDelegate callback) {

                if (!base.HasConnect()) {

                    base.EnableConnect();
                }

                MemoryStream outputStream = new MemoryStream();

                MsgPack.Serialize(payload, outputStream);
                outputStream.Position = 0; 

                byte[] bytes = outputStream.ToArray();

                FPData data = new FPData();
                data.SetFlag(0x1);
                data.SetMtype(0x1);
                data.SetMethod("which");
                data.SetPayload(bytes);

                base.SendQuest(data, base.QuestCallback(callback), timeout);
        	}
        }

        private class FileClient:BaseClient {

            public FileClient(string endpoint, int timeout, bool startTimerThread):base(endpoint, false, timeout, startTimerThread) {}
            public FileClient(string host, int port, int timeout, bool startTimerThread):base(host, port, false, timeout, startTimerThread) {}

            public override void AddListener() {

                base.GetEvent().AddListener("connect", (evd) => {

                    Debug.Log("[FileClient] connected!");
                });

                base.GetEvent().AddListener("close", (evd) => {

                    Debug.Log("[FileClient] closed!");
                });

                base.GetEvent().AddListener("error", (evd) => {

                    Debug.Log(evd.GetException().Message);
                });
            }

            public void Send(string method, byte[] fileBytes, string token, Dictionary<string, object> payload, int timeout, CallbackDelegate callback) {

                String fileMd5 = base.CalcMd5(fileBytes, false);
                String sign = base.CalcMd5(fileMd5 + ":" + token, false);

                if (string.IsNullOrEmpty(sign)) {

                    base.GetEvent().FireEvent(new EventData("error", new Exception("wrong sign!")));
                    return;
                }

                if (!base.HasConnect()) {

                    base.EnableConnect();
                }

                Dictionary<string, string> attrs = new Dictionary<string, string>();
                attrs.Add("sign", sign);

                MemoryStream jsonStream = new MemoryStream();
                Json.Serialize(attrs, jsonStream);

                string jsonStr = System.Text.Encoding.UTF8.GetString(jsonStream.ToArray());

                payload.Add("token", token);
                payload.Add("file", fileBytes);
                payload.Add("attrs", jsonStr);

                long mid = (long)Convert.ToInt64(payload["mid"]);

                MemoryStream msgpackStream = new MemoryStream();

                MsgPack.Serialize(payload, msgpackStream);
                msgpackStream.Position = 0; 

                byte[] bytes = msgpackStream.ToArray();

                FPData data = new FPData();
                data.SetFlag(0x1);
                data.SetMtype(0x1);
                data.SetMethod(method);
                data.SetPayload(bytes);

                FileClient self = this;

                base.SendQuest(data, base.QuestCallback((cbd) => {

                    cbd.SetMid(mid);
                    self.Destroy();

                    if (callback != null) {

                        callback(cbd);
                    }
                }), timeout);
            }
        }

        private class BaseClient:FPClient {

            public BaseClient(string endpoint, bool reconnect, int timeout, bool startTimerThread):base(endpoint, reconnect, timeout) {

                if (startTimerThread) {

                    ThreadPool.Instance.StartTimerThread();
                }

                this.AddListener();
            }

            public BaseClient(string host, int port, bool reconnect, int timeout, bool startTimerThread):base(host, port, reconnect, timeout) {

                if (startTimerThread) {

                    ThreadPool.Instance.StartTimerThread();
                }

                this.AddListener();
            }

        	public virtual void AddListener() {}

            public void EnableConnect() {

                base.Connect();
            }

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
                Dictionary<string, object> payload = null;

                if (data != null) {

                	if (data.GetFlag() == 0) {

                		payload = Json.Deserialize<Dictionary<string, object>>(data.JsonPayload());
                	}

                	if (data.GetFlag() == 1) {

                        MemoryStream inputStream = new MemoryStream(data.MsgpackPayload());
		                payload = MsgPack.Deserialize<Dictionary<string, object>>(inputStream);
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
    }
}