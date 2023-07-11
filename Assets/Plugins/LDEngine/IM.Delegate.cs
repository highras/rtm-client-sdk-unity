using System.Collections;
using System.Collections.Generic;
using com.fpnn.common;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public delegate void IMLIB_PushFriendChangedDelegate(long uid, int changeType, string attrs);
    public delegate void IMLIB_PushAddFriendApplyDelegate(long uid, string msg, string attrs);
    public delegate void IMLIB_PushAcceptFriendApplyDelegate(long uid, string attrs);
    public delegate void IMLIB_PushRefuseFriendApplyDelegate(long uid, string attrs);

    public delegate void IMLIB_PushEnterGroupApplyDelegate(long groupId, long from, long invitedUid, string attrs);
    public delegate void IMLIB_PushAcceptEnterGroupApplyDelegate(long groupId, long from, string attrs);
    public delegate void IMLIB_PushRefuseEnterGroupApplyDelegate(long groupId, long from, string attrs);
    public delegate void IMLIB_PushInvitedIntoGroupDelegate(long groupId, long from, string attrs);
    public delegate void IMLIB_PushAccpetInvitedIntoGroupDelegate(long groupId, long from, string attrs);
    public delegate void IMLIB_PushRefuseInvitedIntoGroupDelegate(long groupId, long from, string attrs);
    public delegate void IMLIB_PushGroupChangedDelegate(long groupId, int changType, string attrs);
    public delegate void IMLIB_PushGroupMemberChangedDelegate(long groupId, int changeType, long from);
    public delegate void IMLIB_PushGroupOwnerChangedDelegate(long groupId, long oldOwner, long newOwner);
    public delegate void IMLIB_PushGroupManagerChangedDelegate(long groupId, int changeType, List<long> managers);

    public delegate void IMLIB_PushEnterRoomApplyDelegate(long roomId, long from, long invitedUid, string attrs);
    public delegate void IMLIB_PushAcceptEnterRoomApplyDelegate(long roomId, long from, string attrs);
    public delegate void IMLIB_PushRefuseEnterRoomApplyDelegate(long roomId, long from, string attrs);
    public delegate void IMLIB_PushInvitedIntoRoomDelegate(long roomId, long from, string attrs);
    public delegate void IMLIB_PushAccpetInvitedIntoRoomDelegate(long roomId, long from, string attrs);
    public delegate void IMLIB_PushRefuseInvitedIntoRoomDelegate(long roomId, long from, string attrs);
    public delegate void IMLIB_PushRoomChangedDelegate(long roomId, int changType, string attrs);
    public delegate void IMLIB_PushRoomMemberChangedDelegate(long roomId, int changeType, long from);
    public delegate void IMLIB_PushRoomOwnerChangedDelegate(long roomId, long oldOwner, long newOwner);
    public delegate void IMLIB_PushRoomManagerChangedDelegate(long roomId, int changeType, List<long> managers);

    public delegate void IMLIB_GetGroupListDelegate(HashSet<long> groupList, int errorCode);
    public delegate void IMLIB_GetGroupInfosDelegate(List<IMLIB_GroupInfo> groupInfos, int errorCode);
    public delegate void IMLIB_GetGroupMembersDelegate(List<IMLIB_GroupMemberInfo> groupMemberList, int errorCode);
    public delegate void IMLIB_GetGroupMemberCountDelegate(int memberCount, int errorCode);
    public delegate void IMLIB_GetGroupApplyListDelegate(List<IMLIB_GroupApply> applyList, int errorCode);
    public delegate void IMLIB_GetGroupInviteListDelegate(List<IMLIB_GroupInvite> inviteList, int errorCode);
    public delegate void IMLIB_GetGroupRequestListDelegate(List<IMLIB_GroupRequest> requestList, int errorCode);

    public delegate void IMLIB_GetRoomListDelegate(HashSet<long> roomList, int errorCode);
    public delegate void IMLIB_GetRoomInfosDelegate(List<IMLIB_RoomInfo> roomInfos, int errorCode);
    public delegate void IMLIB_GetRoomMembersDelegate(List<IMLIB_RoomMemberInfo> roomMemberList, int errorCode);
    public delegate void IMLIB_GetRoomMemberCountDelegate(int memberCount, int errorCode);
    public delegate void IMLIB_GetRoomInviteListDelegate(List<IMLIB_RoomInvite> inviteList, int errorCode);

    public delegate void IMLIB_GetUserInfosDelegate(List<IMLIB_UserInfo> userInfos, int errorCode);
    public delegate void IMLIB_GetFriendApplyListDelegate(List<IMLIB_FriendApply> applyList, int errorCode);
    public delegate void IMLIB_GetFriendRequestListDelegate(List<IMLIB_FriendRequest> requestList, int errorCode);
}