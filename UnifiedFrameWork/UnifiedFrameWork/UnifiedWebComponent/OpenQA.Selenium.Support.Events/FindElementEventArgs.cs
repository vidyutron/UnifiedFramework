using System;

namespace OpenQA.Selenium.Support.Events
{
	public class FindElementEventArgs : EventArgs
	{
		private IWebDriver driver;

		private IWebElement element;

		private By method;

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

		public By FindMethod
		{
			get
			{
				return this.method;
			}
		}

		public FindElementEventArgs(IWebDriver driver, By method) : this(driver, null, method)
		{
		}

		public FindElementEventArgs(IWebDriver driver, IWebElement element, By method)
		{
			this.driver = driver;
			this.element = element;
			this.method = method;
		}
	}
}
