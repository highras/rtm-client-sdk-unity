# 文件接口文档

### 发送图片文件

	public bool SendImageFile(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
参数:

+ `MessageIdDelegate callback`

	异步调用回调，请参照[MessageIdDelegate](../Delegates.md#MessageIdDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

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

	public bool SendAudioFile(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
参数:

+ `MessageIdDelegate callback`

	异步调用回调，请参照[MessageIdDelegate](../Delegates.md#MessageIdDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

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


### 发送视频文件

	public bool SendVideoFile(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
参数:

+ `MessageIdDelegate callback`

	异步调用回调，请参照[MessageIdDelegate](../Delegates.md#MessageIdDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

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


### 发送语音消息

	public bool SendAudioMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
参数:

+ `MessageIdDelegate callback`

	异步调用回调，请参照[MessageIdDelegate](../Delegates.md#MessageIdDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

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


### 发送普通文件

	public bool SendNormalMessage(MessageIdDelegate callback, MessageCategory messageCategory, long id, byte[] fileContent, string filename, string fileExtension = "", string attrs = "", int timeout = 120);
	
参数:

+ `MessageIdDelegate callback`

	异步调用回调，请参照[MessageIdDelegate](../Delegates.md#MessageIdDelegate).

+ `MessageCategory messageCategory`

	消息种类，请参照[MessageCategory](../Delegates.md#MessageCategory).

+ `long id`

    消息接收ID，发送P2P消息时为对方用户ID，发送群组/房间消息时，为群组/房间ID。

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

