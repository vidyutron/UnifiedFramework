using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteWindow : IWindow
	{
		private RemoteWebDriver driver;

		public Point Position
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("windowHandle", "current");
				Response response = this.driver.InternalExecute(DriverCommand.GetWindowPosition, dictionary);
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)response.Value;
				int x = Convert.ToInt32(dictionary2["x"], CultureInfo.InvariantCulture);
				int y = Convert.ToInt32(dictionary2["y"], CultureInfo.InvariantCulture);
				return new Point(x, y);
			}
			set
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("windowHandle", "current");
				dictionary.Add("x", value.X);
				dictionary.Add("y", value.Y);
				this.driver.InternalExecute(DriverCommand.SetWindowPosition, dictionary);
			}
		}

		public Size Size
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("windowHandle", "current");
				Response response = this.driver.InternalExecute(DriverCommand.GetWindowSize, dictionary);
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)response.Value;
				int height = Convert.ToInt32(dictionary2["height"], CultureInfo.InvariantCulture);
				int width = Convert.ToInt32(dictionary2["width"], CultureInfo.InvariantCulture);
				return new Size(width, height);
			}
			set
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("windowHandle", "current");
				dictionary.Add("width", value.Width);
				dictionary.Add("height", value.Height);
				this.driver.InternalExecute(DriverCommand.SetWindowSize, dictionary);
			}
		}

		public RemoteWindow(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public void Maximize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("windowHandle", "current");
			this.driver.InternalExecute(DriverCommand.MaximizeWindow, dictionary);
		}
	}
}
