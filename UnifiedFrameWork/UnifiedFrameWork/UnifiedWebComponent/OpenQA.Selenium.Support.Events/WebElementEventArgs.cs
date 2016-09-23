using System;

namespace OpenQA.Selenium.Support.Events
{
	public class WebElementEventArgs : EventArgs
	{
		private IWebDriver driver;

		private IWebElement element;

		public IWebDriver Driver
		{
			get
			{
				return this.driver;
			}
		}

		public IWebElement Element
		{
			get
			{
				return this.element;
			}
		}

		public WebElementEventArgs(IWebDriver driver, IWebElement element)
		{
			this.driver = driver;
			this.element = element;
		}
	}
}
