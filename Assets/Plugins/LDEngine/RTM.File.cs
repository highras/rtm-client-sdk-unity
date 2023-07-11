using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class RTM
    {
        public bool SendFile(MessageIdDelegate callback, MessageCategory messageCategory, long id, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120)
        {
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendFile((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, type, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, type, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            { 
                return client.SendRoomFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, type, fileContent, filename, fileExtension, attrs, timeout);
            }
            else
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(0, rtm.ErrorCode.RTM_EC_INVALID_PARAMETER);
                });
                return false;
            }
        }

        public bool SendAudioMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, RTMAudioData audioData, string attrs = "", int timeout = 120)
        {
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendFile((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, audioData, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, audioData, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            { 
                return client.SendRoomFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, audioData, attrs, timeout);
            }
            else
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(0, rtm.ErrorCode.RTM_EC_INVALID_PARAMETER);
                });
                return false;
            }
        }

        public bool UploadFile(Action<string, uint, int> callback, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120)
        {
            return client.UploadFile((string url, uint size, int errorCode) => { 
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(url, size, errorCode);
                });
            }, type, fileContent, filename, fileExtension, attrs, timeout);
        }
    }
}

