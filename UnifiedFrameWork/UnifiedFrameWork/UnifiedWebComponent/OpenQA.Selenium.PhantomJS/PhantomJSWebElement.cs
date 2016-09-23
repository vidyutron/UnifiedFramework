using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.PhantomJS
{
	public class PhantomJSWebElement : RemoteWebElement
	{
		public PhantomJSWebElement(PhantomJSDriver parent, string id) : base(parent, id)
		{
		}
	}
}
