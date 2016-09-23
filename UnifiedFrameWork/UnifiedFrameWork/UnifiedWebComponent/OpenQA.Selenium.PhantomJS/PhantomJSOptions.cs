using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.PhantomJS
{
	public class PhantomJSOptions
	{
		private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();

		public void AddAdditionalCapability(string capabilityName, object capabilityValue)
		{
			if (string.IsNullOrEmpty(capabilityName))
			{
				throw new ArgumentException("Capability name may not be null an empty string.", "capabilityName");
			}
			this.additionalCapabilities[capabilityName] = capabilityValue;
		}

		public ICapabilities ToCapabilities()
		{
			DesiredCapabilities desiredCapabilities = DesiredCapabilities.PhantomJS();
			foreach (KeyValuePair<string, object> current in this.additionalCapabilities)
			{
				desiredCapabilities.SetCapability(current.Key, current.Value);
			}
			return desiredCapabilities;
		}
	}
}
