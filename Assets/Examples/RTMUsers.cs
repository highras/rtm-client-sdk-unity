using System.Collections.Generic;
using UnityEngine;
using com.fpnn.rtm;

class Users : Main.ITestCase
{
    private RTMClient client;

    public void Start(string endpoint, long pid, long uid, string token)
    {
        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
        {
            Debug.Log("User " + uid + " login RTM failed.");
            return;
        }

        GetOnlineUsers(client, new HashSet<long>() { 99688848, 123456, 234567, 345678, 456789 });

        SetUserInfos(client, "This is public info", "This is private info");
        GetUserInfos(client);

        Debug.Log("======== =========");

        SetUserInfos(client, "", "This is private info");
        GetUserInfos(client);

        Debug.Log("======== =========");

        SetUserInfos(client, "This is public info", "");
        GetUserInfos(client);

        Debug.Log("======== only change the private infos =========");

        SetUserInfos(client, null, "balabala");
        GetUserInfos(client);

        SetUserInfos(client, "This is public info", "This is private info");
        client.Bye();

        Debug.Log("======== user relogin =========");

        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
            return;

        GetUserInfos(client);

        GetUsersInfos(client, new HashSet<long>() { 99688848, 123456, 234567, 345678, 456789 });

        Debug.Log("============== Demo completed ================");
    }

    public void Stop() { }

    static RTMClient LoginRTM(string rtmEndpoint, long pid, long uid, string token)
    {
        RTMClient client = new RTMClient(rtmEndpoint, pid, uid, new example.common.RTMExampleQuestProcessor());

        int errorCode = client.Login(out bool ok, token);
        if (ok)
        {
            Debug.Log("RTM login success.");
            return client;
        }
        else
        {
            Debug.Log("RTM login failed, error code: " + errorCode);
            return null;
        }
    }

    static void GetOnlineUsers(RTMClient client, HashSet<long> willCheckedUids)
    {
        int errorCode = client.GetOnlineUsers(out HashSet<long> onlineUids, willCheckedUids);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get online users in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get online users in sync success");
            Debug.Log("Only " + onlineUids.Count + " user(s) online in total " + willCheckedUids.Count + " checked users");
            foreach (long uid in onlineUids)
                Debug.Log("-- online uid: " + uid);
        }
    }

    static void SetUserInfos(RTMClient client, string publicInfos, string privateInfos)
    {
        int errorCode = client.SetUserInfo(publicInfos, privateInfos);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Set user infos in sync failed, error code is " + errorCode);
        else
            Debug.Log("Set user infos in sync successed.");
    }

    static void GetUserInfos(RTMClient client)
    {
        int errorCode = client.GetUserInfo(out string publicInfos, out string privateInfos);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get user infos in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get user infos in sync successed.");
            Debug.Log("Public info: " + (publicInfos ?? "null"));
            Debug.Log("Private info: " + (privateInfos ?? "null"));
        }
    }

    static void GetUsersInfos(RTMClient client, HashSet<long> uids)
    {
        int errorCode = client.GetUserPublicInfo(out Dictionary<long, string> publicInfos, uids);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get users' info in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get users' info in sync success");
            foreach (var kvp in publicInfos)
                Debug.Log("-- uid: " + kvp.Key + " info: [" + kvp.Value + "]");
        }
    }
}