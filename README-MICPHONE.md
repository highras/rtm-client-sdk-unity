# fpnn rtm sdk unity #

#### 实现接口 ####
```c#
//Microphone
class BaseMicrophone : RTMMicrophone.IMicrophone {
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
    public void OnRecord(AudioClip clip) {}
}
```

#### API ####

* `InitMic(string device, IMicrophone micPhone)`: 音频录制初始化
    * `device`: **(string)** 音频录制设备, 使用默认设备传`null`
    * `micPhone`: **(IMicrophone)** 音频录制接口实现

* `StartInput()`: 开始录制音频数据, 默认长度15s

* `CancelInput()`: 取消录制音频数据

* `FinishInput()`: 提前完成录制音频数据

* `SetVolumeType(VolumeType value)`: 设置音量返回类型, 参考`RTMMicrophone.VolumeType`成员, 默认`VolumeType.VolumePeak`

* `GetLoudness()`: 返回声音强度

* `GetAudioClip()`: 返回当次录制的音频片段`AudioClip`

* `GetAdpcmData()`: 返回当次录制的音频数据, 压缩格式IMA-ADPCM(https://wiki.multimedia.cx/index.php/IMA_ADPCM)

* `GetAudioClip(byte[] adpcmData)`: 返回压缩数据的音频片段`AudioClip`
    * `adpcmData`: **(byte[])** 音频数据(压缩格式)
