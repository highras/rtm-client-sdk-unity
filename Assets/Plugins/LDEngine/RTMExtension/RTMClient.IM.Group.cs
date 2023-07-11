using UnityEngine;
using System.Collections;
using com.fpnn.livedata;
using System.Collections.Generic;
using System;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //===========================[ IMLIB_GetGroupInfos ]=========================//
        public bool IMLIB_GetGroupInfos(Action<List<IMLIB_GroupInfo>, int> callback, HashSet<long> groupIds, int timeout = 0)
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

            Quest quest = new Quest("imclient_getgroupinfos");
            quest.Param("gids", groupIds);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                List<IMLIB_GroupInfo> infos = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        infos = new List<IMLIB_GroupInfo>();
                        List<long> xid = GetLongList(answer, "xid");
                        List<long> ownerUids = GetLongList(answer, "ownerUid");
                        List<string> names = GetStringList(answer, "name");
                        List<string> portraitUrls = GetStringList(answer, "portraitUrl");
                        List<string> profiles = GetStringList(answer, "profile");
                        List<string> attrs = GetStringList(answer, "attrs");
                        List<int> applyGrants = GetIntList(answer, "applyGrant");
                        List<int> inviteGrants = GetIntList(answer, "inviteGrant");
                        List<List<long>> managerUids = GetLongListList(answer, "managerUids");
                        for (int i = 0; i < xid.Count && i < ownerUids.Count && i < names.Count && i < portraitUrls.Count && i < profiles.Count && i < attrs.Count && i < applyGrants.Count && i < inviteGrants.Count && i < managerUids.Count; ++i)
                        {
                            IMLIB_GroupInfo info = new IMLIB_GroupInfo();
                            info.groupId = xid[i];
                            info.ownerUid = ownerUids[i];
                            info.name = names[i];
                            info.portraitUrl = portraitUrls[i];
                            info.profile = profiles[i];
                            info.attrs = attrs[i];
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

        public int IMLIB_GetGroupInfos(out List<IMLIB_GroupInfo> infos, HashSet<long> groupIds, int timeout = 0)
        {
            infos = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getgroupinfos");
            quest.Param("gids", groupIds);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                infos = new List<IMLIB_GroupInfo>();
                List<long> xid = GetLongList(answer, "xid");
                List<long> ownerUids = GetLongList(answer, "ownerUid");
                List<string> names = GetStringList(answer, "name");
                List<string> portraitUrls = GetStringList(answer, "portraitUrl");
                List<string> profiles = GetStringList(answer, "profile");
                List<string> attrs = GetStringList(answer, "attrs");
                List<int> applyGrants = GetIntList(answer, "applyGrant");
                List<int> inviteGrants = GetIntList(answer, "inviteGrant");
                List<List<long>> managerUids = GetLongListList(answer, "managerUids");
                for (int i = 0; i < xid.Count && i < ownerUids.Count && i < names.Count && i < portraitUrls.Count && i < profiles.Count && i < attrs.Count && i < applyGrants.Count && i < inviteGrants.Count && i < managerUids.Count; ++i)
                {
                    IMLIB_GroupInfo info = new IMLIB_GroupInfo();
                    info.groupId = xid[i];
                    info.ownerUid = ownerUids[i];
                    info.name = names[i];
                    info.portraitUrl = portraitUrls[i];
                    info.profile = profiles[i];
                    info.attrs = attrs[i];
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

        //===========================[ IMLIB_GetGroupInfo ]=========================//
        public bool IMLIB_GetGroupInfo(Action<IMLIB_GroupInfo, int> callback, long groupId, int timeout = 0)
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

            Quest quest = new Quest("imclient_getgroupinfo");
            quest.Param("gid", groupId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                IMLIB_GroupInfo info = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        info = new IMLIB_GroupInfo();
                        info.groupId = groupId;
                        info.name = answer.Want<string>("name");
                        info.ownerUid = answer.Want<long>("ownerUid");
                        info.portraitUrl = answer.Want<string>("portraitUrl");
                        info.profile = answer.Want<string>("profile");
                        info.attrs = answer.Want<string>("attrs");
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

        public int IMLIB_GetGroupInfo(out IMLIB_GroupInfo info, long groupId, int timeout = 0)
        {
            info = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getgroupinfo");
            quest.Param("gid", groupId);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                info = new IMLIB_GroupInfo();
                info.groupId = groupId;
                info.name = answer.Want<string>("name");
                info.ownerUid = answer.Want<long>("ownerUid");
                info.portraitUrl = answer.Want<string>("portraitUrl");
                info.profile = answer.Want<string>("profile");
                info.attrs = answer.Want<string>("attrs");
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

        //===========================[ IMLIB_JoinGroup ]=========================//
        public bool IMLIB_JoinGroup(DoneDelegate callback, long groupId, string extra = null, string attrs = null, int timeout = 0)
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

            Quest quest = new Quest("imclient_joingroup");
            quest.Param("gid", groupId);
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

        public int IMLIB_JoinGroup(long groupId, string extra = null, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_joingroup");
            quest.Param("gid", groupId);
            if (extra != null)
                quest.Param("extra", extra);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_AckJoinGroup ]=========================//
        public bool IMLIB_AckJoinGroup(DoneDelegate callback, long groupId, long fromUid, bool agree, string attrs = null, int timeout = 0)
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

            Quest quest = new Quest("imclient_ackjoingroup");
            quest.Param("gid", groupId);
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

        public int IMLIB_AckJoinGroup(long groupId, long fromUid, bool agree, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_ackjoingroup");
            quest.Param("gid", groupId);
            quest.Param("from", fromUid);
            quest.Param("agree", agree);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_InviteIntoGroup ]=========================//
        public bool IMLIB_InviteIntoGroup(DoneDelegate callback, long groupId, HashSet<long> uids, string extra = null, string attrs = null ,int timeout = 0)
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

            Quest quest = new Quest("imclient_inviteintogroup");
            quest.Param("gid", groupId);
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

        public int IMLIB_InviteIntoGroup(long groupId, HashSet<long> uids, string extra = null, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_inviteintogroup");
            quest.Param("gid", groupId);
            quest.Param("target_uids", uids);
            if (extra != null)
                quest.Param("extra", extra);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_AckInviteIntoGroup ]=========================//
        public bool IMLIB_AckInviteIntoGroup(DoneDelegate callback, long groupId, long fromUid, bool agree, string attrs = null, int timeout = 0)
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

            Quest quest = new Quest("imclient_ackinviteintogroup");
            quest.Param("gid", groupId);
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

        public int IMLIB_AckInviteIntoGroup(long groupId, long fromUid, bool agree, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_ackinviteintogroup");
            quest.Param("gid", groupId);
            quest.Param("from", fromUid);
            quest.Param("agree", agree);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_DismissGroup ]=========================//
        public bool IMLIB_DismissGroup(DoneDelegate callback, long groupId, int timeout = 0)
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

            Quest quest = new Quest("imclient_dismissgroup");
            quest.Param("gid", groupId);

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

        public int IMLIB_DismissGroup(long groupId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_dismissgroup");
            quest.Param("gid", groupId);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_RemoveGroupMembers ]=========================//
        public bool IMLIB_RemoveGroupMembers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
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

            Quest quest = new Quest("imclient_removegroupmembers");
            quest.Param("gid", groupId);
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

        public int IMLIB_RemoveGroupMembers(long groupId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_removegroupmembers");
            quest.Param("gid", groupId);
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_LeaveGroup ]=========================//
        public bool IMLIB_LeaveGroup(DoneDelegate callback, long groupId, int timeout = 0)
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

            Quest quest = new Quest("imclient_leavegroup");
            quest.Param("gid", groupId);

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

        public int IMLIB_LeaveGroup(long groupId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_leavegroup");
            quest.Param("gid", groupId);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_TransferGroup ]=========================//
        public bool IMLIB_TransferGroup(DoneDelegate callback, long groupId, long uid, int timeout = 0)
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

            Quest quest = new Quest("imclient_transfergroup");
            quest.Param("gid", groupId);
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

        public int IMLIB_TransferGroup(long groupId, long uid, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_transfergroup");
            quest.Param("gid", groupId);
            quest.Param("to_uid", uid);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_AddGroupManagers ]=========================//
        public bool IMLIB_AddGroupManagers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
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

            Quest quest = new Quest("imclient_addgroupmanagers");
            quest.Param("gid", groupId);
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

        public int IMLIB_AddGroupManagers(long groupId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_addgroupmanagers");
            quest.Param("gid", groupId);
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_RemoveGroupManagers ]=========================//
        public bool IMLIB_RemoveGroupManagers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
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

            Quest quest = new Quest("imclient_removegroupmanagers");
            quest.Param("gid", groupId);
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

        public int IMLIB_RemoveGroupManagers(long groupId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_removegroupmanagers");
            quest.Param("gid", groupId);
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_AddGroupMemberBan ]=========================//
        public bool IMLIB_AddGroupMemberBan(DoneDelegate callback, long groupId, long uid, int banTime, int timeout = 0)
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

            Quest quest = new Quest("imclient_addgroupmemberban");
            quest.Param("gid", groupId);
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

        public int IMLIB_AddGroupMemberBan(long groupId, long uid, int banTime, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_addgroupmemberban");
            quest.Param("gid", groupId);
            quest.Param("to_uid", uid);
            quest.Param("ban_time", banTime);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_RemoveGroupMemberBan ]=========================//
        public bool IMLIB_RemoveGroupMemberBan(DoneDelegate callback, long groupId, long uid, int timeout = 0)
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

            Quest quest = new Quest("imclient_removegroupmemberban");
            quest.Param("gid", groupId);
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

        public int IMLIB_RemoveGroupMemberBan(long groupId, long uid, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_removegroupmemberban");
            quest.Param("gid", groupId);
            quest.Param("to_uid", uid);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_GetGroupRoomNick ]=========================//
        public bool IMLIB_GetGroupRoomNick(Action<Dictionary<string, string>, int> callback, int type, long xid, HashSet<long> uids, int timeout = 0)
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

            Quest quest = new Quest("imclient_getgrouproomnick");
            quest.Param("type", type);
            quest.Param("xid", xid);
            quest.Param("uids", uids);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        data = WantStringDictionary(answer, "data");
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

        public int IMLIB_GetGroupRoomNick(out Dictionary<string, string> data, int type, long xid, HashSet<long> uids, int timeout = 0)
        {
            data = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getgrouproomnick");
            quest.Param("type", type);
            quest.Param("xid", xid);
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                data = WantStringDictionary(answer, "data");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetGroupMembers ]=========================//
        public bool IMLIB_GetGroupMembers(Action<List<Dictionary<string, string>>, int> callback, long groupId, int timeout = 0)
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

            Quest quest = new Quest("imclient_getgroupmembers");
            quest.Param("gid", groupId);

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

        public int IMLIB_GetGroupMembers(out List<Dictionary<string, string>> data, long groupId, int timeout = 0)
        {
            data = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getgroupmembers");
            quest.Param("gid", groupId);

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

        //===========================[ IMLIB_GetGroupMemberCount ]=========================//
        public bool IMLIB_GetGroupMemberCount(Action<int, int> callback, long groupId, int timeout = 0)
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

            Quest quest = new Quest("imclient_getgroupmembercount");
            quest.Param("gid", groupId);

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

        public int IMLIB_GetGroupMemberCount(out int count, long groupId, int timeout = 0)
        { 
            count = -1;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getgroupmembercount");
            quest.Param("gid", groupId);

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

        //===========================[ IMLIB_GetGroupApplyList ]=========================//
        public bool IMLIB_GetGroupApplyList(Action<List<IMLIB_GroupApply>, int> callback, long groupId, int timeout = 0)
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

            Quest quest = new Quest("imclient_getgroupapplylist");
            quest.Param("gid", groupId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                List<IMLIB_GroupApply> applyList = new List<IMLIB_GroupApply>();
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
                            IMLIB_GroupApply groupApply = new IMLIB_GroupApply();
                            groupApply.uid = fromUid[i];
                            groupApply.status = status[i];
                            groupApply.extra = grantExtra[i];
                            groupApply.createTime = createTime[i];
                            groupApply.inviteUid = inviteUid[i];
                            groupApply.attrs = attrs[i];
                            applyList.Add(groupApply);
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

        public int IMLIB_GetGroupApplyList(out List<IMLIB_GroupApply> applyList, long groupId, int timeout = 0)
        {
            applyList = new List<IMLIB_GroupApply>();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getgroupapplylist");
            quest.Param("gid", groupId); ;

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
                    IMLIB_GroupApply groupApply = new IMLIB_GroupApply();
                    groupApply.uid = fromUid[i];
                    groupApply.status = status[i];
                    groupApply.extra = grantExtra[i];
                    groupApply.createTime = createTime[i];
                    groupApply.inviteUid = inviteUid[i];
                    groupApply.attrs = attrs[i];
                    applyList.Add(groupApply);
                }
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetGroupRequestList ]=========================//
        public bool IMLIB_GetGroupRequestList(Action<List<IMLIB_GroupRequest>, int> callback, int timeout = 0)
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

            Quest quest = new Quest("imclient_getgrouprequestlist");

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                List<IMLIB_GroupRequest> requestList = new List<IMLIB_GroupRequest>();
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> targetXid = GetLongList(answer, "targetXid");
                        List<long> createTime = GetLongList(answer, "createTime");
                        List<string> attrs = GetStringList(answer, "attrs");
                        for (int i = 0; i < targetXid.Count && i < createTime.Count && i < attrs.Count; ++i)
                        {
                            IMLIB_GroupRequest groupRequest = new IMLIB_GroupRequest();
                            groupRequest.gid = targetXid[i];
                            groupRequest.createTime = createTime[i];
                            groupRequest.attrs = attrs[i];
                            requestList.Add(groupRequest);
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

        public int IMLIB_GetGroupRequestList(out List<IMLIB_GroupRequest> requestList, int timeout = 0)
        {
            requestList = new List<IMLIB_GroupRequest>();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getgrouprequestlist");

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
                    IMLIB_GroupRequest groupRequest = new IMLIB_GroupRequest();
                    groupRequest.gid = targetXid[i];
                    groupRequest.createTime = createTime[i];
                    groupRequest.attrs = attrs[i];
                    requestList.Add(groupRequest);
                }
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetGroupInviteList ]=========================//
        public bool IMLIB_GetGroupInviteList(Action<List<IMLIB_GroupInvite>, int> callback, int timeout = 0)
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

            Quest quest = new Quest("imclient_getgroupinvitelist");

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                List<IMLIB_GroupInvite> inviteList = new List<IMLIB_GroupInvite>();
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
                            IMLIB_GroupInvite groupInvite = new IMLIB_GroupInvite();
                            groupInvite.gid = targetXid[i];
                            groupInvite.createTime = createTime[i];
                            groupInvite.fromUid = fromXid[i];
                            groupInvite.attrs = attrs[i];
                            inviteList.Add(groupInvite);
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

        public int IMLIB_GetGroupInviteList(out List<IMLIB_GroupInvite> inviteList, int timeout = 0)
        {
            inviteList = new List<IMLIB_GroupInvite>();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getgroupinvitelist");

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
                    IMLIB_GroupInvite groupInvite = new IMLIB_GroupInvite();
                    groupInvite.gid = targetXid[i];
                    groupInvite.createTime = createTime[i];
                    groupInvite.fromUid = fromXid[i];
                    groupInvite.attrs = attrs[i];
                    inviteList.Add(groupInvite);
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
