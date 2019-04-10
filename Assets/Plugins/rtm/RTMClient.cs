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

    public delegate void CallbackDelegate(Hashtable ht);

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

                    return Convert.ToInt64(Convert.ToString(FPCommon.GetMilliTimestamp()) + strFix);
                }
            }
        }

        public ConnectedCallbackDelegate ConnectedCallback { 

            get { 

                return this._onConnect; 
            }

            set {

                this._onConnect = value;
            }
        }

        public ClosedCallbackDelegate ClosedCallback { 

            get { 

                return this._onClose; 
            }

            set {

                this._onClose = value;
            }
        }

        public ErrorCallbackDelegate ErrorCallback { 

            get { 

                return this._onError; 
            }

            set {

                this._onError = value;
            }
        }

        private string _dispatch;
        private int _pid;
        private long _uid;
        private string _token;
        private string _version;
        private Dictionary<String, String> _attrs;
        private bool _reconnect;
        private int _timeout;

        private bool _ipv6;
        private bool _isClose;
        private string _endpoint;

        private RTMProcessor _psr;

        private BaseClient _baseClient;
        private DispatchClient _dispatchClient;

        private ConnectedCallbackDelegate _onConnect;
        private ClosedCallbackDelegate _onClose;
        private ErrorCallbackDelegate _onError;


        /**
         * @param {string}                      dispatch
         * @param {int}                         pid
         * @param {long}                        uid
         * @param {string}                      token
         * @param {string}                      version
         * @param {Dictionary(string,string)}   attrs
         * @param {bool}                        reconnect
         * @param {int}                         timeout
         */
        public RTMClient(string dispatch, int pid, long uid, string token, string version, Dictionary<string, string> attrs, bool reconnect, int timeout) {

            this._dispatch = dispatch;
            this._pid = pid;
            this._uid = uid;
            this._token = token;
            this._version = version;
            this._attrs = attrs;
            this._reconnect = reconnect;
            this._timeout = timeout;

            this.InitProcessor();
        }

        private void InitProcessor() {

            RTMClient self = this;

            this._psr = new RTMProcessor();
            this._psr.AddPushListener(RTMConfig.SERVER_PUSH.kickOut, (data) => {

                self._isClose = true;
                self._baseClient.Close();
            });
        }

        public RTMProcessor GetProcessor() {

            return this._psr;
        }

        public void SendQuest(FPData data, CallbackDelegate callback, int timeout) {

            if (this._baseClient != null) {

                this._baseClient.SendQuest(data, this._baseClient.QuestCallback(callback), timeout);
            }
        }

        public void Destroy() {

            this.Close();

            if (this._baseClient != null) {

                this._baseClient.Close();
                this._baseClient = null;
            }

            if (this._dispatchClient != null) {

                this._dispatchClient.Close();
                this._dispatchClient = null;
            }
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

                this._dispatchClient = new DispatchClient(this._dispatch, false, this._timeout);

                this._dispatchClient.ClosedCallback = delegate() {

                    Debug.Log("[DispatchClient] closed!");

                    if (self._dispatchClient != null) {

                        self._dispatchClient.Close();
                        self._dispatchClient = null;
                    }

                    if (string.IsNullOrEmpty(self._endpoint)) {

                        self._dispatchClient.ErrorCallback(new Exception("dispatch client close with err!"));
                        self.Reconnect();
                    }
                };
            }

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("pid", this._pid);
            payload.Add("uid", this._uid);
            payload.Add("what", "rtmGated");
            payload.Add("addrType", this._ipv6 ? "ipv6" : "ipv4");
            payload.Add("version", this._version);

            this._dispatchClient.Which(payload, this._timeout, (Hashtable ht) => {

                if (ht.Contains("exception")) {

                    Exception ex = (Exception)ht["exception"];
                    self._dispatchClient.ErrorCallback(ex);
                    return;
                }

                if (ht.Contains("payload")) {

                    Dictionary<string, object> data = (Dictionary<string, object>)ht["payload"];
                    self.Login(Convert.ToString(data["endpoint"]), self._ipv6);
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
         * @param {Hashtable}               ht 
         *
         * <Hashtable>
         * @param {Dictionary(mtime:long)}  payload 
         * @param {Exception}               exception
         * @param {long}                    mid
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("sendmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (Hashtable ht) => {

                ht.Add("mid", mid);

                if (callback != null) {

                    callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary(mtime:long)}  payload 
         * @param {Exception}               exception
         * @param {long}                    mid
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("sendgroupmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (Hashtable ht) => {

                ht.Add("mid", mid);

                if (callback != null) {

                    callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary(mtime:long)}  payload 
         * @param {Exception}               exception
         * @param {long}                    mid
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("sendroommsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (Hashtable ht) => {

                ht.Add("mid", mid);

                if (callback != null) {

                    callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {long}                    mid
         * @param {Exception}               exception
         * @param {Dictionary(p2p:Dictionary(String,int),group:Dictionary(String,int))} payload
         * </Hashtable>
         */
        public void GetUnreadMessage(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * @param {long}                    mid
         * </Hashtable>
         */
        public void CleanUnreadMessage(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {long}                    mid
         * @param {Exception}               exception
         * @param {Dictionary(p2p:Map(String,long),Dictionary:Map(String,long))}    payload
         * </Hashtable>
         */
        public void GetSession(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Exception}               exception
         * @param {Dictionary(num:int,lastid:long,begin:long,end:long,msgs:List(GroupMsg))} payload
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("getgroupmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (Hashtable ht) => {

                if (callback == null) {

                    return;
                }

                if (ht.Contains("payload")) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];

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

                callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Exception}               exception
         * @param {Dictionary(num:int,lastid:long,begin:long,end:long,msgs:List(RoomMsg))} payload
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("getroommsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (Hashtable ht) => {

                if (callback == null) {

                    return;
                }

                if (ht.Contains("payload")) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];

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

                callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Exception}               exception
         * @param {Dictionary(num:int,lastid:long,begin:long,end:long,msgs:List(BroadcastMsg))} payload
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("getbroadcastmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (Hashtable ht) => {

                if (callback == null) {

                    return;
                }

                if (ht.Contains("payload")) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];

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

                callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Exception}               exception
         * @param {Dictionary(num:int,lastid:long,begin:long,end:long,msgs:List(P2PMsg))} payload
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("getp2pmsg");
            data.SetPayload(bytes);

            this.SendQuest(data, (Hashtable ht) => {

                if (callback == null) {

                    return;
                }

                if (ht.Contains("payload")) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];

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

                callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Exception}               exception
         * @param {Dictionary(token:string,endpoint:string)}   payload
         * </Hashtable>
         */
        public void FileToken(string cmd, List<long> tos, long to, long rid, long gid, int timeout, CallbackDelegate callback) {

            //TODO
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("bye");
            data.SetPayload(bytes);

            RTMClient self = this;

            this.SendQuest(data, (Hashtable ht) => {

                self._baseClient.Close();
            }, 5 * 1000);
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
         * @param {Hashtable}                       ht
         *
         * <Hashtable>
         * @param {Exception}                       exception
         * @param {Dictionary}                      payload
         * </Hashtable>
         */
        public void AddAttrs(Dictionary<string, string> attrs, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("attrs", attrs);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Exception}               exception
         * @param {Dictionary(attrs:List(Hashtable))}    payload
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void RemoveDevice(string devicetoken, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("devicetoken", devicetoken);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void SetTranslationLanguage(string targetLanguage, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("lang", targetLanguage);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Exception}               exception
         * @param {Dictionary(stext:string,src:string,dtext:string,dst:string)}    payload
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void AddFriends(List<long> friends, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("friends", friends);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void DeleteFriends(List<long> friends, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("friends", friends);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Exception}               exception
         * @param {List(long)}              payload
         * </Hashtable>
         */
        public void GetFriends(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("getfriends");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);

            this.SendQuest(data, (Hashtable ht) => {

                if (callback == null) {

                    return;
                }

                List<long> ids = new List<long>();

                if (ht.Contains("payload")) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];
                    ids = (List<long>)dict["uids"];
                }

                ht["payload"] = ids;
                callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {List(long)}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void GetGroupMembers(long gid, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("gid", gid);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("getgroupmembers");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);

            this.SendQuest(data, (Hashtable ht) => {

                if (callback == null) {

                    return;
                }

                List<long> ids = new List<long>();

                if (ht.Contains("payload")) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];
                    ids = (List<long>)dict["uids"];
                }

                ht["payload"] = ids;
                callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {List(long)}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void GetUserGroups(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("getusergroups");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);

            this.SendQuest(data, (Hashtable ht) => {

                if (callback == null) {

                    return;
                }

                List<long> ids = new List<long>();

                if (ht.Contains("payload")) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];
                    ids = (List<long>)dict["gids"];
                }

                ht["payload"] = ids;
                callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void EnterRoom(long rid, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("rid", rid);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void LeaveRoom(long rid, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("rid", rid);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {List(long)}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void GetUserRooms(int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("getuserrooms");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);

            this.SendQuest(data, (Hashtable ht) => {

                if (callback == null) {

                    return;
                }

                List<long> ids = new List<long>();

                if (ht.Contains("payload")) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];
                    ids = (List<long>)dict["rooms"];
                }

                ht["payload"] = ids;
                callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {List(long)}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void GetOnlineUsers(List<long> uids, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("uids", uids);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("getonlineusers");
            data.SetPayload(bytes);

            this.SendQuest(data, callback, timeout);

            this.SendQuest(data, (Hashtable ht) => {

                if (callback == null) {

                    return;
                }

                List<long> ids = new List<long>();

                if (ht.Contains("payload")) {

                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];
                    ids = (List<long>)dict["uids"];
                }

                ht["payload"] = ids;
                callback(ht);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
         */
        public void Kickout(string ce, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("ce", ce);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Exception}               exception
         * @param {Dictionary(val:String)}  payload
         * </Hashtable>
         */
        public void DBGet(string key, int timeout, CallbackDelegate callback) {

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("key", key);

            MemoryStream outputStream = new MemoryStream();

            MsgPack.Serialize(payload, outputStream);
            outputStream.Position = 0; 

            byte[] bytes = outputStream.ToArray();

            FPData data = new FPData();
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
         * @param {Hashtable}               ht
         *
         * <Hashtable>
         * @param {Dictionary}              payload
         * @param {Exception}               exception
         * </Hashtable>
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
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
            data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
            data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
            data.SetMethod("auth");
            data.SetPayload(bytes);

            this.SendQuest(data, (Hashtable ht) => {

                if (ht.Contains("exception")) {

                    Exception ex = (Exception)ht["exception"];

                    if (self._onError != null) {

                        self._onError(ex);
                    }

                    self.Reconnect();
                    return;
                }

                if (ht.Contains("payload")) {

                    bool ok = false;
                    Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];

                    if (dict.ContainsKey("ok")) {

                        ok = Convert.ToBoolean(dict["ok"]);

                        if (ok) {

                            if (self._onConnect != null) {

                                self._onConnect();
                            }

                            return;
                        }
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

                        if (self._onError != null) {

                            self._onError(new Exception("token error!"));
                        }

                        return;
                    }
                }

                if (self._onError != null) {

                    self._onError(new Exception("auth error!"));
                }
            }, timeout);
        }

        private void ConnectRTMGate(int timeout) {

            if (this._baseClient != null) {

                this._baseClient.Close();
            }

            this._baseClient = new BaseClient(this._endpoint, false, timeout);
            this._baseClient.Processor = this._psr.Processor;

            RTMClient self = this;

            this._baseClient.ErrorCallback = this._onError;
            this._baseClient.ConnectedCallback = delegate() {

                self.Auth(timeout);
            };
            this._baseClient.ClosedCallback = delegate() {

                if (this._onClose != null) {

                    this._onClose();
                }

                self._endpoint = null;
                self.Reconnect();
            };

            this._baseClient.Connect();
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

			public DispatchClient(string hostport):base(hostport){}
        	public DispatchClient(string hostport, bool autoReconnect):base(hostport, autoReconnect) {}
        	public DispatchClient(string hostport, bool autoReconnect, int connectionTimeout):base(hostport, autoReconnect, connectionTimeout) {}
        	public DispatchClient(string host, int port, bool autoReconnect = true, int connectionTimeout = 5000):base(host, port, autoReconnect, connectionTimeout) {}

        	public override void AddListener() {

        		this.ErrorCallback = delegate(Exception e) {

                    Debug.Log(e.Message);
                };

                this.ConnectedCallback = delegate() {

                    Debug.Log("[DispatchClient] connected!");
                };
        	}

            public void Which(Dictionary<string, object> payload, int timeout, CallbackDelegate callback) {

                if (!base.HasConnect()) {

                    base.Connect();
                }

                MemoryStream outputStream = new MemoryStream();

                MsgPack.Serialize(payload, outputStream);
                outputStream.Position = 0; 

                byte[] bytes = outputStream.ToArray();

                FPData data = new FPData();
                data.SetFlag(FP_FLAG.FP_FLAG_MSGPACK);
                data.SetMType(FP_MSG_TYPE.FP_MT_TWOWAY);
                data.SetMethod("which");
                data.SetPayload(bytes);

                base.SendQuest(data, base.QuestCallback(callback), timeout);
        	}
        }

        private class BaseClient:FPClient {

        	public BaseClient(string hostport):base(hostport) {

        		this.AddListener();
        	}

        	public BaseClient(string hostport, bool autoReconnect):base(hostport, autoReconnect) {

        		this.AddListener();
        	}

        	public BaseClient(string hostport, bool autoReconnect, int connectionTimeout):base(hostport, autoReconnect, connectionTimeout) {

        		this.AddListener();
        	}

        	public BaseClient(string host, int port, bool autoReconnect = true, int connectionTimeout = 5000):base(host, port, autoReconnect, connectionTimeout) {

        		this.AddListener();
        	}

        	public virtual void AddListener() {}

            protected virtual void OnSecond(int timestamp) {

            }

        	private Exception CheckException(Dictionary<string, object> dict) {

	            if (dict.ContainsKey("ex") && dict.ContainsKey("code")) {

	                int errorCode = Convert.ToInt32(dict["code"]);
	                string errorMsg = Convert.ToString(errorCode) + " : " + dict["ex"];

	                return new Exception(errorMsg);
	            }

	            return null;
	        }

        	private Hashtable CheckFPCallback(CallbackData cbd) {

        		bool isAnswerException = false;
                Hashtable result = new Hashtable();

        		if (cbd.Exception != null) {

        			result.Add("exception", cbd.Exception);
        			return result;
                }

                FPData data = cbd.Data;
                Dictionary<string, object> payload = null;

                if (data.payload != null) {

            		MemoryStream inputStream = new MemoryStream(data.payload);

                	if ((data.flag & Convert.ToByte(FP_FLAG.FP_FLAG_JSON)) != 0) {

                		payload = Json.Deserialize<Dictionary<string, object>>(inputStream);
                	}

                	if ((data.flag & Convert.ToByte(FP_FLAG.FP_FLAG_MSGPACK)) != 0) {

		                payload = MsgPack.Deserialize<Dictionary<string, object>>(inputStream);
                	}

                	if (data.IsAnswer()) {

                		isAnswerException = data.ss != 0;
                	}
                }

                if (isAnswerException) {

                	cbd.Exception = CheckException(payload);
                    result.Add("exception", cbd.Exception);
                    return result;
                }
                	
            	result.Add("payload", payload);
                return result;
        	}

        	public FPCallback QuestCallback(CallbackDelegate callback) {

        		BaseClient self = this;

        		return (CallbackData cbd) => {

        			if (callback == null) {

        				return;
        			}

    				callback(self.CheckFPCallback(cbd));
	            };
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
        }
    }
}