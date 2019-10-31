using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using GameDevWare.Serialization;

using com.fpnn;
using com.rtm;

using UnityEngine;

public class SingleClientConcurrency : Main.ITestCase {

    private class ThreadLocker {

        public int Status = 0;
    }

    private const int THREAD_COUNT = 20;

    private bool trace_log = true;

    private object self_locker = new object();
    private ThreadLocker thread_locker = new ThreadLocker();

    private RTMClient _client;

    public RTMClient GetClient() {
        return this._client;
    }

    /**
     *  单客户端实例并发脚本
     */
    public SingleClientConcurrency() {}

    private DateTime _startTime;
    private String _endpoint;

    public void StartTest(byte[] fileBytes) {
        SingleClientConcurrency self = this;
        this._client = new RTMClient(
            "52.83.245.22:13325",
            11000001,
            777779,
            "A4BF1F8755F86E96F65F07240AB5F6F9",
            RTMConfig.TRANS_LANGUAGE.en,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        this._client.GetEvent().AddListener("login", (evd) => {
            if (evd.GetException() != null) {
                Debug.LogError(evd.GetException());
                return;
            }

            lock (self_locker) {
                self._endpoint = Convert.ToString(evd.GetPayload());
                Debug.Log("[ CONCURRENCY ] login! endpoint: " + self._endpoint);
            }
        });
        this._client.GetEvent().AddListener("close", (evd) => {
            Debug.Log("[ CONCURRENCY ] closed!");
        });
        this._client.GetEvent().AddListener("error", (evd) => {
            Debug.LogError(evd.GetException());
        });
        this.StartThread();
    }

    public void Update() {}

    public void StopTest() {
        this.StopThread();

        if (this._client != null) {
            this._client.Destroy();
        }
    }

    private List<Thread> _threads = new List<Thread>(THREAD_COUNT);

    private void StartThread() {
        lock (thread_locker) {
            if (thread_locker.Status != 0) {
                return;
            }

            thread_locker.Status = 1;

            for (int i = THREAD_COUNT; i > 0; i--) {
                Thread t = new Thread(new ThreadStart(RandomAction));
                t.IsBackground = true;
                t.Start();
                this._threads.Add(t);
            }
        }

        if (trace_log) {
            Debug.Log("[ CONCURRENCY ] start thread, count: " + THREAD_COUNT);
        }
    }

    private void StopThread() {
        lock (thread_locker) {
            thread_locker.Status = 0;
            this._threads.Clear();
        }

        if (trace_log) {
            Debug.Log("[ CONCURRENCY ] stop thread");
        }
    }

    private System.Random _random = new System.Random();

    private void RandomAction() {
        try {
            while (true) {
                int action = 7;
                int sleep = 1000;

                lock (thread_locker) {
                    if (thread_locker.Status == 0) {
                        return;
                    }

                    action = this._random.Next(1, 5);
                    sleep = this._random.Next(6, 11) * 100;
                }

                switch (action) {
                    case 1:
                        this.Login();
                        break;

                    case 2:
                        this.LoginEndpoint();
                        break;

                    case 3:
                        this.SendMessage();
                        break;

                    case 4:
                        this.Close();
                        break;
                }

                Thread.Sleep(sleep);
            }
        } catch (Exception ex) {
            Debug.LogError(ex);
        }
    }

    // action 1
    private void Login() {
        if (trace_log) {
            Debug.Log("[ CONCURRENCY ] Login");
        }

        if (this._client != null) {
            this._client.Login(null);
        }
    }

    // action 2
    private void LoginEndpoint() {
        if (trace_log) {
            Debug.Log("[ CONCURRENCY ] LoginEndpoint");
        }

        String ep = null;

        lock (self_locker) {
            ep = this._endpoint;
        }

        if (this._client != null) {
            this._client.Login(ep);
        }
    }

    // action 3
    private void SendMessage() {
        if (trace_log) {
            Debug.Log("[ CONCURRENCY ] SendMessage");
        }

        if (this._client != null) {
            this._client.SendMessage(778899, (byte) 8, "hello", "", 0, 20 * 1000, (cbd) => {
                if (cbd.GetException() != null) {
                    Debug.LogError(cbd.GetException());
                    return;
                }
                Debug.Log("SendMessage success");
            });
        }
    }

    // action 4
    private void Close() {
        if (trace_log) {
            Debug.Log("[ CONCURRENCY ] Close");
        }

        if (this._client != null) {
            this._client.Close();
        }
    }
}