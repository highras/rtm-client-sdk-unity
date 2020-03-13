# RTM Client Unity SDK Init API Docs

# Index

[TOC]

### FPNN SDK Init (REQUIRED)

	using com.fpnn;
	ClientEngine.Init();
	ClientEngine.Init(Config config);

**com.fpnn.Config Fields:**

* Config.taskThreadPoolConfig.initThreadCount

	Inited threads count of SDK task thread pool. Default value is 1.

* Config.taskThreadPoolConfig.perfectThreadCount

	Max resident threads count of SDK task thread pool. Default value is 2.

* Config.taskThreadPoolConfig.maxThreadCount

	Max threads count of SDK task thread pool, including resident threads and temporary threads. Default value is 4.

* Config.taskThreadPoolConfig.maxQueueLengthLimitation

	Max tasks count of SDK task thread pool. Default value is 0, means no limitation.

* Config.taskThreadPoolConfig.tempLatencySeconds

	How many seconds are waited for the next dispatched task before the temporary thread exit. Default value is 60.

* Config.globalConnectTimeoutSeconds

	Global client connecting timeout setting when no special connecting timeout are set for a client or connect function.

	Default is 5 seconds.

* Config.globalQuestTimeoutSeconds

	Global quest timeout setting when no special quest timeout are set for a client or sendQuest function.

	Default is 5 seconds.

* Config.maxPayloadSize

	Max bytes limitation for the quest & answer package. Default is 4MB.

* Config.errorRecorder

	Instance of com.fpnn.common.ErrorRecoder implemented. Default is null.

### RTM SDK Init (REQUIRED)

	using com.fpnn.rtm;
	RTMControlCenter.Init();
	RTMControlCenter.Init(RTMConfig config);

**com.fpnn.rtm.RTMConfig Fields:**

* RTMConfig.maxPingInterval

	Max interval in seconds for pings from RTM servers. If the seconds are elapsed and client has not received any ping, RTMClient will consider the connection is broken.

	Default is 120 seconds.

* RTMConfig.globalConnectTimeout

	Global RTMClient connecting timeout setting when no special connecting timeout are set for a RTMClient or login function.

	Default is 30 seconds.

* RTMConfig.globalQuestTimeout
	
	Global quest timeout setting when no special quest timeout are set for a client or sendQuest function.

	Default is 30 seconds.

* RTMConfig.fileClientHoldingSeconds

	Cached seconds for file clients when the file transportation functions are used.

	Default is 150 seconds; 


* RTMConfig.defaultErrorRecorder

	Instance of com.fpnn.common.ErrorRecoder implemented. Default is null.
