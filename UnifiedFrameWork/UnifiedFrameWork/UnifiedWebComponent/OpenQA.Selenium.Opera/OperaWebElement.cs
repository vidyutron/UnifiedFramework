using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.Opera
{
	public class OperaWebElement : RemoteWebElement
	{
		public OperaWebElement(OperaDriver parent, string elementId) : base(parent, elementId)
		{
		}
	}
}
