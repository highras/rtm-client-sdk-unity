using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public class IMLIB_FriendApply
    {
        public long uid;
        public long createTime;
        public int status;
        public string extra;
        public string attrs;
    }

    public class IMLIB_FriendRequest
    {
        public long uid;
        public long createTime;
        public string attrs;
    }

    public class IMLIB_GroupRequest
    {
        public long gid;
        public long createTime;
        public string attrs;
    }

    public class IMLIB_GroupApply
    {
        public long uid;
        public long inviteUid;
        public long createTime;
        public int status;
        public string extra;
        public string attrs;
    }

    public class IMLIB_GroupInvite
    {
        public long gid;
        public long fromUid;
        public long createTime;
        public string attrs;
    }

    public class IMLIB_RoomRequest
    {
        public long rid;
        public long createTime;
        public string attrs;
    }

    public class IMLIB_RoomApply
    {
        public long uid;
        public long inviteUid;
        public long createTime;
        public int status;
        public string extra;
        public string attrs;
    }

    public class IMLIB_RoomInvite
    {
        public long rid;
        public long fromUid;
        public long createTime;
        public string attrs;
    }

    public class IMLIB_UserInfo
    {
        public long uid;
        public string name;
        public string portraitUrl;
        public string profile;
        public string attrs;
        public IMLIB_ApplyGrant applyGrant;
    }

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

    public class IMLIB_GroupMemberInfo
    {
        public long userId;
        public IMLIB_GroupRoomMemberRole role;
        public bool online;
    }

    public class IMLIB_RoomInfo
    {
        public long roomId;
        public string name;
        public string portraitUrl;
        public string profile;
        public string attrs;
        public string extra;
        public long ownerUid;
        public List<long> managerUids;
        public IMLIB_ApplyGrant applyGrant;
        public IMLIB_InviteGrant inviteGrant;
    }

    public class IMLIB_RoomMemberInfo
    {
        public long userId;
        public IMLIB_GroupRoomMemberRole role;
    }

    public enum IMLIB_ApplyGrant
    {
        NONE = -1,
        ALL = 0,
        VERIFY = 1,
        DENYALL = 2,
    }

    public enum IMLIB_InviteGrant
    {
        NONE = -1,
        DENY = 0,
        ALL = 1,
    }

    public enum IMLIB_GroupRoomMemberRole
    {
        OWNER = 0,
        MANAGER = 1,
        MEMBER = 2,
    }

    public enum IMLIB_PushAppType
    {
        FCM = 0,
        APNS = 1,
    }
}

