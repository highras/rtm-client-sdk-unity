using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using com.fpnn.rtm;

class Histories : Main.ITestCase
{
    private static long peerUid = 12345678;
    private static long groupId = 223344;
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

        EnterRoom(client, roomId);

        int fetchCount = 100;
        Debug.Log("\n================[ Get P2P Message " + fetchCount + " items ]==================");
        GetP2PMessageInSync(client, peerUid, fetchCount);

        Debug.Log("\n================[ Get Group Message " + fetchCount + " items ]==================");
        GetGroupMessageInSync(client, groupId, fetchCount);

        Debug.Log("\n================[ Get Room Message " + fetchCount + " items ]==================");
        GetRoomMessageInSync(client, roomId, fetchCount);

        Debug.Log("\n================[ Get Broadcast Message " + fetchCount + " items ]==================");
        GetBroadcastMessageInSync(client, fetchCount);


        Debug.Log("\n================[ Get P2P Chat " + fetchCount + " items ]==================");
        GetP2PChatInSync(client, peerUid, fetchCount);

        Debug.Log("\n================[ Get Group Chat " + fetchCount + " items in sync mode ]==================");
        GetGroupChatInSync(client, groupId, fetchCount);

        Debug.Log("\n================[ Get Group Chat " + fetchCount + " items in async mode ]==================");
        GetGroupChatInASync(client, groupId, fetchCount);
        Thread.Sleep(3000);

        Debug.Log("\n================[ Get Room Chat " + fetchCount + " items ]==================");
        GetRoomChatInSync(client, roomId, fetchCount);

        Debug.Log("\n================[ Get Broadcast Chat " + fetchCount + " items ]==================");
        GetBroadcastChatInSync(client, fetchCount);

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

    //------------------------[ Desplay Histories Message ]-------------------------//
    static void DisplayHistoryMessages(List<HistoryMessage> messages)
    {
        foreach (HistoryMessage hm in messages)
        {
            if (hm.binaryMessage != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("-- [Binary message] Fetched: ID: {0}, from {1}, mtype {2}, mid {3}, binary message length {4}, attrs {5}, mtime {6}",
                    hm.cursorId, hm.fromUid, hm.messageType, hm.messageId, hm.binaryMessage.Length, hm.attrs, hm.modifiedTime);

                Debug.Log(sb.ToString());
            }
            else if (hm.messageId >= 40 && hm.messageId <= 50)
            {
                StringBuilder sb = new StringBuilder();

                if (hm.fileInfo.isRTMAudio)
                    sb.AppendFormat("-- [RTM Audio message] Fetched: ID: {0}, from {1}, mtype {2}, mid {3}, url {4}, size (5), attrs {6}, mtime {7}, language {8}, duration: {9}ms",
                            hm.cursorId, hm.fromUid, hm.messageType, hm.messageId, hm.fileInfo.url, hm.fileInfo.size, hm.attrs, hm.modifiedTime, hm.fileInfo.language, hm.fileInfo.duration);
                else
                    sb.AppendFormat("-- [File message] Fetched: ID: {0}, from {1}, mtype {2}, mid {3}, url {4}, size (5), attrs {6}, mtime {7}",
                        hm.cursorId, hm.fromUid, hm.messageType, hm.messageId, hm.fileInfo.url, hm.fileInfo.size, hm.attrs, hm.modifiedTime);

                Debug.Log(sb.ToString());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("-- [String message] Fetched: ID: {0}, from {1}, mtype {2}, mid {3}, message {4}, attrs {5}, mtime {6}",
                    hm.cursorId, hm.fromUid, hm.messageType, hm.messageId, hm.stringMessage, hm.attrs, hm.modifiedTime);

                Debug.Log(sb.ToString());
            }
        }
    }

    //------------------------[ Message Histories Demo ]-------------------------//
    static void GetP2PMessageInSync(RTMClient client, long peerUid, int count)
    {
        long beginMsec = 0;
        long endMsec = 0;
        long lastId = 0;
        int fetchedCount = 0;

        while (count > 0)
        {
            int maxCount = (count > 20) ? 20 : count;
            count -= maxCount;

            int errorCode = client.GetP2PMessage(out HistoryMessageResult result, peerUid, true, maxCount, beginMsec, endMsec, lastId);
            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                Debug.Log("Get P2P history message with user " + peerUid + " in sync failed, current fetched " + fetchedCount + " items, error Code "+ errorCode);
                return;
            }

            fetchedCount += result.count;
            DisplayHistoryMessages(result.messages);

            beginMsec = result.beginMsec;
            endMsec = result.endMsec;
            lastId = result.lastCursorId;
        }

