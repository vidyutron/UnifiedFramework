using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.Opera
{
	public class OperaDriver : RemoteWebDriver
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

		public OperaDriver() : this(new OperaOptions())
		{
		}

		public OperaDriver(OperaOptions options) : this(OperaDriverService.CreateDefaultService(), options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public OperaDriver(string operaDriverDirectory) : this(operaDriverDirectory, new OperaOptions())
		{
		}

		public OperaDriver(string operaDriverDirectory, OperaOptions options) : this(operaDriverDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public OperaDriver(string operaDriverDirectory, OperaOptions options, TimeSpan commandTimeout) : this(OperaDriverService.CreateDefaultService(operaDriverDirectory), options, commandTimeout)
		{
		}

		public OperaDriver(OperaDriverService service, OperaOptions options) : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public OperaDriver(OperaDriverService service, OperaOptions options, TimeSpan commandTimeout) : base(new DriverServiceCommandExecutor(service, commandTimeout), OperaDriver.ConvertOptionsToCapabilities(options))
		{
		}

		private static ICapabilities ConvertOptionsToCapabilities(OperaOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options", "options must not be null");
			}
			return options.ToCapabilities();
		}
	}
}
