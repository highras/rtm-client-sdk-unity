# RTM Client Unity SDK Chat API Docs

# Index

[TOC]

### Send P2P Chat

	//-- Async Method
	public bool SendChat(MessageIdDelegate callback, long uid, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendChat(out long messageId, long uid, string message, string attrs = "", int timeout = 0);

Send P2P text message.

Parameters:

+ `MessageIdDelegate callback`

		public delegate void MessageIdDelegate(long messageId, int errorCode);

	Callabck for async method. Please refer [MessageIdDelegate](Delegates.md#MessageIdDelegate).

+ `out long messageId`

	Sent message id.

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
	public bool SendGroupChat(MessageIdDelegate callback, long groupId, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendGroupChat(out long messageId, long groupId, string message, string attrs = "", int timeout = 0);

Send text message in group.

Parameters:

+ `MessageIdDelegate callback`

		public delegate void MessageIdDelegate(long messageId, int errorCode);

	Callabck for async method. Please refer [MessageIdDelegate](Delegates.md#MessageIdDelegate).

+ `out long messageId`

	Sent message id.

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
	public bool SendRoomChat(MessageIdDelegate callback, long roomId, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendRoomChat(out long messageId, long roomId, string message, string attrs = "", int timeout = 0);

Send text message in room.

Parameters:

+ `MessageIdDelegate callback`

		public delegate void MessageIdDelegate(long messageId, int errorCode);

	Callabck for async method. Please refer [MessageIdDelegate](Delegates.md#MessageIdDelegate).

+ `out long messageId`

	Sent message id.

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
	public bool SendCmd(MessageIdDelegate callback, long uid, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendCmd(out long messageId, long uid, string message, string attrs = "", int timeout = 0);

Send P2P text cmd.

Parameters:

+ `MessageIdDelegate callback`

		public delegate void MessageIdDelegate(long messageId, int errorCode);

	Callabck for async method. Please refer [MessageIdDelegate](Delegates.md#MessageIdDelegate).

+ `out long messageId`

	Sent message id.

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
	public bool SendGroupCmd(MessageIdDelegate callback, long groupId, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendGroupCmd(out long messageId, long groupId, string message, string attrs = "", int timeout = 0);

Send text cmd in group.

Parameters:

+ `MessageIdDelegate callback`

		public delegate void MessageIdDelegate(long messageId, int errorCode);

	Callabck for async method. Please refer [MessageIdDelegate](Delegates.md#MessageIdDelegate).

+ `out long messageId`

	Sent message id.

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
	public bool SendRoomCmd(MessageIdDelegate callback, long roomId, string message, string attrs = "", int timeout = 0);
	
	//-- Sync Method
	public int SendRoomCmd(out long messageId, long roomId, string message, string attrs = "", int timeout = 0);

Send text cmd in room.

Parameters:

+ `MessageIdDelegate callback`

		public delegate void MessageIdDelegate(long messageId, int errorCode);

	Callabck for async method. Please refer [MessageIdDelegate](Delegates.md#MessageIdDelegate).

+ `out long messageId`

	Sent message id.

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


### Get P2P Chat

	//-- Async Method
	public bool GetP2PChat(HistoryMessageDelegate callback, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);
	
	//-- Sync Method
	public int GetP2PChat(out HistoryMessageResult result, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0);

Get history data for P2P chat, including text chat, text cmd and file message.

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

Get history data for group chat, including text chat, text cmd and file message.

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

Get history data for room chat, including text chat, text cmd and file message.

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

Get history data for broadcast chat, including text chat, text cmd and file message.

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


### Get P2P Unread Infos

	//-- Async Method
	public bool GetP2PUnread(Action<Dictionary<long, int>, int> callback, HashSet<long> uids, HashSet<byte> mTypes = null, int timeout = 0);
	public bool GetP2PUnread(Action<Dictionary<long, int>, int> callback, HashSet<long> uids, long startTime, HashSet<byte> mTypes = null, int timeout = 0);
	public bool GetP2PUnread(Action<Dictionary<long, int>, Dictionary<long, long>, int> callback, HashSet<long> uids, HashSet<byte> mTypes = null, int timeout = 0);
	public bool GetP2PUnread(Action<Dictionary<long, int>, Dictionary<long, long>, int> callback, HashSet<long> uids, long startTime, HashSet<byte> mTypes = null, int timeout = 0);
	
	//-- Sync Method
	public int GetP2PUnread(out Dictionary<long, int> unreadDictionary, HashSet<long> uids, HashSet<byte> mTypes = null, int timeout = 0);
	public int GetP2PUnread(out Dictionary<long, int> unreadDictionary, HashSet<long> uids, long startTime, HashSet<byte> mTypes = null, int timeout = 0);
	public int GetP2PUnread(out Dictionary<long, int> unreadDictionary, out Dictionary<long, long> lastUnreadTimestampDictionary, HashSet<long> uids, HashSet<byte> mTypes = null, int timeout = 0);
	public int GetP2PUnread(out Dictionary<long, int> unreadDictionary, out Dictionary<long, long> lastUnreadTimestampDictionary, HashSet<long> uids, long startTime, HashSet<byte> mTypes = null, int timeout = 0);

Get P2P unread infos when indicated P2P sessions have unread messages.

Parameters:

+ `Action<Dictionary<long, int>, int> callback`

	Callabck for async method.  
	First `Dictionary<long, int>` is the unread dictionary which key is the peer's uid who has some unread messages, and value is the count of the unread messages;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `Action<Dictionary<long, int>, Dictionary<long, long>, int> callback`

	Callabck for async method.  
	First `Dictionary<long, int>` is the unread dictionary which key is the peer's uid who has some unread messages, and value is the count of the unread messages;  
	Second `Dictionary<long, long>` is the last unread message timestamp dictionary which key is the peer's uid who has some unread messages, and value is the timestamp in milliseconds of the latest unread message;  
	Thrid `int` is the error code indicating the calling is successful or the failed reasons.

+ `out Dictionary<long, int> unreadDictionary`

	Unread dictionary which key is the peer's uid who has some unread messages, and value is the count of the unread messages.

+ `out Dictionary<long, long> lastUnreadTimestampDictionary`

	Last unread message timestamp dictionary which key is the peer's uid who has some unread messages, and value is the timestamp in milliseconds of the latest unread message.

+ `HashSet<long> uids`

	The uids of the peer in P2P sessions which will be wanted to be checked.

+ `long startTime`

	The timestamp in millisecond which indicated the start time to calculate the unread messages. `0` means using the last offline/logout time. 

+ `HashSet<byte> mTypes`

	Which message types will be checked. If set is null or empty, only chat messages, cmd messages and file messages will be checked.

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


### Get Group Unread Infos

	//-- Async Method
	public bool GetGroupUnread(Action<Dictionary<long, int>, int> callback, HashSet<long> groupIds, HashSet<byte> mTypes = null, int timeout = 0);
	public bool GetGroupUnread(Action<Dictionary<long, int>, int> callback, HashSet<long> groupIds, long startTime, HashSet<byte> mTypes = null, int timeout = 0);
	public bool GetGroupUnread(Action<Dictionary<long, int>, Dictionary<long, long>, int> callback, HashSet<long> groupIds, HashSet<byte> mTypes = null, int timeout = 0);
	public bool GetGroupUnread(Action<Dictionary<long, int>, Dictionary<long, long>, int> callback, HashSet<long> groupIds, long startTime, HashSet<byte> mTypes = null, int timeout = 0);
	
	//-- Sync Method
	public int GetGroupUnread(out Dictionary<long, int> unreadDictionary, HashSet<long> groupIds, HashSet<byte> mTypes = null, int timeout = 0);
	public int GetGroupUnread(out Dictionary<long, int> unreadDictionary, HashSet<long> groupIds, long startTime, HashSet<byte> mTypes = null, int timeout = 0);
	public int GetGroupUnread(out Dictionary<long, int> unreadDictionary, out Dictionary<long, long> lastUnreadTimestampDictionary, HashSet<long> groupIds, HashSet<byte> mTypes = null, int timeout = 0);
	public int GetGroupUnread(out Dictionary<long, int> unreadDictionary, out Dictionary<long, long> lastUnreadTimestampDictionary, HashSet<long> groupIds, long startTime, HashSet<byte> mTypes = null, int timeout = 0);

Get group unread infos when indicated group sessions have unread messages.

Parameters:

+ `Action<Dictionary<long, int>, int> callback`

	Callabck for async method.  
	First `Dictionary<long, int>` is the unread dictionary which key is the group id which has some unread messages, and value is the count of the unread messages;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `Action<Dictionary<long, int>, Dictionary<long, long>, int> callback`

	Callabck for async method.  
	First `Dictionary<long, int>` is the unread dictionary which key is the group id which has some unread messages, and value is the count of the unread messages;  
	Second `Dictionary<long, long>` is the last unread message timestamp dictionary which key is the group id which has some unread messages, and value is the timestamp in milliseconds of the latest unread message;  
	Thrid `int` is the error code indicating the calling is successful or the failed reasons.

+ `out Dictionary<long, int> unreadDictionary`

	Unread dictionary which key is the group id which has some unread messages, and value is the count of the unread messages.

+ `out Dictionary<long, int> lastUnreadTimestampDictionary`

	Last unread message timestamp dictionary which key is the group id which has some unread messages, and value is the timestamp in milliseconds of the latest unread message.

+ `HashSet<long> groupIds`

	The ids of groups which want to be checked.

+ `long startTime`

	The timestamp in millisecond which indicated the start time to calculate the unread messages. `0` means using the last offline/logout time. 

+ `HashSet<byte> mTypes`

	Which message types will be checked. If set is null or empty, only chat messages, cmd messages and file messages will be checked.

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
	public bool DeleteChat(DoneDelegate callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0);
	
	//-- Sync Method
	public int DeleteChat(long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0);

Delete a sent chat message.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `fromUid`

	Uid of the chat sender, which chat is wanted to be deleted.

+ `toId`

	If the chat is P2P chat, `toId` means the uid of peer;  
	If the chat is group chat, `toId` means the `groupId`;  
	If the chat is room chat, `toId` means the `roomId`.

+ `messageId`

	Message id for the chat message which wanted to be deleted.

+ `messageCategory`

	MessageCategory enumeration.

	Can be MessageCategory.P2PMessage, MessageCategory.GroupMessage, MessageCategory.RoomMessage.


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
	public bool GetChat(Action<RetrievedMessage, int> callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0);
	
	//-- Sync Method
	public int GetChat(out RetrievedMessage retrievedMessage, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0);

Retrieve a sent chat message.

Parameters:

+ `Action<RetrievedMessage, int> callback`

	Callabck for async method.  
	First `RetrievedMessage` is retrieved data, please refer [RetrievedMessage](Structures.md#RetrievedMessage);  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out RetrievedMessage retrievedMessage`

	The retrieved data, please refer [RetrievedMessage](Structures.md#RetrievedMessage).

+ `fromUid`

	Uid of the chat sender, which chat is wanted to be retrieved.

+ `toId`

	If the chat is P2P chat, `toId` means the uid of peer;  
	If the chat is group chat, `toId` means the `groupId`;  
	If the chat is room chat, `toId` means the `roomId`;  
	If the chat is broadcast chat, `toId` is `0`.

+ `messageId`

	Message id for the chat message which wanted to be retrieved.

+ `messageCategory`

	MessageCategory enumeration.

	Can be MessageCategory.P2PMessage, MessageCategory.GroupMessage, MessageCategory.RoomMessage, MessageCategory.BroadcastMessage.

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

