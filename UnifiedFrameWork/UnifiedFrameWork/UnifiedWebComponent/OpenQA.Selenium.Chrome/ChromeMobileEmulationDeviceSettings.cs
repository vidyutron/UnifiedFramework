using System;

namespace OpenQA.Selenium.Chrome
{
	public class ChromeMobileEmulationDeviceSettings
	{
		private string userAgent = string.Empty;

		private long width;

		private long height;

		private double pixelRatio;

		private bool enableTouchEvents = true;

		public string UserAgent
		{
			get
			{
				return this.userAgent;
			}
			set
			{
				this.userAgent = value;
			}
		}

		public long Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		public long Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		public double PixelRatio
		{
			get
			{
				return this.pixelRatio;
			}
			set
			{
				this.pixelRatio = value;
			}
		}

		public bool EnableTouchEvents
		{
			get
			{
				return this.enableTouchEvents;
			}
			set
			{
				this.enableTouchEvents = value;
			}
		}

		public ChromeMobileEmulationDeviceSettings()
		{
		}

		public ChromeMobileEmulationDeviceSettings(string userAgent)
		{
			this.userAgent = userAgent;
		}
	}
}
