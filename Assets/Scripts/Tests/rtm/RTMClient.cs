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

    public static class RTMRegistration {

        static public void Register() {
            FPManager.Instance.Init();
        }
    }

    public class RTMClient {

        private static class MidGenerator {

            static private long Count = 0;
            static private StringBuilder sb = new StringBuilder(20);
            static private object lock_obj = new object();

            static public long Gen() {
                lock (lock_obj) {
                    if (++Count > 999) {
                        Count = 1;
                    }

                    long c = Count;
                    //.Net >= 4.0  sb.Clear();
                    sb.Length = 0;
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
        private bool _debug = true;

        private bool _isClose;
        private string _endpoint;
        private string _switchGate;
        private string _targetLanguage;

        private RTMSender _sender;
        private RTMProcessor _processor;

        private BaseClient _baseClient;
        private DispatchClient _dispatchClient;

        /**
         * @param {string}                      dispatch
         * @param {int}                         pid
         * @param {long}                        uid
         * @param {string}                      token
         * @param {string}                      version
         * @param {string}                      lang
         * @param {IDictionary(string,string)}  attrs
         * @param {bool}                        reconnect
         * @param {int}                         timeout
         * @param {bool}                        debug
         */
        public RTMClient(string dispatch, int pid, long uid, string token, string version, string lang, IDictionary<string, string> attrs, bool reconnect, int timeout, bool debug) {
            Debug.Log("[RTM] rtm_sdk@" + RTMConfig.VERSION + ", fpnn_sdk@" + FPConfig.VERSION);
            this._dispatch = dispatch;
            this._pid = pid;
            this._uid = uid;
            this._token = token;
            this._version = version;
            this._targetLanguage = lang;
            this._reconnect = reconnect;
            this._timeout = timeout;
            this._debug = debug;

            if (attrs == null) {
                this._attrs = new Dictionary<string, string>();
            } else {
                this._attrs = new Dictionary<string, string>(attrs);
            }

            this.InitProcessor();
        }

        private void InitProcessor() {
            RTMClient self = this;
            this._sender = new RTMSender();
            this._processor = new RTMProcessor();
            this._processor.AddPushService(RTMConfig.KICKOUT, OnKickout);
            FPManager.Instance.AddSecond(OnSecondDelegate);
            ErrorRecorderHolder.setInstance(new RTMErrorRecorder(this._debug));
        }

        private void OnKickout(IDictionary<string, object> data) {
            lock (self_locker) {
                this._isClose = true;

                if (this._baseClient != null) {
                    this._baseClient.Close();
                }
            }
        }

        private void OnSecondDelegate(EventData evd) {
            long lastPingTimestamp = 0;
            long timestamp = evd.GetTimestamp();

            lock (self_locker) {
                if (this._processor != null) {
                    lastPingTimestamp = this._processor.GetPingTimestamp();
                }
            }

            if (lastPingTimestamp > 0 && timestamp - lastPingTimestamp > RTMConfig.RECV_PING_TIMEOUT) {
                lock (self_locker) {
                    if (this._baseClient != null && this._baseClient.IsOpen()) {
                        this._baseClient.Close(new Exception("ping timeout"));
                    }
                }
            }

            this.DelayConnect(timestamp);
        }

        public RTMProcessor GetProcessor() {
            lock (self_locker) {
                return this._processor;
            }
        }

        public FPPackage GetPackage() {
            lock (self_locker) {
                if (this._baseClient != null) {
                    return this._baseClient.GetPackage();
                }
            }

            return null;
        }

        public void SendQuest(string method, IDictionary<string, object> payload, CallbackDelegate callback, int timeout) {
            FPData data = new FPData();
            data.SetFlag(0x1);
            data.SetMtype(0x1);
            data.SetMethod(method);

            lock (self_locker) {
                if (this._sender != null && this._baseClient != null) {
                    BaseClient client = this._baseClient;
                    this._sender.AddQuest(client, data, payload, client.QuestCallback(callback), timeout);
                }
            }
        }

        private object self_locker = new object();

        public void Destroy() {
            lock (delayconn_locker) {
                delayconn_locker.Status = 0;
                this._reconnCount = 0;
                this._lastConnectTime = 0;
            }

            lock (self_locker) {
                this._isClose = true;
                FPManager.Instance.RemoveSecond(OnSecondDelegate);
                this._event.FireEvent(new EventData("close", false));
                this._event.RemoveListener();

                if (this._sender != null) {
                    this._sender.Destroy();
                    this._sender = null;
                }

                if (this._processor != null) {
                    this._processor.Destroy();
                    this._processor = null;
                }

                if (this._dispatchClient != null) {
                    this._dispatchClient.Close();
                    this._dispatchClient = null;
                }

                if (this._baseClient != null) {
                    this._baseClient.Close();
                    this._baseClient = null;
                }
            }
        }

        /**
         * @param {string}  endpoint
         */
        public void Login(string endpoint) {
            lock (self_locker) {
                this._isClose = false;
                this._endpoint = endpoint;

                if (string.IsNullOrEmpty(this._endpoint)) {
                    this.ConnDispatchClient();
                    return;
                }

                this.ConnBaseClient();
            }
        }

        private void ConnDispatchClient() {
            if (this._dispatchClient == null) {
                this._dispatchClient = new DispatchClient(this._dispatch, this._timeout);
                this._dispatchClient.Client_Close = DispatchClient_OnClose;
                this._dispatchClient.Client_Connect = DispatchClient_OnConnect;
                this._dispatchClient.Connect();
            }
        }

        private void DispatchClient_OnClose(EventData evd) {
            bool reconn = false;

            lock (self_locker) {
                if (this._dispatchClient != null) {
                    this._dispatchClient = null;
                }

                reconn = string.IsNullOrEmpty(this._endpoint);
            }

            if (reconn) {
                this.Reconnect();
            }
        }

        private void DispatchClient_OnConnect(EventData evd) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "pid", this._pid },
                { "uid", this._uid },
                { "what", "rtmGated" },
                { "addrType", "ipv4" },
                { "version", this._version }
            };

            lock (self_locker) {
                if (this._dispatchClient == null) {
                    return;
                }

                payload["addrType"] = this._dispatchClient.IsIPv6() ? "ipv6" : "ipv4";
                this._dispatchClient.Which(this._sender, payload, this._timeout, DispatchClient_Which_OnCallback);
            }
        }

        private void ConnBaseClient() {
            if (this._baseClient == null) {
                this._baseClient = new BaseClient(this._endpoint, this._timeout);
                this._baseClient.Client_Close = BaseClient_Close;
                this._baseClient.Client_Connect = BaseClient_Connect;
                this._baseClient.GetProcessor().SetProcessor(this._processor);
                this._baseClient.Connect();
            }
        }

        private void BaseClient_Connect(EventData evd) {
            this.Auth();
        }

        private void BaseClient_Close(EventData evd) {
            bool retry = false;

            lock (self_locker) {
                if (this._baseClient != null) {
                    this._baseClient = null;
                }

                if (!string.IsNullOrEmpty(this._switchGate)) {
                    this._endpoint = this._switchGate;
                    this._switchGate = null;
                }

                retry = !this._isClose && this._reconnect;
            }

            this.GetEvent().FireEvent(new EventData("close", retry));
            this.Reconnect();
        }

        private void DispatchClient_Which_OnCallback(CallbackData cbd) {
            string ep = null;
            IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

            if (dict != null && dict.ContainsKey("endpoint")) {
                ep = Convert.ToString(dict["endpoint"]);
            }

            lock (self_locker) {
                if (this._dispatchClient != null) {
                    this._dispatchClient.Close(cbd.GetException());
                }
            }

            this.Login(ep);
        }

        /**
         *
         * rtmGate (1a)
         *
         */
        private void Auth() {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "pid", this._pid },
                { "uid", this._uid },
                { "token", this._token },
                { "version", this._version },
                { "attrs", this._attrs }
            };

            lock (self_locker) {
                if (!string.IsNullOrEmpty(this._targetLanguage)) {
                    payload.Add("lang", this._targetLanguage);
                }
            }

            this.SendQuest("auth", payload, Auth_OnCallback, this._timeout);
        }

        private void Auth_OnCallback(CallbackData cbd) {
            Exception exception = cbd.GetException();

            if (exception != null) {
                lock (self_locker) {
                    if (this._baseClient != null) {
                        this._baseClient.Close(exception);
                    }
                }

                return;
            }

            object obj = cbd.GetPayload();

            if (obj != null) {
                IDictionary<string, object> dict = (IDictionary<string, object>)obj;

                if (dict.ContainsKey("ok")) {
                    bool ok = Convert.ToBoolean(dict["ok"]);

                    if (ok) {
                        lock (delayconn_locker) {
                            this._reconnCount = 0;
                        }

                        string ep = null;

                        lock (self_locker) {
                            if (this._processor != null) {
                                this._processor.InitPingTimestamp();
                            }

                            ep = this._endpoint;
                        }

                        this.GetEvent().FireEvent(new EventData("login", ep));
                        return;
                    }
                }

                if (dict.ContainsKey("gate")) {
                    string gate = Convert.ToString(dict["gate"]);

                    if (!string.IsNullOrEmpty(gate)) {
                        lock (self_locker) {
                            this._switchGate = gate;

                            if (this._baseClient != null) {
                                this._baseClient.Close();
                            }
                        }

                        return;
                    }
                }

                lock (self_locker) {
                    this._isClose = true;
                }

                this.GetEvent().FireEvent(new EventData("login", new Exception("token error!")));
                return;
            }

            lock (self_locker) {
                if (this._baseClient != null) {
                    this._baseClient.Close(new Exception("auth error!"));
                }
            }
        }

        /**
         * rtmGate (1b)
         */
        public void Close() {
            BaseClient client;

            lock (self_locker) {
                this._isClose = true;
                client = this._baseClient;
            }

            this.SendQuest("bye", new Dictionary<string, object>(), (cbd) => {
                if (client != null) {
                    client.Close();
                }
            }, 500);

            lock (self_locker) {
                this._baseClient = null;
            }
        }

        /**
         *
         * rtmGate (1c)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "ce", ce
                }
            };
            this.SendQuest("kickout", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (1d)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "attrs", attrs
                }
            };
            this.SendQuest("addattrs", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (1e)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {List(IDictionary<string, string>)}    payload
         * </CallbackData>
         *
         * <IDictionary<string, string>>
         * @param {string}                  ce
         * @param {string}                  login
         * @param {string}                  my
         * </IDictionary<string, string>>
         */
        public void GetAttrs(int timeout, CallbackDelegate callback) {
            this.SendQuest("getattrs", new Dictionary<string, object>(), (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("attrs")) {
                    callback(new CallbackData(dict["attrs"]));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (1f)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "msg", msg },
                { "attrs", attrs }
            };
            this.SendQuest("adddebuglog", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (1g)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "apptype", apptype },
                { "devicetoken", devicetoken }
            };
            this.SendQuest("adddevice", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (1h)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "devicetoken", devicetoken
                }
            };
            this.SendQuest("removedevice", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (2a)
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
        public void SendMessage(long to, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback) {
            if (mid == 0) {
                mid = MidGenerator.Gen();
            }

            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "to", to },
                { "mid", mid },
                { "mtype", mtype },
                { "msg", msg },
                { "attrs", attrs }
            };
            this.SendQuest("sendmsg", payload, (cbd) => {
                cbd.SetMid(mid);

                if (callback != null) {
                    callback(cbd);
                }
            }, timeout);
        }

        /**
         *
         * rtmGate (2b)
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

            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "gid", gid },
                { "mid", mid },
                { "mtype", mtype },
                { "msg", msg },
                { "attrs", attrs }
            };
            this.SendQuest("sendgroupmsg", payload, (cbd) => {
                cbd.SetMid(mid);

                if (callback != null) {
                    callback(cbd);
                }
            }, timeout);
        }

        /**
         *
         * rtmGate (2c)
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

            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "rid", rid },
                { "mid", mid },
                { "mtype", mtype },
                { "msg", msg },
                { "attrs", attrs }
            };
            this.SendQuest("sendroommsg", payload, (cbd) => {
                cbd.SetMid(mid);

                if (callback != null) {
                    callback(cbd);
                }
            }, timeout);
        }

        /**
         *
         * rtmGate (2d)
         *
         * @param {long}                    gid
         * @param {bool}                    desc
         * @param {int}                     num
         * @param {long}                    begin
         * @param {long}                    end
         * @param {long}                    lastid
         * @param {List<Byte>}              mtypes
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
        public void GetGroupMessage(long gid, bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "gid", gid },
                { "desc", desc },
                { "num", num }
            };

            if (begin > 0) {
                payload.Add("begin", begin);
            }

            if (end > 0) {
                payload.Add("end", end);
            }

            if (lastid > 0) {
                payload.Add("lastid", lastid);
            }

            if (mtypes != null && mtypes.Count > 0) {
                payload.Add("mtypes", mtypes);
            }

            this.SendQuest("getgroupmsg", payload, (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("msgs")) {
                    List<object> ol = (List<object>)dict["msgs"];
                    List<IDictionary<string, object>> nl = new List<IDictionary<string, object>>();

                    foreach (List<object> items in ol) {
                        IDictionary<string, object> GroupMsg = new Dictionary<string, object>() {
                            { "id", items[0] },
                            { "from", items[1] },
                            { "mtype", items[2] },
                            { "mid", items[3] },
                            { "deleted", items[4] },
                            { "msg", items[5] },
                            { "attrs", items[6] },
                            { "mtime", items[7] }
                        };
                        byte mtype = Convert.ToByte(GroupMsg["mtype"]);

                        if (mtype == 30) {
                            GroupMsg.Remove("mtype");
                        }

                        nl.Add(GroupMsg);
                    }

                    dict["msgs"] = nl;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (2e)
         *
         * @param {long}                    rid
         * @param {bool}                    desc
         * @param {int}                     num
         * @param {long}                    begin
         * @param {long}                    end
         * @param {long}                    lastid
         * @param {List<Byte>}              mtypes
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
        public void GetRoomMessage(long rid, bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "rid", rid },
                { "desc", desc },
                { "num", num }
            };

            if (begin > 0) {
                payload.Add("begin", begin);
            }

            if (end > 0) {
                payload.Add("end", end);
            }

            if (lastid > 0) {
                payload.Add("lastid", lastid);
            }

            if (mtypes != null && mtypes.Count > 0) {
                payload.Add("mtypes", mtypes);
            }

            this.SendQuest("getroommsg", payload, (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("msgs")) {
                    List<object> ol = (List<object>)dict["msgs"];
                    List<IDictionary<string, object>> nl = new List<IDictionary<string, object>>();

                    foreach (List<object> items in ol) {
                        IDictionary<string, object> RoomMsg = new Dictionary<string, object>() {
                            { "id", items[0] },
                            { "from", items[1] },
                            { "mtype", items[2] },
                            { "mid", items[3] },
                            { "deleted", items[4] },
                            { "msg", items[5] },
                            { "attrs", items[6] },
                            { "mtime", items[7] }
                        };
                        byte mtype = Convert.ToByte(RoomMsg["mtype"]);

                        if (mtype == 30) {
                            RoomMsg.Remove("mtype");
                        }

                        nl.Add(RoomMsg);
                    }

                    dict["msgs"] = nl;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (2f)
         *
         * @param {bool}                    desc
         * @param {int}                     num
         * @param {long}                    begin
         * @param {long}                    end
         * @param {long}                    lastid
         * @param {List<Byte>}              mtypes
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
        public void GetBroadcastMessage(bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "desc", desc },
                { "num", num }
            };

            if (begin > 0) {
                payload.Add("begin", begin);
            }

            if (end > 0) {
                payload.Add("end", end);
            }

            if (lastid > 0) {
                payload.Add("lastid", lastid);
            }

            if (mtypes != null && mtypes.Count > 0) {
                payload.Add("mtypes", mtypes);
            }

            this.SendQuest("getbroadcastmsg", payload, (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("msgs")) {
                    List<object> ol = (List<object>)dict["msgs"];
                    List<IDictionary<string, object>> nl = new List<IDictionary<string, object>>();

                    foreach (List<object> items in ol) {
                        IDictionary<string, object> BroadcastMsg = new Dictionary<string, object>() {
                            { "id", items[0] },
                            { "from", items[1] },
                            { "mtype", items[2] },
                            { "mid", items[3] },
                            { "deleted", items[4] },
                            { "msg", items[5] },
                            { "attrs", items[6] },
                            { "mtime", items[7] }
                        };
                        byte mtype = Convert.ToByte(BroadcastMsg["mtype"]);

                        if (mtype == 30) {
                            BroadcastMsg.Remove("mtype");
                        }

                        nl.Add(BroadcastMsg);
                    }

                    dict["msgs"] = nl;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (2g)
         *
         * @param {long}                    ouid
         * @param {bool}                    desc
         * @param {int}                     num
         * @param {long}                    begin
         * @param {long}                    end
         * @param {long}                    lastid
         * @param {List<Byte>}              mtypes
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
        public void GetP2PMessage(long ouid, bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "ouid", ouid },
                { "desc", desc },
                { "num", num }
            };

            if (begin > 0) {
                payload.Add("begin", begin);
            }

            if (end > 0) {
                payload.Add("end", end);
            }

            if (lastid > 0) {
                payload.Add("lastid", lastid);
            }

            if (mtypes != null && mtypes.Count > 0) {
                payload.Add("mtypes", mtypes);
            }

            this.SendQuest("getp2pmsg", payload, (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("msgs")) {
                    List<object> ol = (List<object>)dict["msgs"];
                    List<IDictionary<string, object>> nl = new List<IDictionary<string, object>>();

                    foreach (List<object> items in ol) {
                        IDictionary<string, object> P2PMsg = new Dictionary<string, object>() {
                            { "id", items[0] },
                            { "direction", items[1] },
                            { "mtype", items[2] },
                            { "mid", items[3] },
                            { "deleted", items[4] },
                            { "msg", items[5] },
                            { "attrs", items[6] },
                            { "mtime", items[7] }
                        };
                        byte mtype = Convert.ToByte(P2PMsg["mtype"]);

                        if (mtype == 30) {
                            P2PMsg.Remove("mtype");
                        }

                        nl.Add(P2PMsg);
                    }

                    dict["msgs"] = nl;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (2h)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "mid", mid },
                { "xid", xid },
                { "type", type }
            };
            this.SendQuest("delmsg", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (3a)
         *
         * @param {long}                    to
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
        public void SendChat(long to, string msg, string attrs, long mid, int timeout, CallbackDelegate callback) {
            this.SendMessage(to, (byte) 30, msg, attrs, mid, timeout, callback);
        }

        /**
         *
         * rtmGate (3b)
         *
         * @param {long}                    gid
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
        public void SendGroupChat(long gid, string msg, string attrs, long mid, int timeout, CallbackDelegate callback) {
            this.SendGroupMessage(gid, (byte) 30, msg, attrs, mid, timeout, callback);
        }

        /**
         *
         * rtmGate (3c)
         *
         * @param {long}                    rid
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
        public void SendRoomChat(long rid, string msg, string attrs, long mid, int timeout, CallbackDelegate callback) {
            this.SendRoomMessage(rid, (byte) 30, msg, attrs, mid, timeout, callback);
        }

        /**
         *
         * rtmGate (3d)
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
        public void GetGroupChat(long gid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback) {
            this.GetGroupMessage(gid, desc, num, begin, end, lastid, new List<Byte> { (byte) 30 }, timeout, callback);
        }

        /**
         *
         * rtmGate (3e)
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
        public void GetRoomChat(long rid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback) {
            this.GetRoomMessage(rid, desc, num, begin, end, lastid, new List<Byte> { (byte) 30 }, timeout, callback);
        }

        /**
         *
         * rtmGate (3f)
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
        public void GetBroadcastChat(bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback) {
            this.GetBroadcastMessage(desc, num, begin, end, lastid, new List<Byte> { (byte) 30 }, timeout, callback);
        }

        /**
         *
         * rtmGate (3g)
         *
         * @param {long}                    ouid
         * @param {bool}                    desc
         * @param {int}                     num
         * @param {long}                    begin
         * @param {long}                    end
         * @param {long}                    lastid
         * @param {byte[]}                  mtypes
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
        public void GetP2PChat(long ouid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback) {
            this.GetP2PMessage(ouid, desc, num, begin, end, lastid, new List<Byte> { (byte) 30 }, timeout, callback);
        }

        /**
         *
         * rtmGate (3h)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(p2p:List(long),group:List(long))} payload
         * </CallbackData>
         */
        public void GetUnreadMessage(int timeout, CallbackDelegate callback) {
            this.SendQuest("getunreadmsg", new Dictionary<string, object>(), callback, timeout);
        }

        /**
         *
         * rtmGate (3i)
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
         * </CallbackData>
         */
        public void CleanUnreadMessage(int timeout, CallbackDelegate callback) {
            this.SendQuest("cleanunreadmsg", new Dictionary<string, object>(), callback, timeout);
        }

        /**
         *
         * rtmGate (3j)
         *
         * @param {int}                     timeout
         * @param {CallbackDelegate}        callback
         *
         * @callback
         * @param {CallbackData}            cbd
         *
         * <CallbackData>
         * @param {Exception}               exception
         * @param {IDictionary(p2p:List(long),group:List(long))}    payload
         * </CallbackData>
         */
        public void GetSession(int timeout, CallbackDelegate callback) {
            this.SendQuest("getsession", new Dictionary<string, object>(), callback, timeout);
        }

        /**
         *
         * rtmGate (3k)
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
        public void DeleteChat(long mid, long xid, byte type, int timeout, CallbackDelegate callback) {
            this.DeleteMessage(mid, xid, type, timeout, callback);
        }

        /**
         *
         * rtmGate (3l)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "lang", targetLanguage
                }
            };
            RTMClient self = this;
            this.SendQuest("setlang", payload, (cbd) => {
                Exception ex = cbd.GetException();

                if (ex == null) {
                    lock (self_locker) {
                        self._targetLanguage = targetLanguage;
                    }
                }

                if (callback != null) {
                    callback(cbd);
                }
            }, timeout);
        }

        /**
         *
         * rtmGate (3m)
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
         * @param {IDictionary(source:string,target:string,sourceText:string,targetText:string)}    payload
         * </CallbackData>
         */
        public void Translate(string originalMessage, string originalLanguage, string targetLanguage, string type, string profanity, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "text", originalMessage },
                { "dst", targetLanguage }
            };

            if (!string.IsNullOrEmpty(originalLanguage)) {
                payload.Add("src", originalLanguage);
            }

            if (!string.IsNullOrEmpty(type)) {
                payload.Add("type", type);
            }

            if (!string.IsNullOrEmpty(profanity)) {
                payload.Add("profanity", profanity);
            }

            this.SendQuest("translate", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (3n)
         *
         * @param {string}                      text
         * @param {string}                      action
         * @param {int}                         timeout
         * @param {CallbackDelegate}            callback
         *
         * @callback
         * @param {CallbackData}                cbd
         *
         * <CallbackData>
         * @param {Exception}                   exception
         * @param {IDictionary(text:string)}    payload
         * </CallbackData>
         */
        public void Profanity(string text, string action, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "text", text
                }
            };

            if (!string.IsNullOrEmpty(action)) {
                payload.Add("action", action);
            }

            this.SendQuest("profanity", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (4a)
         *
         * @param {string}                  cmd
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
        public void FileToken(string cmd, long to, long rid, long gid, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "cmd", cmd
                }
            };

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
         *
         * rtmGate (5a)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "uids", uids
                }
            };
            this.SendQuest("getonlineusers", payload, (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("uids")) {
                    callback(new CallbackData(dict["uids"]));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (5b)
         *
         * @param {string}                  oinfo
         * @param {string}                  pinfo
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
        public void SetUserInfo(string oinfo, string pinfo, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(oinfo)) {
                payload.Add("oinfo", oinfo);
            }

            if (!string.IsNullOrEmpty(pinfo)) {
                payload.Add("pinfo", pinfo);
            }

            this.SendQuest("setuserinfo", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (5c)
         *
         * @param {int}                         timeout
         * @param {CallbackDelegate}            callback
         *
         * @callback
         * @param {CallbackData}                cbd
         *
         * <CallbackData>
         * @param {IDictionary(oinfo:string,pinfo:string)}  payload
         * @param {Exception}                   exception
         * </CallbackData>
         */
        public void GetUserInfo(int timeout, CallbackDelegate callback) {
            this.SendQuest("getuserinfo", new Dictionary<string, object>(), callback, timeout);
        }

        /**
         *
         * rtmGate (5d)
         *
         * @param {List(long)}                  uids
         * @param {int}                         timeout
         * @param {CallbackDelegate}            callback
         *
         * @callback
         * @param {CallbackData}                cbd
         *
         * <CallbackData>
         * @param {IDictionary(string,string)}  payload
         * @param {Exception}                   exception
         * </CallbackData>
         */
        public void GetUserOpenInfo(List<long> uids, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "uids", uids
                }
            };
            this.SendQuest("getuseropeninfo", payload, (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("info")) {
                    callback(new CallbackData(dict["info"]));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (6a)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "friends", friends
                }
            };
            this.SendQuest("addfriends", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (6b)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "friends", friends
                }
            };
            this.SendQuest("delfriends", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (6c)
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
            this.SendQuest("getfriends", new Dictionary<string, object>(), (cbd) => {
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
         * rtmGate (7a)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "gid", gid },
                { "uids", uids }
            };
            this.SendQuest("addgroupmembers", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (7b)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "gid", gid },
                { "uids", uids }
            };
            this.SendQuest("delgroupmembers", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (7c)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "gid", gid
                }
            };
            this.SendQuest("getgroupmembers", payload, (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("uids")) {
                    callback(new CallbackData(dict["uids"]));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (7d)
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
            this.SendQuest("getusergroups", new Dictionary<string, object>(), (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("gids")) {
                    callback(new CallbackData(dict["gids"]));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (7e)
         *
         * @param {long}                    gid
         * @param {string}                  oinfo
         * @param {string}                  pinfo
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
        public void SetGroupInfo(long gid, string oinfo, string pinfo, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "gid", gid
                }
            };

            if (!string.IsNullOrEmpty(oinfo)) {
                payload.Add("oinfo", oinfo);
            }

            if (!string.IsNullOrEmpty(pinfo)) {
                payload.Add("pinfo", pinfo);
            }

            this.SendQuest("setgroupinfo", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (7f)
         *
         * @param {long}                        gid
         * @param {int}                         timeout
         * @param {CallbackDelegate}            callback
         *
         * @callback
         * @param {CallbackData}                cbd
         *
         * <CallbackData>
         * @param {IDictionary(oinfo:string,pinfo:string)}  payload
         * @param {Exception}                   exception
         * </CallbackData>
         */
        public void GetGroupInfo(long gid, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "gid", gid
                }
            };
            this.SendQuest("getgroupinfo", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (7g)
         *
         * @param {long}                        gid
         * @param {int}                         timeout
         * @param {CallbackDelegate}            callback
         *
         * @callback
         * @param {CallbackData}                cbd
         *
         * <CallbackData>
         * @param {IDictionary(oinfo:string)}   payload
         * @param {Exception}                   exception
         * </CallbackData>
         */
        public void GetGroupOpenInfo(long gid, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "gid", gid
                }
            };
            this.SendQuest("getgroupopeninfo", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (8a)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "rid", rid
                }
            };
            this.SendQuest("enterroom", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (8b)
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
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "rid", rid
                }
            };
            this.SendQuest("leaveroom", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (8c)
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
            this.SendQuest("getuserrooms", new Dictionary<string, object>(), (cbd) => {
                if (callback == null) {
                    return;
                }

                IDictionary<string, object> dict = (IDictionary<string, object>)cbd.GetPayload();

                if (dict != null && dict.ContainsKey("rooms")) {
                    callback(new CallbackData(dict["rooms"]));
                    return;
                }

                callback(cbd);
            }, timeout);
        }

        /**
         *
         * rtmGate (8d)
         *
         * @param {long}                    rid
         * @param {string}                  oinfo
         * @param {string}                  pinfo
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
        public void SetRoomInfo(long rid, string oinfo, string pinfo, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "rid", rid
                }
            };

            if (!string.IsNullOrEmpty(oinfo)) {
                payload.Add("oinfo", oinfo);
            }

            if (!string.IsNullOrEmpty(pinfo)) {
                payload.Add("pinfo", pinfo);
            }

            this.SendQuest("setroominfo", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (8e)
         *
         * @param {long}                        rid
         * @param {int}                         timeout
         * @param {CallbackDelegate}            callback
         *
         * @callback
         * @param {CallbackData}                cbd
         *
         * <CallbackData>
         * @param {Exception}                   exception
         * @param {IDictionary(oinfo:string,pinfo:string)}  payload
         * </CallbackData>
         */
        public void GetRoomInfo(long rid, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "rid", rid
                }
            };
            this.SendQuest("getroominfo", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (8f)
         *
         * @param {long}                        rid
         * @param {int}                         timeout
         * @param {CallbackDelegate}            callback
         *
         * @callback
         * @param {CallbackData}                cbd
         *
         * <CallbackData>
         * @param {IDictionary(oinfo:string)}   payload
         * @param {Exception}                   exception
         * </CallbackData>
         */
        public void GetRoomOpenInfo(long rid, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "rid", rid
                }
            };
            this.SendQuest("getroomopeninfo", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (9a)
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
         * @param {IDictionary(val:string)} payload
         * </CallbackData>
         */
        public void DataGet(string key, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "key", key
                }
            };
            this.SendQuest("dataget", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (9b)
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
        public void DataSet(string key, string value, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                { "key", key },
                { "val", value }
            };
            this.SendQuest("dataset", payload, callback, timeout);
        }

        /**
         *
         * rtmGate (9c)
         *
         * @param {string}                  key
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
        public void DataDelete(string key, int timeout, CallbackDelegate callback) {
            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "key", key
                }
            };
            this.SendQuest("datadel", payload, callback, timeout);
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

            Hashtable ops = new Hashtable() {
                { "cmd", "sendfile" },
                { "to", to },
                { "mtype", mtype },
                { "file", fileBytes }
            };
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

            Hashtable ops = new Hashtable() {
                { "cmd", "sendgroupfile" },
                { "gid", gid },
                { "mtype", mtype },
                { "file", fileBytes }
            };
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

            Hashtable ops = new Hashtable() {
                { "cmd", "sendroomfile" },
                { "rid", rid },
                { "mtype", mtype },
                { "file", fileBytes }
            };
            this.FileSendProcess(ops, mid, timeout, callback);
        }

        private void FileSendProcess(Hashtable ops, long mid, int timeout, CallbackDelegate callback) {
            if (ops == null) {
                return;
            }

            if (!ops.Contains("mtype") || !ops.Contains("cmd") || !ops.Contains("file")) {
                return;
            }

            IDictionary<string, object> payload = new Dictionary<string, object>() {
                {
                    "cmd", ops["cmd"]
                }
            };

            if (ops.Contains("to")) {
                payload.Add("to", ops["to"]);
            }

            if (ops.Contains("rid")) {
                payload.Add("rid", ops["rid"]);
            }

            if (ops.Contains("gid")) {
                payload.Add("gid", ops["gid"]);
            }

            this.Filetoken(payload, this.FileSendProcess_Callback(ops, mid, timeout, callback), timeout);
        }

        private void Filetoken(IDictionary<string, object> payload, CallbackDelegate callback, int timeout) {
            this.SendQuest("filetoken", payload, callback, timeout);
        }

        private CallbackDelegate FileSendProcess_Callback(Hashtable ops, long mid, int timeout, CallbackDelegate callback) {
            RTMClient self = this;
            return (cbd) => {
                if (cbd.GetException() != null) {
                    if (callback != null) {
                        callback(cbd);
                    }

                    return;
                }

                object obj = cbd.GetPayload();

                if (obj == null) {
                    return;
                }

                string token = null;
                string endpoint = null;
                IDictionary<string, object> dict = (IDictionary<string, object>)obj;

                if (dict != null) {
                    if (dict.ContainsKey("token")) {
                        token = Convert.ToString(dict["token"]);
                    }

                    if (dict.ContainsKey("endpoint")) {
                        endpoint = Convert.ToString(dict["endpoint"]);
                    }
                }

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(endpoint)) {
                    self.GetEvent().FireEvent(new EventData("error", new Exception("file token error!")));
                    return;
                }

                FileClient fileClient = new FileClient(endpoint, timeout);
                dict = new Dictionary<string, object>() {
                    { "pid", self._pid },
                    { "mtype", ops["mtype"] },
                    { "mid", mid != 0 ? mid : MidGenerator.Gen() },
                    { "from", self._uid }
                };

                if (ops.Contains("to")) {
                    dict.Add("to", ops["to"]);
                }

                if (ops.Contains("rid")) {
                    dict.Add("rid", ops["rid"]);
                }

                if (ops.Contains("gid")) {
                    dict.Add("gid", ops["gid"]);
                }

                lock (self_locker) {
                    fileClient.Send(self._sender, Convert.ToString(ops["cmd"]), (byte[])ops["file"], token, dict, timeout, callback);
                }
            };
        }

        private int _reconnCount = 0;

        private void Reconnect() {
            if (!this._reconnect) {
                return;
            }

            string endpoint = null;

            lock (self_locker) {
                if (this._processor != null) {
                    this._processor.ClearPingTimestamp();
                }

                if (this._isClose) {
                    return;
                }

                endpoint = this._endpoint;
            }

            int count = 0;

            lock (delayconn_locker) {
                this._reconnCount++;
                count = this._reconnCount;
            }

            if (count <= RTMConfig.RECONN_COUNT_ONCE) {
                this.Login(endpoint);
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

            string endpoint = null;

            lock (self_locker) {
                endpoint = this._endpoint;
            }

            this.Login(endpoint);
        }

        private class DispatchClient: BaseClient {

            public DispatchClient(string endpoint, int timeout): base(endpoint, timeout) {}
            public DispatchClient(string host, int port, int timeout): base(host, port, timeout) {}

            public override void AddListener() {
                base.AddListener();
            }

            public void Which(RTMSender sender, IDictionary<string, object> payload, int timeout, CallbackDelegate callback) {
                FPData data = new FPData();
                data.SetFlag(0x1);
                data.SetMtype(0x1);
                data.SetMethod("which");

                if (sender != null) {
                    sender.AddQuest(this, data, payload, this.QuestCallback(callback), timeout);
                }
            }
        }

        private class FileClient: BaseClient {

            public FileClient(string endpoint, int timeout): base(endpoint, timeout) {}
            public FileClient(string host, int port, int timeout): base(host, port, timeout) {}

            public override void AddListener() {
                base.AddListener();
            }

            public void Send(RTMSender sender, string method, byte[] fileBytes, string token, IDictionary<string, object> payload, int timeout, CallbackDelegate callback) {
                string fileMd5 = base.CalcMd5(fileBytes, false);
                string sign = base.CalcMd5(fileMd5 + ":" + token, false);

                if (string.IsNullOrEmpty(sign)) {
                    ErrorRecorderHolder.recordError(new Exception("wrong sign!"));
                    return;
                }

                if (!base.HasConnect()) {
                    base.Connect();
                }

                IDictionary<string, string> attrs = new Dictionary<string, string>() {
                    {
                        "sign", sign
                    }
                };
                payload.Add("token", token);
                payload.Add("file", fileBytes);
                payload.Add("attrs", Json.SerializeToString(attrs));
                long mid = (long)Convert.ToInt64(payload["mid"]);
                FPData data = new FPData();
                data.SetFlag(0x1);
                data.SetMtype(0x1);
                data.SetMethod(method);
                FileClient self = this;

                if (sender != null) {
                    sender.AddQuest(this, data, payload, this.QuestCallback((cbd) => {
                        cbd.SetMid(mid);
                        self.Close();

                        if (callback != null) {
                            callback(cbd);
                        }
                    }), timeout);
                }
            }
        }

        private class BaseClient: FPClient {

            public BaseClient(string endpoint, int timeout): base(endpoint, timeout) {
                this.AddListener();
            }

            public BaseClient(string host, int port, int timeout): base(host, port, timeout) {
                this.AddListener();
            }

            public virtual void AddListener() {
                base.Client_Error = (evd) => {
                    ErrorRecorderHolder.recordError(evd.GetException());
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

            private void CheckFPCallback(CallbackData cbd) {
                bool isAnswerException = false;
                FPData data = cbd.GetData();
                IDictionary<string, object> payload = null;

                if (data != null) {
                    if (data.GetFlag() == 0) {
                        try {
                            payload = Json.Deserialize<IDictionary<string, object>>(data.JsonPayload());
                        } catch (Exception ex) {
                            ErrorRecorderHolder.recordError(ex);
                        }
                    }

                    if (data.GetFlag() == 1) {
                        try {
                            using (MemoryStream inputStream = new MemoryStream(data.MsgpackPayload())) {
                                payload = MsgPack.Deserialize<IDictionary<string, object>>(inputStream);
                            }
                        } catch (Exception ex) {
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

        private class RTMErrorRecorder: ErrorRecorder {

            private bool _debug;

            public RTMErrorRecorder(bool debug) {
                this._debug = debug;
            }

            public override void recordError(Exception ex) {
                if (this._debug) {
                    Debug.LogError(ex);
                }
            }
        }
    }
}