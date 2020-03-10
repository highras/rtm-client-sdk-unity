using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using GameDevWare.Serialization;
using com.fpnn;

namespace com.rtm {

    public class RTMAudioManager {

        private static RTMAdpcm adpcm = new RTMAdpcm();
        private static List<AudioClip> clipPool = new List<AudioClip>();

        public static AudioClip GetOrCreateAudioClip(int lenSamples, int channels, int frequency, bool threeD) {
            AudioClip pooled_clip = ReturnPooledAudioClip(lenSamples, channels);

            if (pooled_clip != null) {
                RemoveFromPool(pooled_clip);
                return pooled_clip;
            } else {
                return CreateAudioClip(lenSamples, channels, frequency, threeD);
            }
        }

        public static AudioClip CreateAudioClip(int lenSamples, int channels, int frequency, bool threeD) {
            return AudioClip.Create("clip", lenSamples / channels, channels, frequency, threeD, false);
        }

        private static void RemoveFromPool(AudioClip clip) {
            clipPool.Remove(clip);
        }

        private static AudioClip ReturnPooledAudioClip(int samples, int channels) {
            foreach (AudioClip clip in clipPool) {
                if (clip.channels == channels && clip.samples == samples / channels) {
                    return clip;
                } else {
                    continue;
                }
            }

            return null;
        }

        public static void PoolAudioClip(AudioClip clip) {
            clipPool.Add(clip);
        }

        public static void PlayAudioClip(AudioClip clip, Vector3 pos) {
            if (clip == null) {
                return;
            }

            try {
                AudioSource.PlayClipAtPoint(clip, pos);
            } catch (Exception ex) {
                Debug.LogWarning(ex);
            }
        }

        public static byte[] AudioClipToBytes(AudioClip clip) {
            MemoryStream ms = new MemoryStream();
            ConvertAndWriteWav(ms, clip);
            WriteWavHeader(ms, clip); 
            return ConvertToAmrwb(ms.ToArray());
        }

        public static byte[] ConvertToAmrwb(byte[] wavBuffer)
        {
            int status = 0;
            int amrSize = 0;
            
            IntPtr wavSrcPtr = Marshal.AllocHGlobal(wavBuffer.Length);
            Marshal.Copy(wavBuffer, 0, wavSrcPtr, wavBuffer.Length);

            IntPtr amrPtr = RTMAudioConvert.convert_wav_to_amrwb(wavSrcPtr, wavBuffer.Length, ref status, ref amrSize);

            Marshal.FreeHGlobal(wavSrcPtr);

            if (amrPtr != null && status == 0) {
                byte[] amrBuffer = new byte[amrSize];
                Marshal.Copy(amrPtr, amrBuffer, 0, amrSize);
                RTMAudioConvert.free_memory(amrPtr);
                return amrBuffer;
            }

            if (amrPtr != null)
                RTMAudioConvert.free_memory(amrPtr);

            return null;
        }

        public static byte[] ConvertToWav(byte[] amrBuffer)
        {
            int status = 0;
            int wavSize = 0;
            
            IntPtr amrSrcPtr = Marshal.AllocHGlobal(amrBuffer.Length);
            Marshal.Copy(amrBuffer, 0, amrSrcPtr, amrBuffer.Length);

            IntPtr wavPtr = RTMAudioConvert.convert_amrwb_to_wav(amrSrcPtr, amrBuffer.Length, ref status, ref wavSize);

            Marshal.FreeHGlobal(amrSrcPtr);

            if (wavPtr != null && status == 0) {
                byte[] wavBuffer = new byte[wavSize];
                Marshal.Copy(wavPtr, wavBuffer, 0, wavSize);
                RTMAudioConvert.free_memory(wavPtr);
                return wavBuffer;
            }

            if (wavPtr != null)
                RTMAudioConvert.free_memory(wavPtr);

            return null;
        }

        public static void ConvertAndWriteWav(MemoryStream stream, AudioClip clip) {
            float[] samples = new float[clip.samples];
            clip.GetData(samples, 0);

            Int16[] intData = new Int16[samples.Length];

            Byte[] bytesData = new Byte[samples.Length * 2];

            int rescaleFactor = 32767; //to convert float to Int16  

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

            UInt16 two = 2;
            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);
            stream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(channels);
            stream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(hz);
            stream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2  
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

        public static short[] AudioClipToShorts(AudioClip clip, float gain=1.0f) {
            try {
                float[] samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);

                short[] data = new short[clip.samples * clip.channels];
                for (int i = 0; i < samples.Length; i++) {
                    //convert to the -3267 to +3267 range
                    float g = samples[i] * gain;

                    if (Mathf.Abs(g) > 1.0f) {
                        if (g > 0) {
                            g = 1.0f;
                        } else {
                            g = -1.0f;
                        }
                    }
                    float conv = g * 3267.0f;
                    //int c = Mathf.RoundToInt(conv);
                    data[i] = (short)conv;
                }
                return data;
            } catch(Exception ex) {
                Debug.LogWarning(ex);
            }
            return null;
        }

