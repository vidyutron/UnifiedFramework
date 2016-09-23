using System;

namespace OpenQA.Selenium.Support.Events
{
	public class WebDriverExceptionEventArgs : EventArgs
	{
		private Exception thrownException;

		private IWebDriver driver;

		public Exception ThrownException
		{
			get
			{
				return this.thrownException;
			}
		}

		public IWebDriver Driver
		{
			get
			{
				return this.driver;
			}
		}

		public WebDriverExceptionEventArgs(IWebDriver driver, Exception thrownException)
		{
			this.driver = driver;
			this.thrownException = thrownException;
		}
	}
}
