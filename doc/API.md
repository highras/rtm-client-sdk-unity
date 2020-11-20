# RTM Client Unity SDK API Docs

# Index

[TOC]

## Current Version

	public static readonly string com.fpnn.rtm.RTMConfig.SDKVersion = "2.3.2";

## Init & Config SDK

Initialize and configure RTM SDK, and dependent FPNN SDK. 

PLease refer：[Init & Config FPNN SDK & RTM SDK](Init.md)

## RTM Client

### Overview

* **Namespace**

		com.fpnn.rtm

* **Declaration**

		public class RTMClient

### Constructors

	public RTMClient(string endpoint, long projectId, long uid, RTMQuestProcessor serverPushProcessor, bool autoRelogin = true)

* endpoint:

	RTM servers endpoint. Please get your project endpoint from RTM Console.

* pid:

	Project ID. Please get your project id from RTM Console.

* uid:

	User id assigned by your project.

* serverPushProcessor:

	Instance of events processor inheriting com.fpnn.rtm.RTMQuestProcessor.

	Please refer [Event Process](EventProcess.md)

* autoRelogin:

	Enable auto-relogin when connection lost after **first successful login** if user's token is available and user isn't forbidden.

### Properties

* **ConnectTimeout**

		public int ConnectTimeout { get; set; }

	Connecting/Login timeout for this RTMClient instance. Default is 0, meaning using the global config. 

* **public int QuestTimeout**

		public int QuestTimeout { get; set; }

	Quest timeout for this RTMClient instance. Default is 0, meaning using the global config.

* **public int Pid**

		public int Pid { get; }

	Project ID.

* **public int Uid**

		public int Uid { get; }

	User ID assigned by you project.

* **public RTMClient.ClientStatus Status**

		public RTMClient.ClientStatus Status { get; }

	RTM client current status.

	Values:

	+ ClientStatus.Closed
	+ ClientStatus.Connecting
	+ ClientStatus.Connected

* **public com.fpnn.common.ErrorRecorder ErrorRecorder**

		public com.fpnn.common.ErrorRecorder ErrorRecorder { set; }

	Config the ErrorRecorder instance for this RTMClient. Default is null.

### Structures

Please refer [RTM Structures](Structures.md)

### Delegates

Please refer [RTM Delegates](Delegates.md)

### Event Process

Please refer [Event Process](EventProcess.md)

### Methods

#### Login & Logout Functions

Please refer [Login & Logout Functions](LoginLogout.md)

#### RTM System Functions

Please refer [RTM System Functions](System.md)

#### Chat Functions

Please refer [Chat Functions](Chat.md)

#### Value-Added Functions

Please refer [Value-Added Functions](ValueAdded.md)

#### Messages Functions

Please refer [Messages Functions](Messages.md)

#### Audio Functions

Please refer [Audio Functions](Audio.md)

#### Files Functions

Please refer [Files Functions](Files.md)

#### Friends Functions

Please refer [Friends Functions](Friends.md)

#### Groups Functions

Please refer [Groups Functions](Groups.md)

#### Rooms Functions

Please refer [Rooms Functions](Rooms.md)

#### Users Functions

Please refer [Users Functions](Users.md)

#### Data Functions

Please refer [Data Functions](Data.md)

### Error Codes

[RTM SDK Error Codes](https://github.com/highras/rtm-client-sdk-unity/blob/master/Assets/Plugins/rtm/ErrorCode.cs)  
[FPNN SDK (Transport Layer) Error Codes](https://github.com/highras/rtm-client-sdk-unity/blob/master/Assets/Plugins/fpnn/ErrorCode.cs)
