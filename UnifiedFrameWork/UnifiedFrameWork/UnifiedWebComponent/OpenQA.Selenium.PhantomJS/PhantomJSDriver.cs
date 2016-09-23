using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.PhantomJS
{
	public class PhantomJSDriver : RemoteWebDriver
	{
		private const string CommandExecutePhantomScript = "executePhantomScript";

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

		public PhantomJSDriver() : this(new PhantomJSOptions())
		{
		}

		public PhantomJSDriver(PhantomJSOptions options) : this(PhantomJSDriverService.CreateDefaultService(), options, TimeSpan.FromSeconds(60.0))
		{
		}

		public PhantomJSDriver(PhantomJSDriverService service) : this(service, new PhantomJSOptions())
		{
		}

		public PhantomJSDriver(string phantomJSDriverServerDirectory) : this(phantomJSDriverServerDirectory, new PhantomJSOptions())
		{
		}

		public PhantomJSDriver(string phantomJSDriverServerDirectory, PhantomJSOptions options) : this(phantomJSDriverServerDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public PhantomJSDriver(string phantomJSDriverServerDirectory, PhantomJSOptions options, TimeSpan commandTimeout) : this(PhantomJSDriverService.CreateDefaultService(phantomJSDriverServerDirectory), options, commandTimeout)
		{
		}

		public PhantomJSDriver(PhantomJSDriverService service, PhantomJSOptions options) : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public PhantomJSDriver(PhantomJSDriverService service, PhantomJSOptions options, TimeSpan commandTimeout) : base(new DriverServiceCommandExecutor(service, commandTimeout, false), PhantomJSDriver.ConvertOptionsToCapabilities(options))
		{
			CommandInfo commandInfo = new CommandInfo("POST", "/session/{sessionId}/phantom/execute");
			base.CommandExecutor.CommandInfoRepository.TryAddCommand("executePhantomScript", commandInfo);
		}

		public object ExecutePhantomJS(string script, params object[] args)
		{
			return base.ExecuteScriptCommand(script, "executePhantomScript", args);
		}

		private static ICapabilities ConvertOptionsToCapabilities(PhantomJSOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options", "options must not be null");
			}
			return options.ToCapabilities();
		}
	}
}
