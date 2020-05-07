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
		string resultText = "", resultLanguage = "";
        someRtmClient.Transcribe(out resultText, out resultLanguage, audioData.Audio);

		// you can send the audio to others: 
		long mtime;
		someRtmClient.SendAudio(out mtime, otherUid, audioData.Audio, "");

		// you can play the audio like this:
		// note: the following code may be required run in Unity main thread
		AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = AudioClip.Create("testSound", audioData.Samples, 1, audioData.Frequency, false, false);
        audioSource.clip.SetData(audioData.PcmData, 0);
        audioSource.Play();
    }
}

AudioRecorder.Instance.Init("zh-CN", null, new MyAudioRecorder());  // init AudioRecorder
AudioRecorder.Instance.StartInput();  // start record
AudioRecorder.Instance.FinishInput(); // finish record


```

## Audio Push Handler Example


```

// in QuestProcessor
public void PushAudio(long fromUid, long toUid, long mid, byte[] message, string attrs, long mtime)
{
	RTMAudioData audioData = new RTMAudioData(message); // create RTMAudioData

	// if your project has automatic speech recognition turned on, you can get the recognition result like this:
	string resultText = audioData.RecognitionText;
	string resultLanguage = audioData.RecognitionLang;

	// you can do the speech recognition like this:
	string resultText = "", resultLanguage = "";
	someRtmClient.Transcribe(out resultText, out resultLanguage, audioData.Audio);

	// you can send the audio to others: 
	long mtime;
	someRtmClient.SendAudio(out mtime, otherUid, audioData.Audio, "");

	// you can play the audio like this:
	// note: the following code may be required run in Unity main thread
	AudioSource audioSource = GetComponent<AudioSource>();
	audioSource.clip = AudioClip.Create("testSound", audioData.Samples, 1, audioData.Frequency, false, false);
	audioSource.clip.SetData(audioData.PcmData, 0);
	audioSource.Play();
}


```
