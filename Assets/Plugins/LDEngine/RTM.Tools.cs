using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class RTM
    {
        public bool AddDevice(DoneDelegate callback, string appType, string deviceToken, int timeout = 0)
        {
            return client.AddDevice((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, appType, deviceToken, null, timeout);
        }

        public bool RemoveDevice(DoneDelegate callback, string deviceToken, int timeout = 0)
        {
            return client.RemoveDevice((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, deviceToken, null, timeout);
        }

        public bool AddDevicePushOption(DoneDelegate callback, MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return client.AddDevicePushOption((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, messageCategory, targetId, mTypes, timeout);
        }

        public bool RemoveDevicePushOption(DoneDelegate callback, MessageCategory messageCategory, long targetId, HashSet<byte> mTypes = null, int timeout = 0)
        {
            return client.RemoveDevicePushOption((int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, messageCategory, targetId, mTypes, timeout);
        }

        public bool GetDevicePushOption(Action<Dictionary<long, HashSet<byte>>, Dictionary<long, HashSet<byte>>, int> callback, int timeout = 0)
        {
            return client.GetDevicePushOption((Dictionary<long, HashSet<byte>> p2p, Dictionary<long, HashSet<byte>> group, int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(p2p, group, errorCode);
                });
            }, timeout);
        }

        public bool DataGet(Action<string, int> callback, string key, int timeout = 0)
        {
            return client.DataGet((string value, int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(value, errorCode);
                });
            }, key, timeout);
        }

        public bool DataSet(DoneDelegate callback, string key, string value, int timeout = 0)
        {
            return client.DataSet((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, key, value, timeout);
        }

        public bool DataDelete(DoneDelegate callback, string key, int timeout = 0)
        {
            return client.DataDelete((int errorCode) =>
            { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, key, timeout);
        }
    }
}

