using System;
using System.IO;
using System.Collections.Generic;
using com.fpnn.msgpack;

namespace com.fpnn.proto
{
    public class Message
    {
        private Dictionary<object, object> payload;

        public Message()
        {
            payload = new Dictionary<object, object>();
        }

        public Message(Dictionary<object, object> payload)
        {
            this.payload = payload;
        }

        //-----------------[ Data Accessing Functions ]-------------------
        public void Param(string key, object value)
        {
            payload.Add(key, value);
        }

        public object Get(string key, object defValue = null)
        {
            if (payload.TryGetValue(key, out object value))
            {
                return value;
            }
            
            return defValue;
        }

        public T Get<T>(string key, T defValue)
        {
            if (payload.TryGetValue(key, out object value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return defValue;
        }

        public object Want(string key)
        {
            return payload[key];
        }

        public T Want<T>(string key)
        {
            object obj = payload[key];
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        //-----------------[ To Bytes Array Functions ]-------------------
        protected void Raw(Stream stream)
        {
            MsgPacker.Pack(stream, payload);
        }

        public byte[] Raw()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                MsgPacker.Pack(stream, payload);
                return stream.ToArray();
            }
        }
    }
}
