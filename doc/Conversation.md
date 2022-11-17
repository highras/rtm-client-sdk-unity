# RTM Client Unity SDK Conversation API Docs

# Index

[TOC]

### Get P2P Conversation List

	//-- Async Method
	public bool GetP2PConversationList(Action<List<Conversation>, int> callback, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

	
	//-- Sync Method
	public int GetP2PConversationList(out List<Conversation> conversationList, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

Get P2P conversation list.

Parameters:

+ `Action<List<Conversation>, int> callback`

	Callabck for async method.  
	First `List<Conversation>` is conversation data, please refer [Conversation](Structures.md#Conversation);  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out List<Conversation> conversationList`

	P2P conversation list.

+ `HashSet<byte> mTypes`

	Which message types will be checked. If set is null or empty, only chat messages, cmd messages and file messages will be checked.

+ `long startTime`

	The timestamp in millisecond which indicated the start time to calculate the unread messages. `0` means using the last offline/logout time. 

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


### Get P2P Unread Conversation List

	//-- Async Method
	public bool GetP2PUnreadConversationList(Action<List<Conversation>, int> callback, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

	
	//-- Sync Method
	public int GetP2PUnreadConversationList(out List<Conversation> conversationList, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

Get P2P unread conversation list.

Parameters:

+ `Action<List<Conversation>, int> callback`

	Callabck for async method.  
	First `List<Conversation>` is conversation data, please refer [Conversation](Structures.md#Conversation);  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out List<Conversation> conversationList`

	P2P unread conversation list.

+ `HashSet<byte> mTypes`

	Which message types will be checked. If set is null or empty, only chat messages, cmd messages and file messages will be checked.

+ `long startTime`

	The timestamp in millisecond which indicated the start time to calculate the unread messages. `0` means using the last offline/logout time. 

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


### Get Group Conversation List

	//-- Async Method
	public bool GetGroupConversationList(Action<List<Conversation>, int> callback, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

	
	//-- Sync Method
	public int GetGroupConversationList(out List<Conversation> conversationList, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

Get group conversation list.

Parameters:

+ `Action<List<Conversation>, int> callback`

	Callabck for async method.  
	First `List<Conversation>` is conversation data, please refer [Conversation](Structures.md#Conversation);  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out List<Conversation> conversationList`

	Group conversation list.

+ `HashSet<byte> mTypes`

	Which message types will be checked. If set is null or empty, only chat messages, cmd messages and file messages will be checked.

+ `long startTime`

	The timestamp in millisecond which indicated the start time to calculate the unread messages. `0` means using the last offline/logout time. 

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


### Get Group Unread Conversation List

	//-- Async Method
	public bool GetGroupUnreadConversationList(Action<List<Conversation>, int> callback, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

	
	//-- Sync Method
	public int GetGroupUnreadConversationList(out List<Conversation> conversationList, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

Get group unread conversation list.

Parameters:

+ `Action<List<Conversation>, int> callback`

	Callabck for async method.  
	First `List<Conversation>` is conversation data, please refer [Conversation](Structures.md#Conversation);  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out List<Conversation> conversationList`

	Group unread conversation list.

+ `HashSet<byte> mTypes`

	Which message types will be checked. If set is null or empty, only chat messages, cmd messages and file messages will be checked.

+ `long startTime`

	The timestamp in millisecond which indicated the start time to calculate the unread messages. `0` means using the last offline/logout time. 

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

### Get Unread Conversation List

	//-- Async Method
	public bool GetUnreadConversationList(Action<List<Conversation>, List<Conversation>, int> callback, bool clear = true, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

	
	//-- Sync Method
	public int GetUnreadConversationList(out List<Conversation> groupConversationList, out List<Conversation> p2pConversationList, bool clear = true, HashSet<byte> mTypes = null, long startTime = 0, int timeout = 0);

Get unread conversation list.

Parameters:

+ `Action<List<Conversation>, int> callback`

	Callabck for async method.  
	First `List<Conversation>` is group conversation data, please refer [Conversation](Structures.md#Conversation);  
	Second `List<Conversation>` is p2p conversation data, please refer [Conversation](Structures.md#Conversation);  
	Third `int` is the error code indicating the calling is successful or the failed reasons.

+ `out List<Conversation> groupConversationList`

	Group unread conversation list.

+ `out List<Conversation> p2pConversationList`

	P2P unread conversation list.

+ `HashSet<byte> mTypes`

	Which message types will be checked. If set is null or empty, only chat messages, cmd messages and file messages will be checked.

+ `long startTime`

	The timestamp in millisecond which indicated the start time to calculate the unread messages. `0` means using the last offline/logout time. 

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

