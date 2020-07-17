# RTM Client Unity SDK API Docs: Structures

# Index

[TOC]

### AudioInfo

    public class AudioInfo
    {
        public string sourceLanguage;
        public string recognizedLanguage;
        public string recognizedText;
        public int duration;
    }

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
        public AudioInfo audioInfo;
        public TranslatedInfo translatedInfo;
    }

Using for receiving server pushed messages with interfaces of class RTMQuestProcessor.

When `messageType == (byte)MessageType.Chat`, `translatedInfo` will be assigned, `stringMessage` will be the translated messsge or original message if translation is not enabled, and `binaryMessage` will be null;  
When `messageType == (byte)MessageType.Audio`, if `binaryMessage` is assigned, means this message included the binary data of RTM audio message, else the `audioInfo` will be assigned, and the `stringMessage` may be the recognized message, or empty string;  
When `messageType == (byte)MessageType.Cmd`, `stringMessage` will be assigned and `binaryMessage` will be null,  
When `messageType` is a kinds of File types, `stringMessage` will be assigned the stored address and `binaryMessage` will be null.  
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
    }

Using for retrieving chat or messages.

If retrieved data is text data, `stringMessage` will be assigned and `binaryMessage` will be null,  
If retrieved data is binary data, `stringMessage` will be null and `binaryMessage` will be assigned.

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
        public AudioInfo audioInfo;
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