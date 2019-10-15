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
    null,
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

    // 发送消息
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

#### Events ####
* `event`:
    * `login`: 登陆
        * `exception`: **(Exception)** auth失败, token失效需重新获取
        * `payload`: **(IDictionary)** 当前连接的RTMGate地址, 可在本地缓存, 下次登陆可使用该地址以加速登陆过程, **每次登陆成功需更新本地缓存**
    * `error`: 发生异常
        * `exception`: **(Exception)**
    * `close`: 连接关闭
        * `retry`: **(bool)** 是否执行自动重连(未启用或者被踢掉则不会执行自动重连)

#### PushService ####
* `RTMProcessor::AddPushService(string name, Action<IDictionary<string, object>> action)`: 添加推送回调
    * `name`: **(string)** 推送服务类型, 参考`RTMConfig.SERVER_PUSH`成员
    * `action`: **(Action(IDictionary(string, object)))** 回调方法

* `RTMProcessor::RemovePushService(string name)`: 删除推送回调
    * `name`: **(string)** 推送服务类型, 参考`RTMConfig.SERVER_PUSH`成员

* `RTMProcessor::HasPushService(string name)`: 是否存在推送回调
    * `name`: **(string)** 推送服务类型, 参考`RTMConfig.SERVER_PUSH`成员

* `RTMConfig.SERVER_PUSH`:
    * `kickoutroom`: RTMGate主动从Room移除
        * `data`: **(IDictionary(string, object))**
            * `data.rid`: **(long)** Room id

    * `ping`: RTMGate主动ping
        * `data`: **(IDictionary(string, object))**

    * `pushmsg`: RTMGate主动推送P2P消息
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.mtype`: **(byte)** 消息类型
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.msg`: **(string)** 消息内容
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**

    * `pushgroupmsg`: RTMGate主动推送Group消息
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.gid`: **(long)** Group id
            * `data.mtype`: **(byte)** 消息类型
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.msg`: **(string)** 消息内容
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**

    * `pushroommsg`: RTMGate主动推送Room消息
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.rid`: **(long)** Room id
            * `data.mtype`: **(byte)** 消息类型
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.msg`: **(string)** 消息内容
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**

    * `pushbroadcastmsg`: RTMGate主动推送广播消息
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.mtype`: **(byte)** 消息类型
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.msg`: **(string)** 消息内容
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**

    * `pushfile`: RTMGate主动推送P2P文件
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.mtype`: **(byte)** 文件类型, 请参考 `RTMConfig.FILE_TYPE` 成员
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.msg`: **(string)** 文件获取地址(url)
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**

    * `pushgroupfile`: RTMGate主动推送Group文件
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.gid`: **(long)** Group id
            * `data.mtype`: **(byte)** 文件类型, 请参考 `RTMConfig.FILE_TYPE` 成员
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.msg`: **(string)** 文件获取地址(url)
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**

    * `pushroomfile`: RTMGate主动推送Room文件
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.rid`: **(long)** Room id
            * `data.mtype`: **(byte)** 文件类型, 请参考 `RTMConfig.FILE_TYPE` 成员
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.msg`: **(string)** 文件获取地址(url)
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**

    * `pushbroadcastfile`: RTMGate主动推送广播文件
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.mtype`: **(byte)** 文件类型, 请参考 `RTMConfig.FILE_TYPE` 成员
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.msg`: **(string)** 文件获取地址(url)
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**

    * `pushchat`: RTMGate主动推送P2P消息
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**
            * `data.msg`: **(JsonString)** 消息内容
                * `source`: **(string)** 原始消息语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
                * `target`: **(string)** 翻译后的语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
                * `sourceText`: **(string)** 原始消息
                * `targetText`: **(string)** 翻译后的消息

    * `pushgroupchat`: RTMGate主动推送Group消息
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.gid`: **(long)** Group id
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**
            * `data.msg`: **(JsonString)** 消息内容
                * `source`: **(string)** 原始消息语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
                * `target`: **(string)** 翻译后的语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
                * `sourceText`: **(string)** 原始消息
                * `targetText`: **(string)** 翻译后的消息

    * `pushroomchat`: RTMGate主动推送Room消息
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.rid`: **(long)** Room id
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**
            * `data.msg`: **(JsonString)** 消息内容
                * `source`: **(string)** 原始消息语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
                * `target`: **(string)** 翻译后的语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
                * `sourceText`: **(string)** 原始消息
                * `targetText`: **(string)** 翻译后的消息

    * `pushbroadcastchat`: RTMGate主动推送广播消息
        * `data`: **(IDictionary(string, object))**
            * `data.from`: **(long)** 发送者 id
            * `data.mid`: **(long)** 消息 id, 当前链接会话内唯一
            * `data.attrs`: **(string)** 发送时附加的自定义内容
            * `data.mtime`: **(long)**
            * `data.msg`: **(JsonString)** 消息内容
                * `source`: **(string)** 原始消息语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
                * `target`: **(string)** 翻译后的语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
                * `sourceText`: **(string)** 原始消息
                * `targetText`: **(string)** 翻译后的消息

