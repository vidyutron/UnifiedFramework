using System;

namespace OpenQA.Selenium.Remote
{
	public interface ICommandServer : IDisposable
	{
		void Start();
	}
}
