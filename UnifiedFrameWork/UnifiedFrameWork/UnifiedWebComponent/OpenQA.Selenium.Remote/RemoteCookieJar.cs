using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteCookieJar : ICookieJar
	{
		private RemoteWebDriver driver;

		public ReadOnlyCollection<Cookie> AllCookies
		{
			get
			{
				return this.GetAllCookies();
			}
		}

		public RemoteCookieJar(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public void AddCookie(Cookie cookie)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("cookie", cookie);
			this.driver.InternalExecute(DriverCommand.AddCookie, dictionary);
		}

		public void DeleteCookieNamed(string name)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("name", name);
			this.driver.InternalExecute(DriverCommand.DeleteCookie, dictionary);
		}

		public void DeleteCookie(Cookie cookie)
		{
			if (cookie != null)
			{
				this.DeleteCookieNamed(cookie.Name);
			}
		}

		public void DeleteAllCookies()
		{
			this.driver.InternalExecute(DriverCommand.DeleteAllCookies, null);
		}

		public Cookie GetCookieNamed(string name)
		{
			Cookie result = null;
			if (name != null)
			{
				ReadOnlyCollection<Cookie> allCookies = this.AllCookies;
				foreach (Cookie current in allCookies)
				{
					if (name.Equals(current.Name))
					{
						result = current;
						break;
					}
				}
			}
			return result;
		}

		private ReadOnlyCollection<Cookie> GetAllCookies()
		{
			List<Cookie> list = new List<Cookie>();
			object value = this.driver.InternalExecute(DriverCommand.GetAllCookies, new Dictionary<string, object>()).Value;
			ReadOnlyCollection<Cookie> result;
			try
			{
				object[] array = value as object[];
				if (array != null)
				{
					object[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						object obj = array2[i];
						Dictionary<string, object> rawCookie = obj as Dictionary<string, object>;
						if (obj != null)
						{
							list.Add(Cookie.FromDictionary(rawCookie));
						}
					}
				}
				result = new ReadOnlyCollection<Cookie>(list);
			}
			catch (Exception innerException)
			{
				throw new WebDriverException("Unexpected problem getting cookies", innerException);
			}
			return result;
		}
	}
}
