using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace OpenQA.Selenium.Remote
{
	public class RemoteWebElement : IWebElement, ISearchContext, IFindsByLinkText, IFindsById, IFindsByName, IFindsByTagName, IFindsByClassName, IFindsByXPath, IFindsByPartialLinkText, IFindsByCssSelector, IWrapsDriver, ILocatable, ITakesScreenshot
	{
		private RemoteWebDriver driver;

		private string elementId;

		public IWebDriver WrappedDriver
		{
			get
			{
				return this.driver;
			}
		}

		public string TagName
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("id", this.elementId);
				Response response = this.Execute(DriverCommand.GetElementTagName, dictionary);
				return response.Value.ToString();
			}
		}

		public string Text
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("id", this.elementId);
				Response response = this.Execute(DriverCommand.GetElementText, dictionary);
				return response.Value.ToString();
			}
		}

		public bool Enabled
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("id", this.elementId);
				Response response = this.Execute(DriverCommand.IsElementEnabled, dictionary);
				return (bool)response.Value;
			}
		}

		public bool Selected
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("id", this.elementId);
				Response response = this.Execute(DriverCommand.IsElementSelected, dictionary);
				return (bool)response.Value;
			}
		}

		public Point Location
		{
			get
			{
				string commandToExecute = DriverCommand.GetElementLocation;
				if (this.driver.IsSpecificationCompliant)
				{
					commandToExecute = DriverCommand.GetElementRect;
				}
				Response response = this.Execute(commandToExecute, new Dictionary<string, object>
				{
					{
						"id",
						this.Id
					}
				});
				Dictionary<string, object> dictionary = (Dictionary<string, object>)response.Value;
				int x = Convert.ToInt32(dictionary["x"], CultureInfo.InvariantCulture);
				int y = Convert.ToInt32(dictionary["y"], CultureInfo.InvariantCulture);
				return new Point(x, y);
			}
		}

		public Size Size
		{
			get
			{
				string commandToExecute = DriverCommand.GetElementSize;
				if (this.driver.IsSpecificationCompliant)
				{
					commandToExecute = DriverCommand.GetElementRect;
				}
				Response response = this.Execute(commandToExecute, new Dictionary<string, object>
				{
					{
						"id",
						this.Id
					}
				});
				Dictionary<string, object> dictionary = (Dictionary<string, object>)response.Value;
				int width = Convert.ToInt32(dictionary["width"], CultureInfo.InvariantCulture);
				int height = Convert.ToInt32(dictionary["height"], CultureInfo.InvariantCulture);
				return new Size(width, height);
			}
		}

		public bool Displayed
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("id", this.Id);
				Response response = this.Execute(DriverCommand.IsElementDisplayed, dictionary);
				return (bool)response.Value;
			}
		}

		public Point LocationOnScreenOnceScrolledIntoView
		{
			get
			{
				Dictionary<string, object> dictionary;
				if (this.driver.IsSpecificationCompliant)
				{
					dictionary = (this.driver.ExecuteScript("return arguments[0].getBoundingClientRect();", new object[]
					{
						this
					}) as Dictionary<string, object>);
				}
				else
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("id", this.Id);
					Response response = this.Execute(DriverCommand.GetElementLocationOnceScrolledIntoView, dictionary2);
					dictionary = (Dictionary<string, object>)response.Value;
				}
				int x = Convert.ToInt32(dictionary["x"], CultureInfo.InvariantCulture);
				int y = Convert.ToInt32(dictionary["y"], CultureInfo.InvariantCulture);
				return new Point(x, y);
			}
		}

		public ICoordinates Coordinates
		{
			get
			{
				return new RemoteCoordinates(this);
			}
		}

		internal string InternalElementId
		{
			get
			{
				return this.elementId;
			}
		}

		protected string Id
		{
			get
			{
				return this.elementId;
			}
		}

		public RemoteWebElement(RemoteWebDriver parentDriver, string id)
		{
			this.driver = parentDriver;
			this.elementId = id;
		}

		public void Clear()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.elementId);
			this.Execute(DriverCommand.ClearElement, dictionary);
		}

		public void SendKeys(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text", "text cannot be null");
			}
			if (this.driver.FileDetector.IsFile(text))
			{
				text = this.UploadFile(text);
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.elementId);
			if (this.driver.IsSpecificationCompliant)
			{
				dictionary.Add("value", text.ToCharArray());
			}
			else
			{
				dictionary.Add("value", new object[]
				{
					text
				});
			}
			this.Execute(DriverCommand.SendKeysToElement, dictionary);
		}

		public void Submit()
		{
			if (this.driver.IsSpecificationCompliant)
			{
				IWebElement webElement = this.FindElement(By.XPath("./ancestor-or-self::form"));
				this.driver.ExecuteScript("var e = arguments[0].ownerDocument.createEvent('Event');e.initEvent('submit', true, true);if (arguments[0].dispatchEvent(e)) { arguments[0].submit(); }", new object[]
				{
					webElement
				});
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.elementId);
			this.Execute(DriverCommand.SubmitElement, dictionary);
		}

		public void Click()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.elementId);
			this.Execute(DriverCommand.ClickElement, dictionary);
		}

		public string GetAttribute(string attributeName)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.elementId);
			dictionary.Add("name", attributeName);
			Response response = this.Execute(DriverCommand.GetElementAttribute, dictionary);
			string text = string.Empty;
			if (response.Value == null)
			{
				text = null;
			}
			else
			{
				text = response.Value.ToString();
				if (response.Value is bool)
				{
					text = text.ToLowerInvariant();
				}
			}
			return text;
		}

		public string GetCssValue(string propertyName)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.Id);
			if (this.driver.IsSpecificationCompliant)
			{
				dictionary.Add("name", propertyName);
			}
			else
			{
				dictionary.Add("propertyName", propertyName);
			}
			Response response = this.Execute(DriverCommand.GetElementValueOfCssProperty, dictionary);
			return response.Value.ToString();
		}

		public ReadOnlyCollection<IWebElement> FindElements(By by)
		{
			if (by == null)
			{
				throw new ArgumentNullException("by", "by cannot be null");
			}
			return by.FindElements(this);
		}

		public IWebElement FindElement(By by)
		{
			if (by == null)
			{
				throw new ArgumentNullException("by", "by cannot be null");
			}
			return by.FindElement(this);
		}

		public IWebElement FindElementByLinkText(string linkText)
		{
			return this.FindElement("link text", linkText);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText)
		{
			return this.FindElements("link text", linkText);
		}

		public IWebElement FindElementById(string id)
		{
			if (this.driver.IsSpecificationCompliant)
			{
				return this.FindElement("css selector", "#" + RemoteWebDriver.EscapeCssSelector(id));
			}
			return this.FindElement("id", id);
		}

		public ReadOnlyCollection<IWebElement> FindElementsById(string id)
		{
			if (this.driver.IsSpecificationCompliant)
			{
				return this.FindElements("css selector", "#" + RemoteWebDriver.EscapeCssSelector(id));
			}
			return this.FindElements("id", id);
		}

		public IWebElement FindElementByName(string name)
		{
			if (this.driver.IsSpecificationCompliant)
			{
				return this.FindElement("css selector", "*[name=\"" + name + "\"]");
			}
			return this.FindElement("name", name);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
		{
			if (this.driver.IsSpecificationCompliant)
			{
				return this.FindElements("css selector", "*[name=\"" + name + "\"]");
			}
			return this.FindElements("name", name);
		}

		public IWebElement FindElementByTagName(string tagName)
		{
			if (this.driver.IsSpecificationCompliant)
			{
				return this.FindElement("css selector", tagName);
			}
			return this.FindElement("tag name", tagName);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
		{
			if (this.driver.IsSpecificationCompliant)
			{
				return this.FindElements("css selector", tagName);
			}
			return this.FindElements("tag name", tagName);
		}

		public IWebElement FindElementByClassName(string className)
		{
			if (this.driver.IsSpecificationCompliant)
			{
				return this.FindElement("css selector", "." + RemoteWebDriver.EscapeCssSelector(className));
			}
			return this.FindElement("class name", className);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
		{
			if (this.driver.IsSpecificationCompliant)
			{
				return this.FindElements("css selector", "." + RemoteWebDriver.EscapeCssSelector(className));
			}
			return this.FindElements("class name", className);
		}

		public IWebElement FindElementByXPath(string xpath)
		{
			return this.FindElement("xpath", xpath);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
		{
			return this.FindElements("xpath", xpath);
		}

		public IWebElement FindElementByPartialLinkText(string partialLinkText)
		{
			return this.FindElement("partial link text", partialLinkText);
		}

		public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
		{
			return this.FindElements("partial link text", partialLinkText);
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
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.elementId);
			Response response = this.Execute(DriverCommand.ElementScreenshot, dictionary);
			string base64EncodedScreenshot = response.Value.ToString();
			return new Screenshot(base64EncodedScreenshot);
		}

		public override int GetHashCode()
		{
			return this.elementId.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			IWebElement webElement = obj as IWebElement;
			if (webElement == null)
			{
				return false;
			}
			IWrapsElement wrapsElement = obj as IWrapsElement;
			if (wrapsElement != null)
			{
				webElement = wrapsElement.WrappedElement;
			}
			RemoteWebElement remoteWebElement = webElement as RemoteWebElement;
			if (remoteWebElement == null)
			{
				return false;
			}
			if (this.elementId == remoteWebElement.Id)
			{
				return true;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.elementId);
			dictionary.Add("other", remoteWebElement.Id);
			Response response = this.Execute(DriverCommand.ElementEquals, dictionary);
			object value = response.Value;
			return value != null && value is bool && (bool)value;
		}

		protected IWebElement FindElement(string mechanism, string value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.elementId);
			dictionary.Add("using", mechanism);
			dictionary.Add("value", value);
			Response response = this.Execute(DriverCommand.FindChildElement, dictionary);
			return this.driver.GetElementFromResponse(response);
		}

		protected ReadOnlyCollection<IWebElement> FindElements(string mechanism, string value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", this.elementId);
			dictionary.Add("using", mechanism);
			dictionary.Add("value", value);
			Response response = this.Execute(DriverCommand.FindChildElements, dictionary);
			return this.driver.GetElementsFromResponse(response);
		}

		protected Response Execute(string commandToExecute, Dictionary<string, object> parameters)
		{
			return this.driver.InternalExecute(commandToExecute, parameters);
		}

		private string UploadFile(string localFile)
		{
			string value = string.Empty;
			string result;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (ZipStorer zipStorer = ZipStorer.Create(memoryStream, string.Empty))
					{
						string fileName = Path.GetFileName(localFile);
						zipStorer.AddFile(ZipStorer.CompressionMethod.Deflate, localFile, fileName, string.Empty);
						value = Convert.ToBase64String(memoryStream.ToArray());
					}
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("file", value);
				Response response = this.Execute(DriverCommand.UploadFile, dictionary);
				result = response.Value.ToString();
			}
			catch (IOException innerException)
			{
				throw new WebDriverException("Cannot upload " + localFile, innerException);
			}
			return result;
		}
	}
}
