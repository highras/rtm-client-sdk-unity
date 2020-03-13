# RTM Client Unity SDK System API Docs

# Index

[TOC]

### Kickout

	//-- Async Method
	public bool Kickout(DoneDelegate callback, string endpoint, int timeout = 0);
	
	//-- Sync Method
	public int Kickout(string endpoint, int timeout = 0);

Kickout another session in multi-login mode.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `string endpoint`

	Endpoint of another session, which can be retrieved by `Get Attributes` methods.

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


### Add Attributes

	//-- Async Method
	public bool AddAttributes(DoneDelegate callback, Dictionary<string, string> attrs, int timeout = 0);
	
	//-- Sync Method
	public int AddAttributes(Dictionary<string, string> attrs, int timeout = 0);

Add session or connection attributes. That can be fetch by all sessions of the user.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `Dictionary<string, string> attrs`

	Session or connection attributes. That can be fetch by all sessions of the user.

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

### Get Attributes

	//-- Async Method
	public bool GetAttributes(Action<List<Dictionary<string, string>>, int> callback, int timeout = 0);
	
	//-- Sync Method
	public int GetAttributes(out List<Dictionary<string, string>> attributes, int timeout = 0);

Get session or connection attributes.

Parameters:

+ `Action<List<Dictionary<string, string>>, int> callback`

	Callabck for async method.  
	First `List<Dictionary<string, string>>` is sessions' attributes list. Detail please refer the `out List<Dictionary<string, string>> attributes` parameter for sync method;  
	Second `int` is the error code indicating the calling is successful or the failed reasons.

+ `out List<Dictionary<string, string>> attributes`

	Sessions' attributes.  
	If multi-login is enabled, each session will has its attributes dictionary.  
	Each attributes dictionary has two extra keys:

		* **ce**: the endpoint of this session
		* **login**: the UTC timestamp of this session login
		
	If the attributes dictionary belong to the current session, key **my** will appear.

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

### Add Debug Log

	//-- Async Method
	public bool AddDebugLog(DoneDelegate callback, string message, string attrs, int timeout = 0);
	
	//-- Sync Method
	public int AddDebugLog(string message, string attrs, int timeout = 0);

Add debug log.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `string message`

	log text.

+ `string attrs`

	Attributes for log text.

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


### Add Device

	//-- Async Method
	public bool AddDevice(DoneDelegate callback, string appType, string deviceToken, int timeout = 0);
	
	//-- Sync Method
	public int AddDevice(string appType, string deviceToken, int timeout = 0);

Add device infos.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `string appType`

	Application information.

+ `string deviceToke`

	Device ASPN or FCM push token.

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


### Remove Device

	//-- Async Method
	public bool RemoveDevice(DoneDelegate callback, string deviceToken, int timeout = 0);
	
	//-- Sync Method
	public int RemoveDevice(string deviceToken, int timeout = 0);

Remove device infos.

Parameters:

+ `DoneDelegate callback`

		public delegate void DoneDelegate(int errorCode);

	Callabck for async method. Please refer [DoneDelegate](Delegates.md#DoneDelegate).

+ `string deviceToke`

	Device ASPN or FCM push token.

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
