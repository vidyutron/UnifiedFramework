using System;

namespace OpenQA.Selenium.Internal
{
	public interface IWrapsDriver
	{
		IWebDriver WrappedDriver
		{
			get;
		}
	}
}
