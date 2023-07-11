# 聊天接口文档

### 发送聊天消息

	public bool SendChatMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, string message, string attrs = "", int timeout = 0);
	
参数:

+ `MessageIdDelegate callback`

	异步调用回调，请参照[MessageIdDelegate](../Delegates.md#MessageIdDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

+ `string message`

	消息内容。

+ `string attrs`

	消息自定义属性信息。

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.

### 发送命令消息

	public bool SendCMDMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, string message, string attrs = "", int timeout = 0);

参数:

+ `MessageIdDelegate callback`

	异步调用回调，请参照[MessageIdDelegate](../Delegates.md#MessageIdDelegate).


+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

+ `string message`

	消息内容。

+ `string attrs`

	消息自定义属性信息。

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取单条消息

	public bool GetMessage(Action<RetrievedMessage, int> callback, MessageCategory messageCategory, long fromUid, long toId, long messageId, int timeout = 0)
	
参数:

+ `Action<RetrievedMessage, int> callback`

	异步调用回调，请参照[RetrievedMessage](RTMStructures.md#RetrievedMessage).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long fromUid`

    消息发送者用户ID。

+ `long toUid`

    消息接受ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

+ `long messageId`

	消息ID

+ `int timeout`

	超时时间，0代表使用默认配置.


返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 删除单条消息

	public bool DeleteMessage(DoneDelegate callback, MessageCategory messageCategory, long fromUid, long toId, long messageId, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long fromUid`

    消息发送者用户ID。

+ `long toUid`

    消息接受ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

+ `long messageId`

	消息ID

+ `int timeout`

	超时时间，0代表使用默认配置.


返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取历史聊天消息

	public bool GetHitoryChatMessage(HistoryMessageDelegate callback, MessageCategory messageCategory, long id, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);
	
参数:

+ `HistoryMessageDelegate callback`

	异步调用回调，请参照[HistoryMessageDelegate](../Delegates.md#HistoryMessageDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

+ `bool desc`

	`true`表示倒序，`false`表示正序

+ `int count`

	查询消息条数

+ `long beginMsec`

	开始时间戳（毫秒）,可选默认0,第二次查询传入上次结果的HistoryMessageResult.beginMsec

+ `long endMsec`

	结束时间戳（毫秒）,可选默认0,第二次查询传入上次结果的HistoryMessageResult.endMsec

+ `long lastId`

	索引ID,可选默认0，第一次获取传入0 第二次查询传入上次结果HistoryMessageResult的lastId

+ `int timeout`

	超时时间，0代表使用默认配置.


返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 通过MessageId拉取历史聊天消息

	public bool GetHitoryChatMessageByMessageId(HistoryMessageDelegate callback, MessageCategory messageCategory, long id, bool desc, int count, long messageId, long beginMsec = 0, long endMsec = 0, List<byte> mtypes = null, int timeout = 0);
	
参数:

+ `HistoryMessageDelegate callback`

	异步调用回调，请参照[HistoryMessageDelegate](../Delegates.md#HistoryMessageDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

+ `bool desc`

	`true`表示倒序，`false`表示正序

+ `int count`

	查询消息条数

+ `long messageId`

	消息ID

+ `long beginMsec`

	开始时间戳（毫秒）,可选默认0,第二次查询传入上次结果的HistoryMessageResult.beginMsec

+ `long endMsec`

	结束时间戳（毫秒）,可选默认0,第二次查询传入上次结果的HistoryMessageResult.endMsec

+ `List<byte> mtypes`

	消息类型列表

+ `int timeout`

	超时时间，0代表使用默认配置.


返回值:

+ true: 请求发送成功
+ false: 请求发送失败.
