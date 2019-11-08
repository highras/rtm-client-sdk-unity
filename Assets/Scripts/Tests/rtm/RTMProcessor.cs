using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameDevWare.Serialization;
using com.fpnn;

using UnityEngine;

namespace com.rtm {

    public class RTMProcessor: FPProcessor.IProcessor {

        private static string JSON_PAYLOAD = "{}";
        private static byte[] MSGPACK_PAYLOAD = { 0x80 };

        private class PingLocker {
            public int Status = 0;
        }

        private object self_locker = new object();
        private IDictionary<string, Action<IDictionary<string, object>>> _actionDict = new Dictionary<string, Action<IDictionary<string, object>>>();

        private Type _type;

        public RTMProcessor() {
            this._type = Type.GetType("com.rtm.RTMProcessor");
        }

        public void Destroy() {
            this.ClearPingTimestamp();

            lock (self_locker) {
                this._actionDict.Clear();
                this._duplicateMap.Clear();
            }
        }

        public void Service(FPData data, AnswerDelegate answer) {
            if (data == null) {
                return;
            }

            bool callCb = true;

            if (RTMConfig.KICKOUT == data.GetMethod()) {
                callCb = false;
            }

            if (RTMConfig.SERVER_PUSH.kickOutRoom == data.GetMethod()) {
                callCb = false;
            }

            IDictionary<string, object> payload = null;

            if (data.GetFlag() == 0 && data.JsonPayload() != null) {
                try {
                    if (callCb && answer != null) {
                        answer(JSON_PAYLOAD, false);
                    }

                    payload = Json.Deserialize<IDictionary<string, object>>(data.JsonPayload());
                } catch (Exception ex) {
                    ErrorRecorderHolder.recordError(ex);
                }
            }

            if (data.GetFlag() == 1 && data.MsgpackPayload() != null) {
                try {
                    if (callCb && answer != null) {
                        answer(MSGPACK_PAYLOAD, false);
                    }

                    using (MemoryStream inputStream = new MemoryStream(data.MsgpackPayload())) {
                        // payload = MsgPack.Deserialize<IDictionary<string, object>>(inputStream);
                        payload = MsgPackFix.Deserialize<IDictionary<string, object>>(inputStream, RTMRegistration.RTMEncoding);
                    }
                } catch (Exception ex) {
                    ErrorRecorderHolder.recordError(ex);
                }
            }

            if (payload != null) {
                try {
                    MethodInfo method = this._type.GetMethod(data.GetMethod());

                    if (method != null) {
                        object[] paras = new object[] {payload};
                        method.Invoke(this, paras);
                    }
                } catch (Exception ex) {
                    ErrorRecorderHolder.recordError(ex);
                }
            }
        }

        public bool HasPushService(string name) {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            return true;
        }

        public void AddPushService(string name, Action<IDictionary<string, object>> action) {
            if (string.IsNullOrEmpty(name)) {
                return;
            }

            lock (self_locker) {
                if (!this._actionDict.ContainsKey(name)) {
                    this._actionDict.Add(name, action);
                } else {
                    Debug.LogWarning("push service exist");
                }
            }
        }

        public void RemovePushService(string name) {
            if (string.IsNullOrEmpty(name)) {
                return;
            }

            lock (self_locker) {
                if (this._actionDict.ContainsKey(name)) {
                    this._actionDict.Remove(name);
                }
            }
        }

        private void PushService(string name, IDictionary<string, object> data) {
            lock (self_locker) {
                if (this._actionDict.ContainsKey(name)) {
                    Action<IDictionary<string, object>> action = this._actionDict[name];

                    try {
                        if (action != null) {
                            action(data);
                        }
                    } catch (Exception ex) {
                        ErrorRecorderHolder.recordError(ex);
                    }
                }
            }
        }

        /**
         *
         * serverPush(1a)
         *
         * @param {IDictionary<string, object>} data
         */
        public void kickout(IDictionary<string, object> data) {
            this.PushService(RTMConfig.KICKOUT, data);
        }

        /**
         *
         * serverPush(1b)
         *
         * @param {Map} data
         */
        public void ping(IDictionary<string, object> data) {
            this.PushService(RTMConfig.SERVER_PUSH.recvPing, data);

            lock (ping_locker) {
                this._lastPingTimestamp = FPManager.Instance.GetMilliTimestamp();
            }
        }

