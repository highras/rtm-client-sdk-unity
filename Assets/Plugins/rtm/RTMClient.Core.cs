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
        private readonly long pid;
        private readonly long uid;

        private ClientStatus status;
        private volatile bool requireClose;
        private ManualResetEvent syncConnectingEvent;

        public volatile int ConnectTimeout;
        public volatile int QuestTimeout;

        private RTMQuestProcessor processor;
        private TCPClient dispatch;
        private TCPClient rtmGate;
        private Int64 rtmGateConnectionId;

        private AuthStatusInfo authStatsInfo;
        private string lastAvailableRtmGateEndpoint;
        private common.ErrorRecorder errorRecorder;

        public RTMClient(string endpoint, long pid, long uid, IRTMQuestProcessor serverPushProcessor)
        {
            interLocker = new object();
            this.pid = pid;
            this.uid = uid;
            status = ClientStatus.Closed;
            requireClose = false;
            syncConnectingEvent = new ManualResetEvent(false);

            ConnectTimeout = 0;
            QuestTimeout = 0;

            processor = new RTMQuestProcessor();
            processor.SetProcessor(serverPushProcessor);

            dispatch = TCPClient.Create(endpoint, true);
            errorRecorder = RTMConfig.errorRecorder;
            if (errorRecorder != null)
            {
                processor.SetErrorRecorder(errorRecorder);
                dispatch.SetErrorRecorder(errorRecorder);
            }
        }

        //-------------[ Fack Fields ]--------------------------//

        public long Pid
        {
            get
            {
                return pid;
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
            TCPClient client = null;
            long connectionId = 0;
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
                    connectionId = rtmGateConnectionId;
                    rtmGateConnectionId = 0;
                    client = rtmGate;
                    rtmGate = null;
                }

                currInfo = authStatsInfo;
                authStatsInfo = null;
                currUid = uid;

                syncConnectingEvent.Set();
            }

            if (currInfo != null)
                currInfo.authCallback(pid, currUid, authStatus, errorCode);

            if (connectionId != 0)
                RTMControlCenter.UnregisterSession(connectionId);

            if (client != null)
                client.Close();
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
                lock (interLocker)
                {
                    trigger = rtmGateConnectionId == connectionId;
                    if (trigger)
                        status = ClientStatus.Closed;
                }

                if (trigger)
                    processor.SessionClosed(causedByError ? com.fpnn.ErrorCode.FPNN_EC_CORE_UNKNOWN_ERROR : com.fpnn.ErrorCode.FPNN_EC_OK);
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
                            lastAvailableRtmGateEndpoint = ipv4endpoint;
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

        private void Auth()
        {
            if (!AdjustAuthRemainedTimeout())
                return;

            processor.SetConnectionId(rtmGateConnectionId);
            
            Quest quest = new Quest("auth");
            quest.Param("pid", pid);
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
            }, authStatsInfo.remainedTimeout);
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
                    lastAvailableRtmGateEndpoint = endpoint;
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
            lock (interLocker)
            {
                if (status == ClientStatus.Connected)
                    return false;

                if (status == ClientStatus.Connecting)
                    return false;

                status = ClientStatus.Connecting;
                syncConnectingEvent.Reset();
            }

            if (timeout == 0)
            {
                timeout = ((ConnectTimeout == 0) ? RTMConfig.globalConnectTimeoutSeconds : ConnectTimeout)
                    + ((QuestTimeout == 0) ? RTMConfig.globalQuestTimeoutSeconds : QuestTimeout);
            }

            authStatsInfo = new AuthStatusInfo
            {
                remainedTimeout = timeout,
                authCallback = callback,
                rtmClients = new HashSet<TCPClient>(),
            };

            authStatsInfo.token = token;
            authStatsInfo.attr = attr;
            authStatsInfo.lang = lang;
            authStatsInfo.lastActionMsecTimeStamp = ClientEngine.GetCurrentMilliseconds();

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
            bool actionBegin = Login((long pid, long uid, bool authStatus, int errorCode) => {
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

        public void Close()
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
                    clients.Add(rtmGate);

                status = ClientStatus.Closed;
            }

            foreach (TCPClient client in clients)
                client.Close();

            if (isConnecting)
                syncConnectingEvent.WaitOne();
        }
    }
}
