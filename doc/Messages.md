# RTM Client Unity SDK Messages API Docs

# Index

[TOC]

### Send P2P Message

	//-- Async Method
	public bool SendMessage(ActTimeDelegate callback, long uid, byte mtype, string message, string attrs = "", int timeout = 0);
	public bool SendMessage(ActTimeDelegate callback, long uid, byte mtype, byte[] message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendMessage(out long mtime, long uid, byte mtype, string message, string attrs = "", int timeout = 0);
	public int SendMessage(out long mtime, long uid, byte mtype, byte[] message, string attrs = "", int timeout = 0);

Send P2P message.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long uid`

	Receiver user id.

+ `byte mtype`

	Message type for message. MUST large than 50.

+ `string message`

	Text message.

+ `byte[] message`

	Binary message.

+ `string attrs`

	Message attributes in Json.

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async sending is start.
	* false: Start async sending is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means sending successed.

	Others are the reason for sending failed.


### Send Group Messsage

	//-- Async Method
	public bool SendGroupMessage(ActTimeDelegate callback, long groupId, byte mtype, string message, string attrs = "", int timeout = 0);
	public bool SendGroupMessage(ActTimeDelegate callback, long groupId, byte mtype, byte[] message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendGroupMessage(out long mtime, long groupId, byte mtype, string message, string attrs = "", int timeout = 0);
	public int SendGroupMessage(out long mtime, long groupId, byte mtype, byte[] message, string attrs = "", int timeout = 0);

Send message in group.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long groupId`

	Group id.

+ `byte mtype`

	Message type for message. MUST large than 50.

+ `string message`

	Text message.

+ `byte[] message`

	Binary message.

+ `string attrs`

	Message attributes in Json.

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async sending is start.
	* false: Start async sending is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means sending successed.

	Others are the reason for sending failed.


### Send Room Message

	//-- Async Method
	public bool SendRoomMessage(ActTimeDelegate callback, long roomId, byte mtype, string message, string attrs = "", int timeout = 0);
	public bool SendRoomMessage(ActTimeDelegate callback, long roomId, byte mtype, byte[] message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendRoomMessage(out long mtime, long roomId, byte mtype, string message, string attrs = "", int timeout = 0);
	public int SendRoomMessage(out long mtime, long roomId, byte mtype, byte[] message, string attrs = "", int timeout = 0);

Send message in room.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long roomId`

	Room id.

+ `byte mtype`

	Message type for message. MUST large than 50.

+ `string message`

	Text message.

+ `byte[] message`

	Binary message.

+ `string attrs`

	Message attributes in Json.

+ `int timeout`

	Timeout in second.

	0 means using default setting.


Return Values:

+ bool for Async

	* true: Async sending is start.
	* false: Start async sending is failed.

+ int for Sync

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means sending successed.

	Others are the reason for sending failed.

### Get P2P Message

	//-- Async Method
	public bool GetP2PMessage(HistoryMessageDelegate callback, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0);
	
	//-- Sync Method
	public int GetP2PMessage(out HistoryMessageResult result, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0);

Get history data for P2P message.

Parameters:

+ `HistoryMessageDelegate callback`

		public delegate void HistoryMessageDelegate(int count, long lastId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode);

	Callabck for async method. Please refer [HistoryMessageDelegate](Delegates.md#HistoryMessageDelegate).

+ `out HistoryMessageResult result`

	Fetched history data. Please refer [HistoryMessageResult](Structures.md#HistoryMessageResult).

+ `long peerUid`

	Peer user id.

+ `bool desc`

	* true: desc order;
	* false: asc order.

+ `int count`

	Count for retrieving. Max is 20 for each calling.

+ `long beginMsec`

	Beginning timestamp in milliseconds.

+ `long endMsec`

	Ending timestamp in milliseconds.

+ `long lastId`

	Last data id returned when last calling. First calling using 0.

+ `List<byte> mtypes`

	Message types for retrieved message. `null` means all types.

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


### Get Group Messsage


	//-- Async Method
	public bool GetGroupMessage(HistoryMessageDelegate callback, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0);
	
	//-- Sync Method
	public int GetGroupMessage(out HistoryMessageResult result, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0);

Get history data for group message.

Parameters:

+ `HistoryMessageDelegate callback`

		public delegate void HistoryMessageDelegate(int count, long lastId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode);

	Callabck for async method. Please refer [HistoryMessageDelegate](Delegates.md#HistoryMessageDelegate).

+ `out HistoryMessageResult result`

	Fetched history data. Please refer [HistoryMessageResult](Structures.md#HistoryMessageResult).

+ `long groupId`

	Group id.

+ `bool desc`

	* true: desc order;
	* false: asc order.

+ `int count`

	Count for retrieving. Max is 20 for each calling.

+ `long beginMsec`

	Beginning timestamp in milliseconds.

+ `long endMsec`

	Ending timestamp in milliseconds.

+ `long lastId`

	Last data id returned when last calling. First calling using 0.

+ `List<byte> mtypes`

	Message types for retrieved message. `null` means all types.

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


### Get Room Message

	//-- Async Method
	public bool GetRoomMessage(HistoryMessageDelegate callback, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0);
	
	//-- Sync Method
	public int GetRoomMessage(out HistoryMessageResult result, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0);

Get history data for room message.

Parameters:

+ `HistoryMessageDelegate callback`

		public delegate void HistoryMessageDelegate(int count, long lastId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode);

	Callabck for async method. Please refer [HistoryMessageDelegate](Delegates.md#HistoryMessageDelegate).

+ `out HistoryMessageResult result`

	Fetched history data. Please refer [HistoryMessageResult](Structures.md#HistoryMessageResult).

+ `long roomId`

	Room id.

+ `bool desc`

	* true: desc order;
	* false: asc order.

+ `int count`

	Count for retrieving. Max is 20 for each calling.

+ `long beginMsec`

	Beginning timestamp in milliseconds.

+ `long endMsec`

	Ending timestamp in milliseconds.

+ `long lastId`

	Last data id returned when last calling. First calling using 0.

+ `List<byte> mtypes`

	Message types for retrieved message. `null` means all types.

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

### Get Broadcast Message


	//-- Async Method
	public bool GetBroadcastMessage(HistoryMessageDelegate callback, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0);
	
	//-- Sync Method
	public int GetBroadcastMessage(out HistoryMessageResult result, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, List<byte> mtypes = null, int timeout = 0);

Get history data for broadcast message.

Parameters:

+ `HistoryMessageDelegate callback`

		public delegate void HistoryMessageDelegate(int count, long lastId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode);

	Callabck for async method. Please refer [HistoryMessageDelegate](Delegates.md#HistoryMessageDelegate).

+ `out HistoryMessageResult result`

	Fetched history data. Please refer [HistoryMessageResult](Structures.md#HistoryMessageResult).

+ `bool desc`

	* true: desc order;
	* false: asc order.

+ `int count`

	Count for retrieving. Max is 20 for each calling.

+ `long beginMsec`

	Beginning timestamp in milliseconds.

+ `long endMsec`

	Ending timestamp in milliseconds.

+ `long lastId`

	Last data id returned when last calling. First calling using 0.

+ `List<byte> mtypes`

	Message types for retrieved message. `null` means all types.

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

### Delete Message

	//-- Async Method
	public bool DeleteMessage(DoneDelegate callback, long xid, long mid, int type, int timeout = 0);
	
	//-- Sync Method
	public int DeleteMessage(long xid, long mid, int type, int timeout = 0);

Delete a sent message.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `long xid`

	Peer uid for P2P chat, or group id for group chat, or room id for room chat.

+ `long mid`

	The mid of chat message.

+ `int type`

	* 1: P2P Chat
	* 2: Group Chat
	* 3: Room Chat

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


### Get Message

	//-- Async Method
	public bool GetMessage(Action<RetrievedMessage, int> callback, long xid, long mid, int type, int timeout = 0);
	
	//-- Sync Method
	public int GetMessage(out RetrievedMessage retrievedMessage, long xid, long mid, int type, int timeout = 0);

Retrieve a sent message.

Parameters:

+ `Action<RetrievedMessage, int> callback`

	Callabck for async method.  
	First `RetrievedMessage` is retrieved data, please refer [RetrievedMessage](Structures.md#RetrievedMessage);  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out RetrievedMessage retrievedMessage`

	The retrieved data, please refer [RetrievedMessage](Structures.md#RetrievedMessage).	

+ `long xid`

	Peer uid for P2P chat, or group id for group chat, or room id for room chat.

+ `long mid`

	The mid of chat.

+ `int type`

	* 1: P2P Chat
	* 2: Group Chat
	* 3: Room Chat

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

