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

        private class AuthStatusInfo
        {
            public AuthDelegate authCallback;
            public string token;
            public Dictionary<string, string> attr;
            public string lang;

            public bool usingDefaultQuestTimeout;
            public int remainedTimeout;
            public long lastActionMsecTimeStamp;

            //-- Unity IOS used.
            public int parallelConnectingCount;
            public bool parallelCompleted;
            public HashSet<TCPClient> rtmClients;
        }

        private class ParallelLoginStatusInfo
        {
            public int remainedTimeout;
            public long lastActionMsecTimeStamp;

            public static ParallelLoginStatusInfo Clone(AuthStatusInfo info)
            {
                ParallelLoginStatusInfo rev = new ParallelLoginStatusInfo();
                rev.remainedTimeout = info.remainedTimeout;
                rev.lastActionMsecTimeStamp = info.lastActionMsecTimeStamp;
                return rev;
            }

            public ParallelLoginStatusInfo Clone()
            {
                ParallelLoginStatusInfo rev = new ParallelLoginStatusInfo();
                rev.remainedTimeout = remainedTimeout;
                rev.lastActionMsecTimeStamp = lastActionMsecTimeStamp;
                return rev;
            }
        }

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
        private TCPClient dispatch;
        private TCPClient rtmGate;
        private Int64 rtmGateConnectionId;
        private Int64 reservedRtmGateConnectionId;

        private AuthStatusInfo authStatsInfo;
        private common.ErrorRecorder errorRecorder;

        //-- Obsolete in v.2.2.0
        [Obsolete("Constructor with interface IRTMQuestProcessor is deprecated, please use Constructor with class RTMQuestProcessor instead.")]
        public RTMClient(string endpoint, long projectId, long uid, IRTMQuestProcessor serverPushProcessor)
        {
            interLocker = new object();
            this.projectId = projectId;
            this.uid = uid;
            status = ClientStatus.Closed;
            requireClose = false;
            syncConnectingEvent = new ManualResetEvent(false);

            ConnectTimeout = 0;
            QuestTimeout = 0;

            RTMMasterProcessorV1 processorV1 = new RTMMasterProcessorV1();
            processorV1.SetProcessor(serverPushProcessor);
            processor = processorV1;

            dispatch = TCPClient.Create(endpoint, true);
            errorRecorder = RTMConfig.errorRecorder;
            if (errorRecorder != null)
            {
                processor.SetErrorRecorder(errorRecorder);
                dispatch.SetErrorRecorder(errorRecorder);
            }
        }

        public RTMClient(string endpoint, long projectId, long uid, RTMQuestProcessor serverPushProcessor)
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

            dispatch = TCPClient.Create(endpoint, true);
            errorRecorder = RTMConfig.errorRecorder;
            if (errorRecorder != null)
            {
                processor.SetErrorRecorder(errorRecorder);
                dispatch.SetErrorRecorder(errorRecorder);
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
                    dispatch.SetErrorRecorder(errorRecorder);

                    if (rtmGate != null)
                        rtmGate.SetErrorRecorder(errorRecorder);
                }
            }
        }

        public bool ConnectionIsAlive()
        {
            return processor.ConnectionIsAlive();
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

        //-------------[ Auth(Login) utilies functions ]--------------------------//
        private string ConvertIPv4ToIPv6(string ipv4)
        {
            string[] parts = ipv4.Split(new Char[] { '.' });
            if (parts.Length != 4)
                return string.Empty;

            foreach (string part in parts)
            {
                int partInt = Int32.Parse(part);
                if (partInt > 255 || partInt < 0)
                    return string.Empty;
            }

            string part7 = Convert.ToString(Int32.Parse(parts[0]) * 256 + Int32.Parse(parts[1]), 16);
            string part8 = Convert.ToString(Int32.Parse(parts[2]) * 256 + Int32.Parse(parts[3]), 16);
            return "64:ff9b::" + part7 + ":" + part8;
        }

        private bool ConvertIPv4EndpointToIPv6IPPort(string ipv4endpoint, out string ipv6, out int port)
        {
            int idx = ipv4endpoint.LastIndexOf(':');
            if (idx == -1)
            {
                ipv6 = string.Empty;
                port = 0;

                return false;
            }

            string ipv4 = ipv4endpoint.Substring(0, idx);
            string portString = ipv4endpoint.Substring(idx + 1);
            port = Convert.ToInt32(portString, 10);

            ipv6 = ConvertIPv4ToIPv6(ipv4);
            if (ipv6.Length == 0)
                return false;

            return true;
        }

        //-------------[ Auth(Login) processing functions ]--------------------------//
        private bool AsyncFetchRtmGateEndpoint(string addressType, AnswerDelegate callback, int timeout)
        {
            Quest quest = new Quest("which");
            quest.Param("what", "rtmGated");
            quest.Param("addrType", addressType);
            quest.Param("proto", "tcp");

            return dispatch.SendQuest(quest, callback, timeout);
        }

        private void AuthFinish(bool authStatus, int errorCode)
        {
            AuthStatusInfo currInfo;
            long currUid;

            lock (interLocker)
            {
                if (status != ClientStatus.Connecting)
                    return;

                if (authStatus)
                {
                    status = ClientStatus.Connected;
                    authStatsInfo.rtmClients.Remove(rtmGate);
                }
                else
                {
                    status = ClientStatus.Closed;
                    reservedRtmGateConnectionId = rtmGateConnectionId;
                    rtmGateConnectionId = 0;
                    //-- Reserving rtmGate without closing for quick relogin.
                }

                currInfo = authStatsInfo;
                authStatsInfo = null;
                currUid = uid;

                syncConnectingEvent.Set();
            }

            if (currInfo != null)
                currInfo.authCallback(projectId, currUid, authStatus, errorCode);

            if (reservedRtmGateConnectionId != 0)
                RTMControlCenter.UnregisterSession(reservedRtmGateConnectionId);
        }

        private bool AdjustAuthRemainedTimeout()
        {
            long curr = ClientEngine.GetCurrentMilliseconds();
            int passSeconds = (int)(curr - authStatsInfo.lastActionMsecTimeStamp) / 1000;
            authStatsInfo.lastActionMsecTimeStamp = curr;
            authStatsInfo.remainedTimeout -= passSeconds;
            if (authStatsInfo.remainedTimeout <= 0)
            {
                AuthFinish(false, com.fpnn.ErrorCode.FPNN_EC_CORE_TIMEOUT);
                return false;
            }

            return true;
        }

        private void ConfigRtmGateClient(TCPClient client, ConnectionConnectedDelegate ccd, int timeout)
        {
            client.ConnectTimeout = timeout;
            client.QuestTimeout = RTMConfig.globalQuestTimeoutSeconds;

            if (errorRecorder != null)
                client.SetErrorRecorder(errorRecorder);

            client.SetQuestProcessor(processor);
            client.SetConnectionConnectedDelegate(ccd);
            client.SetConnectionCloseDelegate((Int64 connectionId, string endpoint, bool causedByError) => {

                bool trigger = false;
                bool isConnecting = false;
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
                }

                if (trigger)
                {
                    if (isConnecting)
                        AuthFinish(false, com.fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    else
                        processor.SessionClosed(causedByError ? com.fpnn.ErrorCode.FPNN_EC_CORE_UNKNOWN_ERROR : com.fpnn.ErrorCode.FPNN_EC_OK);
                }
            });
        }

        private void DispatchCallBack_Which_IPv4(Answer answer, int errorCode)
        {
            if (requireClose)
            {
                AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED);
                return;
            }

            if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
            {
                if (!AdjustAuthRemainedTimeout())
                    return;

                string ipv4endpoint = answer.Get<string>("endpoint", string.Empty);
                if (ipv4endpoint.Length > 0)
                {
                    TCPClient client = TCPClient.Create(ipv4endpoint, false);
                    lock (interLocker)
                    {
                        rtmGate = client;
                        authStatsInfo.rtmClients.Add(client);
                    }
                    ConfigRtmGateClient(client, (Int64 connectionId, string endpoint, bool connected) => {
                        if (requireClose)
                        {
                            AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED);
                            return;
                        }

                        if (connected)
                        {
                            rtmGateConnectionId = connectionId;
                            RTMControlCenter.RegisterSession(rtmGateConnectionId, this);
                            Auth();
                        }
                        else
                        {
                            if (!AdjustAuthRemainedTimeout())
                                return;

                            StartParallelConnect(ipv4endpoint);
                        }
                    }, authStatsInfo.remainedTimeout);
                    client.AsyncConnect();

                    if (requireClose)
                        client.Close();
                    return;
                }
                else
                {
                    StartParallelConnect(string.Empty);
                }
            }
            else
                AuthFinish(false, errorCode);
        }

        private void Auth(bool checkRemainedTimeout = true)
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
            if (authStatsInfo.usingDefaultQuestTimeout && RTMConfig.globalQuestTimeoutSeconds < timeout)
                timeout = RTMConfig.globalQuestTimeoutSeconds;

            bool status = rtmGate.SendQuest(quest, (Answer answer, int errorCode) => {
                if (requireClose)
                {
                    AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED);
                    return;
                }

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    if (answer.Get<bool>("ok", false))
                    {
                        AuthFinish(true, fpnn.ErrorCode.FPNN_EC_OK);
                    }
                    else
                    {
                        string endpoint = answer.Get<string>("gate", "");
                        if (endpoint.Length == 0)
                        {
                            if (errorRecorder != null)
                                errorRecorder.RecordError("RtmGated auth return ok = false, but gate is empty. Token maybe expired.");

                            AuthFinish(false, fpnn.ErrorCode.FPNN_EC_OK);
                            return;
                        }
                        else
                        {
                            if (!AdjustAuthRemainedTimeout())
                                return;

                            RedirectToNewRtmGate(endpoint, false);
                        }
                    }
                }
                else
                {
                    AuthFinish(false, errorCode);
                }
            }, timeout);
            if (!status)
                AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
        }

        private void RedirectToNewRtmGate(string endpoint, bool addressConverted)
        {
            TCPClient client = TCPClient.Create(endpoint, false);
            lock (interLocker)
            {
                rtmGate = client;
                authStatsInfo.rtmClients.Add(client);
            }
            ConfigRtmGateClient(client, (Int64 connectionId, string rtmGateEndpoint, bool connected) => {
                if (requireClose)
                {
                    AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED);
                    return;
                }

                if (connected)
                {
                    rtmGateConnectionId = connectionId;
                    RTMControlCenter.RegisterSession(rtmGateConnectionId, this);
                    Auth();
                }
                else
                {
                    if (addressConverted)
                    {
                        AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                        return;
                    }

                    if (!AdjustAuthRemainedTimeout())
                        return;


                    if (!ConvertIPv4EndpointToIPv6IPPort(endpoint, out string ipv6, out int port))
                    {
                        AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                        return;
                    }

                    RedirectToNewRtmGate(ipv6 + ":" + port, true);
                }
            }, authStatsInfo.remainedTimeout);
            client.AsyncConnect();

            if (requireClose)
                client.Close();
        }

        private void QuickRelogin()
        {
            rtmGateConnectionId = reservedRtmGateConnectionId;
            RTMControlCenter.RegisterSession(rtmGateConnectionId, this);
            Auth(false);
        }

        private void QuickConnectRelogin()
        {
            TCPClient client;
            lock (interLocker)
            {
                client = rtmGate;
                authStatsInfo.rtmClients.Add(rtmGate);
            }
            ConfigRtmGateClient(client, (Int64 connectionId, string endpoint, bool connected) => {
                if (requireClose)
                {
                    AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_CONNECTION_CLOSED);
                    return;
                }

                if (connected)
                {
                    rtmGateConnectionId = connectionId;
                    RTMControlCenter.RegisterSession(rtmGateConnectionId, this);
                    Auth();
                }
                else
                {
                    if (!AdjustAuthRemainedTimeout())
                        return;

                    if (AsyncFetchRtmGateEndpoint("ipv4", DispatchCallBack_Which_IPv4, authStatsInfo.remainedTimeout))
                    {
                        AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                        return;
                    }
                }
            }, authStatsInfo.remainedTimeout);
            client.AsyncConnect();
        }

        //-------------[ Login & System interfaces ]--------------------------//
        /**
         * Return false if logined, or other async Login is processing, or No available connection.
         * Please check Status fields.
         */
        public bool Login(AuthDelegate callback, string token, int timeout = 0)
        {
            return Login(callback, token, null, "", timeout);
        }

        /**
         * Return false if logined, or other async Login is processing, or No available connection.
         * Please check Status fields.
         */
        public bool Login(AuthDelegate callback, string token, Dictionary<string, string> attr, TranslateLanguage language = TranslateLanguage.None, int timeout = 0)
        {
            return Login(callback, token, null, GetTranslatedLanguage(language), timeout);
        }

        /**
         * Return false if logined, or other async Login is processing, or No available connection.
         * Please check Status fields.
         */
        private bool Login(AuthDelegate callback, string token, Dictionary<string, string> attr, string lang = "", int timeout = 0)
        {
            bool quickLogin = false;
            bool quickConnectLogin = false;

            lock (interLocker)
            {
                if (status == ClientStatus.Connected || status == ClientStatus.Connecting)
                {
                    if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                        ClientEngine.RunTask(() =>
                        {
                            callback(projectId, uid, false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                        });

                    return false;
                }

                status = ClientStatus.Connecting;
                syncConnectingEvent.Reset();

                requireClose = false;

                if (rtmGate != null)
                {
                    if (rtmGate.IsConnected())
                        quickLogin = true;
                    else
                        quickConnectLogin = true;
                }
            }

            bool usingDefaultQuestTimeout = false;
            if (timeout == 0)
            {
                usingDefaultQuestTimeout = true;

                if (quickLogin)
                    timeout = RTMConfig.globalQuestTimeoutSeconds;
                else
                    timeout = ((ConnectTimeout == 0) ? RTMConfig.globalConnectTimeoutSeconds : ConnectTimeout)
                        + ((QuestTimeout == 0) ? RTMConfig.globalQuestTimeoutSeconds : QuestTimeout);
            }

            authStatsInfo = new AuthStatusInfo
            {
                usingDefaultQuestTimeout = usingDefaultQuestTimeout,
                remainedTimeout = timeout,
                authCallback = callback,
                rtmClients = new HashSet<TCPClient>(),
            };

            authStatsInfo.token = token;
            authStatsInfo.attr = attr;
            authStatsInfo.lang = lang;
            authStatsInfo.lastActionMsecTimeStamp = ClientEngine.GetCurrentMilliseconds();

            if (quickLogin)
            {
                QuickRelogin();
                return true;
            }

            if (quickConnectLogin)
            {
                QuickConnectRelogin();
                return true;
            }

            if (AsyncFetchRtmGateEndpoint("ipv4", DispatchCallBack_Which_IPv4, timeout))
                return true;
            else
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Login for uid " + uid + " failed. No available connection.");

                lock (interLocker)
                {
                    status = ClientStatus.Closed;
                    syncConnectingEvent.Set();

                    if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                        ClientEngine.RunTask(() =>
                        {
                            callback(projectId, uid, false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                        });
                }

                return false;
            }
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
            return Login(out ok, token, null, GetTranslatedLanguage(language), timeout);
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

        public void Close(bool waitConnectingCannelled = true)
        {
            HashSet<TCPClient> clients = new HashSet<TCPClient>();
            clients.Add(dispatch);

            bool isConnecting = false;

            lock (interLocker)
            {
                if (status == ClientStatus.Closed)
                    return;

                requireClose = true;

                if (status == ClientStatus.Connecting)
                {
                    foreach (TCPClient client in authStatsInfo.rtmClients)
                        clients.Add(client);

                    isConnecting = true;
                }
                else
                {
                    clients.Add(rtmGate);
                    status = ClientStatus.Closed;
                }
            }

            foreach (TCPClient client in clients)
                client.Close();

            if (isConnecting && waitConnectingCannelled)
                syncConnectingEvent.WaitOne();
        }
    }
}
