using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using GameDevWare.Serialization;
using UnityEngine;

using com.fpnn;
using com.rtm;

public class TestCase : Main.ITestCase {

    private class SendLocker {

        public int Status = 0;
    }

    private int _sleepCount;
    private byte[] _fileBytes;
    private RTMClient _client;
    private SendLocker send_locker = new SendLocker();

    private long _uid;
    private String _token;

    public TestCase(long uid, string token) {
        this._uid = uid;
        this._token = token;
    }

    public void StartTest(byte[] fileBytes) {
        this._fileBytes = fileBytes;
        this._client = new RTMClient(
            "52.82.27.68:13325",
            11000001,
            this._uid,
            this._token,
            // "rtm-intl-frontgate.funplus.com:13325",
            // 11000002,
            // 777779,
            // "BE3732174850E479209443BCCDF4747D",
            // "52.83.245.22:13325",
            // 1000012,
            // 654321,
            // "63B3F146B2A1DA8660B167D26A610C0D",
            // "rtm-intl-frontgate.funplus.com:13325",
            // 11000001,
            // 777779,
            // "12861748F2D641907D181D1CDB6DF174",
            RTMConfig.TRANS_LANGUAGE.en,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        RTMProcessor processor = this._client.GetProcessor();
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvMessage, (data) => {
            Debug.Log("[recvMessage]: " + Json.SerializeToString(data));
            Debug.Log("[recvMessage]: " + data["msg"]);
        });
        TestCase self = this;
        this._client.GetEvent().AddListener("login", (evd) => {
            Exception ex = evd.GetException();

            if (ex != null) {
                Debug.Log("TestCase connect err: " + ex.Message);
            } else {
                Debug.Log("TestCase connect! gate: " + evd.GetPayload());
                self.StartThread();
            }
        });
        this._client.GetEvent().AddListener("close", (evd) => {
            Debug.Log("TestCase closed!");
        });
        this._client.GetEvent().AddListener("error", (evd) => {
            Debug.Log("TestCase error: " + evd.GetException());
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
            this._thread = new Thread(new ThreadStart(BeginTest));
            this._thread.Start();
        }
    }

    private void StopThread() {
        lock (send_locker) {
            send_locker.Status = 0;
        }
    }

    private void BeginTest() {
        try {
            this.DoTest();
        } catch (Exception ex) {
            Debug.Log(ex);
        }
    }

    private void DoTest() {
        long to = 778899;
        long fuid = 778898;
        long gid = 999;
        long rid = 666;
        IDictionary<String, String>  attrs = new Dictionary<String, String>();
        attrs.Add("user1", "test user1 attrs");
        List<long> tos = new List<long>();
        tos.Add((long)654321);
        tos.Add(fuid);
        tos.Add(to);
        List<long> friends = new List<long>();
        friends.Add(fuid);
        friends.Add(to);
        int timeout = 20 * 1000;
        int sleep = 1000;
        Debug.Log("[TEST] begin");
        this.ThreadSleep(sleep);
        //rtmGate (1c)
        //---------------------------------Kickout--------------------------------------
        this._client.Kickout("", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] Kickout: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] Kickout: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (1d)
        //---------------------------------AddAttrs--------------------------------------
        this._client.AddAttrs(attrs, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] AddAttrs: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] AddAttrs: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (1e)
        //---------------------------------GetAttrs--------------------------------------
        this._client.GetAttrs(timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetAttrs: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetAttrs: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (1f)
        //---------------------------------AddDebugLog--------------------------------------
        this._client.AddDebugLog("msg", "attrs", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] AddDebugLog: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] AddDebugLog: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (1g)
        //---------------------------------AddDevice--------------------------------------
        this._client.AddDevice("app-info", "device-token", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] AddDevice: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] AddDevice: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (1h)
        //---------------------------------RemoveDevice--------------------------------------
        this._client.RemoveDevice("device-token", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] RemoveDevice: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] RemoveDevice: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2a)
        //---------------------------------SendMessage--------------------------------------
        this._client.SendMessage(to, (byte) 8, "hello !", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendMessage: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
            } else {
                Debug.Log("[ERR] SendMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2b)
        //---------------------------------SendGroupMessage--------------------------------------
        this._client.SendGroupMessage(gid, (byte) 8, "hello !", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendGroupMessage: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
            } else {
                Debug.Log("[ERR] SendGroupMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2c)
        //---------------------------------SendRoomMessage--------------------------------------
        this._client.SendRoomMessage(rid, (byte) 8, "hello !", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendRoomMessage: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
            } else {
                Debug.Log("[ERR] SendRoomMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2d)
        //---------------------------------GetGroupMessage--------------------------------------
        this._client.GetGroupMessage(gid, true, 10, 0, 0, 0, new List<Byte> {8}, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetGroupMessage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetGroupMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2e)
        //---------------------------------GetRoomMessage--------------------------------------
        this._client.GetRoomMessage(rid, true, 10, 0, 0, 0, new List<Byte> {8}, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetRoomMessage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetRoomMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2f)
        //---------------------------------GetBroadcastMessage--------------------------------------
        this._client.GetBroadcastMessage(true, 10, 0, 0, 0, new List<Byte> {8}, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetBroadcastMessage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetBroadcastMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2g)
        //---------------------------------GetP2PMessage--------------------------------------
        this._client.GetP2PMessage(to, true, 10, 0, 0, 0, new List<Byte> {8}, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetP2PMessage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetP2PMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2h)
        //---------------------------------DeleteMessage--------------------------------------
        this._client.DeleteMessage(0, to, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DeleteMessage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DeleteMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2h')
        //---------------------------------DeleteGroupMessage--------------------------------------
        this._client.DeleteGroupMessage(0, gid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DeleteGroupMessage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DeleteGroupMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (2h'')
        //---------------------------------DeleteRoomMessage--------------------------------------
        this._client.DeleteRoomMessage(0, rid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DeleteRoomMessage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DeleteRoomMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3a)
        //---------------------------------SendChat--------------------------------------
        this._client.SendChat(to, "hello !", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendChat: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
            } else {
                Debug.Log("[ERR] SendChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3a'')
        //---------------------------------SendCmd--------------------------------------
        this._client.SendCmd(to, "friends_invite", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendCmd: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
            } else {
                Debug.Log("[ERR] SendCmd: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3b)
        //---------------------------------SendGroupChat--------------------------------------
        this._client.SendGroupChat(gid, "hello !", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendGroupChat: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
            } else {
                Debug.Log("[ERR] SendGroupChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3b'')
        //---------------------------------SendGroupCmd--------------------------------------
        this._client.SendGroupCmd(gid, "group_friends_invite", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendGroupCmd: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
            } else {
                Debug.Log("[ERR] SendGroupCmd: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3c)
        //---------------------------------SendRoomChat--------------------------------------
        this._client.SendRoomChat(rid, "hello !", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendRoomChat: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
            } else {
                Debug.Log("[ERR] SendRoomChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3c'')
        //---------------------------------SendRoomCmd--------------------------------------
        this._client.SendRoomCmd(rid, "room_friends_invite", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendRoomCmd: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
            } else {
                Debug.Log("[ERR] SendRoomCmd: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3d)
        //---------------------------------GetGroupChat--------------------------------------
        this._client.GetGroupChat(gid, true, 10, 0, 0, 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetGroupChat: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetGroupChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3e)
        //---------------------------------GetRoomChat--------------------------------------
        this._client.GetRoomChat(rid, true, 10, 0, 0, 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetRoomChat: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetRoomChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3f)
        //---------------------------------GetBroadcastChat--------------------------------------
        this._client.GetBroadcastChat(true, 10, 0, 0, 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetBroadcastChat: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetBroadcastChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3g)
        //---------------------------------GetP2PChat--------------------------------------
        this._client.GetP2PChat(to, true, 10, 0, 0, 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetP2PChat: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetP2PChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3h)
        //---------------------------------GetUnreadMessage--------------------------------------
        this._client.GetUnreadMessage(true, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetUnreadMessage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetUnreadMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3i)
        //---------------------------------CleanUnreadMessage--------------------------------------
        this._client.CleanUnreadMessage(timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] CleanUnreadMessage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] CleanUnreadMessage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3j)
        //---------------------------------GetSession--------------------------------------
        this._client.GetSession(timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetSession: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetSession: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3k)
        //---------------------------------DeleteChat--------------------------------------
        this._client.DeleteChat(0, to, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DeleteChat: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DeleteChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3k')
        //---------------------------------DeleteGroupChat--------------------------------------
        this._client.DeleteGroupChat(0, gid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DeleteGroupChat: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DeleteGroupChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3k'')
        //---------------------------------DeleteRoomChat--------------------------------------
        this._client.DeleteRoomChat(0, rid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DeleteRoomChat: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DeleteRoomChat: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3l)
        //---------------------------------SetTranslationLanguage--------------------------------------
        this._client.SetTranslationLanguage(RTMConfig.TRANS_LANGUAGE.en, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SetTranslationLanguage: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] SetTranslationLanguage: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3m)
        //---------------------------------Translate--------------------------------------
        this._client.Translate("点数优惠", RTMConfig.TRANS_LANGUAGE.zh_cn, RTMConfig.TRANS_LANGUAGE.en, "chat", "censor", false, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] Translate: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] Translate: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3n)
        //---------------------------------Profanity--------------------------------------
        this._client.Profanity("点数优惠", true, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] Profanity: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] Profanity: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (3o)
        //---------------------------------Transcribe--------------------------------------
        // this.ThreadSleep(sleep);
        
        //rtmGate (4a)
        //---------------------------------FileToken--------------------------------------
        this._client.FileToken("sendfile", to, 0, 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] FileToken: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] FileToken: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (5a)
        //---------------------------------GetOnlineUsers--------------------------------------
        this._client.GetOnlineUsers(tos, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetOnlineUsers: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetOnlineUsers: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (5b)
        //---------------------------------SetUserInfo--------------------------------------
        this._client.SetUserInfo("oinfo", "pinfo", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SetUserInfo: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] SetUserInfo: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (5c)
        //---------------------------------GetUserInfo--------------------------------------
        this._client.GetUserInfo(timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetUserInfo: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetUserInfo: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (5d)
        //---------------------------------GetUserOpenInfo--------------------------------------
        this._client.GetUserOpenInfo(tos, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetUserOpenInfo: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetUserOpenInfo: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (6a)
        //---------------------------------AddFriends--------------------------------------
        this._client.AddFriends(friends, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] AddFriends: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] AddFriends: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (6b)
        //---------------------------------DeleteFriends--------------------------------------
        this._client.DeleteFriends(friends, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DeleteFriends: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DeleteFriends: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (6c)
        //---------------------------------GetFriends--------------------------------------
        this._client.GetFriends(timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetFriends: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetFriends: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (7a)
        //---------------------------------AddGroupMembers--------------------------------------
        this._client.AddGroupMembers(gid, tos, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] AddGroupMembers: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] AddGroupMembers: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (7b)
        //---------------------------------DeleteGroupMembers--------------------------------------
        this._client.DeleteGroupMembers(rid, tos, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DeleteGroupMembers: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DeleteGroupMembers: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (7c)
        //---------------------------------GetGroupMembers--------------------------------------
        this._client.GetGroupMembers(gid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetGroupMembers: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetGroupMembers: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (7d)
        //---------------------------------GetUserGroups--------------------------------------
        this._client.GetUserGroups(timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetUserGroups: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetUserGroups: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (7e)
        //---------------------------------SetGroupInfo--------------------------------------
        this._client.SetGroupInfo(gid, "oinfo", "pinfo", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SetGroupInfo: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] SetGroupInfo: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (7f)
        //---------------------------------GetGroupInfo--------------------------------------
        this._client.GetGroupInfo(gid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetGroupInfo: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetGroupInfo: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (7g)
        //---------------------------------GetGroupOpenInfo--------------------------------------
        this._client.GetGroupOpenInfo(gid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetGroupOpenInfo: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetGroupOpenInfo: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (8a)
        //---------------------------------EnterRoom--------------------------------------
        this._client.EnterRoom(rid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] EnterRoom: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] EnterRoom: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (8b)
        //---------------------------------LeaveRoom--------------------------------------
        this._client.LeaveRoom(rid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] LeaveRoom: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] LeaveRoom: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (8c)
        //---------------------------------GetUserRooms--------------------------------------
        this._client.GetUserRooms(timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetUserRooms: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetUserRooms: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (8d)
        //---------------------------------SetRoomInfo--------------------------------------
        this._client.SetRoomInfo(rid, "oinfo", "pinfo", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SetRoomInfo: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] SetRoomInfo: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (8e)
        //---------------------------------GetRoomInfo--------------------------------------
        this._client.GetRoomInfo(rid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetRoomInfo: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetRoomInfo: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (8f)
        //---------------------------------GetRoomOpenInfo--------------------------------------
        this._client.GetRoomOpenInfo(rid, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] GetRoomOpenInfo: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] GetRoomOpenInfo: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (9b)
        //---------------------------------DataSet--------------------------------------
        this._client.DataSet("db-test-key", "db-test-value", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DataSet: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DataSet: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (9a)
        //---------------------------------DataGet--------------------------------------
        this._client.DataGet("db-test-key", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DataGet: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DataGet: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //rtmGate (9c)
        //---------------------------------DataGet--------------------------------------
        this._client.DataDelete("db-test-key", timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] DataDelete: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] DataDelete: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep);
        //fileGate (1)
        //---------------------------------SendFile--------------------------------------
        this._client.SendFile((byte)50, to, this._fileBytes, null, null, 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendFile: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] SendFile: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep * 5);
        //fileGate (3)
        //---------------------------------SendGroupFile--------------------------------------
        this._client.SendGroupFile((byte)50, gid, this._fileBytes, "", "", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendGroupFile: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] SendGroupFile: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep * 5);
        //fileGate (4)
        //---------------------------------SendRoomFile--------------------------------------
        this._client.SendRoomFile((byte)50, rid, this._fileBytes, "jpg", "pic", 0, timeout, (cbd) => {
            object obj = cbd.GetPayload();

            if (obj != null) {
                Debug.Log("[DATA] SendRoomFile: " + Json.SerializeToString(obj));
            } else {
                Debug.Log("[ERR] SendRoomFile: " + cbd.GetException().Message);
            }
        });
        this.ThreadSleep(sleep * 5);
        //rtmGate (1b)
        //---------------------------------Close--------------------------------------
        this._client.Close();
        Debug.Log("[TEST] end@" + (this._sleepCount - 1));
    }

    private void ThreadSleep(int ms) {
        lock (send_locker) {
            if (send_locker.Status == 0) {
                return;
            }
        }

        System.Threading.Thread.Sleep(ms);
        this._sleepCount++;
    }
}