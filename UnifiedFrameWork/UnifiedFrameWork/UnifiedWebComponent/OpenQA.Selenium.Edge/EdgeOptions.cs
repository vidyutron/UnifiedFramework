using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenQA.Selenium.Edge
{
	public class EdgeOptions
	{
		private EdgePageLoadStrategy pageLoadStrategy;

		private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();

		public EdgePageLoadStrategy PageLoadStrategy
		{
			get
			{
				return this.pageLoadStrategy;
			}
			set
			{
				this.pageLoadStrategy = value;
			}
		}

		public void AddAdditionalCapability(string capabilityName, object capabilityValue)
		{
			if (capabilityName == CapabilityType.PageLoadStrategy)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "There is already an option for the {0} capability. Please use that instead.", new object[]
				{
					capabilityName
				});
				throw new ArgumentException(message, "capabilityName");
			}
			if (string.IsNullOrEmpty(capabilityName))
			{
				throw new ArgumentException("Capability name may not be null an empty string.", "capabilityName");
			}
			this.additionalCapabilities[capabilityName] = capabilityValue;
		}

		public ICapabilities ToCapabilities()
		{
			DesiredCapabilities desiredCapabilities = DesiredCapabilities.Edge();
			if (this.pageLoadStrategy != EdgePageLoadStrategy.Default)
			{
				string capabilityValue = "normal";
				switch (this.pageLoadStrategy)
				{
				case EdgePageLoadStrategy.Eager:
					capabilityValue = "eager";
					break;
				case EdgePageLoadStrategy.None:
					capabilityValue = "none";
					break;
				}
				desiredCapabilities.SetCapability(CapabilityType.PageLoadStrategy, capabilityValue);
			}
			foreach (KeyValuePair<string, object> current in this.additionalCapabilities)
			{
				desiredCapabilities.SetCapability(current.Key, current.Value);
			}
			return desiredCapabilities;
		}
	}
}
