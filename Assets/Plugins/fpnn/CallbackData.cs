using System;

namespace com.fpnn
{
    public class CallbackData
    {
        public FPData Data { get; set; } = null;

        public CallbackData(FPData data)
        {
            Data = data;
        }

        public Exception Exception { get; set; } = null;


        public CallbackData(Exception ex)
        {
            Exception = ex;
        }
    }
}