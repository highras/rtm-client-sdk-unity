# RTM Client Unity SDK Files API Docs

# Index

[TOC]

### Send P2P File

	//-- Async Method
	public bool SendFile(MessageIdDelegate callback, long peerUid, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
	//-- Sync Method
	public int SendFile(out long messageId, long peerUid, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);

Send P2P file.

Parameters:

+ `MessageIdDelegate callback`

		public delegate void MessageIdDelegate(long messageId, int errorCode);

	Callabck for async method. Please refer [MessageIdDelegate](Delegates.md#MessageIdDelegate).

+ `out long messageId`

	Sent message id.

+ `long peerUid`

	Receiver user id.

+ `MessageType type`

	Message type for file.

+ `byte[] fileContent`

	File content.

+ `string filename`

	File name.

+ `string fileExtension`

	File extension.

+ `string attrs`

	Text file attributes in Json.

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


### Send Group File

	//-- Async Method
	public bool SendGroupFile(MessageIdDelegate callback, long groupId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
	//-- Sync Method
	public int SendGroupFile(out long messageId, long groupId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);

Send file in group.

Parameters:

+ `MessageIdDelegate callback`

		public delegate void MessageIdDelegate(long messageId, int errorCode);

	Callabck for async method. Please refer [MessageIdDelegate](Delegates.md#MessageIdDelegate).

+ `out long messageId`

	Sent message id.

+ `long groupId`

	Group id.

+ `MessageType type`

	Message type for file.

+ `byte[] fileContent`

	File content.

+ `string filename`

	File name.

+ `string fileExtension`

	File extension.

+ `string attrs`

	Text file attributes in Json.

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


### Send Room File

	//-- Async Method
	public bool SendRoomFile(MessageIdDelegate callback, long roomId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
	//-- Sync Method
	public int SendRoomFile(out long messageId, long roomId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);

Send file in room.

Parameters:

+ `MessageIdDelegate callback`

		public delegate void MessageIdDelegate(long messageId, int errorCode);

	Callabck for async method. Please refer [MessageIdDelegate](Delegates.md#MessageIdDelegate).

+ `out long messageId`

	Sent message id.

+ `long roomId`

	Room id.

+ `MessageType type`

	Message type for file.

+ `byte[] fileContent`

	File content.

+ `string filename`

	File name.

+ `string fileExtension`

	File extension.

+ `string attrs`

	Text file attributes in Json.

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

### Upload File

	//-- Async Method
	public bool UploadFile(Action<string, uint, int> callback, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
	//-- Sync Method
	public int UploadFile(out string url, out uint size, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);

Upload file.

Parameters:

+ `Action<string, uint, int> callback`

	Callabck for async method.  
	`string` is url of the uploaded file.  
	`uint` is size of the uploaded file.
	`int` is the error code indicating the calling is successful or the failed reasons.
		
+ `out string url`

	URL of the uploaded file.

+ `out uint size`

	Size of the uploaded file.

+ `MessageType type`

	Message type for file.

+ `byte[] fileContent`

	File content.

+ `string filename`

	File name.

+ `string fileExtension`

	File extension.

+ `string attrs`

	Text file attributes in Json.

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

