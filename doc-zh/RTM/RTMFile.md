# 文件接口文档

### 发送文件

	public bool SendFile(MessageIdDelegate callback, MessageCategory messageCategory, long id, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
参数:

+ `MessageIdDelegate callback`

	异步调用回调，请参照[MessageIdDelegate](../Delegates.md#MessageIdDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

+ `MessageType type`

	消息类型

+ `byte[] fileContent`

    文件二进制内容

+ `string filename`

    文件名

+ `string fileExtension`

    文件扩展名

+ `string attrs`

	消息自定义属性信息。

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.

### 发送语音文件

	public bool SendAudioMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, RTMAudioData audioData, string attrs = "", int timeout = 120);
	
参数:

+ `MessageIdDelegate callback`

	异步调用回调，请参照[MessageIdDelegate](../Delegates.md#MessageIdDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

+ `RTMAudioData audioData`

    音频文件数据结构，请参照[RTMAudioRecorder](RTMAudioRecorder.md)

+ `string attrs`

	消息自定义属性信息。

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 上传文件

	public bool UploadFile(Action<string, uint, int> callback, MessageType type, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
参数:

+ `Action<string, uint, int> callback`

	异步调用回调.第一个参数`string`为url，第二个参数`uint`为文件大小，第三个参数`int`为错误码

+ `MessageType type`

	消息类型

+ `byte[] fileContent`

    文件二进制内容

+ `string filename`

    文件名

+ `string fileExtension`

    文件扩展名

+ `string attrs`

	消息自定义属性信息。

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.