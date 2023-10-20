﻿using System;
namespace com.fpnn.rtm
{
    public class RegressiveStrategy
    {
        public int maxIntervalSeconds = 8;                     //-- 退行性重连最大时间间隔
        public int linearRegressiveCount = 4;                  //-- 从第一次退行性连接开始，到最大链接时间，允许尝试几次连接，每次时间间隔都会增大
        public int maxRegressvieCount = 10;                     //-- 退行性重连最大次数，超出该次数则不再进行重连
    }

    public class RTMConfig
    {
        public static readonly string SDKVersion = "2.7.20";
        public static readonly string InterfaceVersion = "2.7.3";
        public static readonly string RTMGameObjectName = "RTM_GAMEOBJECT";

        internal static int lostConnectionAfterLastPingInSeconds = 60;
        internal static int globalConnectTimeoutSeconds = 30;
        internal static int globalQuestTimeoutSeconds = 30;
        internal static int fileGateClientHoldingSeconds = 150;
        internal static common.ErrorRecorder errorRecorder = null;
        internal static bool triggerCallbackIfAsyncMethodReturnFalse = false;
        internal static RegressiveStrategy globalRegressiveStrategy = new RegressiveStrategy();

        public int maxPingInterval;
        public int globalConnectTimeout;
        public int globalQuestTimeout;
        public int fileClientHoldingSeconds;
        public common.ErrorRecorder defaultErrorRecorder;
        public bool forceTriggerCallbackWhenAsyncMethodReturnFalse;
        public long adminID;
        public RegressiveStrategy regressiveStrategy;

        public RTMConfig()
        {
            maxPingInterval = 60;
            globalConnectTimeout = 30;
            globalQuestTimeout = 30;
            fileClientHoldingSeconds = 150;
            forceTriggerCallbackWhenAsyncMethodReturnFalse = false;
            adminID = 111;

            regressiveStrategy = new RegressiveStrategy();
        }

        internal static void Config(RTMConfig config)
        {
            lostConnectionAfterLastPingInSeconds = config.maxPingInterval;
            globalConnectTimeoutSeconds = config.globalConnectTimeout;
            globalQuestTimeoutSeconds = config.globalQuestTimeout;
            fileGateClientHoldingSeconds = config.fileClientHoldingSeconds;
            errorRecorder = config.defaultErrorRecorder;
            triggerCallbackIfAsyncMethodReturnFalse = config.forceTriggerCallbackWhenAsyncMethodReturnFalse;

            globalRegressiveStrategy = config.regressiveStrategy;
        }
    }

    public enum TranslateLanguage
    {
        ar,             //阿拉伯语
        nl,             //荷兰语
        en,             //英语
        fr,             //法语
        de,             //德语
        el,             //希腊语
        id,             //印度尼西亚语
        it,             //意大利语
        ja,             //日语
        ko,             //韩语
        no,             //挪威语
        pl,             //波兰语
        pt,             //葡萄牙语
        ru,             //俄语
        es,             //西班牙语
        sv,             //瑞典语
        tl,             //塔加路语（菲律宾语）
        th,             //泰语
        tr,             //土耳其语
        vi,             //越南语
        zh_cn,       //中文（简体）
        zh_tw,       //中文（繁体）
        None
    }

    public enum MessageType : byte
    {
        Withdraw = 1,
        GEO = 2,
        SystemNotification = 6,
        MultiLogin = 7,
        Chat = 30,
        Cmd = 32,
        RealAudio = 35,
        RealVideo = 36,
        ImageFile = 40,
        AudioFile = 41,
        VideoFile = 42,
        VoiceFile = 43,
        NormalFile = 50
    }

    public enum MessageCategory : byte
    {
        P2PMessage = 1,
        GroupMessage = 2,
        RoomMessage = 3,
        BroadcastMessage = 4
    }

    public enum IMLIB_MessageType
    { 
        AddFriendApply = 1,
        RefuseFriendApply = 2,
        EnterGroupApply = 3,
        RefuseEnterGroupApply = 4,
        InvitedIntoGroup = 5,
        RefuseInvitedIntoGroup = 7,
        FriendChanged = 15,
        GroupChanged = 17,
        GroupMemberChanged = 19,
        RoomMemberChanged = 20,
        AcceptFriendApply = 21,
        AcceptEnterGroupApply = 22,
        AcceptInvitedIntoGroup = 24,
        AddGroupManagers = 26,
        RemoveGroupManagers = 27,
        GroupOwnerChanged = 28,
        AddRoomManagers = 29,
        RemoveRoomManagers = 30,
        RoomOwnerChanged = 31,
    }
}
