using System;
namespace com.fpnn
{
    public class Config
    {
        public static readonly string Version = "2.0.2";

        //----------------[ Nested Structure ]-----------------------//
        public struct TaskThreadPoolConfig
        {
            public int initThreadCount;
            public int perfectThreadCount;
            public int maxThreadCount;
            public int maxQueueLengthLimitation;
            public int tempLatencySeconds;
        }

        //----------------[ Customized Fields ]-----------------------//

        public TaskThreadPoolConfig taskThreadPoolConfig;
        public int globalConnectTimeoutSeconds;
        public int globalQuestTimeoutSeconds;
        public int maxPayloadSize;
        public common.ErrorRecorder errorRecorder;

        public Config()
        {
            taskThreadPoolConfig.initThreadCount = 1;
            taskThreadPoolConfig.perfectThreadCount = 2;
            taskThreadPoolConfig.maxThreadCount = 4;
            taskThreadPoolConfig.maxQueueLengthLimitation = 0;
            taskThreadPoolConfig.tempLatencySeconds = 60;

            globalConnectTimeoutSeconds = 5;
            globalQuestTimeoutSeconds = 5;
            maxPayloadSize = 1024 * 1024 * 4;        //-- 4MB
        }
    }
}
