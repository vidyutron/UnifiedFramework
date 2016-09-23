using System;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteOptions : IOptions
	{
		private RemoteWebDriver driver;

		public ICookieJar Cookies
		{
			get
			{
				return new RemoteCookieJar(this.driver);
			}
		}

		public IWindow Window
		{
			get
			{
				return new RemoteWindow(this.driver);
			}
		}

		public RemoteOptions(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public ITimeouts Timeouts()
		{
			return new RemoteTimeouts(this.driver);
		}
	}
}
