using System;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteNavigator : INavigation
	{
		private RemoteWebDriver driver;

		public RemoteNavigator(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public void Back()
		{
			this.driver.InternalExecute(DriverCommand.GoBack, null);
		}

		public void Forward()
		{
			this.driver.InternalExecute(DriverCommand.GoForward, null);
		}

		public void GoToUrl(string url)
		{
			this.driver.Url = url;
		}

		public void GoToUrl(Uri url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url", "URL cannot be null.");
			}
			this.driver.Url = url.ToString();
		}

		public void Refresh()
		{
			this.driver.InternalExecute(DriverCommand.Refresh, null);
		}
	}
}
