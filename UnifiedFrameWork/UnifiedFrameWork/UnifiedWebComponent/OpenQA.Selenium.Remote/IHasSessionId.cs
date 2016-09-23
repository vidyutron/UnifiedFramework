using System;

namespace OpenQA.Selenium.Remote
{
	public interface IHasSessionId
	{
		SessionId SessionId
		{
			get;
		}
	}
}
