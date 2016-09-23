using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace OpenQA.Selenium.Support.UI
{
	[Serializable]
	public class UnexpectedTagNameException : WebDriverException
	{
		public UnexpectedTagNameException(string expected, string actual) : base(string.Format(CultureInfo.InvariantCulture, "Element should have been {0} but was {1}", new object[]
		{
			expected,
			actual
		}))
		{
		}

		public UnexpectedTagNameException()
		{
		}

		public UnexpectedTagNameException(string message) : base(message)
		{
		}

		public UnexpectedTagNameException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UnexpectedTagNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
