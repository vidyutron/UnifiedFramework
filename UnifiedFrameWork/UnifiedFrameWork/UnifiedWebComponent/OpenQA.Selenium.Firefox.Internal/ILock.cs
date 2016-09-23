using System;

namespace OpenQA.Selenium.Firefox.Internal
{
	internal interface ILock : IDisposable
	{
		[Obsolete("Timeouts should be expressed as a TimeSpan. Use the LockObject overload taking a TimeSpan parameter instead")]
		void LockObject(long timeoutInMilliseconds);

		void LockObject(TimeSpan timeout);

		void UnlockObject();
	}
}
