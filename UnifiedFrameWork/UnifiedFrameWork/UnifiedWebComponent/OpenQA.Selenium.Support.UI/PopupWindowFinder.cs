using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenQA.Selenium.Support.UI
{
	public class PopupWindowFinder
	{
		private readonly IWebDriver driver;

		private readonly TimeSpan timeout;

		private readonly TimeSpan sleepInterval;

		private static TimeSpan DefaultTimeout
		{
			get
			{
				return TimeSpan.FromSeconds(5.0);
			}
		}

		private static TimeSpan DefaultSleepInterval
		{
			get
			{
				return TimeSpan.FromMilliseconds(250.0);
			}
		}

		public PopupWindowFinder(IWebDriver driver) : this(driver, PopupWindowFinder.DefaultTimeout, PopupWindowFinder.DefaultSleepInterval)
		{
		}

		public PopupWindowFinder(IWebDriver driver, TimeSpan timeout) : this(driver, timeout, PopupWindowFinder.DefaultSleepInterval)
		{
		}

		public PopupWindowFinder(IWebDriver driver, TimeSpan timeout, TimeSpan sleepInterval)
		{
			this.driver = driver;
			this.timeout = timeout;
			this.sleepInterval = sleepInterval;
		}

		public string Click(IWebElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element", "element cannot be null");
			}
			return this.Invoke(delegate
			{
				element.Click();
			});
		}

		public string Invoke(Action popupMethod)
		{
			if (popupMethod == null)
			{
				throw new ArgumentNullException("popupMethod", "popupMethod cannot be null");
			}
			IList<string> existingHandles = this.driver.WindowHandles;
			popupMethod();
			WebDriverWait webDriverWait = new WebDriverWait(new SystemClock(), this.driver, this.timeout, this.sleepInterval);
			return webDriverWait.Until<string>(delegate(IWebDriver d)
			{
				string result = null;
				IList<string> difference = PopupWindowFinder.GetDifference(existingHandles, this.driver.WindowHandles);
				if (difference.Count > 0)
				{
					result = difference[0];
				}
				return result;
			});
		}

		private static IList<string> GetDifference(IList<string> existingHandles, IList<string> currentHandles)
		{
			return currentHandles.Except(existingHandles).ToList<string>();
		}
	}
}
