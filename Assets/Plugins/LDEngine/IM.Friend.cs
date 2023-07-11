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
		public bool AddFriend(DoneDelegate callback, long uid, string extra = null, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_AddFriend((int errorCode) => {
				RTMControlCenter.callbackQueue.PostAction(() =>
				{
					callback(errorCode);
				});
            }, uid, extra, attrs, timeout);
        }

		public bool AckAddFriend(DoneDelegate callback, long uid, bool agree, string attrs = null, int timeout = 0)
		{
			return client.IMLIB_AckAddFriend((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() =>
				{
					callback(errorCode);
				});
            }, uid, agree, attrs, timeout);
        }

		public bool DeleteFriends(DoneDelegate callback, HashSet<long> uids, int timeout = 0)
		{
			return client.DeleteFriends((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() =>
				{
					callback(errorCode);
				});
            }, uids, timeout);
        }

		public bool GetFriendList(GetFriendListDelegate callback, int timeout = 0)
		{
			return client.GetFriends((HashSet<long> friendList, int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() =>
				{
					callback(friendList, errorCode);
				});
            }, timeout);
        }

		public bool AddBlackList(DoneDelegate callback, HashSet<long> uids, int timeout = 0)
		{
			return client.AddBlacklist((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() =>
				{
					callback(errorCode);
				});
            }, uids, timeout);
        }

		public bool DeleteBlackList(DoneDelegate callback, HashSet<long> uids, int timeout = 0)
		{ 
			return client.DeleteBlacklist((int errorCode) => { 
				RTMControlCenter.callbackQueue.PostAction(() =>
				{
					callback(errorCode);
				});
            }, uids, timeout);
        }

		public bool GetBlackList(GetBlackListDelegate callback, int timeout = 0)
        {
            return client.GetBlacklist((HashSet<long> uids, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(uids, errorCode);
                });
            }, timeout);
        }

		public bool GetFriendApplyList(IMLIB_GetFriendApplyListDelegate callback, int timeout = 0)
		{
			return client.IMLIB_GetFriendApplyList((List<IMLIB_FriendApply> applyList, int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(applyList, errorCode);
                });
            }, timeout);
        }

		public bool GetFriendRequestList(IMLIB_GetFriendRequestListDelegate callback, int timeout = 0)
		{
			return client.IMLIB_GetFriendRequestList((List<IMLIB_FriendRequest> requestList, int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(requestList, errorCode);
                });
            }, timeout);
        }
 
    }
}