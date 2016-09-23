using System;

namespace OpenQA.Selenium.Html5
{
	public interface IWebStorage
	{
		ILocalStorage LocalStorage
		{
			get;
		}

		ISessionStorage SessionStorage
		{
			get;
		}
	}
}
