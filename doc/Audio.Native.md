# RTM Client Unity SDK Native Audio & Record Docs

# Index

[TOC]

## Audio Native Record Example



```

using com.fpnn;
using com.fpnn.rtm;

// implement your own recording interface
class MyAudioRecorderListener : AudioRecorderNative.IAudioRecorderListener {    
    public void RecordStart()
    { 
        Debug.Log("RecordStart");
    }

    public void RecordEnd()
    { 
        Debug.Log("RecordEnds");
    }

    public void OnRecord(RTMAudioData audioData)
    {
        Debug.Log("OnRecord");

        // you can do the speech to text like this:
		rtmClient.SpeechToText((string
         resultText, string resultLanguage, int errorCode) => {
			if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
				Debug.Log("SpeechToText resultText: " + resultTextAsync + " resultLanguage: " + resultLanguageAsync);
			else
				Debug.Log("SpeechToText error: " + errorCode);
		}, audioData.Audio, audioData.Language);

		// you can send the audio message to others: 
		rtmClient.SendFile((long messageId, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
				Debug.Log("SendFile ok, messageId: " + messageId);
			else
				Debug.Log("SendFile error: " + errorCode);
		}, otherUserID, audioData.Audio);

       	// you can play the audio like this:
		// note: the following code may be required run in Unity main thread 
        AudioRecorderNative.Instance.Play(audioData);
    }

    public void OnVolumn(double db)
    { 
        Debug.Log("OnVolumn db=" + db);
    }

    public void PlayEnd()
    { 
        Debug.Log("PlayEnd");
    }
}

```

## Audio Push Handler Example


```csharp

// in QuestProcessor

public override void PushFile(RTMMessage message) {
	if (message.messageType == MessageType.AudioFile && message.fileInfo != null && message.fileInfo.isRTMAudio) 
	{
		string audioUrl = message.fileInfo.url;
		
		byte[] audioArray = Download_From_Url(audioUrl); // here is a fake code, means to download the audio file from url

		RTMAudioData audioData = new RTMAudioData(audioArray, message.fileInfo);  // create the RTMAudioData instance


		// you can do the speech to text like this:
		rtmClient.SpeechToText((string resultText, string resultLanguage, int errorCode) => {
			if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK)
				Debug.Log("SpeechToText resultText: " + resultTextAsync + " resultLanguage: " + resultLanguageAsync);
			else
				Debug.Log("SpeechToText error: " + errorCode);
		}, audioData.Audio, audioData.Language);


		// you can play the audio like this:
		// note: the following code may be required run in Unity main thread
		AudioRecorderNative.Instance.Play(audioData);
	}
}


```

## Api

### AudioRecorderNative Init

	public void Init(string language, IAudioRecorderListener listener)

Init the Singleton AudioRecorderNative.

Parameters:

+ `string lang`

	Supported language . Please refer [Supported Language](https://docs.ilivedata.com/stt/production/).

+ `IAudioRecorderListener listener`

	Instance of AudioRecorderNative.IAudioRecorderListener.

### Start Record

	public void StartRecord()

Start Record.


### Stop Record

	public void StopRecord()

Stop Record.

### Start Play Audio

	public void StartPlay(RTMAudioData data)

Start Play Aduio.


Parameters:

+ `RTMAudioData data`

	Audio Data.

### Stop Play Audio

    public void StopPlay()

Stop Play Audio.