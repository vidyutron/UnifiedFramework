using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.Text;

namespace OpenQA.Selenium.Chrome
{
	public sealed class ChromeDriverService : DriverService
	{
		private const string DefaultChromeDriverServiceExecutableName = "chromedriver";

		private static readonly Uri ChromeDriverDownloadUrl = new Uri("http://chromedriver.storage.googleapis.com/index.html");

		private string logPath = string.Empty;

		private string urlPathPrefix = string.Empty;

		private string portServerAddress = string.Empty;

		private string whitelistedIpAddresses = string.Empty;

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

		public string WhitelistedIPAddresses
		{
			get
			{
				return this.whitelistedIpAddresses;
			}
			set
			{
				this.whitelistedIpAddresses = value;
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
				if (!string.IsNullOrEmpty(this.whitelistedIpAddresses))
				{
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, " -whitelisted-ips={0}", new object[]
					{
						this.whitelistedIpAddresses
					}));
				}
				return stringBuilder.ToString();
			}
		}

		private ChromeDriverService(string executablePath, string executableFileName, int port) : base(executablePath, port, executableFileName, ChromeDriverService.ChromeDriverDownloadUrl)
		{
		}

		public static ChromeDriverService CreateDefaultService()
		{
			string driverPath = DriverService.FindDriverServiceExecutable(ChromeDriverService.ChromeDriverServiceFileName(), ChromeDriverService.ChromeDriverDownloadUrl);
			return ChromeDriverService.CreateDefaultService(driverPath);
		}

		public static ChromeDriverService CreateDefaultService(string driverPath)
		{
			return ChromeDriverService.CreateDefaultService(driverPath, ChromeDriverService.ChromeDriverServiceFileName());
		}

		public static ChromeDriverService CreateDefaultService(string driverPath, string driverExecutableFileName)
		{
			return new ChromeDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
		}

		private static string ChromeDriverServiceFileName()
		{
			string text = "chromedriver";
			switch (Environment.OSVersion.Platform)
			{
			case PlatformID.Win32S:
			case PlatformID.Win32Windows:
			case PlatformID.Win32NT:
			case PlatformID.WinCE:
				text += ".exe";
				return text;
			case PlatformID.Unix:
			case PlatformID.MacOSX:
				return text;
			}
			if (Environment.OSVersion.Platform != (PlatformID)128)
			{
				throw new WebDriverException("Unsupported platform: " + Environment.OSVersion.Platform);
			}
			return text;
		}
	}
}
