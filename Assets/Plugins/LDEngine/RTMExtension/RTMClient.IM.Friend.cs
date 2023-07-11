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
        //===========================[ IMLIB_AddFriend ]=========================//
        public bool IMLIB_AddFriend(DoneDelegate callback, long uid, string extra = null, string attrs = null, int timeout = 0)
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

            Quest quest = new Quest("imclient_addfriend");
            quest.Param("ouid", uid);
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

        public int IMLIB_AddFriend(long uid, string extra = null, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_addfriend");
            quest.Param("ouid", uid);
            if (extra != null)
                quest.Param("extra", extra);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_AckAddFriend ]=========================//
        public bool IMLIB_AckAddFriend(DoneDelegate callback, long uid, bool agree, string attrs = null, int timeout = 0)
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

            Quest quest = new Quest("imclient_ackaddfriend");
            quest.Param("ouid", uid);
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

        public int IMLIB_AckAddFriend(long uid, bool agree, string attrs = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_ackaddfriend");
            quest.Param("ouid", uid);
            quest.Param("agree", agree);
            if (attrs != null)
                quest.Param("attrs", attrs);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_SetFriendAlias ]=========================//
        public bool IMLIB_SetFriendAlias(DoneDelegate callback, long uid, string alias, int timeout = 0)
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

            Quest quest = new Quest("imclient_setfriendalias");
            quest.Param("target_uid", uid);
            quest.Param("alias", alias);

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

        public int IMLIB_SetFriendAlias(long uid, string alias, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_setfriendalias");
            quest.Param("target_uid", uid);
            quest.Param("alias", alias);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ IMLIB_GetFriendAlias ]=========================//
        public bool IMLIB_GetFriendAlias(Action<string, int> callback, long uid, int timeout = 0)
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

            Quest quest = new Quest("imclient_getfriendalias");
            quest.Param("target_uid", uid);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                string val = null;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        val = answer.Want<string>("val");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(val, errorCode);
            }, timeout);


            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int IMLIB_GetFriendAlias(out string alias, long uid, int timeout = 0)
        {
            alias = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getfriendalias");
            quest.Param("target_uid", uid);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                alias = answer.Want<string>("val");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetFriendApplyList ]=========================//
        public bool IMLIB_GetFriendApplyList(Action<List<IMLIB_FriendApply>, int> callback, int timeout = 0)
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

            Quest quest = new Quest("imclient_getfriendapplylist");

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                List<IMLIB_FriendApply> applyList = new List<IMLIB_FriendApply>();
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> fromUid = GetLongList(answer, "fromUid");
                        List<int> status = GetIntList(answer, "status");
                        List<string> grantExtra = GetStringList(answer, "grantExtra");
                        List<long> createTime = GetLongList(answer, "createTime");
                        List<string> attrs = GetStringList(answer, "attrs");
                        for (int i = 0; i < fromUid.Count && i < status.Count && i < grantExtra.Count && i < createTime.Count && i < attrs.Count; ++i)
                        {
                            IMLIB_FriendApply friendApply = new IMLIB_FriendApply();
                            friendApply.uid = fromUid[i];
                            friendApply.status = status[i];
                            friendApply.extra = grantExtra[i];
                            friendApply.createTime = createTime[i];
                            friendApply.attrs = attrs[i];
                            applyList.Add(friendApply);
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

        public int IMLIB_GetFriendApplyList(out List<IMLIB_FriendApply> applyList, int timeout = 0)
        {
            applyList = new List<IMLIB_FriendApply>();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getfriendapplylist");

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                List<long> fromUid = GetLongList(answer, "fromUid");
                List<int> status = GetIntList(answer, "status");
                List<string> grantExtra = GetStringList(answer, "grantExtra");
                List<long> createTime = GetLongList(answer, "createTime");
                List<string> attrs = GetStringList(answer, "attrs");
                for (int i = 0; i < fromUid.Count && i < status.Count && i < grantExtra.Count && i < createTime.Count && i < attrs.Count; ++i)
                {
                    IMLIB_FriendApply friendApply = new IMLIB_FriendApply();
                    friendApply.uid = fromUid[i];
                    friendApply.status = status[i];
                    friendApply.extra = grantExtra[i];
                    friendApply.createTime = createTime[i];
                    friendApply.attrs = attrs[i];
                    applyList.Add(friendApply);
                }
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetFriendRequestList ]=========================//
        public bool IMLIB_GetFriendRequestList(Action<List<IMLIB_FriendRequest>, int> callback, int timeout = 0)
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

            Quest quest = new Quest("imclient_getfriendrequestlist");

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                List<IMLIB_FriendRequest> requestList = new List<IMLIB_FriendRequest>();
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        List<long> targetXid = GetLongList(answer, "targetXid");
                        List<long> createTime = GetLongList(answer, "createTime");
                        List<string> attrs = GetStringList(answer, "attrs");
                        for (int i = 0; i < targetXid.Count && i < createTime.Count && i < attrs.Count; ++i)
                        {
                            IMLIB_FriendRequest friendRequest = new IMLIB_FriendRequest();
                            friendRequest.uid = targetXid[i];
                            friendRequest.createTime = createTime[i];
                            friendRequest.attrs = attrs[i];
                            requestList.Add(friendRequest);
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

        public int IMLIB_GetFriendRequestList(out List<IMLIB_FriendRequest> requestList, int timeout = 0)
        {
            requestList = new List<IMLIB_FriendRequest>();

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getfriendrequestlist");

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
                    IMLIB_FriendRequest friendRequest = new IMLIB_FriendRequest();
                    friendRequest.uid = targetXid[i];
                    friendRequest.createTime = createTime[i];
                    friendRequest.attrs = attrs[i];
                    requestList.Add(friendRequest);
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
