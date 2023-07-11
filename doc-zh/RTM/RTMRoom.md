# 房间接口文档

### 进入房间

	public bool EnterRoom(DoneDelegate callback, long roomId, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

	房间ID.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 离开房间

	public bool LeaveRoom(DoneDelegate callback, long roomId, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

	房间ID.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取房间成员数量

	public bool GetRoomMemberCount(Action<Dictionary<long, int>, int> callback, HashSet<long> roomIds, int timeout = 0);
	
参数:

+ `Action<int, int> callback`

	异步调用回调，第一个`int`参数为房间成员数，第二个`int`为错误码.

+ `long roomId`

	房间ID.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取房间成员列表

	public bool GetRoomMembers(Action<HashSet<long>, int> callback, long roomId, int timeout = 0);
	
参数:

+ `Action<HashSet<long>, int> callback`

	异步调用回调，第一个`HashSet<long>`参数为房间成员列表,第二个`int`参数为错误码.

+ `long roomId`

	房间ID.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取房间信息

	public bool GetRoomInfo(Action<string, string, int> callback, long roomId, int timeout = 0);
	
参数:

+ `Action<string, string, int> callback`

	异步调用回调，第一个参数`string`为公开信息，第二个参数`string`为内部信息，第三个参数`int`为错误码.

+ `long roomId`

	房间ID.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取房间公开信息

	public bool GetRoomsPublicInfo(Action<Dictionary<long, string>, int> callback, HashSet<long> roomIds, int timeout = 0);
	
参数:

+ `Action<Dictionary<long, string>, int> callback`

	异步调用回调，第一个参数`Dictionary<long, string>`为公开信息，key为群组ID，value为公开信息内容；第二个参数`int`为错误码.

+ `HashSet<long> roomIds`

	房间ID列表.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 设置房间信息

	public bool SetRoomInfo(DoneDelegate callback, long roomId, string publicInfo = null, string privateInfo = null, int timeout = 0);
	
参数:

+ `DoneDelegate callback`

	异步调用回调，请参照[DoneDelegate](../Delegates.md#DoneDelegate).

+ `long roomId`

	房间ID.

+ `string publicInfo`

    公开信息

+ `string privateInfo`

    内部信息

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.


### 获取用户所在房间列表

	public bool GetUserRooms(Action<HashSet<long>, int> callback, int timeout = 0);
	
参数:

+ `Action<HashSet<long>, int> callback`

	异步调用回调，第一个`HashSet<long>`参数为房间ID列表，第二个`int`为错误码.

+ `int timeout`

	超时时间，0代表使用默认配置.

返回值:

+ true: 请求发送成功
+ false: 请求发送失败.

