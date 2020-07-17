# RTM Client Unity SDK Audio & Record Docs

# Index

[TOC]

## Audio Record Example



```

using com.fpnn;
using com.fpnn.rtm;

// implement your own recording interface
class MyAudioRecorder : AudioRecorder.IMicrophone {
    
    public void Start() {
        Debug.Log("on record start");
    }

    public void End() {
        Debug.Log("on record end");
    }

    public void OnRecord(RTMAudioData audioData) {
        Debug.Log("on get record data");

        // you can do the speech recognition like this:
		someRtmClient.Transcribe((string text, string language, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK) {
                Debug.Log("Transcribe ok. text: " + text + " language: " + language);
            } else {
                Debug.Log("Transcribe error: " + errorCode);
            }
        }, audioData.Audio);

		// you can send the audio to others: 
		someRtmClient.SendAudio((long mtime, int errorCode) => {
            if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK) {
                Debug.Log("SendAudio ok. mtime: " + mtime);
            } else {
                Debug.Log("SendAudio error: " + errorCode);
            }
        }, otherUid, audioData.Audio, "");

		// you can play the audio like this:
		// note: the following code may be required run in Unity main thread
		AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = AudioClip.Create("testSound", audioData.Samples, 1, audioData.Frequency, false, false);
        audioSource.clip.SetData(audioData.PcmData, 0);
        audioSource.Play();
    }
}

AudioRecorder.Instance.Init(RTMClient.GetTranslatedLanguage(TranslateLanguage.zh_cn), null, new MyAudioRecorder());  // init AudioRecorder
AudioRecorder.Instance.StartInput();  // start record
AudioRecorder.Instance.FinishInput(); // finish record


```

## Audio Push Handler Example


```

// in QuestProcessor
public void PushAudio(RTMMessage message) {
	Debug.Log($"Receive push audio message info: from {message.fromUid}, " +
			$"mid: {message.messageId}, attrs: {message.attrs}, " +
			$"source language {message.audioInfo.sourceLanguage} " +
			$"recognized language {message.audioInfo.recognizedLanguage} " +
			$"duration {message.audioInfo.duration} " +
			$"recognized content '{message.audioInfo.recognizedText}'.");

	// Get audio raw binary
	if (retrievedMessage.binaryMessage != null) {
		RTMAudioData audioData = new RTMAudioData(retrievedMessage.binaryMessage); // create RTMAudioData
		
		// you can do the speech recognition like this:
		someRtmClient.Transcribe((string text, string language, int errorCode) => {
			if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK) {
				Debug.Log("Transcribe ok. text: " + text + " language: " + language);
			} else {
				Debug.Log("Transcribe error: " + errorCode);
			}
		}, audioData.Audio);

		// you can send the audio to others: 
		long mtime;
		someRtmClient.SendAudio((long mtime, int errorCode) => {
			if (errorCode == com.fpnn.ErrorCode.FPNN_EC_OK) {
				Debug.Log("SendAudio ok. mtime: " + mtime);
			} else {
				Debug.Log("SendAudio error: " + errorCode);
			}
		}, otherUid, audioData.Audio, "");

		// you can play the audio like this:
		// note: the following code may be required run in Unity main thread
		AudioSource audioSource = GetComponent<AudioSource>();
		audioSource.clip = AudioClip.Create("testSound", audioData.Samples, 1, audioData.Frequency, false, false);
		audioSource.clip.SetData(audioData.PcmData, 0);
		audioSource.Play();
	}
}



```
