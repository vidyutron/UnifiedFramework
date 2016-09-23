using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.Edge
{
	public class EdgeWebElement : RemoteWebElement
	{
		public EdgeWebElement(EdgeDriver parent, string elementId) : base(parent, elementId)
		{
		}
	}
}
