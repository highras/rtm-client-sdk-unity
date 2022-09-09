using System.Collections;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

namespace com.fpnn.rtm
{
    public class AudioRecorderNative : Singleton<AudioRecorderNative>
    {
        public interface IAudioRecorderListener
        {
            void RecordStart(bool success);
            void RecordEnd();
            void OnRecord(RTMAudioData audioData);
            void OnVolumn(double db);
            void PlayStart(bool success);
            void PlayEnd();
        }

        static internal IAudioRecorderListener audioRecorderListener;
        static internal string language;
        static volatile bool cancelRecord = false;
        static volatile bool recording = false;

        delegate void VolumnCallbackDelegate(float volumn);
        [MonoPInvokeCallback(typeof(VolumnCallbackDelegate))]
        private static void VolumnCallback(float volumn)
        {
            if (audioRecorderListener != null)
            {
                float minValue = -60;
                float range = 60;
                float outRange = 100;
                if (volumn < minValue)
                    volumn = minValue;

                volumn = (volumn + range) / range * outRange;
                audioRecorderListener.OnVolumn(volumn);
            }
        }

        delegate void StartRecordCallbackDelegate(bool success);
        [MonoPInvokeCallback(typeof(StartRecordCallbackDelegate))]
        private static void StartRecordCallback(bool success)
        {
            if (audioRecorderListener != null)
                audioRecorderListener.RecordStart(success);
        }

        delegate void StopRecordCallbackDelegate(IntPtr data, int length, long time);
        [MonoPInvokeCallback(typeof(StopRecordCallbackDelegate))]
        private static void StopRecordCallback(IntPtr data, int length, long time)
        {
            recording = false;
            if (audioRecorderListener != null)
            {
                audioRecorderListener.RecordEnd();
                if (cancelRecord)
                {
                    cancelRecord = false;
                    return;
                }
                byte[] payload = new byte[length];
                Marshal.Copy(data, payload, 0, length);

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
                RTMAudioData audioData = new RTMAudioData(AudioConvert.ConvertToAmrwb(payload), language, time);
#else
                RTMAudioData audioData = new RTMAudioData(payload, language, time);
#endif
                audioRecorderListener.OnRecord(audioData);
            }
        }

        delegate void PlayFinishCallbackDelegate();
        [MonoPInvokeCallback(typeof(PlayFinishCallbackDelegate))]
        private static void PlayFinishCallback()
        {
            if (audioRecorderListener != null)
                audioRecorderListener.PlayEnd();
        }

        delegate void PlayStartCallbackDelegate(bool success);
        [MonoPInvokeCallback(typeof(PlayStartCallbackDelegate))]
        private static void PlayStartCallback(bool success)
        {
            audioRecorderListener?.PlayStart(success);
        }

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
        [DllImport("RTMNative")]
        private static extern void startRecord(VolumnCallbackDelegate callback, StartRecordCallbackDelegate startCallback);

        [DllImport("RTMNative")]
        private static extern void stopRecord(StopRecordCallbackDelegate callback);

        [DllImport("RTMNative")]
        private static extern void startPlay(byte[] data, int length, PlayFinishCallbackDelegate callback, PlayStartCallbackDelegate playStartCallback);

        [DllImport("RTMNative")]
        private static extern void stopPlay();

        [DllImport("RTMNative")]
        private static extern void playWithPath(byte[] data, int length, PlayFinishCallbackDelegate callback);
#elif UNITY_IOS
        [DllImport("__Internal")]
        private static extern void startRecord(VolumnCallbackDelegate callback, StartRecordCallbackDelegate startCallback);

        [DllImport("__Internal")]
        private static extern void stopRecord(StopRecordCallbackDelegate callback);

        [DllImport("__Internal")]
        private static extern void startPlay(byte[] data, int length, PlayFinishCallbackDelegate callback, PlayStartCallbackDelegate playStartCallback);

        [DllImport("__Internal")]
        private static extern void stopPlay();
#elif UNITY_ANDROID
        class AudioRecordAndroidProxy : AndroidJavaProxy
        {
            public AudioRecordAndroidProxy() : base("com.NetForUnity.IAudioAction")
            {
            }

