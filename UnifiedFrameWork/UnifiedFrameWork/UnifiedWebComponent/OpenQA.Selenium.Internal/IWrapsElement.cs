using System;

namespace OpenQA.Selenium.Internal
{
	public interface IWrapsElement
	{
		IWebElement WrappedElement
		{
			get;
		}
	}
}
