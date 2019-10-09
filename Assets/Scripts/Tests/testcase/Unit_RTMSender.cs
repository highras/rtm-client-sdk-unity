using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

using com.fpnn;
using com.rtm;

public class Unit_RTMSender {

    private FPClient _client;
    private RTMSender _sender;

    [SetUp]
    public void SetUp() {
        this._sender = new RTMSender();
        this._client = new FPClient("52.83.245.22", 13325, 20 * 1000);
    }

    [TearDown]
    public void TearDown() {
        if (this._sender != null) {
            this._sender.Destroy();
            this._sender = null;
        }

        if (this._client != null) {
            this._client.Close();
            this._client = null;
        }
    }

    /**
     *  AddQuest(FPClient client, FPData data, IDictionary<string, object> payload, CallbackDelegate callback, int timeout)
     */
    [Test]
    public void Sender_AddQuest_NullClient() {
        int count = 0;
        this._sender.AddQuest(null, new FPData(), new Dictionary<string, object>(), (cbd) => {}, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Sender_AddQuest_NullData() {
        int count = 0;
        this._sender.AddQuest(this._client, null, new Dictionary<string, object>(), (cbd) => {}, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Sender_AddQuest_EmptyData() {
        int count = 0;
        this._sender.AddQuest(this._client, new FPData(), new Dictionary<string, object>(), (cbd) => {}, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Sender_AddQuest_NullPayload() {
        int count = 0;
        this._sender.AddQuest(this._client, new FPData(), null, (cbd) => {}, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Sender_AddQuest_EmptyPayload() {
        int count = 0;
        this._sender.AddQuest(this._client, new FPData(), new Dictionary<string, object>(), (cbd) => {}, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Sender_AddQuest_NullCallback() {
        int count = 0;
        this._sender.AddQuest(this._client, new FPData(), new Dictionary<string, object>(), null, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Sender_AddQuest_ZeroTimeout() {
        int count = 0;
        this._sender.AddQuest(this._client, new FPData(), new Dictionary<string, object>(), (cbd) => {}, 0);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Sender_AddQuest_NegativeTimeout() {
        int count = 0;
        this._sender.AddQuest(this._client, new FPData(), new Dictionary<string, object>(), (cbd) => {}, -1);
        Assert.AreEqual(0, count);
    }
}