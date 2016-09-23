using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace OpenQA.Selenium.Support.Events
{
	public class EventFiringWebDriver : IWebDriver, ISearchContext, IDisposable, IJavaScriptExecutor, ITakesScreenshot, IWrapsDriver
	{
		private class EventFiringNavigation : INavigation
		{
			private EventFiringWebDriver parentDriver;

			private INavigation wrappedNavigation;

			public EventFiringNavigation(EventFiringWebDriver driver)
			{
				this.parentDriver = driver;
				this.wrappedNavigation = this.parentDriver.WrappedDriver.Navigate();
			}

			public void Back()
			{
				try
				{
					WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.parentDriver);
					this.parentDriver.OnNavigatingBack(e);
					this.wrappedNavigation.Back();
					this.parentDriver.OnNavigatedBack(e);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
			}

			public void Forward()
			{
				try
				{
					WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.parentDriver);
					this.parentDriver.OnNavigatingForward(e);
					this.wrappedNavigation.Forward();
					this.parentDriver.OnNavigatedForward(e);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
			}

			public void GoToUrl(string url)
			{
				try
				{
					WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.parentDriver, url);
					this.parentDriver.OnNavigating(e);
					this.wrappedNavigation.GoToUrl(url);
					this.parentDriver.OnNavigated(e);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
			}

			public void GoToUrl(Uri url)
			{
				if (url == null)
				{
					throw new ArgumentNullException("url", "url cannot be null");
				}
				try
				{
					WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.parentDriver, url.ToString());
					this.parentDriver.OnNavigating(e);
					this.wrappedNavigation.GoToUrl(url);
					this.parentDriver.OnNavigated(e);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
			}

			public void Refresh()
			{
				try
				{
					this.wrappedNavigation.Refresh();
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
			}
		}

		private class EventFiringOptions : IOptions
		{
			private IOptions wrappedOptions;

			public ICookieJar Cookies
			{
				get
				{
					return this.wrappedOptions.Cookies;
				}
			}

			public IWindow Window
			{
				get
				{
					return this.wrappedOptions.Window;
				}
			}

			public EventFiringOptions(EventFiringWebDriver driver)
			{
				this.wrappedOptions = driver.WrappedDriver.Manage();
			}

			public ITimeouts Timeouts()
			{
				return new EventFiringWebDriver.EventFiringTimeouts(this.wrappedOptions);
			}
		}

		private class EventFiringTargetLocator : ITargetLocator
		{
			private ITargetLocator wrappedLocator;

			private EventFiringWebDriver parentDriver;

			public EventFiringTargetLocator(EventFiringWebDriver driver)
			{
				this.parentDriver = driver;
				this.wrappedLocator = this.parentDriver.WrappedDriver.SwitchTo();
			}

			public IWebDriver Frame(int frameIndex)
			{
				IWebDriver result = null;
				try
				{
					result = this.wrappedLocator.Frame(frameIndex);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public IWebDriver Frame(string frameName)
			{
				IWebDriver result = null;
				try
				{
					result = this.wrappedLocator.Frame(frameName);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public IWebDriver Frame(IWebElement frameElement)
			{
				IWebDriver result = null;
				try
				{
					IWrapsElement wrapsElement = frameElement as IWrapsElement;
					result = this.wrappedLocator.Frame(wrapsElement.WrappedElement);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public IWebDriver ParentFrame()
			{
				IWebDriver result = null;
				try
				{
					result = this.wrappedLocator.ParentFrame();
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public IWebDriver Window(string windowName)
			{
				IWebDriver result = null;
				try
				{
					result = this.wrappedLocator.Window(windowName);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public IWebDriver DefaultContent()
			{
				IWebDriver result = null;
				try
				{
					result = this.wrappedLocator.DefaultContent();
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public IWebElement ActiveElement()
			{
				IWebElement result = null;
				try
				{
					result = this.wrappedLocator.ActiveElement();
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public IAlert Alert()
			{
				IAlert result = null;
				try
				{
					result = this.wrappedLocator.Alert();
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}
		}

		private class EventFiringTimeouts : ITimeouts
		{
			private ITimeouts wrappedTimeouts;

			public EventFiringTimeouts(IOptions options)
			{
				this.wrappedTimeouts = options.Timeouts();
			}

			public ITimeouts ImplicitlyWait(TimeSpan timeToWait)
			{
				return this.wrappedTimeouts.ImplicitlyWait(timeToWait);
			}

			public ITimeouts SetScriptTimeout(TimeSpan timeToWait)
			{
				return this.wrappedTimeouts.SetScriptTimeout(timeToWait);
			}

			public ITimeouts SetPageLoadTimeout(TimeSpan timeToWait)
			{
				this.wrappedTimeouts.SetPageLoadTimeout(timeToWait);
				return this;
			}
		}

		private class EventFiringWebElement : IWebElement, ISearchContext, IWrapsElement
		{
			private IWebElement underlyingElement;

			private EventFiringWebDriver parentDriver;

			public IWebElement WrappedElement
			{
				get
				{
					return this.underlyingElement;
				}
			}

			public string TagName
			{
				get
				{
					string result = string.Empty;
					try
					{
						result = this.underlyingElement.TagName;
					}
					catch (Exception thrownException)
					{
						this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
						throw;
					}
					return result;
				}
			}

			public string Text
			{
				get
				{
					string result = string.Empty;
					try
					{
						result = this.underlyingElement.Text;
					}
					catch (Exception thrownException)
					{
						this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
						throw;
					}
					return result;
				}
			}

			public bool Enabled
			{
				get
				{
					bool result = false;
					try
					{
						result = this.underlyingElement.Enabled;
					}
					catch (Exception thrownException)
					{
						this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
						throw;
					}
					return result;
				}
			}

			public bool Selected
			{
				get
				{
					bool result = false;
					try
					{
						result = this.underlyingElement.Selected;
					}
					catch (Exception thrownException)
					{
						this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
						throw;
					}
					return result;
				}
			}

			public Point Location
			{
				get
				{
					Point result = default(Point);
					try
					{
						result = this.underlyingElement.Location;
					}
					catch (Exception thrownException)
					{
						this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
						throw;
					}
					return result;
				}
			}

			public Size Size
			{
				get
				{
					Size result = default(Size);
					try
					{
						result = this.underlyingElement.Size;
					}
					catch (Exception thrownException)
					{
						this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
						throw;
					}
					return result;
				}
			}

			public bool Displayed
			{
				get
				{
					bool result = false;
					try
					{
						result = this.underlyingElement.Displayed;
					}
					catch (Exception thrownException)
					{
						this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
						throw;
					}
					return result;
				}
			}

			protected EventFiringWebDriver ParentDriver
			{
				get
				{
					return this.parentDriver;
				}
			}

			public EventFiringWebElement(EventFiringWebDriver driver, IWebElement element)
			{
				this.underlyingElement = element;
				this.parentDriver = driver;
			}

			public void Clear()
			{
				try
				{
					WebElementEventArgs e = new WebElementEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement);
					this.parentDriver.OnElementValueChanging(e);
					this.underlyingElement.Clear();
					this.parentDriver.OnElementValueChanged(e);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
			}

			public void SendKeys(string text)
			{
				try
				{
					WebElementEventArgs e = new WebElementEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement);
					this.parentDriver.OnElementValueChanging(e);
					this.underlyingElement.SendKeys(text);
					this.parentDriver.OnElementValueChanged(e);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
			}

			public void Submit()
			{
				try
				{
					this.underlyingElement.Submit();
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
			}

			public void Click()
			{
				try
				{
					WebElementEventArgs e = new WebElementEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement);
					this.parentDriver.OnElementClicking(e);
					this.underlyingElement.Click();
					this.parentDriver.OnElementClicked(e);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
			}

			public string GetAttribute(string attributeName)
			{
				string result = string.Empty;
				try
				{
					result = this.underlyingElement.GetAttribute(attributeName);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public string GetCssValue(string propertyName)
			{
				string result = string.Empty;
				try
				{
					result = this.underlyingElement.GetCssValue(propertyName);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public IWebElement FindElement(By by)
			{
				IWebElement result = null;
				try
				{
					FindElementEventArgs e = new FindElementEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement, by);
					this.parentDriver.OnFindingElement(e);
					IWebElement webElement = this.underlyingElement.FindElement(by);
					this.parentDriver.OnFindElementCompleted(e);
					result = this.parentDriver.WrapElement(webElement);
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return result;
			}

			public ReadOnlyCollection<IWebElement> FindElements(By by)
			{
				List<IWebElement> list = new List<IWebElement>();
				try
				{
					FindElementEventArgs e = new FindElementEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement, by);
					this.parentDriver.OnFindingElement(e);
					ReadOnlyCollection<IWebElement> readOnlyCollection = this.underlyingElement.FindElements(by);
					this.parentDriver.OnFindElementCompleted(e);
					foreach (IWebElement current in readOnlyCollection)
					{
						IWebElement item = this.parentDriver.WrapElement(current);
						list.Add(item);
					}
				}
				catch (Exception thrownException)
				{
					this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, thrownException));
					throw;
				}
				return list.AsReadOnly();
			}
		}

		private IWebDriver driver;

		public event EventHandler<WebDriverNavigationEventArgs> Navigating;

		public event EventHandler<WebDriverNavigationEventArgs> Navigated;

		public event EventHandler<WebDriverNavigationEventArgs> NavigatingBack;

		public event EventHandler<WebDriverNavigationEventArgs> NavigatedBack;

		public event EventHandler<WebDriverNavigationEventArgs> NavigatingForward;

		public event EventHandler<WebDriverNavigationEventArgs> NavigatedForward;

		public event EventHandler<WebElementEventArgs> ElementClicking;

		public event EventHandler<WebElementEventArgs> ElementClicked;

		public event EventHandler<WebElementEventArgs> ElementValueChanging;

		public event EventHandler<WebElementEventArgs> ElementValueChanged;

		public event EventHandler<FindElementEventArgs> FindingElement;

		public event EventHandler<FindElementEventArgs> FindElementCompleted;

		public event EventHandler<WebDriverScriptEventArgs> ScriptExecuting;

		public event EventHandler<WebDriverScriptEventArgs> ScriptExecuted;

		public event EventHandler<WebDriverExceptionEventArgs> ExceptionThrown;

		public IWebDriver WrappedDriver
		{
			get
			{
				return this.driver;
			}
		}

		public string Url
		{
			get
			{
				string result = string.Empty;
				try
				{
					result = this.driver.Url;
				}
				catch (Exception thrownException)
				{
					this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
					throw;
				}
				return result;
			}
			set
			{
				try
				{
					WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.driver, value);
					this.OnNavigating(e);
					this.driver.Url = value;
					this.OnNavigated(e);
				}
				catch (Exception thrownException)
				{
					this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
					throw;
				}
			}
		}

		public string Title
		{
			get
			{
				string result = string.Empty;
				try
				{
					result = this.driver.Title;
				}
				catch (Exception thrownException)
				{
					this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
					throw;
				}
				return result;
			}
		}

		public string PageSource
		{
			get
			{
				string result = string.Empty;
				try
				{
					result = this.driver.PageSource;
				}
				catch (Exception thrownException)
				{
					this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
					throw;
				}
				return result;
			}
		}

		public string CurrentWindowHandle
		{
			get
			{
				string result = string.Empty;
				try
				{
					result = this.driver.CurrentWindowHandle;
				}
				catch (Exception thrownException)
				{
					this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
					throw;
				}
				return result;
			}
		}

		public ReadOnlyCollection<string> WindowHandles
		{
			get
			{
				ReadOnlyCollection<string> result = null;
				try
				{
					result = this.driver.WindowHandles;
				}
				catch (Exception thrownException)
				{
					this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
					throw;
				}
				return result;
			}
		}

		public EventFiringWebDriver(IWebDriver parentDriver)
		{
			this.driver = parentDriver;
		}

		public void Close()
		{
			try
			{
				this.driver.Close();
			}
			catch (Exception thrownException)
			{
				this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
				throw;
			}
		}

		public void Quit()
		{
			try
			{
				this.driver.Quit();
			}
			catch (Exception thrownException)
			{
				this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
				throw;
			}
		}

		public IOptions Manage()
		{
			return new EventFiringWebDriver.EventFiringOptions(this);
		}

		public INavigation Navigate()
		{
			return new EventFiringWebDriver.EventFiringNavigation(this);
		}

		public ITargetLocator SwitchTo()
		{
			return new EventFiringWebDriver.EventFiringTargetLocator(this);
		}

		public IWebElement FindElement(By by)
		{
			IWebElement result = null;
			try
			{
				FindElementEventArgs e = new FindElementEventArgs(this.driver, by);
				this.OnFindingElement(e);
				IWebElement underlyingElement = this.driver.FindElement(by);
				this.OnFindElementCompleted(e);
				result = this.WrapElement(underlyingElement);
			}
			catch (Exception thrownException)
			{
				this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
				throw;
			}
			return result;
		}

		public ReadOnlyCollection<IWebElement> FindElements(By by)
		{
			List<IWebElement> list = new List<IWebElement>();
			try
			{
				FindElementEventArgs e = new FindElementEventArgs(this.driver, by);
				this.OnFindingElement(e);
				ReadOnlyCollection<IWebElement> readOnlyCollection = this.driver.FindElements(by);
				this.OnFindElementCompleted(e);
				foreach (IWebElement current in readOnlyCollection)
				{
					IWebElement item = this.WrapElement(current);
					list.Add(item);
				}
			}
			catch (Exception thrownException)
			{
				this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
				throw;
			}
			return list.AsReadOnly();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public object ExecuteScript(string script, params object[] args)
		{
			IJavaScriptExecutor javaScriptExecutor = this.driver as IJavaScriptExecutor;
			if (javaScriptExecutor == null)
			{
				throw new NotSupportedException("Underlying driver instance does not support executing JavaScript");
			}
			object result = null;
			try
			{
				object[] args2 = EventFiringWebDriver.UnwrapElementArguments(args);
				WebDriverScriptEventArgs e = new WebDriverScriptEventArgs(this.driver, script);
				this.OnScriptExecuting(e);
				result = javaScriptExecutor.ExecuteScript(script, args2);
				this.OnScriptExecuted(e);
			}
			catch (Exception thrownException)
			{
				this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
				throw;
			}
			return result;
		}

		public object ExecuteAsyncScript(string script, params object[] args)
		{
			object result = null;
			IJavaScriptExecutor javaScriptExecutor = this.driver as IJavaScriptExecutor;
			if (javaScriptExecutor == null)
			{
				throw new NotSupportedException("Underlying driver instance does not support executing JavaScript");
			}
			try
			{
				object[] args2 = EventFiringWebDriver.UnwrapElementArguments(args);
				WebDriverScriptEventArgs e = new WebDriverScriptEventArgs(this.driver, script);
				this.OnScriptExecuting(e);
				result = javaScriptExecutor.ExecuteAsyncScript(script, args2);
				this.OnScriptExecuted(e);
			}
			catch (Exception thrownException)
			{
				this.OnException(new WebDriverExceptionEventArgs(this.driver, thrownException));
				throw;
			}
			return result;
		}

		public Screenshot GetScreenshot()
		{
			ITakesScreenshot takesScreenshot = this.driver as ITakesScreenshot;
			if (this.driver == null)
			{
				throw new NotSupportedException("Underlying driver instance does not support taking screenshots");
			}
			return takesScreenshot.GetScreenshot();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.driver.Dispose();
			}
		}

		protected virtual void OnNavigating(WebDriverNavigationEventArgs e)
		{
			if (this.Navigating != null)
			{
				this.Navigating(this, e);
			}
		}

		protected virtual void OnNavigated(WebDriverNavigationEventArgs e)
		{
			if (this.Navigated != null)
			{
				this.Navigated(this, e);
			}
		}

		protected virtual void OnNavigatingBack(WebDriverNavigationEventArgs e)
		{
			if (this.NavigatingBack != null)
			{
				this.NavigatingBack(this, e);
			}
		}

		protected virtual void OnNavigatedBack(WebDriverNavigationEventArgs e)
		{
			if (this.NavigatedBack != null)
			{
				this.NavigatedBack(this, e);
			}
		}

		protected virtual void OnNavigatingForward(WebDriverNavigationEventArgs e)
		{
			if (this.NavigatingForward != null)
			{
				this.NavigatingForward(this, e);
			}
		}

		protected virtual void OnNavigatedForward(WebDriverNavigationEventArgs e)
		{
			if (this.NavigatedForward != null)
			{
				this.NavigatedForward(this, e);
			}
		}

		protected virtual void OnElementClicking(WebElementEventArgs e)
		{
			if (this.ElementClicking != null)
			{
				this.ElementClicking(this, e);
			}
		}

		protected virtual void OnElementClicked(WebElementEventArgs e)
		{
			if (this.ElementClicked != null)
			{
				this.ElementClicked(this, e);
			}
		}

		protected virtual void OnElementValueChanging(WebElementEventArgs e)
		{
			if (this.ElementValueChanging != null)
			{
				this.ElementValueChanging(this, e);
			}
		}

		protected virtual void OnElementValueChanged(WebElementEventArgs e)
		{
			if (this.ElementValueChanged != null)
			{
				this.ElementValueChanged(this, e);
			}
		}

		protected virtual void OnFindingElement(FindElementEventArgs e)
		{
			if (this.FindingElement != null)
			{
				this.FindingElement(this, e);
			}
		}

		protected virtual void OnFindElementCompleted(FindElementEventArgs e)
		{
			if (this.FindElementCompleted != null)
			{
				this.FindElementCompleted(this, e);
			}
		}

		protected virtual void OnScriptExecuting(WebDriverScriptEventArgs e)
		{
			if (this.ScriptExecuting != null)
			{
				this.ScriptExecuting(this, e);
			}
		}

		protected virtual void OnScriptExecuted(WebDriverScriptEventArgs e)
		{
			if (this.ScriptExecuted != null)
			{
				this.ScriptExecuted(this, e);
			}
		}

		protected virtual void OnException(WebDriverExceptionEventArgs e)
		{
			if (this.ExceptionThrown != null)
			{
				this.ExceptionThrown(this, e);
			}
		}

		private static object[] UnwrapElementArguments(object[] args)
		{
			List<object> list = new List<object>();
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				EventFiringWebDriver.EventFiringWebElement eventFiringWebElement = obj as EventFiringWebDriver.EventFiringWebElement;
				if (eventFiringWebElement != null)
				{
					list.Add(eventFiringWebElement.WrappedElement);
				}
				else
				{
					list.Add(obj);
				}
			}
			return list.ToArray();
		}

		private IWebElement WrapElement(IWebElement underlyingElement)
		{
			return new EventFiringWebDriver.EventFiringWebElement(this, underlyingElement);
		}
	}
}
