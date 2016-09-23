using OpenQA.Selenium.Html5;
using System;
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
	public class RemoteApplicationCache : IApplicationCache
	{
		private RemoteWebDriver driver;

		public AppCacheStatus Status
		{
			get
			{
				Response response = this.driver.InternalExecute(DriverCommand.GetAppCacheStatus, null);
				Type typeFromHandle = typeof(AppCacheStatus);
				int num = Convert.ToInt32(response.Value, CultureInfo.InvariantCulture);
				if (!Enum.IsDefined(typeFromHandle, num))
				{
					throw new InvalidOperationException("Value returned from remote end is not a number or is not in the specified range of values. Actual value was " + response.Value.ToString());
				}
				return (AppCacheStatus)Enum.ToObject(typeFromHandle, response.Value);
			}
		}

		public RemoteApplicationCache(RemoteWebDriver driver)
		{
			this.driver = driver;
		}
	}
}