        /**
         *
         * serverPush(1c)
         *
         * @param {long} data.rid
         */
        public void kickoutroom(IDictionary<string, object> data) {
            this.PushService(RTMConfig.SERVER_PUSH.kickOutRoom, data);
        }

        /**
         *
         * serverPush(2a)
         *
         * @param {long}   data.from
         * @param {long}   data.to
         * @param {byte}   data.mtype
         * @param {long}   data.mid
         * @param {string} data.msg
         * @param {string} data.attrs
         * @param {long}   data.mtime
         */
        public void pushmsg(IDictionary<string, object> data) {
            byte mtype = 0;
            string name = RTMConfig.SERVER_PUSH.recvMessage;

            if (data == null) {
                this.PushService(name, data);
                return;
            }

            if (data.ContainsKey("mid") && data.ContainsKey("from")) {
                if (!this.CheckMid(1, (long)Convert.ToInt64(data["mid"]), (long)Convert.ToInt64(data["from"]), 0)) {
                    return;
                }
            }

            if (data.ContainsKey("mtype")) {
                mtype = Convert.ToByte(data["mtype"]);
            }

            if (mtype == RTMConfig.CHAT_TYPE.text) {
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvChat;
            }

            if (mtype == RTMConfig.CHAT_TYPE.audio) {
                if (data.ContainsKey("msg") && data["msg"].GetType() == typeof(String)) {
                    byte[] msg = Json.DefaultEncoding.GetBytes(Convert.ToString(data["msg"]));
                    data["msg"] = msg;
                }
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvAudio;
            }

            if (mtype == RTMConfig.CHAT_TYPE.cmd) {
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvCmd;
            }

            if (mtype >= 40 && mtype <= 50) {
                name = RTMConfig.SERVER_PUSH.recvFile;
            }

            this.PushService(name, data);
        }

        /**
         *
         * serverPush(2b)
         *
         * @param {long}   data.from
         * @param {long}   data.gid
         * @param {byte}   data.mtype
         * @param {long}   data.mid
         * @param {string} data.msg
         * @param {string} data.attrs
         * @param {long}   data.mtime
         */
        public void pushgroupmsg(IDictionary<string, object> data) {
            byte mtype = 0;
            string name = RTMConfig.SERVER_PUSH.recvGroupMessage;

            if (data == null) {
                this.PushService(name, data);
                return;
            }

            if (data.ContainsKey("mid") && data.ContainsKey("from") && data.ContainsKey("gid")) {
                if (!this.CheckMid(2, (long)Convert.ToInt64(data["mid"]), (long)Convert.ToInt64(data["from"]), (long)Convert.ToInt64(data["gid"]))) {
                    return;
                }
            }

            if (data.ContainsKey("mtype")) {
                mtype = Convert.ToByte(data["mtype"]);
            }

            if (mtype == RTMConfig.CHAT_TYPE.text) {
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvGroupChat;
            }

            if (mtype == RTMConfig.CHAT_TYPE.audio) {
                if (data.ContainsKey("msg") && data["msg"].GetType() == typeof(String)) {
                    byte[] msg = Json.DefaultEncoding.GetBytes(Convert.ToString(data["msg"]));
                    data["msg"] = msg;
                }
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvGroupAudio;
            }

            if (mtype == RTMConfig.CHAT_TYPE.cmd) {
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvGroupCmd;
            }

            if (mtype >= 40 && mtype <= 50) {
                name = RTMConfig.SERVER_PUSH.recvGroupFile;
            }

            this.PushService(name, data);
        }

