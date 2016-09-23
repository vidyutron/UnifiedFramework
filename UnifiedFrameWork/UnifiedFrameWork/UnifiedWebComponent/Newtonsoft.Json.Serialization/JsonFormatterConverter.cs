using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonFormatterConverter : IFormatterConverter
	{
		private readonly JsonSerializer _serializer;

		public JsonFormatterConverter(JsonSerializer serializer)
		{
			ValidationUtils.ArgumentNotNull(serializer, "serializer");
			this._serializer = serializer;
		}

		private T GetTokenValue<T>(object value)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			JValue jValue = (JValue)value;
			return (T)((object)System.Convert.ChangeType(jValue.Value, typeof(T), CultureInfo.InvariantCulture));
		}

		public object Convert(object value, Type type)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			JToken jToken = value as JToken;
			if (jToken == null)
			{
				throw new ArgumentException("Value is not a JToken.", "value");
			}
			return this._serializer.Deserialize(jToken.CreateReader(), type);
		}

		public object Convert(object value, TypeCode typeCode)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			if (value is JValue)
			{
				value = ((JValue)value).Value;
			}
			return System.Convert.ChangeType(value, typeCode, CultureInfo.InvariantCulture);
		}

		public bool ToBoolean(object value)
		{
			return this.GetTokenValue<bool>(value);
		}

		public byte ToByte(object value)
		{
			return this.GetTokenValue<byte>(value);
		}

		public char ToChar(object value)
		{
			return this.GetTokenValue<char>(value);
		}

		public DateTime ToDateTime(object value)
		{
			return this.GetTokenValue<DateTime>(value);
		}

		public decimal ToDecimal(object value)
		{
			return this.GetTokenValue<decimal>(value);
		}

		public double ToDouble(object value)
		{
			return this.GetTokenValue<double>(value);
		}

		public short ToInt16(object value)
		{
			return this.GetTokenValue<short>(value);
		}

		public int ToInt32(object value)
		{
			return this.GetTokenValue<int>(value);
		}

		public long ToInt64(object value)
		{
			return this.GetTokenValue<long>(value);
		}

		public sbyte ToSByte(object value)
		{
			return this.GetTokenValue<sbyte>(value);
		}

		public float ToSingle(object value)
		{
			return this.GetTokenValue<float>(value);
		}

		public string ToString(object value)
		{
			return this.GetTokenValue<string>(value);
		}

		public ushort ToUInt16(object value)
		{
			return this.GetTokenValue<ushort>(value);
		}

		public uint ToUInt32(object value)
		{
			return this.GetTokenValue<uint>(value);
		}

		public ulong ToUInt64(object value)
		{
			return this.GetTokenValue<ulong>(value);
		}
	}
}