#### API ####
* `RTMRegistration::Register()`: 在`Unity`主线程中注册RTM服务

* `Constructor(string dispatch, int pid, long uid, string token, string version, string lang, IDictionary<string, string> attrs, bool reconnect, int timeout, bool debug)`: 构造RTMClient
    * `dispatch`: **(string)** Dispatch服务地址, RTM提供
    * `pid`: **(int)** 应用编号, RTM提供
    * `uid`: **(long)** 用户ID
    * `token`: **(string)** 用户登录Token, 通过服务端RTM获取
    * `version`: **(string)** RTM服务版本号, RTM提供
    * `lang`: **(string)** 指定本客户端自动翻译目标语言, 也可以通过`setlang`设置, 参考`RTMConfig.TRANS_LANGUAGE`成员
    * `attrs`: **(IDictionary(string,string))** 设置用户端信息, 保存在当前链接中, 客户端可以获取到
    * `reconnect`: **(bool)** 是否自动重连
    * `timeout`: **(int)** 超时时间(ms), 默认: `30 * 1000`
    * `debug`: **(bool)** 是否开启调试日志

* `GetProcessor`: **(RTMProcessor)** 监听PushService的句柄

* `Destroy()`: 断开连接并销毁

* `Login(string endpoint)`: 连接并登陆
    * `endpoint`: **(string)** RTMGate服务地址, 由Dispatch服务获取, 或由RTM提供

* `Close()`: 断开连接

