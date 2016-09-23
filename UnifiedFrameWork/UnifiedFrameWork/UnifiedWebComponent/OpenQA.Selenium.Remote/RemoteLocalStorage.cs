using OpenQA.Selenium.Html5;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
	public class RemoteLocalStorage : ILocalStorage
	{
		private RemoteWebDriver driver;

		public int Count
		{
			get
			{
				Response response = this.driver.InternalExecute(DriverCommand.GetLocalStorageSize, null);
				return Convert.ToInt32(response.Value, CultureInfo.InvariantCulture);
			}
		}

		public RemoteLocalStorage(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public string GetItem(string key)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("key", key);
			Response response = this.driver.InternalExecute(DriverCommand.GetLocalStorageItem, dictionary);
			if (response.Value == null)
			{
				return null;
			}
			return response.Value.ToString();
		}

		public ReadOnlyCollection<string> KeySet()
		{
			List<string> list = new List<string>();
			Response response = this.driver.InternalExecute(DriverCommand.GetLocalStorageKeys, null);
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
			this.driver.InternalExecute(DriverCommand.SetLocalStorageItem, dictionary);
		}

		public string RemoveItem(string key)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("key", key);
			Response response = this.driver.InternalExecute(DriverCommand.RemoveLocalStorageItem, dictionary);
			if (response.Value == null)
			{
				return null;
			}
			return response.Value.ToString();
		}

		public void Clear()
		{
			this.driver.InternalExecute(DriverCommand.ClearLocalStorage, null);
		}
	}
}
