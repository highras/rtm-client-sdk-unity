using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameDevWare.Serialization;
using com.fpnn;

namespace com.rtm {

	public delegate void RTMPushCallbackDelegate(Dictionary<string, object> dict);

	public class RTMProcessor {

		public FPProcessor Processor;

		public RTMProcessor() {

			this.Processor = new FPProcessor();
		}

		public void AddPushListener(string name, RTMPushCallbackDelegate callback) {

            this.Processor.AddListener(name, (data) => {

            	Dictionary<string, object> payload = null;
	    		MemoryStream inputStream = new MemoryStream(data.payload);

	        	if ((data.flag & Convert.ToByte(FP_FLAG.FP_FLAG_JSON)) != 0) {

	        		payload = Json.Deserialize<Dictionary<string, object>>(inputStream);
	        	}

	        	if ((data.flag & Convert.ToByte(FP_FLAG.FP_FLAG_MSGPACK)) != 0) {

	                payload = MsgPack.Deserialize<Dictionary<string, object>>(inputStream);
	        	}

	        	callback(payload);
            });
        }
	}
}