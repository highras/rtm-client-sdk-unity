using System;
using System.IO;
using System.Collections.Generic;
using com.fpnn.msgpack;

namespace com.fpnn.rtm
{
    public class RTMAudioHeader
    {
        public enum ContainerType : byte
		{
			CodecInherent = 0
		}

        public enum CodecType : byte
		{
			AmrWb = 1,
            AmrNb = 2
		}

        public static byte CurrentVersion = 1;

        public byte version = 0;
        public ContainerType containerType;
        public CodecType codecType;
        public string lang;
        public long duration;
        public int sampleRate;

        private readonly byte[] headerArray;

        public RTMAudioHeader()
        {
            
        }

        public RTMAudioHeader(byte version, ContainerType containerType, CodecType codecType, string lang, long duration, int sampleRate)
        {
            this.version = version;
            this.containerType = containerType;
            this.codecType = codecType;
            this.lang = lang;
            this.duration = duration;
            this.sampleRate = sampleRate;

            MemoryStream stream = new MemoryStream();

            Dictionary<string, object> infoData = new Dictionary<string, object>() {
                { "lang", this.lang},
                { "dur", this.duration },
                { "srate", this.sampleRate }
            };
            byte[] bytes = new byte[0];

            using (MemoryStream outputStream = new MemoryStream())
            {
                MsgPacker.Pack(outputStream, infoData);
                bytes = outputStream.ToArray();
            }

            stream.Write(new byte[4]{CurrentVersion, (byte)containerType, (byte)codecType, 1}, 0, 4);
            stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            stream.Write(bytes, 0, bytes.Length);
            this.headerArray = stream.ToArray();
        }
        
        public byte[] HeaderArray
        {
            get
            {
                return headerArray;
            }
        }
    }
}