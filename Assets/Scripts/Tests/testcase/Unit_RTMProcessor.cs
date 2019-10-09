using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

using com.fpnn;
using com.rtm;

public class Unit_RTMProcessor {

    private RTMProcessor _processor;

    [SetUp]
    public void SetUp() {
        this._processor = new RTMProcessor();
    }

    [TearDown]
    public void TearDown() {
        if (this._processor != null) {
            this._processor.Destroy();
            this._processor = null;
        }
    }


    /**
     *  Service(FPData data, AnswerDelegate answer)
     */
    [Test]
    public void Processor_Service_NullData() {
        int count = 0;
        this._processor.Service(null, (payload, exception) => {});
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_Service_EmptyData() {
        int count = 0;
        this._processor.Service(new FPData(), (payload, exception) => {});
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_Service_NullAnswer() {
        int count = 0;
        this._processor.Service(new FPData(), null);
        Assert.AreEqual(0, count);
    }


    /**
     *  HasPushService(string name)
     */
    [Test]
    public void Processor_HasPushService() {
        int count = 0;
        this._processor.HasPushService("ping");
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_HasPushService_NullName() {
        int count = 0;
        this._processor.HasPushService(null);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_HasPushService_EmptyName() {
        int count = 0;
        this._processor.HasPushService("");
        Assert.AreEqual(0, count);
    }


    /**
     *  AddPushService(string name, Action<IDictionary<string, object>> action)
     */
    [Test]
    public void Processor_AddPushService() {
        int count = 0;
        this._processor.AddPushService("ping", (data) => {});
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_AddPushService_NullName() {
        int count = 0;
        this._processor.AddPushService(null, (data) => {});
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_AddPushService_EmptyName() {
        int count = 0;
        this._processor.AddPushService("", (data) => {});
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_AddPushService_NullAction() {
        int count = 0;
        this._processor.AddPushService("ping", null);
        Assert.AreEqual(0, count);
    }


    /**
     *  RemovePushService(string name)
     */
    [Test]
    public void Processor_RemovePushService() {
        int count = 0;
        this._processor.RemovePushService("ping");
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_RemovePushService_NullName() {
        int count = 0;
        this._processor.RemovePushService(null);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_RemovePushService_EmptyName() {
        int count = 0;
        this._processor.RemovePushService("");
        Assert.AreEqual(0, count);
    }


    /**
     *  kickout(IDictionary<string, object> data)
     */
    [Test]
    public void Processor_kickout_NullData() {
        int count = 0;
        this._processor.kickout(null);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_kickout_EmptyData() {
        int count = 0;
        this._processor.kickout(new Dictionary<string, object>());
        Assert.AreEqual(0, count);
    }


    /**
     *  kickoutroom(IDictionary<string, object> data)
     */
    [Test]
    public void Processor_kickoutroom_NullData() {
        int count = 0;
        this._processor.kickoutroom(null);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_kickoutroom_EmptyData() {
        int count = 0;
        this._processor.kickoutroom(new Dictionary<string, object>());
        Assert.AreEqual(0, count);
    }


    /**
     *  pushmsg(IDictionary<string, object> data)
     */
    [Test]
    public void Processor_pushmsg_NullData() {
        int count = 0;
        this._processor.pushmsg(null);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_pushmsg_EmptyData() {
        int count = 0;
        this._processor.pushmsg(new Dictionary<string, object>());
        Assert.AreEqual(0, count);
    }


    /**
     *  pushgroupmsg(IDictionary<string, object> data)
     */
    [Test]
    public void Processor_pushgroupmsg_NullData() {
        int count = 0;
        this._processor.pushgroupmsg(null);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_pushgroupmsg_EmptyData() {
        int count = 0;
        this._processor.pushgroupmsg(new Dictionary<string, object>());
        Assert.AreEqual(0, count);
    }


    /**
     *  pushroommsg(IDictionary<string, object> data)
     */
    [Test]
    public void Processor_pushroommsg_NullData() {
        int count = 0;
        this._processor.pushroommsg(null);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_pushroommsg_EmptyData() {
        int count = 0;
        this._processor.pushroommsg(new Dictionary<string, object>());
        Assert.AreEqual(0, count);
    }


    /**
     *  pushbroadcastmsg(IDictionary<string, object> data)
     */
    [Test]
    public void Processor_pushbroadcastmsg_NullData() {
        int count = 0;
        this._processor.pushbroadcastmsg(null);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_pushbroadcastmsg_EmptyData() {
        int count = 0;
        this._processor.pushbroadcastmsg(new Dictionary<string, object>());
        Assert.AreEqual(0, count);
    }


    /**
     *  ping(IDictionary<string, object> data)
     */
    [Test]
    public void Processor_ping_NullData() {
        int count = 0;
        this._processor.ping(null);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_ping_EmptyData() {
        int count = 0;
        this._processor.ping(new Dictionary<string, object>());
        Assert.AreEqual(0, count);
    }


    /**
     *  GetPingTimestamp()
     */
    [Test]
    public void Processor_GetPingTimestamp() {
        int count = 0;
        this._processor.GetPingTimestamp();
        Assert.AreEqual(0, count);
    }


    /**
     *  ClearPingTimestamp()
     */
    [Test]
    public void Processor_ClearPingTimestamp() {
        int count = 0;
        this._processor.ClearPingTimestamp();
        Assert.AreEqual(0, count);
    }


    /**
     *  InitPingTimestamp()
     */
    [Test]
    public void Processor_InitPingTimestamp() {
        int count = 0;
        this._processor.InitPingTimestamp();
        Assert.AreEqual(0, count);
    }


    /**
     *  OnSecond(long timestamp)
     */
    [Test]
    public void Processor_OnSecond_ZeroTimestamp() {
        int count = 0;
        this._processor.OnSecond(0);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_OnSecond_NegativeTimestamp() {
        int count = 0;
        this._processor.OnSecond(-1);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Processor_OnSecond_SimpleTimestamp() {
        int count = 0;
        this._processor.OnSecond(1568889418000);
        Assert.AreEqual(0, count);
    }
}