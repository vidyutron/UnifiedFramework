using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.Firefox
{
	public class FirefoxOptions
	{
		private bool isMarionette = true;

		public bool IsMarionette
		{
			get
			{
				return this.isMarionette;
			}
			set
			{
				this.isMarionette = value;
			}
		}

		public ICapabilities ToCapabilities()
		{
			DesiredCapabilities desiredCapabilities = DesiredCapabilities.Firefox();
			desiredCapabilities.SetCapability("marionette", this.isMarionette);
			return desiredCapabilities;
		}
	}
}
