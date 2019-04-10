using System;
using System.IO;
using System.Collections;

namespace com.fpnn
{
    public static class FPCommon
    {
        public static byte[] GetUI16Byte (UInt16 s)
		{
			byte[] ui16out = new byte[2];
			ui16out [0] = (byte)(0xff & (s >> 8));
			ui16out [1] = (byte)(0xff & s);
			return ui16out;
		}
		
		public static byte[] GetUI32Byte (UInt32 i32)
		{
			byte[] ui32out = new byte[4];
			ui32out [0] = (byte)(0xff & (i32 >> 24));
			ui32out [1] = (byte)(0xff & (i32 >> 16));
			ui32out [2] = (byte)(0xff & (i32 >> 8));
			ui32out [3] = (byte)(0xff & i32);
			return ui32out;
		}
		
		public static UInt16 ReadUI16 (byte[] ui16in)
		{
			return (UInt16)(((ui16in [0] & 0xff) << 8) | ((ui16in [1] & 0xff)));
		}
		
		public static UInt16 ReadUI16 (byte[] ui16in, int off)
		{
			return (UInt16)(((ui16in [off] & 0xff) << 8) | ((ui16in [off + 1] & 0xff)));
		}
		
		public static UInt32 ReadUI32 (byte[] ui32in)
		{
			return (UInt32)(((ui32in [0] & 0xff) << 24) | ((ui32in [1] & 0xff) << 16) | ((ui32in [2] & 0xff) << 8) | ((ui32in [3] & 0xff)));
		}
		
		public static UInt32 ReadUI32 (byte[] ui32in, int off)
		{
			return (UInt32)(((ui32in [off] & 0xff) << 24) | ((ui32in [off + 1] & 0xff) << 16) | ((ui32in [off + 2] & 0xff) << 8) | ((ui32in [off + 3] & 0xff)));
		}

		public static byte[] StreamToBytes(MemoryStream stream)		
		{
			return ((MemoryStream)stream).ToArray();                
		}

		public static Stream BytesToStream(byte[] bytes)
		{
			MemoryStream stream = new MemoryStream(bytes);
			return stream;
		}

		public static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		public static string GetString(byte[] bytes)
		{
			return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}

        public static string GetString(ArrayList array)
		{
            byte[] bytes = new byte[array.Count];
            bytes = (byte[])array.ToArray(typeof(byte));
			return GetString(bytes);
		}

        public static byte[] GetBytes(ArrayList array)
        {
            byte[] bytes = new byte[array.Count];
            bytes = (byte[])array.ToArray(typeof(byte));
            return bytes;
        }

		public static string GetStringByOffset(byte[] bytes, int start, int len)
		{
			byte[] chars = new byte[len];
			System.Buffer.BlockCopy(bytes, start, chars, 0, len);
			return System.Text.Encoding.UTF8.GetString(chars, 0, chars.Length);
		}

		public static byte[] GetByteArrayRange(byte[] arr, int start, int end)
		{
			byte[] arrNew = new byte[end - start + 1];
			int j = 0;
			for (int i = start; i <= end; i++)
				arrNew[j++] = arr[i];
			return arrNew;
		}

		public static Int64 GetMilliTimestamp()
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(ts.TotalMilliseconds);
		}

		public static int GetTimestamp()
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt32(ts.TotalSeconds);
		}
    }
}