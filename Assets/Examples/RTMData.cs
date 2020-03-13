using UnityEngine;
using com.fpnn.rtm;

class Data : Main.ITestCase
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

        Debug.Log("=========== Begin set user data ===========");

        SetData(client, "key 1", "value 1");
        SetData(client, "key 2", "value 2");

        Debug.Log("=========== Begin get user data ===========");

        GetData(client, "key 1");
        GetData(client, "key 2");

        Debug.Log("=========== Begin delete one of user data ===========");

        DeleteData(client, "key 2");

        Debug.Log("=========== Begin get user data after delete action ===========");

        GetData(client, "key 1");
        GetData(client, "key 2");

        Debug.Log("=========== User logout ===========");

        client.Bye();

        Debug.Log("=========== User relogin ===========");

        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
            return;

        Debug.Log("=========== Begin get user data after relogin ===========");

        GetData(client, "key 1");
        GetData(client, "key 2");

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

    static void SetData(RTMClient client, string key, string value)
    {
        int errorCode = client.DataSet(key, value);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Set user data with key " + key + " in sync failed.");
        else
            Debug.Log("Set user data with key " + key + " in sync success.");
    }

    static void GetData(RTMClient client, string key)
    {
        int errorCode = client.DataGet(out string value, key);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Get user data with key " + key + " in sync failed, error code is " + errorCode);
        else
            Debug.Log("Get user data with key " + key + " in sync success, value is " + (value ?? "null"));
    }

    static void DeleteData(RTMClient client, string key)
    {
        int errorCode = client.DataDelete(key);

        if (errorCode != com.fpnn.ErrorCode.FPNN_EC_OK)
            Debug.Log("Delete user data with key " + key + " in sync failed.");
        else
            Debug.Log("Delete user data with key " + key + " in sync success.");
    }
}