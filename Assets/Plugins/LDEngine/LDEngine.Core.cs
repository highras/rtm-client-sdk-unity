using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.common;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public partial class LDEngine
    {
        internal RTMClient rtmClient;
        static private ErrorRecorder errorRecorder;

        public RTM RTM { get; }
        public IM IM { get; }
        public ValueAdded ValueAdded { get; }

        static public LDEngine CreateEngine(string endpoint, long pid, long uid)
        {
            return new LDEngine(endpoint, pid, uid);
        }

        public LDEngine(string endpoint, long pid, long uid)
        {
            processor = new RTMQuestProcessor();
            rtmClient = RTMClient.getInstance(endpoint, pid, uid, processor);
            RTM = new RTM(rtmClient);
            IM = new IM(rtmClient);
            ValueAdded = new ValueAdded(rtmClient);
        }

        public void SetErrorRecorder(ErrorRecorder errorRecorder)
        {
            LDEngine.errorRecorder = errorRecorder;
        }

        string GetRTMEndpoint(RTMENDPOINT endpointType)
        {
            if (endpointType == RTMENDPOINT.RTMENDPOINT_NX)
                return LDEngineConfig.RTMEndpoint_NX;
            else
                return LDEngineConfig.RTMEndpoint_INTL;
        }

        static internal void ErrorLog(string message)
        {
            LDEngine.errorRecorder?.RecordError(message);
        }

        public RTMClient GetRTMClient() { return rtmClient; }
    }
}


