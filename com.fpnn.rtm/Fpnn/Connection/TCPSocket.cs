using System;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace Fpnn.Connection
{
	public class TCPSocket
	{
		private System.Net.Sockets.TcpClient client = null;
		private NetworkStream stream = null;
		private string host = null;
		private int port = 0;
		private int connectionTimeout = 20;

		public TCPSocket (string hostport) : this(hostport.Split(':')[0], Int32.Parse(hostport.Split(':')[1]))
		{
		}

		public TCPSocket (string host, int port) : this(host, port, 20)
		{
		}

		public TCPSocket (string host, int port, int connectionTimeout)
		{
			this.host = host;
			this.port = port;
			this.connectionTimeout = connectionTimeout;
			initSocket ();
		}

		private void initSocket ()
		{
			client = new System.Net.Sockets.TcpClient ();
		}
		
		public System.Net.Sockets.TcpClient TcpClient {
			get {
				return client;
			}
		}

		public string Host {
			get {
				return host;
			}
		}
		
		public int Port {
			get {
				return port;
			}
		}

		public int read (byte[] buf, int off, int len)
		{
			int got = 0;
			int ret = 0;
			
			while (got < len) {
				ret = stream.Read (buf, off + got, len - got);
				if (ret <= 0) {
					close ();
					throw new Exception ("Cannot read, Remote side has closed");
				}
				got += ret;
			}
			
			return got;
		}
		
		public void write (byte[] buffer, int offset, int size)
		{
			stream.Write (buffer, offset, size);
		}
		
		public void flush ()
		{
			stream.Flush ();
		}
		
		public bool isOpen {
			get {
				if (client == null) {
					return false;
				}
				
				return client.Connected;
			}
		}

		public string convertToIpv6(string ip)
		{
			string[] parts = ip.Split('.');
			if (parts.Length != 4)
				return "";
			foreach (string part in parts)
			{
				if (Int32.Parse(part) > 255)
					return "";
			}
			string part7 = Convert.ToString(Int32.Parse(parts[0]) * 256 + Int32.Parse(parts[1]), 16);
			string part8 = Convert.ToString(Int32.Parse(parts[2]) * 256 + Int32.Parse(parts[3]), 16);
			return "64:ff9b::" + part7 + ":" + part8;
		}

		public void open ()
		{
			if (String.IsNullOrEmpty (host)) {
				throw new System.Exception ("Cannot open null host");
			}
				
			if (port <= 0) {
				throw new System.Exception ("Cannot open without port");
			}

			if (client == null) {
				initSocket ();
			}
			
			IAsyncResult result;
			try
			{
				result = client.BeginConnect(host, port, null, null);
				var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(this.connectionTimeout));
				if (!success)
				{
					client.Close();
					client = null;
					throw new System.Exception("Connection timeout");
				}
				else
				{
					try
					{
						client.EndConnect(result);
						stream = client.GetStream();
					}
					catch
					{
						client.Close();
						client = null;
						throw new System.Exception("Connection error");

					}
				}
			}
			catch
			{
				try
				{
					IPHostEntry ips = Dns.GetHostEntry(host);
					if (ips.AddressList.Length <= 0)
						throw new System.Exception("Connection error");
					client = new System.Net.Sockets.TcpClient(AddressFamily.InterNetworkV6);
					string ipv6 = ips.AddressList[0].ToString();

					if (ips.AddressList[0].AddressFamily.ToString() != ProtocolFamily.InterNetworkV6.ToString())
						ipv6 = this.convertToIpv6(ips.AddressList[0].ToString());

					result = client.BeginConnect(ipv6, port, null, null);
				}
				catch
				{
					throw new System.Exception("Connection error");
				}
				var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(connectionTimeout));
				if (!success)
				{
					client.Close();
					client = null;
					throw new System.Exception("Connection timeout");
				}
				else
				{
					try
					{
						client.EndConnect(result);
						stream = client.GetStream();
					}
					catch
					{
						client.Close();
						client = null;
						throw new System.Exception("Connection error");
					}
				}
			}
		}
		
		public void close ()
		{
			if (client != null) {
				if (stream != null)
					stream.Close ();
				client.Close ();
				client = null;
			}
		}

	}
}

