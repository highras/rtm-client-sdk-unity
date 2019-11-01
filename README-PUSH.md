# fpnn rtm sdk unity #

#### PushService ####

* `RTMProcessor::AddPushService(string name, Action<IDictionary<string, object>> action)`: 添加推送回调
    * `name`: **(string)** 推送服务类型, 参考`RTMConfig.SERVER_PUSH`成员
    * `action`: **(Action(IDictionary(string, object)))** 回调方法

* `RTMProcessor::RemovePushService(string name)`: 删除推送回调
    * `name`: **(string)** 推送服务类型, 参考`RTMConfig.SERVER_PUSH`成员

* `RTMProcessor::HasPushService(string name)`: 是否存在推送回调
    * `name`: **(string)** 推送服务类型, 参考`RTMConfig.SERVER_PUSH`成员

> action push

* `kickoutroom`: RTMGate主动从Room移除
    * `data`: **(IDictionary(string, object))**
        * `data.rid`: **(long)** Room id

* `ping`: RTMGate主动ping
    * `data`: **(IDictionary(string, object))**

> message push

* `pushmsg`: RTMGate主动推送P2P业务消息
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.mtype`: **(byte)** 业务消息类型
        * `data.mid`: **(long)** 业务消息 id, 当前链接会话内唯一
        * `data.msg`: **(string)** 业务消息内容
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

* `pushgroupmsg`: RTMGate主动推送Group业务消息
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.gid`: **(long)** Group id
        * `data.mtype`: **(byte)** 业务消息类型
        * `data.mid`: **(long)** 业务消息 id, 当前链接会话内唯一
        * `data.msg`: **(string)** 业务消息内容
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

* `pushroommsg`: RTMGate主动推送Room业务消息
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.rid`: **(long)** Room id
        * `data.mtype`: **(byte)** 业务消息类型
        * `data.mid`: **(long)** 业务消息 id, 当前链接会话内唯一
        * `data.msg`: **(string)** 业务消息内容
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

* `pushbroadcastmsg`: RTMGate主动推送广播业务消息
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.mtype`: **(byte)** 业务消息类型
        * `data.mid`: **(long)** 业务消息 id, 当前链接会话内唯一
        * `data.msg`: **(string)** 业务消息内容
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

> file push

* `pushfile`: RTMGate主动推送P2P文件
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.mtype`: **(byte)** 文件类型, 请参考 `RTMConfig.FILE_TYPE` 成员
        * `data.mid`: **(long)** 业务文件消息 id, 当前链接会话内唯一
        * `data.msg`: **(string)** 文件获取地址(url)
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

* `pushgroupfile`: RTMGate主动推送Group文件
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.gid`: **(long)** Group id
        * `data.mtype`: **(byte)** 文件类型, 请参考 `RTMConfig.FILE_TYPE` 成员
        * `data.mid`: **(long)** 业务文件消息 id, 当前链接会话内唯一
        * `data.msg`: **(string)** 文件获取地址(url)
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

* `pushroomfile`: RTMGate主动推送Room文件
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.rid`: **(long)** Room id
        * `data.mtype`: **(byte)** 文件类型, 请参考 `RTMConfig.FILE_TYPE` 成员
        * `data.mid`: **(long)** 业务文件消息 id, 当前链接会话内唯一
        * `data.msg`: **(string)** 文件获取地址(url)
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

* `pushbroadcastfile`: RTMGate主动推送广播文件
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.mtype`: **(byte)** 文件类型, 请参考 `RTMConfig.FILE_TYPE` 成员
        * `data.mid`: **(long)** 业务文件消息 id, 当前链接会话内唯一
        * `data.msg`: **(string)** 文件获取地址(url)
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

> chat push

* `pushchat`: RTMGate主动推送P2P聊天消息
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.mid`: **(long)** 聊天消息 id, 当前链接会话内唯一
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**
        * `data.msg`: **(JsonString)** 聊天消息
            * `source`: **(string)** 原始内容语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
            * `target`: **(string)** 翻译后的语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
            * `sourceText`: **(string)** 原始聊天消息
            * `targetText`: **(string)** 翻译后的聊天消息

* `pushgroupchat`: RTMGate主动推送Group聊天消息
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.gid`: **(long)** Group id
        * `data.mid`: **(long)** 聊天消息 id, 当前链接会话内唯一
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**
        * `data.msg`: **(JsonString)** 聊天消息
            * `source`: **(string)** 原始内容语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
            * `target`: **(string)** 翻译后的语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
            * `sourceText`: **(string)** 原始聊天消息
            * `targetText`: **(string)** 翻译后的聊天消息

* `pushroomchat`: RTMGate主动推送Room聊天消息
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.rid`: **(long)** Room id
        * `data.mid`: **(long)** 聊天消息 id, 当前链接会话内唯一
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**
        * `data.msg`: **(JsonString)** 聊天消息
            * `source`: **(string)** 原始内容语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
            * `target`: **(string)** 翻译后的语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
            * `sourceText`: **(string)** 原始聊天消息
            * `targetText`: **(string)** 翻译后的聊天消息

* `pushbroadcastchat`: RTMGate主动推送广播聊天消息
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.mid`: **(long)** 聊天消息 id, 当前链接会话内唯一
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**
        * `data.msg`: **(JsonString)** 聊天消息
            * `source`: **(string)** 原始内容语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
            * `target`: **(string)** 翻译后的语言类型, 参考`RTMConfig.TRANS_LANGUAGE`成员
            * `sourceText`: **(string)** 原始聊天消息
            * `targetText`: **(string)** 翻译后的聊天消息

> audio push

* `pushaudio`: RTMGate主动推送P2P聊天语音
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.mid`: **(long)** 语音消息 id, 当前链接会话内唯一
        * `data.msg`: **(byte[])** 聊天语音数据
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

* `pushgroupaudio`: RTMGate主动推送Group聊天语音
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.gid`: **(long)** Group id
        * `data.msg`: **(byte[])** 聊天语音数据
        * `data.mid`: **(long)** 语音消息 id, 当前链接会话内唯一
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

* `pushroomaudio`: RTMGate主动推送Room聊天语音
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.rid`: **(long)** Room id语音数据
        * `data.msg`: **(byte[])** 聊天语音数据
        * `data.mid`: **(long)** 语音消息 id, 当前链接会话内唯一
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

* `pushbroadcastaudio`: RTMGate主动推送广播聊天语音
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.msg`: **(byte[])** 聊天语音数据
        * `data.mid`: **(long)** 语音消息 id, 当前链接会话内唯一
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**

> cmd push

* `pushcmd`: RTMGate主动推送聊天命令
    * `data`: **(IDictionary(string, object))**
        * `data.from`: **(long)** 发送者 id
        * `data.msg`: **(string)** 命令内容
        * `data.mid`: **(long)** 命令消息 id, 当前链接会话内唯一
        * `data.attrs`: **(string)** 发送时附加的自定义内容
        * `data.mtime`: **(long)**