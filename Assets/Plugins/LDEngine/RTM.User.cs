using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class RTM
    {
        public bool GetUserInfo(Action<string, string, int> callback, int timeout = 0)
        {
            return client.GetUserInfo((string publicInfo, string privateInfo, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(publicInfo, privateInfo, errorCode);
                });
            }, timeout);
        }

        public bool GetUsersPublicInfo(Action<Dictionary<long, string>, int> callback, HashSet<long> uids, int timeout = 0)
        {
            return client.GetUserPublicInfo((Dictionary<long, string> userInfoDict, int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(userInfoDict, errorCode);
                });
            }, uids, timeout);
        }

        public bool SetUserInfo(DoneDelegate callback, string publicInfo = null, string privateInfo = null, int timeout = 0)
        {
            return client.SetUserInfo((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, publicInfo, privateInfo, timeout);
        }

        public bool GetUsersOnlineStatus(Action<HashSet<long>, int> callback, HashSet<long> uids, int timeout = 0)
        {
            return client.GetOnlineUsers((HashSet<long> onlineUids, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(onlineUids, errorCode);
                });
            }, uids, timeout);
        }
	}
}

