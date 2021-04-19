using System;
using System.Collections.Generic;
using System.Threading;
using com.fpnn.proto;

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
            public Dictionary<string, string> attr;
            public string lang;

            public int remainedTimeout;
            public long lastActionMsecTimeStamp;
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

            BuildRtmGateClient(endpoint);

            if (autoRelogin)
            {
                autoReloginInfo = new AutoReloginInfo();
                regressiveStrategy = RTMConfig.globalRegressiveStrategy;
            }
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
                            status = ClientStatus.Closed;
                    }

                    if (autoReloginInfo != null)
                    {
                        startRelogin = (autoReloginInfo.disabled == false && autoReloginInfo.canRelogin);
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

        //-------------[ Auth(Login) processing functions ]--------------------------//
        private void AuthFinish(bool authStatus, int errorCode)
        {
            AuthStatusInfo currInfo;
            long currUid;
            bool isRelogin = false;
            Int64 reservedRtmGateConnectionId = 0;

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
                    //-- Reserving rtmGate without closing for quick relogin.
                }

                if (autoReloginInfo != null)
                {
                    isRelogin = autoReloginInfo.canRelogin;

                    if (authStatsInfo != null)
                    {
                        autoReloginInfo.token = authStatsInfo.token;
                        autoReloginInfo.attr = authStatsInfo.attr;
                        autoReloginInfo.lang = authStatsInfo.lang;
                    }

                    if (authStatus && !autoReloginInfo.canRelogin)
                        autoReloginInfo.LoginSuccessful();
                }

                currInfo = authStatsInfo;
                authStatsInfo = null;
                currUid = uid;

                syncConnectingEvent.Set();
            }

            if (reservedRtmGateConnectionId != 0)
                RTMControlCenter.UnregisterSession(reservedRtmGateConnectionId);

            if (currInfo != null)
                foreach (AuthDelegate callback in currInfo.authDelegates)
                    callback(projectId, currUid, authStatus, errorCode);

            if (authStatus)
                processor.BeginCheckPingInterval();
        }

        private bool AdjustAuthRemainedTimeout()
        {
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
            return Login(callback, token, null, "", timeout);
        }

        public bool Login(AuthDelegate callback, string token, Dictionary<string, string> attr, TranslateLanguage language = TranslateLanguage.None, int timeout = 0)
        {
            return Login(callback, token, attr, GetTranslatedLanguage(language), timeout);
        }

        private bool Login(AuthDelegate callback, string token, Dictionary<string, string> attr, string lang = "", int timeout = 0)
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
            }

            authStatsInfo = new AuthStatusInfo
            {
                authDelegates = new HashSet<AuthDelegate>() { callback },
                remainedTimeout = timeout,
            };

            authStatsInfo.token = token;
            authStatsInfo.attr = attr;
            authStatsInfo.lang = lang;
            authStatsInfo.lastActionMsecTimeStamp = ClientEngine.GetCurrentMilliseconds();

            if (rtmGate.IsConnected())
            {
                if (authStatsInfo.remainedTimeout == 0)
                    authStatsInfo.remainedTimeout = RTMConfig.globalQuestTimeoutSeconds;

                RTMControlCenter.RegisterSession(rtmGateConnectionId, this);
                Auth(false);
            }
            else
            {
                if (authStatsInfo.remainedTimeout == 0)
                    authStatsInfo.remainedTimeout = ((ConnectTimeout == 0) ? RTMConfig.globalConnectTimeoutSeconds : ConnectTimeout)
                        + ((QuestTimeout == 0) ? RTMConfig.globalQuestTimeoutSeconds : QuestTimeout);

                rtmGate.AsyncConnect();
            }

            return true;
        }

        public int Login(out bool ok, string token, int timeout = 0)
        {
            return Login(out ok, token, null, "", timeout);
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
            return Login(out ok, token, attr, GetTranslatedLanguage(language), timeout);
        }

        private int Login(out bool ok, string token, Dictionary<string, string> attr, string lang = "", int timeout = 0)
        {
            SyncLoginStatus syncLoginStatus = new SyncLoginStatus();
            bool actionBegin = Login((long projectId, long uid, bool authStatus, int errorCode) => {
                syncLoginStatus.ok = authStatus;
                syncLoginStatus.errorCode = errorCode;
                syncLoginStatus.Set();
            }, token, attr, lang, timeout);
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
            if (autoReloginInfo.reloginCount <= regressiveStrategy.startConnectFailedCount)
            {
                StartRelogin();
                return;
            }

            int regressiveCount = autoReloginInfo.reloginCount - regressiveStrategy.startConnectFailedCount;
            long interval = regressiveStrategy.maxIntervalSeconds * 1000;
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
            autoReloginInfo.token, autoReloginInfo.attr, autoReloginInfo.lang);

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
                if (status == ClientStatus.Closed)
                    return;

                requireClose = true;

                if (disableRelogin && autoReloginInfo != null)
                    autoReloginInfo.Disable();

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
    }
}
