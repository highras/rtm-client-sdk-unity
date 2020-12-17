# RTM Client Unity SDK Rooms API Docs

# Index

[TOC]

### Enter Room

	//-- Async Method
	public bool EnterRoom(DoneDelegate callback, long roomId, int timeout = 0);

	//-- Sync Method
	public int EnterRoom(long roomId, int timeout = 0);

Enter room.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `long roomId`

	Room id.

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

### Leave Room


	//-- Async Method
	public bool LeaveRoom(DoneDelegate callback, long roomId, int timeout = 0);

	//-- Sync Method
	public int LeaveRoom(long roomId, int timeout = 0);

Leave room.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `long roomId`

	Room id.

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



### Get User Rooms

	//-- Async Method
	public bool GetUserRooms(Action<HashSet<long>, int> callback, int timeout = 0);

	//-- Sync Method
	public int GetUserRooms(out HashSet<long> roomIds, int timeout = 0);

Get current user's all groups.

+ `Action<HashSet<long>, int> callback`

	Callabck for async method.  
	First `HashSet<long>` is gotten current user's room ids;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out HashSet<long> roomIds`

	The gotten current user's room ids.

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

### Set Room Info


	//-- Async Method
	public bool SetRoomInfo(DoneDelegate callback, long roomId, string publicInfo = null, string privateInfo = null, int timeout = 0);
	
	//-- Sync Method
	public int SetRoomInfo(long roomId, string publicInfo = null, string privateInfo = null, int timeout = 0);

Set room public info and private info. Note: Current user MUST in the room.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `long roomId`

	Room id.

+ `string publicInfo`

	New public info for room. `null` means don't change the public info. Max length is 65535 bytes.

+ `string privateInfo`

	New private info for room. `null` means don't change the private info. Max length is 65535 bytes.

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


### Get Room Info

	//-- Async Method
	public bool GetRoomInfo(Action<string, string, int> callback, long roomId, int timeout = 0);
	
	//-- Sync Method
	public int GetRoomInfo(out string publicInfo, out string privateInfo, long roomId, int timeout = 0);

Get room public info and private info. Note: Current user MUST in the room.

Parameters:

+ `Action<string, string, int> callback`

	Callabck for async method.  
	First `string` is gotten public info of this room;  
	Second `string` is gotten private info of this room;  
	Thrid `int` is the error code indicating the calling is successful or the failed reasons.

+ `out string publicInfo`

	The gotten public info of this room.

+ `out string privateInfo`

	The gotten private info of this room.

+ `long roomId`

	Room id.

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


### Get Room Public Info

	//-- Async Method
	public bool GetRoomPublicInfo(Action<string, int> callback, long roomId, int timeout = 0);
	
	//-- Sync Method
	public int GetRoomPublicInfo(out string publicInfo, long roomId, int timeout = 0);

Get Room public info.

Parameters:

+ `Action<string, int> callback`

	Callabck for async method.  
	First `string` is gotten public info of the room;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out string publicInfo`

	The gotten public info of this room.

+ `long roomId`

	Room id.

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


### Get Rooms Public Infos

	//-- Async Method
	public bool GetRoomsPublicInfo(Action<Dictionary<long, string>, int> callback, HashSet<long> roomIds, int timeout = 0);
	
	//-- Sync Method
	public int GetRoomsPublicInfo(out Dictionary<long, string> publicInfos, HashSet<long> roomIds, int timeout = 0);

Get rooms' public infos.

Parameters:

+ `Action<Dictionary<long, string>, int> callback`

	Callabck for async method.  
	First `Dictionary<long, string>` is gotten rooms' public infos. Key is room id, value is the public info;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out Dictionary<long, string> publicInfos`

	The gotten rooms' public infos. Key is room id, value is the public info.

+ `HashSet<long> roomIds`

	Rooms' ids. Max 100 rooms for each calling.

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


### Get Room Members

	//-- Async Method
	public bool GetRoomMembers(Action<HashSet<long>, int> callback, long roomId, int timeout = 0);
	
	//-- Sync Method
	public int GetRoomMembers(out HashSet<long> uids, long roomId, int timeout = 0);

Get room member list.

**IMPORTANT** The synchronous frequence of the statistic data is 5 seconds.

Parameters:

+ `Action<HashSet<long>, int> callback`

	Callabck for async method.  
	First `HashSet<long>` is the member list of indicated room;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out HashSet<long> uids`

	The member list of indicated room.

+ `long roomId`

	Id of room which member list is wanted to be gotten.

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


### Get Room Member Count

	//-- Async Method
	public bool GetRoomMemberCount(Action<int, int> callback, long roomId, int timeout = 0);
	
	//-- Sync Method
	public int GetRoomMemberCount(out int count, long roomId, int timeout = 0);

Get room member count.

**IMPORTANT** The synchronous frequence of the statistic data is 5 seconds.

Parameters:

+ `Action<int, int> callback`

	Callabck for async method.  
	First `int` is the member count of the indicated room;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out int count`

	The member count of the indicated room.

+ `long roomId`

	Id of room which member count is wanted to be gotten.

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


