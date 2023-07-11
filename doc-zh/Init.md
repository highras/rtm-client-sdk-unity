# 初始化

## SDK初始化

+ SDK初始化需要在主线程调用
```
	ClientEngine.Init();
    RTMControlCenter.Init();
```

## 创建LDEngine实例

```
    LDEngine engine = LDEngine.CreateEngine(rtmServerEndpoint, pid, uid);
```

## 设置回调类

### 基础回调

```
    BasePushProcessor basePushProcessor = new BasePushProcessor();
    processor.KickoutCallback = () =>{};
    processor.ReloginWillStartCallback = (int lastErrorCode, int retriedCount) =>{return true;};
    processor.ReloginCompletedCallback = (bool successful, bool retryAgain, int errorCode, int retriedCount) =>{};
    processor.SessionClosedCallback = (int ClosedByErrorCode) =>{};

    engine.SetBasePushProcessor(basePushProcessor);
```

+ ***RTM回调与IM回调互斥,只能设置其中一个***

### RTM回调

```
    RTMPushProcessor rtmPushProcessor = new RTMPushProcessor();
    engine.SetRTMPushProcessor(rtmPushProcessor);
```

### IM回调

```
    IMPushProcessor imPushProcessor = new IMPushProcessor();
    engine.SetIMPushProcessor(imPushProcessor);
```

## 登陆

### 登陆接口

```
    public bool Login(AuthDelegate callback, string token, int timeout = 0);
    public bool LoginV2(AuthDelegate callback, string token, long ts, int timeout = 0);
```
### RTM登陆

```
    engine.RTM.Login((long projectId, long uid, bool successful, int errorCode) => {
        if (successful)
            Debug.Log("login success.");
        else
            Debug.Log("login failed, error code: " + errorCode);
    }, token, timeout);
```

### IM登陆

```
    engine.IM.Login((long projectId, long uid, bool successful, int errorCode) => {
        if (successful)
            Debug.Log("RTM 1 login success.");
        else
            Debug.Log("RTM 1 login failed, error code: " + errorCode);
    }, token, timeout);
```

