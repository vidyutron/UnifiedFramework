using OpenQA.Selenium.Internal;
using System;

namespace OpenQA.Selenium.Edge
{
	public sealed class EdgeDriverService : DriverService
	{
		private const string MicrosoftWebDriverServiceFileName = "MicrosoftWebDriver.exe";

		private static readonly Uri MicrosoftWebDriverDownloadUrl = new Uri("http://go.microsoft.com/fwlink/?LinkId=619687");

		private EdgeDriverService(string executablePath, string executableFileName, int port) : base(executablePath, port, executableFileName, EdgeDriverService.MicrosoftWebDriverDownloadUrl)
		{
		}

		public static EdgeDriverService CreateDefaultService()
		{
			string driverPath = DriverService.FindDriverServiceExecutable("MicrosoftWebDriver.exe", EdgeDriverService.MicrosoftWebDriverDownloadUrl);
			return EdgeDriverService.CreateDefaultService(driverPath);
		}

		public static EdgeDriverService CreateDefaultService(string driverPath)
		{
			return EdgeDriverService.CreateDefaultService(driverPath, "MicrosoftWebDriver.exe");
		}

		public static EdgeDriverService CreateDefaultService(string driverPath, string driverExecutableFileName)
		{
			return EdgeDriverService.CreateDefaultService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
		}

		public static EdgeDriverService CreateDefaultService(string driverPath, string driverExecutableFileName, int port)
		{
			return new EdgeDriverService(driverPath, driverExecutableFileName, port);
		}
	}
}