* `Kickout(string ce, int timeout, CallbackDelegate callback)`: 踢掉一个链接 (只对多用户登录有效，不能踢掉自己，可以用来实现同类设备唯一登录)
    * `ce`: **(string)** 当前链接的`endpoint`, 可以通过调用`getAttrs`获取
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `AddAttrs(IDictionary<string, string> attrs, int timeout, CallbackDelegate callback)`: 设置客户端信息, 保存在当前链接中, 客户端可以获取到
    * `attrs`: **(IDictionary(string,string))** key-value形式的变量
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `GetAttrs(int timeout, CallbackDelegate callback)`: 获取客户端信息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(List(IDictionary(string,string))**
                * `IDictionary.ce` **(string)** 链接的`endpoint`,需要让其下线可以调用`Kickout`
                * `IDictionary.login` **(string)** 登录时间,UTC时间戳
                * `IDictionary.my` **(string)** 当前链接的`attrs`

* `AddDebugLog(string msg, string attrs, int timeout, CallbackDelegate callback)`: 添加debug日志
    * `msg`: **(string)** 调试信息msg
    * `attrs`: **(string)** 调试信息attrs
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `AddDevice(string apptype, string devicetoken, int timeout, CallbackDelegate callback)`: 添加设备, 应用信息
    * `apptype`: **(string)** 应用信息
    * `devicetoken`: **(string)** 设备信息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `RemoveDevice(string devicetoken, int timeout, CallbackDelegate callback)`: 删除设备, 应用信息
    * `devicetoken`: **(string)** 设备信息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `SendMessage(long to, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送消息
    * `to`: **(long)** 接收方uid
    * `mtype`: **(byte)** 消息类型
    * `msg`: **(string)** 消息内容
    * `attrs`: **(string)** 消息附加信息, 没有可传`""`
    * `mid`: **(long)** 消息 id, 用于过滤重复消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendGroupMessage(long gid, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送group消息
    * `gid`: **(long)** group id
    * `mtype`: **(byte)** 消息类型
    * `msg`: **(string)** 消息内容
    * `attrs`: **(string)** 消息附加信息, 可传`""`
    * `mid`: **(long)** 消息 id, 用于过滤重复消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendRoomMessage(long rid, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送room消息
    * `rid`: **(long)** room id
    * `mtype`: **(byte)** 消息类型
    * `msg`: **(string)** 消息内容
    * `attrs`: **(string)** 消息附加信息, 可传`""`
    * `mid`: **(long)** 消息 id, 用于过滤重复消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `GetGroupMessage(long gid, bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback)`: 获取Group历史消息
    * `gid`: **(long)** Group id
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条消息的id, 第一次默认传`0`, 条件：`> or <`
    * `mtypes`: **(List(Byte))** 获取历史消息的消息类型集合
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(GroupMsg)))**
                * `GroupMsg.id` **(long)**
                * `GroupMsg.from` **(long)**
                * `GroupMsg.mtype` **(byte)**
                * `GroupMsg.mid` **(long)**
                * `GroupMsg.deleted` **(bool)**
                * `GroupMsg.msg` **(string)**
                * `GroupMsg.attrs` **(string)**
                * `GroupMsg.mtime` **(long)**

* `GetRoomMessage(long rid, bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback)`: 获取Room历史消息
    * `rid`: **(long)** Room id
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条消息的id, 第一次默认传`0`, 条件：`> or <`
    * `mtypes`: **(List(Byte))** 获取历史消息的消息类型集合
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(RoomMsg)))**
                * `RoomMsg.id` **(long)**
                * `RoomMsg.from` **(long)**
                * `RoomMsg.mtype` **(byte)**
                * `RoomMsg.mid` **(long)**
                * `RoomMsg.deleted` **(bool)**
                * `RoomMsg.msg` **(string)**
                * `RoomMsg.attrs` **(string)**
                * `RoomMsg.mtime` **(long)**

* `GetBroadcastMessage(bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback)`: 获取广播历史消息
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条消息的id, 第一次默认传`0`, 条件：`> or <`
    * `mtypes`: **(List(Byte))** 获取历史消息的消息类型集合
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(BroadcastMsg)))**
                * `BroadcastMsg.id` **(long)**
                * `BroadcastMsg.from` **(long)**
                * `BroadcastMsg.mtype` **(byte)**
                * `BroadcastMsg.mid` **(long)**
                * `BroadcastMsg.deleted` **(bool)**
                * `BroadcastMsg.msg` **(string)**
                * `BroadcastMsg.attrs` **(string)**
                * `BroadcastMsg.mtime` **(long)**

* `GetP2PMessage(long ouid, bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback)`: 获取P2P历史消息
    * `ouid`: **(long)** 获取和两个用户之间的历史消息
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条消息的id, 第一次默认传`0`, 条件：`> or <`
    * `mtypes`: **(List(Byte))** 获取历史消息的消息类型集合
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(P2PMsg)))**
                * `P2PMsg.id` **(long)**
                * `P2PMsg.direction` **(byte)**
                * `P2PMsg.mtype` **(byte)**
                * `P2PMsg.mid` **(long)**
                * `P2PMsg.deleted` **(bool)**
                * `P2PMsg.msg` **(string)**
                * `P2PMsg.attrs` **(string)**
                * `P2PMsg.mtime` **(long)**

* `DeleteMessage(long mid, long xid, byte type, int timeout, CallbackDelegate callback)`: 删除消息
    * `mid`: **(long)** 消息 id
    * `xid`: **(long)** 消息接收方 id (userId/RoomId/GroupId)
    * `type`: **(byte)** 接收方类型 (1:p2p, 2:group, 3:room)
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `SendChat(long to, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送消息, `mtype=(byte) 30`
    * `to`: **(long)** 接收方uid
    * `msg`: **(string)** 消息内容，附加修饰信息不要放这里，方便后继的操作，比如翻译，敏感词过滤等等
    * `attrs`: **(string)** 消息附加信息, 没有可传`""`
    * `mid`: **(long)** 消息 id, 用于过滤重复消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendGroupChat(long gid, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送group消息, `mtype=(byte) 30`
    * `gid`: **(long)** group id
    * `msg`: **(string)** 消息内容，附加修饰信息不要放这里，方便后继的操作，比如翻译，敏感词过滤等等
    * `attrs`: **(string)** 消息附加信息, 可传`""`
    * `mid`: **(long)** 消息 id, 用于过滤重复消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendRoomChat(long rid, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送room消息, `mtype=(byte) 30`
    * `rid`: **(long)** room id
    * `msg`: **(string)** 消息内容，附加修饰信息不要放这里，方便后继的操作，比如翻译，敏感词过滤等等
    * `attrs`: **(string)** 消息附加信息, 可传`""`
    * `mid`: **(long)** 消息 id, 用于过滤重复消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `GetGroupChat(long gid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback)`: 获取Group历史消息, `mtypes=new List<Byte> { (byte) 30 }`
    * `gid`: **(long)** Group id
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条消息的id, 第一次默认传`0`, 条件：`> or <`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(GroupMsg)))**
                * `GroupMsg.id` **(long)**
                * `GroupMsg.from` **(long)**
                * `GroupMsg.mid` **(long)**
                * `GroupMsg.deleted` **(bool)**
                * `GroupMsg.msg` **(string)**
                * `GroupMsg.attrs` **(string)**
                * `GroupMsg.mtime` **(long)**

* `GetRoomChat(long rid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback)`: 获取Room历史消息, `mtypes=new List<Byte> { (byte) 30 }`
    * `rid`: **(long)** Room id
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条消息的id, 第一次默认传`0`, 条件：`> or <`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(RoomMsg)))**
                * `RoomMsg.id` **(long)**
                * `RoomMsg.from` **(long)**
                * `RoomMsg.mid` **(long)**
                * `RoomMsg.deleted` **(bool)**
                * `RoomMsg.msg` **(string)**
                * `RoomMsg.attrs` **(string)**
                * `RoomMsg.mtime` **(long)**

