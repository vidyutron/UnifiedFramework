using System;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Internal
{
	public interface IFindsByLinkText
	{
		IWebElement FindElementByLinkText(string linkText);

		ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText);
	}
}
