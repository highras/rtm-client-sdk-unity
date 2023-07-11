# 会话接口文档

### 获取会话列表

	public bool GetConversation(Action<List<Conversation>, int> callback, ConversationType conversationType, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);
	
参数:

+ `Action<List<Conversation>, int> callback`

	异步调用回调，`Conversation`请参照[Conversation](RTMStructures.md#Conversation).

+ `ConversationType conversationType`

	会话种类，请参照[ConversationType](RTMStructures.md#ConversationType).

+ `HashSet<byte> mTypes`

    消息类型列表,不传则表示所有消息类型

+ `long startTime`

	计算未读消息条数时间，大于该时间的消息视为未读消息，传0则使用上次离线时间计算

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取所有未读会话列表

	public bool GetAllUnreadConversation(Action<List<Conversation>, List<Conversation>, int> callback, bool clear = true, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);
	
参数:

+ `Action<List<Conversation>, List<Conversation>, int> callback`

	异步调用回调，第一个`List<Conversation>`为群组未读会话列表，第二个`List<Conversation>`为P2P未读会话列表
    `Conversation`请参照[Conversation](RTMStructures.md#Conversation).

+ `bool clear`

    是否清除未读会话状态

+ `HashSet<byte> mTypes`

    消息类型列表,不传则表示所有消息类型

+ `long startTime`

	计算未读消息条数时间，大于该时间的消息视为未读消息，传0则使用上次离线时间计算

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.

### 获取未读会话列表

	public bool GetUnreadConversation(Action<List<Conversation>, int> callback, ConversationType conversationType, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);
	
参数:

+ `Action<List<Conversation>, int> callback`

	异步调用回调,`Conversation`请参照[Conversation](RTMStructures.md#Conversation).

+ `ConversationType conversationType`

	会话种类，请参照[ConversationType](RTMStructures.md#ConversationType).

+ `HashSet<byte> mTypes`

    消息类型列表,不传则表示所有消息类型

+ `long startTime`

	计算未读消息条数时间，大于该时间的消息视为未读消息，传0则使用上次离线时间计算

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.

### 清除会话未读状态

	public bool ClearUnread(DoneDelegate callback, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.

### 删除P2P会话

	public bool RemoveP2PConversation(DoneDelegate callback, long uid, bool oneway = false, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long uid`

    要删除的会话的用户ID

+ `bool oneway`

    `ture`表示只从自己的会话列表中删除该会话，`false`表示从双方的会话列表中都删除该会话

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.
