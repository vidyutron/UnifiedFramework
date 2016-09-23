using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteTimeouts : ITimeouts
	{
		private RemoteWebDriver driver;

		public RemoteTimeouts(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public ITimeouts ImplicitlyWait(TimeSpan timeToWait)
		{
			this.ExecuteSetTimeout("implicit", timeToWait);
			return this;
		}

		public ITimeouts SetScriptTimeout(TimeSpan timeToWait)
		{
			this.ExecuteSetTimeout("script", timeToWait);
			return this;
		}

		public ITimeouts SetPageLoadTimeout(TimeSpan timeToWait)
		{
			this.ExecuteSetTimeout("page load", timeToWait);
			return this;
		}

		private void ExecuteSetTimeout(string timeoutType, TimeSpan timeToWait)
		{
			double num = timeToWait.TotalMilliseconds;
			if (timeToWait == TimeSpan.MinValue)
			{
				num = -1.0;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("type", timeoutType);
			dictionary.Add("ms", num);
			this.driver.InternalExecute(DriverCommand.SetTimeout, dictionary);
		}
	}
}
