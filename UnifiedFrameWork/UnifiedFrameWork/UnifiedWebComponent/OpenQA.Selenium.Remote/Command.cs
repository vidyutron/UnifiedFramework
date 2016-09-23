using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Remote
{
	public class Command
	{
		private SessionId commandSessionId;

		private string commandName;

		private Dictionary<string, object> commandParameters = new Dictionary<string, object>();

		[JsonProperty("sessionId")]
		public SessionId SessionId
		{
			get
			{
				return this.commandSessionId;
			}
		}

		[JsonProperty("name")]
		public string Name
		{
			get
			{
				return this.commandName;
			}
		}

		[JsonProperty("parameters")]
		public Dictionary<string, object> Parameters
		{
			get
			{
				return this.commandParameters;
			}
		}

		public string ParametersAsJsonString
		{
			get
			{
				string text = string.Empty;
				if (this.commandParameters != null && this.commandParameters.Count > 0)
				{
					text = JsonConvert.SerializeObject(this.commandParameters);
				}
				if (string.IsNullOrEmpty(text))
				{
					text = "{}";
				}
				return text;
			}
		}

		public Command(string name, string jsonParameters) : this(null, name, Command.ConvertParametersFromJson(jsonParameters))
		{
		}

		public Command(SessionId sessionId, string name, Dictionary<string, object> parameters)
		{
			this.commandSessionId = sessionId;
			if (parameters != null)
			{
				this.commandParameters = parameters;
			}
			this.commandName = name;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[",
				this.SessionId,
				"]: ",
				this.Name,
				" ",
				this.Parameters.ToString()
			});
		}

		private static Dictionary<string, object> ConvertParametersFromJson(string value)
		{
			return JsonConvert.DeserializeObject<Dictionary<string, object>>(value, new JsonConverter[]
			{
				new ResponseValueJsonConverter()
			});
		}
	}
}
