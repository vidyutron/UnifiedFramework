using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Support.PageObjects
{
	public interface IElementLocator
	{
		ISearchContext SearchContext
		{
			get;
		}

		IWebElement LocateElement(IEnumerable<By> bys);

		ReadOnlyCollection<IWebElement> LocateElements(IEnumerable<By> bys);
	}
}
