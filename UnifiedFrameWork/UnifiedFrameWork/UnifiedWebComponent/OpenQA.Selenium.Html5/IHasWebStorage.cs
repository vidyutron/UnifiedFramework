using System;

namespace OpenQA.Selenium.Html5
{
	public interface IHasWebStorage
	{
		bool HasWebStorage
		{
			get;
		}

		IWebStorage WebStorage
		{
			get;
		}
	}
}
