using System;
namespace Fpnn.Connection
{

	public abstract class ErrorRecorder
	{
		public abstract void recordError(string msg);
		public abstract void recordError(Exception e);
	}

	public class DefaultErrorRecorder: ErrorRecorder 
	{
		public override void recordError(string msg)
		{
			Console.WriteLine(msg);
		}

		public override void recordError(Exception e)
		{
			Console.WriteLine(e.Message);
			Console.WriteLine(e.StackTrace);
		}
	}

	public class ErrorRecorderHolder
	{
		private static ErrorRecorder uniqueInstance;
		private static readonly object locker = new object();

		private ErrorRecorderHolder() {}

		public static void setInstance(ErrorRecorder ins)
		{
			lock (locker)
			{
				uniqueInstance = ins;
			}
		}

		public static ErrorRecorder getInstance()
		{
			if (uniqueInstance == null)
			{
				lock (locker)
				{
					if (uniqueInstance == null)
					{
						uniqueInstance = new DefaultErrorRecorder();
					}
				}
			}
			return uniqueInstance;
		}

		public static void recordError(string msg)
		{
			ErrorRecorderHolder.getInstance().recordError(msg);
		}

		public static void recordError(Exception e)
		{
			ErrorRecorderHolder.getInstance().recordError(e);
		}
	}
}

