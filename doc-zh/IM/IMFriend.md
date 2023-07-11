# 好友接口文档

### 添加好友

	public bool AddFriend(DoneDelegate callback, long uid, string extra = null, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long uid`

	好友用户ID

+ `string extra`

    附加信息

+ `string attrs`

    属性信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 回应添加好友

	public bool AckAddFriend(DoneDelegate callback, long uid, bool agree, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long uid`

	好友用户ID

+ `bool agree`

    是否同意添加好友请求

+ `string attrs`

    属性信息

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

	好友用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取好友列表

	public bool GetFriendList(GetFriendListDelegate callback, int timeout = 0);
	
参数:

+ `GetFriendListDelegate callback`

	异步调用回调，请参照[GetFriendListDelegate](IMDelegates.md#GetFriendListDelegate).

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

	public bool GetBlackList(GetBlackListDelegate callback, int timeout = 0);
	
参数:

+ `GetBlackListDelegate callback`

	异步调用回调，请参照[GetBlackListDelegate](IMDelegates.md#GetBlackListDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取加好友申请列表

	public bool GetFriendApplyList(IMLIB_GetFriendApplyListDelegate callback, int timeout = 0);
	
参数:

+ `IMLIB_GetFriendApplyListDelegate callback`

	异步调用回调，请参照[IMLIB_GetFriendApplyListDelegate](IMDelegates.md#IMLIB_GetFriendApplyListDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取自己发出的加好友申请列表

	public bool GetFriendRequestList(IMLIB_GetFriendRequestListDelegate callback, int timeout = 0);
	
参数:

+ `IMLIB_GetFriendRequestListDelegate callback`

	异步调用回调，请参照[IMLIB_GetFriendRequestListDelegate](IMDelegates.md#IMLIB_GetFriendRequestListDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.