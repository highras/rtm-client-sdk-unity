using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using Fpnn.Protocol;

using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Agreement;

namespace Fpnn.Connection
{
	public delegate void ConnectionConnectedCallback();
	public delegate void ConnectionWillCloseCallback(bool causedByError);
	public delegate void PushQueueFullCallback();

	public abstract class FpnnClientProcessor { }



	public class TCPClient
	{
		private TCPSocket sock;
		private CallbackDictionary cbContainer;
		private Thread receiveThreadHandler = null;
		private object opLocker = new object ();
		private bool isConnected = false;
		private object startRecvLocker = new object();
		private bool isRecvThreadStart = false;
		private bool autoReconnect = true;
		private object reconnectLocker = new object();
		private ManualResetEvent stopSignal = new ManualResetEvent(false);
		private object sendLocker = new object ();
		private ConnectionWillCloseCallback closeCb = null;
		private ConnectionConnectedCallback connectCb = null;
		private int pushQueueMaxSize = 500;
		private Queue pushQueue = new Queue ();
		private FpnnClientProcessor processor = null;
		private AutoResetEvent pushEvent = new AutoResetEvent(false);
		private Thread pushThreadHandler = null;
		private PushQueueFullCallback pushFullCb = null;
		private bool isStop = false;

		private byte[] pubKey;
		private byte[] iv;
		private byte[] key;
		private int strength;
		private bool isEncryptor = false;
		private bool canEncryptor = true;

	    public TCPClient (string hostport) 
			: this(hostport.Split(':')[0], Int32.Parse(hostport.Split(':')[1]), true, 10) 
        {
        }

		public TCPClient(string hostport, bool autoReconnect)
			: this(hostport.Split(':')[0], Int32.Parse(hostport.Split(':')[1]), autoReconnect, 10)
		{
		}

        public TCPClient(string hostport, bool autoReconnect, int connectionTimeout)
            : this(hostport.Split(':')[0], Int32.Parse(hostport.Split(':')[1]), autoReconnect, connectionTimeout)
        {
        }

		public TCPClient(string host, int port)
			: this(host, port, true, 10)
		{
		}

		public TCPClient(string host, int port, bool autoReconnect)
			: this(host, port, autoReconnect, 10)
		{
		}

		public TCPClient(string host, int port, bool autoReconnect, int connectionTimeout)
		{
			this.autoReconnect = autoReconnect;
			this.cbContainer = new CallbackDictionary();
			sock = new TCPSocket(host, port, connectionTimeout);
			this.stopSignal.Reset();
			ThreadPool.SetMaxThreads(3, 3);
		}

		public void setPushQueueMaxSize(int max)
		{
			this.pushQueueMaxSize = max;
		}

		public void setProcessor(FpnnClientProcessor proc)
		{
			this.processor = proc;
			this._startPushThread();
		}

		public void setQuestTimeout(int seconds)
		{
			this.cbContainer.setTimeout(seconds);
		}

		public void connect ()
		{
			this._connect(false);
		}

		public void reconnect()
		{
			this._connect(true);
		}

		private void _connect(bool isReconnect)
		{
			lock (opLocker)
			{
				if (isReconnect)
				{
					this.sock.close();
				}
				this.sock.open();
				this._sendEncryptor();
				this._startRecvThread();
				this._startPushThread();
				this.isConnected = true;
			}
			if (this.connectCb != null)
			{
				this.connectCb();
			}
		}

		public void setConnectionConnectedCallback(ConnectionConnectedCallback cb)
		{
			this.connectCb = cb;
		}

		public void setConnectionWillCloseCallback (ConnectionWillCloseCallback cb)
		{
			this.closeCb = cb;
		}

		public void setPushQueueFullCallback(PushQueueFullCallback cb)
		{
			this.pushFullCb = cb;
		}

		public void close ()
		{
			_stop ();
		}

		private byte[] getByteArrayRange(byte[] arr, int start, int end)
		{
			byte[] arrNew = new byte[end - start + 1];
			int j = 0;
			for (int i = start; i <= end; i++)
				arrNew[j++] = arr[i];
			return arrNew;
		}

		public void enableEncryptor(string peerPubData, string curveName = "secp256k1", int strength = 128)
		{
			if (!this.canEncryptor)
				throw new System.Exception("can not enable encryptor");

			if (this.isConnected)
				throw new System.Exception("can not enable encryptor after connect");
		
			if (curveName != "secp256k1" && curveName != "secp256r1" && curveName != "secp192r1" && curveName != "secp224r1")
				curveName = "secp256k1";
			
			if (strength != 128 && strength != 256)
				strength = 128;
			
			this.strength = strength;

			SecureRandom secureRandom = new SecureRandom();
			X9ECParameters curve = SecNamedCurves.GetByName(curveName);
			ECDomainParameters domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());

