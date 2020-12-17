# RTM Client Unity SDK Users API Docs

# Index

[TOC]

### Get Online Users

	//-- Async Method
	public bool GetOnlineUsers(Action<HashSet<long>, int> callback, HashSet<long> uids, int timeout = 0);

	//-- Sync Method
	public int GetOnlineUsers(out HashSet<long> onlineUids, HashSet<long> uids, int timeout = 0);

Get online users.

+ `Action<HashSet<long>, int> callback`

	Callabck for async method.  
	First `HashSet<long>` is the online users' ids;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out HashSet<long> uids`

	The online users' ids.

+ `HashSet<long> uids`

	The users' ids which want to be checked.

	Max 200 uids for each calling.

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

### Set User Info


	//-- Async Method
	public bool SetUserInfo(DoneDelegate callback, string publicInfo = null, string privateInfo = null, int timeout = 0);
	
	//-- Sync Method
	public int SetUserInfo(string publicInfo = null, string privateInfo = null, int timeout = 0);

Set user's public info and private info.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `string publicInfo`

	New public info for group. `null` means don't change the public info. Max length is 65535 bytes.

+ `string privateInfo`

	New private info for group. `null` means don't change the private info. Max length is 65535 bytes.

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


### Get User Info

	//-- Async Method
	public bool GetUserInfo(Action<string, string, int> callback, int timeout = 0);
	
	//-- Sync Method
	public int GetUserInfo(out string publicInfo, out string privateInfo, int timeout = 0);

Get user's public info and private info.

Parameters:

+ `Action<string, string, int> callback`

	Callabck for async method.  
	First `string` is gotten public info of current user;  
	Second `string` is gotten private info of current user;  
	Thrid `int` is the error code indicating the calling is successful or the failed reasons.

+ `out string publicInfo`

	The gotten public info of current user.

+ `out string privateInfo`

	The gotten private info of current user.

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


### Get Users Public Infos

	//-- Async Method
	public bool GetUserPublicInfo(Action<Dictionary<long, string>, int> callback, HashSet<long> uids, int timeout = 0);
	
	//-- Sync Method
	public int GetUserPublicInfo(out Dictionary<long, string> publicInfos, HashSet<long> uids, int timeout = 0);

Get users' public infos.

Parameters:

+ `Action<Dictionary<long, string>, int> callback`

	Callabck for async method.  
	First `Dictionary<long, string>` is gotten users' public infos. Key is uid, value is the public info;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out Dictionary<long, string> publicInfos`

	The gotten users' public infos. Key is uid, value is the public info.

+ `HashSet<long> uids`

	Users' ids.	Max 100 users for each calling.

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


