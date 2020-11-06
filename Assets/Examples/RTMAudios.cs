using System.Collections.Generic;
using UnityEngine;
using com.fpnn.rtm;

class MyAudioRecorder : AudioRecorder.IMicrophone {    
    public MyAudioRecorder() {
    }

    public void Start() {
        Debug.Log("Recorder Start");
    }

    public void End() {
        Debug.Log("Recorder End");
    }

    public void OnRecord(RTMAudioData audioData) {

        Debug.Log("On Record");
        Debug.Log("Record Duration: " + audioData.Duration);
       
        long messageId;
        Audios.client.SendFile(out messageId, Audios.peerUid, audioData);
        Debug.Log("send audio message mid: " + messageId);
    }
}

class Audios : Main.ITestCase
{
    public static long peerUid = 12345678;

    public static RTMClient client;

    public void Start(string endpoint, long pid, long uid, string token)
    {
        client = LoginRTM(endpoint, pid, uid, token);

        if (client == null)
        {
            Debug.Log("User " + uid + " login RTM failed.");
            return;
        }

        Debug.Log("======== Start Recording, please speak now, will stop after 10 seconds =========");

        AudioRecorder.Instance.Init("zh-CN", null, new MyAudioRecorder());
        AudioRecorder.Instance.StartInput();

        System.Threading.Thread.Sleep(10 * 1000);

        AudioRecorder.Instance.FinishInput();

        System.Threading.Thread.Sleep(1000);

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
}