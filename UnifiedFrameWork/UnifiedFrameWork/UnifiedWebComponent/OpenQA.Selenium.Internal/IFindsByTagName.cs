using System;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Internal
{
	public interface IFindsByTagName
	{
		IWebElement FindElementByTagName(string tagName);

		ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName);
	}
}
