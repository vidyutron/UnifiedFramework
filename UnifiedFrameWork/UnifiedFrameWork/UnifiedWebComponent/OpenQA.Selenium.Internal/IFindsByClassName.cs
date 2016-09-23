using System;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Internal
{
	public interface IFindsByClassName
	{
		IWebElement FindElementByClassName(string className);

		ReadOnlyCollection<IWebElement> FindElementsByClassName(string className);
	}
}
