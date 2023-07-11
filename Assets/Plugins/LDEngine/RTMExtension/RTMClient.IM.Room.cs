using UnityEngine;
using System.Collections;
using com.fpnn.livedata;
using System.Collections.Generic;
using System;
using com.fpnn.proto;
using static com.fpnn.rtm.RTMClient;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //===========================[ IMLIB_GetRoomInfos ]=========================//
        public bool IMLIB_GetRoomInfos(Action<List<IMLIB_RoomInfo>, int> callback, HashSet<long> roomIds, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_getroominfos");
            quest.Param("rids", roomIds);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                List<IMLIB_RoomInfo> infos = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        infos = new List<IMLIB_RoomInfo>();
                        List<long> xid = GetLongList(answer, "xid");
                        List<long> ownerUids = GetLongList(answer, "ownerUid");
                        List<string> names = GetStringList(answer, "name");
                        List<string> portraitUrls = GetStringList(answer, "portraitUrl");
                        List<string> profiles = GetStringList(answer, "profile");
                        List<string> attrs = GetStringList(answer, "attrs");
                        List<string> extra = GetStringList(answer, "extra");
                        List<int> applyGrants = GetIntList(answer, "applyGrant");
                        List<int> inviteGrants = GetIntList(answer, "inviteGrant");
                        List<List<long>> managerUids = GetLongListList(answer, "managerUids");
                        for (int i = 0; i < xid.Count && i < ownerUids.Count && i < names.Count && i < portraitUrls.Count && i < profiles.Count && i < attrs.Count && i < applyGrants.Count && i < inviteGrants.Count && i < managerUids.Count; ++i)
                        {
                            IMLIB_RoomInfo info = new IMLIB_RoomInfo();
                            info.roomId = xid[i];
                            info.ownerUid = ownerUids[i];
                            info.name = names[i];
                            info.portraitUrl = portraitUrls[i];
                            info.profile = profiles[i];
                            info.attrs = attrs[i];
                            info.extra = extra[i];
                            info.applyGrant = (IMLIB_ApplyGrant)applyGrants[i];
                            info.inviteGrant = (IMLIB_InviteGrant)inviteGrants[i];
                            info.managerUids = managerUids[i];
                            infos.Add(info);
                        }
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(infos, errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_GetRoomInfos(out List<IMLIB_RoomInfo> infos, HashSet<long> roomIds, int timeout = 0)
        {
            infos = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getroominfos");
            quest.Param("rids", roomIds);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                infos = new List<IMLIB_RoomInfo>();
                List<long> xid = GetLongList(answer, "xid");
                List<long> ownerUids = GetLongList(answer, "ownerUid");
                List<string> names = GetStringList(answer, "name");
                List<string> portraitUrls = GetStringList(answer, "portraitUrl");
                List<string> profiles = GetStringList(answer, "profile");
                List<string> attrs = GetStringList(answer, "attrs");
                List<string> extra = GetStringList(answer, "extra");
                List<int> applyGrants = GetIntList(answer, "applyGrant");
                List<int> inviteGrants = GetIntList(answer, "inviteGrant");
                List<List<long>> managerUids = GetLongListList(answer, "managerUids");
                for (int i = 0; i < xid.Count && i < ownerUids.Count && i < names.Count && i < portraitUrls.Count && i < profiles.Count && i < attrs.Count && i < applyGrants.Count && i < inviteGrants.Count && i < managerUids.Count; ++i)
                {
                    IMLIB_RoomInfo info = new IMLIB_RoomInfo();
                    info.roomId = xid[i];
                    info.ownerUid = ownerUids[i];
                    info.name = names[i];
                    info.portraitUrl = portraitUrls[i];
                    info.profile = profiles[i];
                    info.attrs = attrs[i];
                    info.extra = extra[i];
                    info.applyGrant = (IMLIB_ApplyGrant)applyGrants[i];
                    info.inviteGrant = (IMLIB_InviteGrant)inviteGrants[i];
                    info.managerUids = managerUids[i];
                    infos.Add(info);
                }
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }
        //===========================[ IMLIB_GetRoomInfo ]=========================//
        public bool IMLIB_GetRoomInfo(Action<IMLIB_RoomInfo, int> callback, long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_getroominfo");
            quest.Param("rid", roomId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                IMLIB_RoomInfo info = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        info = new IMLIB_RoomInfo();
                        info.roomId = roomId;
                        info.name = answer.Want<string>("name");
                        info.ownerUid = answer.Want<long>("ownerUid");
                        info.portraitUrl = answer.Want<string>("portraitUrl");
                        info.profile = answer.Want<string>("profile");
                        info.attrs = answer.Want<string>("attrs");
                        info.extra = answer.Want<string>("extra");
                        info.applyGrant = (IMLIB_ApplyGrant)answer.Want<int>("applyGrant");
                        info.inviteGrant = (IMLIB_InviteGrant)answer.Want<int>("inviteGrant");
                        info.managerUids = GetLongList(answer, "managerUids");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(info, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_GetRoomInfo(out IMLIB_RoomInfo info, long roomId, int timeout = 0)
        {
            info = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getroominfo");
            quest.Param("rid", roomId);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                info = new IMLIB_RoomInfo();
                info.roomId = roomId;
                info.name = answer.Want<string>("name");
                info.ownerUid = answer.Want<long>("ownerUid");
                info.portraitUrl = answer.Want<string>("portraitUrl");
                info.profile = answer.Want<string>("profile");
                info.attrs = answer.Want<string>("attrs");
                info.extra = answer.Want<string>("extra");
                info.applyGrant = (IMLIB_ApplyGrant)answer.Want<int>("applyGrant");
                info.inviteGrant = (IMLIB_InviteGrant)answer.Want<int>("inviteGrant");
                info.managerUids = GetLongList(answer, "managerUids");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_JoinRoom ]=========================//
        public bool IMLIB_JoinRoom(DoneDelegate callback, long roomId, string extra = null, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_joinroom");
            quest.Param("rid", roomId);
            if (extra != null)
                quest.Param("extra", extra);
            if (attrs != null)
                quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_JoinRoom(long roomId, string extra = null, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_joinroom");
            quest.Param("rid", roomId);
            if (extra != null)
                quest.Param("extra", extra);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_AckJoinRoom ]=========================//
        public bool IMLIB_AckJoinRoom(DoneDelegate callback, long roomId, long fromUid, bool agree, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_ackjoinroom");
            quest.Param("rid", roomId);
            quest.Param("from", fromUid);
            quest.Param("agree", agree);
            if (attrs != null)
                quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_AckJoinRoom(long roomId, long fromUid, bool agree, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_ackjoinroom");
            quest.Param("rid", roomId);
            quest.Param("from", fromUid);
            quest.Param("agree", agree);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_InviteIntoRoom ]=========================//
        public bool IMLIB_InviteIntoRoom(DoneDelegate callback, long roomId, HashSet<long> uids, string extra = null, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_inviteintoroom");
            quest.Param("rid", roomId);
            quest.Param("target_uids", uids);
            if (extra != null)
                quest.Param("extra", extra);
            if (attrs != null)
                quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_InviteIntoRoom(long roomId, HashSet<long> uids, string extra = null, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_inviteintoroom");
            quest.Param("rid", roomId);
            quest.Param("target_uids", uids);
            if (extra != null)
                quest.Param("extra", extra);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_AckInviteIntoRoom ]=========================//
        public bool IMLIB_AckInviteIntoRoom(DoneDelegate callback, long roomId, long fromUid, bool agree, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_ackinviteintoroom");
            quest.Param("rid", roomId);
            quest.Param("from", fromUid);
            quest.Param("agree", agree);
            if (attrs != null)
                quest.Param("attrs", attrs);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_AckInviteIntoRoom(long roomId, long fromUid, bool agree, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_ackinviteintoroom");
            quest.Param("rid", roomId);
            quest.Param("from", fromUid);
            quest.Param("agree", agree);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_DismissRoom ]=========================//
        public bool IMLIB_DismissRoom(DoneDelegate callback, long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_dismissroom");
            quest.Param("rid", roomId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_DismissRoom(long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_dismissroom");
            quest.Param("rid", roomId);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_RemoveRoomMembers ]=========================//
        public bool IMLIB_RemoveRoomMembers(DoneDelegate callback, long roomId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_removeroommembers");
            quest.Param("rid", roomId);
            quest.Param("uids", uids);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_RemoveRoomMembers(long roomId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_removeroommembers");
            quest.Param("rid", roomId);
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_LeaveRoom ]=========================//
        public bool IMLIB_LeaveRoom(DoneDelegate callback, long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_leaveroom");
            quest.Param("rid", roomId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_LeaveRoom(long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_leaveroom");
            quest.Param("rid", roomId);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_TransferRoom ]=========================//
        public bool IMLIB_TransferRoom(DoneDelegate callback, long roomId, long uid, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_transferroom");
            quest.Param("rid", roomId);
            quest.Param("to_uid", uid);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_TransferRoom(long roomId, long uid, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_transferroom");
            quest.Param("rid", roomId);
            quest.Param("to_uid", uid);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_AddRoomManagers ]=========================//
        public bool IMLIB_AddRoomManagers(DoneDelegate callback, long roomId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_addroommanagers");
            quest.Param("rid", roomId);
            quest.Param("uids", uids);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_AddRoomManagers(long roomId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_addroommanagers");
            quest.Param("rid", roomId);
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_RemoveRoomManagers ]=========================//
        public bool IMLIB_RemoveRoomManagers(DoneDelegate callback, long roomId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_removeroommanagers");
            quest.Param("rid", roomId);
            quest.Param("uids", uids);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_RemoveRoomManagers(long roomId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_removeroommanagers");
            quest.Param("rid", roomId);
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_AddRoomMemberBan ]=========================//
        public bool IMLIB_AddRoomMemberBan(DoneDelegate callback, long roomId, long uid, int banTime, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_addroommemberban");
            quest.Param("rid", roomId);
            quest.Param("to_uid", uid);
            quest.Param("ban_time", banTime);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_AddRoomMemberBan(long roomId, long uid, int banTime, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_addroommemberban");
            quest.Param("rid", roomId);
            quest.Param("to_uid", uid);
            quest.Param("ban_time", banTime);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_RemoveRoomMemberBan ]=========================//
        public bool IMLIB_RemoveRoomMemberBan(DoneDelegate callback, long roomId, long uid, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_removeroommemberban");
            quest.Param("rid", roomId);
            quest.Param("to_uid", uid);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                callback(errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_RemoveRoomMemberBan(long roomId, long uid, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_removeroommemberban");
            quest.Param("rid", roomId);
            quest.Param("to_uid", uid);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_GetRoomMembers ]=========================//
        public bool IMLIB_GetRoomMembers(Action<List<Dictionary<string, string>>, int> callback, long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_getroommembers");
            quest.Param("rid", roomId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        data = GetListStringDictionary(answer, "data");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(data, errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_GetRoomMembers(out List<Dictionary<string, string>> data, long roomId, int timeout = 0)
        {
            data = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getroommembers");
            quest.Param("rid", roomId);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                data = GetListStringDictionary(answer, "data");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetRoomMemberCount ]=========================//
        public bool IMLIB_GetRoomMemberCount(Action<int, int> callback, long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_getroommembercount");
            quest.Param("rid", roomId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                int count = 0;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        count = answer.Want<int>("cn");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(count, errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_GetRoomMemberCount(out int count, long roomId, int timeout = 0)
        { 
            count = -1;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getroommembercount");
            quest.Param("rid", roomId);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                count = answer.Want<int>("cn");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetRoomApplyList ]=========================//
        public bool IMLIB_GetRoomApplyList(Action<List<IMLIB_RoomApply>, int> callback, long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_getroomapplylist");
            quest.Param("rid", roomId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                List<IMLIB_RoomApply> applyList = new List<IMLIB_RoomApply>();
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> fromUid = GetLongList(answer, "fromUid");
                        List<int> status = GetIntList(answer, "status");
                        List<string> grantExtra = GetStringList(answer, "grantExtra");
                        List<long> createTime = GetLongList(answer, "createTime");
                        List<long> inviteUid = GetLongList(answer, "inviteUid");
                        List<string> attrs = GetStringList(answer, "attrs");
                        for (int i = 0; i < fromUid.Count && i < status.Count && i < grantExtra.Count && i < createTime.Count && i < inviteUid.Count && i < attrs.Count; ++i)
                        {
                            IMLIB_RoomApply roomApply = new IMLIB_RoomApply();
                            roomApply.uid = fromUid[i];
                            roomApply.status = status[i];
                            roomApply.extra = grantExtra[i];
                            roomApply.createTime = createTime[i];
                            roomApply.inviteUid = inviteUid[i];
                            roomApply.attrs = attrs[i];
                            applyList.Add(roomApply);
                        }
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(applyList, errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_GetRoomApplyList(out List<IMLIB_RoomApply> applyList, long roomId, int timeout = 0)
        {
            applyList = new List<IMLIB_RoomApply>();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getroomapplylist");
            quest.Param("rid", roomId); ;

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                List<long> fromUid = GetLongList(answer, "fromUid");
                List<int> status = GetIntList(answer, "status");
                List<string> grantExtra = GetStringList(answer, "grantExtra");
                List<long> createTime = GetLongList(answer, "createTime");
                List<long> inviteUid = GetLongList(answer, "inviteUid");
                List<string> attrs = GetStringList(answer, "attrs");
                for (int i = 0; i < fromUid.Count && i < status.Count && i < grantExtra.Count && i < createTime.Count && i < inviteUid.Count && i < attrs.Count; ++i)
                {
                    IMLIB_RoomApply roomApply = new IMLIB_RoomApply();
                    roomApply.uid = fromUid[i];
                    roomApply.status = status[i];
                    roomApply.extra = grantExtra[i];
                    roomApply.createTime = createTime[i];
                    roomApply.inviteUid = inviteUid[i];
                    roomApply.attrs = attrs[i];
                    applyList.Add(roomApply);
                }
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetRoomRequestList ]=========================//
        public bool IMLIB_GetRoomRequestList(Action<List<IMLIB_RoomRequest>, int> callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_getroomrequestlist");

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                List<IMLIB_RoomRequest> requestList = new List<IMLIB_RoomRequest>();
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> targetXid = GetLongList(answer, "targetXid");
                        List<long> createTime = GetLongList(answer, "createTime");
                        List<string> attrs = GetStringList(answer, "attrs");
                        for (int i = 0; i < targetXid.Count && i < createTime.Count && i < attrs.Count; ++i)
                        {
                            IMLIB_RoomRequest roomRequest = new IMLIB_RoomRequest();
                            roomRequest.rid = targetXid[i];
                            roomRequest.createTime = createTime[i];
                            roomRequest.attrs = attrs[i];
                            requestList.Add(roomRequest);
                        }
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(requestList, errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_GetRoomRequestList(out List<IMLIB_RoomRequest> requestList, int timeout = 0)
        {
            requestList = new List<IMLIB_RoomRequest>();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getroomrequestlist");

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                List<long> targetXid = GetLongList(answer, "targetXid");
                List<long> createTime = GetLongList(answer, "createTime");
                List<string> attrs = GetStringList(answer, "attrs");
                for (int i = 0; i < targetXid.Count && i < createTime.Count && i < attrs.Count; ++i)
                {
                    IMLIB_RoomRequest roomRequest = new IMLIB_RoomRequest();
                    roomRequest.rid = targetXid[i];
                    roomRequest.createTime = createTime[i];
                    roomRequest.attrs = attrs[i];
                    requestList.Add(roomRequest);
                }
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetRoomInviteList ]=========================//
        public bool IMLIB_GetRoomInviteList(Action<List<IMLIB_RoomInvite>, int> callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("imclient_getroominvitelist");

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                List<IMLIB_RoomInvite> inviteList = new List<IMLIB_RoomInvite>();
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> targetXid = GetLongList(answer, "targetXid");
                        List<long> createTime = GetLongList(answer, "createTime");
                        List<long> fromXid = GetLongList(answer, "fromXid");
                        List<string> attrs = GetStringList(answer, "attrs");
                        for (int i = 0; i < targetXid.Count && i < createTime.Count && i < fromXid.Count && i < attrs.Count; ++i)
                        {
                            IMLIB_RoomInvite roomInvite = new IMLIB_RoomInvite();
                            roomInvite.rid = targetXid[i];
                            roomInvite.createTime = createTime[i];
                            roomInvite.fromUid = fromXid[i];
                            roomInvite.attrs = attrs[i];
                            inviteList.Add(roomInvite);
                        }
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(inviteList, errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_GetRoomInviteList(out List<IMLIB_RoomInvite> inviteList, int timeout = 0)
        {
            inviteList = new List<IMLIB_RoomInvite>();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getroominvitelist");

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                List<long> targetXid = GetLongList(answer, "targetXid");
                List<long> createTime = GetLongList(answer, "createTime");
                List<long> fromXid = GetLongList(answer, "fromXid");
                List<string> attrs = GetStringList(answer, "attrs");
                for (int i = 0; i < targetXid.Count && i < createTime.Count && i < fromXid.Count && i < attrs.Count; ++i)
                {
                    IMLIB_RoomInvite roomInvite = new IMLIB_RoomInvite();
                    roomInvite.rid = targetXid[i];
                    roomInvite.createTime = createTime[i];
                    roomInvite.fromUid = fromXid[i];
                    roomInvite.attrs = attrs[i];
                    inviteList.Add(roomInvite);
                }
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }
    }
}
