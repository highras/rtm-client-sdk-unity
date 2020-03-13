using System;
namespace com.fpnn.common
{
    public interface ErrorRecorder
    {
        void RecordError(Exception e);
        void RecordError(string message);
        void RecordError(string message, Exception e);
    }
}
