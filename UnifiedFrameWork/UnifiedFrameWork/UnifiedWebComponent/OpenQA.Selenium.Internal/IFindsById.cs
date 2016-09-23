using System;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Internal
{
	public interface IFindsById
	{
		IWebElement FindElementById(string id);

		ReadOnlyCollection<IWebElement> FindElementsById(string id);
	}
}
