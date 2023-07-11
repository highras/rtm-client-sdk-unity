# 用户相关接口文档

### 获取用户信息

	public bool GetUserInfos(IMLIB_GetUserInfosDelegate callback, HashSet<long> uids, int timeout = 0)

参数:

+ `IMLIB_GetUserInfosDelegate callback`

	异步调用回调，请参照[IMLIB_GetUserInfosDelegate](IMDelegates.md#IMLIB_GetUserInfosDelegate).

+ `HashSet<long> uids`

    用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 注册设备的离线推送

	public bool AddDevice(DoneDelegate callback, IMLIB_PushAppType appType, string deviceToken, int timeout = 0);

参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `IMLIB_PushAppType appType`

	应用类型，请参照[IMLIB_PushAppType](IMStruct.md#IMLIB_PushAppType).

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