        public static short[] AudioDataToShorts(float[] samples, int channels, float gain=1.0f) {
            try {
                short[] data = new short[samples.Length * channels];
                for (int i = 0; i < samples.Length; i++) {
                    //convert to the -3267 to +3267 range
                    float g = samples[i] * gain;
                    if (Mathf.Abs(g) > 1.0f) {
                        if (g > 0) {
                            g = 1.0f;
                        } else {
                            g = -1.0f;
                        }
                    }
                    float conv = g * 3267.0f;
                    //int c = Mathf.RoundToInt(conv);
                    data[i] = (short)conv;
                }
                return data;
            } catch(Exception ex) {
                Debug.LogWarning(ex);
            }
            return null;
        }

        public static AudioClip BytesToAudioClip(byte[] data, int channels, int frequency, bool threedimensional, float gain=1.0f) {
            try {
                float[] samples = new float[data.Length];

                for (int i = 0; i < samples.Length; i++) {
                    //convert to integer in -128 to +128 range
                    int c = (int)data[i];
                    c -= 127;
                    samples[i] = ((float)c / 128.0f) * gain;
                }

                AudioClip clip = GetOrCreateAudioClip(data.Length / channels, channels, frequency, threedimensional);
                clip.SetData(samples, 0);
                return clip;
            } catch(Exception ex) {
                Debug.LogWarning(ex);
            }
            return null;
        }

        public static AudioClip ShortsToAudioClip(short[] data, int channels, int frequency, bool threedimensional, float gain=1.0f) {
            try {
                float[] samples = new float[data.Length];

                for (int i = 0; i < samples.Length; i++) {
                    //convert to float in the -1 to 1 range
                    int c = (int)data[i];
                    samples[i] = ((float)c / 3267.0f) * gain;
                }

                AudioClip clip = GetOrCreateAudioClip(data.Length / channels, channels, frequency, threedimensional);

                if (samples.Length > 0) {
                    clip.SetData(samples, 0);
                }
                return clip;
            } catch(Exception ex) {
                Debug.LogWarning(ex);
            }
            return null;
        }

        public static byte RTM_HEADER_VERSION = 1;
        public static byte RTM_HEADER_CONTAINER_TYPE = 0;
        public static byte RTM_HEADER_CODEC_TYPE = 1; // amr-wb

        public static byte[] AddRtmAudioHeader(RTMAudioData audio, string lang) {
            MemoryStream stream = new MemoryStream();

            IDictionary<string, object> infoData = new Dictionary<string, object>() {
                { "lang", lang},
                { "dur", audio.Duration },
                { "srate", RTMMicrophone.SAMPLE_RATE }
            };
            byte[] bytes = new byte[0];

            try {
                using (MemoryStream outputStream = new MemoryStream()) {
                    MsgPack.Serialize(infoData, outputStream);
                    outputStream.Seek(0, SeekOrigin.Begin);
                    bytes = outputStream.ToArray();
                }
            } catch (Exception ex) {
                ErrorRecorderHolder.recordError(ex);
                return null;
            }


            stream.Write(new byte[4]{RTM_HEADER_VERSION, RTM_HEADER_CONTAINER_TYPE, RTM_HEADER_CODEC_TYPE, 1}, 0, 4);
            stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            stream.Write(bytes, 0, bytes.Length);
            stream.Write(audio.AmrData, 0, audio.AmrData.Length);
            return stream.ToArray();
        }

        public static RTMPcmData GetPcmData(byte[] rtmAudioBuffer) {
            int offset = 0;
            if (rtmAudioBuffer.Length < 4)
                return null;
            byte version = rtmAudioBuffer[0];
            byte containerType = rtmAudioBuffer[1];
            byte codecType = rtmAudioBuffer[2];
            byte infoDataCount = rtmAudioBuffer[3];

            if (version != RTM_HEADER_VERSION || containerType != RTM_HEADER_CONTAINER_TYPE || codecType != RTM_HEADER_CODEC_TYPE) {
                return null;
            }
            offset += 4;

            for (byte i = 0; i < infoDataCount; i++) {
                int sectionLength = BitConverter.ToInt32(rtmAudioBuffer, offset);
                offset += 4;
                offset += sectionLength;
            }
            if (offset >= rtmAudioBuffer.Length) {
                return null;
            }

            byte[] amrBuffer = new byte[rtmAudioBuffer.Length - offset];
            Array.Copy(rtmAudioBuffer, offset, amrBuffer, 0, rtmAudioBuffer.Length - offset);
            byte[] wavBuffer = ConvertToWav(amrBuffer);
            return new RTMPcmData(wavBuffer);
        }
    }
}