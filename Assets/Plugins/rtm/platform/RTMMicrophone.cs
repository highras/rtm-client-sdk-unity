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

        public interface IMicrophone {
            string[] GetDevices();
            int GetPosition(string device);
            AudioClip Start(string device, bool loop, int lengthSec, int frequency);
            void End(string device);
            void OnRecord(RTMAudioData audioData);
        }

        public static int RECORD_TIME = 60;
        public static int SAMPLE_RATE = 16000;
        public static int SAMPLE_WINDOW = 128;

        private const float SENSIBILITY = 100.0f;

        private bool _isPause;
        private bool _isFocus;

        private string _device;
        private float _loudness;
        private bool _isRecording;

        private int _position;
        private AudioClip _clipRecord;
        private IMicrophone _micPhone;

        private object self_locker = new object();

        private VolumeType _volumeType = VolumeType.VolumePeak;
        public void SetVolumeType(VolumeType value) {
            lock (self_locker) {
                this._volumeType = value;
            }
        }

        void OnEnable() {
            this._isPause = false;
            this._isFocus = false;
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

        void UpdateLoudness() {
            lock (self_locker) {

                if (!this._isRecording)
                    return;

                if (this._volumeType == VolumeType.VolumePeak) {
                    this._loudness = LevelMax() * SENSIBILITY;
                } else {
                    this._loudness = VolumeRMS() * SENSIBILITY;
                }
            }
        }

        void Update() {

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
            FinishInput();
        }

        private float VolumeRMS() {
            if (this._micPhone == null) {
                return 0;
            }

            float[] data = new float[SAMPLE_WINDOW];
            int pos = this._micPhone.GetPosition(this._device) - (SAMPLE_WINDOW + 1);

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
            if (this._micPhone == null) {
                return 0;
            }

            float levelMax = 0;
            float[] data = new float[SAMPLE_WINDOW];
            int pos = this._micPhone.GetPosition(this._device) - (SAMPLE_WINDOW + 1);

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

        public void InitMic(string device, IMicrophone micPhone) {
            if (micPhone != null) {
                this._micPhone = micPhone;
            }

            if (device == null) {
                device = this._micPhone.GetDevices()[0];
            }

            //CancelInput();

            lock (self_locker) {
                this._device = device;
            }
        }

        public void StartInput() {
            if (this._micPhone == null) {
                return;
            }

            lock (self_locker) {
                if (this._isRecording) {
                    return;
                }
                this._isRecording = true;
                this._clipRecord = this._micPhone.Start(this._device, false, RECORD_TIME, SAMPLE_RATE);
            }
            StartCoroutine("TimeDown");
        }

        public void CancelInput() {
            if (this._micPhone == null) {
                return;
            }

            bool timeDown = false;
            lock (self_locker) {
                if (this._isRecording) {
                    timeDown = true;
                    this._isRecording = false;
                    this._position = this._micPhone.GetPosition(this._device);
                    this._micPhone.End(this._device);
                }
            }

            if (timeDown) {
                StopCoroutine("TimeDown");

                if (this._micPhone != null) {
                    AudioClip clip = this.GetAudioClip();
                    if (clip != null) {

                        this._micPhone.OnRecord(GetAudioData(clip));
                    }
                }
            }
        }

        RTMAudioData GetAudioData(AudioClip clip) {
            var soundData = new float[clip.samples * clip.channels];
            clip.GetData(soundData, 0);
            var newData = new float[this._position * clip.channels];

            //Copy the used samples to a new array
            for (int i = 0; i < newData.Length; i++) {
                newData[i] = soundData[i];
            }
            
            //One does not simply shorten an AudioClip,
            //    so we make a new one with the appropriate length
            var newClip = AudioClip.Create (clip.name,
                                            this._position,
                                            clip.channels,
                                            clip.frequency,
                                            false,
                                            false);
            
            newClip.SetData (newData, 0);        //Give it the data from the old clip

            long duration = (long)(newClip.length * 1000);
            return new RTMAudioData(RTMAudioManager.AudioClipToBytes(newClip), duration);
        }

        public void FinishInput() {
            CancelInput();
        }

        public float GetLoudness() {
            UpdateLoudness();
            lock (self_locker) {
                return this._loudness;
            }
        }

        private AudioClip GetAudioClip() {
            lock (self_locker) {
                if (this._clipRecord == null) {
                    return null;
                }

                short[] data = RTMAudioManager.AudioClipToShorts(this._clipRecord, 1.0f);
                return RTMAudioManager.ShortsToAudioClip(data, this._clipRecord.channels, this._clipRecord.frequency, false, 1.0f);
            }
        }
    }
}