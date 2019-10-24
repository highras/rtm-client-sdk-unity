using UnityEngine;
using System.Collections.Generic;

namespace com.rtm {

    public class RTMAudioManager {
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
            AudioSource.PlayClipAtPoint(clip, pos);
        }

        public static byte[] AudioClipToBytes(AudioClip clip) {
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
        }

        public static short[] AudioClipToShorts(AudioClip clip, float gain=1.0f) {
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
        }

        public static short[] AudioDataToShorts(float[] samples, int channels, float gain=1.0f) {
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
        }

        public static AudioClip BytesToAudioClip(byte[] data, int channels, int frequency, bool threedimensional, float gain=1.0f) {
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
        }

        public static AudioClip ShortsToAudioClip(short[] data, int channels, int frequency, bool threedimensional, float gain=1.0f) {
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
        }
    }
}