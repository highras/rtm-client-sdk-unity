using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using com.fpnn.rtm;

class Rooms : Main.ITestCase
{
    private static long roomId = 556677;

    private RTMClient client;

    public void Start(string endpoint, long pid, long uid, string token)
    {
        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
        {
            Debug.Log("User " + uid + " login RTM failed.");
            return;
        }

        Debug.Log("======== enter room =========");
        EnterRoom(client, roomId);

        Debug.Log("======== get self rooms =========");
        GetSelfRooms(client);

        Debug.Log("======== leave room =========");
        LeaveRoom(client, roomId);

        Debug.Log("======== get self rooms =========");
        GetSelfRooms(client);


        Debug.Log("======== enter room =========");
        EnterRoom(client, roomId);

        Debug.Log("======== set room infos =========");

        SetRoomInfos(client, roomId, "This is public info", "This is private info");
        GetRoomInfos(client, roomId);

        Debug.Log("======== change room infos =========");

        SetRoomInfos(client, roomId, "", "This is private info");
        GetRoomInfos(client, roomId);

        Debug.Log("======== change room infos =========");

        SetRoomInfos(client, roomId, "This is public info", "");
        GetRoomInfos(client, roomId);

        Debug.Log("======== only change the private infos =========");

        SetRoomInfos(client, roomId, null, "balabala");
        GetRoomInfos(client, roomId);

        SetRoomInfos(client, roomId, "This is public info", "This is private info");
        client.Bye();

        Debug.Log("======== user relogin =========");

        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
            return;

        Debug.Log("======== enter room =========");
        EnterRoom(client, roomId);

        GetRoomInfos(client, roomId);

        GetRoomsPublicInfo(client, new HashSet<long>() { 556677, 778899, 445566, 334455, 1234 });

        Debug.Log("======== get room members immediately =========");

        GetRoomMemberCount(client, new HashSet<long>() { roomId, 778899, 445566, 334455, 1234 });
        GetRoomMembers(client, roomId);

        Debug.Log("======== get room members after 6 seconds =========");

        GetRoomMemberCount(client, new HashSet<long>() { roomId, 778899, 445566, 334455, 1234 });
        GetRoomMembers(client, roomId);

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

    static void EnterRoom(RTMClient client, long roomId)
    {
        int errorCode = client.EnterRoom(roomId);
        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Enter room " + roomId + " in sync failed.");
        else
            Debug.Log("Enter room " + roomId + " in sync successed.");
    }

    static void LeaveRoom(RTMClient client, long roomId)
    {
        int errorCode = client.LeaveRoom(roomId);
        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Leave room " + roomId + " in sync failed.");
        else
            Debug.Log("Leave room " + roomId + " in sync successed.");
    }

    static void GetSelfRooms(RTMClient client)
    {
        int errorCode = client.GetUserRooms(out HashSet<long> rids);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get user rooms in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get user rooms in sync successed, current I am in " + rids.Count + " rooms.");
            foreach (long rid in rids)
                Debug.Log("-- room id: " + rid);
        }
    }

    static void SetRoomInfos(RTMClient client, long roomId, string publicInfos, string privateInfos)
    {
        int errorCode = client.SetRoomInfo(roomId, publicInfos, privateInfos);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Set room infos in sync failed, error code is " + errorCode);
        else
            Debug.Log("Set room infos in sync successed.");
    }

    static void GetRoomInfos(RTMClient client, long roomId)
    {
        int errorCode = client.GetRoomInfo(out string publicInfos, out string privateInfos, roomId);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get room infos in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get room infos in sync successed.");
            Debug.Log("Public info: " + (publicInfos ?? "null"));
            Debug.Log("Private info: " + (privateInfos ?? "null"));
        }
    }

    static void GetRoomsPublicInfo(RTMClient client, HashSet<long> roomIds)
    {
        int errorCode = client.GetRoomsPublicInfo(out Dictionary<long, string> publicInfos, roomIds);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get rooms' info in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get rooms' info in sync success");
            foreach (var kvp in publicInfos)
                Debug.Log("-- room id: " + kvp.Key + " info: [" + kvp.Value + "]");
        }
    }

    static void GetRoomMembers(RTMClient client, long roomId)
    {
        int errorCode = client.GetRoomMembers(out HashSet<long> uids, roomId);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log($"Get room members in sync failed, error code is {errorCode}.");
        else
        {
            Debug.Log("Get room members in sync success");
            foreach (var uid in uids)
                Debug.Log($"-- room member: {uid}");
        }

        bool status = client.GetRoomMembers((HashSet<long> uids2, int errorCode2) => {
            if (errorCode2 == com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                Debug.Log("Get room members in async success");
                foreach (var uid in uids2)
                    Debug.Log($"-- room member: {uid}");
            }
            else
                Debug.Log($"Get room members in async failed, error code is {errorCode2}.");
        }, roomId);
        if (!status)
            Debug.Log("Launch room members in async failed.");

        Thread.Sleep(3 * 1000);
    }

    static void GetRoomMemberCount(RTMClient client, HashSet<long> roomIds)
    {
        int errorCode = client.GetRoomMemberCount(out Dictionary<long, int> counts, roomIds);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log($"Get room members count in sync failed, error code is {errorCode}.");
        else
        {
            Debug.Log("Get room members count in sync success");
            foreach (var kvp in counts)
                Debug.Log($"-- room: {kvp.Key}, count: {kvp.Value}");
        }

        bool status = client.GetRoomMemberCount((Dictionary<long, int> counts2, int errorCode2) => {
            if (errorCode2 == com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                Debug.Log("Get room members count in async success");
                foreach (var kvp in counts2)
                    Debug.Log($"-- room: {kvp.Key}, count: {kvp.Value}");
            }
            else
                Debug.Log($"Get room members count in async failed, error code is {errorCode2}.");
        }, roomIds);
        if (!status)
            Debug.Log("Launch room members count in async failed.");

        Thread.Sleep(3 * 1000);
    }
}