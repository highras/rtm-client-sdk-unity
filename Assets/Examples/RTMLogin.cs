using System.Threading;
using UnityEngine;
using com.fpnn.rtm;

class Login : Main.ITestCase
{
    public void Start(string endpoint, long pid, long uid, string token)
    {
        SyncLoginDemo(endpoint, pid, uid, token);
        AsyncLoginDemo(endpoint, pid, uid, token);

        Debug.Log("============== Demo completed ================");
    }

    public void Stop() { }

    static void SyncLoginDemo(string rtmEndpoint, long pid, long uid, string token)
    {
        RTMClient client = new RTMClient(rtmEndpoint, pid, uid, new example.common.RTMExampleQuestProcessor());

        int errorCode = client.Login(out bool ok, token);
        Debug.Log("Login Result: OK: " + ok + ", code: " + errorCode);

        Thread.Sleep(2000);

        client.Close();
        Debug.Log("closed");
        Thread.Sleep(1500);
    }

    static void AsyncLoginDemo(string rtmEndpoint, long pid, long uid, string token)
    {
        RTMClient client = new RTMClient(rtmEndpoint, pid, uid, new example.common.RTMExampleQuestProcessor());
        bool status = client.Login((long pid_, long uid_, bool authStatus, int errorCode) => {
            Debug.Log("Async login " + authStatus + ". pid " + pid_ + ", uid " + uid_ + ", code : " + errorCode);
        }, token);
        if (!status)
        {
            Debug.Log("Async login starting failed.");
            return;
        }

        Debug.Log("Waiting 3 seconds for login, then, close the session.");
        Thread.Sleep(3000);

        client.Close();
        Debug.Log("closed");
        Thread.Sleep(1500);
    }
}