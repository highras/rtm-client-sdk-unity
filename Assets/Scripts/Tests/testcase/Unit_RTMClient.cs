using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

using com.fpnn;
using com.rtm;

public class Unit_RTMClient {

    private string _dispatch = "52.83.245.22:13325";
    private int _pid = 11000001;
    private long _uid = 777779;
    private string _token = "B3D8012408C024293D0557FBE1CA0A88";

    [SetUp]
    public void SetUp() {}

    [TearDown]
    public void TearDown() {}


    /**
     *  RTMClient(string dispatch, int pid, long uid, string token, string version, string lang, IDictionary<string, string> attrs, bool reconnect, int timeout, bool debug)
     */
    [Test]
    public void Client_Debug() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NoDebug() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            false
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NullDispatch() {
        int count = 0;
        RTMClient client = new RTMClient(
            null,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_EmptyDispatch() {
        int count = 0;
        RTMClient client = new RTMClient(
            "",
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_ZeroPid() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            0,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NegativePid() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            -1,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_ZeroUid() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            0,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NegativeUid() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            -1,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NullToken() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            null,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_EmptyToken() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            "",
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NullVersion() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_EmptyVersion() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            "",
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_EmptyLang() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            "",
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NullLang() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null, 
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NullAttrs() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            null,
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_Reconnect() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NoReconnect() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            false,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_ZeroTimeout() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            0,
            true
        );
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_NegativeTimeout() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            -1,
            true
        );
        Assert.AreEqual(0, count);
    }


    /**
     *  GetProcessor()
     */
    [Test]
    public void Client_GetProcessor() {
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.IsNotNull(client.GetProcessor());
    }


    /**
     *  GetPackage()
     */
    [Test]
    public void Client_GetPackage() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.AreEqual(0, count);
    }


    /**
     *  SendQuest(string method, IDictionary<string, object> payload, CallbackDelegate callback, int timeout)
     */
    [Test]
    public void Client_SendQuest() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.SendQuest("sendmsg", new Dictionary<string, object>(), (cbd) => {}, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_SendQuest_NullMethod() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.SendQuest(null, new Dictionary<string, object>(), (cbd) => {}, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_SendQuest_EmptyMethod() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.SendQuest("", new Dictionary<string, object>(), (cbd) => {}, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_SendQuest_NullPayload() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.SendQuest("sendmsg", null, (cbd) => {}, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_SendQuest_NullCallback() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.SendQuest("sendmsg", new Dictionary<string, object>(), null, 20 * 1000);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_SendQuest_ZeroTimeout() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.SendQuest("sendmsg", new Dictionary<string, object>(), (cbd) => {}, 0);
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_SendQuest_NegativeTimeout() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.SendQuest("sendmsg", new Dictionary<string, object>(), (cbd) => {}, -1);
        Assert.AreEqual(0, count);
    }


    /**
     *  Destroy()
     */
    [Test]
    public void Client_Destroy() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.Destroy();
        Assert.AreEqual(0, count);
    }


    /**
     *  Login(string endpoint)
     */
    [Test]
    public void Client_Login_NullEndpoint() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.Login(null);
        client.Destroy();
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_Login_EmptyEndpoint() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.Login("");
        client.Destroy();
        Assert.AreEqual(0, count);
    }

    [Test]
    public void Client_Login_SimpleEndpoint() {
        int count = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.Login("52.83.245.22:13325");
        client.Destroy();
        Assert.AreEqual(0, count);
    }
}