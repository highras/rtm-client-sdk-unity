# fpnn rtm sdk unity #

#### 依赖 ####
* [fpnn.unitypackage](https://github.com/highras/fpnn-sdk-unity)
* [Json&MsgPack.unitypackage](https://github.com/deniszykov/msgpack-unity3d)

#### IPV6 ####
* `SOCKET`链接支持`IPV6`接口
* 兼容`DNS64/NAT64`网络环境

#### 其他 ####
* 在`Unity`主线程中初始化`RTMRegistration.Register()`
* 若`RTMRegistration`已初始化,`RTMClient`可在任意线程中构造和使用(线程安全)
* 异步函数均由子线程呼叫,不要在其中使用仅UI线程的函数,不要阻塞异步函数
* 默认连接会自动保持,如实现按需连接则要通过`Login()`和`Close()`进行连接或关闭处理
* 或可通过`login`和`close`事件以及注册`ping`服务来对连接进行管理
* 消息发送接口仅支持`UTF-8`格式编码的`string`类型数据,`Binary`数据需进行`Base64`编解码

#### Events ####
* `event`:
    * `login`: 登陆
        * `exception`: **(Exception)** auth失败, token失效需重新获取
        * `payload`: **(IDictionary)** 当前连接的RTMGate地址, 可在本地缓存, 下次登陆可使用该地址以加速登陆过程, **每次登陆成功需更新本地缓存**
    * `error`: 发生异常
        * `exception`: **(Exception)**
    * `close`: 连接关闭
        * `retry`: **(bool)** 是否执行自动重连(未启用或者被踢掉则不会执行自动重连)

#### 一个例子 ####
```c#
using System;
using System.Collections;
using System.Collections.Generic;
using GameDevWare.Serialization;
using com.rtm;

using UnityEngine;
...

// UnityMainThread
RTMRegistration.Register();

// AnyThread
RTMClient client = new RTMClient(
    "52.83.245.22:13325",
    1000012,
    654321,
    "3993142515BD88A7156629A3AE550B9B",
    RTMConfig.TRANS_LANGUAGE.en,
    new Dictionary<string, string>(),
    true,
    20 * 1000,
    true
);

// 添加监听
client.GetEvent().AddListener("login", (evd) => {
    if (evd.GetException() != null) {
        Debug.Log("Auth Fail!");
        return;
    }

    //RTMGate地址
    Debug.Log("Authed! gate: " + evd.GetPayload());

    // 发送业务消息（发送聊天消息请使用SendChat）
    client.SendMessage(778899, (byte) 8, "hello !", "", 0, 5 * 1000, (cbd) => {
        object obj = cbd.GetPayload();
        if (obj != null) {
            Debug.Log("[DATA] SendMessage: " + Json.SerializeToString(obj) + ", mid: " + cbd.GetMid());
        } else {
            Debug.Log("[ERR] SendMessage: " + cbd.GetException().Message);
        }
    });
});

client.GetEvent().AddListener("close", (evd) => {
    Debug.Log("Closed! retry: " + evd.HasRetry());
});

client.GetEvent().AddListener("error", (evd) => {
    Debug.Log("Error: " + evd.GetException().Message);
});

// push service
RTMProcessor processor = client.GetProcessor();

processor.AddPushService(RTMConfig.SERVER_PUSH.recvMessage, (data) => {
    Debug.Log("[PUSH] recv msg: " + Json.SerializeToString(data));
});

// 开启连接
client.Login(null);

// Destroy 
// client.Destroy();
// client = null;
```

#### 接口说明 ####
* [API-SDK接口](README-API.md)
* [PushService-RTM服务主动推送接口](README-PUSH.md)
* [Microphone-音频录制接口](README-MICPHONE.md)