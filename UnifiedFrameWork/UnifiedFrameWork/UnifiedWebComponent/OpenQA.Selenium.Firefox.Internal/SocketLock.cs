using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OpenQA.Selenium.Firefox.Internal
{
	internal class SocketLock : ILock, IDisposable
	{
		private static int delayBetweenSocketChecks = 100;

		private int lockPort;

		private Socket lockSocket;

		public SocketLock(int lockPort)
		{
			this.lockPort = lockPort;
			this.lockSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.PreventSocketInheritance();
		}

		public void LockObject(TimeSpan timeout)
		{
			IPHostEntry hostEntry = Dns.GetHostEntry("localhost");
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
			IPEndPoint address2 = new IPEndPoint(address, this.lockPort);
			DateTime t = DateTime.Now.Add(timeout);
			do
			{
				try
				{
					if (this.IsLockFree(address2))
					{
						return;
					}
					Thread.Sleep(SocketLock.delayBetweenSocketChecks);
				}
				catch (ThreadInterruptedException innerException)
				{
					throw new WebDriverException("the thread was interrupted", innerException);
				}
				catch (IOException innerException2)
				{
					throw new WebDriverException("An unexpected error occurred", innerException2);
				}
			}
			while (DateTime.Now < t);
			throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "Unable to bind to locking port {0} within {1} milliseconds", new object[]
			{
				this.lockPort,
				timeout.TotalMilliseconds
			}));
		}

		[Obsolete("Timeouts should be expressed as a TimeSpan. Use the LockObject overload taking a TimeSpan parameter instead")]
		public void LockObject(long timeoutInMilliseconds)
		{
			this.LockObject(TimeSpan.FromMilliseconds((double)timeoutInMilliseconds));
		}

		public void UnlockObject()
		{
			try
			{
				this.lockSocket.Close();
			}
			catch (IOException innerException)
			{
				throw new WebDriverException("An error occured unlocking the object", innerException);
			}
		}

		public void Dispose()
		{
			if (this.lockSocket != null && this.lockSocket.Connected)
			{
				this.lockSocket.Close();
			}
			GC.SuppressFinalize(this);
		}

		private bool IsLockFree(IPEndPoint address)
		{
			bool result;
			try
			{
				this.lockSocket.Bind(address);
				result = true;
			}
			catch (SocketException)
			{
				result = false;
			}
			return result;
		}

		private void PreventSocketInheritance()
		{
			if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
			{
				NativeMethods.SetHandleInformation(this.lockSocket.Handle, NativeMethods.HandleInformation.Inherit | NativeMethods.HandleInformation.ProtectFromClose, NativeMethods.HandleInformation.None);
			}
		}
	}
}
