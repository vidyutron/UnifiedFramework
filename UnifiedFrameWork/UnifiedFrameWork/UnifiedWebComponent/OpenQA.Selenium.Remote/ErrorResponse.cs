using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Remote
{
	public class ErrorResponse
	{
		private StackTraceElement[] stackTrace;

		private string message = string.Empty;

		private string className = string.Empty;

		private string screenshot = string.Empty;

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

		public string Screenshot
		{
			get
			{
				return this.screenshot;
			}
			set
			{
				this.screenshot = value;
			}
		}

		public StackTraceElement[] StackTrace
		{
			get
			{
				return this.stackTrace;
			}
			set
			{
				this.stackTrace = value;
			}
		}

		public ErrorResponse()
		{
		}

		public ErrorResponse(Dictionary<string, object> responseValue)
		{
			if (responseValue != null)
			{
				if (responseValue.ContainsKey("message"))
				{
					if (responseValue["message"] != null)
					{
						this.message = responseValue["message"].ToString();
					}
					else
					{
						this.message = "The error did not contain a message.";
					}
				}
				if (responseValue.ContainsKey("screen") && responseValue["screen"] != null)
				{
					this.screenshot = responseValue["screen"].ToString();
				}
				if (responseValue.ContainsKey("class") && responseValue["class"] != null)
				{
					this.className = responseValue["class"].ToString();
				}
				if (responseValue.ContainsKey("stackTrace"))
				{
					object[] array = responseValue["stackTrace"] as object[];
					if (array != null)
					{
						List<StackTraceElement> list = new List<StackTraceElement>();
						object[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							object obj = array2[i];
							Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
							if (dictionary != null)
							{
								list.Add(new StackTraceElement(dictionary));
							}
						}
						this.stackTrace = list.ToArray();
					}
				}
			}
		}
	}
}
