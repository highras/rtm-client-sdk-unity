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
        private int maxRecordSeconds = 60;

        private bool isPause;
        private bool isFocus;

        private string lang;
        private string device;
        private bool isRecording;

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
            FinishInput();
        }

        void OnApplicationPause() {
            if (!this.isPause) {
                FinishInput();
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

        private IEnumerator TimeDown() {
            int time = 0;
            while (++time < this.maxRecordSeconds) {
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
            this.lang = lang;
            if (micPhone != null) {
                this.micPhone = micPhone;
            }

            if (Microphone.devices.Length == 0) {
                return;
            }

            if (device == null) {
                device = Microphone.devices[0];
            }

            lock (selfLocker) {
                this.device = device;
            }
#endif
        }

        public void StartInput(int maxRecordSeconds = 60) {
#if RTM_BUILD_NO_AUDIO
            throw new Exception("Audio is disabled, please remove the RTM_BUILD_NO_AUDIO define in \"Scripting Define Symbols\"");
#else
            if (micPhone == null) {
                return;
            }

            lock (selfLocker) {
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
                    micPhone.End();
                }
            }

            if (timeDown) {
                StopCoroutine("TimeDown");

                if (micPhone != null && clipRecord != null) {
                    micPhone.OnRecord(GetAudioData(clipRecord));
                }
            }
#endif
        }

        private RTMAudioData GetAudioData(AudioClip clip) {
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

            RTMAudioHeader rtmAudioHeader = new RTMAudioHeader(RTMAudioHeader.CurrentVersion, RTMAudioHeader.ContainerType.CodecInherent, RTMAudioHeader.CodecType.AmrWb, lang, duration, RECORD_SAMPLE_RATE);

            return new RTMAudioData(rtmAudioHeader, GetRtmAudioData(rtmAudioHeader, newClip, duration), newData, duration, newClip.samples, RECORD_SAMPLE_RATE);
        }

        private byte[] GetRtmAudioData(RTMAudioHeader rtmAudioHeader, AudioClip clip, long duration)
        {
            MemoryStream amrStream = new MemoryStream();
            ConvertAndWriteWav(amrStream, clip);
            WriteWavHeader(amrStream, clip);
            byte[] amrArray = AudioConvert.ConvertToAmrwb(amrStream.ToArray());

            MemoryStream audioStream = new MemoryStream();
            byte[] rtmHeader = rtmAudioHeader.HeaderArray;
            audioStream.Write(rtmHeader, 0, rtmHeader.Length);
            audioStream.Write(amrArray, 0, amrArray.Length);
            return audioStream.ToArray();
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
