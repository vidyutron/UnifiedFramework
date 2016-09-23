using System;
using System.Net;
using System.Net.Sockets;

namespace OpenQA.Selenium.Internal
{
	internal static class PortUtilities
	{
		public static int FindFreePort()
		{
			int result = 0;
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
				socket.Bind(iPEndPoint);
				iPEndPoint = (IPEndPoint)socket.LocalEndPoint;
				result = iPEndPoint.Port;
			}
			finally
			{
				socket.Close();
			}
			return result;
		}
	}
}
