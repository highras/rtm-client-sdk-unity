using System;
using com.fpnn.common;

namespace com.fpnn.rtm
{
    public interface IRTMMasterProcessor: IQuestProcessor
    {
        void SetErrorRecorder(ErrorRecorder recorder);
        void SetConnectionId(Int64 connId);
        bool ConnectionIsAlive();
        void SessionClosed(int ClosedByErrorCode);
    }
}
