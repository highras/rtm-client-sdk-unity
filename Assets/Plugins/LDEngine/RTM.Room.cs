using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class RTM
    {
        public bool EnterRoom(DoneDelegate callback, long roomId, int timeout = 0)
        {
            return client.EnterRoom((int errorCode) => 
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, roomId, timeout);
        }

        public bool LeaveRoom(DoneDelegate callback, long roomId, int timeout = 0)
        { 
            return client.LeaveRoom((int errorCode) => 
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, roomId, timeout);
        }

        public bool GetRoomMembers(Action<HashSet<long>, int> callback, long roomId, int timeout = 0)
        {
            return client.GetRoomMembers((HashSet<long> uids, int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(uids, errorCode);
                });
            }, roomId, timeout);
        }

        public bool GetRoomMemberCount(Action<Dictionary<long, int>, int> callback, HashSet<long> roomIds, int timeout = 0)
        {
            return client.GetRoomMemberCount((Dictionary<long, int> roomMemberCountDict, int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(roomMemberCountDict, errorCode);
                });
            }, roomIds, timeout);
        }

        public bool GetRoomInfo(Action<string, string, int> callback, long roomId, int timeout = 0)
        {
            return client.GetRoomInfo((string publicInfo, string privateInfo, int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(publicInfo, privateInfo, errorCode);
                });
            }, roomId, timeout);
        }

        public bool GetRoomsPublicInfo(Action<Dictionary<long, string>, int> callback, HashSet<long> roomIds, int timeout = 0)
        {
            return client.GetRoomsPublicInfo((Dictionary<long, string> publicInfos, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(publicInfos, errorCode);
                });
            }, roomIds, timeout);
        }

        public bool SetRoomInfo(DoneDelegate callback, long roomId, string publicInfo = null, string privateInfo = null, int timeout = 0)
        {
            return client.SetRoomInfo((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, roomId, publicInfo, privateInfo, timeout);
        }

        public bool GetUserRooms(Action<HashSet<long>, int> callback, int timeout = 0)
        {
            return client.GetUserRooms((HashSet<long> roomIds, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(roomIds, errorCode);
                });
            }, timeout);
        }
	}
}

