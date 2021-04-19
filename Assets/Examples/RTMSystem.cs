using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using com.fpnn.rtm;

class RTMSystem : Main.ITestCase
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

        AddAttributesDemo(client);
        GetAttributesDemo(client);
        
        GetDevicePushOption(client);
        AddDevicePushOption(client, MessageCategory.P2PMessage, 12345);
        AddDevicePushOption(client, MessageCategory.GroupMessage, 223344, new HashSet<byte>());

        AddDevicePushOption(client, MessageCategory.P2PMessage, 34567, null);
        AddDevicePushOption(client, MessageCategory.GroupMessage, 445566, new HashSet<byte>() { 23, 35, 56, 67, 78, 89 });

        GetDevicePushOption(client);

        RemoveDevicePushOption(client, MessageCategory.GroupMessage, 223344, new HashSet<byte>() { 23, 35, 56, 67, 78, 89 });
        RemoveDevicePushOption(client, MessageCategory.GroupMessage, 445566, new HashSet<byte>());

        GetDevicePushOption(client);

        RemoveDevicePushOption(client, MessageCategory.P2PMessage, 12345);
        RemoveDevicePushOption(client, MessageCategory.P2PMessage, 34567);
        RemoveDevicePushOption(client, MessageCategory.GroupMessage, 223344);
        RemoveDevicePushOption(client, MessageCategory.GroupMessage, 445566, new HashSet<byte>() { 23, 35, 56, 67, 78, 89 });

        GetDevicePushOption(client);

        client.Bye();
        Thread.Sleep(1000);

        Debug.Log("============== Demo completed ================");
    }

    public void Stop() { }

    static RTMClient LoginRTM(string rtmEndpoint, long pid, long uid, string token)
    {
        RTMClient client = new RTMClient(rtmEndpoint, pid, uid, new example.common.RTMExampleQuestProcessor());

        int errorCode = client.Login(out bool ok, token, new Dictionary<string, string>() {
                { "attr1", "demo 123" },
                { "attr2", " demo 234" },
            });
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

    static void AddAttributesDemo(RTMClient client)
    {
        int errorCode = client.AddAttributes(new Dictionary<string, string>() {
                { "key1", "value1" },
                { "key2", "value2" }
            });

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Add attributes in sync failed.");
        else
            Debug.Log("Add attributes in sync success.");
    }

    static void GetAttributesDemo(RTMClient client)
    {
        int errorCode = client.GetAttributes(out Dictionary<string, string> attributes);
        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
        {
            Debug.Log("Get attributes in sync failed. error code " + errorCode);
            return;
        }

        Debug.Log("Attributes has " + attributes.Count + " items.");

        foreach (KeyValuePair<string, string> kvp in attributes)
            Debug.Log("Key " + kvp.Key  + ", value " + kvp.Value);
    }

    static void AddDevicePushOption(RTMClient client, MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null)
    {
        Debug.Log($"===== [ AddDevicePushOption ] =======");

        int errorCode = client.AddDevicePushOption(messageCategory, targetId, mTypes);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Add device push option in sync failed.");
        else
            Debug.Log("Add device push option in sync success.");
    }

    static void RemoveDevicePushOption(RTMClient client, MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null)
    {
        Debug.Log($"===== [ RemoveDevicePushOption ] =======");

        int errorCode = client.RemoveDevicePushOption(messageCategory, targetId, mTypes);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Remove device push option in sync failed.");
        else
            Debug.Log("Remove device push option in sync success.");
    }

    static void PrintDevicePushOption(string categroy, Dictionary<long, HashSet<byte>> optionDictionary)
    {
        Debug.Log($"===== {categroy} has {optionDictionary.Count} items. =======");
        foreach (KeyValuePair<long, HashSet<byte>> kvp in optionDictionary)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("ID: ").Append(kvp.Key).Append(", count: ").Append(kvp.Value.Count);
            if (kvp.Value.Count > 0)
            {
                sb.Append(": {");
                foreach (byte mType in kvp.Value)
                    sb.Append($" {mType},");

                sb.Append("}");
            }

            Debug.Log(sb);
        }
    }

    static void GetDevicePushOption(RTMClient client)
    {
        Debug.Log($"===== [ GetDevicePushOption ] =======");

        int errorCode = client.GetDevicePushOption(
            out Dictionary<long, HashSet<byte>> p2pDictionary, out Dictionary<long, HashSet<byte>> groupDictionary);
        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
        {
            Debug.Log($"Get device push option in sync failed. error code {errorCode}");
            return;
        }

        PrintDevicePushOption("P2P", p2pDictionary);
        PrintDevicePushOption("Group", groupDictionary);
    }
}