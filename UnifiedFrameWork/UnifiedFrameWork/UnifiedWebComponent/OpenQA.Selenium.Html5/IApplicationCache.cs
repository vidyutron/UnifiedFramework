using System;

namespace OpenQA.Selenium.Html5
{
	public interface IApplicationCache
	{
		AppCacheStatus Status
		{
			get;
		}
	}
}
