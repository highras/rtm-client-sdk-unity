using System.Collections.Generic;
using UnityEngine;
using com.fpnn.rtm;

class Friends : Main.ITestCase
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

        AddFriends(client, new HashSet<long>() { 123456, 234567, 345678, 456789 });

        GetFriends(client);

        DeleteFriends(client, new HashSet<long>() { 234567, 345678 });

        System.Threading.Thread.Sleep(2000);   //-- Wait for server sync action.

        GetFriends(client);

        //-- Blacklist
        AddBlacklist(client, new HashSet<long>() { 123456, 234567, 345678, 456789 });

        GetBlacklist(client);

        DeleteBlacklist(client, new HashSet<long>() { 234567, 345678 });

        GetBlacklist(client);

        DeleteBlacklist(client, new HashSet<long>() { 123456, 234567, 345678, 456789 });

        GetBlacklist(client);

        Debug.Log("Demo completed.");
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

    //------------------------[ Friend Operations ]-------------------------//
    static void AddFriends(RTMClient client, HashSet<long> uids)
    {
        int errorCode = client.AddFriends(uids);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Add friends in sync failed, error code is " + errorCode);
        else
            Debug.Log("Add friends in sync success");
    }

    static void DeleteFriends(RTMClient client, HashSet<long> uids)
    {
        int errorCode = client.DeleteFriends(uids);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Delete friends in sync failed, error code is " + errorCode);
        else
            Debug.Log("Delete friends in sync success");
    }

    static void GetFriends(RTMClient client)
    {
        int errorCode = client.GetFriends(out HashSet<long> uids);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get friends in sync failed, error code is " + errorCode);
        else
        {
            Debug.Log("Get friends in sync success");
            foreach (long uid in uids)
                Debug.Log("-- Friend uid: " + uid);
        }
    }

    //------------------------[ Blacklist Operations ]-------------------------//
        static void AddBlacklist(RTMClient client, HashSet<long> uids)
        {
            int errorCode = client.AddBlacklist(uids);

            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Add users to blacklist in sync failed, error code is " + errorCode);
            else
                Debug.Log("Add users to blacklist in sync success");
        }

        static void DeleteBlacklist(RTMClient client, HashSet<long> uids)
        {
            int errorCode = client.DeleteBlacklist(uids);

            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Delete from blacklist in sync failed, error code is " + errorCode);
            else
                Debug.Log("Delete from blacklist in sync success");
        }

        static void GetBlacklist(RTMClient client)
        {
            int errorCode = client.GetBlacklist(out HashSet<long> uids);

            if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
                Debug.Log("Get blacklist in sync failed, error code is " + errorCode);
            else
            {
                Debug.Log("Get blacklist in sync success");
                foreach (long uid in uids)
                    Debug.Log("-- blocked uid: " + uid);
            }
        }
}