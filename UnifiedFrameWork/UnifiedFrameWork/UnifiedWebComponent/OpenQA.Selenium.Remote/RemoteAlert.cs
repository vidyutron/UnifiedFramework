using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteAlert : IAlert
	{
		private RemoteWebDriver driver;

		public string Text
		{
			get
			{
				Response response = this.driver.InternalExecute(DriverCommand.GetAlertText, null);
				return response.Value.ToString();
			}
		}

		public RemoteAlert(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public void Dismiss()
		{
			this.driver.InternalExecute(DriverCommand.DismissAlert, null);
		}

		public void Accept()
		{
			this.driver.InternalExecute(DriverCommand.AcceptAlert, null);
		}

		public void SendKeys(string keysToSend)
		{
			if (keysToSend == null)
			{
				throw new ArgumentNullException("keysToSend", "Keys to send must not be null.");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (this.driver.IsSpecificationCompliant)
			{
				dictionary.Add("text", keysToSend.ToCharArray());
			}
			else
			{
				dictionary.Add("text", keysToSend);
			}
			this.driver.InternalExecute(DriverCommand.SetAlertValue, dictionary);
		}

		public void SetAuthenticationCredentials(string userName, string password)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("username", userName);
			dictionary.Add("password", password);
			this.driver.InternalExecute(DriverCommand.SetAlertCredentials, dictionary);
		}
	}
}
