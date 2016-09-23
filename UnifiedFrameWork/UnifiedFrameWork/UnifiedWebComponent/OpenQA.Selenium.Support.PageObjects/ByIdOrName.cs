using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace OpenQA.Selenium.Support.PageObjects
{
	public class ByIdOrName : By
	{
		private string elementIdentifier = string.Empty;

		private By idFinder;

		private By nameFinder;

		public ByIdOrName(string elementIdentifier)
		{
			if (string.IsNullOrEmpty(elementIdentifier))
			{
				throw new ArgumentException("element identifier cannot be null or the empty string", "elementIdentifier");
			}
			this.elementIdentifier = elementIdentifier;
			this.idFinder = By.Id(this.elementIdentifier);
			this.nameFinder = By.Name(this.elementIdentifier);
		}

		public override IWebElement FindElement(ISearchContext context)
		{
			IWebElement result;
			try
			{
				result = this.idFinder.FindElement(context);
			}
			catch (NoSuchElementException)
			{
				result = this.nameFinder.FindElement(context);
			}
			return result;
		}

		public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
		{
			List<IWebElement> list = new List<IWebElement>();
			list.AddRange(this.idFinder.FindElements(context));
			list.AddRange(this.nameFinder.FindElements(context));
			return list.AsReadOnly();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ByIdOrName([{0}])", new object[]
			{
				this.elementIdentifier
			});
		}
	}
}
