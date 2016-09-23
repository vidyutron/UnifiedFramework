using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.Text;

namespace OpenQA.Selenium.Opera
{
	public sealed class OperaDriverService : DriverService
	{
		private const string OperaDriverServiceFileName = "operadriver.exe";

		private static readonly Uri OperaDriverDownloadUrl = new Uri("https://github.com/operasoftware/operachromiumdriver/releases");

		private string logPath = string.Empty;

		private string urlPathPrefix = string.Empty;

		private string portServerAddress = string.Empty;

		private int adbPort = -1;

		private bool enableVerboseLogging;

		public string LogPath
		{
			get
			{
				return this.logPath;
			}
			set
			{
				this.logPath = value;
			}
		}

		public string UrlPathPrefix
		{
			get
			{
				return this.urlPathPrefix;
			}
			set
			{
				this.urlPathPrefix = value;
			}
		}

		public string PortServerAddress
		{
			get
			{
				return this.portServerAddress;
			}
			set
			{
				this.portServerAddress = value;
			}
		}

		public int AndroidDebugBridgePort
		{
			get
			{
				return this.adbPort;
			}
			set
			{
				this.adbPort = value;
			}
		}

		public bool EnableVerboseLogging
		{
			get
			{
				return this.enableVerboseLogging;
			}
			set
			{
				this.enableVerboseLogging = value;
			}
		}

		protected override string CommandLineArguments
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(base.CommandLineArguments);
				if (this.adbPort > 0)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --adb-port={0}", new object[]
					{
						this.adbPort
					});
				}
				if (base.SuppressInitialDiagnosticInformation)
				{
					stringBuilder.Append(" --silent");
				}
				if (this.enableVerboseLogging)
				{
					stringBuilder.Append(" --verbose");
				}
				if (!string.IsNullOrEmpty(this.logPath))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --log-path={0}", new object[]
					{
						this.logPath
					});
				}
				if (!string.IsNullOrEmpty(this.urlPathPrefix))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --url-base={0}", new object[]
					{
						this.urlPathPrefix
					});
				}
				if (!string.IsNullOrEmpty(this.portServerAddress))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --port-server={0}", new object[]
					{
						this.portServerAddress
					});
				}
				return stringBuilder.ToString();
			}
		}

		private OperaDriverService(string executablePath, string executableFileName, int port) : base(executablePath, port, executableFileName, OperaDriverService.OperaDriverDownloadUrl)
		{
		}

		public static OperaDriverService CreateDefaultService()
		{
			string driverPath = DriverService.FindDriverServiceExecutable("operadriver.exe", OperaDriverService.OperaDriverDownloadUrl);
			return OperaDriverService.CreateDefaultService(driverPath);
		}

		public static OperaDriverService CreateDefaultService(string driverPath)
		{
			return OperaDriverService.CreateDefaultService(driverPath, "operadriver.exe");
		}

		public static OperaDriverService CreateDefaultService(string driverPath, string driverExecutableFileName)
		{
			return new OperaDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
		}
	}
}
