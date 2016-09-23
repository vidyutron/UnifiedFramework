using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Support.PageObjects
{
	public class ByAll : By
	{
		private readonly By[] bys;

		public ByAll(params By[] bys)
		{
			this.bys = bys;
		}

		public override IWebElement FindElement(ISearchContext context)
		{
			ReadOnlyCollection<IWebElement> readOnlyCollection = this.FindElements(context);
			if (readOnlyCollection.Count == 0)
			{
				throw new NoSuchElementException("Cannot locate an element using " + this.ToString());
			}
			return readOnlyCollection[0];
		}

		public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
		{
			if (this.bys.Length == 0)
			{
				return new List<IWebElement>().AsReadOnly();
			}
			IEnumerable<IWebElement> enumerable = null;
			By[] array = this.bys;
			for (int i = 0; i < array.Length; i++)
			{
				By by = array[i];
				ReadOnlyCollection<IWebElement> readOnlyCollection = by.FindElements(context);
				if (readOnlyCollection.Count == 0)
				{
					return new List<IWebElement>().AsReadOnly();
				}
				if (enumerable == null)
				{
					enumerable = readOnlyCollection;
				}
				else
				{
					enumerable = enumerable.Intersect(by.FindElements(context));
				}
			}
			return enumerable.ToList<IWebElement>().AsReadOnly();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			By[] array = this.bys;
			for (int i = 0; i < array.Length; i++)
			{
				By value = array[i];
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(value);
			}
			return string.Format(CultureInfo.InvariantCulture, "By.All([{0}])", new object[]
			{
				stringBuilder.ToString()
			});
		}
	}
}
