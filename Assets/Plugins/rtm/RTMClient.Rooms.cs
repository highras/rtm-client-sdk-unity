﻿using System;
using System.Collections.Generic;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //===========================[ Enter Room ]=========================//
        public bool EnterRoom(DoneDelegate callback, long roomId, int timeout = 0)
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

            Quest quest = new Quest("enterroom");
            quest.Param("rid", roomId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int EnterRoom(long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("enterroom");
            quest.Param("rid", roomId);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Leave Room ]=========================//
        public bool LeaveRoom(DoneDelegate callback, long roomId, int timeout = 0)
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

            Quest quest = new Quest("leaveroom");
            quest.Param("rid", roomId);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int LeaveRoom(long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("leaveroom");
            quest.Param("rid", roomId);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Get User Rooms ]=========================//
        //-- Action<roomIds, errorCode>
        public bool GetUserRooms(Action<HashSet<long>, int> callback, int timeout = 0)
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

            Quest quest = new Quest("getuserrooms");
            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                HashSet<long> roomIds = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        roomIds = WantLongHashSet(answer, "rooms");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(roomIds, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetUserRooms(out HashSet<long> roomIds, int timeout = 0)
        {
            roomIds = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getuserrooms");
            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                roomIds = WantLongHashSet(answer, "rooms");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Set Room Info ]=========================//
        public bool SetRoomInfo(DoneDelegate callback, long roomId, string publicInfo = null, string privateInfo = null, int timeout = 0)
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

            Quest quest = new Quest("setroominfo");
            quest.Param("rid", roomId);
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

        public int SetRoomInfo(long roomId, string publicInfo = null, string privateInfo = null, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("setroominfo");
            quest.Param("rid", roomId);
            if (publicInfo != null)
                quest.Param("oinfo", publicInfo);
            if (privateInfo != null)
                quest.Param("pinfo", privateInfo);

            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Get Room Info ]=========================//
        //-- Action<publicInfo, privateInfo, errorCode>
        public bool GetRoomInfo(Action<string, string, int> callback, long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback("", "", fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getroominfo");
            quest.Param("rid", roomId);

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
                    callback("", "", fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetRoomInfo(out string publicInfo, out string privateInfo, long roomId, int timeout = 0)
        {
            publicInfo = null;
            privateInfo = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getroominfo");
            quest.Param("rid", roomId);
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

        //===========================[ Get Room Open Info ]=========================//
        //-- Action<public_info, errorCode>
        public bool GetRoomPublicInfo(Action<string, int> callback, long roomId, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback("", fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getroomopeninfo");
            quest.Param("rid", roomId);

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
                    callback("", fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetRoomPublicInfo(out string publicInfo, long roomId, int timeout = 0)
        {
            publicInfo = "";

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getroomopeninfo");
            quest.Param("rid", roomId);

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

        //===========================[ Get Rooms Open Info ]=========================//
        //-- Action<Dictionary<string_roomId, public_info>, errorCode>
        public bool GetRoomsPublicInfo(Action<Dictionary<string, string>, int> callback, HashSet<long> roomIds, int timeout = 0)
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

            Quest quest = new Quest("getroomsopeninfo");
            quest.Param("rids", roomIds);

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

        public int GetRoomsPublicInfo(out Dictionary<string, string> publicInfos, HashSet<long> roomIds, int timeout = 0)
        {
            publicInfos = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getroomsopeninfo");
            quest.Param("rids", roomIds);

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
    }
}
