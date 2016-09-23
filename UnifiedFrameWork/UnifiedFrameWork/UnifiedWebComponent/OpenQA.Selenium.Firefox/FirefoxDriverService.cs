using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace OpenQA.Selenium.Firefox
{
	public sealed class FirefoxDriverService : DriverService
	{
		private const string FirefoxDriverServiceFileName = "wires.exe";

		private static readonly Uri FirefoxDriverDownloadUrl = new Uri("https://github.com/jgraham/wires/releases");

		private string browserBinaryPath = "C:\\Program Files (x86)\\Nightly\\firefox.exe";

		private int browserCommunicationPort = -1;

		public string FirefoxBinaryPath
		{
			get
			{
				return this.browserBinaryPath;
			}
			set
			{
				this.browserBinaryPath = value;
			}
		}

		public int BrowserCommunicationPort
		{
			get
			{
				return this.browserCommunicationPort;
			}
			set
			{
				this.browserCommunicationPort = value;
			}
		}

		protected override TimeSpan InitializationTimeout
		{
			get
			{
				return TimeSpan.FromSeconds(2.0);
			}
		}

		protected override TimeSpan TerminationTimeout
		{
			get
			{
				return TimeSpan.FromMilliseconds(100.0);
			}
		}

		protected override bool IsInitialized
		{
			get
			{
				bool result = false;
				try
				{
					Uri requestUri = new Uri(base.ServiceUrl, new Uri("/session/FakeSessionIdForPollingPurposes", UriKind.Relative));
					HttpWebRequest httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;
					httpWebRequest.KeepAlive = false;
					httpWebRequest.Timeout = 5000;
					httpWebRequest.Method = "DELETE";
					HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
					result = (httpWebResponse.StatusCode == HttpStatusCode.OK && httpWebResponse.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase));
					httpWebResponse.Close();
				}
				catch (WebException ex)
				{
					HttpWebResponse httpWebResponse2 = ex.Response as HttpWebResponse;
					if (httpWebResponse2 != null)
					{
						result = (httpWebResponse2.StatusCode == HttpStatusCode.InternalServerError && httpWebResponse2.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase));
					}
					else
					{
						Console.WriteLine(ex.Message);
					}
				}
				return result;
			}
		}

		protected override string CommandLineArguments
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.browserCommunicationPort > 0)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --marionette-port {0}", new object[]
					{
						this.browserCommunicationPort
					});
				}
				if (base.Port > 0)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --webdriver-port {0}", new object[]
					{
						base.Port
					});
				}
				if (!string.IsNullOrEmpty(this.browserBinaryPath))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --binary \"{0}\"", new object[]
					{
						this.browserBinaryPath
					});
				}
				return stringBuilder.ToString().Trim();
			}
		}

		private FirefoxDriverService(string executablePath, string executableFileName, int port) : base(executablePath, port, executableFileName, FirefoxDriverService.FirefoxDriverDownloadUrl)
		{
		}

		public static FirefoxDriverService CreateDefaultService()
		{
			string driverPath = DriverService.FindDriverServiceExecutable("wires.exe", FirefoxDriverService.FirefoxDriverDownloadUrl);
			return FirefoxDriverService.CreateDefaultService(driverPath);
		}

		public static FirefoxDriverService CreateDefaultService(string driverPath)
		{
			return FirefoxDriverService.CreateDefaultService(driverPath, "wires.exe");
		}

		public static FirefoxDriverService CreateDefaultService(string driverPath, string driverExecutableFileName)
		{
			return new FirefoxDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
		}
	}
}