        /**
         *
         * serverPush(2c)
         *
         * @param {long}   data.from
         * @param {long}   data.rid
         * @param {byte}   data.mtype
         * @param {long}   data.mid
         * @param {string} data.msg
         * @param {string} data.attrs
         * @param {long}   data.mtime
         */
        public void pushroommsg(IDictionary<string, object> data) {
            byte mtype = 0;
            string name = RTMConfig.SERVER_PUSH.recvRoomMessage;

            if (data == null) {
                this.PushService(name, data);
                return;
            }

            if (data.ContainsKey("mid") && data.ContainsKey("from") && data.ContainsKey("rid")) {
                if (!this.CheckMid(3, (long)Convert.ToInt64(data["mid"]), (long)Convert.ToInt64(data["from"]), (long)Convert.ToInt64(data["rid"]))) {
                    return;
                }
            }

            if (data.ContainsKey("mtype")) {
                mtype = Convert.ToByte(data["mtype"]);
            }

            if (mtype == RTMConfig.CHAT_TYPE.text) {
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvRoomChat;
            }

            if (mtype == RTMConfig.CHAT_TYPE.audio) {
                if (data.ContainsKey("msg") && data["msg"].GetType() == typeof(String)) {
                    byte[] msg = Json.DefaultEncoding.GetBytes(Convert.ToString(data["msg"]));
                    data["msg"] = msg;
                }
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvRoomAudio;
            }

            if (mtype == RTMConfig.CHAT_TYPE.cmd) {
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvRoomCmd;
            }

            if (mtype >= 40 && mtype <= 50) {
                name = RTMConfig.SERVER_PUSH.recvRoomFile;
            }

            this.PushService(name, data);
        }

        /**
         *
         * serverPush(2d)
         *
         * @param {long}   data.from
         * @param {byte}   data.mtype
         * @param {long}   data.mid
         * @param {string} data.msg
         * @param {string} data.attrs
         * @param {long}   data.mtime
         */
        public void pushbroadcastmsg(IDictionary<string, object> data) {
            byte mtype = 0;
            string name = RTMConfig.SERVER_PUSH.recvBroadcastMessage;

            if (data == null) {
                this.PushService(name, data);
                return;
            }

            if (data.ContainsKey("mid") && data.ContainsKey("from")) {
                if (!this.CheckMid(4, (long)Convert.ToInt64(data["mid"]), (long)Convert.ToInt64(data["from"]), 0)) {
                    return;
                }
            }

            if (data.ContainsKey("mtype")) {
                mtype = Convert.ToByte(data["mtype"]);
            }

            if (mtype == RTMConfig.CHAT_TYPE.text) {
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvBroadcastChat;
            }

            if (mtype == RTMConfig.CHAT_TYPE.audio) {
                if (data.ContainsKey("msg") && data["msg"].GetType() == typeof(String)) {
                    byte[] msg = Json.DefaultEncoding.GetBytes(Convert.ToString(data["msg"]));
                    data["msg"] = msg;
                }
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvBroadcastAudio;
            }

            if (mtype == RTMConfig.CHAT_TYPE.cmd) {
                data.Remove("mtype");
                name = RTMConfig.SERVER_PUSH.recvBroadcastCmd;
            }

            if (mtype >= 40 && mtype <= 50) {
                name = RTMConfig.SERVER_PUSH.recvBroadcastFile;
            }

            this.PushService(name, data);
        }