* `GetBroadcastChat(bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback)`: 获取广播历史消息, `mtypes=new List<Byte> { (byte) 30 }`
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条消息的id, 第一次默认传`0`, 条件：`> or <`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(BroadcastMsg)))**
                * `BroadcastMsg.id` **(long)**
                * `BroadcastMsg.from` **(long)**
                * `BroadcastMsg.mid` **(long)**
                * `BroadcastMsg.deleted` **(bool)**
                * `BroadcastMsg.msg` **(string)**
                * `BroadcastMsg.attrs` **(string)**
                * `BroadcastMsg.mtime` **(long)**

* `GetP2PChat(long ouid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback)`: 获取P2P历史消息, `mtypes=new List<Byte> { (byte) 30 }`
    * `ouid`: **(long)** 获取和两个用户之间的历史消息
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条消息的id, 第一次默认传`0`, 条件：`> or <`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(P2PMsg)))**
                * `P2PMsg.id` **(long)**
                * `P2PMsg.direction` **(byte)**
                * `P2PMsg.mid` **(long)**
                * `P2PMsg.deleted` **(bool)**
                * `P2PMsg.msg` **(string)**
                * `P2PMsg.attrs` **(string)**
                * `P2PMsg.mtime` **(long)**

* `GetUnreadMessage(int timeout, CallbackDelegate callback)`: 检测是否有未读消息, (p2p:返回有未读消息的uid集合, group:返回有未读消息的gid集合)
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(p2p:List(long),group:List(long)))**

* `CleanUnreadMessage(int timeout, CallbackDelegate callback)`: 清除未读消息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `GetSession(int timeout, CallbackDelegate callback)`: 获取所有会话, (p2p:返回存在会话的uid集合, group:返回存在会话的gid集合)
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(p2p:List(long),group:List(long)))**

