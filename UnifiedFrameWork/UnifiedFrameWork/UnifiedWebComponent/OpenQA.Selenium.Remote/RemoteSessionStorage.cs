using OpenQA.Selenium.Html5;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
	public class RemoteSessionStorage : ISessionStorage
	{
		private RemoteWebDriver driver;

		public int Count
		{
			get
			{
				Response response = this.driver.InternalExecute(DriverCommand.GetSessionStorageSize, null);
				return Convert.ToInt32(response.Value, CultureInfo.InvariantCulture);
			}
		}

		public RemoteSessionStorage(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public string GetItem(string key)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("key", key);
			Response response = this.driver.InternalExecute(DriverCommand.GetSessionStorageItem, dictionary);
			if (response.Value == null)
			{
				return null;
			}
			return response.Value.ToString();
		}

		public ReadOnlyCollection<string> KeySet()
		{
			List<string> list = new List<string>();
			Response response = this.driver.InternalExecute(DriverCommand.GetSessionStorageKeys, null);
			object[] array = response.Value as object[];
			object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string item = (string)array2[i];
				list.Add(item);
			}
			return list.AsReadOnly();
		}

		public void SetItem(string key, string value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("key", key);
			dictionary.Add("value", value);
			this.driver.InternalExecute(DriverCommand.SetSessionStorageItem, dictionary);
		}

		public string RemoveItem(string key)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("key", key);
			Response response = this.driver.InternalExecute(DriverCommand.RemoveSessionStorageItem, dictionary);
			if (response.Value == null)
			{
				return null;
			}
			return response.Value.ToString();
		}

		public void Clear()
		{
			this.driver.InternalExecute(DriverCommand.ClearSessionStorage, null);
		}
	}
}
