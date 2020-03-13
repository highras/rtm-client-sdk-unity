# RTM Client Unity SDK Friends API Docs

# Index

[TOC]

### Add Friends

	//-- Async Method
	public bool AddFriends(DoneDelegate callback, HashSet<long> uids, int timeout = 0);

	//-- Sync Method
	public int AddFriends(HashSet<long> uids, int timeout = 0);

Add friends.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `HashSet<long> uids`

	Friends' uids set. Max 100 users for each calling.

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


### Delete Friends

	//-- Async Method
	public bool DeleteFriends(DoneDelegate callback, HashSet<long> uids, int timeout = 0);

	//-- Sync Method
	public int DeleteFriends(HashSet<long> uids, int timeout = 0);

Delete friends.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `HashSet<long> uids`

	Friends' uids set. Max 100 users for each calling.

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


### Get Friends

	//-- Async Method
	public bool GetFriends(Action<HashSet<long>, int> callback, int timeout = 0);

	//-- Sync Method
	public int GetFriends(out HashSet<long> friends, int timeout = 0);

Get friends.

+ `Action<HashSet<long>, int> callback`

	Callabck for async method.  
	First `HashSet<long>` is gotten friends' uids;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out HashSet<long> friends`

	The gotten friends' uids.

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
