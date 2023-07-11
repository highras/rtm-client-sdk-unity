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
		public bool JoinGroup(DoneDelegate callback, long groupId, string extra = null, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_JoinGroup((int errorCode) => {
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, extra, attrs, timeout);
        }

		public bool AckJoinGroup(DoneDelegate callback, long groupId, long fromUid, bool agree, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_AckJoinGroup((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, fromUid, agree, attrs, timeout);
        }

		public bool InviteIntoGroup(DoneDelegate callback, long groupId, HashSet<long> uids, string extra = null, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_InviteIntoGroup((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, uids, extra, attrs, timeout);
        }

		public bool AckInviteIntoGroup(DoneDelegate callback, long groupId, long fromUid, bool agree, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_AckInviteIntoGroup((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, fromUid, agree, attrs, timeout);
        }

		public bool GetGroupList(IMLIB_GetGroupListDelegate callback, int timeout = 0)
		{
			return client.GetUserGroups((HashSet<long> groupList, int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(groupList, errorCode);
                });
            }, timeout);
        }

		public bool LeaveGroup(DoneDelegate callback, long groupId, int timeout = 0)
		{
			return client.IMLIB_LeaveGroup((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, timeout);
        }

		public bool DismissGroup(DoneDelegate callback, long groupId, int timeout = 0)
		{ 
			return client.IMLIB_DismissGroup((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, timeout);
        }

		public bool GetGroupInfos(IMLIB_GetGroupInfosDelegate callback, HashSet<long> groupIds, int timeout = 0)
		{
			return client.IMLIB_GetGroupInfos((List<IMLIB_GroupInfo> groupInfos, int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(groupInfos, errorCode);
                });
            }, groupIds, timeout);
        }

		public bool GetGroupMembers(IMLIB_GetGroupMembersDelegate callback, long groupId, int timeout = 0)
		{
			return client.IMLIB_GetGroupMembers((List<Dictionary<string, string>> groupMembersInfo, int errorCode) => {
				List<IMLIB_GroupMemberInfo> memberList = new List<IMLIB_GroupMemberInfo>();
				if (groupMembersInfo != null)
                {
                    foreach (var member in groupMembersInfo)
                    {
						IMLIB_GroupMemberInfo memberInfo = new IMLIB_GroupMemberInfo();
						member.TryGetValue("uid", out string uid);
						memberInfo.userId = Convert.ToInt64(uid);
						member.TryGetValue("role", out string role);
						memberInfo.role = (IMLIB_GroupRoomMemberRole)Convert.ToInt32(role);
						member.TryGetValue("online", out string online);
						memberInfo.online = (online == "1");
                    }
                }
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(memberList, errorCode);
                });
            }, groupId, timeout);
        }

		public bool GetGroupMemberCount(IMLIB_GetGroupMemberCountDelegate callback, long groupId, int timeout = 0)
		{
			return client.IMLIB_GetGroupMemberCount((int memberCount, int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(memberCount, errorCode);
                });
            }, groupId, timeout);
        }

		public bool RemoveGroupMembers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
		{
			return client.IMLIB_RemoveGroupMembers((int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, uids, timeout);
        }

		public bool TransferGroup(DoneDelegate callback, long groupId, long targetUid, int timeout = 0)
		{
			return client.IMLIB_TransferGroup((int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, targetUid, timeout);
        }

		public bool AddGroupManagers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
		{
			return client.IMLIB_AddGroupManagers((int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, uids, timeout);
        }

		public bool RemoveGroupManagers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
		{ 
			return client.IMLIB_RemoveGroupManagers((int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, groupId, uids, timeout);
        }

		public bool GetGroupApplyList(IMLIB_GetGroupApplyListDelegate callback, long groupId, int timeout = 0)
		{
			return client.IMLIB_GetGroupApplyList((List<IMLIB_GroupApply> applyList, int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(applyList, errorCode);
                });
            }, groupId, timeout);
        }

		public bool GetGroupInviteList(IMLIB_GetGroupInviteListDelegate callback, int timeout = 0)
		{
			return client.IMLIB_GetGroupInviteList((List<IMLIB_GroupInvite> inviteList, int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(inviteList, errorCode);
                });
            }, timeout);
        }

		public bool GetGroupRequestList(IMLIB_GetGroupRequestListDelegate callback, int timeout = 0)
		{
			return client.IMLIB_GetGroupRequestList((List<IMLIB_GroupRequest> requestList, int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() => {
					callback(requestList, errorCode);
                });
            }, timeout);
        }
	}
}

