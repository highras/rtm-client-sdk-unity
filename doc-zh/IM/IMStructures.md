# 数据结构类文档

### IMLIB_FriendApply

    public class IMLIB_FriendApply
    {
        public long uid;
        public long createTime;
        public int status;
        public string extra;
        public string attrs;
    }

添加好友申请

* `long uid`

    申请人用户ID


* `long createTime`

    申请创建时间

* `int status`

    申请当前状态

* `string extra`

    留言

* `string attrs`

    附加信息


### IMLIB_FriendRequest

    public class IMLIB_FriendRequest
    {
        public long uid;
        public long createTime;
        public string attrs;
    }

自己发出的好友申请

* `long uid`

    申请人用户ID


* `long createTime`

    申请创建时间

* `string attrs`

    附加信息


### IMLIB_GroupApply

    public class IMLIB_GroupApply
    {
        public long uid;
        public long inviteUid;
        public long createTime;
        public int status;
        public string extra;
        public string attrs;
    }

加入群组申请

* `long uid`

    申请人用户ID

* `long inviteUid`

    邀请人用户ID

* `long createTime`

    申请创建时间

* `int status`

    申请当前状态

* `string extra`

    留言

* `string attrs`

    附加信息



### IMLIB_GroupRequest

    public class IMLIB_GroupRequest
    {
        public long gid;
        public long createTime;
        public string attrs;
    }

自己发出的加入群组申请

* `long gid`

    群组ID

* `long createTime`

    请求创建时间

* `string attrs`

    附加信息


### IMLIB_GroupInvite

    public class IMLIB_GroupInvite
    {
        public long gid;
        public long fromUid;
        public long createTime;
        public string attrs;
    }

加入群组邀请

* `long gid`

    群组ID

* `long fromUid`

    邀请者用户ID

* `long createTime`

    邀请创建时间

* `string attrs`

    附加信息

### IMLIB_RoomApply

    public class IMLIB_RoomApply
    {
        public long uid;
        public long inviteUid;
        public long createTime;
        public int status;
        public string extra;
        public string attrs;
    }

加入房间申请

* `long uid`

    申请人用户ID

* `long inviteUid`

    邀请人用户ID

* `long createTime`

    申请创建时间

* `int status`

    申请当前状态

* `string extra`

    留言

* `string attrs`

    附加信息



### IMLIB_RoomRequest

    public class IMLIB_RoomRequest
    {
        public long rid;
        public long createTime;
        public string attrs;
    }

自己发出的加入群组申请

* `long rid`

    房间ID

* `long createTime`

    请求创建时间

* `string attrs`

    附加信息


### IMLIB_RoomInvite

    public class IMLIB_RoomInvite
    {
        public long rid;
        public long fromUid;
        public long createTime;
        public string attrs;
    }

加入房间邀请

* `long rid`

    房间ID

* `long fromUid`

    邀请者用户ID

* `long createTime`

    邀请创建时间

* `string attrs`

    附加信息


### IMLIB_UserInfo

    public class IMLIB_UserInfo
    {
        public long uid;
        public string name;
        public string portraitUrl;
        public string profile;
        public string attrs;
        public IMLIB_ApplyGrant applyGrant;
    }

用户信息

* `long uid`

    用户ID

* `string name`

    用户名字

* `string portraitUrl`

    头像URL

* `string profile`

    简介

* `string attrs`

    附加信息

* `IMLIB_ApplyGrant applyGrant`

    申请加好友权限


### IMLIB_GroupInfo

    public class IMLIB_GroupInfo
    {
        public long groupId;
        public string name;
        public string portraitUrl;
        public string profile;
        public string attrs;
        public long ownerUid;
        public List<long> managerUids;
        public IMLIB_ApplyGrant applyGrant;
        public IMLIB_InviteGrant inviteGrant;
    }

群组信息

* `long groupId`

    群组ID

* `string name`

    群组名字

* `string portraitUrl`

    头像URL

* `string profile`

    简介

* `string attrs`

    附加信息

* `long ownerUid`

    群主用户ID

* `List<long> managerUids`

    管理员用户ID列表

* `IMLIB_ApplyGrant applyGrant`

    申请加入群组权限

* `IMLIB_InviteGrant inviteGrant`

    邀请加入群组权限


### IMLIB_GroupMemberInfo
    public class IMLIB_GroupMemberInfo
    {
        public long userId;
        public IMLIB_GroupRoomMemberRole role;
        public bool online;
    }

群组成员信息

* `long userId`

    用户ID

* `IMLIB_GroupRoomMemberRole role`

    群组成员角色

* `bool online`

    是否在线


### IMLIB_RoomInfo

    public class IMLIB_RoomInfo
    {
        public long roomId;
        public string name;
        public string portraitUrl;
        public string profile;
        public string attrs;
        public long ownerUid;
        public List<long> managerUids;
        public IMLIB_ApplyGrant applyGrant;
        public IMLIB_InviteGrant inviteGrant;
    }

房间信息

* `long roomId`

    房间ID

* `string name`

    房间名字

* `string portraitUrl`

    头像URL

* `string profile`

    简介

* `string attrs`

    附加信息

* `long ownerUid`

    房主用户ID

* `List<long> managerUids`

    管理员用户ID列表

* `IMLIB_ApplyGrant applyGrant`

    申请加入房间权限

* `IMLIB_InviteGrant inviteGrant`

    邀请加入房间权限


### IMLIB_RoomMemberInfo
    public class IMLIB_RoomMemberInfo
    {
        public long userId;
        public IMLIB_GroupRoomMemberRole role;
    }

房间成员信息

* `long userId`

    用户ID

* `IMLIB_GroupRoomMemberRole role`

    房间成员角色


### IMLIB_ApplyGrant
    public enum IMLIB_ApplyGrant
    {
        NONE = -1,
        ALL = 0,    //直接添加
        VERIFY = 1, //需要审核
        DENYALL = 2,//禁止添加
    }


### IMLIB_InviteGrant
    public enum IMLIB_InviteGrant
    {
        NONE = -1,
        DENY = 0,   //不允许成员邀请
        ALL = 1,    //所有人都可以邀请
    }


### IMLIB_GroupRoomMemberRole
    public enum IMLIB_GroupRoomMemberRole
    {
        OWNER = 0,  //群主/房主
        MANAGER = 1,//管理员
        MEMBER = 2, //成员
    }


### IMLIB_PushAppType
    public enum IMLIB_PushAppType
    {
        FCM = 0,
        APNS = 1,
    }