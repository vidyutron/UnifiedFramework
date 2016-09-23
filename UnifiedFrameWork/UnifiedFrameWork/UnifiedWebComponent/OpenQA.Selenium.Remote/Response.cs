using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
	public class Response
	{
		private object responseValue;

		private string responseSessionId;

		private WebDriverResult responseStatus;

		private bool isSpecificationCompliant;

		public object Value
		{
			get
			{
				return this.responseValue;
			}
			set
			{
				this.responseValue = value;
			}
		}

		public string SessionId
		{
			get
			{
				return this.responseSessionId;
			}
			set
			{
				this.responseSessionId = value;
			}
		}

		public WebDriverResult Status
		{
			get
			{
				return this.responseStatus;
			}
			set
			{
				this.responseStatus = value;
			}
		}

		public bool IsSpecificationCompliant
		{
			get
			{
				return this.isSpecificationCompliant;
			}
		}

		public Response()
		{
		}

		public Response(SessionId sessionId)
		{
			if (sessionId != null)
			{
				this.responseSessionId = sessionId.ToString();
			}
		}

		private Response(Dictionary<string, object> rawResponse)
		{
			if (rawResponse.ContainsKey("sessionId") && rawResponse["sessionId"] != null)
			{
				this.responseSessionId = rawResponse["sessionId"].ToString();
			}
			if (rawResponse.ContainsKey("value"))
			{
				this.responseValue = rawResponse["value"];
			}
			if (rawResponse.ContainsKey("status"))
			{
				this.responseStatus = (WebDriverResult)Convert.ToInt32(rawResponse["status"], CultureInfo.InvariantCulture);
				return;
			}
			this.isSpecificationCompliant = true;
			if (!rawResponse.ContainsKey("value") && this.responseValue == null)
			{
				if (rawResponse.ContainsKey("capabilities"))
				{
					this.responseValue = rawResponse["capabilities"];
				}
				else
				{
					this.responseValue = rawResponse;
				}
			}
			if (rawResponse.ContainsKey("error"))
			{
				this.responseStatus = WebDriverError.ResultFromError(rawResponse["error"].ToString());
			}
		}

		public static Response FromJson(string value)
		{
			Dictionary<string, object> rawResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(value, new JsonConverter[]
			{
				new ResponseValueJsonConverter()
			});
			return new Response(rawResponse);
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "({0} {1}: {2})", new object[]
			{
				this.SessionId,
				this.Status,
				this.Value
			});
		}
	}
}
