using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.Firefox
{
	public class FirefoxWebElement : RemoteWebElement
	{
		public FirefoxWebElement(FirefoxDriver parentDriver, string id) : base(parentDriver, id)
		{
		}

		public override bool Equals(object obj)
		{
			IWebElement webElement = obj as IWebElement;
			if (webElement == null)
			{
				return false;
			}
			if (webElement is IWrapsElement)
			{
				webElement = ((IWrapsElement)obj).WrappedElement;
			}
			FirefoxWebElement firefoxWebElement = webElement as FirefoxWebElement;
			return firefoxWebElement != null && base.Id == firefoxWebElement.Id;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
