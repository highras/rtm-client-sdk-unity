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
                "BC53223CC192F2F14B63216A92A8CDB3",
                null,
                new Dictionary<string, string>(),
                true,
                20 * 1000
	        );

			this._client.GetProcessor().AddPushListener(RTMConfig.SERVER_PUSH.recvPing, (dict) => {

				MemoryStream jsonStream = new MemoryStream();
                Json.Serialize(dict, jsonStream);

	        	Debug.Log("[PUSH] ping: " + System.Text.Encoding.UTF8.GetString(jsonStream.ToArray()));
            });

	        this._client.ErrorCallback = delegate(Exception e) {

                Debug.Log("TestCase error: " + e.Message);
            };

            this._client.ClosedCallback = delegate() {

                Debug.Log("TestCase closed!");
            };

            this._client.ConnectedCallback = delegate() {

            	Debug.Log("TestCase connect!");

            	onLogin();
            };

            this._client.Login(null, false);
		}

		public void Close() {

			this._client.Destroy();
		}

		private void onLogin() {

			long to = 778899;
			long gid = 999;
	        long rid = 666;

			int timeout = 20 * 1000;
	        int sleep = 1000;

			Debug.Log("\ntest start!");
			Thread.Sleep(sleep);

			//rtmGate (2)
	        //---------------------------------SendMessage--------------------------------------
	        this._client.SendMessage(to, (byte) 8, "hello !", "", 0, timeout, (Hashtable ht) => {


	        	if (ht.Contains("exception")) {

	        		Exception ex = (Exception)ht["exception"];
	        		Debug.Log("[ERR] SendMessage: " + ex.Message);
	        	}

	        	if (ht.Contains("payload")) {

	        		Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];

                    MemoryStream jsonStream = new MemoryStream();
                    Json.Serialize(dict, jsonStream);

	        		Debug.Log("[DATA] SendMessage: " + System.Text.Encoding.UTF8.GetString(jsonStream.ToArray()) + ", mid: " + ht["mid"]);
	        	}
	        });

	        Thread.Sleep(sleep);

			//rtmGate (3)
	        //---------------------------------SendGroupMessage--------------------------------------
	        this._client.SendGroupMessage(gid, (byte) 8, "hello !", "", 0, timeout, (Hashtable ht) => {


	        	if (ht.Contains("exception")) {

	        		Exception ex = (Exception)ht["exception"];
	        		Debug.Log("[ERR] SendGroupMessage: " + ex.Message);
	        	}

	        	if (ht.Contains("payload")) {

	        		Dictionary<string, object> dict = (Dictionary<string, object>)ht["payload"];

                    MemoryStream jsonStream = new MemoryStream();
                    Json.Serialize(dict, jsonStream);

	        		Debug.Log("[DATA] SendGroupMessage: " + System.Text.Encoding.UTF8.GetString(jsonStream.ToArray()) + ", mid: " + ht["mid"]);
	        	}
	        });
		}
	} 
}