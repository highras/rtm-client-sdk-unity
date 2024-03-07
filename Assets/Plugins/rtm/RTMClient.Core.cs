using System;
using System.Collections.Generic;
using System.Threading;
using com.fpnn.proto;
using UnityEngine;
using Random = UnityEngine.Random;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        public enum ClientStatus
        {
            Closed,
            Connecting,
            Connected
        }

        //-------------[ Private class for Login ]--------------------------//

        private class AutoReloginInfo
        {
            public bool disabled = true;
            public bool canRelogin = false;
            public int reloginCount = 0;
            public int lastErrorCode = 0;
            public long lastReloginMS = 0;

            public string token;
            public long ts;
            public Dictionary<string, string> attr;
            public string lang;

            public void Login()
            {
                if (disabled)
                {
                    disabled = false;
                    reloginCount = 0;
                    lastErrorCode = 0;
                }
                else if (canRelogin)
                {
                    reloginCount += 1;
                }

                lastReloginMS = ClientEngine.GetCurrentMilliseconds();
            }

            public void LoginSuccessful()
            {
                canRelogin = true;
                reloginCount = 0;
                lastErrorCode = 0;
            }

            public void Disable()
            {
                disabled = true;
                canRelogin = false;
            }
        }

        private class AuthStatusInfo
        {
            public HashSet<AuthDelegate> authDelegates;
            public string token;
            public long ts;
            public Dictionary<string, string> attr;
            public string lang;

            public int remainedTimeout;
            public long lastActionMsecTimeStamp;
        }

        private class LoginRetryInfo
        {
            public string backupEndpoint;
            public bool usingBackupEndpoint;
            public List<string> gameIpList;
            public int gameIpIndex;
            public int gameIpRetriedCount;
        }

        //-------------[ Static Fields ]--------------------------//
        private static readonly HashSet<int> reloginStopCodes = new HashSet<int>
        {
            fpnn.ErrorCode.FPNN_EC_CORE_FORBIDDEN,

            ErrorCode.RTM_EC_INVALID_PID_OR_UID,
            ErrorCode.RTM_EC_INVALID_PID_OR_SIGN,
            ErrorCode.RTM_EC_PERMISSION_DENIED,
            ErrorCode.RTM_EC_AUTH_DENIED,
            ErrorCode.RTM_EC_ADMIN_LOGIN,
            ErrorCode.RTM_EC_ADMIN_ONLY,
            ErrorCode.RTM_EC_INVALID_AUTH_TOEKN,
            ErrorCode.RTM_EC_BLOCKED_USER,
        };

        //-------------[ Fields ]--------------------------//
        private object interLocker;
        private static object instanceLocker = new object();
        private readonly long projectId;
        private readonly long uid;

        private ClientStatus status;
        private volatile bool requireClose;
        private ManualResetEvent syncConnectingEvent;

        public volatile int ConnectTimeout;
        public volatile int QuestTimeout;

        private IRTMMasterProcessor processor;
        private TCPClient rtmGate;
        private Int64 rtmGateConnectionId;

        private AuthStatusInfo authStatsInfo;
        private AutoReloginInfo autoReloginInfo;
        private RegressiveStrategy regressiveStrategy;
        private common.ErrorRecorder errorRecorder;
        private LoginRetryInfo loginRetryInfo;

        public RTMClient(string endpoint, long projectId, long uid, RTMQuestProcessor serverPushProcessor, bool autoRelogin = true)
        {
            interLocker = new object();
            this.projectId = projectId;
            this.uid = uid;
            status = ClientStatus.Closed;
            requireClose = false;
            syncConnectingEvent = new ManualResetEvent(false);

            ConnectTimeout = 0;
            QuestTimeout = 0;

            RTMMasterProcessor processorCurrent = new RTMMasterProcessor();
            processorCurrent.SetProcessor(serverPushProcessor);
            processor = processorCurrent;

            errorRecorder = RTMConfig.errorRecorder;
            if (errorRecorder != null)
                processor.SetErrorRecorder(errorRecorder);

            loginRetryInfo = new LoginRetryInfo();
            BuildRtmGateClient(endpoint);

            if (autoRelogin)
            {
                autoReloginInfo = new AutoReloginInfo();
                regressiveStrategy = RTMConfig.globalRegressiveStrategy;
            }
        }

        private void reset(RTMQuestProcessor serverPushProcessor, bool autoRelogin)
        {
            lock (interLocker)
            {
                (processor as RTMMasterProcessor).SetProcessor(serverPushProcessor);
                if (errorRecorder != null)
                    processor.SetErrorRecorder(errorRecorder);

                if (autoRelogin)
                {
                    autoReloginInfo = new AutoReloginInfo();
                    regressiveStrategy = RTMConfig.globalRegressiveStrategy;
                }
                else
                {
                    autoReloginInfo = null;
                    regressiveStrategy = null;
                }

                loginRetryInfo = new LoginRetryInfo();
            }
        }

        public static RTMClient getInstance(string endpoint, long projectId, long uid, RTMQuestProcessor serverPushProcessor, bool autoRelogin = true)
        {
            RTMClient client = null;
            lock (instanceLocker)
            {
                client = RTMControlCenter.FetchClient(projectId, uid);
                if (client != null)
                {
                    client.reset(serverPushProcessor, autoRelogin);
                }
                else
                {
                    client = new RTMClient(endpoint, projectId, uid, serverPushProcessor, autoRelogin);
                    RTMControlCenter.AddClient(projectId, uid, client);
                }
            }
            return client;
        }
        

        //-------------[ Fack Fields ]--------------------------//

        //-- Obsolete in v.2.2.0
        [Obsolete("Property Pid is deprecated, please use ProjectId instead.")]
        public long Pid
        {
            get
            {
                return projectId;
            }
        }

        public long ProjectId
        {
            get
            {
                return projectId;
            }
        }

        public long Uid
        {
            get
            {
                return uid;
            }
        }

        public ClientStatus Status
        {
            get
            {
                lock (interLocker)
                    return status;
            }
        }

        public common.ErrorRecorder ErrorRecorder
        {
            set
            {
                lock (interLocker)
                {
                    errorRecorder = value;
                    processor.SetErrorRecorder(errorRecorder);
                    rtmGate.SetErrorRecorder(errorRecorder);
                }
            }
        }

        public bool ConnectionIsAlive()
        {
            return processor.ConnectionIsAlive();
        }

        public void SetRegressiveStrategy(RegressiveStrategy strategy)
        {
            regressiveStrategy = strategy;
        }

        private TCPClient GetCoreClient()
        {
            lock (interLocker)
            {
                if (status == ClientStatus.Connected)
                    return rtmGate;
                else
                    return null;
            }
        }

        //-------------[ Init Functions ]--------------------------//
        private string adjustEndpoint(string originalEndpoint)
        {
            int idx = originalEndpoint.LastIndexOf(':');
            if (idx < 1)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Invalid RTM client endpoint: " + originalEndpoint);

                return originalEndpoint;
            }

            string portString = originalEndpoint.Substring(idx + 1);
            int port = Convert.ToInt32(portString, 10);
            if (port == 13321)
                return originalEndpoint;

            if (port == 13325)
                return originalEndpoint.Substring(0, idx) + ":" + 13321;

            if (errorRecorder != null)
                errorRecorder.RecordError("Invalid RTM client endpoint: " + originalEndpoint + " (invalid port)");

            return originalEndpoint;
        }

        private void BuildRtmGateClient(string originalEndpoint)
        {
            rtmGate = TCPClient.Create(adjustEndpoint(originalEndpoint), true);

            if (errorRecorder != null)
                rtmGate.SetErrorRecorder(errorRecorder);

            rtmGate.SetQuestProcessor(processor);
            rtmGate.SetConnectionConnectedDelegate((Int64 connectionId, string endpoint, bool connected) => {
                if (requireClose)
                {
                    AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED);
                    return;
                }

                if (connected)
                {
                    rtmGateConnectionId = connectionId;
                    RTMControlCenter.RegisterSession(rtmGateConnectionId, this);
                    Auth(true);
                }
                else
                {
                    AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                }
            });

            rtmGate.SetConnectionCloseDelegate((Int64 connectionId, string endpoint, bool causedByError) => {

                bool trigger = false;
                bool isConnecting = false;
                bool startRelogin = false;
                lock (interLocker)
                {
                    trigger = rtmGateConnectionId == connectionId;
                    if (trigger)
                    {
                        if (status == ClientStatus.Connecting)
                            isConnecting = true;
                        else
                        { 
                            status = ClientStatus.Closed;
                            rtmGateConnectionId = 0;
                        }
                    }

                    if (autoReloginInfo != null)
                    {
                        startRelogin = CheckRelogin();
                        autoReloginInfo.lastErrorCode = (causedByError ? fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED : fpnn.ErrorCode.FPNN_EC_OK);
                    }
                }

                if (trigger)
                {
                    if (isConnecting)
                        AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    else
                    {
                        if (startRelogin)
                            StartRelogin();
                        else
                            processor.SessionClosed(causedByError ? fpnn.ErrorCode.FPNN_EC_CORE_UNKNOWN_ERROR : fpnn.ErrorCode.FPNN_EC_OK);
                    }
                }
            });
        }

        public void SetBackupEndpoint(string endpoint)
        {
            lock (interLocker)
            {
                loginRetryInfo.backupEndpoint = endpoint;
            }
        }

        public bool CheckRelogin()
        {
            return autoReloginInfo.disabled == false && autoReloginInfo.canRelogin && RTMControlCenter.NetworkStatus != NetworkType.NetworkType_Unreachable;
        }

        //-------------[ Auth(Login) processing functions ]--------------------------//
        private void AuthFinish(bool authStatus, int errorCode)
        {
            AuthStatusInfo currInfo;
            long currUid;
            bool isRelogin = false;
            Int64 reservedRtmGateConnectionId = 0;
            bool needClose = false;

            lock (interLocker)
            {
                if (status != ClientStatus.Connecting)
                    return;

                if (authStatus)
                {
                    status = ClientStatus.Connected;
                }
                else
                {
                    status = ClientStatus.Closed;
                    reservedRtmGateConnectionId = rtmGateConnectionId;
                    rtmGateConnectionId = 0;
                    needClose = true;
                    //-- Reserving rtmGate without closing for quick relogin.
                }

                if (autoReloginInfo != null)
                {
                    isRelogin = autoReloginInfo.canRelogin;

                    if (authStatsInfo != null)
                    {
                        autoReloginInfo.token = authStatsInfo.token;
                        autoReloginInfo.ts = authStatsInfo.ts;
                        autoReloginInfo.attr = authStatsInfo.attr;
                        autoReloginInfo.lang = authStatsInfo.lang;
                    }

                    if (authStatus && !autoReloginInfo.canRelogin)
                        autoReloginInfo.LoginSuccessful();
                }

                currInfo = authStatsInfo;
                authStatsInfo = null;
                currUid = uid;

                if (needClose)
                    rtmGate.Close();

                syncConnectingEvent.Set();
            }

            if (reservedRtmGateConnectionId != 0)
                RTMControlCenter.UnregisterSession(reservedRtmGateConnectionId);

            if (currInfo != null)
            {
                bool finish = true;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION || errorCode == fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED)
                {
                    finish = ReloginWithBackupEndpoint(currInfo);
                    if (finish)
                        finish = ReloginWithIP(currInfo);
                }

                if (finish)
                {
                    foreach (AuthDelegate callback in currInfo.authDelegates)
                        callback(projectId, currUid, authStatus, errorCode);                    
                }
            }

            if (authStatus)
                processor.BeginCheckPingInterval();
        }

        private bool ReloginWithBackupEndpoint(AuthStatusInfo info)
        {
            lock (interLocker)
            {
                if (loginRetryInfo.backupEndpoint == null || loginRetryInfo.usingBackupEndpoint)
                    return true;
                loginRetryInfo.usingBackupEndpoint = true;
                BuildRtmGateClient(loginRetryInfo.backupEndpoint);                
            }
            Login((long projectId, long uid, bool successful, int errorCode) =>
            {
                if (info != null)
                {
                    foreach (AuthDelegate callback in info.authDelegates)
                        callback(projectId, uid, successful, errorCode);                    
                }

            }, info.token, info.attr, info.ts, info.lang, info.remainedTimeout);
            return false;
        }

        private bool ReloginWithIP(AuthStatusInfo info)
        {
            if (info.ts != 0 || RTMConfig.reloginWithIP == false)
                return true;
            lock (interLocker)
            {
                if (loginRetryInfo.gameIpList == null)
                {
                    loginRetryInfo.gameIpList = GetIPFromToken(info.token);
                    System.Random random = new System.Random();
                    loginRetryInfo.gameIpIndex = random.Next(0, loginRetryInfo.gameIpList.Count);
                }
                else
                {
                    loginRetryInfo.gameIpIndex += 1;
                    loginRetryInfo.gameIpIndex %= loginRetryInfo.gameIpList.Count;
                }

                if (loginRetryInfo.gameIpRetriedCount >= loginRetryInfo.gameIpList.Count)
                    return true;

                loginRetryInfo.gameIpRetriedCount += 1;
                BuildRtmGateClient(loginRetryInfo.gameIpList[loginRetryInfo.gameIpIndex]+":13321");
            }

            Login((long projectId, long uid, bool successful, int errorCode) =>
            {
                if (info != null)
                {
                    foreach (AuthDelegate callback in info.authDelegates)
                        callback(projectId, uid, successful, errorCode);                    
                }

            }, info.token, info.attr, info.ts, info.lang, info.remainedTimeout);
             
            return false;
        }

        private bool AdjustAuthRemainedTimeout()
        {
            if (authStatsInfo == null)
                return false;
            long curr = ClientEngine.GetCurrentMilliseconds();
            int passSeconds = (int)(curr - authStatsInfo.lastActionMsecTimeStamp) / 1000;
            authStatsInfo.lastActionMsecTimeStamp = curr;
            authStatsInfo.remainedTimeout -= passSeconds;
            if (authStatsInfo.remainedTimeout <= 0)
            {
                AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_TIMEOUT);
                return false;
            }

            return true;
        }

        private void Auth(bool checkRemainedTimeout)
        {
            if (checkRemainedTimeout && !AdjustAuthRemainedTimeout())
                return;

            processor.SetConnectionId(rtmGateConnectionId);
            
            Quest quest = new Quest("auth");
            quest.Param("pid", projectId);
            quest.Param("uid", uid);
            quest.Param("token", authStatsInfo.token);
            if (authStatsInfo.ts != 0)
            { 
                quest.Param("ts", authStatsInfo.ts);
                quest.Param("authv", 2);
            }


#if UNITY_2017_1_OR_NEWER
            quest.Param("version", "Unity-" + RTMConfig.SDKVersion);
#else
            quest.Param("version", "C#-" + RTMConfig.SDKVersion);
#endif

            if (authStatsInfo.lang.Length > 0)
                quest.Param("lang", authStatsInfo.lang);

            if (authStatsInfo.attr != null && authStatsInfo.attr.Count > 0)
                quest.Param("attrs", authStatsInfo.attr);

            int timeout = authStatsInfo.remainedTimeout;

            bool status = rtmGate.SendQuest(quest, (Answer answer, int errorCode) => {
                if (requireClose)
                {
                    AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED);
                    return;
                }

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    bool ok = answer.Get<bool>("ok", false);
                    AuthFinish(ok, fpnn.ErrorCode.FPNN_EC_OK);
                }
                else
                {
                    AuthFinish(false, errorCode);
                }
            }, timeout);
            if (!status)
                AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
        }

        //-------------[ Login & System interfaces ]--------------------------//
        public bool Login(AuthDelegate callback, string token, int timeout = 0)
        {
            return Login(callback, token, null, 0, "", timeout);
        }

        public bool Login(AuthDelegate callback, string token, long ts, int timeout = 0)
        {
            return Login(callback, token, null, ts, "", timeout);
        }

        public bool Login(AuthDelegate callback, string token, Dictionary<string, string> attr, TranslateLanguage language = TranslateLanguage.None, int timeout = 0)
        {
            return Login(callback, token, attr, 0, GetTranslatedLanguage(language), timeout);
        }

        public bool Login(AuthDelegate callback, string token, long ts, Dictionary<string, string> attr, TranslateLanguage language = TranslateLanguage.None, int timeout = 0)
        {
            return Login(callback, token, attr, ts, GetTranslatedLanguage(language), timeout);
        }

        private bool Login(AuthDelegate callback, string token, Dictionary<string, string> attr, long ts = 0, string lang = "", int timeout = 0)
        {
            lock (interLocker)
            {
                if (status == ClientStatus.Connected)
                {
                    ClientEngine.RunTask(() =>
                    {
                        callback(projectId, uid, true, fpnn.ErrorCode.FPNN_EC_OK);
                    });

                    return true;
                }

                if (status == ClientStatus.Connecting)
                {
                    authStatsInfo.authDelegates.Add(callback);
                    return true;
                }

                status = ClientStatus.Connecting;
                syncConnectingEvent.Reset();

                requireClose = false;

                if (autoReloginInfo != null)
                    autoReloginInfo.Login();
                authStatsInfo = new AuthStatusInfo
                {
                    authDelegates = new HashSet<AuthDelegate>() { callback },
                    remainedTimeout = timeout,
                };

                authStatsInfo.token = token;
                authStatsInfo.ts = ts;
                authStatsInfo.attr = attr;
                authStatsInfo.lang = lang;
                authStatsInfo.lastActionMsecTimeStamp = ClientEngine.GetCurrentMilliseconds();
                if (authStatsInfo.remainedTimeout == 0)
                    authStatsInfo.remainedTimeout = ((ConnectTimeout == 0) ? RTMConfig.globalConnectTimeoutSeconds : ConnectTimeout)
                        + ((QuestTimeout == 0) ? RTMConfig.globalQuestTimeoutSeconds : QuestTimeout);
            }

            rtmGate.AsyncConnect();

            return true;
        }

        public int Login(out bool ok, string token, int timeout = 0)
        {
            return Login(out ok, token, null, 0, "", timeout);
        }

        public int Login(out bool ok, string token, long ts, int timeout = 0)
        {
            return Login(out ok, token, null, ts, "", timeout);
        }

        private class SyncLoginStatus
        {
            private ManualResetEvent syncWaiter;
            public bool ok;
            public int errorCode;

            public SyncLoginStatus()
            {
                syncWaiter = new ManualResetEvent(false);
                syncWaiter.Reset();
            }

            public void Set()
            {
                syncWaiter.Set();
            }

            public void Wait()
            {
                syncWaiter.WaitOne();
            }
        }

        public int Login(out bool ok, string token, Dictionary<string, string> attr, TranslateLanguage language = TranslateLanguage.None, int timeout = 0)
        {
            return Login(out ok, token, attr, 0, GetTranslatedLanguage(language), timeout);
        }

        public int Login(out bool ok, string token, long ts, Dictionary<string, string> attr, TranslateLanguage language = TranslateLanguage.None, int timeout = 0)
        {
            return Login(out ok, token, attr, ts, GetTranslatedLanguage(language), timeout);
        }

        private int Login(out bool ok, string token, Dictionary<string, string> attr, long ts = 0, string lang = "", int timeout = 0)
        {
            SyncLoginStatus syncLoginStatus = new SyncLoginStatus();
            bool actionBegin = Login((long projectId, long uid, bool authStatus, int errorCode) => {
                syncLoginStatus.ok = authStatus;
                syncLoginStatus.errorCode = errorCode;
                syncLoginStatus.Set();
            }, token, attr, ts, lang, timeout);
            if (!actionBegin)
            {
                lock (interLocker)
                {
                    if (status == ClientStatus.Connected)
                    {
                        ok = true;
                        return fpnn.ErrorCode.FPNN_EC_OK;
                    }

                    if (status == ClientStatus.Closed)
                    {
                        ok = false;
                        return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
                    }
                }
            }

            syncLoginStatus.Wait();

            lock (interLocker)
            {
                ok = syncLoginStatus.ok;
                return syncLoginStatus.errorCode;
            }
        }

        //-------------[ Relogin interfaces ]--------------------------//
        private void StartNextRelogin()
        {
            int regressiveCount = autoReloginInfo.reloginCount;
            long interval = regressiveStrategy.maxIntervalSeconds * 1000;
            if (regressiveCount > regressiveStrategy.maxRegressvieCount)
            { 
                processor.SessionClosed(autoReloginInfo.lastErrorCode);
                return;
            }
            if (regressiveCount < regressiveStrategy.linearRegressiveCount)
            {
                interval = interval * regressiveCount / regressiveStrategy.linearRegressiveCount;
            }
            RTMControlCenter.DelayRelogin(this, ClientEngine.GetCurrentMilliseconds() + interval);
        }

        internal void StartRelogin()
        {
            bool launch = processor.ReloginWillStart(autoReloginInfo.lastErrorCode, autoReloginInfo.reloginCount);
            if (!launch)
            {
                processor.SessionClosed(autoReloginInfo.lastErrorCode);
                return;
            }

            bool startLogin = Login((long projectId, long uid, bool successful, int errorCode) =>
            {
                if (successful)
                {
                    processor.ReloginCompleted(true, false, errorCode, autoReloginInfo.reloginCount);
                    autoReloginInfo.LoginSuccessful();
                    return;
                }
                else
                {
                    bool connected = false;
                    lock (interLocker)
                    {
                        if (status == ClientStatus.Connected)
                            connected = true;
                    }

                    if (connected || errorCode == ErrorCode.RTM_EC_DUPLCATED_AUTH)
                    {
                        processor.ReloginCompleted(true, false, fpnn.ErrorCode.FPNN_EC_OK, autoReloginInfo.reloginCount);
                        autoReloginInfo.LoginSuccessful();
                        return;
                    }

                    if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                    {
                        processor.ReloginCompleted(false, false, ErrorCode.RTM_EC_INVALID_AUTH_TOEKN, autoReloginInfo.reloginCount);
                        autoReloginInfo.Disable();
                        processor.SessionClosed(ErrorCode.RTM_EC_INVALID_AUTH_TOEKN);
                        return;
                    }

                    bool stopRetry = reloginStopCodes.Contains(errorCode);

                    processor.ReloginCompleted(false, !stopRetry, errorCode, autoReloginInfo.reloginCount);
                    if (stopRetry)
                    {
                        autoReloginInfo.Disable();
                        processor.SessionClosed(errorCode);
                        return;
                    }
                    else
                        autoReloginInfo.lastErrorCode = errorCode;

                    StartNextRelogin();
                }
            },
            autoReloginInfo.token, autoReloginInfo.attr, autoReloginInfo.ts, autoReloginInfo.lang);

            if (!startLogin && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse == false)
            {
                ClientStatus connStatus;
                lock (interLocker)
                {
                    connStatus = status;
                }

                if (connStatus == ClientStatus.Connected)
                {
                    processor.ReloginCompleted(true, false, fpnn.ErrorCode.FPNN_EC_OK, autoReloginInfo.reloginCount);
                    autoReloginInfo.LoginSuccessful();
                    return;
                }
                else
                {
                    int errorCode = fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED;
                    if (connStatus == ClientStatus.Connecting)
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_UNKNOWN_ERROR;

                    processor.ReloginCompleted(false, true, errorCode, autoReloginInfo.reloginCount);
                    autoReloginInfo.lastErrorCode = errorCode;
                    StartNextRelogin();
                }
            }
        }

        //-------------[ Close interfaces ]--------------------------//
        internal void Close(bool disableRelogin, bool waitConnectingCannelled)
        {
            bool isConnecting = false;

            lock (interLocker)
            {
                if (disableRelogin && autoReloginInfo != null)
                    autoReloginInfo.Disable();

                if (status == ClientStatus.Closed)
                    return;

                requireClose = true;

                if (status == ClientStatus.Connecting)
                {
                    isConnecting = true;
                }
                else
                {
                    status = ClientStatus.Closed;
                }
            }

            rtmGate.Close();

            if (isConnecting && waitConnectingCannelled)
                syncConnectingEvent.WaitOne();
        }

        public void Close(bool waitConnectingCannelled = true)
        {
            Close(true, waitConnectingCannelled);
        }

        public bool GetServerTime(Action<long, int> callback, int timeout = 0)
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

            Quest quest = new Quest("getservertime");

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                long msec = 0;
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    { msec = answer.Get<long>("mts", 0); }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(msec, errorCode);
            }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int GetServerTime(out long msec, int timeout = 0)
        {
            msec = 0;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getservertime");

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                msec = answer.Get<long>("mts", 0);
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }
    }
}
