using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace OpenQA.Selenium.Support.PageObjects
{
	public class RetryingElementLocator : IElementLocator
	{
		private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5.0);

		private static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromMilliseconds(500.0);

		private ISearchContext searchContext;

		private TimeSpan timeout;

		private TimeSpan pollingInterval;

		public ISearchContext SearchContext
		{
			get
			{
				return this.searchContext;
			}
		}

		public RetryingElementLocator(ISearchContext searchContext) : this(searchContext, RetryingElementLocator.DefaultTimeout)
		{
		}

		public RetryingElementLocator(ISearchContext searchContext, TimeSpan timeout) : this(searchContext, timeout, RetryingElementLocator.DefaultPollingInterval)
		{
		}

		public RetryingElementLocator(ISearchContext searchContext, TimeSpan timeout, TimeSpan pollingInterval)
		{
			this.searchContext = searchContext;
			this.timeout = timeout;
			this.pollingInterval = pollingInterval;
		}

		public IWebElement LocateElement(IEnumerable<By> bys)
		{
			if (bys == null)
			{
				throw new ArgumentNullException("bys", "List of criteria may not be null");
			}
			string text = null;
			DateTime t = DateTime.Now.Add(this.timeout);
			bool flag = DateTime.Now > t;
			while (!flag)
			{
				foreach (By current in bys)
				{
					try
					{
						IWebElement result = this.SearchContext.FindElement(current);
						return result;
					}
					catch (NoSuchElementException)
					{
						text = ((text == null) ? "Could not find element by: " : (text + ", or: ")) + current;
					}
				}
				flag = (DateTime.Now > t);
				if (!flag)
				{
					Thread.Sleep(this.pollingInterval);
					continue;
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
			DateTime t = DateTime.Now.Add(this.timeout);
			bool flag = DateTime.Now > t;
			while (!flag)
			{
				foreach (By current in bys)
				{
					ReadOnlyCollection<IWebElement> collection = this.SearchContext.FindElements(current);
					list.AddRange(collection);
				}
				flag = (list.Count != 0 || DateTime.Now > t);
				if (!flag)
				{
					Thread.Sleep(this.pollingInterval);
				}
			}
			return list.AsReadOnly();
		}
	}
}
