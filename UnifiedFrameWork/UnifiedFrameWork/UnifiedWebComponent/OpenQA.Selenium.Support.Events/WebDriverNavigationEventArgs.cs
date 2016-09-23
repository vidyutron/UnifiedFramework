using System;

namespace OpenQA.Selenium.Support.Events
{
	public class WebDriverNavigationEventArgs : EventArgs
	{
		private string url;

		private IWebDriver driver;

		public string Url
		{
			get
			{
				return this.url;
			}
		}

		public IWebDriver Driver
		{
			get
			{
				return this.driver;
			}
		}

		public WebDriverNavigationEventArgs(IWebDriver driver) : this(driver, null)
		{
		}

		public WebDriverNavigationEventArgs(IWebDriver driver, string url)
		{
			this.url = url;
			this.driver = driver;
		}
	}
}
