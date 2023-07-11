# 好友接口文档

### 添加好友

	public bool AddFriends(DoneDelegate callback, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `HashSet<long> uids`

	要添加好友的用户ID列表.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 删除好友

	public bool DeleteFriends(DoneDelegate callback, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `HashSet<long> uids`

	要删除好友的用户ID列表.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取好友列表

	public bool GetFriends(Action<HashSet<long>, int> callback, int timeout = 0);
	
参数:

+ `Action<HashSet<long>, int> callback`

	异步调用回调，`HashSet<long>`为好友用户ID列表，`int`为错误码.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 添加黑名单

	public bool AddBlacklist(DoneDelegate callback, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `HashSet<long> uids`

	要添加黑名单的用户ID列表.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 删除黑名单

	public bool DeleteBlacklist(DoneDelegate callback, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `HashSet<long> uids`

	要删除黑名单的用户ID列表.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取黑名单列表

	public bool GetBlacklist(Action<HashSet<long>, int> callback, int timeout = 0);
	
参数:

+ `Action<HashSet<long>, int> callback`

	异步调用回调，`HashSet<long>`为黑名单用户ID列表，`int`为错误码.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.

