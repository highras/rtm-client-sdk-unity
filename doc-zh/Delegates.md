# 回调代理类文档

### AuthDelegate

	public delegate void AuthDelegate(long projectId, long uid, bool successful, int errorCode);

参数:

+ `long pid`

	项目ID

+ `long uid`

	用户ID

+ `bool authStatus`

	* true: 登陆成功
	* false: 登录失败,失败原因请参照errorCode

+ `int errorCode`

	错误码，请参照[ErrorCode](../ErrorCode.md)

### DoneDelegate

	public delegate void DoneDelegate(int errorCode);

参数:

+ `int errorCode`

	0或者com.fpnn.ErrorCode.FPNN_EC_OK表示成功，其余错误码请参照[ErrorCode](ErrorCode.md).

### MessageIdDelegate

	public delegate void MessageIdDelegate(long messageId, int errorCode);

参数:

+ `long messageId`

    如果请求成功则为该消息id，失败则为0

+ `int errorCode`

    0或者com.fpnn.ErrorCode.FPNN_EC_OK表示成功，其余错误码请参照[ErrorCode](ErrorCode.md).

### HistoryMessageDelegate

	public delegate void HistoryMessageDelegate(int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode);

参数:

+ `int count`

	拉取到消息的数量

+ `long lastCursorId`

	当继续查询历史消息时，可以将该参数作为lastId参数输入辅助实现翻页功能。

+ `long beginMsec`

    正序查询时，为本次查询中最晚一条消息的时间。到序查询时为0。
    当继续查询历史消息时，可以将该参数作为beginMsec参数传入辅助实现翻页功能。

+ `long endMsec`

	倒序查询时，为本次查询中最早一条消息的时间。正序查询时为0。
    当继续查询历史消息时，可以将该参数作为endMsec参数传入辅助实现翻页功能。

+ `List<HistoryMessage> messages`

	历史消息列表，历史消息数据结构参见[HistoryMessage](RTMStructures.md#HistoryMessage).

+ `int errorCode`

    0或者com.fpnn.ErrorCode.FPNN_EC_OK表示成功，其余错误码请参照[ErrorCode](ErrorCode.md).


### GetFriendListDelegate

	public delegate void GetFriendListDelegate(HashSet<long> friendList, int errorCode);

参数:

+ `HashSet<long> friendList`

	好友用户ID列表

+ `int errorCode`

    0或者com.fpnn.ErrorCode.FPNN_EC_OK表示成功，其余错误码请参照[ErrorCode](ErrorCode.md).


### GetBlackListDelegate

	public delegate void GetBlackListDelegate(HashSet<long> blackList, int errorCode);

参数:

+ `HashSet<long> blackList`

	黑名单用户ID列表

+ `int errorCode`

    0或者com.fpnn.ErrorCode.FPNN_EC_OK表示成功，其余错误码请参照[ErrorCode](ErrorCode.md).

