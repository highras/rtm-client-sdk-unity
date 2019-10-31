using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using GameDevWare.Serialization;
using UnityEngine;

using com.fpnn;
using com.rtm;

public class SingleClientPush : Main.ITestCase {

    private int trace_interval = 10;

    private RTMClient _client;

    /**
     *  单客户端实例接收QPS脚本
     *  服务控制 https://www.livedata.top/console/rtm/api/tool?pid=11000002
     */
    public SingleClientPush() {}

    public void StartTest(byte[] fileBytes) {
        this._client = new RTMClient(
            "rtm-intl-frontgate.funplus.com:13325",
            11000002,
            777779,
            "266B02147F447DD931C48B747F5ED9E7",
            RTMConfig.TRANS_LANGUAGE.en,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            // false
            true
        );
        SingleClientPush self = this;
        this._client.GetEvent().AddListener("login", (evd) => {
            if (evd.GetException() == null) {
                Debug.Log("test start!");
            } else {
                Debug.Log(evd.GetException());
            }
        });
        this._client.GetEvent().AddListener("close", (evd) => {
            Debug.Log("closed!");
        });
        this._client.GetEvent().AddListener("error", (evd) => {
            Debug.Log(evd.GetException());
        });
        RTMProcessor processor = this._client.GetProcessor();
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvPing, (data) => {
            self.RevcInc();
            // Debug.Log("[PUSH] ping: " + Json.SerializeToString(data));
        });
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvMessage, (data) => {
            self.RevcInc();
            // Debug.Log("[recvMessage]: " + Json.SerializeToString(data));
        });
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvGroupMessage, (data) => {
            self.RevcInc();
            // Debug.Log("[recvGroupMessage]: ");
        });
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvRoomMessage, (data) => {
            self.RevcInc();
            // Debug.Log("[recvRoomMessage]: ");
        });
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvBroadcastMessage, (data) => {
            self.RevcInc();
            // Debug.Log("[recvBroadcastMessage]: ");
        });
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvFile, (data) => {
            self.RevcInc();
            // Debug.Log("[recvFile]: ");
        });
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvRoomFile, (data) => {
            self.RevcInc();
            // Debug.Log("[recvRoomFile]: ");
        });
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvGroupFile, (data) => {
            self.RevcInc();
            // Debug.Log("[recvGroupFile]: ");
        });
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvBroadcastFile, (data) => {
            self.RevcInc();
            // Debug.Log("[recvBroadcastFile]: ");
        });
        this._client.Login(null);
    }

    public void Update() {}

    public void StopTest() {
        if (this._client != null) {
            this._client.Destroy();
        }
    }

    private int _recvCount;
    private long _traceTimestamp;

    private object inc_locker = new object();

    private void RevcInc() {
        lock (inc_locker) {
            this._recvCount++;

            if (this._traceTimestamp <= 0) {
                this._traceTimestamp = com.fpnn.FPManager.Instance.GetMilliTimestamp();
            }

            int interval = (int)((com.fpnn.FPManager.Instance.GetMilliTimestamp() - this._traceTimestamp) / 1000);

            if (interval >= this.trace_interval) {
                Debug.Log(
                    com.fpnn.FPManager.Instance.GetMilliTimestamp()
                    + ", trace interval: " + interval
                    + ", revc qps: " + (int)(this._recvCount / interval)
                );
                this._traceTimestamp = com.fpnn.FPManager.Instance.GetMilliTimestamp();
                this._recvCount = 0;
            }
        }
    }
}