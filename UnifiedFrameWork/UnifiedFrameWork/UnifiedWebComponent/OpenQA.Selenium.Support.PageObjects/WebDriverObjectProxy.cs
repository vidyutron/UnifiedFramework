using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace OpenQA.Selenium.Support.PageObjects
{
	internal abstract class WebDriverObjectProxy : RealProxy
	{
		private readonly IElementLocator locator;

		private readonly IEnumerable<By> bys;

		private readonly bool cache;

		protected IElementLocator Locator
		{
			get
			{
				return this.locator;
			}
		}

		protected IEnumerable<By> Bys
		{
			get
			{
				return this.bys;
			}
		}

		protected bool Cache
		{
			get
			{
				return this.cache;
			}
		}

		protected WebDriverObjectProxy(Type classToProxy, IElementLocator locator, IEnumerable<By> bys, bool cache) : base(classToProxy)
		{
			this.locator = locator;
			this.bys = bys;
			this.cache = cache;
		}

		protected static ReturnMessage InvokeMethod(IMethodCallMessage msg, object representedValue)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg", "The message containing invocation information cannot be null");
			}
			MethodInfo methodInfo = msg.MethodBase as MethodInfo;
			return new ReturnMessage(methodInfo.Invoke(representedValue, msg.Args), null, 0, msg.LogicalCallContext, msg);
		}
	}
}