        /**
         *
         * serverPush(a)
         *
         * @param {long}            data.from
         * @param {long}            data.to
         * @param {byte}            data.mtype
         * @param {long}            data.mid
         * @param {Url}             data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushfile(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(b)
         *
         * @param {long}            data.from
         * @param {long}            data.gid
         * @param {byte}            data.mtype
         * @param {long}            data.mid
         * @param {Url}             data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushgroupfile(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(c)
         *
         * @param {long}            data.from
         * @param {long}            data.rid
         * @param {byte}            data.mtype
         * @param {long}            data.mid
         * @param {Url}             data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushroomfile(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(d)
         *
         * @param {long}            data.from
         * @param {byte}            data.mtype
         * @param {long}            data.mid
         * @param {Url}             data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushbroadcastfile(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3a)
         *
         * @param {long}            data.from
         * @param {long}            data.to
         * @param {long}            data.mid
         * @param {JsonString}      data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         *
         * <JsonString>
         * @param {string}          source
         * @param {string}          target
         * @param {string}          sourceText
         * @param {string}          targetText
         * </JsonString>
         */
        public void pushchat(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3a')
         *
         * @param {long}            data.from
         * @param {long}            data.to
         * @param {long}            data.mid
         * @param {byte[]}          data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushaudio(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3a'')
         *
         * @param {long}            data.from
         * @param {long}            data.to
         * @param {long}            data.mid
         * @param {string}          data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushcmd(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3b)
         *
         * @param {long}            data.from
         * @param {long}            data.gid
         * @param {long}            data.mid
         * @param {JsonString}      data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         *
         * <JsonString>
         * @param {string}          source
         * @param {string}          target
         * @param {string}          sourceText
         * @param {string}          targetText
         * </JsonString>
         */
        public void pushgroupchat(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3b')
         *
         * @param {long}            data.from
         * @param {long}            data.gid
         * @param {long}            data.mid
         * @param {byte[]}          data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushgroupaudio(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3b'')
         *
         * @param {long}            data.from
         * @param {long}            data.gid
         * @param {long}            data.mid
         * @param {string}          data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushgroupcmd(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3c)
         *
         * @param {long}            data.from
         * @param {long}            data.rid
         * @param {long}            data.mid
         * @param {JsonString}      data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         *
         * <JsonString>
         * @param {string}          source
         * @param {string}          target
         * @param {string}          sourceText
         * @param {string}          targetText
         * </JsonString>
         */
        public void pushroomchat(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3c')
         *
         * @param {long}            data.from
         * @param {long}            data.rid
         * @param {long}            data.mid
         * @param {byte[]}          data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushroomaudio(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3c'')
         *
         * @param {long}            data.from
         * @param {long}            data.rid
         * @param {long}            data.mid
         * @param {string}          data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushroomcmd(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3d)
         *
         * @param {long}            data.from
         * @param {long}            data.mid
         * @param {JsonString}      data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         *
         * <JsonString>
         * @param {string}          source
         * @param {string}          target
         * @param {string}          sourceText
         * @param {string}          targetText
         * </JsonString>
         */
        public void pushbroadcastchat(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3d')
         *
         * @param {long}            data.from
         * @param {long}            data.mid
         * @param {byte[]}          data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushbroadcastaudio(IDictionary<string, object> data) {}

        /**
         *
         * serverPush(3d'')
         *
         * @param {long}            data.from
         * @param {long}            data.mid
         * @param {string}          data.msg
         * @param {string}          data.attrs
         * @param {long}            data.mtime
         */
        public void pushbroadcastcmd(IDictionary<string, object> data) {}

        private long _lastPingTimestamp;
        private PingLocker ping_locker = new PingLocker();

        public long GetPingTimestamp() {
            lock (ping_locker) {
                return this._lastPingTimestamp;
            }
        }

        public void ClearPingTimestamp() {
            lock (ping_locker) {
                this._lastPingTimestamp = 0;
            }
        }

        public void InitPingTimestamp() {
            lock (ping_locker) {
                if (this._lastPingTimestamp == 0) {
                    this._lastPingTimestamp = FPManager.Instance.GetMilliTimestamp();
                }
            }
        }

        public void OnSecond(long timestamp) {
            this.CheckExpire(timestamp);
        }

        private IDictionary<string, long> _duplicateMap = new Dictionary<string, long>();

        private bool CheckMid(int type, long mid, long uid, long rgid) {
            StringBuilder sb = new StringBuilder(50);
            sb.Append(Convert.ToString(type));
            sb.Append("_");
            sb.Append(Convert.ToString(mid));
            sb.Append("_");
            sb.Append(Convert.ToString(uid));

            if (rgid > 0) {
                sb.Append("_");
                sb.Append(Convert.ToString(rgid));
            }

            string key = sb.ToString();

            lock (self_locker) {
                long timestamp = FPManager.Instance.GetMilliTimestamp();

                if (this._duplicateMap.ContainsKey(key)) {
                    long expire = this._duplicateMap[key];

                    if (expire > timestamp) {
                        return false;
                    }
                    this._duplicateMap.Remove(key);
                }
                this._duplicateMap.Add(key, RTMConfig.MID_TTL + timestamp);
                return true;
            }
        }

        private void CheckExpire(long timestamp) {
            lock (self_locker) {
                List<string> keys = new List<string>(this._duplicateMap.Keys);

                foreach (string key in keys) {
                    long expire = this._duplicateMap[key];

                    if (expire > timestamp) {
                        continue;
                    }

                    this._duplicateMap.Remove(key);
                }
            }
        }
    }
}