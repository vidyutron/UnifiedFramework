using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.Chrome
{
	public class ChromeDriver : RemoteWebDriver
	{
		public static readonly bool AcceptUntrustedCertificates = true;

		public override IFileDetector FileDetector
		{
			get
			{
				return base.FileDetector;
			}
			set
			{
			}
		}

		public ChromeDriver() : this(new ChromeOptions())
		{
		}

		public ChromeDriver(ChromeOptions options) : this(ChromeDriverService.CreateDefaultService(), options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public ChromeDriver(ChromeDriverService service) : this(service, new ChromeOptions())
		{
		}

		public ChromeDriver(string chromeDriverDirectory) : this(chromeDriverDirectory, new ChromeOptions())
		{
		}

		public ChromeDriver(string chromeDriverDirectory, ChromeOptions options) : this(chromeDriverDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public ChromeDriver(string chromeDriverDirectory, ChromeOptions options, TimeSpan commandTimeout) : this(ChromeDriverService.CreateDefaultService(chromeDriverDirectory), options, commandTimeout)
		{
		}

		public ChromeDriver(ChromeDriverService service, ChromeOptions options) : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public ChromeDriver(ChromeDriverService service, ChromeOptions options, TimeSpan commandTimeout) : base(new DriverServiceCommandExecutor(service, commandTimeout), ChromeDriver.ConvertOptionsToCapabilities(options))
		{
		}

		private static ICapabilities ConvertOptionsToCapabilities(ChromeOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options", "options must not be null");
			}
			return options.ToCapabilities();
		}
	}
}
