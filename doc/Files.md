# RTM Client Unity SDK Files API Docs

# Index

[TOC]

### Send P2P File

	//-- Async Method
	public bool SendFile(ActTimeDelegate callback, long peerUid, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);
	public bool SendFile(ActTimeDelegate callback, long peerUid, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);
	
	//-- Sync Method
	public int SendFile(out long mtime, long peerUid, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);
	public int SendFile(out long mtime, long peerUid, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);

Send P2P file.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long peerUid`

	Receiver user id.

+ `byte mtype`

	Message type for file. MUST in the range: [40 - 50].
	* 50: Generic file. Server maybe change this value to more suitable values.
	* 40: Pictures.
	* 41: Audio.
	* 42: Video.

+ `byte[] fileContent`

	File content.

+ `string filename`

	File name.

+ `string fileExtension`

	File extension.

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
	public bool SendGroupFile(ActTimeDelegate callback, long groupId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);
	public bool SendGroupFile(ActTimeDelegate callback, long groupId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);
	
	//-- Sync Method
	public int SendGroupFile(out long mtime, long groupId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);
	public int SendGroupFile(out long mtime, long groupId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);

Send file in group.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long groupId`

	Group id.

+ `byte mtype`

	Message type for file. MUST in the range: [40 - 50].
	* 50: Generic file. Server maybe change this value to more suitable values.
	* 40: Pictures.
	* 41: Audio.
	* 42: Video.

+ `byte[] fileContent`

	File content.

+ `string filename`

	File name.

+ `string fileExtension`

	File extension.

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
	public bool SendRoomFile(ActTimeDelegate callback, long roomId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);
	public bool SendRoomFile(ActTimeDelegate callback, long roomId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);
	
	//-- Sync Method
	public int SendRoomFile(out long mtime, long roomId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);
	public int SendRoomFile(out long mtime, long roomId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120);

Send file in room.

Parameters:

+ `ActTimeDelegate callback`

		public delegate void ActTimeDelegate(long mtime, int errorCode);

	Callabck for async method. Please refer [ActTimeDelegate](Delegates.md#ActTimeDelegate).

+ `out long mtime`

	Sending completed time.

+ `long roomId`

	Room id.

+ `byte mtype`

	Message type for file. MUST in the range: [40 - 50].
	* 50: Generic file. Server maybe change this value to more suitable values.
	* 40: Pictures.
	* 41: Audio.
	* 42: Video.

+ `byte[] fileContent`

	File content.

+ `string filename`

	File name.

+ `string fileExtension`

	File extension.

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

