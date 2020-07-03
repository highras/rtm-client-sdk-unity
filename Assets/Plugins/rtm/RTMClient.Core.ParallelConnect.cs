using System;
using System.Collections.Generic;
using System.Threading;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //-------------[ Auth(Login) processing functions ]--------------------------//
        private void StartParallelConnect(string ipv4Endpoint)
        {
            ParallelLoginStatusInfo plsi = ParallelLoginStatusInfo.Clone(authStatsInfo);

            authStatsInfo.parallelCompleted = false;

            if (ipv4Endpoint.Length == 0)
            {
                authStatsInfo.parallelConnectingCount = 2;
            }
            else
            {
                authStatsInfo.parallelConnectingCount = 3;
                ConnectRtmGateWithIPv4ConvertToIPv6(ipv4Endpoint, plsi.Clone());
            }

            StartParallelConnect("ipv6", plsi.Clone());
            StartParallelConnect("domain", plsi.Clone());
        }

        private void StartParallelConnect(string addressType, ParallelLoginStatusInfo info)
        {

            bool status = AsyncFetchRtmGateEndpoint(addressType, (Answer answer, int errorCode) =>
            {
                if (requireClose)
                {
                    DecreaseParallelConnectingCount();
                    return;
                }

                ParallelDispatchCallBack(answer, errorCode, info);
            }, info.remainedTimeout);

            if (!status)
                DecreaseParallelConnectingCount();
        }

        private void ParallelDispatchCallBack(Answer answer, int errorCode, ParallelLoginStatusInfo info)
        {
            if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
            {
                string endpoint = answer.Get<string>("endpoint", string.Empty);
                if (endpoint.Length > 0)
                {
                    ParallelConnectToRtmGate(endpoint, info);
                }
                else
                {
                    DecreaseParallelConnectingCount();
                }
            }
            else
                DecreaseParallelConnectingCount();
        }

        private void ConnectRtmGateWithIPv4ConvertToIPv6(string ipv4endpoint, ParallelLoginStatusInfo info)
        {
            if (!ConvertIPv4EndpointToIPv6IPPort(ipv4endpoint, out string ipv6, out int port))
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Invalid IPv4 endpoint " + ipv4endpoint + " when ConnectRtmGateWithIPv4ConvertToIPv6() called.");

                DecreaseParallelConnectingCount();
                return;
            }

            ParallelConnectToRtmGate(ipv6 + ":" + port, info);
        }

        private void ParallelConnectToRtmGate(string endpoint, ParallelLoginStatusInfo info)
        {
            if (!AdjustAuthRemainedTimeout(info))
            {
                DecreaseParallelConnectingCount();
                return;
            }

            TCPClient client = TCPClient.Create(endpoint, false);
            lock (interLocker)
            {
                if (status != ClientStatus.Connecting)
                    return;

                authStatsInfo.rtmClients.Add(client);
            }
            ConfigRtmGateClient(client, (Int64 connectionId, string rtmGateEndpoint, bool connected) => {
                if (requireClose)
                {
                    DecreaseParallelConnectingCount();
                    return;
                }

                if (connected)
                {
                    lock (interLocker)
                    {
                        if (status != ClientStatus.Connecting)
                            return;

                        authStatsInfo.parallelConnectingCount -= 1;
                        if (authStatsInfo.parallelCompleted)
                            return;

                        authStatsInfo.parallelCompleted = true;
                        authStatsInfo.remainedTimeout = info.remainedTimeout;
                        authStatsInfo.lastActionMsecTimeStamp = info.lastActionMsecTimeStamp;

                        rtmGate = client;
                    }

                    rtmGateConnectionId = connectionId;
                    RTMControlCenter.RegisterSession(rtmGateConnectionId, this);
                    Auth();
                }
                else
                {
                    DecreaseParallelConnectingCount();
                }
            }, info.remainedTimeout);
            client.AsyncConnect();

            if (requireClose)
                client.Close();
        }

        private bool AdjustAuthRemainedTimeout(ParallelLoginStatusInfo plsi)
        {
            long curr = ClientEngine.GetCurrentMilliseconds();
            int passSeconds = (int)(curr - plsi.lastActionMsecTimeStamp) / 1000;
            plsi.lastActionMsecTimeStamp = curr;
            plsi.remainedTimeout -= passSeconds;
            if (plsi.remainedTimeout <= 0)
                return false;
            else
                return true;
        }

        private void DecreaseParallelConnectingCount()
        {
            bool failed = false;
            lock (interLocker)
            {
                if (status != ClientStatus.Connecting)
                    return;

                authStatsInfo.parallelConnectingCount -= 1;
                if (authStatsInfo.parallelCompleted)
                    return;

                if (authStatsInfo.parallelConnectingCount == 0)
                    failed = true;
            }

            if (failed)
                AuthFinish(false, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
        }
    }
}