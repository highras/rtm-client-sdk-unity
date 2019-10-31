using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

using GameDevWare.Serialization;
using com.fpnn;
using com.rtm;

public class Integration_RTMClient {

    private string _dispatch = "52.83.245.22:13325";
    private int _pid = 11000001;
    private long _uid = 777779;
    private string _token = "A21617E46173009FA8EC3F0CD0D4A1F6";

    [SetUp]
    public void SetUp() {
        RTMRegistration.Register();
    }

    [TearDown]
    public void TearDown() {}

    [UnityTest]
    public IEnumerator Client_GetProcessor() {
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.IsNotNull(client.GetProcessor());
        client.Destroy();
        yield return new WaitForSeconds(1.0f);
    }

    [UnityTest]
    public IEnumerator Client_GetPackage() {
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        Assert.IsNull(client.GetPackage());
        client.Destroy();
        yield return new WaitForSeconds(1.0f);
    }

    [UnityTest]
    public IEnumerator Client_Login_GetPackage() {
        int closeCount = 0;
        int loginCount = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.GetEvent().AddListener("login", (evd) => {
            loginCount++;
        });
        client.GetEvent().AddListener("close", (evd) => {
            closeCount++;
        });
        client.Login(null);
        yield return new WaitForSeconds(2.0f);
        Assert.IsNotNull(client.GetPackage());
        Assert.IsNotNull(client.GetProcessor());
        client.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, loginCount);
        Assert.AreEqual(1, closeCount);
    }

    [UnityTest]
    public IEnumerator Client_Login_Login() {
        int closeCount = 0;
        int loginCount = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.GetEvent().AddListener("login", (evd) => {
            loginCount++;
        });
        client.GetEvent().AddListener("close", (evd) => {
            closeCount++;
        });
        client.Login(null);
        client.Login(null);
        yield return new WaitForSeconds(2.0f);
        client.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, loginCount);
        Assert.AreEqual(1, closeCount);
    }

    [UnityTest]
    public IEnumerator Client_Login_Destroy_Login() {
        int closeCount = 0;
        int loginCount = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.GetEvent().AddListener("login", (evd) => {
            loginCount++;
        });
        client.GetEvent().AddListener("close", (evd) => {
            closeCount++;
        });
        client.Login(null);
        yield return new WaitForSeconds(2.0f);
        client.Destroy();
        yield return new WaitForSeconds(1.0f);
        client.Login(null);
        yield return new WaitForSeconds(2.0f);
        client.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, loginCount);
        Assert.AreEqual(1, closeCount);
    }

    [UnityTest]
    public IEnumerator Client_Login_Close_Login() {
        int closeCount = 0;
        int loginCount = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.GetEvent().AddListener("login", (evd) => {
            loginCount++;
        });
        client.GetEvent().AddListener("close", (evd) => {
            closeCount++;
        });
        client.Login(null);
        yield return new WaitForSeconds(2.0f);
        client.Close();
        yield return new WaitForSeconds(1.0f);
        client.Login(null);
        yield return new WaitForSeconds(2.0f);
        client.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(2, loginCount);
        Assert.AreEqual(2, closeCount);
    }

    [UnityTest]
    public IEnumerator Client_Close_Login() {
        int closeCount = 0;
        int loginCount = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.GetEvent().AddListener("login", (evd) => {
            loginCount++;
        });
        client.GetEvent().AddListener("close", (evd) => {
            closeCount++;
        });
        client.Close();
        yield return new WaitForSeconds(1.0f);
        client.Login(null);
        yield return new WaitForSeconds(2.0f);
        client.Destroy();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(1, loginCount);
        Assert.AreEqual(1, closeCount);
    }

    [UnityTest]
    public IEnumerator Client_Destroy_Login() {
        int closeCount = 0;
        int loginCount = 0;
        RTMClient client = new RTMClient(
            this._dispatch,
            this._pid,
            this._uid,
            this._token,
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client.GetEvent().AddListener("login", (evd) => {
            loginCount++;
        });
        client.GetEvent().AddListener("close", (evd) => {
            closeCount++;
        });
        client.Destroy();
        yield return new WaitForSeconds(1.0f);
        client.Login(null);
        yield return new WaitForSeconds(2.0f);
        Assert.AreEqual(0, loginCount);
        Assert.AreEqual(1, closeCount);
    }
}