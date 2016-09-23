using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace OpenQA.Selenium.Support.PageObjects
{
	internal class WebElementListProxy : WebDriverObjectProxy
	{
		private List<IWebElement> collection;

		private List<IWebElement> ElementList
		{
			get
			{
				if (!base.Cache || this.collection == null)
				{
					this.collection = new List<IWebElement>();
					this.collection.AddRange(base.Locator.LocateElements(base.Bys));
				}
				return this.collection;
			}
		}

		private WebElementListProxy(Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool cache) : base(typeToBeProxied, locator, bys, cache)
		{
		}

		public static object CreateProxy(Type classToProxy, IElementLocator locator, IEnumerable<By> bys, bool cacheLookups)
		{
			return new WebElementListProxy(classToProxy, locator, bys, cacheLookups).GetTransparentProxy();
		}

		public override IMessage Invoke(IMessage msg)
		{
			List<IWebElement> elementList = this.ElementList;
			return WebDriverObjectProxy.InvokeMethod(msg as IMethodCallMessage, elementList);
		}
	}
}
