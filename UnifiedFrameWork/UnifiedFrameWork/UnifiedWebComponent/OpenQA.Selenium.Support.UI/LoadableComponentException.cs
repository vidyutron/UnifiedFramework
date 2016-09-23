using System;
using System.Runtime.Serialization;

namespace OpenQA.Selenium.Support.UI
{
	[Serializable]
	public class LoadableComponentException : WebDriverException
	{
		public LoadableComponentException()
		{
		}

		public LoadableComponentException(string message) : base(message)
		{
		}

		public LoadableComponentException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected LoadableComponentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
