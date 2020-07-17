# RTM Client Unity SDK API Docs: Delegates

# Index

[TOC]

### AuthDelegate

	public delegate void AuthDelegate(long projectId, long uid, bool successful, int errorCode);

Parameters:

+ `long pid`

	Project id.

+ `long uid`

	User id.

+ `bool authStatus`

	* true: login success
	* false: login failed. Reason can be deduced from errorCode.

+ `int errorCode`

	Reason for login failed.

### DoneDelegate

	public delegate void DoneDelegate(int errorCode);

Parameters:

+ `int errorCode`

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means successed.

	Others are the reason for failed.

### ActTimeDelegate

	public delegate void ActTimeDelegate(long modifiedTime, int errorCode);

Parameters:

+ `long modifiedTime`

	If action is successful, `modifiedTime` is the action completed time.

	If action is failed, `modifiedTime` is 0.

+ `int errorCode`

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means successed.

	Others are the reason for failed.

### HistoryMessageDelegate

	public delegate void HistoryMessageDelegate(int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode);

Parameters:

+ `int count`

	Retrieved messages count.

+ `long lastCursorId`

	When calling history functions for fetching following messsges, using this for corresponding patameter.

+ `long beginMsec`

	When calling history functions for fetching following messsges, using this for corresponding patameter.

+ `long endMsec`

	When calling history functions for fetching following messsges, using this for corresponding patameter.

+ `List<HistoryMessage> messages`

	Retrieved history messages. Declaration of struct HistoryMessage can be found at [HistoryMessage](Structures.md#HistoryMessage).

+ `int errorCode`

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means successed.

	Others are the reason for failed.
