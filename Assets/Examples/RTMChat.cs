using System.Threading;
using UnityEngine;
using com.fpnn.rtm;

class Chat : Main.ITestCase
{
    private static long peerUid = 12345678;
    private static long groupId = 223344;
    private static long roomId = 556677;

    private static string textMessage = "Hello, RTM!";

    private RTMClient client;

    public void Start(string endpoint, long pid, long uid, string token)
    {
        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
        {
            Debug.Log("User " + uid + " login RTM failed.");
            return;
        }

        SendP2PChatInAsync(client, peerUid);
        SendP2PChatInSync(client, peerUid);

        SendP2PCmdInAsync(client, peerUid);
        SendP2PCmdInSync(client, peerUid);

        SendGroupChatInAsync(client, groupId);
        SendGroupChatInSync(client, groupId);

        SendGroupCmdInAsync(client, groupId);
        SendGroupCmdInSync(client, groupId);

        if (EnterRoom(client, roomId))
        {
            SendRoomChatInAsync(client, roomId);
            SendRoomChatInSync(client, roomId);

            SendRoomCmdInAsync(client, roomId);
            SendRoomCmdInSync(client, roomId);
        }

        Debug.Log("Running for receiving server pushed chat & cmd &c audio if those are being demoed ...");
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

    static bool EnterRoom(RTMClient client, long roomId)
    {
        int errorCode = client.EnterRoom(roomId);
        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
        {
            Debug.Log("Enter room " + roomId + " in sync failed.");
            return false;
        }
        else
            return true;
    }

    //------------------------[ Chat Demo ]-------------------------//
    static void SendP2PChatInAsync(RTMClient client, long peerUid)
    {
        bool status = client.SendChat((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Send chat message to user " + peerUid + " in sync successed, mtime is " + mtime);
            else
                Debug.Log("Send chat message to user " + peerUid + " in sync failed, errorCode is " + errorCode);
        }, peerUid, textMessage);

        if (!status)
            Debug.Log("Perpare send chat message to user " + peerUid + " in async failed.");
        else
            Thread.Sleep(1000);     //-- Waiting callback desipay result info
    }

    static void SendP2PChatInSync(RTMClient client, long peerUid)
    {
        int errorCode = client.SendChat(out long mtime, peerUid, textMessage);

        if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Send chat message to user " + peerUid + " in sync successed, mtime is " + mtime);
        else
            Debug.Log("Send chat message to user " + peerUid + " in sync failed.");
    }

    static void SendGroupChatInAsync(RTMClient client, long groupId)
    {
        bool status = client.SendGroupChat((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Send chat message to group " + groupId + " in sync successed, mtime is " + mtime);
            else
                Debug.Log("Send chat message to group " + groupId + " in sync failed, errorCode is " + errorCode);
        }, groupId, textMessage);

        if (!status)
            Debug.Log("Perpare send chat message to group " + groupId + " in async failed.");
        else
            Thread.Sleep(1000);     //-- Waiting callback desipay result info
    }

    static void SendGroupChatInSync(RTMClient client, long groupId)
    {
        int errorCode = client.SendGroupChat(out long mtime, groupId, textMessage);

        if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Send chat message to group " + groupId + " in sync successed, mtime is " + mtime);
        else
            Debug.Log("Send chat message to group " + groupId + " in sync failed.");
    }

    static void SendRoomChatInAsync(RTMClient client, long roomId)
    {
        bool status = client.SendRoomChat((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Send chat message to room " + roomId + " in sync successed, mtime is " + mtime);
            else
                Debug.Log("Send chat message to room " + roomId + " in sync failed, errorCode is " + errorCode);
        }, roomId, textMessage);

        if (!status)
            Debug.Log("Perpare send chat message to room " + roomId + " in async failed.");
        else
            Thread.Sleep(1000);     //-- Waiting callback desipay result info
    }

    static void SendRoomChatInSync(RTMClient client, long roomId)
    {
        long mtime;
        int errorCode = client.SendRoomChat(out mtime, roomId, textMessage);

        if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Send chat message to room " + roomId + " in sync successed, mtime is " + mtime);
        else
            Debug.Log("Send chat message to room " + roomId + " in sync failed.");
    }

    //------------------------[ Cmd Demo ]-------------------------//
    static void SendP2PCmdInAsync(RTMClient client, long peerUid)
    {
        bool status = client.SendCmd((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Send cmd message to user " + peerUid + " in sync successed, mtime is " + mtime);
            else
                Debug.Log("Send cmd message to user " + peerUid + " in sync failed, errorCode is " + errorCode);
        }, peerUid, textMessage);

        if (!status)
            Debug.Log("Perpare send cmd message to user " + peerUid + " in async failed.");
        else
            Thread.Sleep(1000);     //-- Waiting callback desipay result info
    }

    static void SendP2PCmdInSync(RTMClient client, long peerUid)
    {
        int errorCode = client.SendCmd(out long mtime, peerUid, textMessage);

        if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Send cmd message to user " + peerUid + " in sync successed, mtime is " + mtime);
        else
            Debug.Log("Send cmd message to user " + peerUid + " in sync failed.");
    }

    static void SendGroupCmdInAsync(RTMClient client, long groupId)
    {
        bool status = client.SendGroupCmd((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Send cmd message to group " + groupId + " in sync successed, mtime is " + mtime);
            else
                Debug.Log("Send cmd message to group " + groupId + " in sync failed, errorCode is " + errorCode);
        }, groupId, textMessage);

        if (!status)
            Debug.Log("Perpare send cmd message to group " + groupId + " in async failed.");
        else
            Thread.Sleep(1000);     //-- Waiting callback desipay result info
    }

    static void SendGroupCmdInSync(RTMClient client, long groupId)
    {
        int errorCode = client.SendGroupCmd(out long mtime, groupId, textMessage);

        if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Send cmd message to group " + groupId + " in sync successed, mtime is " + mtime);
        else
            Debug.Log("Send cmd message to group " + groupId + " in sync failed.");
    }

    static void SendRoomCmdInAsync(RTMClient client, long roomId)
    {
        bool status = client.SendRoomCmd((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Send cmd message to room " + roomId + " in sync successed, mtime is " + mtime);
            else
                Debug.Log("Send cmd message to room " + roomId + " in sync failed, errorCode is " + errorCode);
        }, roomId, textMessage);

        if (!status)
            Debug.Log("Perpare cmd chat message to room " + roomId + " in async failed.");
        else
            Thread.Sleep(1000);     //-- Waiting callback desipay result info
    }

    static void SendRoomCmdInSync(RTMClient client, long roomId)
    {
        long mtime;
        int errorCode = client.SendRoomCmd(out mtime, roomId, textMessage);

        if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Send cmd message to room " + roomId + " in sync successed, mtime is " + mtime);
        else
            Debug.Log("Send cmd message to room " + roomId + " in sync failed.");
    }
}

