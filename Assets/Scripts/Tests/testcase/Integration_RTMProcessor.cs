using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using GameDevWare.Serialization;
using com.fpnn;
using com.rtm;

public class Integration_RTMProcessor {

    private byte[] _bytes;
    private RTMProcessor _processor;
    private EventDelegate _eventDelegate;

    [SetUp]
    public void SetUp() {
        RTMRegistration.Register();
        this._processor = new RTMProcessor();
        Integration_RTMProcessor self = this;
        this._eventDelegate = (evd) => {
            self._processor.OnSecond(evd.GetTimestamp());
        };
        FPManager.Instance.AddSecond(this._eventDelegate);
        IDictionary<string, object> dict = new Dictionary<string, object>() {
            { "from", 123 },
            { "to", 456 },
            { "gid", 666 },
            { "rid", 777 },
            { "mtype", 8 },
            { "mid", 1570535270000003},
            { "msg", "hello"},
            { "attrs", "{}"},
            { "mtime", 157053527000},
        };

        try {
            using (MemoryStream outputStream = new MemoryStream()) {
                MsgPack.Serialize(dict, outputStream);
                outputStream.Seek(0, SeekOrigin.Begin);
                this._bytes = outputStream.ToArray();
            }
        } catch (Exception ex) {
            Debug.LogError(ex);
        }
    }

    [TearDown]
    public void TearDown() {
        if (this._eventDelegate != null) {
            FPManager.Instance.RemoveSecond(this._eventDelegate);
            this._eventDelegate = null;
        }

        if (this._processor != null) {
            this._processor.Destroy();
            this._processor = null;
        }
    } 

    [UnityTest]
    public IEnumerator Processor_AddPushService_Service() {
        int count = 0;
        this._processor.AddPushService("ping", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("ping");
        data.SetPayload(bytes);
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_AddPushService_Destroy_Service() {
        int count = 0;
        this._processor.AddPushService("ping", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("ping");
        data.SetPayload(bytes);
        this._processor.Destroy();
        this._processor.Service(data, (payload, exception) => {});

        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(0, count);
    }

    [UnityTest]
    public IEnumerator Processor_AddPushService_AddPushService_Service() {
        int count = 0;
        this._processor.AddPushService("ping", (dict) => {
            count++;
        });
        this._processor.AddPushService("ping", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("ping");
        data.SetPayload(bytes);
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_AddPushService_RemovePushService_Service() {
        int count = 0;
        this._processor.AddPushService("ping", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("ping");
        data.SetPayload(bytes);
        this._processor.RemovePushService("ping");
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(0, count);
    }

    [UnityTest]
    public IEnumerator Processor_RemovePushService_AddPushService_Service() {
        int count = 0;
        this._processor.RemovePushService("ping");
        this._processor.AddPushService("ping", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("ping");
        data.SetPayload(bytes);
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_Service_AddPushService() {
        int count = 0;
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("ping");
        data.SetPayload(bytes);
        this._processor.Service(data, (payload, exception) => {});
        this._processor.AddPushService("ping", (dict) => {
            count++;
        });
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(0, count);
    }

    [UnityTest]
    public IEnumerator Processor_AddPushService_Service_GetPingTimestamp() {
        int count = 0;
        this._processor.AddPushService("ping", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("ping");
        data.SetPayload(bytes);
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.Service(data, (payload, exception) => {});
        long ts = this._processor.GetPingTimestamp();
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreNotEqual(0, ts);
    }

    [UnityTest]
    public IEnumerator Processor_InitPingTimestamp_ClearPingTimestamp() {
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.InitPingTimestamp();
        Assert.AreNotEqual(0, this._processor.GetPingTimestamp());
        this._processor.ClearPingTimestamp();
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
    }

    [UnityTest]
    public IEnumerator Processor_InitPingTimestamp_InitPingTimestamp() {
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.InitPingTimestamp();
        long ts = this._processor.GetPingTimestamp();
        this._processor.InitPingTimestamp();
        Assert.AreEqual(ts, this._processor.GetPingTimestamp());
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
    }

    [UnityTest]
    public IEnumerator Processor_KickoutService() {
        int count = 0;
        this._processor.AddPushService("kickout", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("kickout");
        data.SetPayload(bytes);
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_KickoutroomService() {
        int count = 0;
        this._processor.AddPushService("kickoutroom", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("kickoutroom");
        data.SetPayload(bytes);
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_PushmsgService() {
        int count = 0;
        this._processor.AddPushService("pushmsg", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("pushmsg");
        data.SetPayload(this._bytes);
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_PushgroupmsgService() {
        int count = 0;
        this._processor.AddPushService("pushgroupmsg", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("pushgroupmsg");
        data.SetPayload(this._bytes);
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_PushroommsgService() {
        int count = 0;
        this._processor.AddPushService("pushroommsg", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("pushroommsg");
        data.SetPayload(this._bytes);
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_PushbroadcastmsgService() {
        int count = 0;
        this._processor.AddPushService("pushbroadcastmsg", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data = new FPData();
        data.SetMethod("pushbroadcastmsg");
        data.SetPayload(this._bytes);
        Assert.AreEqual(0, this._processor.GetPingTimestamp());
        this._processor.Service(data, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_PingService_PingService() {
        int count = 0;
        this._processor.AddPushService("ping", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data_1 = new FPData();
        data_1.SetMethod("ping");
        data_1.SetPayload(bytes);
        this._processor.Service(data_1, (payload, exception) => {});
        FPData data_2 = new FPData();
        data_2.SetMethod("ping");
        data_2.SetPayload(bytes);
        this._processor.Service(data_2, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(2, count);
    }

    [UnityTest]
    public IEnumerator Processor_PushmsgService_PushmsgService() {
        int count = 0;
        this._processor.AddPushService("pushmsg", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data_1 = new FPData();
        data_1.SetMethod("pushmsg");
        data_1.SetPayload(this._bytes);
        this._processor.Service(data_1, (payload, exception) => {});
        FPData data_2 = new FPData();
        data_2.SetMethod("pushmsg");
        data_2.SetPayload(this._bytes);
        this._processor.Service(data_2, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Processor_PushmsgService_Delay_PushmsgService() {
        int count = 0;
        this._processor.AddPushService("pushmsg", (dict) => {
            count++;
        });
        byte[] bytes = { 0x80 };
        FPData data_1 = new FPData();
        data_1.SetMethod("pushmsg");
        data_1.SetPayload(this._bytes);
        this._processor.Service(data_1, (payload, exception) => {});
        yield return new WaitForSeconds(6.0f);
        FPData data_2 = new FPData();
        data_2.SetMethod("pushmsg");
        data_2.SetPayload(this._bytes);
        this._processor.Service(data_2, (payload, exception) => {});
        this._processor.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(2, count);
    }
}