* `DeleteChat(long mid, long xid, byte type, int timeout, CallbackDelegate callback)`: 删除消息
    * `mid`: **(long)** 消息 id
    * `xid`: **(long)** 消息接收方 id (userId/RoomId/GroupId)
    * `type`: **(byte)** 接收方类型 (1:p2p, 2:group, 3:room)
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `SetTranslationLanguage(string targetLanguage, int timeout, CallbackDelegate callback)`: 设置自动翻译的默认目标语言类型, 如果`targetLanguage`为空字符串, 则取消自动翻译
    * `targetLanguage`: **(string)** 翻译的目标语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `Translate(string originalMessage, string originalLanguage, string targetLanguage, string type, string profanity, int timeout, CallbackDelegate callback)`: 翻译消息, 返回{`source`:原始消息语言类型,`target`:翻译后的语言类型,`sourceText`:原始消息,`targetText`:翻译后的消息}
    * `originalMessage`: **(string)** 待翻译的原始消息
    * `originalLanguage`: **(string)** 待翻译的消息的语言类型, 可为`null`, 参考`RTMConfig.TRANS_LANGUAGE`成员
    * `targetLanguage`: **(string)** 本次翻译的目标语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
    * `type`: **(string)** 可选值为`chat`或`mail`, 默认:`chat`
    * `profanity`: **(string)** 敏感语过滤, 设置为以下三项之一: `off` `stop` `censor`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(source:string,target:string,sourceText:string,targetText:string))**

* `Profanity(string text, string action, int timeout, CallbackDelegate callback)`: 敏感词过滤, 返回过滤后的字符串或者以错误形式返回
    * `text`: **(string)** 待检查文本
    * `action`: **(string)** 检查结果返回形式, `stop`: 以错误形式返回, `censor`: 用`*`替换敏感词
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(text:string))**

* `FileToken(string cmd, long to, long rid, long gid, int timeout, CallbackDelegate callback)`: 获取发送文件的`token`以及需要连接的`endpoint`
    * `cmd`: **(string)** 文件发送方式`sendfile | sendroomfile | sendgroupfile`
    * `to`: **(long)** 接收方 uid
    * `rid`: **(long)** Room id
    * `gid`: **(long)** Group id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(token:string, endpoint:string))**

* `GetOnlineUsers(List<long> uids, int timeout, CallbackDelegate callback)`: 获取在线用户, 限制每次最多获取200个
    * `uids`: **(List(long))** 多个用户 id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(List(long))**
            * `exception`: **(Exception)**

* `SetUserInfo(string oinfo, string pinfo, int timeout, CallbackDelegate callback)`: 设置用户自己的公开信息和私有信息, 建议从服务端调用此接口
    * `oinfo`: **(string)** 公开信息
    * `pinfo`: **(string)** 私有信息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `GetUserInfo(int timeout, CallbackDelegate callback)`: 获取用户自己的公开信息和私有信息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(oinfo:string,pinfo:string))**
            * `exception`: **(Exception)**

* `GetUserOpenInfo(List<long> uids, int timeout, CallbackDelegate callback)`: 获取其他用户的公开信息, 每次最多获取100人
    * `uids`: **(List(long))** 多个用户 id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(string,string))**
            * `exception`: **(Exception)**

* `AddFriends(List<long> friends, int timeout, CallbackDelegate callback)`: 添加好友, 每次最多添加100人
    * `friends`: **(List(long))** 多个好友 id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `DeleteFriends(List<long> friends, int timeout, CallbackDelegate callback)`: 删除好友, 每次最多删除100人
    * `friends`: **(List(long))** 多个好友 id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `GetFriends(int timeout, CallbackDelegate callback)`: 获取好友
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(List(long))**
            * `exception`: **(Exception)**

* `AddGroupMembers(long gid, List<long> uids, int timeout, CallbackDelegate callback)`: 添加group成员, 每次最多添加100人
    * `gid`: **(long)** group id
    * `uids`: **(List(long))** 多个用户 id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(List(long))**
            * `exception`: **(Exception)**

* `DeleteGroupMembers(long gid, List<long> uids, int timeout, CallbackDelegate callback)`: 删除group成员, 每次最多删除100人
    * `gid`: **(long)** group id
    * `uids`: **(List(long))** 多个用户 id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(List(long))**
            * `exception`: **(Exception)**

