using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Newtonsoft.Json.Converters
{
	internal class DataTableConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			DataTable dataTable = (DataTable)value;
			DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
			writer.WriteStartArray();
			foreach (DataRow dataRow in dataTable.Rows)
			{
				writer.WriteStartObject();
				foreach (DataColumn dataColumn in dataRow.Table.Columns)
				{
					if (serializer.NullValueHandling != NullValueHandling.Ignore || (dataRow[dataColumn] != null && dataRow[dataColumn] != DBNull.Value))
					{
						writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName(dataColumn.ColumnName) : dataColumn.ColumnName);
						serializer.Serialize(writer, dataRow[dataColumn]);
					}
				}
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			DataTable dataTable = existingValue as DataTable;
			if (dataTable == null)
			{
				dataTable = ((objectType == typeof(DataTable)) ? new DataTable() : ((DataTable)Activator.CreateInstance(objectType)));
			}
			if (reader.TokenType == JsonToken.PropertyName)
			{
				dataTable.TableName = (string)reader.Value;
				reader.Read();
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				reader.Read();
			}
			while (reader.TokenType != JsonToken.EndArray)
			{
				DataTableConverter.CreateRow(reader, dataTable);
				reader.Read();
			}
			return dataTable;
		}

		private static void CreateRow(JsonReader reader, DataTable dt)
		{
			DataRow dataRow = dt.NewRow();
			reader.Read();
			while (reader.TokenType == JsonToken.PropertyName)
			{
				string text = (string)reader.Value;
				reader.Read();
				DataColumn dataColumn = dt.Columns[text];
				if (dataColumn == null)
				{
					Type columnDataType = DataTableConverter.GetColumnDataType(reader);
					dataColumn = new DataColumn(text, columnDataType);
					dt.Columns.Add(dataColumn);
				}
				if (dataColumn.DataType == typeof(DataTable))
				{
					if (reader.TokenType == JsonToken.StartArray)
					{
						reader.Read();
					}
					DataTable dataTable = new DataTable();
					while (reader.TokenType != JsonToken.EndArray)
					{
						DataTableConverter.CreateRow(reader, dataTable);
						reader.Read();
					}
					dataRow[text] = dataTable;
				}
				else if (dataColumn.DataType.IsArray)
				{
					if (reader.TokenType == JsonToken.StartArray)
					{
						reader.Read();
					}
					List<object> list = new List<object>();
					while (reader.TokenType != JsonToken.EndArray)
					{
						list.Add(reader.Value);
						reader.Read();
					}
					Array array = Array.CreateInstance(dataColumn.DataType.GetElementType(), list.Count);
					Array.Copy(list.ToArray(), array, list.Count);
					dataRow[text] = array;
				}
				else
				{
					dataRow[text] = (reader.Value ?? DBNull.Value);
				}
				reader.Read();
			}
			dataRow.EndEdit();
			dt.Rows.Add(dataRow);
		}

		private static Type GetColumnDataType(JsonReader reader)
		{
			JsonToken tokenType = reader.TokenType;
			switch (tokenType)
			{
			case JsonToken.StartArray:
			{
				reader.Read();
				if (reader.TokenType == JsonToken.StartObject)
				{
					return typeof(DataTable);
				}
				Type columnDataType = DataTableConverter.GetColumnDataType(reader);
				return columnDataType.MakeArrayType();
			}
			case JsonToken.Integer:
				return typeof(long);
			case JsonToken.Float:
				return typeof(double);
			case JsonToken.String:
			case JsonToken.Null:
			case JsonToken.Undefined:
				return typeof(string);
			case JsonToken.Boolean:
				return typeof(bool);
			case JsonToken.Date:
				return typeof(DateTime);
			}
			throw new JsonException("Unexpected JSON token while reading DataTable: {0}".FormatWith(CultureInfo.InvariantCulture, tokenType));
		}

		public override bool CanConvert(Type valueType)
		{
			return typeof(DataTable).IsAssignableFrom(valueType);
		}
	}
}
