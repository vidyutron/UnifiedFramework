using OpenQA.Selenium.Html5;
using System;

namespace OpenQA.Selenium.Remote
{
	public class RemoteWebStorage : IWebStorage
	{
		private RemoteWebDriver driver;

		public ILocalStorage LocalStorage
		{
			get
			{
				return new RemoteLocalStorage(this.driver);
			}
		}

		public ISessionStorage SessionStorage
		{
			get
			{
				return new RemoteSessionStorage(this.driver);
			}
		}

		public RemoteWebStorage(RemoteWebDriver driver)
		{
			this.driver = driver;
		}
	}
}
