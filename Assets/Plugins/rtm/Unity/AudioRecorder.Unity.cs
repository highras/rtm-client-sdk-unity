#if UNITY_2017_1_OR_NEWER

using System.Collections;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.fpnn.rtm
{
    public class AudioRecorder : Singleton<AudioRecorder>
    {
        public interface IMicrophone {
            void Start();
            void End();
            void OnRecord(RTMAudioData audioData);
        }

        public static int RECORD_SAMPLE_RATE = 16000;
        private int LOUDNESS_SAMPLE_WINDOW = 128;
        private int UPDATE_LOUDNESS_MS = 50;

        private int maxRecordSeconds = 60;
        private bool isPause;
        private bool isFocus;

        private string lang;
        private string device;
        private bool isRecording;
        private float loudness;
        private long lastUpdateLoudnessTime = 0;

        private int position;
        private AudioClip clipRecord;
        private IMicrophone micPhone;
        private object selfLocker = new object();

        void OnEnable() {
            this.isPause = false;
            this.isFocus = false;
        }

        void OnDisable() {
            StopAllCoroutines();
            CancelInput();
        }

        void OnApplicationPause() {
            if (!this.isPause) {
                CancelInput();
            } else {
                this.isFocus = true;
            }

            this.isPause = true;
        }

        void OnApplicationFocus() {
            if (isFocus) {
                isPause = false;
                isFocus = false;
            }

            if (isPause) {
                isFocus = true;
            }
        }

        void Update() {
            lock (selfLocker) {
                if (!isRecording)
                    return;
                
                long now = ClientEngine.GetCurrentMilliseconds();
                if (now - lastUpdateLoudnessTime > UPDATE_LOUDNESS_MS) {
                    lastUpdateLoudnessTime = now;
                    UpdateLoudness();
                }
            }
        }

        void UpdateLoudness() {
#if RTM_BUILD_NO_AUDIO
            throw new Exception("Audio is disabled, please remove the RTM_BUILD_NO_AUDIO define in \"Scripting Define Symbols\"");
#else
            if (micPhone == null) {
                return;
            }

            float levelMax = 0;
            float[] data = new float[LOUDNESS_SAMPLE_WINDOW];
            int pos = Microphone.GetPosition(device) - (LOUDNESS_SAMPLE_WINDOW + 1);

            if (pos < 0) {
                return;
            }

            clipRecord.GetData(data, pos);

            for (int i = 0; i < LOUDNESS_SAMPLE_WINDOW; i++) {
                float wavePeak = data[i] * data[i];
                if (levelMax < wavePeak) {
                    levelMax = wavePeak;
                }
            }
            loudness = levelMax;
#endif
        }

        private IEnumerator TimeDown() {
            int time = 0;
            while (++time <= this.maxRecordSeconds) {
                lock (selfLocker) {
                    if (!isRecording) {
                        yield return 0;
                    }
                }
                yield return new WaitForSeconds(1);
            }
            FinishInput();
        }

        public void Init(string lang, string device, IMicrophone micPhone) {
            position = 0;
            isRecording = false;
            this.lang = lang;
#if RTM_BUILD_NO_AUDIO
            throw new Exception("Audio is disabled, please remove the RTM_BUILD_NO_AUDIO define in \"Scripting Define Symbols\"");
#else
            if (micPhone != null) {
                this.micPhone = micPhone;
            }

            lock (selfLocker) {
                this.device = device;
            }
#endif
        }

        public void SetLanguage(string lang) {
            this.lang = lang;
        }

        public int GetRelativeLoudness(float maxLoudness) {
            float loudnessNormalized = loudness;
            if (loudnessNormalized > maxLoudness)
                loudnessNormalized = maxLoudness;
            return (int)(loudnessNormalized / maxLoudness * 100);
        }

        public float GetAbsoluteLoudness() {
            return loudness;
        }

        public void StartInput(int maxRecordSeconds = 60) {
#if RTM_BUILD_NO_AUDIO
            throw new Exception("Audio is disabled, please remove the RTM_BUILD_NO_AUDIO define in \"Scripting Define Symbols\"");
#else
            if (micPhone == null) {
                return;
            }

            if (Microphone.devices.Length == 0) {
                return;
            }

            lock (selfLocker) {
                if (device == null) {
                    device = Microphone.devices[0];
                }

                if (isRecording) {
                    return;
                }
                this.maxRecordSeconds = maxRecordSeconds;
                isRecording = true;
                clipRecord = Microphone.Start(device, false, this.maxRecordSeconds, RECORD_SAMPLE_RATE);
                micPhone.Start();
            }
            StartCoroutine("TimeDown");
#endif
        }

        public void FinishInput() {
#if RTM_BUILD_NO_AUDIO
            throw new Exception("Audio is disabled, please remove the RTM_BUILD_NO_AUDIO define in \"Scripting Define Symbols\"");
#else
            if (micPhone == null) {
                return;
            }

            bool timeDown = false;
            lock (selfLocker) {
                if (isRecording) {
                    timeDown = true;
                    isRecording = false;
                    position = Microphone.GetPosition(device);
                    Microphone.End(device);
                    micPhone.End();
                }
            }

            if (timeDown) {
                StopCoroutine("TimeDown");

                if (micPhone != null && clipRecord != null) {
                    GetAudioData(clipRecord);
                }
            }
#endif
            loudness = 0;
        }

        public void CancelInput() {
#if RTM_BUILD_NO_AUDIO
            throw new Exception("Audio is disabled, please remove the RTM_BUILD_NO_AUDIO define in \"Scripting Define Symbols\"");
#else
            if (micPhone == null) {
                return;
            }

            bool timeDown = false;
            lock (selfLocker) {
                if (isRecording) {
                    timeDown = true;
                    isRecording = false;
                    Microphone.End(device);
                    micPhone.End();
                }
            }

            if (timeDown) {
                StopCoroutine("TimeDown");
            }
#endif
            loudness = 0;
        }

        private void GetAudioData(AudioClip clip) {
            var soundData = new float[clip.samples * clip.channels];
            clip.GetData(soundData, 0);
            var newData = new float[position * clip.channels];

            for (int i = 0; i < newData.Length; i++) {
                newData[i] = soundData[i];
            }
            var newClip = AudioClip.Create (clip.name,
                                            position,
                                            clip.channels,
                                            clip.frequency,
                                            false);
            
            newClip.SetData(newData, 0);
            long duration = (long)(newClip.length * 1000);

            MemoryStream amrStream = new MemoryStream();
            ConvertAndWriteWav(amrStream, newClip);
            WriteWavHeader(amrStream, newClip);
            int lengthSamples = newClip.samples;

            ClientEngine.RunTask(() => {
                micPhone.OnRecord(new RTMAudioData(AudioConvert.ConvertToAmrwb(amrStream.ToArray()), newData, RTMAudioData.DefaultCodec, lang, duration, lengthSamples, RECORD_SAMPLE_RATE));
            });
        }

        private void ConvertAndWriteWav(MemoryStream stream, AudioClip clip) {
            float[] samples = new float[clip.samples];
            clip.GetData(samples, 0);

            Int16[] intData = new Int16[samples.Length];

            Byte[] bytesData = new Byte[samples.Length * 2];

            int rescaleFactor = 32767;

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * rescaleFactor);
                Byte[] byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }
            stream.Write(bytesData, 0, bytesData.Length);
        }

        public static void WriteWavHeader(MemoryStream stream, AudioClip clip)
        {
            int hz = clip.frequency;
            int channels = clip.channels;
            int samples = clip.samples;

            stream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            stream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(stream.Length - 8);
            stream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            stream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            stream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            stream.Write(subChunk1, 0, 4);
            UInt16 one = 1;
            Byte[] audioFormat = BitConverter.GetBytes(one);
            stream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(channels);
            stream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(hz);
            stream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
            stream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(channels * 2);
            stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(bps);
            stream.Write(bitsPerSample, 0, 2);

            Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
            stream.Write(datastring, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
            stream.Write(subChunk2, 0, 4);
        }
    }
}

#endif
