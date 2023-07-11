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
		public bool GetUserInfos(IMLIB_GetUserInfosDelegate callback, HashSet<long> uids, int timeout = 0)
		{
			return client.IMLIB_GetUserInfos((List<IMLIB_UserInfo> userInfos, int errorCode) => {
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(userInfos, errorCode);
                });
            }, uids, timeout);
        }

		public bool AddDevice(DoneDelegate callback, IMLIB_PushAppType appType, string deviceToken, int timeout = 0)
		{
			string type = appType == IMLIB_PushAppType.FCM ? "fcm" : "apns";
			return client.AddDevice((int errorCode) => {
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, type, deviceToken, null, timeout);
        }

		public bool RemoveDevice(DoneDelegate callback, string deviceToken, int timeout = 0)
		{ 
			return client.RemoveDevice((int errorCode) => {
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, deviceToken, null, timeout);
        }

		public bool AddDevicePushOption(DoneDelegate callback, MessageCategory messageCategory, long targetId, HashSet<byte> mtypes = null, int timeout = 0)
		{
			return client.AddDevicePushOption((int errorCode) => {
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, messageCategory, targetId, mtypes, timeout);
        }

		public bool RemoveDevicePushOption(DoneDelegate callback, MessageCategory messageCategory, long targetId, HashSet<byte> mtypes = null, int timeout = 0)
		{ 
			return client.RemoveDevicePushOption((int errorCode) => {
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(errorCode);
                });
            }, messageCategory, targetId, mtypes, timeout);
        }

		public bool GetDevicePushOption(GetDevicePushOptionDelegate callback, int timeout = 0)
		{
			return client.GetDevicePushOption((Dictionary<long, HashSet<byte>> p2p, Dictionary<long, HashSet<byte>> group, int errorCode) => {
				DevicePushOption option = new DevicePushOption();
				option.p2p = p2p;
				option.group = group;
				RTMControlCenter.callbackQueue.PostAction(() => {
					callback(option, errorCode);
                });
            }, timeout);
        }
	}
}

