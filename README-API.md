# fpnn rtm sdk unity #

#### API ####

* `RTMRegistration::Register()`: 在`Unity`主线程中注册RTM服务

> constructor

* `Constructor(string dispatch, int pid, long uid, string token, string lang, IDictionary<string, string> attrs, bool reconnect, int timeout, bool debug)`: 构造RTMClient
    * `dispatch`: **(string)** Dispatch服务地址, RTM提供
    * `pid`: **(int)** 应用编号, RTM提供
    * `uid`: **(long)** 用户ID
    * `token`: **(string)** 用户登录Token, 默认有效期为24小时, 客户端应在每次登录前通过服务端RTM获取
    * `lang`: **(string)** 指定本客户端自动翻译目标语言, 也可以通过`setlang`设置, 参考`RTMConfig.TRANS_LANGUAGE`成员
    * `attrs`: **(IDictionary(string,string))** 设置用户端信息, 保存在当前链接中, 客户端可以获取到
    * `reconnect`: **(bool)** 是否自动重连
    * `timeout`: **(int)** 超时时间(ms), 默认: `30 * 1000`
    * `debug`: **(bool)** 是否开启调试日志

> action

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

> message action

* `SendMessage(long to, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送P2P业务消息
    * `to`: **(long)** 接收方uid
    * `mtype`: **(byte)** 业务消息类型（请使用51-127，禁止使用50及以下的值）
    * `msg`: **(string)** 业务消息内容
    * `attrs`: **(string)** 业务消息附加信息, 没有可传`""`
    * `mid`: **(long)** 业务消息 id, 用于过滤重复业务消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendGroupMessage(long gid, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送group业务消息
    * `gid`: **(long)** group id
    * `mtype`: **(byte)** 业务消息类型（请使用51-127，禁止使用50及以下的值）
    * `msg`: **(string)** 业务消息内容
    * `attrs`: **(string)** 业务消息附加信息, 可传`""`
    * `mid`: **(long)** 业务消息 id, 用于过滤重复业务消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendRoomMessage(long rid, byte mtype, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送room业务消息
    * `rid`: **(long)** room id
    * `mtype`: **(byte)** 业务消息类型（请使用51-127，禁止使用50及以下的值）
    * `msg`: **(string)** 业务消息内容
    * `attrs`: **(string)** 业务消息附加信息, 可传`""`
    * `mid`: **(long)** 业务消息 id, 用于过滤重复业务消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `GetGroupMessage(long gid, bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback)`: 获取Group历史业务消息
    * `gid`: **(long)** Group id
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条业务消息的id, 第一次默认传`0`, 条件：`> or <`
    * `mtypes`: **(List(Byte))** 获取历史业务消息的类型集合
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(GroupMsg)))**
                * `GroupMsg.id` **(long)**
                * `GroupMsg.from` **(long)**
                * `GroupMsg.mtype` **(byte)**
                * `GroupMsg.mid` **(long)**
                * `GroupMsg.msg` **(string)**
                * `GroupMsg.attrs` **(string)**
                * `GroupMsg.mtime` **(long)**

* `GetRoomMessage(long rid, bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback)`: 获取Room历史业务消息
    * `rid`: **(long)** Room id
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条业务消息的id, 第一次默认传`0`, 条件：`> or <`
    * `mtypes`: **(List(Byte))** 获取历史业务消息的类型集合
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(RoomMsg)))**
                * `RoomMsg.id` **(long)**
                * `RoomMsg.from` **(long)**
                * `RoomMsg.mtype` **(byte)**
                * `RoomMsg.mid` **(long)**
                * `RoomMsg.msg` **(string)**
                * `RoomMsg.attrs` **(string)**
                * `RoomMsg.mtime` **(long)**

* `GetBroadcastMessage(bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback)`: 获取广播历史业务消息
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条业务消息的id, 第一次默认传`0`, 条件：`> or <`
    * `mtypes`: **(List(Byte))** 获取历史业务消息的类型集合
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(BroadcastMsg)))**
                * `BroadcastMsg.id` **(long)**
                * `BroadcastMsg.from` **(long)**
                * `BroadcastMsg.mtype` **(byte)**
                * `BroadcastMsg.mid` **(long)**
                * `BroadcastMsg.msg` **(string)**
                * `BroadcastMsg.attrs` **(string)**
                * `BroadcastMsg.mtime` **(long)**

* `GetP2PMessage(long ouid, bool desc, int num, long begin, long end, long lastid, List<Byte> mtypes, int timeout, CallbackDelegate callback)`: 获取P2P历史业务消息
    * `ouid`: **(long)** 获取和两个用户之间的历史业务消息
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条业务消息的id, 第一次默认传`0`, 条件：`> or <`
    * `mtypes`: **(List(Byte))** 获取历史业务消息的类型集合
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(P2PMsg)))**
                * `P2PMsg.id` **(long)**
                * `P2PMsg.direction` **(byte)** `1`发出去的消息, `2`收到的消息
                * `P2PMsg.mtype` **(byte)**
                * `P2PMsg.mid` **(long)**
                * `P2PMsg.msg` **(string)**
                * `P2PMsg.attrs` **(string)**
                * `P2PMsg.mtime` **(long)**

* `DeleteMessage(long mid, long to, int timeout, CallbackDelegate callback)`: 删除P2P业务消息
    * `mid`: **(long)** 业务消息 id
    * `to`: **(long)** 业务消息接收方User id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `DeleteGroupMessage(long mid, long gid, int timeout, CallbackDelegate callback)`: 删除Gourp业务消息
    * `mid`: **(long)** 业务消息 id
    * `gid`: **(long)** 业务消息接收方Group id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `DeleteRoomMessage(long mid, long rid, int timeout, CallbackDelegate callback)`: 删除Room业务消息
    * `mid`: **(long)** 业务消息 id
    * `rid`: **(long)** 业务消息接收方Room id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

> chat action

* `SendChat(long to, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送聊天消息, 消息类型`RTMConfig.CHAT_TYPE.text`
    * `to`: **(long)** 接收方uid
    * `msg`: **(string)** 聊天消息，附加修饰信息不要放这里，方便后继的操作，比如翻译，敏感词过滤等等
    * `attrs`: **(string)** 聊天附加信息, 没有可传`""`
    * `mid`: **(long)** 聊天消息 id, 用于过滤重复聊天消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendAudio(long to, byte[] audio, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送聊天语音, 消息类型`RTMConfig.CHAT_TYPE.audio`
    * `to`: **(long)** 接收方uid
    * `audio`: **(byte[])** 语音数据
    * `attrs`: **(string)** 附加信息, `Json`字符串, 至少带两个参数(`lang`: 语言类型, `duration`: 语音长度 ms)
    * `mid`: **(long)** 语音消息 id, 用于过滤重复聊天语音, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendCmd(long to, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送聊天命令, 消息类型`RTMConfig.CHAT_TYPE.cmd`
    * `to`: **(long)** 接收方uid
    * `msg`: **(string)** 聊天命令
    * `attrs`: **(string)** 命令附加信息, 没有可传`""`
    * `mid`: **(long)** 命令消息 id, 用于过滤重复聊天消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendGroupChat(long gid, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送group聊天消息, 消息类型`RTMConfig.CHAT_TYPE.text`
    * `gid`: **(long)** group id
    * `msg`: **(string)** 聊天消息，附加修饰信息不要放这里，方便后继的操作，比如翻译，敏感词过滤等等
    * `attrs`: **(string)** 聊天附加信息, 可传`""`
    * `mid`: **(long)** 聊天消息 id, 用于过滤重复聊天消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendGroupAudio(long gid, byte[] audio, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送group聊天语音, 消息类型`RTMConfig.CHAT_TYPE.audio`
    * `gid`: **(long)** group id
    * `audio`: **(byte[])** 语音数据
    * `attrs`: **(string)** 附加信息, `Json`字符串, 至少带两个参数(`lang`: 语言类型, `duration`: 语音长度 ms)
    * `mid`: **(long)** 语音消息 id, 用于过滤重复聊天语音, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendGroupCmd(long gid, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送group聊天命令, 消息类型`RTMConfig.CHAT_TYPE.cmd`
    * `gid`: **(long)** group id
    * `msg`: **(string)** 聊天命令
    * `attrs`: **(string)** 命令附加信息, 没有可传`""`
    * `mid`: **(long)** 命令消息 id, 用于过滤重复聊天消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendRoomChat(long rid, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送room聊天消息, 消息类型`RTMConfig.CHAT_TYPE.text`
    * `rid`: **(long)** room id
    * `msg`: **(string)** 聊天消息，附加修饰信息不要放这里，方便后继的操作，比如翻译，敏感词过滤等等
    * `attrs`: **(string)** 聊天附加信息, 可传`""`
    * `mid`: **(long)** 聊天消息 id, 用于过滤重复聊天消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendRoomAudio(long rid, byte[] audio, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送room聊天语音, 消息类型`RTMConfig.CHAT_TYPE.audio`
    * `rid`: **(long)** room id
    * `audio`: **(byte[])** 语音数据
    * `attrs`: **(string)** 附加信息, `Json`字符串, 至少带两个参数(`lang`: 语言类型, `duration`: 语音长度 ms)
    * `mid`: **(long)** 语音消息 id, 用于过滤重复聊天语音, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendRoomCmd(long rid, string msg, string attrs, long mid, int timeout, CallbackDelegate callback)`: 发送room聊天命令, 消息类型`RTMConfig.CHAT_TYPE.cmd`
    * `rid`: **(long)** room id
    * `msg`: **(string)** 聊天命令
    * `attrs`: **(string)** 命令附加信息, 没有可传`""`
    * `mid`: **(long)** 命令消息 id, 用于过滤重复聊天消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `GetGroupChat(long gid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback)`: 获取Group聊天消息历史, 消息类型`RTMConfig.CHAT_TYPE.text, RTMConfig.CHAT_TYPE.audio, RTMConfig.CHAT_TYPE.cmd`
    * `gid`: **(long)** Group id
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条聊天消息的id, 第一次默认传`0`, 条件：`> or <`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(GroupMsg)))**
                * `GroupMsg.id` **(long)**
                * `GroupMsg.from` **(long)**
                * `GroupMsg.mtype` **(byte)**
                * `GroupMsg.mid` **(long)**
                * `GroupMsg.msg` **(string/byte[])**
                * `GroupMsg.attrs` **(string)**
                * `GroupMsg.mtime` **(long)**

* `GetRoomChat(long rid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback)`: 获取Room聊天消息历史, 消息类型`RTMConfig.CHAT_TYPE.text, RTMConfig.CHAT_TYPE.audio, RTMConfig.CHAT_TYPE.cmd`
    * `rid`: **(long)** Room id
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条聊天消息的id, 第一次默认传`0`, 条件：`> or <`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(RoomMsg)))**
                * `RoomMsg.id` **(long)**
                * `RoomMsg.from` **(long)**
                * `RoomMsg.mtype` **(byte)**
                * `RoomMsg.mid` **(long)**
                * `RoomMsg.msg` **(string/byte[])**
                * `RoomMsg.attrs` **(string)**
                * `RoomMsg.mtime` **(long)**

* `GetBroadcastChat(bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback)`: 获取广播聊天消息历史, 消息类型`RTMConfig.CHAT_TYPE.text, RTMConfig.CHAT_TYPE.audio, RTMConfig.CHAT_TYPE.cmd`
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条聊天消息的id, 第一次默认传`0`, 条件：`> or <`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(BroadcastMsg)))**
                * `BroadcastMsg.id` **(long)**
                * `BroadcastMsg.from` **(long)**
                * `BroadcastMsg.mtype` **(byte)**
                * `BroadcastMsg.mid` **(long)**
                * `BroadcastMsg.msg` **(string/byte[])**
                * `BroadcastMsg.attrs` **(string)**
                * `BroadcastMsg.mtime` **(long)**

* `GetP2PChat(long ouid, bool desc, int num, long begin, long end, long lastid, int timeout, CallbackDelegate callback)`: 获取P2P聊天消息历史, 消息类型`RTMConfig.CHAT_TYPE.text, RTMConfig.CHAT_TYPE.audio, RTMConfig.CHAT_TYPE.cmd`
    * `ouid`: **(long)** 获取和两个用户之间的历史聊天消息
    * `desc`: **(bool)** `true`: 则从`end`的时间戳开始倒序翻页, `false`: 则从`begin`的时间戳顺序翻页
    * `num`: **(int)** 获取数量, **一次最多获取20条, 建议10条**
    * `begin`: **(long)** 开始时间戳, 毫秒, 默认`0`, 条件：`>=`
    * `end`: **(long)** 结束时间戳, 毫秒, 默认`0`, 条件：`<=`
    * `lastid`: **(long)** 最后一条聊天消息的id, 第一次默认传`0`, 条件：`> or <`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(num:int,lastid:long,begin:long,end:long,msgs:List(P2PMsg)))**
                * `P2PMsg.id` **(long)**
                * `P2PMsg.direction` **(byte)** `1`发出去的消息, `2`收到的消息
                * `P2PMsg.mtype` **(byte)**
                * `P2PMsg.mid` **(long)**
                * `P2PMsg.msg` **(string/byte[])**
                * `P2PMsg.attrs` **(string)**
                * `P2PMsg.mtime` **(long)**

* `GetUnreadMessage(bool clear, int timeout, CallbackDelegate callback)`: 检测是否有未读聊天消息, (p2p:返回有未读聊天消息的uid集合, group:返回有未读聊天消息的gid集合), 消息类型`RTMConfig.CHAT_TYPE.text, RTMConfig.CHAT_TYPE.audio, RTMConfig.CHAT_TYPE.cmd`
    * `clear`: **(bool)** 是否清除未读消息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(p2p:List(long),group:List(long)))**

* `CleanUnreadMessage(int timeout, CallbackDelegate callback)`: 清除未读聊天消息
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `GetSession(int timeout, CallbackDelegate callback)`: 获取所有会话, 会话仅包括聊天消息产生的会话(p2p:返回存在会话的uid集合, group:返回存在会话的gid集合), 消息类型`RTMConfig.CHAT_TYPE.text, RTMConfig.CHAT_TYPE.audio`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(p2p:List(long),group:List(long)))**

* `DeleteChat(long mid, long to, int timeout, CallbackDelegate callback)`: 删除P2P聊天消息
    * `mid`: **(long)** 聊天消息 id
    * `xid`: **(long)** 聊天消息接收方User id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `DeleteGroupChat(long mid, long gid, int timeout, CallbackDelegate callback)`: 删除Group聊天消息
    * `mid`: **(long)** 聊天消息 id
    * `gid`: **(long)** 聊天消息接收方Group id
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary)**
            * `exception`: **(Exception)**

* `DeleteRoomChat(long mid, long rid, int timeout, CallbackDelegate callback)`: 删除Room聊天消息
    * `mid`: **(long)** 聊天消息 id
    * `rid`: **(long)** 聊天消息接收方Room id
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

* `Translate(string originalMessage, string originalLanguage, string targetLanguage, string type, string profanity, int timeout, CallbackDelegate callback)`: 翻译聊天消息, 需启用翻译服务, 返回{`source`:原始聊天消息语言类型,`target`:翻译后的语言类型,`sourceText`:原始聊天消息,`targetText`:翻译后的消息}
    * `originalMessage`: **(string)** 待翻译的原始聊天消息
    * `originalLanguage`: **(string)** 待翻译的聊天消息的语言类型, 可为`null`, 参考`RTMConfig.TRANS_LANGUAGE`成员
    * `targetLanguage`: **(string)** 本次翻译的目标语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
    * `type`: **(string)** 可选值为`
    `或`mail`, 默认:`chat`
    * `profanity`: **(string)** 敏感语过滤, 设置为以下三项之一: `off` `stop` `censor`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(source:string,target:string,sourceText:string,targetText:string))**

* `Profanity(string text, string action, int timeout, CallbackDelegate callback)`: 敏感词过滤, 返回过滤后的字符串或者以错误形式返回, 需启用翻译服务
    * `text`: **(string)** 待检查文本
    * `action`: **(string)** 检查结果返回形式, `stop`: 以错误形式返回, `censor`: 用`*`替换敏感词
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(text:string))**

* `Transcribe(byte[] audio, string lang, string action, int timeout, CallbackDelegate callback)`: 语音识别, 返回过滤后的字符串或者以错误形式返回, 需启用翻译服务, 设置超时时间不低于60s
    * `audio`: **(byte[])** 待识别语音数据
    * `lang`: **(string)** 待识别语音的类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
    * `action`: **(string)** 检查结果返回形式, `stop`: 以错误形式返回, `censor`: 用`*`替换敏感词
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `exception`: **(Exception)**
            * `payload`: **(IDictionary(text:string,lang:string))**

> file token

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

> user action

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

> friends action

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

> group action

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

> room action

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

> data save

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

> file send

* `SendFile(byte mtype, long to, byte[] fileBytes, string fileExt, string fileName, long mid, int timeout, CallbackDelegate callback)`: 发送文件
    * `mtype`: **(byte)** 业务文件消息类型
    * `to`: **(long)** 接收者 id
    * `fileBytes`: **(byte[])** 要发送的文件
    * `fileExt`: **(string)** 文件扩展名
    * `fileName`: **(string)** 文件名
    * `mid`: **(long)** 业务文件消息 id, 用于过滤重复业务文件消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendGroupFile(byte mtype, long gid, byte[] fileBytes, string fileExt, string fileName, long mid, int timeout, CallbackDelegate callback)`: 发送文件
    * `mtype`: **(byte)** 业务文件消息类型
    * `gid`: **(long)** Group id
    * `fileBytes`: **(byte[])** 要发送的文件
    * `fileExt`: **(string)** 文件扩展名
    * `fileName`: **(string)** 文件名
    * `mid`: **(long)** 业务文件消息 id, 用于过滤重复业务文件消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**

* `SendRoomFile(byte mtype, long rid, byte[] fileBytes, string fileExt, string fileName, long mid, int timeout, CallbackDelegate callback)`: 发送文件
    * `mtype`: **(byte)** 业务文件消息类型
    * `rid`: **(long)** Room id
    * `fileBytes`: **(byte[])** 要发送的文件
    * `fileExt`: **(string)** 文件扩展名
    * `fileName`: **(string)** 文件名
    * `mid`: **(long)** 业务文件消息 id, 用于过滤重复业务文件消息, 非重发时为`0`
    * `timeout`: **(int)** 超时时间(ms)
    * `callback`: **(CallbackDelegate)** 回调方法
        * `cbd`: **(CallbackData)**
            * `payload`: **(IDictionary(mtime:long))**
            * `exception`: **(Exception)**
            * `mid`: **(long)**