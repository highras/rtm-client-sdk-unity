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
        //===========================[ IMLIB_SetUserInfo ]=========================//
        public bool IMLIB_SetUserInfo(DoneDelegate callback, string name = null, string portraitUrl = null, string profile = null, string attrs = null, IMLIB_ApplyGrant applyGrant = IMLIB_ApplyGrant.NONE, int timeout = 0)
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

            Quest quest = new Quest("imclient_setuserinfos");
            if (name != null)
                quest.Param("name", name);
            if (portraitUrl != null)
                quest.Param("portraitUrl", portraitUrl);
            if (profile != null)
                quest.Param("profile", profile);
            if (attrs != null)
                quest.Param("attrs", attrs);
            if (applyGrant != IMLIB_ApplyGrant.NONE)
                quest.Param("applyGrant", (int)applyGrant);

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

        public int IMLIB_SetUserInfo(string name = null, string portraitUrl = null, string profile = null, string attrs = null, IMLIB_ApplyGrant applyGrant = IMLIB_ApplyGrant.NONE, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_setuserinfos");
            if (name != null)
                quest.Param("name", name);
            if (portraitUrl != null)
                quest.Param("portraitUrl", portraitUrl);
            if (profile != null)
                quest.Param("profile", profile);
            if (attrs != null)
                quest.Param("attrs", attrs);
            if (applyGrant != IMLIB_ApplyGrant.NONE)
                quest.Param("applyGrant", (int)applyGrant);

            Answer answer = client.SendQuest(quest, timeout);

            return answer.ErrorCode();
        }

        //===========================[ IMLIB_CreateGroup ]=========================//
        public bool IMLIB_CreateGroup(DoneDelegate callback, long groupId, IMLIB_ApplyGrant applyGrant = IMLIB_ApplyGrant.NONE, IMLIB_InviteGrant inviteGrant = IMLIB_InviteGrant.NONE, int timeout = 0)
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

            Quest quest = new Quest("imclient_creategroup");
            quest.Param("gid", groupId);
            if (applyGrant != IMLIB_ApplyGrant.NONE)
                quest.Param("applyGrant", (int)applyGrant);
            if (inviteGrant != IMLIB_InviteGrant.NONE)
                quest.Param("inviteGrant", (int)inviteGrant);

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

        public int IMLIB_CreateGroup(long groupId, IMLIB_ApplyGrant applyGrant = IMLIB_ApplyGrant.NONE, IMLIB_InviteGrant inviteGrant = IMLIB_InviteGrant.NONE, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_creategroup");
            quest.Param("gid", groupId);
            if (applyGrant != IMLIB_ApplyGrant.NONE)
                quest.Param("applyGrant", (int)applyGrant);
            if (inviteGrant != IMLIB_InviteGrant.NONE)
                quest.Param("inviteGrant", (int)inviteGrant);

            Answer answer = client.SendQuest(quest, timeout);

            return answer.ErrorCode();
        }

        //===========================[ IMLIB_SetGroupInfo ]=========================//
        public bool IMLIB_SetGroupInfo(DoneDelegate callback, long groupId, string name = null, string portraitUrl = null, string profile = null, string attrs = null, IMLIB_ApplyGrant applyGrant = IMLIB_ApplyGrant.NONE, IMLIB_InviteGrant inviteGrant = IMLIB_InviteGrant.NONE, int timeout = 0)
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

            Quest quest = new Quest("imclient_setgroupinfos");
            quest.Param("gid", groupId);
            if (name != null)
                quest.Param("name", name);
            if (portraitUrl != null)
                quest.Param("portraitUrl", portraitUrl);
            if (profile != null)
                quest.Param("profile", profile);
            if (attrs != null)
                quest.Param("attrs", attrs);
            if (applyGrant != IMLIB_ApplyGrant.NONE)
                quest.Param("applyGrant", (int)applyGrant);
            if (inviteGrant != IMLIB_InviteGrant.NONE)
                quest.Param("inviteGrant", (int)inviteGrant);

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

        public int IMLIB_SetGroupInfo(long groupId, string name = null, string portraitUrl = null, string profile = null, string attrs = null, IMLIB_ApplyGrant applyGrant = IMLIB_ApplyGrant.NONE, IMLIB_InviteGrant inviteGrant = IMLIB_InviteGrant.NONE, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_setgroupinfos");
            quest.Param("gid", groupId);
            if (name != null)
                quest.Param("name", name);
            if (portraitUrl != null)
                quest.Param("portraitUrl", portraitUrl);
            if (profile != null)
                quest.Param("profile", profile);
            if (attrs != null)
                quest.Param("attrs", attrs);
            if (applyGrant != IMLIB_ApplyGrant.NONE)
                quest.Param("applyGrant", (int)applyGrant);
            if (inviteGrant != IMLIB_InviteGrant.NONE)
                quest.Param("inviteGrant", (int)inviteGrant);

            Answer answer = client.SendQuest(quest, timeout);

            return answer.ErrorCode();
        }
    }
}

