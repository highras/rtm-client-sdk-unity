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

    private const string TOKEN_778899 = "A2A618F3F5A032989E68CC7A85D298F6";
    private const string TOKEN_777779 = "F2256AA281D26FE28CE90B7F164CFC35";

    public class BaseMicrophone : RTMMicrophone.IMicrophone {

        private RTMClient sendClient;

        public BaseMicrophone() {
            sendClient = new RTMClient(
                "52.82.27.68:13325",
                11000001,
                778899,
                TOKEN_778899,
                RTMConfig.TRANS_LANGUAGE.en,
                new Dictionary<string, string>(),
                true,
                20 * 1000,
                true
            );
            sendClient.GetEvent().AddListener("login", (evd) => {
                if (evd.GetException() == null) {
                    Debug.Log("778899 login!");
                } else {
                    Debug.Log(evd.GetException());
                }
            });
            sendClient.GetEvent().AddListener("error", (evd) => {
                Debug.Log(evd.GetException());
            });
            sendClient.Login(null);
        }

        public void Destroy() {
            if (sendClient != null) {
                sendClient.Destroy();
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
        public void OnRecord(RTMAudioData audioData) {
            Debug.Log("end record");

            if (sendClient != null) {
                sendClient.SendAudio(123456, audioData, "", 0, 20 * 1000, (cbd) => {
                    if (cbd.GetException() != null) {
                        Debug.Log(cbd.GetException());
                    } else {
                        Debug.Log("778899 send audio!");
                    }
                });
            }
        }
    }

    private RTMClient receiveClient;
    private BaseMicrophone _micphone;
    private byte[] _audioBytes;
    private object self_locker = new object();

    /**
     *  录音测试脚本
     */
    public SingleMicphone() {}

    public void StartTest(byte[] fileBytes) {
        receiveClient = new RTMClient(
            "52.82.27.68:13325",
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
        receiveClient.GetEvent().AddListener("login", (evd) => {
            if (evd.GetException() == null) {
                Debug.Log("777779 login!");
            } else {
                Debug.Log(evd.GetException());
            }
        });
        receiveClient.GetEvent().AddListener("error", (evd) => {
            Debug.Log(evd.GetException());
        });
        RTMProcessor processor = receiveClient.GetProcessor();
        processor.AddPushService(RTMConfig.SERVER_PUSH.recvAudio, (data) => {
            Debug.Log("777779 receive audio!");
            lock (self_locker) {
                self._audioBytes = (byte[])data["msg"];
            }
        });
        receiveClient.Login(null);
        this._micphone = new BaseMicrophone();
        RTMMicrophone.Instance.InitMic(null, this._micphone);
        Debug.Log("microphone start input!");
        RTMMicrophone.Instance.StartInput();
    }

    public void Update() {
        AudioClip clip = null;

        lock (self_locker) {
            if (this._audioBytes != null) {
                RTMPcmData pcmData = RTMAudioManager.GetPcmData(this._audioBytes);
                 clip = AudioClip.Create("testSound", pcmData.SampleCount, 1, pcmData.Frequency, false, false);
                clip.SetData(pcmData.LeftChannel, 0);
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

        if (receiveClient != null) {
            receiveClient.Destroy();
        }

        if (this._micphone != null) {
            this._micphone.Destroy();
        }
    }
}