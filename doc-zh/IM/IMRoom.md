# 房间接口文档

### 加入房间

	public bool JoinRoom(DoneDelegate callback, long roomId, string extra = null, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

	房间ID

+ `string extra`

    附加信息

+ `string attrs`

    属性信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 回应加入房间

	public bool AckJoinRoom(DoneDelegate callback, long roomId, long fromUid, bool agree, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

	房间ID

+ `bool agree`

    是否同意

+ `string attrs`

    属性信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 邀请加入房间

	public bool InviteIntoRoom(DoneDelegate callback, long roomId, HashSet<long> uids, string extra = null, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

	房间ID

+ `HashSet<long> uids`

    邀请用户ID列表

+ `string extra`

    附加信息

+ `string attrs`

    属性信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 回应邀请加入房间

	public bool AckInviteIntoRoom(DoneDelegate callback, long roomId, long fromUid, bool agree, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

	房间ID

+ `long fromUid`

    邀请用户ID

+ `bool agree`

    是否同意

+ `string attrs`

    属性信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取已加入房间列表

	public bool GetRoomList(IMLIB_GetRoomListDelegate callback, int timeout = 0);
	
参数:

+ `IMLIB_GetRoomListDelegate callback`

	异步调用回调，请参照[IMLIB_GetRoomListDelegate](IMDelegates.md#IMLIB_GetRoomListDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 退出房间

	public bool LeaveRoom(DoneDelegate callback, long roomId, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

    房间ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 解散房间

	public bool DismissRoom(DoneDelegate callback, long roomId, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

    房间ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取房间信息

	public bool GetRoomInfos(IMLIB_GetRoomInfosDelegate callback, HashSet<long> roomIds, int timeout = 0);
	
参数:

+ `IMLIB_GetRoomInfosDelegate callback`

	异步调用回调，请参照[IMLIB_GetRoomInfosDelegate](IMDelegates.md#IMLIB_GetRoomInfosDelegate).

+ `HashSet<long> roomIds`

    房间ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取房间成员信息

	public bool GetRoomMembers(IMLIB_GetRoomMembersDelegate callback, long roomId, int timeout = 0);
	
参数:

+ `IMLIB_GetRoomMembersDelegate callback`

	异步调用回调，请参照[IMLIB_GetRoomMembersDelegate](IMDelegates.md#IMLIB_GetRoomMembersDelegate).

+ `long roomId`

    房间ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取房间成员数量

	public bool GetRoomMemberCount(IMLIB_GetRoomMemberCountDelegate callback, long roomId, int timeout = 0);
	
参数:

+ `IMLIB_GetRoomMemberCountDelegate callback`

	异步调用回调，请参照[IMLIB_GetRoomMemberCountDelegate](IMDelegates.md#IMLIB_GetRoomMemberCountDelegate).

+ `long roomId`

    房间ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 移除房间成员

	public bool RemoveRoomMembers(DoneDelegate callback, long roomId, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

    房间ID

+ `HashSet<long> uids`

    要移除的用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 转让群主

	public bool TransferRoom(DoneDelegate callback, long roomId, long targetUid, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

    房间ID

+ `long targetUid`

    要转让的用户ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 添加房间管理员

	public bool AddRoomManagers(DoneDelegate callback, long roomId, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

    房间ID

+ `HashSet<long> uids`

    要添加为管理员的用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 移除房间管理员

	public bool RemoveRoomManagers(DoneDelegate callback, long roomId, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

    房间ID

+ `HashSet<long> uids`

    要移除的管理员用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取加入房间申请列表

	public bool GetRoomApplyList(IMLIB_GetRoomApplyListDelegate callback, long roomId, int timeout = 0);
	
参数:

+ `IMLIB_GetRoomApplyListDelegate callback`

	异步调用回调，请参照[IMLIB_GetRoomApplyListDelegate](IMDelegates.md#IMLIB_GetRoomApplyListDelegate).

+ `long roomId`

    房间ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取邀请加入房间列表

	public bool GetRoomInviteList(IMLIB_GetRoomInviteListDelegate callback, int timeout = 0);
	
参数:

+ `IMLIB_GetRoomInviteListDelegate callback`

	异步调用回调，请参照[IMLIB_GetRoomInviteListDelegate](IMDelegates.md#IMLIB_GetRoomInviteListDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取自己申请加入房间列表

	public bool GetRoomRequestList(IMLIB_GetRoomRequestListDelegate callback, int timeout = 0);
	
参数:

+ `IMLIB_GetRoomRequestListDelegate callback`

	异步调用回调，请参照[IMLIB_GetRoomRequestListDelegate](IMDelegates.md#IMLIB_GetRoomRequestListDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.