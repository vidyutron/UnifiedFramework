using System;

namespace OpenQA.Selenium.Html5
{
	public interface IHasLocationContext
	{
		bool HasLocationContext
		{
			get;
		}

		ILocationContext LocationContext
		{
			get;
		}
	}
}
