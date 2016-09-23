using System;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Internal
{
	public interface IFindsByXPath
	{
		IWebElement FindElementByXPath(string xpath);

		ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath);
	}
}
