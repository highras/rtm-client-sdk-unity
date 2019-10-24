using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameDevWare.Serialization;
using com.fpnn;

using UnityEngine;

namespace com.rtm {

    public class RTMMicrophone : Singleton<RTMMicrophone> {

        public enum VolumeType {
            VolumeRMS,
            VolumePeak
        }

        private static bool isInit;
        private static object lock_obj = new object();
        public static bool HasInit() {
            lock (lock_obj) {
                return RTMMicrophone.isInit;
            }
        }

        public Action<AudioClip> OnRecord;

        private VolumeType _volumeType = VolumeType.VolumePeak;
        public void SetVolumeType(VolumeType value) {
            lock (self_locker) {
                this._volumeType = value;
            }
        }

        private const int RECORD_TIME = 20;
        private const int SAMPLE_RATE = 8000;
        private const int SAMPLE_WINDOW = 128;

        private const float SENSIBILITY = 100.0f;

        private bool _isPause;
        private bool _isFocus;

        private string _device;
        private float _loudness;
        private bool _isRecording;

        private int _position;
        private AudioClip _clipRecord;

        private RTMAdpcm _adpcm = new RTMAdpcm();
        private object self_locker = new object();

        void OnEnable() {
            this._isPause = false;
            this._isFocus = false;

            lock (lock_obj) {
                if (RTMMicrophone.isInit) {
                    return;
                }

                InitMic(null);
            }
        }

        void OnDisable() {
            StopAllCoroutines();
            CancelInput();
        }

        void OnApplicationPause() {
            if (!this._isPause) {
                //bg
                CancelInput();
            } else {
                this._isFocus = true;
            }

            this._isPause = true;
        }

        void OnApplicationFocus() {
            if (this._isFocus) {
                this._isPause = false;
                this._isFocus = false;
            }

            if (this._isPause) {
                this._isFocus = true;
                //fg
            }
        }

        void Update() {
            lock (self_locker) {
                if (this._volumeType == VolumeType.VolumePeak) {
                    this._loudness = LevelMax() * SENSIBILITY;
                } else {
                    this._loudness = VolumeRMS() * SENSIBILITY;
                }
            }
        }

        private IEnumerator TimeDown() {
            int time = 0;

            while (++time < RECORD_TIME) {
                lock (self_locker) {
                    if (!this._isRecording) {
                        yield return 0;
                    }
                }

                yield return new WaitForSeconds(1);
            }

            StopInput();
        }

        private float VolumeRMS() {
            float[] data = new float[SAMPLE_WINDOW];
            int pos = Microphone.GetPosition(this._device) - (SAMPLE_WINDOW + 1);

            if (pos < 0) {
                return 0;
            }

            var sum = 0.0f;
            this._clipRecord.GetData(data, pos);

            for (int i = 0; i < SAMPLE_WINDOW; i++) {
                sum += data[i] * data[i];
            }

            return Mathf.Sqrt(sum / SAMPLE_WINDOW);
        }

        private float LevelMax() {
            float levelMax = 0;
            float[] data = new float[SAMPLE_WINDOW];
            int pos = Microphone.GetPosition(this._device) - (SAMPLE_WINDOW + 1);

            if (pos < 0) {
                return 0;
            }

            this._clipRecord.GetData(data, pos);

            for (int i = 0; i < SAMPLE_WINDOW; i++) {
                float wavePeak = data[i] * data[i];

                if (levelMax < wavePeak) {
                    levelMax = wavePeak;
                }
            }

            return levelMax;
        }

        public void InitMic(string device) {
            CancelInput();

            lock (self_locker) {
                if (device == null) {
                    device = Microphone.devices[0];
                }

                this._device = device;
            }
        }

        public void StartInput() {
            lock (self_locker) {
                if (this._isRecording) {
                    return;
                }

                this._isRecording = true;
                this._clipRecord = Microphone.Start(this._device, false, RECORD_TIME, SAMPLE_RATE);
                StartCoroutine(TimeDown());
            }
        }

        public void CancelInput() {
            lock (self_locker) {
                if (this._isRecording) {
                    this._isRecording = false;
                    this._position = Microphone.GetPosition(this._device);
                    Microphone.End(this._device);
                    StopCoroutine(TimeDown());
                }
            }
        }

        public void StopInput() {
            CancelInput();

            if (OnRecord != null) {
                AudioClip clip = GetAudioClip();
                OnRecord(clip);
            }
        }

        public float GetLoudness() {
            lock (self_locker) {
                return this._loudness;
            }
        }

        public AudioClip GetAudioClip() {
            lock (self_locker) {
                if (this._clipRecord == null) {
                    return null;
                }

                short[] data = RTMAudioManager.AudioClipToShorts(this._clipRecord, 1.0f);
                return RTMAudioManager.ShortsToAudioClip(data, this._clipRecord.channels, this._clipRecord.frequency, false, 1.0f);
            }
        }

        public byte[] GetAdpcmData() {
            byte[] bytes;
            short[] data;
            int channels = 1;
            int frequency = SAMPLE_RATE;

            lock (self_locker) {
                if (this._clipRecord == null) {
                    return null;
                }

                channels = this._clipRecord.channels;
                frequency = this._clipRecord.frequency;
                data = RTMAudioManager.AudioClipToShorts(this._clipRecord, 1.0f);
            }

            if (data == null) {
                return null;
            }

            byte[] adpcmData = this._adpcm.Encode(data);

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
        }

        public AudioClip GetAudioClip(byte[] adpcmData) {
            if (adpcmData == null || adpcmData.Length < 8) {
                return null;
            }

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
            short[] data = this._adpcm.Decode(bytes);
            return RTMAudioManager.ShortsToAudioClip(data, channels, frequency, false, 1.0f);
        }
    }
}