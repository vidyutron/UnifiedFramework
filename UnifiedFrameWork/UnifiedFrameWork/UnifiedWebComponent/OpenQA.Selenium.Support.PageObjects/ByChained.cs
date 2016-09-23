using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace OpenQA.Selenium.Support.PageObjects
{
	public class ByChained : By
	{
		private readonly By[] bys;

		public ByChained(params By[] bys)
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
			List<IWebElement> list = null;
			By[] array = this.bys;
			for (int i = 0; i < array.Length; i++)
			{
				By by = array[i];
				List<IWebElement> list2 = new List<IWebElement>();
				if (list == null)
				{
					list2.AddRange(by.FindElements(context));
				}
				else
				{
					foreach (IWebElement current in list)
					{
						list2.AddRange(current.FindElements(by));
					}
				}
				list = list2;
			}
			return list.AsReadOnly();
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
			return string.Format(CultureInfo.InvariantCulture, "By.Chained([{0}])", new object[]
			{
				stringBuilder.ToString()
			});
		}
	}
}
