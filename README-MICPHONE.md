# fpnn rtm sdk unity #

#### 实现接口 ####
```c#
using System;
using UnityEngine;

using com.rtm;

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
    public void OnRecord(AudioClip clip) {
        //完成音频采集
    }
}

RTMMicrophone.Instance.InitMic(null, new BaseMicrophone());
//开始音频采集
//RTMMicrophone.Instance.StartInput();
```

#### 其他 ####
* 使用`UnityEngine.Microphone`类会引入相关权限, 具体参考[Microphone](https://docs.unity3d.com/ScriptReference/Microphone.html)
* 使用`UnityEngine.AudioSource`类对[AudioClip](https://docs.unity3d.com/ScriptReference/AudioClip.html)音频片段进行播放控制, 具体参考[AudioSource](https://docs.unity3d.com/ScriptReference/AudioSource.html)

#### API ####
* `InitMic(string device, IMicrophone micPhone)`: 音频采集初始化
    * `device`: **(string)** 音频采集设备, 使用默认设备传`null`
    * `micPhone`: **(IMicrophone)** 音频采集接口实现

* `StartInput()`: 开始采集音频数据, 默认长度15s

* `CancelInput()`: 取消采集音频数据

* `FinishInput()`: 提前完成采集音频数据

* `SetVolumeType(VolumeType value)`: 设置音量返回类型, 参考`RTMMicrophone.VolumeType`成员, 默认`VolumeType.VolumePeak`

* `GetLoudness()`: 返回声音强度

* `GetAudioClip()`: 返回当次采集的音频片段`AudioClip`

* `GetAdpcmData()`: 返回当次采集的音频数据, 压缩格式[IMA-ADPCM](https://wiki.multimedia.cx/index.php/IMA_ADPCM)

* `GetAudioClip(byte[] adpcmData)`: 返回压缩数据的音频片段`AudioClip`
    * `adpcmData`: **(byte[])** 音频数据(压缩格式)
