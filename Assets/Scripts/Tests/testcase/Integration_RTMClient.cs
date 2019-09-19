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
    private string _secret = "6212d7c7-adb7-46c0-bd82-2fed00ce90c9";
    private string _endpoint = "rum-nx-front.ifunplus.cn:13609";

    [SetUp]
    public void SetUp() {

        RTMRegistration.Register();
    }

    [TearDown]
    public void TearDown() {}

    [UnityTest]
    public IEnumerator Client_SendMessage_GetUnreadMessage_GetSession_GetSession() {

        RTMClient client_1 = new RTMClient(
            this._dispatch,
            this._pid,
            86333,
            "7B640064DB1E6735E7EE337951D4A8B9",
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );

        client_1.GetEvent().AddListener("error", (evd) => {
            Debug.LogError(evd.GetException());
        });
        client_1.GetEvent().AddListener("close", (evd) => {
            Debug.Log("client_1 closed!");
        });
        client_1.GetEvent().AddListener("login", (evd) => {
            if (evd.GetException() != null) {
                Debug.LogError(evd.GetException());
                return;
            }
            Debug.Log("client_1 connect!");
            client_1.SendMessage(86555, (byte) 8, "hello !", "", 0, 20 * 1000, (cbd) => {
                if (evd.GetException() != null) {
                    Debug.LogError(evd.GetException());
                    return;
                }
                Debug.Log("client_1 SendMessage: " + Json.SerializeToString(cbd.GetPayload()) + ", mid: " + cbd.GetMid());
                client_1.Destroy();
            });
        });

        client_1.Login(null);
        yield return new WaitForSeconds(10.0f);

        RTMClient client_2 = new RTMClient(
            this._dispatch,
            this._pid,
            86555,
            "24AEC1BE77200A35A06124B5E2B2A809",
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        client_2.GetEvent().AddListener("error", (evd) => {
            Debug.LogError(evd.GetException());
        });
        client_2.GetEvent().AddListener("close", (evd) => {
            Debug.Log("client_2 closed!");
        });
        client_2.GetEvent().AddListener("login", (evd) => {
            if (evd.GetException() != null) {
                Debug.LogError(evd.GetException());
                return;
            }
            Debug.Log("client_2 connect!");
            client_2.GetUnreadMessage(20 * 1000, (cbd) => {
                if (evd.GetException() != null) {
                    Debug.LogError(evd.GetException());
                    return;
                }
                Debug.Log("client_2 GetUnreadMessage: " + Json.SerializeToString(cbd.GetPayload()));
                // client_2.CleanUnreadMessage(20 * 1000, null);
            });
            client_2.GetSession(20 * 1000, (cbd) => {
                if (evd.GetException() != null) {
                    Debug.LogError(evd.GetException());
                    return;
                }
                Debug.Log("client_2 GetSession: " + Json.SerializeToString(cbd.GetPayload()));
            });
        });

        client_2.Login(null);
        yield return new WaitForSeconds(10.0f);

        RTMClient client_3 = new RTMClient(
            this._dispatch,
            this._pid,
            86333,
            "7B640064DB1E6735E7EE337951D4A8B9",
            null,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );

        client_3.GetEvent().AddListener("error", (evd) => {
            Debug.LogError(evd.GetException());
        });
        client_3.GetEvent().AddListener("close", (evd) => {
            Debug.Log("client_3 closed!");
        });
        client_3.GetEvent().AddListener("login", (evd) => {
            if (evd.GetException() != null) {
                Debug.LogError(evd.GetException());
                return;
            }
            Debug.Log("client_3 connect!");
            client_3.GetSession(20 * 1000, (cbd) => {
                if (evd.GetException() != null) {
                    Debug.LogError(evd.GetException());
                    return;
                }
                Debug.Log("client_3 GetSession: " + Json.SerializeToString(cbd.GetPayload()));
            });
        });
        
        client_3.Login(null);
        yield return new WaitForSeconds(10.0f);

        client_1.Destroy();
        client_2.Destroy();
        client_3.Destroy();
        yield return new WaitForSeconds(1.0f);
    }
}
