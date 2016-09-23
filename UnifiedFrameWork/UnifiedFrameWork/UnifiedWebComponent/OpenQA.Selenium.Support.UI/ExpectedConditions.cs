using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenQA.Selenium.Support.UI
{
	public sealed class ExpectedConditions
	{
		private ExpectedConditions()
		{
		}

		public static Func<IWebDriver, bool> TitleIs(string title)
		{
			return (IWebDriver driver) => title == driver.Title;
		}

		public static Func<IWebDriver, bool> TitleContains(string title)
		{
			return (IWebDriver driver) => driver.Title.Contains(title);
		}

		public static Func<IWebDriver, bool> UrlToBe(string url)
		{
			return (IWebDriver driver) => driver.Url.ToLowerInvariant().Equals(url.ToLowerInvariant());
		}

		public static Func<IWebDriver, bool> UrlContains(string fraction)
		{
			return (IWebDriver driver) => driver.Url.ToLowerInvariant().Contains(fraction.ToLowerInvariant());
		}

		public static Func<IWebDriver, bool> UrlMatches(string regex)
		{
			return delegate(IWebDriver driver)
			{
				string url = driver.Url;
				Regex regex2 = new Regex(regex, RegexOptions.IgnoreCase);
				Match match = regex2.Match(url);
				return match.Success;
			};
		}

		public static Func<IWebDriver, IWebElement> ElementExists(By locator)
		{
			return (IWebDriver driver) => driver.FindElement(locator);
		}

		public static Func<IWebDriver, IWebElement> ElementIsVisible(By locator)
		{
			return delegate(IWebDriver driver)
			{
				IWebElement result;
				try
				{
					result = ExpectedConditions.ElementIfVisible(driver.FindElement(locator));
				}
				catch (StaleElementReferenceException)
				{
					result = null;
				}
				return result;
			};
		}

		public static Func<IWebDriver, ReadOnlyCollection<IWebElement>> VisibilityOfAllElementsLocatedBy(By locator)
		{
			return delegate(IWebDriver driver)
			{
				ReadOnlyCollection<IWebElement> result;
				try
				{
					ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(locator);
					if (readOnlyCollection.Any((IWebElement element) => !element.Displayed))
					{
						result = null;
					}
					else
					{
						result = (readOnlyCollection.Any<IWebElement>() ? readOnlyCollection : null);
					}
				}
				catch (StaleElementReferenceException)
				{
					result = null;
				}
				return result;
			};
		}

		public static Func<IWebDriver, ReadOnlyCollection<IWebElement>> VisibilityOfAllElementsLocatedBy(ReadOnlyCollection<IWebElement> elements)
		{
			return delegate(IWebDriver driver)
			{
				ReadOnlyCollection<IWebElement> result;
				try
				{
					if (elements.Any((IWebElement element) => !element.Displayed))
					{
						result = null;
					}
					else
					{
						result = (elements.Any<IWebElement>() ? elements : null);
					}
				}
				catch (StaleElementReferenceException)
				{
					result = null;
				}
				return result;
			};
		}

		public static Func<IWebDriver, ReadOnlyCollection<IWebElement>> PresenceOfAllElementsLocatedBy(By locator)
		{
			return delegate(IWebDriver driver)
			{
				ReadOnlyCollection<IWebElement> result;
				try
				{
					ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(locator);
					result = (readOnlyCollection.Any<IWebElement>() ? readOnlyCollection : null);
				}
				catch (StaleElementReferenceException)
				{
					result = null;
				}
				return result;
			};
		}

		public static Func<IWebDriver, bool> TextToBePresentInElement(IWebElement element, string text)
		{
			return delegate(IWebDriver driver)
			{
				bool result;
				try
				{
					string text2 = element.Text;
					result = text2.Contains(text);
				}
				catch (StaleElementReferenceException)
				{
					result = false;
				}
				return result;
			};
		}

		public static Func<IWebDriver, bool> TextToBePresentInElementLocated(By locator, string text)
		{
			return delegate(IWebDriver driver)
			{
				bool result;
				try
				{
					IWebElement webElement = driver.FindElement(locator);
					string text2 = webElement.Text;
					result = text2.Contains(text);
				}
				catch (StaleElementReferenceException)
				{
					result = false;
				}
				return result;
			};
		}

		public static Func<IWebDriver, bool> TextToBePresentInElementValue(IWebElement element, string text)
		{
			return delegate(IWebDriver driver)
			{
				bool result;
				try
				{
					string attribute = element.GetAttribute("value");
					if (attribute != null)
					{
						result = attribute.Contains(text);
					}
					else
					{
						result = false;
					}
				}
				catch (StaleElementReferenceException)
				{
					result = false;
				}
				return result;
			};
		}

		public static Func<IWebDriver, bool> TextToBePresentInElementValue(By locator, string text)
		{
			return delegate(IWebDriver driver)
			{
				bool result;
				try
				{
					IWebElement webElement = driver.FindElement(locator);
					string attribute = webElement.GetAttribute("value");
					if (attribute != null)
					{
						result = attribute.Contains(text);
					}
					else
					{
						result = false;
					}
				}
				catch (StaleElementReferenceException)
				{
					result = false;
				}
				return result;
			};
		}

		public static Func<IWebDriver, IWebDriver> FrameToBeAvailableAndSwitchToIt(string frameLocator)
		{
			return delegate(IWebDriver driver)
			{
				IWebDriver result;
				try
				{
					result = driver.SwitchTo().Frame(frameLocator);
				}
				catch (NoSuchFrameException)
				{
					result = null;
				}
				return result;
			};
		}

		public static Func<IWebDriver, IWebDriver> FrameToBeAvailableAndSwitchToIt(By locator)
		{
			return delegate(IWebDriver driver)
			{
				IWebDriver result;
				try
				{
					IWebElement frameElement = driver.FindElement(locator);
					result = driver.SwitchTo().Frame(frameElement);
				}
				catch (NoSuchFrameException)
				{
					result = null;
				}
				return result;
			};
		}

		public static Func<IWebDriver, bool> InvisibilityOfElementLocated(By locator)
		{
			return delegate(IWebDriver driver)
			{
				bool result;
				try
				{
					IWebElement webElement = driver.FindElement(locator);
					result = !webElement.Displayed;
				}
				catch (NoSuchElementException)
				{
					result = true;
				}
				catch (StaleElementReferenceException)
				{
					result = true;
				}
				return result;
			};
		}

		public static Func<IWebDriver, bool> InvisibilityOfElementWithText(By locator, string text)
		{
			return delegate(IWebDriver driver)
			{
				bool result;
				try
				{
					IWebElement webElement = driver.FindElement(locator);
					string text2 = webElement.Text;
					if (string.IsNullOrEmpty(text2))
					{
						result = true;
					}
					else
					{
						result = !text2.Equals(text);
					}
				}
				catch (NoSuchElementException)
				{
					result = true;
				}
				catch (StaleElementReferenceException)
				{
					result = true;
				}
				return result;
			};
		}

		public static Func<IWebDriver, IWebElement> ElementToBeClickable(By locator)
		{
			return delegate(IWebDriver driver)
			{
				IWebElement webElement = ExpectedConditions.ElementIfVisible(driver.FindElement(locator));
				IWebElement result;
				try
				{
					if (webElement != null && webElement.Enabled)
					{
						result = webElement;
					}
					else
					{
						result = null;
					}
				}
				catch (StaleElementReferenceException)
				{
					result = null;
				}
				return result;
			};
		}

		public static Func<IWebDriver, IWebElement> ElementToBeClickable(IWebElement element)
		{
			return delegate(IWebDriver driver)
			{
				IWebElement result;
				try
				{
					if (element != null && element.Displayed && element.Enabled)
					{
						result = element;
					}
					else
					{
						result = null;
					}
				}
				catch (StaleElementReferenceException)
				{
					result = null;
				}
				return result;
			};
		}

		public static Func<IWebDriver, bool> StalenessOf(IWebElement element)
		{
			return delegate(IWebDriver driver)
			{
				bool result;
				try
				{
					result = (element == null || !element.Enabled);
				}
				catch (StaleElementReferenceException)
				{
					result = true;
				}
				return result;
			};
		}

		public static Func<IWebDriver, bool> ElementToBeSelected(IWebElement element)
		{
			return ExpectedConditions.ElementSelectionStateToBe(element, true);
		}

		public static Func<IWebDriver, bool> ElementToBeSelected(IWebElement element, bool selected)
		{
			return (IWebDriver driver) => element.Selected == selected;
		}

		public static Func<IWebDriver, bool> ElementSelectionStateToBe(IWebElement element, bool selected)
		{
			return (IWebDriver driver) => element.Selected == selected;
		}

		public static Func<IWebDriver, bool> ElementToBeSelected(By locator)
		{
			return ExpectedConditions.ElementSelectionStateToBe(locator, true);
		}

		public static Func<IWebDriver, bool> ElementSelectionStateToBe(By locator, bool selected)
		{
			return delegate(IWebDriver driver)
			{
				bool result;
				try
				{
					IWebElement webElement = driver.FindElement(locator);
					result = (webElement.Selected == selected);
				}
				catch (StaleElementReferenceException)
				{
					result = false;
				}
				return result;
			};
		}

		public static Func<IWebDriver, IAlert> AlertIsPresent()
		{
			return delegate(IWebDriver driver)
			{
				IAlert result;
				try
				{
					result = driver.SwitchTo().Alert();
				}
				catch (NoAlertPresentException)
				{
					result = null;
				}
				return result;
			};
		}

		public static Func<IWebDriver, bool> AlertState(bool state)
		{
			return delegate(IWebDriver driver)
			{
				bool result;
				try
				{
					driver.SwitchTo().Alert();
					bool flag = true;
					result = (flag == state);
				}
				catch (NoAlertPresentException)
				{
					bool flag = false;
					result = (flag == state);
				}
				return result;
			};
		}

		private static IWebElement ElementIfVisible(IWebElement element)
		{
			if (!element.Displayed)
			{
				return null;
			}
			return element;
		}
	}
}
