using OpenQA.Selenium.Firefox.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OpenQA.Selenium.Firefox
{
	public class FirefoxDriverServer : ICommandServer, IDisposable
	{
		private string host;

		private List<IPEndPoint> addresses = new List<IPEndPoint>();

		private FirefoxProfile profile;

		private FirefoxBinary process;

		private Uri extensionUri;

		public Uri ExtensionUri
		{
			get
			{
				return this.extensionUri;
			}
		}

		public FirefoxDriverServer(FirefoxBinary binary, FirefoxProfile profile, string host)
		{
			this.host = host;
			if (profile == null)
			{
				this.profile = new FirefoxProfile();
			}
			else
			{
				this.profile = profile;
			}
			if (binary == null)
			{
				this.process = new FirefoxBinary();
				return;
			}
			this.process = binary;
		}

		public void Start()
		{
			using (ILock @lock = new SocketLock(this.profile.Port - 1))
			{
				@lock.LockObject(this.process.Timeout);
				try
				{
					int num = FirefoxDriverServer.DetermineNextFreePort(this.host, this.profile.Port);
					this.profile.Port = num;
					this.profile.WriteToDisk();
					this.process.StartProfile(this.profile, new string[]
					{
						"-foreground"
					});
					this.SetAddress(num);
					this.extensionUri = new Uri(string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}/hub/", new object[]
					{
						this.host,
						num
					}));
					this.ConnectToBrowser(this.process.Timeout);
				}
				finally
				{
					@lock.UnlockObject();
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.process.Dispose();
				this.profile.Clean();
			}
		}

		private static int DetermineNextFreePort(string host, int port)
		{
			int i;
			for (i = port; i < port + 200; i++)
			{
				using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				{
					socket.ExclusiveAddressUse = true;
					IPHostEntry hostEntry = Dns.GetHostEntry(host);
					IPAddress address = IPAddress.Parse("127.0.0.1");
					IPAddress[] addressList = hostEntry.AddressList;
					for (int j = 0; j < addressList.Length; j++)
					{
						IPAddress iPAddress = addressList[j];
						if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
						{
							address = iPAddress;
							break;
						}
					}
					IPEndPoint localEP = new IPEndPoint(address, i);
					try
					{
						socket.Bind(localEP);
						return i;
					}
					catch (SocketException)
					{
					}
				}
			}
			throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "Cannot find free port in the range {0} to {1} ", new object[]
			{
				port,
				i
			}));
		}

		private static List<IPEndPoint> ObtainLoopbackAddresses(int port)
		{
			List<IPEndPoint> list = new List<IPEndPoint>();
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			NetworkInterface[] array = allNetworkInterfaces;
			for (int i = 0; i < array.Length; i++)
			{
				NetworkInterface networkInterface = array[i];
				IPInterfaceProperties iPProperties = networkInterface.GetIPProperties();
				foreach (IPAddressInformation current in iPProperties.UnicastAddresses)
				{
					if (current.Address.AddressFamily == AddressFamily.InterNetwork && IPAddress.IsLoopback(current.Address))
					{
						list.Add(new IPEndPoint(current.Address, port));
					}
				}
			}
			return list;
		}

		private static bool IsSocketConnected(Socket extensionSocket)
		{
			return extensionSocket != null && extensionSocket.Connected;
		}

		private void SetAddress(int port)
		{
			if (string.Compare("localhost", this.host, StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.addresses = FirefoxDriverServer.ObtainLoopbackAddresses(port);
			}
			else
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(this.host);
				IPAddress address = IPAddress.Parse("127.0.0.1");
				IPAddress[] addressList = hostEntry.AddressList;
				for (int i = 0; i < addressList.Length; i++)
				{
					IPAddress iPAddress = addressList[i];
					if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						address = iPAddress;
						break;
					}
				}
				IPEndPoint item = new IPEndPoint(address, port);
				this.addresses.Add(item);
			}
			if (this.addresses.Count == 0)
			{
				throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "Could not find any IPv4 addresses for host '{0}'", new object[]
				{
					this.host
				}));
			}
		}

		private void ConnectToBrowser(TimeSpan timeToWait)
		{
			Socket socket = null;
			DateTime t = DateTime.Now.AddMilliseconds(timeToWait.TotalMilliseconds);
			try
			{
				while (!FirefoxDriverServer.IsSocketConnected(socket) && t > DateTime.Now)
				{
					foreach (IPEndPoint current in this.addresses)
					{
						try
						{
							socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
							socket.Connect(current);
							break;
						}
						catch (SocketException)
						{
							Thread.Sleep(250);
						}
					}
				}
				if (!FirefoxDriverServer.IsSocketConnected(socket))
				{
					if (socket == null || socket.RemoteEndPoint == null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						foreach (IPEndPoint current2 in this.addresses)
						{
							if (stringBuilder.Length > 0)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
							{
								current2.Address.ToString(),
								current2.Port.ToString(CultureInfo.InvariantCulture)
							});
						}
						throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "Failed to start up socket within {0} milliseconds. Attempted to connect to the following addresses: {1}", new object[]
						{
							timeToWait.TotalMilliseconds,
							stringBuilder.ToString()
						}));
					}
					IPEndPoint iPEndPoint = (IPEndPoint)socket.RemoteEndPoint;
					string message = string.Format(CultureInfo.InvariantCulture, "Unable to connect to host {0} on port {1} after {2} milliseconds", new object[]
					{
						iPEndPoint.Address.ToString(),
						iPEndPoint.Port.ToString(CultureInfo.InvariantCulture),
						timeToWait.TotalMilliseconds
					});
					throw new WebDriverException(message);
				}
			}
			finally
			{
				socket.Close();
			}
		}
	}
}
