using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteTargetLocator : ITargetLocator
	{
		private RemoteWebDriver driver;

		public RemoteTargetLocator(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public IWebDriver Frame(int frameIndex)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", frameIndex);
			this.driver.InternalExecute(DriverCommand.SwitchToFrame, dictionary);
			return this.driver;
		}

		public IWebDriver Frame(string frameName)
		{
			if (frameName == null)
			{
				throw new ArgumentNullException("frameName", "Frame name cannot be null");
			}
			string text = Regex.Replace(frameName, "(['\"\\\\#.:;,!?+<>=~*^$|%&@`{}\\-/\\[\\]\\(\\)])", "\\$1");
			ReadOnlyCollection<IWebElement> readOnlyCollection = this.driver.FindElements(By.CssSelector(string.Concat(new string[]
			{
				"frame[name='",
				text,
				"'],iframe[name='",
				text,
				"']"
			})));
			if (readOnlyCollection.Count == 0)
			{
				readOnlyCollection = this.driver.FindElements(By.CssSelector("frame#" + text + ",iframe#" + text));
				if (readOnlyCollection.Count == 0)
				{
					throw new NoSuchFrameException("No frame element found with name or id " + frameName);
				}
			}
			return this.Frame(readOnlyCollection[0]);
		}

		public IWebDriver Frame(IWebElement frameElement)
		{
			if (frameElement == null)
			{
				throw new ArgumentNullException("frameElement", "Frame element cannot be null");
			}
			RemoteWebElement remoteWebElement = frameElement as RemoteWebElement;
			if (remoteWebElement == null)
			{
				IWrapsElement wrapsElement = frameElement as IWrapsElement;
				if (wrapsElement != null)
				{
					remoteWebElement = (wrapsElement.WrappedElement as RemoteWebElement);
				}
			}
			if (remoteWebElement == null)
			{
				throw new ArgumentException("frameElement cannot be converted to RemoteWebElement", "frameElement");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("ELEMENT", remoteWebElement.InternalElementId);
			dictionary.Add("element-6066-11e4-a52e-4f735466cecf", remoteWebElement.InternalElementId);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("id", dictionary);
			this.driver.InternalExecute(DriverCommand.SwitchToFrame, dictionary2);
			return this.driver;
		}

		public IWebDriver ParentFrame()
		{
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			this.driver.InternalExecute(DriverCommand.SwitchToParentFrame, parameters);
			return this.driver;
		}

		public IWebDriver Window(string windowHandleOrName)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (this.driver.IsSpecificationCompliant)
			{
				dictionary.Add("handle", windowHandleOrName);
				try
				{
					this.driver.InternalExecute(DriverCommand.SwitchToWindow, dictionary);
					IWebDriver result = this.driver;
					return result;
				}
				catch (NoSuchWindowException)
				{
					string currentWindowHandle = this.driver.CurrentWindowHandle;
					foreach (string current in this.driver.WindowHandles)
					{
						this.Window(current);
						if (windowHandleOrName == this.driver.ExecuteScript("return window.name", new object[0]).ToString())
						{
							IWebDriver result = this.driver;
							return result;
						}
					}
					this.Window(currentWindowHandle);
					throw;
				}
			}
			dictionary.Add("name", windowHandleOrName);
			this.driver.InternalExecute(DriverCommand.SwitchToWindow, dictionary);
			return this.driver;
		}

		public IWebDriver DefaultContent()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", null);
			this.driver.InternalExecute(DriverCommand.SwitchToFrame, dictionary);
			return this.driver;
		}

		public IWebElement ActiveElement()
		{
			Response response = this.driver.InternalExecute(DriverCommand.GetActiveElement, null);
			return this.driver.GetElementFromResponse(response);
		}

		public IAlert Alert()
		{
			this.driver.InternalExecute(DriverCommand.GetAlertText, null);
			return new RemoteAlert(this.driver);
		}
	}
}