* `GetGroupMembers(long gid, int timeout, CallbackDelegate callback)`: 获取group成员
    * `gid`: **(long)** group id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(List(long))**
            * `exception`: **(Exception)**

* `GetUserGroups(int timeout, CallbackDelegate callback)`: 获取用户所在的Group
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(List(long))**
            * `exception`: **(Exception)**

* `SetGroupInfo(long gid, string oinfo, string pinfo, int timeout, CallbackDelegate callback)`: 设置Group的公开信息和私有信息, 会检查用户是否在Group内, 建议从服务端调用此接口
    * `gid`: **(long)** group id
    * `oinfo`: **(string)** 公开信息
    * `pinfo`: **(string)** 私有信息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `GetGroupInfo(long gid, int timeout, CallbackDelegate callback)`: 获取Group的公开信息和私有信息, 会检查用户是否在组内
    * `gid`: **(long)** group id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(oinfo:string,pinfo:string))**
            * `exception`: **(Exception)**

* `GetGroupOpenInfo(long gid, int timeout, CallbackDelegate callback)`: 获取Group的公开信息
    * `gid`: **(long)** group id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(oinfo:string))**
            * `exception`: **(Exception)**

* `EnterRoom(long rid, int timeout, CallbackDelegate callback)`: 进入Room
    * `rid`: **(long)** room id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `LeaveRoom(long rid, int timeout, CallbackDelegate callback)`: 离开Room
    * `rid`: **(long)** room
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `GetUserRooms(int timeout, CallbackDelegate callback)`: 获取用户所在的Room
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(List(long))**
            * `exception`: **(Exception)**

* `SetRoomInfo(long rid, string oinfo, string pinfo, int timeout, CallbackDelegate callback)`: 设置Room的公开信息和私有信息, 会检查用户是否在Room内, 建议从服务端调用此接口
    * `rid`: **(long)** room id
    * `oinfo`: **(string)** 公开信息
    * `pinfo`: **(string)** 私有信息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `GetRoomInfo(long rid, int timeout, CallbackDelegate callback)`: 获取Room的公开信息和私有信息, 会检查用户是否在Room内
    * `rid`: **(long)** room id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(oinfo:string,pinfo:string))**
            * `exception`: **(Exception)**

* `GetRoomOpenInfo(long rid, int timeout, CallbackDelegate callback)`: 获取Room的公开信息
    * `rid`: **(long)** room id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(oinfo:string))**
            * `exception`: **(Exception)**

* `DataGet(string key, int timeout, CallbackDelegate callback)`: 获取存储的数据信息
    * `key`: **(string)** 存储数据对应键值, 最长`128字节`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(val:string))**

* `DataSet(string key, string value, int timeout, CallbackDelegate callback)`: 设置存储的数据信息
    * `key`: **(string)** 存储数据对应键值, 最长`128 字节`
    * `value`: **(string)** 存储数据实际内容, 最长`1024 * 1024 * 2字节`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `DataDelete(string key, int timeout, CallbackDelegate callback)`: 删除存储的数据信息
    * `key`: **(string)** 存储数据对应键值, 最长`128字节`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `SendFile(byte mtype, long to, byte[] fileBytes, long mid, int timeout, CallbackDelegate callback)`: 发送文件
    * `mtype`: **(byte)** 消息类型
    * `to`: **(long)** 接收者 id
    * `fileBytes`: **(byte[])** 要发送的文件
    * `mid`: **(long)** 消息 id, 用于过滤重复消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendGroupFile(byte mtype, long gid, byte[] fileBytes, long mid, int timeout, CallbackDelegate callback)`: 发送文件
    * `mtype`: **(byte)** 消息类型
    * `gid`: **(long)** Group id
    * `fileBytes`: **(byte[])** 要发送的文件
    * `mid`: **(long)** 消息 id, 用于过滤重复消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendRoomFile(byte mtype, long rid, byte[] fileBytes, long mid, int timeout, CallbackDelegate callback)`: 发送文件
    * `mtype`: **(byte)** 消息类型
    * `rid`: **(long)** Room id
    * `fileBytes`: **(byte[])** 要发送的文件
    * `mid`: **(long)** 消息 id, 用于过滤重复消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**
