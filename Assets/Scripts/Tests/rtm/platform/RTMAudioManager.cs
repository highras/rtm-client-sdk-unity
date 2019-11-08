using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

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
            try {
                float[] samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);

                byte[] data = new byte[clip.samples * clip.channels];
                for (int i = 0; i < samples.Length; i++) {
                    //convert to the -128 to +128 range
                    float conv = samples[i] * 128.0f;
                    int c = Mathf.RoundToInt(conv);
                    c += 127;

                    if (c < 0) {
                        c = 0;
                    }

                    if (c > 255) {
                        c = 255;
                    }

                    data[i] = (byte)c;
                }
                return data;
            } catch(Exception ex) {
                Debug.LogWarning(ex);
            }
            return null;
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

        public static byte[] EncodeAudioClip(AudioClip clip) {
            byte[] bytes;
            short[] data;
            int channels = 1;
            int frequency = RTMMicrophone.SAMPLE_RATE;

            if (clip == null) {
                return null;
            }

            channels = clip.channels;
            frequency = clip.frequency;

            try {
                data = RTMAudioManager.AudioClipToShorts(clip, 1.0f);

                if (data == null) {
                    return null;
                }

                byte[] adpcmData = adpcm.Encode(data);

                if (adpcmData == null || adpcmData.Length == 0) {
                    return null;
                }

                using (MemoryStream stream = new MemoryStream()) {
                    //channels
                    bytes = BitConverter.GetBytes(channels);
                    stream.Write(bytes, 0, bytes.Length);
                    //frequency
                    bytes = BitConverter.GetBytes(frequency);
                    stream.Write(bytes, 0, bytes.Length);
                    //adpcmData
                    stream.Write(adpcmData, 0, adpcmData.Length);
                    bytes = stream.ToArray();
                }
                return bytes;
            } catch(Exception ex) {
                Debug.LogWarning(ex);
            }
            return null;
        }

        public static AudioClip DncodeAdpcmData(byte[] adpcmData) {
            if (adpcmData == null || adpcmData.Length < 8) {
                return null;
            }

            try {
                //channels
                byte[] bytes = new byte[4];
                Buffer.BlockCopy(adpcmData, 0, bytes, 0, bytes.Length);
                int channels = (int)BitConverter.ToUInt32(bytes, 0);
                //frequency
                bytes = new byte[4];
                Buffer.BlockCopy(adpcmData, 4, bytes, 0, bytes.Length);
                int frequency = (int)BitConverter.ToUInt32(bytes, 0);
                //adpcmData
                bytes = new byte[adpcmData.Length - 8];
                Buffer.BlockCopy(adpcmData, 8, bytes, 0, bytes.Length);
                short[] data = adpcm.Decode(bytes);
                return RTMAudioManager.ShortsToAudioClip(data, channels, frequency, false, 1.0f);
            } catch(Exception ex) {
                Debug.LogWarning(ex);
            }
            return null;
        }
    }
}