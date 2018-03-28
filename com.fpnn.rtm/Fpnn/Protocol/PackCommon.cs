using System;
using System.IO;

namespace Fpnn.Protocol
{
	public static class PackCommon
	{
		public static byte[] getUI16Byte (UInt16 s)
		{
			byte[] ui16out = new byte[2];
			ui16out [0] = (byte)(0xff & (s >> 8));
			ui16out [1] = (byte)(0xff & s);
			return ui16out;
		}
		
		public static byte[] getUI32Byte (UInt32 i32)
		{
			byte[] ui32out = new byte[4];
			ui32out [0] = (byte)(0xff & (i32 >> 24));
			ui32out [1] = (byte)(0xff & (i32 >> 16));
			ui32out [2] = (byte)(0xff & (i32 >> 8));
			ui32out [3] = (byte)(0xff & i32);
			return ui32out;
		}
		
		public static UInt16 readUI16 (byte[] ui16in)
		{
			return (UInt16)(((ui16in [0] & 0xff) << 8) | ((ui16in [1] & 0xff)));
		}
		
		public static UInt16 readUI16 (byte[] ui16in, int off)
		{
			return (UInt16)(((ui16in [off] & 0xff) << 8) | ((ui16in [off + 1] & 0xff)));
		}
		
		public static UInt32 readUI32 (byte[] ui32in)
		{
			return (UInt32)(((ui32in [0] & 0xff) << 24) | ((ui32in [1] & 0xff) << 16) | ((ui32in [2] & 0xff) << 8) | ((ui32in [3] & 0xff)));
		}
		
		public static UInt32 readUI32 (byte[] ui32in, int off)
		{
			return (UInt32)(((ui32in [off] & 0xff) << 24) | ((ui32in [off + 1] & 0xff) << 16) | ((ui32in [off + 2] & 0xff) << 8) | ((ui32in [off + 3] & 0xff)));
		}

		public static byte[] streamToBytes(MemoryStream stream)		
		{
			return ((MemoryStream)stream).ToArray();                
		}

		public static Stream bytesToStream(byte[] bytes)
		{
			MemoryStream stream = new MemoryStream(bytes);
			return stream;
		}

		public static byte[] getBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		public static string getString(byte[] bytes)
		{
			return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}

		public static string getStringByOffset(byte[] bytes, int start, int len)
		{
			byte[] chars = new byte[len];
			System.Buffer.BlockCopy(bytes, start, chars, 0, len);
			return System.Text.Encoding.UTF8.GetString(chars, 0, chars.Length);
		}

		public static Int64 getMilliTimestamp()
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(ts.TotalMilliseconds);
		}

		public static int getTimestamp()
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt32(ts.TotalSeconds);
		}
	}
}

