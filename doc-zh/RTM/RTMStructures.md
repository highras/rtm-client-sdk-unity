# 数据结构类文档

### CheckResult

    public class CheckResult
    {
        public int result;
        public List<int> tags;
    }

* `result`

    审核结果.

    接口文档:
    - 图片审核: [imageSpams -> result](https://docs.ilivedata.com/imagecheck/techdocs/respon/)
    - 音频审核: [result](https://docs.ilivedata.com/audiocheck/techdoc/callres/)
    - 直播音频审核: [audioSpams -> result](https://docs.ilivedata.com/audiocheck/livetechdoc/livecallres/)
    - 视频审核: [result](https://docs.ilivedata.com/videocheck/techdoc/callres/)
    - 直播视频审核: [result](https://docs.ilivedata.com/videocheck/livetechdoc/livecallres/)

* `tags`

    敏感信息分类标签。

    接口文档:
    - 图片审核: [tags](https://docs.ilivedata.com/imagecheck/techdocs/respon/)
    - 音频审核: [tags](https://docs.ilivedata.com/audiocheck/techdoc/callres/)
    - 直播音频审核: [tags](https://docs.ilivedata.com/audiocheck/livetechdoc/livecallres/)
    - 视频审核: [tags](https://docs.ilivedata.com/videocheck/techdoc/callres/)
    - 直播视频审核: [tags](https://docs.ilivedata.com/videocheck/livetechdoc/livecallres/)

### TextCheckResult

    public class TextCheckResult
    {
        public int result;
        public List<int> tags;
        public string text;
        public List<string> wlist;
    }

* `result`

    审核结果。 
    请参见 [textSpams -> result](https://docs.ilivedata.com/textcheck/technologydocument/http/)

* `tags`

    敏感信息分类标签。
    请参见 [tags](https://docs.ilivedata.com/textcheck/technologydocument/http/)

* `text`

    如果所有词都通过了审核，该字段为空。
    如果有敏感内容，该字段为过滤后的文本，敏感词会被替换为`*`

* `wlist`

    敏感词列表

### FileInfo

    public class FileInfo
    {
        //-- For all FileInfo instance
        public string url;
        public int size;

        //-- For image type
        public string surl;         //-- Thumb url, only for image type.

        //-- For RTM audio
        public bool isRTMAudio;
        public string language;
        public int duration;
    }

文件信息结构
* 当`isRTMAudio`为`true`时，`language`和`duration`可用.

### TranslatedInfo

    public class TranslatedInfo
    {
        public string sourceLanguage;
        public string targetLanguage;
        public string sourceText;
        public string targetText;
    }

### RTMMessage

    public class RTMMessage
    {
        public long fromUid;
        public long toId;
        public byte messageType;
        public long messageId;
        public string stringMessage;
        public byte[] binaryMessage;
        public string attrs;
        public long modifiedTime;
        public FileInfo fileInfo;
        public TranslatedInfo translatedInfo;
    }

在线收到服务端推送消息结构

* 当`messageType`为`MessageType.Chat`时，若开启了自动翻译功能，则`translatedInfo`是可用的, `stringMessage`会是翻译后的文本，`binaryMessage`则会是`null`;
* 当`messageType`为`MessageType.Cmd`时, `stringMessage`是可用的，而`binaryMessage`则会是`null`
* 当`messageType`为文件类型消息时, `fileInfo`是可用的, 而`binaryMessage`则会是`null`.  
* 如果消息内容是文本，`stringMessage`是可用的，而`binaryMessage`则是`null`
* 如果消息内容是二进制数据，`stringMessage`会是`null`,而`binaryMessage`会是可用的.

### RetrievedMessage

    public class RetrievedMessage
    {
        public long cursorId;
        public byte messageType;
        public string stringMessage;
        public byte[] binaryMessage;
        public string attrs;
        public long modifiedTime;
        public FileInfo fileInfo;
    }

拉取单条消息结构

### HistoryMessage

    public class HistoryMessage
    {
        public long cursorId;
        public long fromUid;
        public long toId;
        public byte messageType;
        public long messageId;
        public string stringMessage;
        public byte[] binaryMessage;
        public string attrs;
        public long modifiedTime;
        public FileInfo fileInfo;
        public TranslatedInfo translatedInfo;
    }

历史消息结构

### HistoryMessageResult

    public class HistoryMessageResult
    {
        public int count;
        public long lastCursorId;
        public long beginMsec;
        public long endMsec;
        public List<HistoryMessage> messages;
    }

历史消息列表结构

### Conversation

    public class Conversation
    {
        public long id;
        public ConversationType conversationType;
        public int unreadCount;
        public HistoryMessage lastMessage;
    }

P2P和群组会话结构