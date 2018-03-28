using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MsgPack;
using MsgPack.Serialization;


namespace Fpnn.Protocol
{
	public class FPReader
	{
		protected Unpacker unpacker;
		protected MemoryStream stream;
		protected bool readFirst = true;

		public FPReader (byte[] payload)
		{
			this.stream = new MemoryStream (payload);
			this.unpacker = Unpacker.Create(stream);
		}

		public FPReader (Unpacker unpacker)
		{
			this.stream = new MemoryStream ();
			this.unpacker = unpacker;
			this.readFirst = false;
		}

		public FPReader ()
		{
		}

		public void setPayload(byte[] payload)
		{
			this.stream = new MemoryStream(payload);
			this.unpacker = Unpacker.Create(stream);
		}

		public T get<T>(string key, T dft)
		{
			try
			{
				return want<T>(key);
			}
			catch (Exception)
			{
				return dft;
			}
		}

		public T want<T>(string key)
		{
			stream.Position = 0;
			if (readFirst && !unpacker.Read())
			{
				throw new Exception("Read answer error");
			}
			if (!unpacker.IsMapHeader)
			{
				throw new Exception("Answer is not a map object");
			}

			MessagePackSerializer<T> valueSerializer = MessagePackSerializer.Create<T>();
			MessagePackSerializer<object> otherSerializer = MessagePackSerializer.Create<object>();

			long count = unpacker.ItemsCount;

			for (int i = 0; i < count; i++)
			{
				if (!unpacker.Read())
				{
					throw new Exception("Read answer error");
				}

				string getKey = unpacker.Unpack<string>();

				if (!unpacker.Read())
				{
					throw new Exception("Read answer error");
				}

				if (getKey == key)
				{
					return valueSerializer.UnpackFrom(unpacker);
				}
				else {
					otherSerializer.UnpackFrom(unpacker);
					continue;
				}
			}
			throw new Exception("Cannot find value of key");
		}

		public Unpacker getObject(string key)
		{
			if (readFirst && !unpacker.Read())
			{
				throw new Exception("Read answer error");
			}
			if (!unpacker.IsMapHeader)
			{
				throw new Exception("Answer is not a map object");
			}

			MessagePackSerializer<object> otherSerializer = MessagePackSerializer.Create<object>();

			long count = unpacker.ItemsCount;

			for (int i = 0; i < count; i++)
			{
				if (!unpacker.Read())
				{
					throw new Exception("Read answer error");
				}

				string getKey = unpacker.Unpack<string>();

				if (!unpacker.Read())
				{
					throw new Exception("Read answer error");
				}

				if (getKey == key)
				{
					return unpacker;
				}
				else {
					otherSerializer.UnpackFrom(unpacker);
					continue;
				}
			}
			throw new Exception("Cannot find value of key");
		}
	}

	public class FPQReader : FPReader
	{
		public FPQuest quest;

		public FPQReader(FPQuest quest) : base()
		{
			byte[] pl = new byte[quest.psize - quest.ss];
			System.Buffer.BlockCopy(quest.payload, quest.ss, pl, 0, pl.Length);
			this.setPayload(pl);
			this.quest = quest;
		}

		public UInt32 seqNum()
		{
			return quest.seqNum;
		}

		public FPQuest getQuest()
		{
			return quest;
		}

		public string getMethod()
		{
			return quest.method;
		}
	}

	public class FPAReader : FPReader
	{
		public FPAnswer answer;
		private bool error = false;
		private int code = 0;
		private string ex = "";
		private string msg = "";
		private string raiser = "";

		public FPAReader (FPAnswer answer) : base(answer.payload)
		{
			this.answer = answer;
			this.error = this.answer.status != FP_ST.FP_ST_OK;
			if (this.error) {
				this.code = get<int> ("code", 0);
				this.ex = get<string> ("ex", "");
				this.msg = get<string> ("msg", "");
				this.raiser = get<string> ("raiser", "");
			}
		}

		public FPAReader (Unpacker unpacker) : base(unpacker)
		{
		}

		public FPAReader (int code, string ex, string msg, string raiser)
		{
			this.error = true;
			this.code = code;
			this.ex = ex;
			this.msg = msg;
			this.raiser = raiser;
		}

		public UInt32 seqNum ()
		{
			return answer.seqNum;
		}

		public FP_ST status ()
		{
			return answer.status;
		}

		public bool isError ()
		{
			return error;
		}

		public int errorCode ()
		{
			return code;
		}

		public string errorException ()
		{
			return ex;
		}

		public string errorMessage ()
		{
			return msg;
		}

		public string errorRaiser ()
		{
			return raiser;
		}

		public FPAnswer getAnswer ()
		{
			return answer;
		}
	}
}

