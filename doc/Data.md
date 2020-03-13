# RTM Client Unity SDK Data API Docs

# Index

[TOC]

### Get Data

	//-- Async Method
	public bool DataGet(Action<string, int> callback, string key, int timeout = 0);
	
	//-- Sync Method
	public int DataGet(out string value, string key, int timeout = 0);

Get user's data.

Parameters:

+ `Action<string, int> callback`

	Callabck for async method.  
	First `string` is gotten data associated the inputted `string key`;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out string value`

	The gotten data associated the inputted `string key`.

+ `string key`

	The key of wanted to be gotten data.

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async calling is start.
	* false: Start async calling is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means calling successed.

	Others are the reason for calling failed.

### Set Data

	//-- Async Method
	public bool DataSet(DoneDelegate callback, string key, string value, int timeout = 0);
	
	//-- Sync Method
	public int DataSet(string key, string value, int timeout = 0);

Set user's data.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `string key`

	The key of user's data. Max 128 bytes length.

+ `string value`

	The value of user's data. Max 65535 bytes length.

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async calling is start.
	* false: Start async calling is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means calling successed.

	Others are the reason for calling failed.

### Delete Data

	//-- Async Method
	public bool DataDelete(DoneDelegate callback, string key, int timeout = 0);
	
	//-- Sync Method
	public int DataDelete(string key, int timeout = 0);

Delete user's data.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `string key`

	The key of user's data.

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async calling is start.
	* false: Start async calling is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means calling successed.

	Others are the reason for calling failed.



