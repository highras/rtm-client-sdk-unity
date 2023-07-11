# 群组接口文档

### 获取群组成员数量

	public bool GetGroupMemberCount(Action<int, int> callback, long groupId, int timeout = 0);
	
参数:

+ `Action<int, int> callback`

	异步调用回调，第一个`int`参数为群组成员数，第二个`int`为错误码.

+ `long groupId`

	群组ID.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取群组成员列表

	public bool GetGroupMembers(Action<HashSet<long>, HashSet<long>, int> callback, long groupId, bool online = true, int timeout = 0);
	
参数:

+ `Action<HashSet<long>, HashSet<long>, int> callback`

	异步调用回调，第一个`HashSet<long>`参数为群组成员列表;第二个`HashSet<long>`为在线群组成员列表，若online参数为false则该字段为null;第三个`int`参数为错误码.

+ `long groupId`

	群组ID.

+ `bool online`

    是否查询群组成员在线状态

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 添加群组成员

	public bool AddGroupMembers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

	群组ID.

+ `HashSet<long> uids`

    要添加进群组的用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 删除群组成员

	public bool DeleteGroupMembers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

	群组ID.

+ `HashSet<long> uids`

    要添加进群组的用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取群组信息

	public bool GetGroupInfo(Action<string, string, int> callback, long groupId, int timeout = 0);
	
参数:

+ `Action<string, string, int> callback`

	异步调用回调，第一个参数`string`为公开信息，第二个参数`string`为内部信息，第三个参数`int`为错误码.

+ `long groupId`

	群组ID.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取群组公开信息

	public bool GetGroupsPublicInfo(Action<Dictionary<long, string>, int> callback, HashSet<long> groupIds, int timeout = 0);
	
参数:

+ `Action<Dictionary<long, string>, int> callback`

	异步调用回调，第一个参数`Dictionary<long, string>`为公开信息，key为群组ID，value为公开信息内容；第二个参数`int`为错误码.

+ `HashSet<long> groupIds`

	群组ID列表.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 设置群组信息

	public bool SetGroupInfo(DoneDelegate callback, long groupId, string publicInfo = null, string privateInfo = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

	群组ID.

+ `string publicInfo`

    公开信息

+ `string privateInfo`

    内部信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取用户所在群组列表

	public bool GetUserGroups(Action<HashSet<long>, int> callback, int timeout = 0);
	
参数:

+ `Action<HashSet<long>, int> callback`

	异步调用回调，第一个`HashSet<long>`参数为群组ID列表，第二个`int`为错误码.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


