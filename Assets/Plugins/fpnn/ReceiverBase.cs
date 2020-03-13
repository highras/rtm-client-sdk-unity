using System;
using com.fpnn.proto;
namespace com.fpnn
{
    internal class ReceiverErrorMessageException : Exception
    {
        public ReceiverErrorMessageException(string message) : base(message) { }
    }

    public abstract class ReceiverBase
    {
        public byte[] buffer;
        public int offset;
        public int requireLength;

        public abstract void Done(out Quest quest, out Answer answer);
    }
}
