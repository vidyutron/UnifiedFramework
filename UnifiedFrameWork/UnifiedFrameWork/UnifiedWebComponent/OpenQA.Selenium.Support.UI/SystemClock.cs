using System;

namespace OpenQA.Selenium.Support.UI
{
	public class SystemClock : IClock
	{
		public DateTime Now
		{
			get
			{
				return DateTime.Now;
			}
		}

		public DateTime LaterBy(TimeSpan delay)
		{
			return DateTime.Now.Add(delay);
		}

		public bool IsNowBefore(DateTime otherDateTime)
		{
			return DateTime.Now < otherDateTime;
		}
	}
}
