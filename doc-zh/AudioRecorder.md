# 录音接口文档

## 接口

+ 录音相关接口都在AudioRecorderNative类中
  
### 初始化

    static public void Init(string language, IAudioRecorderListener listener);

参数:

+ `string langugae`

    语言

+ `IAudioRecorderListener listener`

    回调对象

### 开始录音

    static public void StartRecord();

### 停止录音

    static public void StopRecord();

### 取消录音

    static public void CancelRecord();

### 开始播放

    static public void Play(RTMAudioData data);

### 停止播放

    static public void StopPlay()

## 回调类

+ 回调类需继承IAudioRecorderListener

### 录音开始

    void RecordStart(bool success);

参数:

+ `bool success`
    是否成功开始录音

### 录音结束

    void RecordEnd();

### 录音结果

    void OnRecord(RTMAudioData audioData);

参数:

+ `RTMAudioData audioData`
    录音内容

### 音量

    void OnVolumn(double db);

参数:

+ `double db`
    分贝

### 播放开始

    void PlayStart(bool success);

参数:

+ `bool success`
    是否成功开始播放

### 播放结束

    void PlayEnd();