			var ec = new ECKeyPairGenerator();
			ECKeyGenerationParameters keygenParams = new ECKeyGenerationParameters(domain, secureRandom);
			ec.Init(keygenParams);

			AsymmetricCipherKeyPair keypair = ec.GenerateKeyPair();
			ECPublicKeyParameters publicKeyParam = (ECPublicKeyParameters)keypair.Public;

			byte[] pubKeyByte = publicKeyParam.Q.GetEncoded();
			this.pubKey = this.getByteArrayRange(pubKeyByte, 1, pubKeyByte.Length - 1);

			PemReader pemRd = new PemReader(new StreamReader(new MemoryStream(System.Text.Encoding.Default.GetBytes(peerPubData), false)));
			ECPublicKeyParameters peerPublicKeyParam = (ECPublicKeyParameters)pemRd.ReadObject();

			ECDHCBasicAgreement aKeyAgree = new ECDHCBasicAgreement();
			aKeyAgree.Init(keypair.Private);
			BigInteger secretInt = aKeyAgree.CalculateAgreement(peerPublicKeyParam);
			byte[] secret = secretInt.ToByteArrayUnsigned();

			if (this.strength == 128)
				this.key = this.getByteArrayRange(secret, 0, 15);
			else {
				if (secret.Length == 32)
					this.key = this.getByteArrayRange(secret, 0, 31);
				else {
					SHA256Managed crypt = new SHA256Managed();
					byte[] sha256 = crypt.ComputeHash(secret);
					this.key = this.getByteArrayRange(sha256, 0, secret.Length - 1);
				}
			}

			MD5 md5 = System.Security.Cryptography.MD5.Create();
			this.iv = md5.ComputeHash(secret);

			this.isEncryptor = true;
		}

		public UInt32 sendQuest (FPQuest quest, FPCallback cb, int timeoutSeconds = 0)
		{
			if (quest.isOneWay ()) {
				return _send (quest, null);
			} else {
				FPClientDelegate cbDelegate = new FPClientDelegate (cb);
				cbDelegate.timeoutSecond = timeoutSeconds;
				return _send (quest, cbDelegate);
			}
		}

		public UInt32 sendQuest (FPQuest quest, FPClientCallback cb = null, int timeoutSeconds = 0)
		{
			cb.timeoutSecond = timeoutSeconds;
			return _send (quest, cb);
		}

		public FPAReader sendQuestSync(FPQuest quest, int timeoutSeconds = 0)
		{
			FPClientCallback cb = new FPClientCallback();
			cb.timeoutSecond = timeoutSeconds;
			cb.syncSignal = new ManualResetEvent(false);
			cb.syncSignal.Reset();
			_send(quest, cb);

			cb.syncSignal.WaitOne();

			if (cb.syncReader != null)
				return cb.syncReader;
			else
				return new FPAReader(-100, "SendQuestSyncException", "Send quest sync error", "FPClient");
		}

