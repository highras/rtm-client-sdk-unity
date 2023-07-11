using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.common;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
	public partial class IM 
    {
		public bool JoinRoom(DoneDelegate callback, long roomId, string extra = null, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_JoinRoom((int errorCode) => {
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, extra, attrs, timeout);
        }

		public bool AckJoinRoom(DoneDelegate callback, long roomId, long fromUid, bool agree, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_AckJoinRoom((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, fromUid, agree, attrs, timeout);
        }

		public bool InviteIntoRoom(DoneDelegate callback, long roomId, HashSet<long> uids, string extra = null, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_InviteIntoRoom((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, uids, extra, attrs, timeout);
        }

		public bool AckInviteIntoRoom(DoneDelegate callback, long roomId, long fromUid, bool agree, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_AckInviteIntoRoom((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, fromUid, agree, attrs, timeout);
        }

		public bool GetRoomList(IMLIB_GetRoomListDelegate callback, int timeout = 0)
		{
			return client.GetUserRooms((HashSet<long> roomList, int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(roomList, errorCode);
                });
            }, timeout);
        }

		public bool LeaveRoom(DoneDelegate callback, long roomId, int timeout = 0)
		{
			return client.IMLIB_LeaveRoom((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, timeout);
        }

		public bool DismissRoom(DoneDelegate callback, long roomId, int timeout = 0)
		{ 
			return client.IMLIB_DismissRoom((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, timeout);
        }

		public bool GetRoomInfos(IMLIB_GetRoomInfosDelegate callback, HashSet<long> roomIds, int timeout = 0)
		{
			return client.IMLIB_GetRoomInfos((List<IMLIB_RoomInfo> roomInfos, int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(roomInfos, errorCode);
                });
            }, roomIds, timeout);
        }

		public bool GetRoomMembers(IMLIB_GetRoomMembersDelegate callback, long roomId, int timeout = 0)
		{
			return client.IMLIB_GetRoomMembers((List<Dictionary<string, string>> roomMembersInfo, int errorCode) => {
				List<IMLIB_RoomMemberInfo> memberList = new List<IMLIB_RoomMemberInfo>();
				if (roomMembersInfo != null)
                {
                    foreach (var member in roomMembersInfo)
                    {
						IMLIB_RoomMemberInfo memberInfo = new IMLIB_RoomMemberInfo();
						member.TryGetValue("uid", out string uid);
						memberInfo.userId = Convert.ToInt64(uid);
						member.TryGetValue("role", out string role);
						memberInfo.role = (IMLIB_GroupRoomMemberRole)Convert.ToInt32(role);
                    }
                }
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(memberList, errorCode);
                });
            }, roomId, timeout);
        }

		public bool GetRoomMemberCount(IMLIB_GetRoomMemberCountDelegate callback, long roomId, int timeout = 0)
		{
			return client.IMLIB_GetRoomMemberCount((int memberCount, int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(memberCount, errorCode);
                });
            }, roomId, timeout);
        }

		public bool RemoveRoomMembers(DoneDelegate callback, long roomId, HashSet<long> uids, int timeout = 0)
		{
			return client.IMLIB_RemoveRoomMembers((int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, uids, timeout);
        }

		public bool TransferRoom(DoneDelegate callback, long roomId, long targetUid, int timeout = 0)
		{
			return client.IMLIB_TransferRoom((int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, targetUid, timeout);
        }

		public bool AddRoomManagers(DoneDelegate callback, long roomId, HashSet<long> uids, int timeout = 0)
		{
			return client.IMLIB_AddRoomManagers((int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, uids, timeout);
        }

		public bool RemoveRoomManagers(DoneDelegate callback, long roomId, HashSet<long> uids, int timeout = 0)
		{ 
			return client.IMLIB_RemoveRoomManagers((int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, roomId, uids, timeout);
        }

		public bool GetRoomInviteList(IMLIB_GetRoomInviteListDelegate callback, int timeout = 0)
		{
			return client.IMLIB_GetRoomInviteList((List<IMLIB_RoomInvite> inviteList, int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(inviteList, errorCode);
                });
            });
        }
	}
}

