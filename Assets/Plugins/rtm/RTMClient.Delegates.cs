using System.Collections.Generic;
namespace com.fpnn.rtm
{
    public delegate void AuthDelegate(long projectId, long uid, bool successful, int errorCode);
    public delegate void DoneDelegate(int errorCode);
    public delegate void ActTimeDelegate(long modifiedTime, int errorCode);

    //-- Obsolete in v.2.2.0
    [System.Obsolete("TranslatedMessage is deprecated, please use RTMMessage instead.")]
    public class TranslatedMessage
    {
        public string source;
        public string target;
        public string sourceText;
        public string targetText;
    }

    public class RetrievedMessage
    {
        public long cursorId;
        public byte messageType;
        public string stringMessage = null;
        public byte[] binaryMessage = null;
        public string attrs;
        public long modifiedTime;

        //-- Compatible with version 2.1.4 and before.
        [System.Obsolete("Field id is deprecated, please use cursorId instead.")]
        public long id
        {
            get { return cursorId; }
            set { cursorId = value; }
        }

        [System.Obsolete("Field mtype is deprecated, please use messageType instead.")]
        public byte mtype
        {
            get { return messageType; }
            set { messageType = value; }
        }

        [System.Obsolete("Field mtime is deprecated, please use modifiedTime instead.")]
        public long mtime
        {
            get { return modifiedTime; }
            set { modifiedTime = value; }
        }
    }

    public class AudioInfo
    {
        public string sourceLanguage;
        public string recognizedLanguage;
        public string recognizedText;
        public int duration;
    }

    public class TranslatedInfo
    {
        public string sourceLanguage;
        public string targetLanguage;
        public string sourceText;
        public string targetText;
    }

    public class RTMMessage
    {
        public long fromUid;
        public long toId;                   //-- xid
        public byte messageType;
        public long messageId;
        public string stringMessage = null;
        public byte[] binaryMessage = null;
        public string attrs;
        public long modifiedTime;
        public AudioInfo audioInfo = null;
        public TranslatedInfo translatedInfo = null;

        //-- Compatible with version 2.1.4 and before.
        [System.Obsolete("Field mtype is deprecated, please use messageType instead.")]
        public byte mtype
        {
            get { return messageType;  }
            set { messageType = value; }
        }

        [System.Obsolete("Field mid is deprecated, please use messageId instead.")]
        public long mid
        {
            get { return messageId; }
            set { messageId = value; }
        }

        [System.Obsolete("Field mtime is deprecated, please use modifiedTime instead.")]
        public long mtime
        {
            get { return modifiedTime; }
            set { modifiedTime = value; }
        }
    }

    public class HistoryMessage : RTMMessage
    {
        public long cursorId;

        //-- Compatible with version 2.1.4 and before.
        [System.Obsolete("Field id is deprecated, please use cursorId instead.")]
        public long id
        {
            get { return cursorId; }
            set { cursorId = value; }
        }
    }

    public class HistoryMessageResult
    {
        public int count;
        public long lastCursorId;
        public long beginMsec;
        public long endMsec;
        public List<HistoryMessage> messages;

        [System.Obsolete("Field lastId is deprecated, please use lastCursorId instead.")]
        public long lastId
        {
            get { return lastCursorId; }
            set { lastCursorId = value; }
        }
    }

    public delegate void HistoryMessageDelegate(int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode);
}