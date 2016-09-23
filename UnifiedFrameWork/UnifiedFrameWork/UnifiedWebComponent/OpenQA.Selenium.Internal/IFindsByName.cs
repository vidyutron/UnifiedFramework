using System;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Internal
{
	public interface IFindsByName
	{
		IWebElement FindElementByName(string name);

		ReadOnlyCollection<IWebElement> FindElementsByName(string name);
	}
}
