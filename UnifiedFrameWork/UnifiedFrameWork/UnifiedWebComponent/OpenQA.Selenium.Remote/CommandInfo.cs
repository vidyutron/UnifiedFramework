using System;
using System.Globalization;
using System.Net;

namespace OpenQA.Selenium.Remote
{
	public class CommandInfo
	{
		public const string PostCommand = "POST";

		public const string GetCommand = "GET";

		public const string DeleteCommand = "DELETE";

		private const string SessionIdPropertyName = "sessionId";

		private string resourcePath;

		private string method;

		public string ResourcePath
		{
			get
			{
				return this.resourcePath;
			}
		}

		public string Method
		{
			get
			{
				return this.method;
			}
		}

		public CommandInfo(string method, string resourcePath)
		{
			this.resourcePath = resourcePath;
			this.method = method;
		}

		public HttpWebRequest CreateWebRequest(Uri baseUri, Command commandToExecute)
		{
			string[] array = this.resourcePath.Split(new string[]
			{
				"/"
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.StartsWith("{", StringComparison.OrdinalIgnoreCase) && text.EndsWith("}", StringComparison.OrdinalIgnoreCase))
				{
					array[i] = CommandInfo.GetCommandPropertyValue(text, commandToExecute);
				}
			}
			string text2 = string.Join("/", array);
			Uri relativeUri = new Uri(text2, UriKind.Relative);
			Uri requestUri;
			bool flag = Uri.TryCreate(baseUri, relativeUri, out requestUri);
			if (flag)
			{
				HttpWebRequest httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;
				httpWebRequest.Method = this.method;
				return httpWebRequest;
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to create URI from base {0} and relative path {1}", new object[]
			{
				(baseUri == null) ? string.Empty : baseUri.ToString(),
				text2
			}));
		}

		private static string GetCommandPropertyValue(string propertyName, Command commandToExecute)
		{
			string result = string.Empty;
			propertyName = propertyName.Substring(1, propertyName.Length - 2);
			if (propertyName == "sessionId")
			{
				if (commandToExecute.SessionId != null)
				{
					result = commandToExecute.SessionId.ToString();
				}
			}
			else if (commandToExecute.Parameters != null && commandToExecute.Parameters.Count > 0 && commandToExecute.Parameters.ContainsKey(propertyName) && commandToExecute.Parameters[propertyName] != null)
			{
				result = commandToExecute.Parameters[propertyName].ToString();
				commandToExecute.Parameters.Remove(propertyName);
			}
			return result;
		}
	}
}
