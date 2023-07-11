# 基础推送文档

## BasePushProcessor
+ 回调事件类为BasePushProcessor
+ 所有回调会在主线程触发

## Delegate

### SessionClosedDelegate
    public delegate void SessionClosedDelegate(int ClosedByErrorCode);

参数:

+ `int ClosedByErrorCode`
    导致连接关闭的错误码

### ReloginWillStartDelegate
    public delegate bool ReloginWillStartDelegate(int lastErrorCode, int retriedCount);

参数:

+ `int lastErrorCode`
    上次登陆失败的错误码

+ `int retriedCount`
    已尝试重新登陆次数

### ReloginCompletedDelegate
    public delegate void ReloginCompletedDelegate(bool successful, bool retryAgain, int errorCode, int retriedCount);

参数:

+ `bool successful`
    是否登陆成功

+ `bool retryAgain`
    是否继续重试

+ `int errorCode`
    本次登陆的错误码

+ `int retriedCount`
    已尝试重新登陆次数

### KickOutDelegate
    public delegate void KickOutDelegate();

## 推送回调函数

### 连接关闭或登陆失败
    public SessionClosedDelegate SessionClosedCallback;

### 开始自动重连
    public ReloginWillStartDelegate ReloginWillStartCallback;

### 自动重连结束
    public ReloginCompletedDelegate ReloginCompletedCallback;

### 被踢下线
    public KickOutDelegate KickoutCallback;
