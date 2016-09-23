using System;

namespace OpenQA.Selenium.Support.UI
{
	public class WebDriverWait : DefaultWait<IWebDriver>
	{
		private static TimeSpan DefaultSleepTimeout
		{
			get
			{
				return TimeSpan.FromMilliseconds(500.0);
			}
		}

		public WebDriverWait(IWebDriver driver, TimeSpan timeout) : this(new SystemClock(), driver, timeout, WebDriverWait.DefaultSleepTimeout)
		{
		}

		public WebDriverWait(IClock clock, IWebDriver driver, TimeSpan timeout, TimeSpan sleepInterval) : base(driver, clock)
		{
			base.Timeout = timeout;
			base.PollingInterval = sleepInterval;
			base.IgnoreExceptionTypes(new Type[]
			{
				typeof(NotFoundException)
			});
		}
	}
}
