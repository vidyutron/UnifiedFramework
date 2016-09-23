using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace OpenQA.Selenium.Support.PageObjects
{
	internal sealed class WebElementProxy : WebDriverObjectProxy, IWrapsElement
	{
		private IWebElement cachedElement;

		public IWebElement WrappedElement
		{
			get
			{
				return this.Element;
			}
		}

		private IWebElement Element
		{
			get
			{
				if (!base.Cache || this.cachedElement == null)
				{
					this.cachedElement = base.Locator.LocateElement(base.Bys);
				}
				return this.cachedElement;
			}
		}

		private WebElementProxy(Type classToProxy, IElementLocator locator, IEnumerable<By> bys, bool cache) : base(classToProxy, locator, bys, cache)
		{
		}

		public static object CreateProxy(Type classToProxy, IElementLocator locator, IEnumerable<By> bys, bool cacheLookups)
		{
			return new WebElementProxy(classToProxy, locator, bys, cacheLookups).GetTransparentProxy();
		}

		public override IMessage Invoke(IMessage msg)
		{
			IWebElement element = this.Element;
			IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;
			if (typeof(IWrapsElement).IsAssignableFrom((methodCallMessage.MethodBase as MethodInfo).DeclaringType))
			{
				return new ReturnMessage(element, null, 0, methodCallMessage.LogicalCallContext, methodCallMessage);
			}
			return WebDriverObjectProxy.InvokeMethod(methodCallMessage, element);
		}
	}
}
