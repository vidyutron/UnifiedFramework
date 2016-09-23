using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteKeyboard : IKeyboard
	{
		private RemoteWebDriver driver;

		public RemoteKeyboard(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public void SendKeys(string keySequence)
		{
			if (keySequence == null)
			{
				throw new ArgumentException("key sequence to send must not be null", "keySequence");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("value", keySequence.ToCharArray());
			this.driver.InternalExecute(DriverCommand.SendKeysToActiveElement, dictionary);
		}

		public void PressKey(string keyToPress)
		{
			if (keyToPress == null)
			{
				throw new ArgumentException("key to press must not be null", "keyToPress");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("value", keyToPress.ToCharArray());
			this.driver.InternalExecute(DriverCommand.SendKeysToActiveElement, dictionary);
		}

		public void ReleaseKey(string keyToRelease)
		{
			if (keyToRelease == null)
			{
				throw new ArgumentException("key to release must not be null", "keyToRelease");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("value", keyToRelease.ToCharArray());
			this.driver.InternalExecute(DriverCommand.SendKeysToActiveElement, dictionary);
		}
	}
}
