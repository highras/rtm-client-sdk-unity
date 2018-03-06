using System;
namespace com.fpnn.rtm
{
	public class RTMException : Exception
	{
		private int code = 0;
		private string message = "";
		private string raiser = "";

		public RTMException() { }

		public RTMException(string message): base(message) 
		{
			this.message = message;
		}

		public RTMException(string message, Exception e)
        : base(message, e)
   		{
			this.message = message;
		}

		public RTMException(int code, string ex, string raiser)
		: base(ex)
		{
			this.code = code;
			this.message = ex;
			this.raiser = raiser;
		}

		public string str()
		{
			return "code(" + this.code + ") message(" + this.message + ") raiser(" + this.raiser + ")";
		}
	}
}

