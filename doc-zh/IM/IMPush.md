# IM推送文档

## IMPushProcessor
+ 回调事件类为IMPushProcessor
+ 所有回调会在主线程触发

## Delegate

### KickoutRoomDelegate
    public delegate void KickoutRoomDelegate(long roomId);

参数:

+ `long roomId`

    被踢出的房间ID


### RTMPushMessageDelegate
    public delegate void RTMPushMessageDelegate(MessageCategory messageCategory, RTMMessage message);

参数:

+ `MessageCategory messageCategory`

    消息种类

+ `RTMMessage message`

    消息内容


### IMLIB_PushFriendChangedDelegate
    public delegate void IMLIB_PushFriendChangedDelegate(long uid, int changeType, string attrs);

参数:

+ `long uid`

    好友用户ID

+ `int changeType`

    改变类型，0添加好友，1删除好友

+ `string attrs`

    附加属性

### IMLIB_PushAddFriendApplyDelegate
    public delegate void IMLIB_PushAddFriendApplyDelegate(long uid, string msg, string attrs);

参数:

+ `long uid`

    好友用户ID

+ `string msg`

    留言

+ `string attrs`

    附加属性


### IMLIB_PushAcceptFriendApplyDelegate
    public delegate void IMLIB_PushAcceptFriendApplyDelegate(long uid, string attrs);

参数:

+ `long uid`

    好友用户ID

+ `string attrs`

    附加属性


### IMLIB_PushRefuseFriendApplyDelegate
    public delegate void IMLIB_PushRefuseFriendApplyDelegate(long uid, string attrs);

参数:

+ `long uid`

    好友用户ID

+ `string attrs`

    附加属性


### IMLIB_PushEnterGroupApplyDelegate
    public delegate void IMLIB_PushEnterGroupApplyDelegate(long groupId, long from, long invitedUid, string attrs);

参数:

+ `long groupId`

    群组ID

+ `long from`

    申请者用户ID

+ `long invitedUid`

    邀请者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushAcceptEnterGroupApplyDelegate
    public delegate void IMLIB_PushAcceptEnterGroupApplyDelegate(long groupId, long from, string attrs);

参数:

+ `long groupId`

    群组ID

+ `long from`

    审批者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushRefuseEnterGroupApplyDelegate
    public delegate void IMLIB_PushRefuseEnterGroupApplyDelegate(long groupId, long from, string attrs);

参数:

+ `long groupId`

    群组ID

+ `long from`

    审批者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushInvitedIntoGroupDelegate
    public delegate void IMLIB_PushInvitedIntoGroupDelegate(long groupId, long from, string attrs);

参数:

+ `long groupId`

    群组ID

+ `long from`

    邀请者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushAccpetInvitedIntoGroupDelegate
    public delegate void IMLIB_PushAccpetInvitedIntoGroupDelegate(long groupId, long from, string attrs);

参数:

+ `long groupId`

    群组ID

+ `long from`

    审批者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushRefuseInvitedIntoGroupDelegate
    public delegate void IMLIB_PushRefuseInvitedIntoGroupDelegate(long groupId, long from, string attrs);

参数:

+ `long groupId`

    群组ID

+ `long from`

    审批者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushGroupChangedDelegate
    public delegate void IMLIB_PushGroupChangedDelegate(long groupId, int changType, string attrs);

参数:

+ `long groupId`

    群组ID

+ `int changeType`

    改变类型，0加入,1主动离开,2群组解散,3被踢出

+ `string attrs`

    附加属性


### IMLIB_PushGroupMemberChangedDelegate
    public delegate void IMLIB_PushGroupMemberChangedDelegate(long groupId, int changeType, long from);

参数:

+ `long groupId`

    群组ID

+ `int changeType`

    改变类型，0加入,1主动离开,2群组解散,3被踢出

+ `string attrs`

    附加属性


### IMLIB_PushGroupOwnerChangedDelegate
    public delegate void IMLIB_PushGroupOwnerChangedDelegate(long groupId, long oldOwner, long newOwner);

参数:

+ `long groupId`

    群组ID

+ `long oldOwner`

    原群主用户ID

+ `long newOwner`

    新群主用户ID


### IMLIB_PushGroupManagerChangedDelegate
    public delegate void IMLIB_PushGroupManagerChangedDelegate(long groupId, int changeType, List<long> managers);

参数:

+ `long groupId`

    群组ID

+ `int changeType`

    改变类型，0是添加管理员，1是删除管理员

+ `List<long> managers`

    改变的管理员用户ID


### IMLIB_PushEnterRoomApplyDelegate
    public delegate void IMLIB_PushEnterRoomApplyDelegate(long roomId, long from, long invitedUid, string attrs);

参数:

+ `long roomId`

    房间ID

+ `long from`

    申请者用户ID

+ `long invitedUid`

    邀请者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushAcceptEnterRoomApplyDelegate
    public delegate void IMLIB_PushAcceptEnterRoomApplyDelegate(long roomId, long from, string attrs);

参数:

+ `long roomId`

    房间ID

+ `long from`

    审批者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushRefuseEnterRoomApplyDelegate
    public delegate void IMLIB_PushRefuseEnterRoomApplyDelegate(long roomId, long from, string attrs);

参数:

+ `long roomId`

    房间ID

+ `long from`

    审批者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushInvitedIntoRoomDelegate
    public delegate void IMLIB_PushInvitedIntoRoomDelegate(long roomId, long from, string attrs);

参数:

+ `long roomId`

    房间ID

