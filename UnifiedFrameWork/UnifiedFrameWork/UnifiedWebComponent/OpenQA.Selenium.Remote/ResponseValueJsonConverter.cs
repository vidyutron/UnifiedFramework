using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Remote
{
	internal class ResponseValueJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return true;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return this.ProcessToken(reader);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (serializer != null)
			{
				serializer.Serialize(writer, value);
			}
		}

		private object ProcessToken(JsonReader reader)
		{
			object result = null;
			if (reader != null)
			{
				if (reader.TokenType == JsonToken.StartObject)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					while (reader.Read() && reader.TokenType != JsonToken.EndObject)
					{
						string key = reader.Value.ToString();
						reader.Read();
						dictionary.Add(key, this.ProcessToken(reader));
					}
					result = dictionary;
				}
				else if (reader.TokenType == JsonToken.StartArray)
				{
					List<object> list = new List<object>();
					while (reader.Read() && reader.TokenType != JsonToken.EndArray)
					{
						list.Add(this.ProcessToken(reader));
					}
					result = list.ToArray();
				}
				else
				{
					reader.DateParseHandling = DateParseHandling.None;
					result = reader.Value;
				}
			}
			return result;
		}
	}
}
