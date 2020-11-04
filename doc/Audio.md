# RTM Client Unity SDK Audio & Record Docs

# Index

[TOC]

## Remove Audio-Related Features
* You can remove the audio-related features by add a symbols named: "RTM_BUILD_NO_AUDIO" into "Player Settings => Other Settings => Configuration => Scripting Define Symbols"

## Audio Record Example



```

using com.fpnn;
using com.fpnn.rtm;

// implement your own recording interface
class MyAudioRecorder : AudioRecorder.IMicrophone {
    
    public void Start() {
        Debug.Log("Start");
    }

    public void End() {
        Debug.Log("End");
    }

    public void OnRecord(RTMAudioData audioData) {
        Debug.Log("OnRecord");

        // you can do the speech to text like this:
		rtmClient.SpeechToText((string resultText, string resultLanguage, int errorCode) => {
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
		AudioSource audioSource = GetComponent<AudioSource>();
		audioSource.clip = AudioClip.Create("testSound", audioData.LengthSamples, 1, audioData.Frequency, false, false);
		audioSource.clip.SetData(audioData.PcmData, 0);
		audioSource.Play();
    }
}

AudioRecorder.Instance.Init(RTMClient.GetTranslatedLanguage(TranslateLanguage.zh_cn), null, new MyAudioRecorder());  // init AudioRecorder
AudioRecorder.Instance.StartInput();  // start record
AudioRecorder.Instance.FinishInput(); // finish record


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
		AudioSource audioSource = GetComponent<AudioSource>();
		audioSource.clip = AudioClip.Create("testSound", audioData.LengthSamples, 1, audioData.Frequency, false, false);
		audioSource.clip.SetData(audioData.PcmData, 0);
		audioSource.Play();
	}
}


```
