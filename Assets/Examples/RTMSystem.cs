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
        int errorCode = client.GetAttributes(out List<Dictionary<string, string>> attributes);
        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
        {
            Debug.Log("Get attributes in sync failed. error code " + errorCode);
            return;
        }

        Debug.Log("Attributes has " + attributes.Count + " dictory.");
        int dictCount = 0;

        foreach (Dictionary<string, string> dict in attributes)
        {
            dictCount += 1;

            Debug.Log("Dictory " + dictCount + " has " + dict.Count + " items.");

            foreach (KeyValuePair<string, string> kvp in dict)
                Debug.Log("Key " + kvp.Key  + ", value " + kvp.Value);

            Debug.Log("");
        }
    }
}