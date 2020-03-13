## FPNN MsgPack Implement

* **[msgpack SPEC](https://github.com/msgpack/msgpack/blob/master/spec.md)**
* **[Project Home](https://github.com/highras/msgpack-csharp)**

### Compatibility Version:

C# .Net Standard 2.0

### For Packer:

* input kinds:

    null, bool, sbyte, byte, short, ushort, Int32, UInt32, Int64, Uint64, float, double, string

    Decimal, Tuple

    byte[], DateTime, IEnumerable (such as List, Dictionary, List\<object\>, Dictionary\<object, object\>, ...)

* usage:

        using com.fpnn.msgpack;
        void MsgPacker.Pack(Stream stream, Object obj);


### For Unpacker:

* output kinds:

    Object, which maybe the following kinds:

    null, bool, sbyte, byte, short, ushort, Int32, UInt32, Int64, Uint64, float, double, string

    byte[], DateTime, List\<object\>, Dictionary\<object, object\>

* usage:

        using com.fpnn.msgpack;
        Dictionary<Object, Object> MsgUnpacker.Unpack(byte[] binary);

        Dictionary<Object, Object> MsgUnpacker.Unpack(byte[] binary, int offset, int length = 0);

        //-- unpack one object.
        Object MsgUnpacker.Unpack(byte[] binary, int offset, out int endOffset);


### Exception:

        using com.fpnn.msgpack;
        public class MsgPackException: Exception;
        public class UnsupportedTypeException : MsgPackException;
        public class UnrecognizedDataException : MsgPackException;
        public class InsufficientException : MsgPackException;
        public class InvalidDataException : MsgPackException;
