# 群组接口文档

### 加入群组

	public bool JoinGroup(DoneDelegate callback, long groupId, string extra = null, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

	群组ID

+ `string extra`

    附加信息

+ `string attrs`

    属性信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 回应加入群组

	public bool AckJoinGroup(DoneDelegate callback, long groupId, long fromUid, bool agree, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

	群组ID

+ `bool agree`

    是否同意

+ `string attrs`

    属性信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 邀请加入群组

	public bool InviteIntoGroup(DoneDelegate callback, long groupId, HashSet<long> uids, string extra = null, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

	群组ID

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


### 回应邀请加入群组

	public bool AckInviteIntoGroup(DoneDelegate callback, long groupId, long fromUid, bool agree, string attrs = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

	群组ID

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


### 获取已加入群组列表

	public bool GetGroupList(IMLIB_GetGroupListDelegate callback, int timeout = 0);
	
参数:

+ `IMLIB_GetGroupListDelegate callback`

	异步调用回调，请参照[IMLIB_GetGroupListDelegate](IMDelegates.md#IMLIB_GetGroupListDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 退出群组

	public bool LeaveGroup(DoneDelegate callback, long groupId, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

    群组ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 解散群组

	public bool DismissGroup(DoneDelegate callback, long groupId, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

    群组ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取群组信息

	public bool GetGroupInfos(IMLIB_GetGroupInfosDelegate callback, HashSet<long> groupIds, int timeout = 0);
	
参数:

+ `IMLIB_GetGroupInfosDelegate callback`

	异步调用回调，请参照[IMLIB_GetGroupInfosDelegate](IMDelegates.md#IMLIB_GetGroupInfosDelegate).

+ `HashSet<long> groupIds`

    群组ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取群组成员信息

	public bool GetGroupMembers(IMLIB_GetGroupMembersDelegate callback, long groupId, int timeout = 0);
	
参数:

+ `IMLIB_GetGroupMembersDelegate callback`

	异步调用回调，请参照[IMLIB_GetGroupMembersDelegate](IMDelegates.md#IMLIB_GetGroupMembersDelegate).

+ `long groupId`

    群组ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取群组成员数量

	public bool GetGroupMemberCount(IMLIB_GetGroupMemberCountDelegate callback, long groupId, int timeout = 0);
	
参数:

+ `IMLIB_GetGroupMemberCountDelegate callback`

	异步调用回调，请参照[IMLIB_GetGroupMemberCountDelegate](IMDelegates.md#IMLIB_GetGroupMemberCountDelegate).

+ `long groupId`

    群组ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 移除群组成员

	public bool RemoveGroupMembers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

    群组ID

+ `HashSet<long> uids`

    要移除的用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 转让群主

	public bool TransferGroup(DoneDelegate callback, long groupId, long targetUid, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

    群组ID

+ `long targetUid`

    要转让的用户ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 添加群组管理员

	public bool AddGroupManagers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

    群组ID

+ `HashSet<long> uids`

    要添加为管理员的用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 移除群组管理员

	public bool RemoveGroupManagers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long groupId`

    群组ID

+ `HashSet<long> uids`

    要移除的管理员用户ID列表

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取加入群组申请列表

	public bool GetGroupApplyList(IMLIB_GetGroupApplyListDelegate callback, long groupId, int timeout = 0);
	
参数:

+ `IMLIB_GetGroupApplyListDelegate callback`

	异步调用回调，请参照[IMLIB_GetGroupApplyListDelegate](IMDelegates.md#IMLIB_GetGroupApplyListDelegate).

+ `long groupId`

    群组ID

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取邀请加入群组列表

	public bool GetGroupInviteList(IMLIB_GetGroupInviteListDelegate callback, int timeout = 0);
	
参数:

+ `IMLIB_GetGroupInviteListDelegate callback`

	异步调用回调，请参照[IMLIB_GetGroupInviteListDelegate](IMDelegates.md#IMLIB_GetGroupInviteListDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取自己申请加入群组列表

	public bool GetGroupRequestList(IMLIB_GetGroupRequestListDelegate callback, int timeout = 0);
	
参数:

+ `IMLIB_GetGroupRequestListDelegate callback`

	异步调用回调，请参照[IMLIB_GetGroupRequestListDelegate](IMDelegates.md#IMLIB_GetGroupRequestListDelegate).

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.