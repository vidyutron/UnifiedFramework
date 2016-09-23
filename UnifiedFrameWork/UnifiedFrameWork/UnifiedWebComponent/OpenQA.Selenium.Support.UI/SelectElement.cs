using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OpenQA.Selenium.Support.UI
{
	public class SelectElement : IWrapsElement
	{
		private readonly IWebElement element;

		public IWebElement WrappedElement
		{
			get
			{
				return this.element;
			}
		}

		public bool IsMultiple
		{
			get;
			private set;
		}

		public IList<IWebElement> Options
		{
			get
			{
				return this.element.FindElements(By.TagName("option"));
			}
		}

		public IWebElement SelectedOption
		{
			get
			{
				foreach (IWebElement current in this.Options)
				{
					if (current.Selected)
					{
						return current;
					}
				}
				throw new NoSuchElementException("No option is selected");
			}
		}

		public IList<IWebElement> AllSelectedOptions
		{
			get
			{
				List<IWebElement> list = new List<IWebElement>();
				foreach (IWebElement current in this.Options)
				{
					if (current.Selected)
					{
						list.Add(current);
					}
				}
				return list;
			}
		}

		public SelectElement(IWebElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element", "element cannot be null");
			}
			if (string.IsNullOrEmpty(element.TagName) || string.Compare(element.TagName, "select", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new UnexpectedTagNameException("select", element.TagName);
			}
			this.element = element;
			string attribute = element.GetAttribute("multiple");
			this.IsMultiple = (attribute != null && attribute.ToLowerInvariant() != "false");
		}

		public void SelectByText(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text", "text must not be null");
			}
			IList<IWebElement> list = this.element.FindElements(By.XPath(".//option[normalize-space(.) = " + SelectElement.EscapeQuotes(text) + "]"));
			bool flag = false;
			foreach (IWebElement current in list)
			{
				SelectElement.SetSelected(current);
				if (!this.IsMultiple)
				{
					return;
				}
				flag = true;
			}
			if (list.Count == 0 && text.Contains(" "))
			{
				string longestSubstringWithoutSpace = SelectElement.GetLongestSubstringWithoutSpace(text);
				IList<IWebElement> list2;
				if (string.IsNullOrEmpty(longestSubstringWithoutSpace))
				{
					list2 = this.element.FindElements(By.TagName("option"));
				}
				else
				{
					list2 = this.element.FindElements(By.XPath(".//option[contains(., " + SelectElement.EscapeQuotes(longestSubstringWithoutSpace) + ")]"));
				}
				foreach (IWebElement current2 in list2)
				{
					if (text == current2.Text)
					{
						SelectElement.SetSelected(current2);
						if (!this.IsMultiple)
						{
							return;
						}
						flag = true;
					}
				}
			}
			if (!flag)
			{
				throw new NoSuchElementException("Cannot locate element with text: " + text);
			}
		}

		public void SelectByValue(string value)
		{
			StringBuilder stringBuilder = new StringBuilder(".//option[@value = ");
			stringBuilder.Append(SelectElement.EscapeQuotes(value));
			stringBuilder.Append("]");
			IList<IWebElement> list = this.element.FindElements(By.XPath(stringBuilder.ToString()));
			bool flag = false;
			foreach (IWebElement current in list)
			{
				SelectElement.SetSelected(current);
				if (!this.IsMultiple)
				{
					return;
				}
				flag = true;
			}
			if (!flag)
			{
				throw new NoSuchElementException("Cannot locate option with value: " + value);
			}
		}

		public void SelectByIndex(int index)
		{
			string b = index.ToString(CultureInfo.InvariantCulture);
			bool flag = false;
			foreach (IWebElement current in this.Options)
			{
				if (current.GetAttribute("index") == b)
				{
					SelectElement.SetSelected(current);
					if (!this.IsMultiple)
					{
						return;
					}
					flag = true;
				}
			}
			if (!flag)
			{
				throw new NoSuchElementException("Cannot locate option with index: " + index);
			}
		}

		public void DeselectAll()
		{
			if (!this.IsMultiple)
			{
				throw new InvalidOperationException("You may only deselect all options if multi-select is supported");
			}
			foreach (IWebElement current in this.Options)
			{
				if (current.Selected)
				{
					current.Click();
				}
			}
		}

		public void DeselectByText(string text)
		{
			StringBuilder stringBuilder = new StringBuilder(".//option[normalize-space(.) = ");
			stringBuilder.Append(SelectElement.EscapeQuotes(text));
			stringBuilder.Append("]");
			IList<IWebElement> list = this.element.FindElements(By.XPath(stringBuilder.ToString()));
			foreach (IWebElement current in list)
			{
				if (current.Selected)
				{
					current.Click();
				}
			}
		}

		public void DeselectByValue(string value)
		{
			StringBuilder stringBuilder = new StringBuilder(".//option[@value = ");
			stringBuilder.Append(SelectElement.EscapeQuotes(value));
			stringBuilder.Append("]");
			IList<IWebElement> list = this.element.FindElements(By.XPath(stringBuilder.ToString()));
			foreach (IWebElement current in list)
			{
				if (current.Selected)
				{
					current.Click();
				}
			}
		}

		public void DeselectByIndex(int index)
		{
			string a = index.ToString(CultureInfo.InvariantCulture);
			foreach (IWebElement current in this.Options)
			{
				if (a == current.GetAttribute("index") && current.Selected)
				{
					current.Click();
				}
			}
		}

		private static string EscapeQuotes(string toEscape)
		{
			if (toEscape.IndexOf("\"", StringComparison.OrdinalIgnoreCase) > -1 && toEscape.IndexOf("'", StringComparison.OrdinalIgnoreCase) > -1)
			{
				bool flag = false;
				if (toEscape.LastIndexOf("\"", StringComparison.OrdinalIgnoreCase) == toEscape.Length - 1)
				{
					flag = true;
				}
				List<string> list = new List<string>(toEscape.Split(new char[]
				{
					'"'
				}));
				if (flag && string.IsNullOrEmpty(list[list.Count - 1]))
				{
					list.RemoveAt(list.Count - 1);
				}
				StringBuilder stringBuilder = new StringBuilder("concat(");
				for (int i = 0; i < list.Count; i++)
				{
					stringBuilder.Append("\"").Append(list[i]).Append("\"");
					if (i == list.Count - 1)
					{
						if (flag)
						{
							stringBuilder.Append(", '\"')");
						}
						else
						{
							stringBuilder.Append(")");
						}
					}
					else
					{
						stringBuilder.Append(", '\"', ");
					}
				}
				return stringBuilder.ToString();
			}
			if (toEscape.IndexOf("\"", StringComparison.OrdinalIgnoreCase) > -1)
			{
				return string.Format(CultureInfo.InvariantCulture, "'{0}'", new object[]
				{
					toEscape
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[]
			{
				toEscape
			});
		}

		private static string GetLongestSubstringWithoutSpace(string s)
		{
			string text = string.Empty;
			string[] array = s.Split(new char[]
			{
				' '
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text2 = array2[i];
				if (text2.Length > text.Length)
				{
					text = text2;
				}
			}
			return text;
		}

		private static void SetSelected(IWebElement option)
		{
			if (!option.Selected)
			{
				option.Click();
			}
		}
	}
}
