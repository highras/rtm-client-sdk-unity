using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

using GameDevWare.Serialization;
using com.fpnn;
using com.rtm;

public class Integration_RTMSender {

    private FPClient _client;
    private RTMSender _sender;
    private FPData _data;
    private IDictionary<string, object> _payload;

    [SetUp]
    public void SetUp() {
        RTMRegistration.Register();
        this._sender = new RTMSender();
        this._client = new FPClient("52.83.245.22", 13325, 20 * 1000);
        this._payload = new Dictionary<string, object>() {
            { "pid", 11000001 },
            { "uid", 777779 },
            { "what", "rtmGated" },
            { "addrType", "ipv4" },
            { "version", null }
        };
        this._data = new FPData();
        this._data.SetFlag(0x1);
        this._data.SetMtype(0x1);
        this._data.SetMethod("which");
    }

    [TearDown]
    public void TearDown() {
        if (this._sender != null) {
            this._sender.Destroy();
            this._sender = null;
        }
    }

    [UnityTest]
    public IEnumerator Sender_AddQuest() {
        int count = 0;
        FPClient client = new FPClient("52.83.245.22", 13325, 20 * 1000);
        client.Connect();
        yield return new WaitForSeconds(1.0f);
        this._sender.AddQuest(client, this._data, this._payload, (cbd) => {
            count++;
        }, 5 * 1000);
        yield return new WaitForSeconds(2.0f);
        this._sender.Destroy();
        client.Close();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Sender_AddQuest_Destroy() {
        int count = 0;
        FPClient client = new FPClient("52.83.245.22", 13325, 20 * 1000);
        client.Connect();
        yield return new WaitForSeconds(1.0f);
        this._sender.AddQuest(client, this._data, this._payload, (cbd) => {
            count++;
        }, 5 * 1000);
        this._sender.Destroy();
        yield return new WaitForSeconds(1.0f);
        client.Close();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Sender_AddQuest_Destroy_Destroy() {
        int count = 0;
        FPClient client = new FPClient("52.83.245.22", 13325, 20 * 1000);
        client.Connect();
        yield return new WaitForSeconds(1.0f);
        this._sender.AddQuest(client, this._data, this._payload, (cbd) => {
            count++;
        }, 5 * 1000);
        this._sender.Destroy();
        this._sender.Destroy();
        yield return new WaitForSeconds(1.0f);
        client.Close();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }

    [UnityTest]
    public IEnumerator Sender_AddQuest_AddQuest() {
        int count = 0;
        FPClient client = new FPClient("52.83.245.22", 13325, 20 * 1000);
        client.Connect();
        yield return new WaitForSeconds(1.0f);
        FPData data = new FPData();
        data.SetFlag(0x1);
        data.SetMtype(0x1);
        data.SetMethod("which");
        this._sender.AddQuest(client, data, this._payload, (cbd) => {
            count++;
        }, 5 * 1000);
        this._sender.AddQuest(client, this._data, this._payload, (cbd) => {
            count++;
        }, 5 * 1000);
        yield return new WaitForSeconds(1.0f);
        this._sender.Destroy();
        client.Close();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(2, count);
    }

    [UnityTest]
    public IEnumerator Sender_AddQuest_Destroy_AddQuest() {
        int count = 0;
        FPClient client = new FPClient("52.83.245.22", 13325, 20 * 1000);
        client.Connect();
        yield return new WaitForSeconds(1.0f);
        FPData data = new FPData();
        data.SetFlag(0x1);
        data.SetMtype(0x1);
        data.SetMethod("which");
        this._sender.AddQuest(client, data, this._payload, (cbd) => {
            count++;
        }, 5 * 1000);
        this._sender.Destroy();
        this._sender.AddQuest(client, this._data, this._payload, (cbd) => {
            count++;
        }, 5 * 1000);
        yield return new WaitForSeconds(1.0f);
        this._sender.Destroy();
        client.Close();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, count);
    }
}
