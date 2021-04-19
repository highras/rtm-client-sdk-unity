using System;
using System.Collections.Generic;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //===========================[ Add Group Members ]=========================//
        public bool AddGroupMembers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
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

            Quest quest = new Quest("addgroupmembers");
            quest.Param("gid", groupId);
            quest.Param("uids", uids);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int AddGroupMembers(long groupId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("addgroupmembers");
            quest.Param("gid", groupId);
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Delete Group Members ]=========================//
        public bool DeleteGroupMembers(DoneDelegate callback, long groupId, HashSet<long> uids, int timeout = 0)
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

            Quest quest = new Quest("delgroupmembers");
            quest.Param("gid", groupId);
            quest.Param("uids", uids);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int DeleteGroupMembers(long groupId, HashSet<long> uids, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("delgroupmembers");
            quest.Param("gid", groupId);
            quest.Param("uids", uids);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Get Group Members ]=========================//
        //-- Action<uids, errorCode>
        public bool GetGroupMembers(Action<HashSet<long>, int> callback, long groupId, int timeout = 0)
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

            Quest quest = new Quest("getgroupmembers");
            quest.Param("gid", groupId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                HashSet<long> uids = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        uids = WantLongHashSet(answer, "uids");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(uids, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        //-- Action<member_uids, online_uids, errorCode>
        public bool GetGroupMembers(Action<HashSet<long>, HashSet<long>, int> callback, long groupId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getgroupmembers");
            quest.Param("gid", groupId);
            quest.Param("online", true);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                HashSet<long> allUids = null;
                HashSet<long> onlineUids = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        allUids = WantLongHashSet(answer, "uids");
                        onlineUids = GetLongHashSet(answer, "onlines");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(allUids, onlineUids, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetGroupMembers(out HashSet<long> uids, long groupId, int timeout = 0)
        {
            uids = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupmembers");
            quest.Param("gid", groupId);
            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                uids = WantLongHashSet(answer, "uids");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        public int GetGroupMembers(out HashSet<long> allUids, out HashSet<long> onlineUids, long groupId, int timeout = 0)
        {
            allUids = null;
            onlineUids = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupmembers");
            quest.Param("gid", groupId);
            quest.Param("online", true);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                allUids = WantLongHashSet(answer, "uids");
                onlineUids = GetLongHashSet(answer, "onlines");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Get Group Count ]=========================//
        //-- Action<member_count, errorCode>
        public bool GetGroupCount(Action<int, int> callback, long groupId, int timeout = 0)
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

            Quest quest = new Quest("getgroupcount");
            quest.Param("gid", groupId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                int memberCount = 0;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        memberCount = answer.Want<int>("cn");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(memberCount, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        //-- Action<member_count, online_count, errorCode>
        public bool GetGroupCount(Action<int, int, int> callback, long groupId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, 0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getgroupcount");
            quest.Param("gid", groupId);
            quest.Param("online", true);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                int memberCount = 0;
                int onlineCount = 0;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        memberCount = answer.Want<int>("cn");
                        onlineCount = answer.Want<int>("online");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(memberCount, onlineCount, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, 0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetGroupCount(out int memberCount, long groupId, int timeout = 0)
        {
            memberCount = 0;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupcount");
            quest.Param("gid", groupId);
            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                memberCount = answer.Want<int>("cn");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        public int GetGroupCount(out int memberCount, out int onlineCount, long groupId, int timeout = 0)
        {
            memberCount = 0;
            onlineCount = 0;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupcount");
            quest.Param("gid", groupId);
            quest.Param("online", true);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                memberCount = answer.Want<int>("cn");
                onlineCount = answer.Want<int>("online");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Get User Groups ]=========================//
        //-- Action<groupIds, errorCode>
        public bool GetUserGroups(Action<HashSet<long>, int> callback, int timeout = 0)
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

            Quest quest = new Quest("getusergroups");
            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                HashSet<long> groupIds = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        groupIds = WantLongHashSet(answer, "gids");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(groupIds, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetUserGroups(out HashSet<long> groupIds, int timeout = 0)
        {
            groupIds = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getusergroups");
            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                groupIds = WantLongHashSet(answer, "gids");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Set Group Info ]=========================//
        public bool SetGroupInfo(DoneDelegate callback, long groupId, string publicInfo = null, string privateInfo = null, int timeout = 0)
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

            Quest quest = new Quest("setgroupinfo");
            quest.Param("gid", groupId);
            if (publicInfo != null)
                quest.Param("oinfo", publicInfo);
            if (privateInfo != null)
                quest.Param("pinfo", privateInfo);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int SetGroupInfo(long groupId, string publicInfo = null, string privateInfo = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("setgroupinfo");
            quest.Param("gid", groupId);
            if (publicInfo != null)
                quest.Param("oinfo", publicInfo);
            if (privateInfo != null)
                quest.Param("pinfo", privateInfo);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Get Group Info ]=========================//
        //-- Action<publicInfo, privateInfo, errorCode>
        public bool GetGroupInfo(Action<string, string, int> callback, long groupId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getgroupinfo");
            quest.Param("gid", groupId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                string publicInfo = "";
                string privateInfo = "";

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        publicInfo = answer.Want<string>("oinfo");
                        privateInfo = answer.Want<string>("pinfo");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(publicInfo, privateInfo, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetGroupInfo(out string publicInfo, out string privateInfo, long groupId, int timeout = 0)
        {
            publicInfo = null;
            privateInfo = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupinfo");
            quest.Param("gid", groupId);
            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                publicInfo = answer.Want<string>("oinfo");
                privateInfo = answer.Want<string>("pinfo");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Get Group Open Info ]=========================//
        //-- Action<public_info, errorCode>
        public bool GetGroupPublicInfo(Action<string, int> callback, long groupId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getgroupopeninfo");
            quest.Param("gid", groupId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                string publicInfo = "";
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    { publicInfo = answer.Want<string>("oinfo"); }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(publicInfo, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetGroupPublicInfo(out string publicInfo, long groupId, int timeout = 0)
        {
            publicInfo = "";

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupopeninfo");
            quest.Param("gid", groupId);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                publicInfo = answer.Want<string>("oinfo");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Get Groups Open Info ]=========================//
        //-- Action<Dictionary<string_groupId, public_info>, errorCode>
        [System.Obsolete("GetGroupsPublicInfo() with dictionary in string key type is deprecated, please using the overloaded function with dictionary in long key type instead.")]
        public bool GetGroupsPublicInfo(Action<Dictionary<string, string>, int> callback, HashSet<long> groupIds, int timeout = 0)
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

            Quest quest = new Quest("getgroupsopeninfo");
            quest.Param("gids", groupIds);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                Dictionary<string, string> publicInfos = null;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        publicInfos = WantStringDictionary(answer, "info");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(publicInfos, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        //-- Action<Dictionary<groupId, public_info>, errorCode>
        public bool GetGroupsPublicInfo(Action<Dictionary<long, string>, int> callback, HashSet<long> groupIds, int timeout = 0)
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

            Quest quest = new Quest("getgroupsopeninfo");
            quest.Param("gids", groupIds);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                Dictionary<long, string> publicInfos = null;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        publicInfos = WantLongStringDictionary(answer, "info");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(publicInfos, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        [System.Obsolete("GetGroupsPublicInfo() with dictionary in string key type is deprecated, please using the overloaded function with dictionary in long key type instead.")]
        public int GetGroupsPublicInfo(out Dictionary<string, string> publicInfos, HashSet<long> groupIds, int timeout = 0)
        {
            publicInfos = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupsopeninfo");
            quest.Param("gids", groupIds);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                publicInfos = WantStringDictionary(answer, "info");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        public int GetGroupsPublicInfo(out Dictionary<long, string> publicInfos, HashSet<long> groupIds, int timeout = 0)
        {
            publicInfos = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getgroupsopeninfo");
            quest.Param("gids", groupIds);

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                publicInfos = WantLongStringDictionary(answer, "info");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }
    }
}
