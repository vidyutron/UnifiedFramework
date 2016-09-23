using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace OpenQA.Selenium.Support.UI
{
	public class DefaultWait<T> : IWait<T>
	{
		private T input;

		private IClock clock;

		private TimeSpan timeout = DefaultWait<T>.DefaultSleepTimeout;

		private TimeSpan sleepInterval = DefaultWait<T>.DefaultSleepTimeout;

		private string message = string.Empty;

		private List<Type> ignoredExceptions = new List<Type>();

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		public TimeSpan PollingInterval
		{
			get
			{
				return this.sleepInterval;
			}
			set
			{
				this.sleepInterval = value;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		private static TimeSpan DefaultSleepTimeout
		{
			get
			{
				return TimeSpan.FromMilliseconds(500.0);
			}
		}

		public DefaultWait(T input) : this(input, new SystemClock())
		{
		}

		public DefaultWait(T input, IClock clock)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input", "input cannot be null");
			}
			if (clock == null)
			{
				throw new ArgumentNullException("clock", "input cannot be null");
			}
			this.input = input;
			this.clock = clock;
		}

		public void IgnoreExceptionTypes(params Type[] exceptionTypes)
		{
			if (exceptionTypes == null)
			{
				throw new ArgumentNullException("exceptionTypes", "exceptionTypes cannot be null");
			}
			for (int i = 0; i < exceptionTypes.Length; i++)
			{
				Type c = exceptionTypes[i];
				if (!typeof(Exception).IsAssignableFrom(c))
				{
					throw new ArgumentException("All types to be ignored must derive from System.Exception", "exceptionTypes");
				}
			}
			this.ignoredExceptions.AddRange(exceptionTypes);
		}

		public TResult Until<TResult>(Func<T, TResult> condition)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition", "condition cannot be null");
			}
			Type typeFromHandle = typeof(TResult);
			if ((typeFromHandle.IsValueType && typeFromHandle != typeof(bool)) || !typeof(object).IsAssignableFrom(typeFromHandle))
			{
				throw new ArgumentException("Can only wait on an object or boolean response, tried to use type: " + typeFromHandle.ToString(), "condition");
			}
			Exception lastException = null;
			DateTime otherDateTime = this.clock.LaterBy(this.timeout);
			TResult result;
			while (true)
			{
				try
				{
					TResult tResult = condition(this.input);
					if (typeFromHandle == typeof(bool))
					{
						bool? flag = tResult as bool?;
						if (flag.HasValue && flag.Value)
						{
							result = tResult;
							break;
						}
					}
					else if (tResult != null)
					{
						result = tResult;
						break;
					}
				}
				catch (Exception ex)
				{
					if (!this.IsIgnoredException(ex))
					{
						throw;
					}
					lastException = ex;
				}
				if (!this.clock.IsNowBefore(otherDateTime))
				{
					string text = string.Format(CultureInfo.InvariantCulture, "Timed out after {0} seconds", new object[]
					{
						this.timeout.TotalSeconds
					});
					if (!string.IsNullOrEmpty(this.message))
					{
						text = text + ": " + this.message;
					}
					this.ThrowTimeoutException(text, lastException);
				}
				Thread.Sleep(this.sleepInterval);
			}
			return result;
		}

		protected virtual void ThrowTimeoutException(string exceptionMessage, Exception lastException)
		{
			throw new WebDriverTimeoutException(exceptionMessage, lastException);
		}

		private bool IsIgnoredException(Exception exception)
		{
			return this.ignoredExceptions.Any((Type type) => type.IsAssignableFrom(exception.GetType()));
		}
	}
}
