using System;
namespace com.fpnn.msgpack
{
    public class MsgPackException: Exception
    {
        public MsgPackException(string message) : base(message) { }
        public MsgPackException(String message, Exception ex) : base(message, ex) { }
    }

    public class UnsupportedTypeException : MsgPackException
    {
        public UnsupportedTypeException(string message) : base(message) { }
        static public UnsupportedTypeException Create(Object obj)
        {
            string typeFullName = obj.GetType().FullName;
            return new UnsupportedTypeException("FPNN MsgPacker unsupported type: " + typeFullName);
        }
    }

    public class UnrecognizedDataException : MsgPackException
    {
        public UnrecognizedDataException(string message) : base(message) { }
    }

    public class InsufficientException : MsgPackException
    {
        public InsufficientException(string message) : base(message) { }
    }

    public class InvalidDataException : MsgPackException
    {
        public InvalidDataException(string message) : base(message) { }
        public InvalidDataException(string message, Exception ex) : base(message, ex) { }
    }
}
