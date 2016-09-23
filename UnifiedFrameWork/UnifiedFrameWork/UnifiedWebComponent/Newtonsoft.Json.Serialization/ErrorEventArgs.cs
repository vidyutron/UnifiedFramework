using System;

namespace Newtonsoft.Json.Serialization
{
	internal class ErrorEventArgs : EventArgs
	{
		public object CurrentObject
		{
			get;
			private set;
		}

		public ErrorContext ErrorContext
		{
			get;
			private set;
		}

		public ErrorEventArgs(object currentObject, ErrorContext errorContext)
		{
			this.CurrentObject = currentObject;
			this.ErrorContext = errorContext;
		}
	}
}
