using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace OpenQA.Selenium.Remote
{
	public class RemoteWebDriver : IWebDriver, IDisposable, ISearchContext, IJavaScriptExecutor, IFindsById, IFindsByClassName, IFindsByLinkText, IFindsByName, IFindsByTagName, IFindsByXPath, IFindsByPartialLinkText, IFindsByCssSelector, ITakesScreenshot, IHasInputDevices, IHasCapabilities, IHasWebStorage, IHasLocationContext, IHasApplicationCache, IAllowsFileDetection, IHasSessionId
	{
		protected static readonly TimeSpan DefaultCommandTimeout = TimeSpan.FromSeconds(60.0);

		private ICommandExecutor executor;

		private ICapabilities capabilities;

		private IMouse mouse;

		private IKeyboard keyboard;

		private SessionId sessionId;

		private IWebStorage storage;

		private IApplicationCache appCache;

		private ILocationContext locationContext;

		private IFileDetector fileDetector = new DefaultFileDetector();

		public string Url
		{
			get
			{
				Response response = this.Execute(DriverCommand.GetCurrentUrl, null);
				return response.Value.ToString();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Argument 'url' cannot be null.");
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("url", value);
				try
				{
					this.Execute(DriverCommand.Get, dictionary);
				}
				catch (WebDriverTimeoutException)
				{
					throw;
				}
				catch (WebDriverException)
				{
				}
				catch (InvalidOperationException)
				{
				}
				catch (NotImplementedException)
				{
				}
			}
		}

		public string Title
		{
			get
			{
				Response response = this.Execute(DriverCommand.GetTitle, null);
				object obj = (response != null) ? response.Value : string.Empty;
				return obj.ToString();
			}
		}

		public string PageSource
		{
			get
			{
				string empty = string.Empty;
				Response response = this.Execute(DriverCommand.GetPageSource, null);
				return response.Value.ToString();
			}
		}

		public string CurrentWindowHandle
		{
			get
			{
				Response response = this.Execute(DriverCommand.GetCurrentWindowHandle, null);
				return response.Value.ToString();
			}
		}

		public ReadOnlyCollection<string> WindowHandles
		{
			get
			{
				Response response = this.Execute(DriverCommand.GetWindowHandles, null);
				object[] array = (object[])response.Value;
				List<string> list = new List<string>();
				object[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					object obj = array2[i];
					list.Add(obj.ToString());
				}
				return list.AsReadOnly();
			}
		}

		public IKeyboard Keyboard
		{
			get
			{
				return this.keyboard;
			}
		}

		public IMouse Mouse
		{
			get
			{
				return this.mouse;
			}
		}

		public bool HasWebStorage
		{
			get
			{
				return this.storage != null;
			}
		}

		public IWebStorage WebStorage
		{
			get
			{
				if (this.storage == null)
				{
					throw new InvalidOperationException("Driver does not support manipulating HTML5 web storage. Use the HasWebStorage property to test for the driver capability");
				}
				return this.storage;
			}
		}

		public bool HasApplicationCache
		{
			get
			{
				return this.appCache != null;
			}
		}

		public IApplicationCache ApplicationCache
		{
			get
			{
				if (this.appCache == null)
				{
					throw new InvalidOperationException("Driver does not support manipulating the HTML5 application cache. Use the HasApplicationCache property to test for the driver capability");
				}
				return this.appCache;
			}
		}

		public bool HasLocationContext
		{
			get
			{
				return this.locationContext != null;
			}
		}

		public ILocationContext LocationContext
		{
			get
			{
				if (this.locationContext == null)
				{
					throw new InvalidOperationException("Driver does not support setting HTML5 geolocation information. Use the HasLocationContext property to test for the driver capability");
				}
				return this.locationContext;
			}
		}

		public ICapabilities Capabilities
		{
			get
			{
				return this.capabilities;
			}
		}

		public virtual IFileDetector FileDetector
		{
			get
			{
				return this.fileDetector;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "FileDetector cannot be null");
				}
				this.fileDetector = value;
			}
		}

		public SessionId SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		internal bool IsSpecificationCompliant
		{
			get
			{
				return this.CommandExecutor.CommandInfoRepository.SpecificationLevel > 0;
			}
		}

		protected ICommandExecutor CommandExecutor
		{
			get
			{
				return this.executor;
			}
		}

		public RemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
		{
			this.executor = commandExecutor;
			this.StartClient();
			this.StartSession(desiredCapabilities);
			this.mouse = new RemoteMouse(this);
			this.keyboard = new RemoteKeyboard(this);
			if (this.capabilities.HasCapability(CapabilityType.SupportsApplicationCache))
			{
				object capability = this.capabilities.GetCapability(CapabilityType.SupportsApplicationCache);
				if (capability is bool && (bool)capability)
				{
					this.appCache = new RemoteApplicationCache(this);
				}
			}
			if (this.capabilities.HasCapability(CapabilityType.SupportsLocationContext))
			{
				object capability2 = this.capabilities.GetCapability(CapabilityType.SupportsLocationContext);
				if (capability2 is bool && (bool)capability2)
				{
					this.locationContext = new RemoteLocationContext(this);
				}
			}
			if (this.capabilities.HasCapability(CapabilityType.SupportsWebStorage))
			{
				object capability3 = this.capabilities.GetCapability(CapabilityType.SupportsWebStorage);
				if (capability3 is bool && (bool)capability3)
				{
					this.storage = new RemoteWebStorage(this);
				}
			}
		}

		public RemoteWebDriver(ICapabilities desiredCapabilities) : this(new Uri("http://127.0.0.1:4444/wd/hub"), desiredCapabilities)
		{
		}

		public RemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities) : this(remoteAddress, desiredCapabilities, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public RemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities, TimeSpan commandTimeout) : this(new HttpCommandExecutor(remoteAddress, commandTimeout), desiredCapabilities)
		{
		}

		public IWebElement FindElement(By by)
		{
			if (by == null)
			{
				throw new ArgumentNullException("by", "by cannot be null");
			}
			return by.FindElement(this);
		}

		public ReadOnlyCollection<IWebElement> FindElements(By by)
		{
			if (by == null)
			{
				throw new ArgumentNullException("by", "by cannot be null");
			}
			return by.FindElements(this);
		}

		public void Close()
		{
			this.Execute(DriverCommand.Close, null);
		}

		public void Quit()
		{
			this.Dispose();
		}

		public IOptions Manage()
		{
			return new RemoteOptions(this);
		}

		public INavigation Navigate()
		{
			return new RemoteNavigator(this);
		}

		public ITargetLocator SwitchTo()
		{
			return new RemoteTargetLocator(this);
		}

		public object ExecuteScript(string script, params object[] args)
		{
			return this.ExecuteScriptCommand(script, DriverCommand.ExecuteScript, args);
		}

		public object ExecuteAsyncScript(string script, params object[] args)
		{
			return this.ExecuteScriptCommand(script, DriverCommand.ExecuteAsyncScript, args);
		}

		public IWebElement FindElementById(string id)
		{
			if (this.IsSpecificationCompliant)
			{
				return this.FindElement("css selector", "#" + RemoteWebDriver.EscapeCssSelector(id));
			}
			return this.FindElement("id", id);
		}

		public ReadOnlyCollection<IWebElement> FindElementsById(string id)
		{
			if (!this.IsSpecificationCompliant)
			{
				return this.FindElements("id", id);
			}
			string text = RemoteWebDriver.EscapeCssSelector(id);
			if (string.IsNullOrEmpty(text))
			{
				return new List<IWebElement>().AsReadOnly();
			}
			return this.FindElements("css selector", "#" + text);
		}

		public IWebElement FindElementByClassName(string className)
		{
			if (!this.IsSpecificationCompliant)
			{
				return this.FindElement("class name", className);
			}
			string text = RemoteWebDriver.EscapeCssSelector(className);
			if (text.Contains(" "))
			{
				throw new InvalidSelectorException("Compound class names not allowed. Cannot have whitespace in class name. Use CSS selectors instead.");
			}
			return this.FindElement("css selector", "." + text);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
		{
			if (!this.IsSpecificationCompliant)
			{
				return this.FindElements("class name", className);
			}
			string text = RemoteWebDriver.EscapeCssSelector(className);
			if (text.Contains(" "))
			{
				throw new InvalidSelectorException("Compound class names not allowed. Cannot have whitespace in class name. Use CSS selectors instead.");
			}
			return this.FindElements("css selector", "." + text);
		}

		public IWebElement FindElementByLinkText(string linkText)
		{
			return this.FindElement("link text", linkText);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText)
		{
			return this.FindElements("link text", linkText);
		}

		public IWebElement FindElementByPartialLinkText(string partialLinkText)
		{
			return this.FindElement("partial link text", partialLinkText);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
		{
			return this.FindElements("partial link text", partialLinkText);
		}

		public IWebElement FindElementByName(string name)
		{
			if (this.IsSpecificationCompliant)
			{
				return this.FindElement("css selector", "*[name=\"" + name + "\"]");
			}
			return this.FindElement("name", name);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
		{
			if (this.IsSpecificationCompliant)
			{
				return this.FindElements("css selector", "*[name=\"" + name + "\"]");
			}
			return this.FindElements("name", name);
		}

		public IWebElement FindElementByTagName(string tagName)
		{
			if (this.IsSpecificationCompliant)
			{
				return this.FindElement("css selector", tagName);
			}
			return this.FindElement("tag name", tagName);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
		{
			if (this.IsSpecificationCompliant)
			{
				return this.FindElements("css selector", tagName);
			}
			return this.FindElements("tag name", tagName);
		}

		public IWebElement FindElementByXPath(string xpath)
		{
			return this.FindElement("xpath", xpath);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
		{
			return this.FindElements("xpath", xpath);
		}

		public IWebElement FindElementByCssSelector(string cssSelector)
		{
			return this.FindElement("css selector", cssSelector);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
		{
			return this.FindElements("css selector", cssSelector);
		}

		public Screenshot GetScreenshot()
		{
			Response response = this.Execute(DriverCommand.Screenshot, null);
			string base64EncodedScreenshot = response.Value.ToString();
			return new Screenshot(base64EncodedScreenshot);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal static string EscapeCssSelector(string selector)
		{
			string result = Regex.Replace(selector, "(['\"\\\\#.:;,!?+<>=~*^$|%&@`{}\\-/\\[\\]\\(\\)])", "\\$1");
			if (selector.Length > 0 && char.IsDigit(selector[0]))
			{
				result = "\\" + (30 + int.Parse(selector.Substring(0, 1), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture) + " " + selector.Substring(1);
			}
			return result;
		}

		internal Response InternalExecute(string driverCommandToExecute, Dictionary<string, object> parameters)
		{
			return this.Execute(driverCommandToExecute, parameters);
		}

		internal IWebElement GetElementFromResponse(Response response)
		{
			if (response == null)
			{
				throw new NoSuchElementException();
			}
			RemoteWebElement result = null;
			Dictionary<string, object> dictionary = response.Value as Dictionary<string, object>;
			if (dictionary != null)
			{
				string elementId = string.Empty;
				if (dictionary.ContainsKey("element-6066-11e4-a52e-4f735466cecf"))
				{
					elementId = (string)dictionary["element-6066-11e4-a52e-4f735466cecf"];
				}
				else if (dictionary.ContainsKey("ELEMENT"))
				{
					elementId = (string)dictionary["ELEMENT"];
				}
				result = this.CreateElement(elementId);
			}
			return result;
		}

		internal ReadOnlyCollection<IWebElement> GetElementsFromResponse(Response response)
		{
			List<IWebElement> list = new List<IWebElement>();
			object[] array = response.Value as object[];
			object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				object obj = array2[i];
				Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
				if (dictionary != null)
				{
					string elementId = string.Empty;
					if (dictionary.ContainsKey("element-6066-11e4-a52e-4f735466cecf"))
					{
						elementId = (string)dictionary["element-6066-11e4-a52e-4f735466cecf"];
					}
					else if (dictionary.ContainsKey("ELEMENT"))
					{
						elementId = (string)dictionary["ELEMENT"];
					}
					RemoteWebElement item = this.CreateElement(elementId);
					list.Add(item);
				}
			}
			return list.AsReadOnly();
		}

		protected virtual void Dispose(bool disposing)
		{
			try
			{
				this.Execute(DriverCommand.Quit, null);
			}
			catch (NotImplementedException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			catch (WebDriverException)
			{
			}
			finally
			{
				this.StopClient();
				this.sessionId = null;
			}
		}

		protected void StartSession(ICapabilities desiredCapabilities)
		{
			DesiredCapabilities desiredCapabilities2 = desiredCapabilities as DesiredCapabilities;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("desiredCapabilities", desiredCapabilities2.CapabilitiesDictionary);
			Response response = this.Execute(DriverCommand.NewSession, dictionary);
			Dictionary<string, object> rawMap = (Dictionary<string, object>)response.Value;
			DesiredCapabilities desiredCapabilities3 = new DesiredCapabilities(rawMap);
			this.capabilities = desiredCapabilities3;
			this.sessionId = new SessionId(response.SessionId);
		}

		protected virtual Response Execute(string driverCommandToExecute, Dictionary<string, object> parameters)
		{
			Command commandToExecute = new Command(this.sessionId, driverCommandToExecute, parameters);
			Response response = new Response();
			try
			{
				response = this.executor.Execute(commandToExecute);
			}
			catch (WebException value)
			{
				response.Status = WebDriverResult.UnhandledError;
				response.Value = value;
			}
			if (response.Status != WebDriverResult.Success)
			{
				RemoteWebDriver.UnpackAndThrowOnError(response);
			}
			return response;
		}

		protected virtual void StartClient()
		{
		}

		protected virtual void StopClient()
		{
		}

		protected IWebElement FindElement(string mechanism, string value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("using", mechanism);
			dictionary.Add("value", value);
			Response response = this.Execute(DriverCommand.FindElement, dictionary);
			return this.GetElementFromResponse(response);
		}

		protected ReadOnlyCollection<IWebElement> FindElements(string mechanism, string value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("using", mechanism);
			dictionary.Add("value", value);
			Response response = this.Execute(DriverCommand.FindElements, dictionary);
			return this.GetElementsFromResponse(response);
		}

		protected virtual RemoteWebElement CreateElement(string elementId)
		{
			return new RemoteWebElement(this, elementId);
		}

		protected object ExecuteScriptCommand(string script, string commandName, params object[] args)
		{
			object[] array = RemoteWebDriver.ConvertArgumentsToJavaScriptObjects(args);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("script", script);
			if (array != null && array.Length > 0)
			{
				dictionary.Add("args", array);
			}
			else
			{
				dictionary.Add("args", new object[0]);
			}
			Response response = this.Execute(commandName, dictionary);
			return this.ParseJavaScriptReturnValue(response.Value);
		}

		private static object ConvertObjectToJavaScriptObject(object arg)
		{
			IWrapsElement wrapsElement = arg as IWrapsElement;
			RemoteWebElement remoteWebElement = arg as RemoteWebElement;
			IEnumerable enumerable = arg as IEnumerable;
			IDictionary dictionary = arg as IDictionary;
			if (remoteWebElement == null && wrapsElement != null)
			{
				remoteWebElement = (wrapsElement.WrappedElement as RemoteWebElement);
			}
			object result;
			if (arg is string || arg is float || arg is double || arg is int || arg is long || arg is bool || arg == null)
			{
				result = arg;
			}
			else if (remoteWebElement != null)
			{
				result = new Dictionary<string, object>
				{
					{
						"ELEMENT",
						remoteWebElement.InternalElementId
					},
					{
						"element-6066-11e4-a52e-4f735466cecf",
						remoteWebElement.InternalElementId
					}
				};
			}
			else if (dictionary != null)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				foreach (object current in dictionary.Keys)
				{
					dictionary2.Add(current.ToString(), RemoteWebDriver.ConvertObjectToJavaScriptObject(dictionary[current]));
				}
				result = dictionary2;
			}
			else
			{
				if (enumerable == null)
				{
					throw new ArgumentException("Argument is of an illegal type" + arg.ToString(), "arg");
				}
				List<object> list = new List<object>();
				foreach (object current2 in enumerable)
				{
					list.Add(RemoteWebDriver.ConvertObjectToJavaScriptObject(current2));
				}
				result = list.ToArray();
			}
			return result;
		}

		private static object[] ConvertArgumentsToJavaScriptObjects(object[] args)
		{
			if (args == null)
			{
				return new object[1];
			}
			for (int i = 0; i < args.Length; i++)
			{
				args[i] = RemoteWebDriver.ConvertObjectToJavaScriptObject(args[i]);
			}
			return args;
		}

		private static void UnpackAndThrowOnError(Response errorResponse)
		{
			if (errorResponse.Status == WebDriverResult.Success)
			{
				return;
			}
			Dictionary<string, object> dictionary = errorResponse.Value as Dictionary<string, object>;
			if (dictionary != null)
			{
				ErrorResponse errorResponse2 = new ErrorResponse(dictionary);
				string message = errorResponse2.Message;
				switch (errorResponse.Status)
				{
				case WebDriverResult.NoSuchElement:
					throw new NoSuchElementException(message);
				case WebDriverResult.NoSuchFrame:
					throw new NoSuchFrameException(message);
				case WebDriverResult.UnknownCommand:
					throw new NotImplementedException(message);
				case WebDriverResult.ObsoleteElement:
					throw new StaleElementReferenceException(message);
				case WebDriverResult.ElementNotDisplayed:
					throw new ElementNotVisibleException(message);
				case WebDriverResult.InvalidElementState:
				case WebDriverResult.ElementNotSelectable:
					throw new InvalidElementStateException(message);
				case WebDriverResult.UnhandledError:
					throw new InvalidOperationException(message);
				case WebDriverResult.NoSuchDocument:
					throw new NoSuchElementException(message);
				case WebDriverResult.Timeout:
					throw new WebDriverTimeoutException(message);
				case WebDriverResult.NoSuchWindow:
					throw new NoSuchWindowException(message);
				case WebDriverResult.InvalidCookieDomain:
				case WebDriverResult.UnableToSetCookie:
					throw new WebDriverException(message);
				case WebDriverResult.UnexpectedAlertOpen:
				{
					string alertText = string.Empty;
					if (dictionary.ContainsKey("alert"))
					{
						Dictionary<string, object> dictionary2 = dictionary["alert"] as Dictionary<string, object>;
						if (dictionary2 != null && dictionary2.ContainsKey("text"))
						{
							alertText = dictionary2["text"].ToString();
						}
					}
					throw new UnhandledAlertException(message, alertText);
				}
				case WebDriverResult.NoAlertPresent:
					throw new NoAlertPresentException(message);
				case WebDriverResult.AsyncScriptTimeout:
					throw new WebDriverTimeoutException(message);
				case WebDriverResult.InvalidSelector:
					throw new InvalidSelectorException(message);
				}
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[]
				{
					message,
					errorResponse.Status
				}));
			}
			throw new WebDriverException("Unexpected error. " + errorResponse.Value.ToString());
		}

		private object ParseJavaScriptReturnValue(object responseValue)
		{
			Dictionary<string, object> dictionary = responseValue as Dictionary<string, object>;
			object[] array = responseValue as object[];
			object result;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("element-6066-11e4-a52e-4f735466cecf"))
				{
					string elementId = (string)dictionary["element-6066-11e4-a52e-4f735466cecf"];
					RemoteWebElement remoteWebElement = this.CreateElement(elementId);
					result = remoteWebElement;
				}
				else if (dictionary.ContainsKey("ELEMENT"))
				{
					string elementId2 = (string)dictionary["ELEMENT"];
					RemoteWebElement remoteWebElement2 = this.CreateElement(elementId2);
					result = remoteWebElement2;
				}
				else
				{
					string[] array2 = new string[dictionary.Keys.Count];
					dictionary.Keys.CopyTo(array2, 0);
					string[] array3 = array2;
					for (int i = 0; i < array3.Length; i++)
					{
						string key = array3[i];
						dictionary[key] = this.ParseJavaScriptReturnValue(dictionary[key]);
					}
					result = dictionary;
				}
			}
			else if (array != null)
			{
				bool flag = true;
				List<object> list = new List<object>();
				object[] array4 = array;
				for (int j = 0; j < array4.Length; j++)
				{
					object responseValue2 = array4[j];
					object obj = this.ParseJavaScriptReturnValue(responseValue2);
					if (!(obj is IWebElement))
					{
						flag = false;
					}
					list.Add(obj);
				}
				if (list.Count > 0 && flag)
				{
					List<IWebElement> list2 = new List<IWebElement>();
					foreach (object current in list)
					{
						IWebElement item = current as IWebElement;
						list2.Add(item);
					}
					result = list2.AsReadOnly();
				}
				else
				{
					result = list.AsReadOnly();
				}
			}
			else
			{
				result = responseValue;
			}
			return result;
		}
	}
}
