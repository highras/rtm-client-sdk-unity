# 用户相关接口文档

### 获取用户信息

	public bool GetUserInfo(Action<string, string, int> callback, int timeout = 0);

参数:

+ `Action<string, string, int> callback`

	异步调用回调，第一个参数`string`为公开信息，第二个参数`string`为私有信息，第三个参数`int`为错误码.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取用户公开信息

	public bool GetUsersPublicInfo(Action<Dictionary<long, string>, int> callback, HashSet<long> uids, int timeout = 0);
	
参数:

+ `Action<Dictionary<long, string>, int> callback`

	异步调用回调，第一个参数`Dictionary<long, string>`为公开信息，key为用户ID，value为公开信息内容；第二个参数`int`为错误码.

+ `HashSet<long> uids`

	用户ID列表.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 设置用户信息

	public bool SetUserInfo(DoneDelegate callback, string publicInfo = null, string privateInfo = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `string publicInfo`

    公开信息

+ `string privateInfo`

    私有信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取用户在线状态

	public bool GetUsersOnlineStatus(Action<HashSet<long>, int> callback, HashSet<long> uids, int timeout = 0);
	
参数:

+ `Action<HashSet<long>, int> callback`

	异步调用回调，第一个参数`HashSet<long>`为在线的用户ID列表，第二个参数`int`为错误码.

+ `HashSet<long> uids`

	用户ID列表.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.

