using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameDevWare.Serialization;
using com.rtm;
using UnityEngine;

namespace com.test {

	public class TestCase {

		private RTMClient _client;

		public TestCase() {

			this._client = new RTMClient(
                "52.83.245.22:13325",
                1000012,
                654321,
                "3993142515BD88A7156629A3AE550B9B",
                null,
                new Dictionary<string, string>(),
                true,
                20 * 1000,
                true
	        );

	        RTMProcessor processor = this._client.GetProcessor();

			processor.GetEvent().AddListener(RTMConfig.SERVER_PUSH.recvPing, (evd) => {

				MemoryStream jsonStream = new MemoryStream();
                Json.Serialize((Dictionary<string, object>)evd.GetPayload(), jsonStream);

	        	Debug.Log("[PUSH] ping: " + System.Text.Encoding.UTF8.GetString(jsonStream.ToArray()));
            });

            this._client.GetEvent().AddListener("login", (evd) => {

            	Exception ex = evd.GetException();

            	if (ex != null) {

            		Debug.Log("TestCase connect err: " + ex.Message);
            	} else {

	            	Debug.Log("TestCase connect!");
	            	OnLogin();
            	}
            });

            this._client.GetEvent().AddListener("close", (evd) => {

            	Debug.Log("TestCase closed!");
            });

            this._client.GetEvent().AddListener("error", (evd) => {

            	Debug.Log("TestCase error: " + evd.GetException().Message);
            });

            this._client.Login(null, false);
		}

		public void Close() {

			this._client.Destroy();
		}

		private void OnLogin() {

			long to = 778899;
			long gid = 999;
	        long rid = 666;

			int timeout = 20 * 1000;
	        int sleep = 1000;

			Debug.Log("\ntest start!");
			Thread.Sleep(sleep);

			//rtmGate (2)
	        //---------------------------------SendMessage--------------------------------------
	        this._client.SendMessage(to, (byte) 8, "hello !", "", 0, timeout, (cbd) => {

	        	object obj = cbd.GetPayload();

	        	if (obj != null) {

	        		Dictionary<string, object> dict = (Dictionary<string, object>)obj;

                    MemoryStream jsonStream = new MemoryStream();
                    Json.Serialize(dict, jsonStream);

	        		Debug.Log("[DATA] SendMessage: " + System.Text.Encoding.UTF8.GetString(jsonStream.ToArray()) + ", mid: " + cbd.GetMid());
	        	} else {

	        		Debug.Log("[ERR] SendMessage: " + cbd.GetException().Message);
	        	}
	        });

	        Thread.Sleep(sleep);

			//rtmGate (3)
	        //---------------------------------SendGroupMessage--------------------------------------
	        this._client.SendGroupMessage(gid, (byte) 8, "hello !", "", 0, timeout, (cbd) => {

	        	object obj = cbd.GetPayload();

	        	if (obj != null) {

	        		Dictionary<string, object> dict = (Dictionary<string, object>)obj;

                    MemoryStream jsonStream = new MemoryStream();
                    Json.Serialize(dict, jsonStream);

	        		Debug.Log("[DATA] SendGroupMessage: " + System.Text.Encoding.UTF8.GetString(jsonStream.ToArray()) + ", mid: " + cbd.GetMid());
	        	} else {

	        		Debug.Log("[ERR] SendGroupMessage: " + cbd.GetException().Message);
	        	}
	        });
		}
	} 
}