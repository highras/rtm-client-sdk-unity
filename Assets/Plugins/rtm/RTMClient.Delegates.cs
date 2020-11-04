using System.Collections.Generic;
namespace com.fpnn.rtm
{
    public delegate void AuthDelegate(long projectId, long uid, bool successful, int errorCode);
    public delegate void DoneDelegate(int errorCode);
    public delegate void MessageIdDelegate(long messageId, int errorCode);

    public class CheckResult
    {
        public int result;
        public List<int> tags;
    }

    public class TextCheckResult : CheckResult
    {
        public string text;
        public List<string> wlist;
    }

    public class FileInfo
    {
        //-- Common
        public string url;          //-- File url
        public int size = 0;        //-- File size

        //-- For image type
        public string surl;         //-- Thumb url, only for image type.

        //-- For RTM audio
        public bool isRTMAudio = false;
        public string language;
        public int duration = 0;
    }

    public class BaseMessage
    {
        public byte messageType;
        public string stringMessage = null;
        public byte[] binaryMessage = null;
        public string attrs;
        public long modifiedTime;
        public FileInfo fileInfo = null;

        //-- Compatible with version 2.1.4 and before.
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

    public class RetrievedMessage : BaseMessage
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

    public class TranslatedInfo
    {
        public string sourceLanguage;
        public string targetLanguage;
        public string sourceText;
        public string targetText;
    }

    public class RTMMessage : BaseMessage
    {
        public long fromUid;
        public long toId;                   //-- xid
        public long messageId;
        public TranslatedInfo translatedInfo = null;

        //-- Compatible with version 2.1.4 and before.
        [System.Obsolete("Field mid is deprecated, please use messageId instead.")]
        public long mid
        {
            get { return messageId; }
            set { messageId = value; }
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