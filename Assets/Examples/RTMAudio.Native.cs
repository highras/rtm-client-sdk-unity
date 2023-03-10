using System.Collections.Generic;
using UnityEngine;
using com.fpnn.rtm;

class MyAudioRecorderListener : AudioRecorderNative.IAudioRecorderListener {    
    public void RecordStart(bool success)
    { 
        Debug.Log("RecordStart success = " + success);
    }

    public void RecordEnd()
    { 
        Debug.Log("RecordEnds");
    }

    public void OnRecord(RTMAudioData audioData)
    {
        Debug.Log("OnRecord " + audioData.Duration);
        AudioRecorderNative.Play(audioData);
    }

    public void OnVolumn(double db)
    { 
        Debug.Log("OnVolumn db=" + db);
    }

    public void PlayStart(bool success)
    {
        Debug.Log("PlayStart success = " + success);
    }
    public void PlayEnd()
    { 
        Debug.Log("PlayEnd");
    }
}

class AudioNative : Main.ITestCase
{

    public void Start(string endpoint, long pid, long uid, string token)
    {
        AudioRecorderNative.Init("zh-CN", new MyAudioRecorderListener());
        AudioRecorderNative.StartRecord();

        //System.Threading.Thread.Sleep(10 * 1000);

        Debug.Log("============== Demo completed ================");
    }

    public void Stop()
    {

    }

}