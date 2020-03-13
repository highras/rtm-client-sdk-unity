using System.Threading;
using UnityEngine;
using com.fpnn.rtm;

class Files : Main.ITestCase
{
    private static long peerUid = 12345678;
    private static long groupId = 223344;
    private static long roomId = 556677;

    private static byte fileMType = 50;
    private static string filename = "demo.bin";
    private static byte[] fileContent = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

    private RTMClient client;

    public void Start(string endpoint, long pid, long uid, string token)
    {
        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
        {
            Debug.Log("User " + uid + " login RTM failed.");
            return;
        }

        SendP2PFileInAsync(client, peerUid, fileMType);
        SendP2PFileInSync(client, peerUid, fileMType);

        SendGroupFileInAsync(client, groupId, fileMType);
        SendGroupFileInSync(client, groupId, fileMType);

        EnterRoom(client, roomId);

        SendRoomFileInAsync(client, roomId, fileMType);
        SendRoomFileInSync(client, roomId, fileMType);

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

    //--------------[ Send files Demo ]---------------------//
    static void SendP2PFileInAsync(RTMClient client, long peerUid, byte mtype)
    {
        bool status = client.SendFile((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Send file to user " + peerUid + " in async successed, mtime is " + mtime);
            else
                Debug.Log("Send text message to user " + peerUid + " in async failed, errorCode is " + errorCode);
        }, peerUid, mtype, fileContent, filename);

        if (!status)
            Debug.Log("Perpare send file to user " + peerUid + " in async failed.");
        else
            Thread.Sleep(3000);     //-- Waiting callback desipay result info
    }

    static void SendP2PFileInSync(RTMClient client, long peerUid, byte mtype)
    {
        int errorCode = client.SendFile(out long mtime, peerUid, mtype, fileContent, filename);

        if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Send file to user " + peerUid + " in sync successed, mtime is " + mtime);
        else
            Debug.Log("Send file to user " + peerUid + " in sync failed, error code " + errorCode);
    }

    static void SendGroupFileInAsync(RTMClient client, long groupId, byte mtype)
    {
        bool status = client.SendGroupFile((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Send file to group " + groupId + " in async successed, mtime is " + mtime);
            else
                Debug.Log("Send file to group " + groupId + " in async failed, errorCode is " + errorCode);
        }, groupId, mtype, fileContent, filename);

        if (!status)
            Debug.Log("Perpare send file to group " + groupId + " in async failed.");
        else
            Thread.Sleep(3000);     //-- Waiting callback desipay result info
    }

    static void SendGroupFileInSync(RTMClient client, long groupId, byte mtype)
    {
        int errorCode = client.SendGroupFile(out long mtime, groupId, mtype, fileContent, filename);

        if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Send file to group " + groupId + " in sync successed, mtime is " + mtime);
        else
            Debug.Log("Send file to group " + groupId + " in sync failed, error code " + errorCode);
    }

    static void SendRoomFileInAsync(RTMClient client, long roomId, byte mtype)
    {
        bool status = client.SendRoomFile((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Send file to room " + roomId + " in async successed, mtime is " + mtime);
            else
                Debug.Log("Send file to room " + roomId + " in async failed, errorCode is " + errorCode);
        }, roomId, mtype, fileContent, filename);

        if (!status)
            Debug.Log("Perpare send file to room " + roomId + " in async failed.");
        else
            Thread.Sleep(3000);     //-- Waiting callback desipay result info
    }

    static void SendRoomFileInSync(RTMClient client, long roomId, byte mtype)
    {
        int errorCode = client.SendRoomFile(out long mtime, roomId, mtype, fileContent, filename);

        if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Send file to room " + roomId + " in sync successed, mtime is " + mtime);
        else
            Debug.Log("Send file to room " + roomId + " in sync failed, error code " + errorCode);
    }
}

