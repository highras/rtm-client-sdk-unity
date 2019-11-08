using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using GameDevWare.Serialization;
using UnityEngine;

using com.fpnn;
using com.rtm;

public class SingleClientSend : Main.ITestCase {

    private class SendLocker {

        public int Status = 0;
    }

    private int send_qps = 500;
    private int trace_interval = 10;
    private int batch_count = 10;

    private RTMClient _client;
    private SendLocker send_locker = new SendLocker();

    /**
     *  单客户端实例发送QPS脚本
     *  需要修改服务端接口调用频率
     */
    public SingleClientSend() {}

    public void StartTest(byte[] fileBytes) {
        this._client = new RTMClient(
            "52.83.245.22:13325",
            11000001,
            777779,
            "FA77FB4FA1E19E3EA7A9500DC6D9649C",
            RTMConfig.TRANS_LANGUAGE.en,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            // false
            true
        );
        SingleClientSend self = this;
        this._client.GetEvent().AddListener("login", (evd) => {
            if (evd.GetException() == null) {
                Debug.Log("test start!");
                self.StartThread();
            } else {
                Debug.Log(evd.GetException());
            }
        });
        this._client.GetEvent().AddListener("close", (evd) => {
            Debug.Log("closed!");
            self.StopThread();
        });
        this._client.GetEvent().AddListener("error", (evd) => {
            Debug.Log(evd.GetException());
        });
        RTMProcessor processor = this._client.GetProcessor();
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvMessage, (data) => {
            Debug.Log("[recvMessage]: " + Json.SerializeToString(data));
        });
        this._client.Login(null);
    }

    public void Update() {}

    public void StopTest() {
        this.StopThread();

        if (this._client != null) {
            this._client.Destroy();
        }
    }

    private Thread _thread;

    private void StartThread() {
        lock (send_locker) {
            if (send_locker.Status != 0) {
                return;
            }

            send_locker.Status = 1;
            this._thread = new Thread(new ThreadStart(SendMessage));
            this._thread.Start();
        }
    }

    private void StopThread() {
        lock (send_locker) {
            send_locker.Status = 0;
        }

        lock (inc_locker) {
            this._sendCount = 0;
            this._erroCount = 0;
            this._recvCount = 0;
            this._traceTimestamp = 0;
        }
    }

    private void SendMessage() {
        SingleClientSend self = this;

        try {
            while (true) {
                lock (send_locker) {
                    if (send_locker.Status == 0) {
                        return;
                    }

                    this.batch_count = (this.send_qps < 10) ? this.send_qps : this.batch_count;

                    for (int i = 0; i < this.batch_count; i++) {
                        this._client.SendMessage(778899, (byte) 8, "hello !", "", 0, 20 * 1000, (cbd) => {
                            if (cbd.GetException() != null) {
                                self.RevcInc(true);
                                // Debug.Log(cbd.GetException());
                            } else {
                                self.RevcInc(false);
                            }
                        });
                        this.SendInc();
                    }
                }

                if (this.send_qps > 0) {
                    Thread.Sleep((int) Math.Ceiling((1000f / this.send_qps) * this.batch_count));
                } else {
                    Thread.Sleep(1000);
                }
            }
        } catch (Exception ex) {
            Debug.Log(ex);
        }
    }

    private int _sendCount;
    private int _erroCount;
    private int _recvCount;
    private long _traceTimestamp;

    private object inc_locker = new object();

    private void SendInc() {
        lock (inc_locker) {
            this._sendCount++;

            if (this._traceTimestamp <= 0) {
                this._traceTimestamp = com.fpnn.FPManager.Instance.GetMilliTimestamp();
            }

            int interval = (int)((com.fpnn.FPManager.Instance.GetMilliTimestamp() - this._traceTimestamp) / 1000);

            if (interval >= this.trace_interval) {
                Debug.Log(
                    com.fpnn.FPManager.Instance.GetMilliTimestamp()
                    + ", trace interval: " + interval
                    + ", send count: " + this._sendCount
                    + ", err count: " + this._erroCount
                    + ", revc qps: " + (int)(this._recvCount / interval)
                );
                this._traceTimestamp = com.fpnn.FPManager.Instance.GetMilliTimestamp();
                this._sendCount = 0;
                this._erroCount = 0;
                this._recvCount = 0;
            }
        }
    }

    private void RevcInc(bool err) {
        lock (inc_locker) {
            if (err) {
                this._erroCount++;
            } else {
                this._recvCount++;
            }
        }
    }
}