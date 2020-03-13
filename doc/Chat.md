# RTM Client Unity SDK Chat API Docs

# Index

[TOC]

### Send P2P Chat

	//-- Async Method
	public bool SendChat(ActTimeDelegate callback, long uid, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendChat(out long mtime, long uid, string message, string attrs = "", int timeout = 0);

Send P2P text message.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long uid`

	Receiver user id.

+ `string message`

	Chat message.

+ `string attrs`

	Chat message attributes in Json.

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


### Send Group Chat

	//-- Async Method
	public bool SendGroupChat(ActTimeDelegate callback, long groupId, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendGroupChat(out long mtime, long groupId, string message, string attrs = "", int timeout = 0);

Send text message in group.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long groupId`

	Group id.

+ `string message`

	Chat message.

+ `string attrs`

	Chat message attributes in Json.

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


### Send Room Chat

	//-- Async Method
	public bool SendRoomChat(ActTimeDelegate callback, long roomId, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendRoomChat(out long mtime, long roomId, string message, string attrs = "", int timeout = 0);

Send text message in room.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long roomId`

	Room id.

+ `string message`

	Chat message.

+ `string attrs`

	Chat message attributes in Json.

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


### Send P2P Cmd

	//-- Async Method
	public bool SendCmd(ActTimeDelegate callback, long uid, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendCmd(out long mtime, long uid, string message, string attrs = "", int timeout = 0);

Send P2P text cmd.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long uid`

	Receiver user id.

+ `string message`

	Text cmd.

+ `string attrs`

	Text cmd attributes in Json.

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


### Send Group Cmd

	//-- Async Method
	public bool SendGroupCmd(ActTimeDelegate callback, long groupId, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendGroupCmd(out long mtime, long groupId, string message, string attrs = "", int timeout = 0);

Send text cmd in group.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long groupId`

	Group id.

+ `string message`

	Text cmd.

+ `string attrs`

	Text cmd attributes in Json.

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


### Send Room Cmd

	//-- Async Method
	public bool SendRoomCmd(ActTimeDelegate callback, long roomId, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendRoomCmd(out long mtime, long roomId, string message, string attrs = "", int timeout = 0);

Send text cmd in room.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long roomId`

	Room id.

+ `string message`

	Text cmd.

+ `string attrs`

	Text cmd attributes in Json.

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


### Send P2P Audio

	//-- Async Method
	public bool SendAudio(ActTimeDelegate callback, long uid, byte[] message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendAudio(out long mtime, long uid, byte[] message, string attrs = "", int timeout = 0);

Send P2P binary audio data.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long uid`

	Receiver user id.

+ `byte[] message`

	RTM audio data.  

+ `string attrs`

	Audio data attributes in Json.

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


### Send Group Audio

	//-- Async Method
	public bool SendGroupAudio(ActTimeDelegate callback, long groupId, byte[] message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendGroupAudio(out long mtime, long groupId, byte[] message, string attrs = "", int timeout = 0);

Send binary audio data in group.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long groupId`

	Group id.

+ `byte[] message`

	RTM audio data.  

+ `string attrs`

	Audio data attributes in Json.

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


### Send Room Audio

	//-- Async Method
	public bool SendRoomAudio(ActTimeDelegate callback, long roomId, byte[] message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendRoomAudio(out long mtime, long roomId, byte[] message, string attrs = "", int timeout = 0);

Send binary audio data in room.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long roomId`

	Room id.

+ `byte[] message`

	RTM audio data.  

+ `string attrs`

	Audio data attributes in Json.

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


### Get P2P Chat

	//-- Async Method
	public bool GetP2PChat(HistoryMessageDelegate callback, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);
	
	//-- Sync Method
	public int GetP2PChat(out HistoryMessageResult result, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);

Get history data for P2P chat, including text chat, text cmd and binary audio.

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


### Get Group Chat


	//-- Async Method
	public bool GetGroupChat(HistoryMessageDelegate callback, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);
	
	//-- Sync Method
	public int GetGroupChat(out HistoryMessageResult result, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);

Get history data for group chat, including text chat, text cmd and binary audio.

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


### Get Room Chat

	//-- Async Method
	public bool GetRoomChat(HistoryMessageDelegate callback, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);
	
	//-- Sync Method
	public int GetRoomChat(out HistoryMessageResult result, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);

Get history data for room chat, including text chat, text cmd and binary audio.

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

### Get Broadcast Chat


	//-- Async Method
	public bool GetBroadcastChat(HistoryMessageDelegate callback, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);
	
	//-- Sync Method
	public int GetBroadcastChat(out HistoryMessageResult result, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);

Get history data for broadcast chat, including text chat, text cmd and binary audio.

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


### Get Unread Chat Infos

	//-- Async Method
	public bool GetUnread(Action<List<long>, List<long>, int> callback, bool clear = false, int timeout = 0);
	
	//-- Sync Method
	public int GetUnread(out List<long> p2pList, out List<long> groupList, bool clear = false, int timeout = 0);

Get unread infos about which P2P sessions and groups have unread chat messages.

Parameters:

+ `Action<List<long>, List<long>, int> callback`

	Callabck for async method.  
	First `List<long>` is uids list which including the peer uids that have the unread chat messages;  
	Second `List<long>` is group ids list which including the groups that have the unread chat messages.  
	Thrid `int` is the error code indicating the calling is successful or the failed reasons.

+ `out List<long> p2pList`

	User ids list including the peer uids that have the unread chat messages.

+ `out List<long> groupList`

	Group ids list including the groups that have the unread chat messages.

+ `bool clear`

	Whether clear the unread hint after calling.

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


### Clear Unread infos

	//-- Async Method
	public bool ClearUnread(DoneDelegate callback, int timeout = 0);
	
	//-- Sync Method
	public int ClearUnread(int timeout = 0);

Clear unread infos.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

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


### Get Session


	//-- Async Method
	public bool GetSession(Action<List<long>, List<long>, int> callback, int timeout = 0);
	
	//-- Sync Method
	public int GetSession(out List<long> p2pList, out List<long> groupList, int timeout = 0);

Get chat sessions.

Parameters:

+ `Action<List<long>, List<long>, int> callback`

	Callabck for async method.  
	First `List<long>` is uids list which have chat sessions with current user;  
	Second `List<long>` is group ids list which members including current user.  
	Thrid `int` is the error code indicating the calling is successful or the failed reasons.

+ `out List<long> p2pList`

	User ids list which have chat sessions with current user.

+ `out List<long> groupList`

	Group ids list which members including current user.

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



### Delete Chat

	//-- Async Method
	public bool DeleteChat(DoneDelegate callback, long xid, long mid, int type, int timeout = 0);
	
	//-- Sync Method
	public int DeleteChat(long xid, long mid, int type, int timeout = 0);

Delete a sent chat message.

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


### Get Chat Message

	//-- Async Method
	public bool GetChat(Action<RetrievedMessage, int> callback, long xid, long mid, int type, int timeout = 0);
	
	//-- Sync Method
	public int GetChat(out RetrievedMessage retrievedMessage, long xid, long mid, int type, int timeout = 0);

Retrieve a sent chat message.

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

