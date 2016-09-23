using System;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Html5
{
	public interface ISessionStorage
	{
		int Count
		{
			get;
		}

		string GetItem(string key);

		ReadOnlyCollection<string> KeySet();

		void SetItem(string key, string value);

		string RemoveItem(string key);

		void Clear();
	}
}
