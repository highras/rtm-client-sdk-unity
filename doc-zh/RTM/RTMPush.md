# 消息相关推送文档

## RTMPushProcessor类
+ 回调事件类为RTMPushProcessor
+ 所有回调会在主线程触发

## Delegate

### KickoutRoomDelegate
    public delegate void KickoutRoomDelegate(long roomId);

参数:

+ `long roomId`

    被踢出的房间ID

### RTMPushMessageDelegate
    public delegate void RTMPushMessageDelegate(MessageCategory messageCategory, RTMMessage message);

参数:

+ `MessageCategory messageCategory`

    消息种类

+ `RTMMessage message`

    消息内容

## 推送回调函数

### 被踢出房间
    public KickoutRoomDelegate KickoutRoomCallback;

### 收到基础消息
    public RTMPushMessageDelegate PushBasicMessageCallback;

### 收到聊天消息
    public RTMPushMessageDelegate PushChatMessageCallback;

### 收到命令消息
    public RTMPushMessageDelegate PushCmdMessageCallback;

### 收到文件消息
    public RTMPushMessageDelegate PushFileMessageCallback;

### 收到语音文件消息
    public RTMPushMessageDelegate PushAudioMessageCallback;