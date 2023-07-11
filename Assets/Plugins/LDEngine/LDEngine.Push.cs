using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.common;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class LDEngine
    {
        RTMQuestProcessor processor;
        BasePushProcessor basePushProcessor;
        RTMPushProcessor rtmPushProcessor;
        IMPushProcessor imPushProcessor;

        public void SetBasePushProcessor(BasePushProcessor processor)
        {
            if (processor == null)
                return;
            basePushProcessor = processor;
            this.processor.SessionClosedCallback = processor.SessionClosedCallback;
            this.processor.ReloginWillStartCallback = processor.ReloginWillStartCallback;
            this.processor.ReloginCompletedCallback = processor.ReloginCompletedCallback;
            this.processor.KickoutCallback = processor.KickoutCallback;
        }

        public void SetRTMPushProcessor(RTMPushProcessor processor)
        {
            if (processor == null)
                return;
            if (imPushProcessor != null)
            {
                errorRecorder?.RecordError("imPushProcessor has been setted.");
                return;
            }
            rtmPushProcessor = processor;
            this.processor.KickoutRoomCallback = processor.KickoutRoomCallback;
            this.processor.PushMessageCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushBasicMessageCallback?.Invoke(MessageCategory.P2PMessage, message);
            };
            this.processor.PushGroupMessageCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushBasicMessageCallback?.Invoke(MessageCategory.GroupMessage, message);
            };
            this.processor.PushRoomMessageCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushBasicMessageCallback?.Invoke(MessageCategory.RoomMessage, message);
            };
            this.processor.PushBroadcastMessageCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushBasicMessageCallback?.Invoke(MessageCategory.BroadcastMessage, message);
            };
            this.processor.PushChatCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushChatMessageCallback?.Invoke(MessageCategory.P2PMessage, message);
            };
            this.processor.PushGroupChatCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushChatMessageCallback?.Invoke(MessageCategory.GroupMessage, message);
            };
            this.processor.PushRoomChatCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushChatMessageCallback?.Invoke(MessageCategory.RoomMessage, message);
            };
            this.processor.PushBroadcastChatCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushChatMessageCallback?.Invoke(MessageCategory.BroadcastMessage, message);
            };
            this.processor.PushCmdCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushCmdMessageCallback?.Invoke(MessageCategory.P2PMessage, message);
            };
            this.processor.PushGroupCmdCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushCmdMessageCallback?.Invoke(MessageCategory.GroupMessage, message);
            };
            this.processor.PushRoomCmdCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushCmdMessageCallback?.Invoke(MessageCategory.RoomMessage, message);
            };
            this.processor.PushBroadcastCmdCallback = (RTMMessage message) =>
            {
                rtmPushProcessor?.PushCmdMessageCallback?.Invoke(MessageCategory.BroadcastMessage, message);
            };

            this.processor.PushFileCallback = (RTMMessage message) =>
            {
                if (message.messageType == (byte)MessageType.AudioFile)
                {
                    rtmPushProcessor?.PushAudioMessageCallback(MessageCategory.P2PMessage, message);
                }
                else
                {
                    rtmPushProcessor?.PushFileMessageCallback?.Invoke(MessageCategory.P2PMessage, message);
                }
            };
            this.processor.PushGroupFileCallback = (RTMMessage message) =>
            {
                if (message.messageType == (byte)MessageType.AudioFile)
                {
                    rtmPushProcessor?.PushAudioMessageCallback(MessageCategory.GroupMessage, message);
                }
                else
                {
                    rtmPushProcessor?.PushFileMessageCallback?.Invoke(MessageCategory.GroupMessage, message);
                }
            };
            this.processor.PushRoomFileCallback = (RTMMessage message) =>
            {
                if (message.messageType == (byte)MessageType.AudioFile)
                {
                    rtmPushProcessor?.PushAudioMessageCallback(MessageCategory.RoomMessage, message);
                }
                else
                {
                    rtmPushProcessor?.PushFileMessageCallback?.Invoke(MessageCategory.RoomMessage, message);
                }
            };
            this.processor.PushBroadcastFileCallback = (RTMMessage message) =>
            {
                if (message.messageType == (byte)MessageType.AudioFile)
                {
                    rtmPushProcessor?.PushAudioMessageCallback(MessageCategory.BroadcastMessage, message);
                }
                else
                {
                    rtmPushProcessor?.PushFileMessageCallback?.Invoke(MessageCategory.BroadcastMessage, message);
                }
            };
        }

        public void SetIMPushProcessor(IMPushProcessor processor)
        {
            if (processor == null)
                return;
            if (rtmPushProcessor != null)
            {
                errorRecorder?.RecordError("rtmPushProcessor has been setted.");
                return;
            }
            imPushProcessor = processor;
            this.processor.KickoutRoomCallback = processor.KickoutRoomCallback;

            this.processor.PushChatCallback = (RTMMessage message) =>
            {
                imPushProcessor?.PushChatMessageCallback?.Invoke(MessageCategory.P2PMessage, message);
            };
            this.processor.PushGroupChatCallback = (RTMMessage message) =>
            {
                imPushProcessor?.PushChatMessageCallback?.Invoke(MessageCategory.GroupMessage, message);
            };
            this.processor.PushRoomChatCallback = (RTMMessage message) =>
            {
                imPushProcessor?.PushChatMessageCallback?.Invoke(MessageCategory.RoomMessage, message);
            };
            this.processor.PushBroadcastChatCallback = (RTMMessage message) =>
            {
                imPushProcessor?.PushChatMessageCallback?.Invoke(MessageCategory.BroadcastMessage, message);
            };

            this.processor.PushFileCallback = (RTMMessage message) =>
            {
                imPushProcessor?.PushFileMessageCallback?.Invoke(MessageCategory.P2PMessage, message);
            };
            this.processor.PushGroupFileCallback = (RTMMessage message) =>
            {
                imPushProcessor?.PushFileMessageCallback?.Invoke(MessageCategory.GroupMessage, message);
            };
            this.processor.PushRoomFileCallback = (RTMMessage message) =>
            {
                imPushProcessor?.PushFileMessageCallback?.Invoke(MessageCategory.RoomMessage, message);
            };
            this.processor.PushBroadcastFileCallback = (RTMMessage message) =>
            {
                imPushProcessor?.PushFileMessageCallback?.Invoke(MessageCategory.BroadcastMessage, message);
            };

            this.processor.PushMessageCallback = (RTMMessage message) =>
            {
                if (message.messageType != (int)MessageType.SystemNotification)
                    return;
                IMLIB_Push(message);
            };
            this.processor.PushGroupCmdCallback = (RTMMessage message) =>
            {
                IMLIB_GroupPush(message);
            };
            this.processor.PushRoomCmdCallback = (RTMMessage message) =>
            {
                IMLIB_RoomPush(message);
            };
        }

        void IMLIB_Push(RTMMessage msg)
        {
            try
            {
                Dictionary<string, object> attrsDict = Json.ParseObject(msg.attrs);
                if (attrsDict == null)
                    return;

                if (attrsDict.TryGetValue("type", out object type))
                {
                    int messageType = Convert.ToInt32(type);
                    if (messageType == (int)IMLIB_MessageType.AddFriendApply)
                    {
                        if (attrsDict.TryGetValue("from", out object from))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushAddFriendApplyCallback?.Invoke(Convert.ToInt64(from), msg.stringMessage, attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.AcceptFriendApply)
                    {
                        if (attrsDict.TryGetValue("userId", out object userId))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushAcceptFriendApplyCallback?.Invoke(Convert.ToInt64(userId), attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.RefuseFriendApply)
                    {
                        if (attrsDict.TryGetValue("userId", out object userId))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushRefuseFriendApplyCallback?.Invoke(Convert.ToInt64(userId), attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.EnterGroupApply)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("from", out object from)
                            && attrsDict.TryGetValue("invitedUid", out object invitedUid))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushEnterGroupApplyCallback?.Invoke(Convert.ToInt64(groupId), Convert.ToInt64(from), Convert.ToInt64(invitedUid), attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.AcceptEnterGroupApply)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("from", out object from))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushAcceptEnterGroupApplyCallback?.Invoke(Convert.ToInt64(groupId), Convert.ToInt64(from), attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.RefuseEnterGroupApply)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("from", out object from))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushRefuseEnterGroupApplyCallback?.Invoke(Convert.ToInt64(groupId), Convert.ToInt64(from), attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.InvitedIntoGroup)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("from", out object from))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushInvitedIntoGroupCallback?.Invoke(Convert.ToInt64(groupId), Convert.ToInt64(from), attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.AcceptInvitedIntoGroup)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("from", out object from))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushAccpetInvitedIntoGroupCallback?.Invoke(Convert.ToInt64(groupId), Convert.ToInt64(from), attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.RefuseInvitedIntoGroup)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("from", out object from))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushRefuseInvitedIntoGroupCallback?.Invoke(Convert.ToInt64(groupId), Convert.ToInt64(from), attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.FriendChanged)
                    {
                        if (attrsDict.TryGetValue("userId", out object userId)
                            && attrsDict.TryGetValue("changeType", out object changeType))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushFriendChangedCallback?.Invoke(Convert.ToInt64(userId), Convert.ToInt32(changeType), attrsStr);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.GroupChanged)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("changeType", out object changeType))
                        {
                            bool hasAttrs = attrsDict.TryGetValue("custom", out object attrs);
                            string attrsStr = hasAttrs ? Convert.ToString(attrs) : null;
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushGroupChangedCallback?.Invoke(Convert.ToInt64(groupId), Convert.ToInt32(changeType), attrsStr);
                            });
                        }
                    }

                }
            }
            catch (JsonException e)
            {
                errorRecorder?.RecordError("Parse message attrs error. Full msg: " + msg.attrs, e);
            }
        }

        void IMLIB_GroupPush(RTMMessage msg)
        {
            try
            {
                Dictionary<string, object> attrsDict = Json.ParseObject(msg.attrs);
                if (attrsDict == null)
                    return;

                if (attrsDict.TryGetValue("type", out object type))
                {
                    int messageType = Convert.ToInt32(type);
                    if (messageType == (int)IMLIB_MessageType.GroupMemberChanged)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("changeType", out object changeType)
                            && attrsDict.TryGetValue("from", out object from))
                        {
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushGroupMemberChangedCallback?.Invoke(Convert.ToInt64(groupId), Convert.ToInt32(changeType), Convert.ToInt64(from));
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.AddGroupManagers)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("managers", out object managers))
                        {
                            List<long> managerList = new List<long>();
                            List<object> originalList = (List<object>)managers;
                            foreach (object obj in originalList)
                                managerList.Add((long)Convert.ChangeType(obj, TypeCode.Int64));

                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushGroupManagerChangedCallback?.Invoke(Convert.ToInt64(groupId), 0, managerList);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.RemoveGroupManagers)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("managers", out object managers))
                        {
                            List<long> managerList = new List<long>();
                            List<object> originalList = (List<object>)managers;
                            foreach (object obj in originalList)
                                managerList.Add((long)Convert.ChangeType(obj, TypeCode.Int64));

                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushGroupManagerChangedCallback?.Invoke(Convert.ToInt64(groupId), 1, managerList);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.GroupOwnerChanged)
                    {
                        if (attrsDict.TryGetValue("groupId", out object groupId)
                            && attrsDict.TryGetValue("oldOwner", out object oldOwner)
                            && attrsDict.TryGetValue("newOwner", out object newOwner))
                        {
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushGroupOwnerChangedCallback?.Invoke(Convert.ToInt64(groupId), Convert.ToInt64(oldOwner), Convert.ToInt64(newOwner));
                            });
                        }
                    }
                }
            }
            catch (JsonException e)
            {
                errorRecorder?.RecordError("Parse message attrs error. Full msg: " + msg.attrs, e);
            }
        }

        void IMLIB_RoomPush(RTMMessage msg)
        {
            try
            {
                Dictionary<string, object> attrsDict = Json.ParseObject(msg.attrs);
                if (attrsDict == null)
                    return;

                if (attrsDict.TryGetValue("type", out object type))
                {
                    int messageType = Convert.ToInt32(type);
                    if (messageType == (int)IMLIB_MessageType.RoomMemberChanged)
                    {
                        if (attrsDict.TryGetValue("roomId", out object roomId)
                            && attrsDict.TryGetValue("changeType", out object changeType)
                            && attrsDict.TryGetValue("from", out object from))
                        {
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushRoomMemberChangedCallback?.Invoke(Convert.ToInt64(roomId), Convert.ToInt32(changeType), Convert.ToInt64(from));
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.AddRoomManagers)
                    {
                        if (attrsDict.TryGetValue("roomId", out object roomId)
                            && attrsDict.TryGetValue("managers", out object managers))
                        {
                            List<long> managerList = new List<long>();
                            List<object> originalList = (List<object>)managers;
                            foreach (object obj in originalList)
                                managerList.Add((long)Convert.ChangeType(obj, TypeCode.Int64));

                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushRoomManagerChangedCallback?.Invoke(Convert.ToInt64(roomId), 0, managerList);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.RemoveGroupManagers)
                    {
                        if (attrsDict.TryGetValue("roomId", out object roomId)
                            && attrsDict.TryGetValue("managers", out object managers))
                        {
                            List<long> managerList = new List<long>();
                            List<object> originalList = (List<object>)managers;
                            foreach (object obj in originalList)
                                managerList.Add((long)Convert.ChangeType(obj, TypeCode.Int64));

                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushRoomManagerChangedCallback?.Invoke(Convert.ToInt64(roomId), 1, managerList);
                            });
                        }
                    }
                    else if (messageType == (int)IMLIB_MessageType.RoomOwnerChanged)
                    {
                        if (attrsDict.TryGetValue("roomId", out object roomId)
                            && attrsDict.TryGetValue("oldOwner", out object oldOwner)
                            && attrsDict.TryGetValue("newOwner", out object newOwner))
                        {
                            RTMControlCenter.callbackQueue.PostAction(() =>
                            {
                                imPushProcessor?.IMLIB_PushRoomOwnerChangedCallback?.Invoke(Convert.ToInt64(roomId), Convert.ToInt64(oldOwner), Convert.ToInt64(newOwner));
                            });
                        }
                    }
                }
            }
            catch (JsonException e)
            {
                errorRecorder?.RecordError("Parse message attrs error. Full msg: " + msg.attrs, e);
            }
        }
    }
}