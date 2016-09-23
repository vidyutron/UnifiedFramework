using System;
using System.Globalization;
using System.Threading;

namespace OpenQA.Selenium.Support.UI
{
	public abstract class SlowLoadableComponent<T> : LoadableComponent<T> where T : SlowLoadableComponent<T>
	{
		private readonly IClock clock;

		private readonly TimeSpan timeout;

		private TimeSpan sleepInterval = TimeSpan.FromMilliseconds(200.0);

		public TimeSpan SleepInterval
		{
			get
			{
				return this.sleepInterval;
			}
			set
			{
				this.sleepInterval = value;
			}
		}

		protected SlowLoadableComponent(TimeSpan timeout) : this(timeout, new SystemClock())
		{
		}

		protected SlowLoadableComponent(TimeSpan timeout, IClock clock)
		{
			this.clock = clock;
			this.timeout = timeout;
		}

		public override T Load()
		{
			if (base.IsLoaded)
			{
				return (T)((object)this);
			}
			base.TryLoad();
			DateTime otherDateTime = this.clock.LaterBy(this.timeout);
			while (this.clock.IsNowBefore(otherDateTime))
			{
				if (base.IsLoaded)
				{
					return (T)((object)this);
				}
				this.HandleErrors();
				this.Wait();
			}
			if (base.IsLoaded)
			{
				return (T)((object)this);
			}
			string message = string.Format(CultureInfo.InvariantCulture, "Timed out after {0} seconds.", new object[]
			{
				this.timeout.TotalSeconds
			});
			throw new WebDriverTimeoutException(message);
		}

		protected virtual void HandleErrors()
		{
		}

		private void Wait()
		{
			Thread.Sleep(this.sleepInterval);
		}
	}
}
