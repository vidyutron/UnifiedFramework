using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Newtonsoft.Json.Converters
{
	internal class KeyValuePairConverter : JsonConverter
	{
		private const string KeyName = "Key";

		private const string ValueName = "Value";

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Type type = value.GetType();
			IList<Type> genericArguments = type.GetGenericArguments();
			Type objectType = genericArguments[0];
			Type objectType2 = genericArguments[1];
			PropertyInfo property = type.GetProperty("Key");
			PropertyInfo property2 = type.GetProperty("Value");
			DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
			writer.WriteStartObject();
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Key") : "Key");
			serializer.Serialize(writer, ReflectionUtils.GetMemberValue(property, value), objectType);
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Value") : "Value");
			serializer.Serialize(writer, ReflectionUtils.GetMemberValue(property2, value), objectType2);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool flag = ReflectionUtils.IsNullableType(objectType);
			if (reader.TokenType != JsonToken.Null)
			{
				Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
				IList<Type> genericArguments = type.GetGenericArguments();
				Type objectType2 = genericArguments[0];
				Type objectType3 = genericArguments[1];
				object obj = null;
				object obj2 = null;
				reader.Read();
				while (reader.TokenType == JsonToken.PropertyName)
				{
					string a = reader.Value.ToString();
					if (string.Equals(a, "Key", StringComparison.OrdinalIgnoreCase))
					{
						reader.Read();
						obj = serializer.Deserialize(reader, objectType2);
					}
					else if (string.Equals(a, "Value", StringComparison.OrdinalIgnoreCase))
					{
						reader.Read();
						obj2 = serializer.Deserialize(reader, objectType3);
					}
					else
					{
						reader.Skip();
					}
					reader.Read();
				}
				return Activator.CreateInstance(type, new object[]
				{
					obj,
					obj2
				});
			}
			if (!flag)
			{
				throw JsonSerializationException.Create(reader, "Cannot convert null value to KeyValuePair.");
			}
			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			Type type = ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
			return type.IsValueType() && type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(KeyValuePair<, >);
		}
	}
}
