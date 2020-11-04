using System.Collections.Generic;
using UnityEngine;
using com.fpnn.rtm;

class Groups : Main.ITestCase
{
    private static long groupId = 223344;

    private RTMClient client;

    public void Start(string endpoint, long pid, long uid, string token)
    {
        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
        {
            Debug.Log("User " + uid + " login RTM failed.");
            return;
        }

        Debug.Log("======== get group members =========");
        GetGroupMembers(client, groupId);

        Debug.Log("======== add group members =========");
        AddGroupMembers(client, groupId, new HashSet<long>() { 99688868, 99688878, 99688888 });

        System.Threading.Thread.Sleep(1500);   //-- Wait for server sync action.

        Debug.Log("======== get group members =========");
        GetGroupMembers(client, groupId);

        Debug.Log("======== delete group members =========");
        DeleteGroupMembers(client, groupId, new HashSet<long>() { 99688878 });

        System.Threading.Thread.Sleep(1500);   //-- Wait for server sync action.

        Debug.Log("======== get group members =========");
        GetGroupMembers(client, groupId);


        Debug.Log("======== get self groups =========");
        GetSelfGroups(client);


        Debug.Log("======== set group infos =========");

        SetGroupInfos(client, groupId, "This is public info", "This is private info");
        GetGroupInfos(client, groupId);

        Debug.Log("======== change group infos =========");

        SetGroupInfos(client, groupId, "", "This is private info");
        GetGroupInfos(client, groupId);

        Debug.Log("======== change group infos =========");

        SetGroupInfos(client, groupId, "This is public info", "");
        GetGroupInfos(client, groupId);

        Debug.Log("======== only change the private infos =========");

        SetGroupInfos(client, groupId, null, "balabala");
        GetGroupInfos(client, groupId);

        SetGroupInfos(client, groupId, "This is public info", "This is private info");
        client.Bye();

        Debug.Log("======== user relogin =========");

        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
            return;

        GetGroupInfos(client, groupId);

        GetGroupsPublicInfo(client, new HashSet<long>() { 223344, 334455, 445566, 667788, 778899 });

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

    static void AddGroupMembers(RTMClient client, long groupId, HashSet<long> uids)
    {
        int errorCode = client.AddGroupMembers(groupId, uids);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Add group members in sync failed, error code is " + errorCode);
        else
            Debug.Log("Add group members in sync successed.");
    }

    static void DeleteGroupMembers(RTMClient client, long groupId, HashSet<long> uids)
    {
        int errorCode = client.DeleteGroupMembers(groupId, uids);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Delete group members in sync failed, error code is " + errorCode);
        else
            Debug.Log("Delete group members in sync successed.");
    }

    static void GetGroupMembers(RTMClient client, long groupId)
    {
        int errorCode = client.GetGroupMembers(out HashSet<long> uids, groupId);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get group members in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get group members in sync successed, current has " + uids.Count + " members.");
            foreach (long uid in uids)
                Debug.Log("-- member uid: " + uid);
        }
    }

    static void GetSelfGroups(RTMClient client)
    {
        int errorCode = client.GetUserGroups(out HashSet<long> gids);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get user groups in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get user groups in sync successed, current I am in " + gids.Count + " groups.");
            foreach (long gid in gids)
                Debug.Log("-- group id: " + gid);
        }
    }

    static void SetGroupInfos(RTMClient client, long groupId, string publicInfos, string privateInfos)
    {
        int errorCode = client.SetGroupInfo(groupId, publicInfos, privateInfos);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Set group infos in sync failed, error code is " + errorCode);
        else
            Debug.Log("Set group infos in sync successed.");
    }

    static void GetGroupInfos(RTMClient client, long groupId)
    {
        int errorCode = client.GetGroupInfo(out string publicInfos, out string privateInfos, groupId);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get group infos in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get group infos in sync successed.");
            Debug.Log("Public info: " + (publicInfos ?? "null"));
            Debug.Log("Private info: " + (privateInfos ?? "null"));
        }
    }

    static void GetGroupsPublicInfo(RTMClient client, HashSet<long> groupIds)
    {
        int errorCode = client.GetGroupsPublicInfo(out Dictionary<string, string> publicInfos, groupIds);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get groups' info in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get groups' info in sync success");
            foreach (var kvp in publicInfos)
                Debug.Log("-- group id: " + kvp.Key + " info: [" + kvp.Value + "]");
        }
    }
}