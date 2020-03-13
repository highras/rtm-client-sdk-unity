# RTM Client Unity SDK API Docs: Structures

# Index

[TOC]

### TranslatedMessage

    public class TranslatedMessage
    {
        public string source;
        public string target;
        public string sourceText;
        public string targetText;
    }

Using for translation functions.

### RetrievedMessage

    public class RetrievedMessage
    {
        public long id;
        public byte mtype;
        public string stringMessage;
        public byte[] binaryMessage;
        public string attrs;
        public long mtime;
    }

Using for retrieving chat or messages.

If retrieved data is text data, `stringMessage` will be assigned and `binaryMessage` will be null,  
If retrieved data is binary data, `stringMessage` will be null and `binaryMessage` will be assigned.

### HistoryMessage

    public class HistoryMessage
    {
        public long id;
        public long fromUid;
        public byte mtype;
        public long mid;
        public string stringMessage;
        public byte[] binaryMessage;
        public string attrs;
        public long mtime;
    }

Using for history message result.

If message data is text data, `stringMessage` will be assigned and `binaryMessage` will be null,  
If message data is binary data, `stringMessage` will be null and `binaryMessage` will be assigned.

### HistoryMessageResult

    public class HistoryMessageResult
    {
        public int count;
        public long lastId;
        public long beginMsec;
        public long endMsec;
        public List<HistoryMessage> messages;
    }

Using as the result of history message & chat methods.

When calling history message or history chat methods for fetching subsequent message or chat data, please using the corresponding fields in the result.