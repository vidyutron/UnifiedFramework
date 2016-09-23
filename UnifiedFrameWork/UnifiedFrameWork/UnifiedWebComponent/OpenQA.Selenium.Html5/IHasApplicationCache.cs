using System;

namespace OpenQA.Selenium.Html5
{
	public interface IHasApplicationCache
	{
		bool HasApplicationCache
		{
			get;
		}

		IApplicationCache ApplicationCache
		{
			get;
		}
	}
}
