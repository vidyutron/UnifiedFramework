using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Newtonsoft.Json.Converters
{
	internal class DiscriminatedUnionConverter : JsonConverter
	{
		private const string CasePropertyName = "Case";

		private const string FieldsPropertyName = "Fields";

		private static bool _initialized;

		private static MethodCall<object, object> _isUnion;

		private static MethodCall<object, object> _getUnionFields;

		private static MethodCall<object, object> _getUnionCases;

		private static MethodCall<object, object> _makeUnion;

		private static Func<object, object> _getUnionCaseInfoName;

		private static Func<object, object> _getUnionCaseInfo;

		private static Func<object, object> _getUnionCaseFields;

		private static MethodCall<object, object> _getUnionCaseInfoFields;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
			Type type = value.GetType();
			MethodCall<object, object> arg_29_0 = DiscriminatedUnionConverter._getUnionFields;
			object arg_29_1 = null;
			object[] array = new object[3];
			array[0] = value;
			array[1] = type;
			object arg = arg_29_0(arg_29_1, array);
			object arg2 = DiscriminatedUnionConverter._getUnionCaseInfo(arg);
			object value2 = DiscriminatedUnionConverter._getUnionCaseFields(arg);
			object obj = DiscriminatedUnionConverter._getUnionCaseInfoName(arg2);
			writer.WriteStartObject();
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Case") : "Case");
			writer.WriteValue((string)obj);
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Fields") : "Fields");
			serializer.Serialize(writer, value2);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			MethodCall<object, object> arg_21_0 = DiscriminatedUnionConverter._getUnionCases;
			object arg_21_1 = null;
			object[] array = new object[2];
			array[0] = objectType;
			IEnumerable enumerable = (IEnumerable)arg_21_0(arg_21_1, array);
			DiscriminatedUnionConverter.ReadAndAssertProperty(reader, "Case");
			DiscriminatedUnionConverter.ReadAndAssert(reader);
			string text = reader.Value.ToString();
			object obj = null;
			foreach (object current in enumerable)
			{
				if ((string)DiscriminatedUnionConverter._getUnionCaseInfoName(current) == text)
				{
					obj = current;
					break;
				}
			}
			if (obj == null)
			{
				throw new JsonSerializationException("No union type found with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture, text));
			}
			DiscriminatedUnionConverter.ReadAndAssertProperty(reader, "Fields");
			DiscriminatedUnionConverter.ReadAndAssert(reader);
			DiscriminatedUnionConverter.ReadAndAssert(reader);
			PropertyInfo[] array2 = (PropertyInfo[])DiscriminatedUnionConverter._getUnionCaseInfoFields(obj, new object[0]);
			List<object> list = new List<object>();
			PropertyInfo[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				PropertyInfo propertyInfo = array3[i];
				list.Add(serializer.Deserialize(reader, propertyInfo.PropertyType));
				DiscriminatedUnionConverter.ReadAndAssert(reader);
			}
			DiscriminatedUnionConverter.ReadAndAssert(reader);
			MethodCall<object, object> arg_164_0 = DiscriminatedUnionConverter._makeUnion;
			object arg_164_1 = null;
			object[] array4 = new object[3];
			array4[0] = obj;
			array4[1] = list.ToArray();
			return arg_164_0(arg_164_1, array4);
		}

		public override bool CanConvert(Type objectType)
		{
			if (typeof(IEnumerable).IsAssignableFrom(objectType))
			{
				return false;
			}
			object[] customAttributes = objectType.GetCustomAttributes(true);
			bool flag = false;
			object[] array = customAttributes;
			for (int i = 0; i < array.Length; i++)
			{
				object obj = array[i];
				Type type = obj.GetType();
				if (type.Name == "CompilationMappingAttribute")
				{
					DiscriminatedUnionConverter.EnsureInitialized(type);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			MethodCall<object, object> arg_83_0 = DiscriminatedUnionConverter._isUnion;
			object arg_83_1 = null;
			object[] array2 = new object[2];
			array2[0] = objectType;
			return (bool)arg_83_0(arg_83_1, array2);
		}

		private static void EnsureInitialized(Type attributeType)
		{
			if (!DiscriminatedUnionConverter._initialized)
			{
				DiscriminatedUnionConverter._initialized = true;
				Assembly assembly = attributeType.Assembly();
				Type type = assembly.GetType("Microsoft.FSharp.Reflection.FSharpType");
				MethodInfo method = type.GetMethod("IsUnion", BindingFlags.Static | BindingFlags.Public);
				DiscriminatedUnionConverter._isUnion = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
				MethodInfo method2 = type.GetMethod("GetUnionCases", BindingFlags.Static | BindingFlags.Public);
				DiscriminatedUnionConverter._getUnionCases = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method2);
				Type type2 = assembly.GetType("Microsoft.FSharp.Reflection.FSharpValue");
				MethodInfo method3 = type2.GetMethod("GetUnionFields", BindingFlags.Static | BindingFlags.Public);
				DiscriminatedUnionConverter._getUnionFields = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method3);
				DiscriminatedUnionConverter._getUnionCaseInfo = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(method3.ReturnType.GetProperty("Item1"));
				DiscriminatedUnionConverter._getUnionCaseFields = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(method3.ReturnType.GetProperty("Item2"));
				MethodInfo method4 = type2.GetMethod("MakeUnion", BindingFlags.Static | BindingFlags.Public);
				DiscriminatedUnionConverter._makeUnion = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method4);
				Type type3 = assembly.GetType("Microsoft.FSharp.Reflection.UnionCaseInfo");
				DiscriminatedUnionConverter._getUnionCaseInfoName = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type3.GetProperty("Name"));
				DiscriminatedUnionConverter._getUnionCaseInfoFields = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(type3.GetMethod("GetFields"));
			}
		}

		private static void ReadAndAssertProperty(JsonReader reader, string propertyName)
		{
			DiscriminatedUnionConverter.ReadAndAssert(reader);
			if (reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value.ToString(), propertyName, StringComparison.OrdinalIgnoreCase))
			{
				throw new JsonSerializationException("Expected JSON property '{0}'.".FormatWith(CultureInfo.InvariantCulture, propertyName));
			}
		}

		private static void ReadAndAssert(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw new JsonSerializationException("Unexpected end.");
			}
		}
	}
}