            public void startRecord(bool success, string errorMsg)
            {
                if (AudioRecorderNative.audioRecorderListener != null)
                    AudioRecorderNative.audioRecorderListener.RecordStart(success);
            }

            public void stopRecord()
            {
                recording = false;
                if (AudioRecorderNative.audioRecorderListener != null)
                    AudioRecorderNative.audioRecorderListener.RecordEnd();
            }

            public void startBroad(bool success)
            {
                if (AudioRecorderNative.audioRecorderListener != null)
                    AudioRecorderNative.audioRecorderListener.PlayStart(success);
            }

            public void broadFinish()
            {
                if (AudioRecorderNative.audioRecorderListener != null)
                    AudioRecorderNative.audioRecorderListener.PlayEnd();
            }

            public void listenVolume(double db)
            {
                if (AudioRecorderNative.audioRecorderListener != null)
                {
                    float minValue = -60;
                    float range = 60;
                    float outRange = 100;
                    if (db < minValue)
                        db = minValue;

                    db = (db + range) / range * outRange;

                    AudioRecorderNative.audioRecorderListener.OnVolumn(db);
                }
            }
        }
        static AndroidJavaObject AudioRecord = null;
#endif

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
        [DllImport("RTMNative")]
        internal static extern void destroy();
#endif

        public void Init(string language, IAudioRecorderListener listener)
        {
            AudioRecorderNative.language = language;
            audioRecorderListener = listener;
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)

#elif UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject appconatext = jc.GetStatic<AndroidJavaObject>("currentActivity");
            if (AudioRecord == null)
            {
                AndroidJavaClass playerClass = new AndroidJavaClass("com.NetForUnity.RTMAudio");
                AudioRecord = playerClass.CallStatic<AndroidJavaObject>("getInstance");
            }
            AudioRecord.Call("init", appconatext, language, new AudioRecordAndroidProxy());
#else

#endif
        }

        public bool IsRecording()
        {
            return recording;
        }

        public void StartRecord()
        {
            recording = true;
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
            startRecord(VolumnCallback, StartRecordCallback);
#elif UNITY_ANDROID
            if (AudioRecord != null)
                AudioRecord.Call("startRecord");
#else
            startRecord(VolumnCallback, StartRecordCallback);
#endif
        }

        public void StopRecord()
        {
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
            stopRecord(StopRecordCallback);
#elif UNITY_ANDROID
            AndroidJavaObject audio = AudioRecord.Call<AndroidJavaObject>("stopRecord");
            int duration = audio.Get<int>("duration");
            byte[] audioData = audio.Get<byte[]>("audioData");
            //byte[] audioData = (byte[])(Array)audio.Get<sbyte[]>("audioData");
            if (cancelRecord)
            {
                cancelRecord = false;
                return;
            }
            if (audioRecorderListener != null)
            {
                RTMAudioData data = new RTMAudioData(audioData, language, duration);
                audioRecorderListener.OnRecord(data);
            }
#else
            stopRecord(StopRecordCallback);
#endif
        }

        public void CancelRecord()
        {
            cancelRecord = true;
            StopRecord();
        }

        public void Play(RTMAudioData data)
        {
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
            byte[] wavBuffer = AudioConvert.ConvertToWav(data.Audio);
            startPlay(wavBuffer, wavBuffer.Length, PlayFinishCallback, PlayStartCallback);
#elif (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
            startPlay(data.Audio, data.Audio.Length, PlayFinishCallback, PlayStartCallback);
#elif UNITY_ANDROID
            if (AudioRecord != null)
                AudioRecord.Call("broadAudio", AudioConvert.ConvertToWav(data.Audio));
#else
            startPlay(data.Audio, data.Audio.Length, PlayFinishCallback, PlayStartCallback);
#endif
        }

        public void StopPlay()
        {
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
            stopPlay();
#elif UNITY_ANDROID
            if (AudioRecord != null)
                AudioRecord.Call("stopAudio");
#else
            stopPlay();
#endif
        }
    }
}