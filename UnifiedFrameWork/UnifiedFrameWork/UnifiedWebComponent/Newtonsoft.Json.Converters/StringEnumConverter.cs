using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Converters
{
	internal class StringEnumConverter : JsonConverter
	{
		private readonly Dictionary<Type, BidirectionalDictionary<string, string>> _enumMemberNamesPerType = new Dictionary<Type, BidirectionalDictionary<string, string>>();

		public bool CamelCaseText
		{
			get;
			set;
		}

		public bool AllowIntegerValues
		{
			get;
			set;
		}

		public StringEnumConverter()
		{
			this.AllowIntegerValues = true;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Enum @enum = (Enum)value;
			string text = @enum.ToString("G");
			if (char.IsNumber(text[0]) || text[0] == '-')
			{
				writer.WriteValue(value);
				return;
			}
			BidirectionalDictionary<string, string> enumNameMap = this.GetEnumNameMap(@enum.GetType());
			string[] array = text.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Trim();
				string text3;
				enumNameMap.TryGetByFirst(text2, out text3);
				text3 = (text3 ?? text2);
				if (this.CamelCaseText)
				{
					text3 = StringUtils.ToCamelCase(text3);
				}
				array[i] = text3;
			}
			string value2 = string.Join(", ", array);
			writer.WriteValue(value2);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool flag = ReflectionUtils.IsNullableType(objectType);
			Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
			if (reader.TokenType != JsonToken.Null)
			{
				try
				{
					if (reader.TokenType == JsonToken.String)
					{
						string text = reader.Value.ToString();
						object result;
						if (text == string.Empty && flag)
						{
							result = null;
							return result;
						}
						BidirectionalDictionary<string, string> enumNameMap = this.GetEnumNameMap(type);
						string value;
						if (text.IndexOf(',') != -1)
						{
							string[] array = text.Split(new char[]
							{
								','
							});
							for (int i = 0; i < array.Length; i++)
							{
								string enumText = array[i].Trim();
								array[i] = StringEnumConverter.ResolvedEnumName(enumNameMap, enumText);
							}
							value = string.Join(", ", array);
						}
						else
						{
							value = StringEnumConverter.ResolvedEnumName(enumNameMap, text);
						}
						result = Enum.Parse(type, value, true);
						return result;
					}
					else if (reader.TokenType == JsonToken.Integer)
					{
						if (!this.AllowIntegerValues)
						{
							throw JsonSerializationException.Create(reader, "Integer value {0} is not allowed.".FormatWith(CultureInfo.InvariantCulture, reader.Value));
						}
						object result = ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, type);
						return result;
					}
				}
				catch (Exception ex)
				{
					throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(reader.Value), objectType), ex);
				}
				throw JsonSerializationException.Create(reader, "Unexpected token {0} when parsing enum.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			if (!ReflectionUtils.IsNullableType(objectType))
			{
				throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			return null;
		}

		private static string ResolvedEnumName(BidirectionalDictionary<string, string> map, string enumText)
		{
			string text;
			map.TryGetBySecond(enumText, out text);
			text = (text ?? enumText);
			return text;
		}

		private BidirectionalDictionary<string, string> GetEnumNameMap(Type t)
		{
			BidirectionalDictionary<string, string> bidirectionalDictionary;
			if (!this._enumMemberNamesPerType.TryGetValue(t, out bidirectionalDictionary))
			{
				lock (this._enumMemberNamesPerType)
				{
					if (this._enumMemberNamesPerType.TryGetValue(t, out bidirectionalDictionary))
					{
						return bidirectionalDictionary;
					}
					bidirectionalDictionary = new BidirectionalDictionary<string, string>(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
					FieldInfo[] fields = t.GetFields();
					for (int i = 0; i < fields.Length; i++)
					{
						FieldInfo fieldInfo = fields[i];
						string name = fieldInfo.Name;
						string text = (from EnumMemberAttribute a in fieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)
						select a.Value).SingleOrDefault<string>() ?? fieldInfo.Name;
						string text2;
						if (bidirectionalDictionary.TryGetBySecond(text, out text2))
						{
							throw new InvalidOperationException("Enum name '{0}' already exists on enum '{1}'.".FormatWith(CultureInfo.InvariantCulture, text, t.Name));
						}
						bidirectionalDictionary.Set(name, text);
					}
					this._enumMemberNamesPerType[t] = bidirectionalDictionary;
				}
				return bidirectionalDictionary;
			}
			return bidirectionalDictionary;
		}

		public override bool CanConvert(Type objectType)
		{
			Type type = ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
			return type.IsEnum();
		}
	}
}
