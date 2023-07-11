using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class RTM
    {
        public bool AddFriends(DoneDelegate callback, HashSet<long> uids, int timeout = 0)
        {
            return client.AddFriends((int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, uids, timeout);
        }

        public bool DeleteFriends(DoneDelegate callback, HashSet<long> uids, int timeout = 0)
        {
            return client.DeleteFriends((int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, uids, timeout);
        }

        public bool GetFriends(Action<HashSet<long>, int> callback, int timeout = 0)
        {
            return client.GetFriends((HashSet<long> uids, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(uids, errorCode);
                });
            }, timeout);
        }

        public bool AddBlacklist(DoneDelegate callback, HashSet<long> uids, int timeout = 0)
        {
            return client.AddBlacklist((int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, uids, timeout);
        }

        public bool DeleteBlacklist(DoneDelegate callback, HashSet<long> uids, int timeout = 0)
        {
            return client.DeleteBlacklist((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, uids, timeout);
        }

        public bool GetBlacklist(Action<HashSet<long>, int> callback, int timeout = 0)
        {
            return client.GetBlacklist((HashSet<long> uids, int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(uids, errorCode);
                });
            }, timeout);
        }
    }
}

