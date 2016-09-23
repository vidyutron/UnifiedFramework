using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
	public class StackTraceElement
	{
		private string fileName = string.Empty;

		private string className = string.Empty;

		private int lineNumber;

		private string methodName = string.Empty;

		[JsonProperty("fileName")]
		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		[JsonProperty("className")]
		public string ClassName
		{
			get
			{
				return this.className;
			}
			set
			{
				this.className = value;
			}
		}

		[JsonProperty("lineNumber")]
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
			set
			{
				this.lineNumber = value;
			}
		}

		[JsonProperty("methodName")]
		public string MethodName
		{
			get
			{
				return this.methodName;
			}
			set
			{
				this.methodName = value;
			}
		}

		public StackTraceElement()
		{
		}

		public StackTraceElement(Dictionary<string, object> elementAttributes)
		{
			if (elementAttributes != null)
			{
				if (elementAttributes.ContainsKey("className") && elementAttributes["className"] != null)
				{
					this.className = elementAttributes["className"].ToString();
				}
				if (elementAttributes.ContainsKey("methodName") && elementAttributes["methodName"] != null)
				{
					this.methodName = elementAttributes["methodName"].ToString();
				}
				if (elementAttributes.ContainsKey("lineNumber"))
				{
					this.lineNumber = Convert.ToInt32(elementAttributes["lineNumber"], CultureInfo.InvariantCulture);
				}
				if (elementAttributes.ContainsKey("fileName") && elementAttributes["fileName"] != null)
				{
					this.fileName = elementAttributes["fileName"].ToString();
				}
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "at {0}.{1} ({2}, {3})", new object[]
			{
				this.className,
				this.methodName,
				this.fileName,
				this.lineNumber
			});
		}
	}
}
