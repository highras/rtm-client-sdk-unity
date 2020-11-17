# RTM Client Unity SDK

[TOC]

## Depends

* [msgpack-csharp](https://github.com/highras/msgpack-csharp)

* [fpnn-sdk-unity](https://github.com/highras/fpnn-sdk-unity)

### Compatibility Version:

C# .Net Standard 2.0

### Capability in Funture

Encryption Capability, depending on FPNN C# SDK.

## Usage

### Using package

	using com.fpnn.rtm;

### Init

**Init MUST in the main thread.**

#### FPNN SDK Init (REQUIRED)

	using com.fpnn;
	ClientEngine.Init();
	ClientEngine.Init(Config config);

#### RTM SDK Init (REQUIRED)

	using com.fpnn.rtm;
	RTMControlCenter.Init();
	RTMControlCenter.Init(RTMConfig config);

### Create

	RTMClient client = new RTMClient(string endpoint, long pid, long uid, RTMQuestProcessor serverPushProcessor);

Please get your project params from RTM Console.

### RTMClient Instance Configure

#### Configure Properties:

	public int ConnectTimeout;
	public int QuestTimeout;
	public com.fpnn.common.ErrorRecorder ErrorRecorder;

### Login

	//-- Async interfaces
	public bool Login(AuthDelegate callback, string token, int timeout = 0);
	public bool Login(AuthDelegate callback, string token, Dictionary<string, string> attr, TranslateLanguage language = TranslateLanguage.None, int timeout = 0);

	//-- Sync interfaces
	public int RTMClient.Login(out bool ok, string token, int timeout = 0);
	public int RTMClient.Login(out bool ok, string token, Dictionary<string, string> attr, TranslateLanguage language = TranslateLanguage.None, int timeout = 0);

### Send messages

* Send P2P Message

		//-- Async interface
		public bool SendMessage(MessageIdDelegate callback, long uid, byte mtype, string message, string attrs = "", int timeout = 0);

		//-- Sync interface
		public int SendMessage(out long messageId, long uid, byte mtype, string message, string attrs = "", int timeout = 0);


* Send Group Message
	
		//-- Async interface
		public bool SendGroupMessage(MessageIdDelegate callback, long groupId, byte mtype, string message, string attrs = "", int timeout = 0);

		//-- Sync interface
		public int SendGroupMessage(out long messageId, long groupId, byte mtype, string message, string attrs = "", int timeout = 0);

* Send Room Message

		//-- Async interface
		public bool SendRoomMessage(MessageIdDelegate callback, long roomId, byte mtype, string message, string attrs = "", int timeout = 0);

		//-- Sync interface
		public int SendRoomMessage(out long messageId, long roomId, byte mtype, string message, string attrs = "", int timeout = 0);


### Send chat

* Send P2P Chat

		//-- Async interface
		public bool SendChat(MessageIdDelegate callback, long uid, string message, string attrs = "", int timeout = 0);

		//-- Sync interface
		public int SendChat(out long messageId, long uid, string message, string attrs = "", int timeout = 0);


* Send Group Chat
	
		//-- Async interface
		public bool SendGroupChat(MessageIdDelegate callback, long groupId, string message, string attrs = "", int timeout = 0);

		//-- Sync interface
		public int SendGroupChat(out long messageId, long groupId, string message, string attrs = "", int timeout = 0);

* Send Room Chat

		//-- Async interface
		public bool SendRoomChat(MessageIdDelegate callback, long roomId, string message, string attrs = "", int timeout = 0);

		//-- Sync interface
		public int SendRoomChat(out long messageId, long roomId, string message, string attrs = "", int timeout = 0);

### SDK Version

	C# `Console.WriteLine("com.fpnn.rtm.RTMConfig.SDKVersion");`
	Unity `Debug.Log("com.fpnn.rtm.RTMConfig.SDKVersion");`

## API docs

Please refer: [API docs](doc/API.md)

## Package release docs

Please refer: [Package release docs](doc/PackageRelease.md)

## Directory structure

* **\<rtm-client-sdk-unity\>/Assets/Plugins/fpnn**

	Codes of FPNN SDK.

* **\<rtm-client-sdk-unity\>/Assets/Plugins/rtm**

	Codes of RTM SDK.

* **\<rtm-client-sdk-unity\>/Assets/Examples**

	Examples codes for using RTM SDK.

* **\<rtm-client-sdk-unity\>/Assets/**

	* Main.cs:

		Entery of all examples.

	* ErrorRecorder.cs:

		Demo implementation of com.fpnn.common.ErrorRecorder for all examples.

	* RTMExampleQuestProcessor.cs:

		Demo implementation of com.fpnn.rtm.IRTMQuestProcessor for all examples.

* **\<rtm-client-sdk-unity\>/doc**

	API documents in markdown format.