using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Linq
{
	internal class JRaw : JValue
	{
		public JRaw(JRaw other) : base(other)
		{
		}

		public JRaw(object rawJson) : base(rawJson, JTokenType.Raw)
		{
		}

		public static JRaw Create(JsonReader reader)
		{
			JRaw result;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
				{
					jsonTextWriter.WriteToken(reader);
					result = new JRaw(stringWriter.ToString());
				}
			}
			return result;
		}

		internal override JToken CloneToken()
		{
			return new JRaw(this);
		}
	}
}
