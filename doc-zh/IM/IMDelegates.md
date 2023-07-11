# 回调代理类文档

### IMLIB_GetGroupListDelegate

	public delegate void IMLIB_GetGroupListDelegate(HashSet<long> groupList, int errorCode);

参数:

+ `HashSet<long> groupList`

	群组ID列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetGroupInfosDelegate

	public delegate void IMLIB_GetGroupInfosDelegate(List<IMLIB_GroupInfo> groupInfos, int errorCode);

参数:

+ `List<IMLIB_GroupInfo> groupInfos`

	群组信息列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetGroupMembersDelegate

	public delegate void IMLIB_GetGroupMembersDelegate(List<IMLIB_GroupMemberInfo> groupMemberList, int errorCode);

参数:

+ `List<IMLIB_GroupMemberInfo> groupMemberList`

	群组成员信息列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetGroupMemberCountDelegate

	public delegate void IMLIB_GetGroupMemberCountDelegate(int memberCount, int errorCode);

参数:

+ `int memberCount`

	群组成员数量

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetGroupApplyListDelegate

	public delegate void IMLIB_GetGroupApplyListDelegate(List<IMLIB_GroupApply> applyList, int errorCode);

参数:

+ `List<IMLIB_GroupApply> applyList`

    加入群组申请列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetGroupInviteListDelegate

	public delegate void IMLIB_GetGroupInviteListDelegate(List<IMLIB_GroupInvite> inviteList, int errorCode);

参数:

+ `List<IMLIB_GroupInvite> inviteList`

    邀请加入群组列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetGroupRequestListDelegate

	public delegate void IMLIB_GetGroupRequestListDelegate(List<IMLIB_GroupRequest> requestList, int errorCode);

参数:

+ `List<IMLIB_GroupRequest> requestList`

    自己发出的加入群组申请列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetRoomListDelegate

	public delegate void IMLIB_GetRoomListDelegate(HashSet<long> roomList, int errorCode);

参数:

+ `HashSet<long> roomList`

    房间列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetRoomInfosDelegate

	public delegate void IMLIB_GetRoomInfosDelegate(List<IMLIB_RoomInfo> roomInfos, int errorCode);

参数:

+ `List<IMLIB_RoomInfo> roomInfos`

    房间信息列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetRoomMembersDelegate

	public delegate void IMLIB_GetRoomMembersDelegate(List<IMLIB_RoomMemberInfo> roomMemberList, int errorCode);

参数:

+ `List<IMLIB_RoomMemberInfo> roomMemberList`

    房间成员信息列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetRoomMemberCountDelegate

	public delegate void IMLIB_GetRoomMemberCountDelegate(int memberCount, int errorCode);

参数:

+ `int memberCount`

    房间成员数量

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)



### IMLIB_GetRoomInviteListDelegate

	public delegate void IMLIB_GetRoomInviteListDelegate(List<IMLIB_RoomInvite> inviteList, int errorCode);

参数:

+ `List<IMLIB_RoomInvite> inviteList`

    邀请加入房间列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetUserInfosDelegate

	public delegate void IMLIB_GetUserInfosDelegate(List<IMLIB_UserInfo> userInfos, int errorCode);

参数:

+ `List<IMLIB_UserInfo> userInfos`

    用户信息列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetFriendApplyListDelegate

	public delegate void IMLIB_GetFriendApplyListDelegate(List<IMLIB_FriendApply> applyList, int errorCode);

参数:

+ `List<IMLIB_FriendApply> applyList`

    加好友申请列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)


### IMLIB_GetFriendRequestListDelegate

	public delegate void IMLIB_GetFriendRequestListDelegate(List<IMLIB_FriendRequest> requestList, int errorCode);

参数:

+ `List<IMLIB_FriendRequest> requestList`

    自己发出的加好友申请列表

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)