+ `long from`

    邀请者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushAccpetInvitedIntoRoomDelegate
    public delegate void IMLIB_PushAccpetInvitedIntoRoomDelegate(long roomId, long from, string attrs);

参数:

+ `long roomId`

    房间ID

+ `long from`

    审批者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushRefuseInvitedIntoRoomDelegate
    public delegate void IMLIB_PushRefuseInvitedIntoRoomDelegate(long roomId, long from, string attrs);

参数:

+ `long roomId`

    房间ID

+ `long from`

    审批者用户ID

+ `string attrs`

    附加属性


### IMLIB_PushRoomChangedDelegate
    public delegate void IMLIB_PushRoomChangedDelegate(long roomId, int changType, string attrs);

参数:

+ `long roomId`

    房间ID

+ `int changeType`

    改变类型，0加入,1主动离开,2群组解散,3被踢出

+ `string attrs`

    附加属性


### IMLIB_PushRoomMemberChangedDelegate
    public delegate void IMLIB_PushRoomMemberChangedDelegate(long roomId, int changeType, long from);

参数:

+ `long roomId`

    房间ID

+ `int changeType`

    改变类型，0加入,1主动离开,2群组解散,3被踢出

+ `string attrs`

    附加属性


### IMLIB_PushRoomOwnerChangedDelegate
    public delegate void IMLIB_PushRoomOwnerChangedDelegate(long roomId, long oldOwner, long newOwner);

参数:

+ `long roomId`

    房间ID

+ `long oldOwner`

    原房主用户ID

+ `long newOwner`

    新房主用户ID


### IMLIB_PushRoomManagerChangedDelegate
    public delegate void IMLIB_PushRoomManagerChangedDelegate(long roomId, int changeType, List<long> managers);

参数:

+ `long roomId`

    房间ID

+ `int changeType`

    改变类型，0是添加管理员，1是删除管理员

+ `List<long> managers`

    改变的管理员用户ID


## 推送回调函数

### 被踢出房间
    public KickoutRoomDelegate KickoutRoomCallback;

### 收到聊天消息
    public RTMPushMessageDelegate PushChatMessageCallback;

### 收到文件消息
    public RTMPushMessageDelegate PushFileMessageCallback;

### 好友变更
    public IMLIB_PushFriendChangedDelegate IMLIB_PushFriendChangedCallback;

### 收到添加好友申请
    public IMLIB_PushAddFriendApplyDelegate IMLIB_PushAddFriendApplyCallback;

### 同意添加好友申请
    public IMLIB_PushAcceptFriendApplyDelegate IMLIB_PushAcceptFriendApplyCallback;

### 拒绝添加好友申请
    public IMLIB_PushRefuseFriendApplyDelegate IMLIB_PushRefuseFriendApplyCallback;

### 收到申请入群申请
    public IMLIB_PushEnterGroupApplyDelegate IMLIB_PushEnterGroupApplyCallback;

### 同意入群申请
    public IMLIB_PushAcceptEnterGroupApplyDelegate IMLIB_PushAcceptEnterGroupApplyCallback;

### 拒绝入群申请
    public IMLIB_PushRefuseEnterGroupApplyDelegate IMLIB_PushRefuseEnterGroupApplyCallback;

### 收到入群邀请
    public IMLIB_PushInvitedIntoGroupDelegate IMLIB_PushInvitedIntoGroupCallback;

### 同意入群邀请
    public IMLIB_PushAccpetInvitedIntoGroupDelegate IMLIB_PushAccpetInvitedIntoGroupCallback;

### 拒绝入群邀请
    public IMLIB_PushRefuseInvitedIntoGroupDelegate IMLIB_PushRefuseInvitedIntoGroupCallback;

### 群组变更
    public IMLIB_PushGroupChangedDelegate IMLIB_PushGroupChangedCallback;

### 群成员变更
    public IMLIB_PushGroupMemberChangedDelegate IMLIB_PushGroupMemberChangedCallback;

### 群主变更
    public IMLIB_PushGroupOwnerChangedDelegate IMLIB_PushGroupOwnerChangedCallback;

### 群管理员变更
    public IMLIB_PushGroupManagerChangedDelegate IMLIB_PushGroupManagerChangedCallback;

### 收到进入房间申请
    public IMLIB_PushEnterRoomApplyDelegate IMLIB_PushEnterRoomApplyCallback;

### 同意进入房间申请
    public IMLIB_PushAcceptEnterRoomApplyDelegate IMLIB_PushAcceptEnterRoomApplyCallback;

### 拒绝进入房间申请
    public IMLIB_PushRefuseEnterRoomApplyDelegate IMLIB_PushRefuseEnterRoomApplyCallback;

### 收到进入房间邀请
    public IMLIB_PushInvitedIntoRoomDelegate IMLIB_PushInvitedIntoRoomCallback;

### 同意进入房间邀请
    public IMLIB_PushAccpetInvitedIntoRoomDelegate IMLIB_PushAccpetInvitedIntoRoomCallback;

### 拒绝进入房间邀请
    public IMLIB_PushRefuseInvitedIntoRoomDelegate IMLIB_PushRefuseInvitedIntoRoomCallback;

### 房间变更
    public IMLIB_PushRoomChangedDelegate IMLIB_PushRoomChangedCallback;

### 房间成员变更
    public IMLIB_PushRoomMemberChangedDelegate IMLIB_PushRoomMemberChangedCallback;

### 房主变更
    public IMLIB_PushRoomOwnerChangedDelegate IMLIB_PushRoomOwnerChangedCallback;

### 房间管理员变更
    public IMLIB_PushRoomManagerChangedDelegate IMLIB_PushRoomManagerChangedCallback;