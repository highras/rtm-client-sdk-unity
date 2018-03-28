using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Fpnn.Protocol;

namespace Fpnn.Connection
{
	public delegate void FPCallback(FPAReader reader);

	public class FPClientCallback
	{
		public ManualResetEvent syncSignal = null;
		public FPAReader syncReader = null;
		public int timeoutSecond = 0;
		public int createTime = 0;

		public FPClientCallback()
		{
			this.createTime = PackCommon.getTimestamp();
		}

		public virtual void callback (FPAReader reader) {}
	}

	public class FPClientDelegate : FPClientCallback
	{
		FPCallback _cb = null;

		public FPClientDelegate (FPCallback cb)
		{
			this._cb = cb;
		}

		public override void callback (FPAReader reader) {
			this._cb (reader);
		}
	}

	public class CallbackDictionary
	{
		
		private Dictionary<UInt32, FPClientCallback> dict;
		private object accessLock = new object ();
		private int timeout = 0;
		private ManualResetEvent stopSignal = new ManualResetEvent(false);
		private Thread checkTimeoutThreadHandler = null;

		public CallbackDictionary ()
		{
			dict = new Dictionary<UInt32, FPClientCallback> ();
			this.stopSignal.Reset();
		}

		public void stop()
		{
			this.stopSignal.Set();
			if (this.checkTimeoutThreadHandler != null)
			{
				this.checkTimeoutThreadHandler.Join();
				this.checkTimeoutThreadHandler = null;
			}
		}

		~CallbackDictionary()
		{
			this.stop();
		}

		private void _startCheckThread()
		{
			lock(accessLock) {
				if (this.checkTimeoutThreadHandler == null) {
					this.checkTimeoutThreadHandler = new Thread(new ThreadStart(checkThread));
					this.checkTimeoutThreadHandler.IsBackground = true;
					this.checkTimeoutThreadHandler.Start();
				}
			}
		}

		private void checkThread()
		{
			while (!this.stopSignal.WaitOne(1000)) {

				List<UInt32> timeoutList = new List<UInt32>();

				lock (accessLock) {
					foreach (var pair in this.dict) {
						if (this.isTimeout(pair.Value))
							timeoutList.Add(pair.Key);
					}
				}

				foreach (UInt32 seqId in timeoutList) {
					FPClientCallback cb = this.get(seqId);
					if (cb != null) {
						FPAReader reader = new FPAReader(-101, "TimeoutException", "Quest timeout", "FPClient");
						if (cb.syncSignal == null)
							cb.callback(reader);
						else {
							cb.syncReader = reader;
							cb.syncSignal.Set();
						}
					}
				}
			}
		}

		private bool isTimeout(FPClientCallback cb)
		{
			int timeoutValue = this.timeout;
			if (cb.timeoutSecond > 0)
				timeoutValue = cb.timeoutSecond;
			if (timeoutValue > 0) {
				int now = PackCommon.getTimestamp();
				return (now - cb.createTime >= timeoutValue);
			}
			return false;
		}

		public void setTimeout(int seconds)
		{
			if (seconds > 0)
				this.timeout = seconds;
			if (this.timeout > 0)
				this._startCheckThread();
		}
		
		public void put (UInt32 seqId, FPClientCallback cb)
		{
			if (cb.timeoutSecond > 0)
				this._startCheckThread();
			
			lock (accessLock) {
				dict.Add (seqId, cb);
			}
		}
		
		public FPClientCallback get (UInt32 seqId)
		{
			FPClientCallback cb = null;
			lock (accessLock) {
				if (dict.TryGetValue (seqId, out cb)) {
					dict.Remove (seqId);
				}
			}
			return cb;
		}

		public void exceptionFlush ()
		{
			lock (accessLock) {
				FPAReader reader = new FPAReader (-100, "FlushException", "Fpnn connection has broken, we have to flush all on-the-fly requests with exception.", "FPClient");
				foreach(FPClientCallback cb in dict.Values) {
					if (cb.syncSignal == null)
						cb.callback(reader);
					else {
						cb.syncReader = reader;
						cb.syncSignal.Set();
					}
				}
				dict.Clear();
			}
		}
		
		public static void procPackageWithException(FPClientCallback cb)
		{
			FPAReader reader = new FPAReader (-100, "BrokenException", "Fpnn connection was broken, you can not send any command before reconnect again.", "FPClient");
			if (cb.syncSignal == null)
				cb.callback(reader);
			else {
				cb.syncReader = reader;
				cb.syncSignal.Set();
			}
		}
	}
}