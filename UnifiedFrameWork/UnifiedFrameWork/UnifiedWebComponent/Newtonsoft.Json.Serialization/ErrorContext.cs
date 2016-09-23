using System;

namespace Newtonsoft.Json.Serialization
{
	internal class ErrorContext
	{
		internal bool Traced
		{
			get;
			set;
		}

		public Exception Error
		{
			get;
			private set;
		}

		public object OriginalObject
		{
			get;
			private set;
		}

		public object Member
		{
			get;
			private set;
		}

		public string Path
		{
			get;
			private set;
		}

		public bool Handled
		{
			get;
			set;
		}

		internal ErrorContext(object originalObject, object member, string path, Exception error)
		{
			this.OriginalObject = originalObject;
			this.Member = member;
			this.Error = error;
			this.Path = path;
		}
	}
}
