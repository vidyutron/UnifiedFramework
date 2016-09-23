using Newtonsoft.Json;
using System;

namespace OpenQA.Selenium.Remote
{
	internal class CharArrayJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType != null && objectType.IsAssignableFrom(typeof(char[]));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (writer != null)
			{
				writer.WriteStartArray();
				char[] array = value as char[];
				if (array != null)
				{
					char[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						char value2 = array2[i];
						int num = Convert.ToInt32(value2);
						if (num >= 32 && num <= 126)
						{
							writer.WriteValue(value2);
						}
						else
						{
							string str = "\\u" + Convert.ToString(num, 16).PadLeft(4, '0');
							writer.WriteRawValue("\"" + str + "\"");
						}
					}
				}
				writer.WriteEndArray();
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
