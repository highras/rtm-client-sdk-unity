using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using GameDevWare.Serialization;
using UnityEngine;

using com.fpnn;
using com.rtm;

public class SingleMicphone : Main.ITestCase {

    private const string TOKEN_778899 = "3914E4172266948990B3929AD9E7F4D2";
    private const string TOKEN_777779 = "03F1B6CB47D249704ED5C0F577D3CBD5";

    public class BaseMicrophone : RTMMicrophone.IMicrophone {

        private RTMClient _client;

        public BaseMicrophone() {
            this._client = new RTMClient(
                "52.83.245.22:13325",
                11000001,
                778899,
                TOKEN_778899,
                RTMConfig.TRANS_LANGUAGE.en,
                new Dictionary<string, string>(),
                true,
                20 * 1000,
                true
            );
            this._client.GetEvent().AddListener("login", (evd) => {
                if (evd.GetException() == null) {
                    Debug.Log("778899 login!");
                } else {
                    Debug.Log(evd.GetException());
                }
            });
            this._client.GetEvent().AddListener("error", (evd) => {
                Debug.Log(evd.GetException());
            });
            this._client.Login(null);
        }

        public void Destroy() {
            if (this._client != null) {
                this._client.Destroy();
            }
        }

        public string[] GetDevices() {
            return Microphone.devices;
        }
        public int GetPosition(string device) {
            return Microphone.GetPosition(device);
        }
        public AudioClip Start(string device, bool loop, int lengthSec, int frequency) {
            return Microphone.Start(device, loop, lengthSec, frequency);
        }
        public void End(string device) {
            Microphone.End(device);
        }
        public void OnRecord(AudioClip clip) {
            byte[] data = RTMMicrophone.Instance.GetAdpcmData();
            Debug.Log("end record, adpcm bytearray len: " + data.Length);

            if (this._client != null) {
                this._client.SendAudio(777779, data, "", 0, 20 * 1000, (cbd) => {
                    if (cbd.GetException() != null) {
                        Debug.Log(cbd.GetException());
                    } else {
                        Debug.Log("778899 send audio!");
                    }
                });
            }
        }
    }

    private RTMClient _client;
    private BaseMicrophone _micphone;
    private byte[] _audioBytes;
    private object self_locker = new object();

    /**
     *  录音测试脚本
     */
    public SingleMicphone() {}

    public void StartTest(byte[] fileBytes) {
        this._client = new RTMClient(
            "52.83.245.22:13325",
            11000001,
            777779,
            TOKEN_777779,
            RTMConfig.TRANS_LANGUAGE.en,
            new Dictionary<string, string>(),
            true,
            20 * 1000,
            true
        );
        SingleMicphone self = this;
        this._client.GetEvent().AddListener("login", (evd) => {
            if (evd.GetException() == null) {
                Debug.Log("777779 login!");
            } else {
                Debug.Log(evd.GetException());
            }
        });
        this._client.GetEvent().AddListener("error", (evd) => {
            Debug.Log(evd.GetException());
        });
        RTMProcessor processor = this._client.GetProcessor();
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvAudio, (data) => {
            Debug.Log("777779 receive audio!");

            lock (self_locker) {
                self._audioBytes = (byte[])data["msg"];
            }
        });
        this._client.Login(null);
        this._micphone = new BaseMicrophone();
        RTMMicrophone.Instance.InitMic(null, this._micphone);
        Debug.Log("microphone start input!");
        RTMMicrophone.Instance.StartInput();
    }

    public void Update() {
        AudioClip clip = null;

        lock (self_locker) {
            if (this._audioBytes != null) {
                clip = RTMMicrophone.Instance.GetAudioClip(this._audioBytes);
                this._audioBytes = null;
            }
        }

        if (clip != null) {
            Debug.Log("audio play!");
            RTMAudioManager.PlayAudioClip(clip, Vector3.zero);
        }
    }

    public void StopTest() {
        RTMMicrophone.Instance.CancelInput();

        if (this._client != null) {
            this._client.Destroy();
        }

        if (this._micphone != null) {
            this._micphone.Destroy();
        }
    }
}