		private byte[] _encrypt(byte[] buffer)
		{
			byte[] encryptedData;
			using (RijndaelManaged aes = new RijndaelManaged())
			{
				aes.Mode = CipherMode.CFB;
				aes.Key = this.key;
				aes.IV = this.iv;
				ICryptoTransform encryptor = aes.CreateEncryptor();
				using (MemoryStream mStream = new MemoryStream())
				{
					using (CryptoStream cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
					{
						cStream.Write(buffer, 0, buffer.Length);
						cStream.FlushFinalBlock();
						encryptedData = mStream.ToArray();
					}
				}
			}
			return this.getByteArrayRange(encryptedData, 0, buffer.Length - 1);
		}

		private byte[] _decrypt(byte[] buffer)
		{
			IBlockCipher theCipher = null;
			theCipher = new RijndaelEngine(this.strength);
			BufferedBlockCipher cipher = new BufferedBlockCipher(new CfbBlockCipher(theCipher, 8 * theCipher.GetBlockSize()));
			ParametersWithIV IVkey = new ParametersWithIV(new KeyParameter(this.key), this.iv);
			cipher.Init(false, IVkey);

			int size = cipher.GetOutputSize(buffer.Length);
			byte[] result = new byte[size];
			int olen = cipher.ProcessBytes(buffer, 0, buffer.Length, result, 0);
			olen += cipher.DoFinal(result, olen);

			if (olen < size)
			{
				byte[] tmp = new byte[olen];
				Array.Copy(result, 0, tmp, 0, olen);
				result = tmp;
			}
			return result;
		}

		public UInt32 _send (FPQuest quest, FPClientCallback cb)
		{
			if (!this.isConnected)
			{
				this._connect(false);
			}

			this.canEncryptor = false;

			quest.genSeqNum();

			UInt32 seqNum = 0;
			byte[] _buf = quest.raw ();

			if (this.isEncryptor) {
				byte[] byteLength = BitConverter.GetBytes(_buf.Length);

				if (!BitConverter.IsLittleEndian)
					Array.Reverse(byteLength);

				byte[] encryptBuf = this._encrypt(_buf);
				byte[] bufferNew = new byte[byteLength.Length + encryptBuf.Length];

				Array.Copy(byteLength, 0, bufferNew, 0, byteLength.Length);
				Array.Copy(encryptBuf, 0, bufferNew, byteLength.Length, encryptBuf.Length);

				_buf = bufferNew;
			}

			lock (reconnectLocker)
			{
				try
				{
					lock (sendLocker)
					{
						this.sock.write(_buf, 0, _buf.Length);
						this.sock.flush();
					}

				}
				catch (Exception)
				{
					if (this.autoReconnect)
					{
						try
						{
							this.reconnect();

							this.sock.write(_buf, 0, _buf.Length);
							this.sock.flush();
						}
						catch (Exception er)
						{
							ErrorRecorderHolder.recordError(er);

							if (cb != null)
								invokeException(cb);

							return 0;
						}
					}
					else
					{
						if (cb != null)
							invokeException(cb);

						return 0;
					}
				}
			}

			if (cb != null)
			{
				seqNum = quest.seqNum;
				cbContainer.put(seqNum, cb);
			}

			return seqNum;
		}

		public void _sendReply(string method, UInt32 seqNum)
		{
			FPReply reply = new FPReply(seqNum);
			byte[] _buf = reply.raw();
			try
			{
				lock (sendLocker)
				{
					this.sock.write(_buf, 0, _buf.Length);
					this.sock.flush();
				}
			}
			catch (Exception e)
			{
				ErrorRecorderHolder.recordError(e);
			}
		}

		protected class ExceptionInvokeThread
		{
			private FPClientCallback cb;
			
			public ExceptionInvokeThread(FPClientCallback cb)
			{
				this.cb = cb;
			}
			
			public void ThreadProc()
			{
				CallbackDictionary.procPackageWithException (cb);
			}
			
		}

		public void invokeException (FPClientCallback cb)
		{
			ExceptionInvokeThread exceptionThreadHandler = new ExceptionInvokeThread (cb);
			Thread th = new Thread(exceptionThreadHandler.ThreadProc);
			th.IsBackground = true;
			th.Start();
		}

		private void ReceiveThread()
		{
			while (!this.stopSignal.WaitOne(0))
			{
				try
				{
					answerHandler(ReadPackage());
				}
				catch (System.Exception e)
				{
					lock (this.startRecvLocker)
					{
						this.isRecvThreadStart = false;
						cbContainer.exceptionFlush();
					}

					if (this.closeCb != null)
					{
						this.closeCb(this.isStop == false);
					}

					if (this.isStop)
						break;

					if (this.autoReconnect)
					{
						try
						{
							this.reconnect();
						}
						catch (System.Exception) {}
					}

					break;
				}
			}
		}

		private void _sendEncryptor()
		{
			if (this.isEncryptor)
			{
				FPQWriter qw = new FPQWriter(3, "*key");
				qw.param("publicKey", this.pubKey);
				qw.param("streamMode", false);
				qw.param("bits", this.strength);
				FPQuest quest = qw.take();
				byte[] _buf = quest.raw();

				lock (sendLocker)
				{
					sock.write(_buf, 0, _buf.Length);
					sock.flush();
				}
			}
		}

		private void _startRecvThread()
		{
			lock (this.startRecvLocker)
			{
				if (!this.isRecvThreadStart)
				{
					this.receiveThreadHandler = new Thread(new ThreadStart(ReceiveThread));
					this.receiveThreadHandler.Start();
					this.receiveThreadHandler.IsBackground = true;
					this.isRecvThreadStart = true;
				}
			}
		}

		private void _startPushThread()
		{
			if (this.pushThreadHandler == null)
			{
				this.pushThreadHandler = new Thread(new ThreadStart(PushHandlerThread));
				this.pushThreadHandler.IsBackground = true;
				this.pushThreadHandler.Start();
			}
		}

		private FPMessage ReadPackage ()
		{
			byte[] buf;
			if (!this.isEncryptor)
			{
				buf = new byte[16];
				this.sock.read(buf, 0, 16);
			}
			else
			{
				byte[] bufLen = new byte[4];
				this.sock.read(bufLen, 0, 4);
				Int32 readSize = BitConverter.ToInt32(this.getByteArrayRange(bufLen, 0, 3), 0);

				if (readSize < 17)
					throw new System.Exception("wrong protocol");

				buf = new byte[readSize];
				this.sock.read(buf, 0, readSize);

				buf = this._decrypt(buf);
			} 

			UInt32 magic = PackCommon.readUI32(this.getByteArrayRange(buf, 0, 3));
			byte version = buf[4];
			byte flag = buf[5];
			byte mtype = buf[6];
			byte ss = buf[7];
			UInt32 psize = BitConverter.ToUInt32(this.getByteArrayRange(buf, 8, 11), 0);
			if (mtype == (byte)FPMessageType.FP_MT_TWOWAY)
			{
				psize += ss;
			}
			UInt32 seq = BitConverter.ToUInt32(this.getByteArrayRange(buf, 12, 15), 0);

			byte[] payload = new byte[psize];

			if (!this.isEncryptor)
				this.sock.read(payload, 0, payload.Length); 
			else {
				payload = this.getByteArrayRange(buf, 16, buf.Length - 1);
			}

			return new FPMessage(magic, version, flag, mtype, ss, psize, seq, payload);
		}
		 
		private void answerHandler(FPMessage msg)
		{
			if (msg.isAnswer()) {
				FPAReader reader = new FPAReader(new FPAnswer(msg));
				FPClientCallback cb = cbContainer.get(reader.seqNum());
				if (cb != null)
				{
					if (cb.syncSignal == null) {
						try
						{
							ThreadPool.QueueUserWorkItem( (state) =>
							{
								cb.callback(reader);
							});
						}
						catch (Exception e)
						{
							ErrorRecorderHolder.recordError(e);
						}
					} else {
						cb.syncReader = reader;
						cb.syncSignal.Set();
					}
				}
			} else if (msg.isQuest()) {
				FPQReader reader = new FPQReader(new FPQuest(msg));
				this._sendReply(reader.getMethod(), reader.seqNum());

				if (this.pushThreadHandler != null)
				{
					if (this.pushQueue.Count > this.pushQueueMaxSize)
					{
						if (this.pushFullCb != null)
							this.pushFullCb();
					}
					else {
						lock (this.pushQueue)
						{
							this.pushQueue.Enqueue(reader);
						}
						this.pushEvent.Set();
					}
				}
			}
		}

		private void PushHandlerThread ()
		{
			Queue transferQueue = new Queue ();
			while (this.pushEvent.WaitOne())
			{
				if (stopSignal.WaitOne(0))
					break;

				lock (this.pushQueue)
				{
					foreach (FPQReader reader in pushQueue)
					{
						transferQueue.Enqueue(reader);
					}
					pushQueue.Clear();
				}

				foreach (FPQReader reader in transferQueue)
				{
					MethodInfo mi = this.processor.GetType().GetMethod(reader.getMethod ());
					if (mi != null)
					{
						try
						{
							mi.Invoke(this.processor, new object[] { reader });
						}
						catch (Exception e)
						{
							ErrorRecorderHolder.recordError(e);
						}
					}
				}
				transferQueue.Clear();
			}
		}

		private void _stop ()
		{
			this.isStop = true;
			this.stopSignal.Set();
			lock (opLocker)
			{
				this.sock.close();
				this.isConnected = false;
			}
				
			if (this.receiveThreadHandler != null) {
				this.receiveThreadHandler.Join ();
				this.receiveThreadHandler = null;
				lock (this.startRecvLocker)
				{
					this.isRecvThreadStart = false;
				}
			}

			if (this.pushThreadHandler != null) {
				this.pushEvent.Set();
				this.pushThreadHandler.Join();
				this.pushThreadHandler = null;     
			}
			this.stopSignal.Reset();
		}

		~TCPClient()
		{
			_stop();
		}
	}
}

