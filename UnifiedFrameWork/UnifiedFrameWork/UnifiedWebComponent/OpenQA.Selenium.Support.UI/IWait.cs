using System;

namespace OpenQA.Selenium.Support.UI
{
	public interface IWait<T>
	{
		TimeSpan Timeout
		{
			get;
			set;
		}

		TimeSpan PollingInterval
		{
			get;
			set;
		}

		string Message
		{
			get;
			set;
		}

		void IgnoreExceptionTypes(params Type[] exceptionTypes);

		TResult Until<TResult>(Func<T, TResult> condition);
	}
}
