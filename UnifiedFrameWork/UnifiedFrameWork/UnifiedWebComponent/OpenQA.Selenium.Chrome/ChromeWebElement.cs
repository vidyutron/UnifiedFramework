using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.Chrome
{
	public class ChromeWebElement : RemoteWebElement
	{
		public ChromeWebElement(ChromeDriver parent, string elementId) : base(parent, elementId)
		{
		}
	}
}