        Debug.Log("Get P2P history message total fetched " + fetchedCount + " items");
    }

    static void GetGroupMessageInSync(RTMClient client, long groupId, int count)
    {
        long beginMsec = 0;
        long endMsec = 0;
        long lastId = 0;
        int fetchedCount = 0;

        while (count > 0)
        {
            int maxCount = (count > 20) ? 20 : count;
            count -= maxCount;

            int errorCode = client.GetGroupMessage(out HistoryMessageResult result, groupId, true, maxCount, beginMsec, endMsec, lastId);
            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Get group history message in group {0} in sync failed, current fetched {1} items, error Code {2}",
                    groupId, fetchedCount, errorCode);
                Debug.Log(sb.ToString());
                return;
            }

            fetchedCount += result.count;
            DisplayHistoryMessages(result.messages);

            beginMsec = result.beginMsec;
            endMsec = result.endMsec;
            lastId = result.lastCursorId;
        }

        Debug.Log("Get group history message total fetched " + fetchedCount + " items");
    }

    static void GetRoomMessageInSync(RTMClient client, long roomId, int count)
    {
        long beginMsec = 0;
        long endMsec = 0;
        long lastId = 0;
        int fetchedCount = 0;

        while (count > 0)
        {
            int maxCount = (count > 20) ? 20 : count;
            count -= maxCount;

            int errorCode = client.GetRoomMessage(out HistoryMessageResult result, roomId, true, maxCount, beginMsec, endMsec, lastId);
            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Get room history message in room {0} in sync failed, current fetched {1} items, error Code {2}",
                    roomId, fetchedCount, errorCode);
                Debug.Log(sb.ToString());
                return;
            }

            fetchedCount += result.count;
            DisplayHistoryMessages(result.messages);

            beginMsec = result.beginMsec;
            endMsec = result.endMsec;
            lastId = result.lastCursorId;
        }

        Debug.Log("Get room history message total fetched " + fetchedCount + " items");
    }

    static void GetBroadcastMessageInSync(RTMClient client, int count)
    {
        long beginMsec = 0;
        long endMsec = 0;
        long lastId = 0;
        int fetchedCount = 0;

        while (count > 0)
        {
            int maxCount = (count > 20) ? 20 : count;
            count -= maxCount;

            int errorCode = client.GetBroadcastMessage(out HistoryMessageResult result, true, maxCount, beginMsec, endMsec, lastId);
            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Get broadcast history message in sync failed, current fetched {0} items, error Code {1}",
                    fetchedCount, errorCode);
                Debug.Log(sb.ToString());
                return;
            }

            fetchedCount += result.count;
            DisplayHistoryMessages(result.messages);

            beginMsec = result.beginMsec;
            endMsec = result.endMsec;
            lastId = result.lastCursorId;
        }

        Debug.Log("Get broadcast history message total fetched " + fetchedCount + " items");
    }

    //------------------------[ Chat Histories Demo ]-------------------------//
    static void GetP2PChatInSync(RTMClient client, long peerUid, int count)
    {
        long beginMsec = 0;
        long endMsec = 0;
        long lastId = 0;
        int fetchedCount = 0;

        while (count > 0)
        {
            int maxCount = (count > 20) ? 20 : count;
            count -= maxCount;

            int errorCode = client.GetP2PChat(out HistoryMessageResult result, peerUid, true, maxCount, beginMsec, endMsec, lastId);
            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Get P2P history chat with user {0} in sync failed, current fetched {1} items, error Code {2}",
                    peerUid, fetchedCount, errorCode);
                Debug.Log(sb.ToString());
                return;
            }

            fetchedCount += result.count;
            DisplayHistoryMessages(result.messages);

            beginMsec = result.beginMsec;
            endMsec = result.endMsec;
            lastId = result.lastCursorId;
        }

        Debug.Log("Get P2P history chat total fetched " + fetchedCount + " items");
    }

    static void GetGroupChatInSync(RTMClient client, long groupId, int count)
    {
        long beginMsec = 0;
        long endMsec = 0;
        long lastId = 0;
        int fetchedCount = 0;

        while (count > 0)
        {
            int maxCount = (count > 20) ? 20 : count;
            count -= maxCount;

            int errorCode = client.GetGroupChat(out HistoryMessageResult result, groupId, true, maxCount, beginMsec, endMsec, lastId);
            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Get group history chat in group {0} in sync failed, current fetched {1} items, error Code {2}",
                    groupId, fetchedCount, errorCode);
                Debug.Log(sb.ToString());
                return;
            }

            fetchedCount += result.count;
            DisplayHistoryMessages(result.messages);

            beginMsec = result.beginMsec;
            endMsec = result.endMsec;
            lastId = result.lastCursorId;
        }

        Debug.Log("Get group history chat total fetched " + fetchedCount + " items");
    }

    internal class InfoPackage
    {
        public bool desc = true;
        public long beginMsec = 0;
        public long endMsec = 0;
        public long lastId = 0;
        public int fetchedCount = 0;
        public int count;
    }

    static void GetGroupChatInASync(RTMClient client, long groupId, int count, InfoPackage info = null)
    {
        const int countLimitation = 10;
        if (info == null)
        {
            info = new InfoPackage();
            info.count = count;
        }

        bool status = client.GetGroupChat((int gotCount, long lastId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
        {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                info.beginMsec = beginMsec;
                info.endMsec = endMsec;
                info.lastId = lastId;
                info.beginMsec = beginMsec;
                info.fetchedCount += gotCount;
                info.count -= gotCount;

                DisplayHistoryMessages(messages);

                if (gotCount == countLimitation && info.count > 0)
                {
                    GetGroupChatInASync(client, groupId, info.count, info);
                }
                else
                {
                    Debug.Log("Get group history chat in group " + groupId + " in async completed. Fetched " + info.fetchedCount + " items.");
                }
            }
            else
            {
                Debug.Log("Fetch group history chat in group "+ groupId + " in async failed. Fetched "+ info.fetchedCount + " items, error code: " + errorCode);
            }
        }, groupId, info.desc, countLimitation, info.beginMsec, info.endMsec, info.lastId);

        if (status == false)
        {
            Debug.Log("Start fetch group history chat in group "+ groupId + " in async failed.");
        }
    }

    static void GetRoomChatInSync(RTMClient client, long roomId, int count)
    {
        long beginMsec = 0;
        long endMsec = 0;
        long lastId = 0;
        int fetchedCount = 0;

        while (count > 0)
        {
            int maxCount = (count > 20) ? 20 : count;
            count -= maxCount;

            int errorCode = client.GetRoomChat(out HistoryMessageResult result, roomId, true, maxCount, beginMsec, endMsec, lastId);
            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Get room history chat in room {0} in sync failed, current fetched {1} items, error Code {2}",
                    roomId, fetchedCount, errorCode);
                Debug.Log(sb.ToString());
                return;
            }

            fetchedCount += result.count;
            DisplayHistoryMessages(result.messages);

            beginMsec = result.beginMsec;
            endMsec = result.endMsec;
            lastId = result.lastCursorId;
        }

        Debug.Log("Get room history chat total fetched " + fetchedCount + " items");
    }

    static void GetBroadcastChatInSync(RTMClient client, int count)
    {
        long beginMsec = 0;
        long endMsec = 0;
        long lastId = 0;
        int fetchedCount = 0;

        while (count > 0)
        {
            int maxCount = (count > 20) ? 20 : count;
            count -= maxCount;

            int errorCode = client.GetBroadcastChat(out HistoryMessageResult result, true, maxCount, beginMsec, endMsec, lastId);
            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Get broadcast history chat in sync failed, current fetched {0} items, error Code {1}",
                    fetchedCount, errorCode);
                Debug.Log(sb.ToString());
                return;
            }

            fetchedCount += result.count;
            DisplayHistoryMessages(result.messages);

            beginMsec = result.beginMsec;
            endMsec = result.endMsec;
            lastId = result.lastCursorId;
        }

        Debug.Log("Get broadcast history chat total fetched " + fetchedCount + " items");
    }
}

