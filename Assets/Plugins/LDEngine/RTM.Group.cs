using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class RTM
    {
        public bool GetGroupMemberCount(Action<int, int> callback, long groupId, int timeout = 0)
        {
            return client.GetGroupCount((int count, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(count, errorCode);
                });
            }, groupId, timeout);
        }

        public bool GetGroupMembers(Action<HashSet<long>, HashSet<long>, int> callback, long groupId, bool online = true, int timeout = 0)
        {
            if (online)
            {
                return client.GetGroupMembers((HashSet<long> members, HashSet<long> onlineMembers, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(members, onlineMembers, errorCode);
                    });
                }, groupId, timeout);
            }
            else
            {
                return client.GetGroupMembers((HashSet<long> members, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(members, null, errorCode);
                    });
                }, groupId, timeout);
            }
        }

        public bool AddGroupMembers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
        {
            return client.AddGroupMembers((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, groupId, uids, timeout);
        }

        public bool DeleteGroupMembers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
        {
            return client.DeleteGroupMembers((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, groupId, uids, timeout);
        }

        public bool GetGroupInfo(Action<string, string, int> callback, long groupId, int timeout = 0)
        {
            return client.GetGroupInfo((string publicInfo, string privateInfo, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(publicInfo, privateInfo, errorCode);
                });
            }, groupId, timeout);
        }

        public bool GetGroupsPublicInfo(Action<Dictionary<long, string>, int> callback, HashSet<long> groupIds, int timeout = 0)
        {
            return client.GetGroupsPublicInfo((Dictionary<long, string> infos, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(infos, errorCode);
                });
            }, groupIds, timeout);
        }

        public bool SetGroupInfo(DoneDelegate callback, long groupId, string publicInfo = null, string privateInfo = null, int timeout = 0)
        {
            return client.SetGroupInfo((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, groupId, publicInfo, privateInfo, timeout);
        }

        public bool GetUserGroups(Action<HashSet<long>, int> callback, int timeout = 0)
        {
            return client.GetUserGroups((HashSet<long> groupIds, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(groupIds, errorCode);
                });
            }, timeout);
        }
	}
}

