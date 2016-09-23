using System;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Internal
{
	public interface IFindsByPartialLinkText
	{
		IWebElement FindElementByPartialLinkText(string partialLinkText);

		ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText);
	}
}
