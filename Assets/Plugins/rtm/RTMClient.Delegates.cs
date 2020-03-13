using System.Collections.Generic;
namespace com.fpnn.rtm
{
    public delegate void AuthDelegate(long pid, long uid, bool authStatus, int errorCode);
    public delegate void DoneDelegate(int errorCode);
    public delegate void ActTimeDelegate(long mtime, int errorCode);

    public class TranslatedMessage
    {
        public string source;
        public string target;
        public string sourceText;
        public string targetText;
    }

    public class RetrievedMessage
    {
        public long id;
        public byte mtype;
        public string stringMessage;
        public byte[] binaryMessage;
        public string attrs;
        public long mtime;
    }

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

    public class HistoryMessageResult
    {
        public int count;
        public long lastId;
        public long beginMsec;
        public long endMsec;
        public List<HistoryMessage> messages;
    }

    public delegate void HistoryMessageDelegate(int count, long lastId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode);
}