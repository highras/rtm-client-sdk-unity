using System.Collections;
using System.Collections.Generic;
using com.fpnn.common;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
	public partial class IM 
 	{
        public bool SendImageFile(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120)
        { 
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendFile((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.ImageFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.ImageFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            { 
                return client.SendRoomFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.ImageFile, fileContent, filename, fileExtension, attrs, timeout);
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

        public bool SendAudioFile(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120)
        { 
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendFile((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.VoiceFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.VoiceFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            { 
                return client.SendRoomFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.VoiceFile, fileContent, filename, fileExtension, attrs, timeout);
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

        public bool SendVideoFile(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120)
        { 
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendFile((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.VideoFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.VideoFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            { 
                return client.SendRoomFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.VideoFile, fileContent, filename, fileExtension, attrs, timeout);
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

        public bool SendAudioMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120)
        { 
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendFile((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.AudioFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.AudioFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            { 
                return client.SendRoomFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.AudioFile, fileContent, filename, fileExtension, attrs, timeout);
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

        public bool SendNormalFile(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120)
        {
            if (messageCategory == MessageCategory.P2PMessage)
            {
                return client.SendFile((long messageId, int errorCode) =>
                {
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.NormalFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.GroupMessage)
            {
                return client.SendGroupFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.NormalFile, fileContent, filename, fileExtension, attrs, timeout);
            }
            else if (messageCategory == MessageCategory.RoomMessage)
            { 
                return client.SendRoomFile((long messageId, int errorCode) =>
                { 
                    RTMControlCenter.callbackQueue.PostAction(() =>
                    {
                        callback(messageId, errorCode);
                    });
                }, id, MessageType.NormalFile, fileContent, filename, fileExtension, attrs, timeout);
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
    }
}

