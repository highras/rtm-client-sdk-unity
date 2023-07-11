# 离线推送和数据存储接口文档

### 注册设备的离线推送

	public bool AddDevice(DoneDelegate callback, string appType, string deviceToken, int timeout = 0);

参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `string appType`

	应用类型，IOS设备使用`apns`，安卓设备使用`fcm`

+ `string deviceToken`

    设备token

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 取消注册设备的离线推送

	public bool RemoveDevice(DoneDelegate callback, string deviceToken, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `string deviceToken`

    设备token

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 添加设备的离线推送选项

	public bool AddDevicePushOption(DoneDelegate callback, MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null, int timeout = 0);

参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `MessageCategory messageCategory`

	要屏蔽的消息种类，只支持P2PMessage和GroupMessage.

+ `long targetId`

    要屏蔽的目标ID，messageCategory为P2PMessage时为用户ID；messageCategory为GroupMessage时为群组ID

+ `HashSet<byte> mTypes`

    要屏蔽的消息类型，全屏蔽填0

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 删除设备的离线推送选项

	public bool RemoveDevicePushOption(DoneDelegate callback, MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null, int timeout = 0);

参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `MessageCategory messageCategory`

	要恢复的消息种类，只支持P2PMessage和GroupMessage.

+ `long targetId`

    要恢复的目标ID，messageCategory为P2PMessage时为用户ID；messageCategory为GroupMessage时为群组ID

+ `HashSet<byte> mTypes`

    要恢复的消息类型，全屏蔽填0

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取设备的离线推送选项

	public bool GetDevicePushOption(Action<Dictionary<long, HashSet<byte>>, Dictionary<long, HashSet<byte>>, int> callback, int timeout = 0);

参数:

+ `Action<Dictionary<long, HashSet<byte>>, Dictionary<long, HashSet<byte>>, int> callback`

	异步调用回调，第一个参数`Dictionary<long, HashSet<byte>>`为P2P会话的推送选项，第二个参数`Dictionary<long, HashSet<byte>>`为群组会话的推送选项，第三个参数`int`为错误码

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取存储数据

	public bool DataGet(Action<string, int> callback, string key, int timeout = 0);

参数:

+ `Action<string, int> callback`

	异步调用回调，第一个参数`string`为存储数据的值，第二个参数`int`为错误码

+ `string key`

    存储数据的键值

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 存储数据

	public bool DataSet(DoneDelegate callback, string key, string value, int timeout = 0);

参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `string key`

    存储数据的键值

+ `string value`

    存储数据的内容

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 删除存储数据

	public bool DataDelete(DoneDelegate callback, string key, int timeout = 0);

参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `string key`

    要删除的存储数据的键值

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.

