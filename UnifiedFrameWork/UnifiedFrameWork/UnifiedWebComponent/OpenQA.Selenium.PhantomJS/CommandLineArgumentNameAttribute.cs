using System;

namespace OpenQA.Selenium.PhantomJS
{
	internal sealed class CommandLineArgumentNameAttribute : Attribute
	{
		private string argumentName = string.Empty;

		public string Name
		{
			get
			{
				return this.argumentName;
			}
		}

		public CommandLineArgumentNameAttribute(string argumentName)
		{
			this.argumentName = argumentName;
		}
	}
}
