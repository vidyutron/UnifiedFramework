using System;

namespace OpenQA.Selenium.Support.Events
{
	public class WebDriverScriptEventArgs : EventArgs
	{
		private IWebDriver driver;

		private string script;

		public IWebDriver Driver
		{
			get
			{
				return this.driver;
			}
		}

		public string Script
		{
			get
			{
				return this.script;
			}
		}

		public WebDriverScriptEventArgs(IWebDriver driver, string script)
		{
			this.driver = driver;
			this.script = script;
		}
	}
}
