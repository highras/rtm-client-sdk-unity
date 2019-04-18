using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameDevWare.Serialization;
using com.fpnn;

// using UnityEngine;

namespace com.rtm {

	public class RTMProcessor:FPProcessor.IProcessor {

		private FPEvent _event;
	    private Hashtable _midMap = new Hashtable();

	    private Type _type;
	    private object _obj;

	    public RTMProcessor(FPEvent evt) {

	        this._event = evt;
	        this._type = Type.GetType("com.rtm.RTMProcessor");
	    }

	    public FPEvent GetEvent() {

	        return this._event;
	    }

	    public void Destroy() {

	        this._midMap.Clear();
	    }

	    public void Service(FPData data, AnswerDelegate answer) {

	    	bool callCb = true;

	        if (RTMConfig.SERVER_PUSH.kickOut == data.GetMethod()) {

	            callCb = false;
	        }

	        if (RTMConfig.SERVER_PUSH.kickOutRoom == data.GetMethod()) {

	            callCb = false;
	        }

	        Dictionary<string, object> payload = null;

	        if (data.GetFlag() == 0) {

	            if (callCb) {

	                MemoryStream jsonStream = new MemoryStream();
                    Json.Serialize(new Dictionary<string, object>(), jsonStream);

	                answer(System.Text.Encoding.UTF8.GetString(jsonStream.ToArray()), false);
	            }

	            payload = Json.Deserialize<Dictionary<string, object>>(data.JsonPayload());
	        }

	        if (data.GetFlag() == 1) {

	        	if (callCb) {

		        	MemoryStream msgpackStream = new MemoryStream();

	                MsgPack.Serialize(new Dictionary<string, object>(), msgpackStream);
	                msgpackStream.Position = 0; 

	                answer(msgpackStream.ToArray(), false);
	        	}

	        	MemoryStream inputStream = new MemoryStream(data.MsgpackPayload());
                payload = MsgPack.Deserialize<Dictionary<string, object>>(inputStream);
	        }

	        if (payload != null) {

	        	MethodInfo method = this._type.GetMethod(data.GetMethod());

	        	if (method != null) {

	        		object[] paras = new object[] {payload};
	        		method.Invoke(this, paras);
	        	}
	        }
	    }

	    /**
	     * @param {Dictionary<string, object>} data
	     */
	    public void kickout(Dictionary<string, object> data) {

	        this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.kickOut, data));
	    }

	    /**
	     * @param {long} data.rid
	     */
	    public void kickoutroom(Dictionary<string, object> data) {

	        this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.kickOutRoom, data));
	    }

	    /**
	     * @param {long}   data.from
	     * @param {long}   data.to
	     * @param {byte}   data.mtype
	     * @param {long}   data.mid
	     * @param {string} data.msg
	     * @param {string} data.attrs
	     * @param {long}   data.mtime
	     */
	    public void pushmsg(Dictionary<string, object> data) {

	        if (data.ContainsKey("mid")) {

	            if (!this.CheckMid(1, (long)Convert.ToInt64(data["mid"]), (long)Convert.ToInt64(data["from"]), 0)) {

	                return;
	            }
	        }

	        byte mtype = Convert.ToByte(data["mtype"]);

	        if (mtype >= 40 && mtype <= 50) {

	            this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.recvFile, data));
	            return;
	        }

	        this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.recvMessage, data));
	    }

	    /**
	     * @param {long}   data.from
	     * @param {long}   data.gid
	     * @param {byte}   data.mtype
	     * @param {long}   data.mid
	     * @param {string} data.msg
	     * @param {string} data.attrs
	     * @param {long}   data.mtime
	     */
	    public void pushgroupmsg(Dictionary<string, object> data) {

	        if (data.ContainsKey("mid")) {

	            if (!this.CheckMid(2, (long)Convert.ToInt64(data["mid"]), (long)Convert.ToInt64(data["from"]), (long)Convert.ToInt64(data["gid"]))) {

	                return;
	            }
	        }

	        byte mtype = Convert.ToByte(data["mtype"]);

	        if (mtype >= 40 && mtype <= 50) {

	            this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.recvGroupFile, data));
	            return;
	        }

	        this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.recvGroupMessage, data));
	    }

	    /**
	     * @param {long}   data.from
	     * @param {long}   data.rid
	     * @param {byte}   data.mtype
	     * @param {long}   data.mid
	     * @param {string} data.msg
	     * @param {string} data.attrs
	     * @param {long}   data.mtime
	     */
	    public void pushroommsg(Dictionary<string, object> data) {

	        if (data.ContainsKey("mid")) {

	            if (!this.CheckMid(3, (long)Convert.ToInt64(data["mid"]), (long)Convert.ToInt64(data["from"]), (long)Convert.ToInt64(data["rid"]))) {

	                return;
	            }
	        }

	        byte mtype = Convert.ToByte(data["mtype"]);

	        if (mtype >= 40 && mtype <= 50) {

	            this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.recvRoomFile, data));
	            return;
	        }

	        this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.recvRoomMessage, data));
	    }

	    /**
	     * @param {long}   data.from
	     * @param {byte}   data.mtype
	     * @param {long}   data.mid
	     * @param {string} data.msg
	     * @param {string} data.attrs
	     * @param {long}   data.mtime
	     */
	    public void pushbroadcastmsg(Dictionary<string, object> data) {

	        if (data.ContainsKey("mid")) {

	            if (!this.CheckMid(4, (long)Convert.ToInt64(data["mid"]), (long)Convert.ToInt64(data["from"]), 0)) {

	                return;
	            }
	        }

	        byte mtype = Convert.ToByte(data["mtype"]);

	        if (mtype >= 40 && mtype <= 50) {

	            this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.recvBroadcastFile, data));
	            return;
	        }

	        this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.recvBroadcastMessage, data));
	    }

	    /**
	     * @param {Map} data
	     */
	    public void ping(Dictionary<string, object> data) {

	        this._event.FireEvent(new EventData(RTMConfig.SERVER_PUSH.recvPing, data));
	    }

	    public void OnSecond(long timestamp) {

	    	this.CheckExpire(timestamp);
	    }

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

	        lock(this._midMap) {

	            long timestamp = ThreadPool.Instance.GetMilliTimestamp();

	            if (this._midMap.ContainsKey(key)) {

	                long expire = (long)Convert.ToInt64(this._midMap[key]);

	                if (expire > timestamp) {

	                    return false;
	                }

	                this._midMap.Remove(key);
	            }

	            this._midMap.Add(key, RTMConfig.MID_TTL + timestamp);
	            return true;
	        }
	    }

	    private void CheckExpire(long timestamp) {

	        lock(this._midMap) {

	            List<string> keys = new List<string>();

	            foreach (DictionaryEntry entry in this._midMap) {

	        		string key = (string)entry.Key;
	                long expire = (long)entry.Value;

	                if (expire > timestamp) {

	                    continue;
	                }

	                keys.Add(key);
	        	}

	        	foreach (string rkey in keys) {

	        		this._midMap.Remove(rkey);
	        	}
	        }
	    }
	}
}