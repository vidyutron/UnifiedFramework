using System;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Internal
{
	public interface IFindsByCssSelector
	{
		IWebElement FindElementByCssSelector(string cssSelector);

		ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector);
	}
}
