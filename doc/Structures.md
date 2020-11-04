# RTM Client Unity SDK API Docs: Structures

# Index

[TOC]

### CheckResult

    public class CheckResult
    {
        public int result;
        public List<int> tags;
    }

* `result`

    Audit's result.

    Chinese documents:

    Image Review: [imageSpams -> result](https://docs.ilivedata.com/imagecheck/techdocs/respon/)

    Audio Checking: [result](https://docs.ilivedata.com/audiocheck/techdoc/callres/)

    Audio Checking (live): [audioSpams -> result](https://docs.ilivedata.com/audiocheck/livetechdoc/livecallres/)

    Video Review: [result](https://docs.ilivedata.com/videocheck/techdoc/callres/)

    Video Review (live): [result](https://docs.ilivedata.com/videocheck/livetechdoc/livecallres/)

* `tags`

    Classified information for found sensitive information.

    Chinese documents:

    Image Review: [tags](https://docs.ilivedata.com/imagecheck/techdocs/respon/)

    Audio Checking: [tags](https://docs.ilivedata.com/audiocheck/techdoc/callres/)

    Audio Checking (live): [tags](https://docs.ilivedata.com/audiocheck/livetechdoc/livecallres/)

    Video Review: [tags](https://docs.ilivedata.com/videocheck/techdoc/callres/)

    Video Review (live): [tags](https://docs.ilivedata.com/videocheck/livetechdoc/livecallres/)

### TextCheckResult

    public class TextCheckResult
    {
        public int result;
        public List<int> tags;
        public string text;
        public List<string> wlist;
    }

* `result`

    Audit's result.  
    Please refer [textSpams -> result](https://docs.ilivedata.com/textcheck/technologydocument/http/) (Chinese document).

* `tags`

    Classified information for found sensitive words.  
    Please refer [tags](https://docs.ilivedata.com/textcheck/technologydocument/http/) (Chinese document).

* `text`

    If all words is passed the audit, the field is null or empty.  
    If any sensitive word is found, this field is the filtered text. The sensitive words are repleaced by `*`.

* `wlist`

    Sensitive words list.

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

* `url` & `size` are available for every FileInfo instance.
* `surl` is available for MessageType.ImageFile.
* `language` & `duration` are available when `isRTMAudio` is `true`.

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

Using for receiving server pushed messages with interfaces of class RTMQuestProcessor.

When `messageType == (byte)MessageType.Chat`, `translatedInfo` will be assigned, `stringMessage` will be the translated messsge or original message if translation is not enabled, and `binaryMessage` will be null;   
When `messageType == (byte)MessageType.Cmd`, `stringMessage` will be assigned and `binaryMessage` will be null,  
When `messageType` is a kinds of File types, `fileInfo` will be assigned, and `binaryMessage` will be null.  
In other cases, if message data is text data, `stringMessage` will be assigned and `binaryMessage` will be null,  
If message data is binary data, `stringMessage` will be null and `binaryMessage` will be assigned.

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

Using for retrieving chat or messages.

The fields are same as the `class RTMMessage`.

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

Using for history message result.

The fields are same as the `class RTMMessage`.

### HistoryMessageResult

    public class HistoryMessageResult
    {
        public int count;
        public long lastCursorId;
        public long beginMsec;
        public long endMsec;
        public List<HistoryMessage> messages;
    }

Using as the result of history message & chat methods.

When calling history message or history chat methods for fetching subsequent message or chat data, please using the corresponding fields in the result.