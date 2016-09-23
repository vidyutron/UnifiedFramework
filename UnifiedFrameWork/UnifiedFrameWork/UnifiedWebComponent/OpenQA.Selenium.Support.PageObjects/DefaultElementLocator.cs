using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Support.PageObjects
{
	public class DefaultElementLocator : IElementLocator
	{
		private ISearchContext searchContext;

		public ISearchContext SearchContext
		{
			get
			{
				return this.searchContext;
			}
		}

		public DefaultElementLocator(ISearchContext searchContext)
		{
			this.searchContext = searchContext;
		}

		public IWebElement LocateElement(IEnumerable<By> bys)
		{
			if (bys == null)
			{
				throw new ArgumentNullException("bys", "List of criteria may not be null");
			}
			string text = null;
			foreach (By current in bys)
			{
				try
				{
					return this.searchContext.FindElement(current);
				}
				catch (NoSuchElementException)
				{
					text = ((text == null) ? "Could not find element by: " : (text + ", or: ")) + current;
				}
			}
			throw new NoSuchElementException(text);
		}

		public ReadOnlyCollection<IWebElement> LocateElements(IEnumerable<By> bys)
		{
			if (bys == null)
			{
				throw new ArgumentNullException("bys", "List of criteria may not be null");
			}
			List<IWebElement> list = new List<IWebElement>();
			foreach (By current in bys)
			{
				ReadOnlyCollection<IWebElement> collection = this.searchContext.FindElements(current);
				list.AddRange(collection);
			}
			return list.AsReadOnly();
		}
	}
}
