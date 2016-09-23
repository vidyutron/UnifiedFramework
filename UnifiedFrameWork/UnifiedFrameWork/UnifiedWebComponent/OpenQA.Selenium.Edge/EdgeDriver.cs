using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.Edge
{
	public class EdgeDriver : RemoteWebDriver
	{
		public EdgeDriver() : this(new EdgeOptions())
		{
		}

		public EdgeDriver(EdgeOptions options) : this(EdgeDriverService.CreateDefaultService(), options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public EdgeDriver(EdgeDriverService service) : this(service, new EdgeOptions())
		{
		}

		public EdgeDriver(string edgeDriverDirectory) : this(edgeDriverDirectory, new EdgeOptions())
		{
		}

		public EdgeDriver(string edgeDriverDirectory, EdgeOptions options) : this(edgeDriverDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public EdgeDriver(string edgeDriverDirectory, EdgeOptions options, TimeSpan commandTimeout) : this(EdgeDriverService.CreateDefaultService(edgeDriverDirectory), options, commandTimeout)
		{
		}

		public EdgeDriver(EdgeDriverService service, EdgeOptions options) : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public EdgeDriver(EdgeDriverService service, EdgeOptions options, TimeSpan commandTimeout) : base(new DriverServiceCommandExecutor(service, commandTimeout), EdgeDriver.ConvertOptionsToCapabilities(options))
		{
		}

		private static ICapabilities ConvertOptionsToCapabilities(EdgeOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options", "options must not be null");
			}
			return options.ToCapabilities();
		}
	}
}
