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
        //===========================[ IMLIB_GetUserInfos ]=========================//
        public bool IMLIB_GetUserInfos(Action<List<IMLIB_UserInfo>, int> callback, HashSet<long> uids, int timeout = 0)
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

            Quest quest = new Quest("imclient_getuserinfos");
            quest.Param("uids", uids);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {

                List<IMLIB_UserInfo> infos = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        infos = new List<IMLIB_UserInfo>();
                        List<long> uids = GetLongList(answer, "uid");
                        List<string> names = GetStringList(answer, "name");
                        List<string> portraitUrls = GetStringList(answer, "portraitUrl");
                        List<string> profiles = GetStringList(answer, "profile");
                        List<string> attrs = GetStringList(answer, "attrs");
                        List<int> applyGrants = GetIntList(answer, "applyGrant");
                        for (int i = 0; i < uids.Count && i < names.Count && i < portraitUrls.Count && i < profiles.Count && i < attrs.Count && i < applyGrants.Count; ++i)
                        {
                            IMLIB_UserInfo info = new IMLIB_UserInfo();
                            info.uid = uids[i];
                            info.name = names[i];
                            info.portraitUrl = portraitUrls[i];
                            info.profile = profiles[i];
                            info.attrs = attrs[i];
                            info.applyGrant = (IMLIB_ApplyGrant)applyGrants[i];
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

        public int IMLIB_GetUserInfos(out List<IMLIB_UserInfo> infos, HashSet<long> uids, int timeout = 0)
        {
            infos = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getuserinfos");
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                infos = new List<IMLIB_UserInfo>();
                List<long> uid = GetLongList(answer, "uid");
                List<string> names = GetStringList(answer, "name");
                List<string> portraitUrls = GetStringList(answer, "portraitUrl");
                List<string> profiles = GetStringList(answer, "profile");
                List<string> attrs = GetStringList(answer, "attrs");
                List<int> applyGrants = GetIntList(answer, "applyGrant");
                for (int i = 0; i < uid.Count && i < names.Count && i < portraitUrls.Count && i < profiles.Count && i < attrs.Count && i < applyGrants.Count; ++i)
                {
                    IMLIB_UserInfo info = new IMLIB_UserInfo();
                    info.uid = uid[i];
                    info.name = names[i];
                    info.portraitUrl = portraitUrls[i];
                    info.profile = profiles[i];
                    info.attrs = attrs[i];
                    info.applyGrant = (IMLIB_ApplyGrant)applyGrants[i];
                    infos.Add(info);
                }
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ IMLIB_GetUserInfo ]=========================//
        public bool IMLIB_GetUserInfo(Action<IMLIB_UserInfo, int> callback, long uid, int timeout = 0)
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

            Quest quest = new Quest("imclient_getuserinfo");
            quest.Param("other_uid", uid);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) =>
            {
                IMLIB_UserInfo info = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        info = new IMLIB_UserInfo();
                        info.uid = uid;
                        info.name = answer.Want<string>("name");
                        info.portraitUrl = answer.Want<string>("portraitUrl");
                        info.profile = answer.Want<string>("profile");
                        info.attrs = answer.Want<string>("attrs");
                        info.applyGrant = (IMLIB_ApplyGrant)(answer.Want<int>("applyGrant"));
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

        public int IMLIB_GetUserInfo(out IMLIB_UserInfo info, long uid, int timeout = 0)
        {
            info = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("imclient_getuserinfo");
            quest.Param("other_uid", uid);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                info = new IMLIB_UserInfo();
                info.uid = uid;
                info.name = answer.Want<string>("name");
                info.portraitUrl = answer.Want<string>("portraitUrl");
                info.profile = answer.Want<string>("profile");
                info.attrs = answer.Want<string>("attrs");
                info.applyGrant = (IMLIB_ApplyGrant)(answer.Want<int>("applyGrant"));
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }


        
    }
}
