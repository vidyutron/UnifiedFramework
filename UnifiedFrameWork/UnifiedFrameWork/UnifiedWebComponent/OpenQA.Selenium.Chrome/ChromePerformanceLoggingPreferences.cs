using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Chrome
{
	public class ChromePerformanceLoggingPreferences
	{
		private bool isCollectingNetworkEvents = true;

		private bool isCollectingPageEvents = true;

		private bool isCollectingTimelineEvents = true;

		private TimeSpan bufferUsageReportingInterval = TimeSpan.FromMilliseconds(1000.0);

		private List<string> tracingCategories = new List<string>();

		public bool IsCollectingNetworkEvents
		{
			get
			{
				return this.isCollectingNetworkEvents;
			}
			set
			{
				this.isCollectingNetworkEvents = value;
			}
		}

		public bool IsCollectingPageEvents
		{
			get
			{
				return this.isCollectingPageEvents;
			}
			set
			{
				this.isCollectingPageEvents = value;
			}
		}

		public bool IsCollectingTimelineEvents
		{
			get
			{
				return this.isCollectingTimelineEvents;
			}
			set
			{
				this.isCollectingTimelineEvents = value;
			}
		}

		public TimeSpan BufferUsageReportingInterval
		{
			get
			{
				return this.bufferUsageReportingInterval;
			}
			set
			{
				if (value.TotalMilliseconds <= 0.0)
				{
					throw new ArgumentException("Interval must be greater than zero.");
				}
				this.bufferUsageReportingInterval = value;
			}
		}

		public string TracingCategories
		{
			get
			{
				if (this.tracingCategories.Count == 0)
				{
					return string.Empty;
				}
				return string.Join(",", this.tracingCategories.ToArray());
			}
		}

		public void AddTracingCategory(string category)
		{
			if (string.IsNullOrEmpty(category))
			{
				throw new ArgumentException("category must not be null or empty", "category");
			}
			this.AddTracingCategories(new string[]
			{
				category
			});
		}

		public void AddTracingCategories(params string[] categoriesToAdd)
		{
			this.AddTracingCategories(new List<string>(categoriesToAdd));
		}

		public void AddTracingCategories(IEnumerable<string> categoriesToAdd)
		{
			if (categoriesToAdd == null)
			{
				throw new ArgumentNullException("categoriesToAdd", "categoriesToAdd must not be null");
			}
			this.isCollectingTimelineEvents = false;
			this.tracingCategories.AddRange(categoriesToAdd);
		}
	}
}
