using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Collections;

namespace com.fpnn
{
    public delegate void FPCallback(CallbackData cbd);

    public class FPClientCallback
	{
		public ManualResetEvent syncSignal = null;
		public CallbackData syncCbd = null;
		public int timeoutMilliSeconds = 0;
		public long createTime = 0;

		public FPClientCallback()
		{
			this.createTime = FPCommon.GetMilliTimestamp();
		}

		public virtual void callback(CallbackData cbd) {}
	}

	public class FPClientDelegate : FPClientCallback
	{
		FPCallback cb = null;

		public FPClientDelegate (FPCallback cb)
		{
			this.cb = cb;
		}

		public override void callback(CallbackData cbd) {
			this.cb(cbd);
		}
	}

    public class CallbackDictionary
	{
		
		private Dictionary<UInt32, FPClientCallback> dict;
		private object accessLock = new object ();
		private int timeoutMilliSeconds = 0;
		private ManualResetEvent stopSignal = new ManualResetEvent(false);
		private Thread checkTimeoutThreadHandler = null;

		public CallbackDictionary ()
		{
			dict = new Dictionary<UInt32, FPClientCallback> ();
			this.stopSignal.Reset();
		}

		public void Stop()
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
			this.Stop();
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
						if (this.IsTimeout(pair.Value))
							timeoutList.Add(pair.Key);
					}
				}

				foreach (UInt32 seqId in timeoutList) {
					FPClientCallback cb = this.Get(seqId);
					if (cb != null) {
						CallbackData cbd = new CallbackData(new Exception("Quest Timeout"));
						if (cb.syncSignal == null)
							cb.callback(cbd);
						else {
							cb.syncCbd = cbd;
							cb.syncSignal.Set();
						}
					}
				}
			}
		}

		private bool IsTimeout(FPClientCallback cb)
		{
			long timeoutValue = this.timeoutMilliSeconds;
			if (cb.timeoutMilliSeconds > 0)
				timeoutValue = cb.timeoutMilliSeconds;
			if (timeoutValue > 0) {
				long now = FPCommon.GetMilliTimestamp();
				return (now - cb.createTime >= timeoutValue);
			}
			return false;
		}

		public void SetTimeout(int milliSeconds)
		{
			if (milliSeconds > 0)
				this.timeoutMilliSeconds = milliSeconds;
			if (this.timeoutMilliSeconds > 0)
				this._startCheckThread();
		}
		
		public void Put (UInt32 seqId, FPClientCallback cb)
		{
			if (cb.timeoutMilliSeconds > 0)
				this._startCheckThread();
			
			lock (accessLock) {
				dict.Add (seqId, cb);
			}
		}
		
		public FPClientCallback Get (UInt32 seqId)
		{
			FPClientCallback cb = null;
			lock (accessLock) {
				if (dict.TryGetValue (seqId, out cb)) {
					dict.Remove (seqId);
				}
			}
			return cb;
		}

		public void FlushAllException (Exception ex)
		{
			lock (accessLock) {
				CallbackData cbd = new CallbackData(ex);
				foreach(FPClientCallback cb in dict.Values) {
					if (cb.syncSignal == null)
					{
						ThreadPool.QueueUserWorkItem( (state) =>
						{
							cb.callback(cbd);
						});
					}
					else {
						cb.syncCbd = cbd;
						cb.syncSignal.Set();
					}
				}
				dict.Clear();
			}
		}
	}
}