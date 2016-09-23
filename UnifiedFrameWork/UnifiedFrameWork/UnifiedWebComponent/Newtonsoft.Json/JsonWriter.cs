using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Newtonsoft.Json
{
	internal abstract class JsonWriter : IDisposable
	{
		internal enum State
		{
			Start,
			Property,
			ObjectStart,
			Object,
			ArrayStart,
			Array,
			ConstructorStart,
			Constructor,
			Closed,
			Error
		}

		private static readonly JsonWriter.State[][] StateArray;

		internal static readonly JsonWriter.State[][] StateArrayTempate;

		private readonly List<JsonPosition> _stack;

		private JsonPosition _currentPosition;

		private JsonWriter.State _currentState;

		private Formatting _formatting;

		private DateFormatHandling _dateFormatHandling;

		private DateTimeZoneHandling _dateTimeZoneHandling;

		private StringEscapeHandling _stringEscapeHandling;

		private FloatFormatHandling _floatFormatHandling;

		private string _dateFormatString;

		private CultureInfo _culture;

		public bool CloseOutput
		{
			get;
			set;
		}

		protected internal int Top
		{
			get
			{
				int num = this._stack.Count;
				if (this.Peek() != JsonContainerType.None)
				{
					num++;
				}
				return num;
			}
		}

		public WriteState WriteState
		{
			get
			{
				switch (this._currentState)
				{
				case JsonWriter.State.Start:
					return WriteState.Start;
				case JsonWriter.State.Property:
					return WriteState.Property;
				case JsonWriter.State.ObjectStart:
				case JsonWriter.State.Object:
					return WriteState.Object;
				case JsonWriter.State.ArrayStart:
				case JsonWriter.State.Array:
					return WriteState.Array;
				case JsonWriter.State.ConstructorStart:
				case JsonWriter.State.Constructor:
					return WriteState.Constructor;
				case JsonWriter.State.Closed:
					return WriteState.Closed;
				case JsonWriter.State.Error:
					return WriteState.Error;
				default:
					throw JsonWriterException.Create(this, "Invalid state: " + this._currentState, null);
				}
			}
		}

		internal string ContainerPath
		{
			get
			{
				if (this._currentPosition.Type == JsonContainerType.None)
				{
					return string.Empty;
				}
				return JsonPosition.BuildPath(this._stack);
			}
		}

		public string Path
		{
			get
			{
				if (this._currentPosition.Type == JsonContainerType.None)
				{
					return string.Empty;
				}
				bool flag = this._currentState != JsonWriter.State.ArrayStart && this._currentState != JsonWriter.State.ConstructorStart && this._currentState != JsonWriter.State.ObjectStart;
				IEnumerable<JsonPosition> positions = (!flag) ? this._stack : this._stack.Concat(new JsonPosition[]
				{
					this._currentPosition
				});
				return JsonPosition.BuildPath(positions);
			}
		}

		public Formatting Formatting
		{
			get
			{
				return this._formatting;
			}
			set
			{
				this._formatting = value;
			}
		}

		public DateFormatHandling DateFormatHandling
		{
			get
			{
				return this._dateFormatHandling;
			}
			set
			{
				this._dateFormatHandling = value;
			}
		}

		public DateTimeZoneHandling DateTimeZoneHandling
		{
			get
			{
				return this._dateTimeZoneHandling;
			}
			set
			{
				this._dateTimeZoneHandling = value;
			}
		}

		public StringEscapeHandling StringEscapeHandling
		{
			get
			{
				return this._stringEscapeHandling;
			}
			set
			{
				this._stringEscapeHandling = value;
				this.OnStringEscapeHandlingChanged();
			}
		}

		public FloatFormatHandling FloatFormatHandling
		{
			get
			{
				return this._floatFormatHandling;
			}
			set
			{
				this._floatFormatHandling = value;
			}
		}

		public string DateFormatString
		{
			get
			{
				return this._dateFormatString;
			}
			set
			{
				this._dateFormatString = value;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this._culture ?? CultureInfo.InvariantCulture;
			}
			set
			{
				this._culture = value;
			}
		}

		internal static JsonWriter.State[][] BuildStateArray()
		{
			List<JsonWriter.State[]> list = JsonWriter.StateArrayTempate.ToList<JsonWriter.State[]>();
			JsonWriter.State[] item = JsonWriter.StateArrayTempate[0];
			JsonWriter.State[] item2 = JsonWriter.StateArrayTempate[7];
			using (IEnumerator<object> enumerator = EnumUtils.GetValues(typeof(JsonToken)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					JsonToken jsonToken = (JsonToken)enumerator.Current;
					if (list.Count <= (int)jsonToken)
					{
						switch (jsonToken)
						{
						case JsonToken.Integer:
						case JsonToken.Float:
						case JsonToken.String:
						case JsonToken.Boolean:
						case JsonToken.Null:
						case JsonToken.Undefined:
						case JsonToken.Date:
						case JsonToken.Bytes:
							list.Add(item2);
							continue;
						}
						list.Add(item);
					}
				}
			}
			return list.ToArray();
		}

		static JsonWriter()
		{
			JsonWriter.StateArrayTempate = new JsonWriter.State[][]
			{
				new JsonWriter.State[]
				{
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error
				},
				new JsonWriter.State[]
				{
					JsonWriter.State.ObjectStart,
					JsonWriter.State.ObjectStart,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.ObjectStart,
					JsonWriter.State.ObjectStart,
					JsonWriter.State.ObjectStart,
					JsonWriter.State.ObjectStart,
					JsonWriter.State.Error,
					JsonWriter.State.Error
				},
				new JsonWriter.State[]
				{
					JsonWriter.State.ArrayStart,
					JsonWriter.State.ArrayStart,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.ArrayStart,
					JsonWriter.State.ArrayStart,
					JsonWriter.State.ArrayStart,
					JsonWriter.State.ArrayStart,
					JsonWriter.State.Error,
					JsonWriter.State.Error
				},
				new JsonWriter.State[]
				{
					JsonWriter.State.ConstructorStart,
					JsonWriter.State.ConstructorStart,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.ConstructorStart,
					JsonWriter.State.ConstructorStart,
					JsonWriter.State.ConstructorStart,
					JsonWriter.State.ConstructorStart,
					JsonWriter.State.Error,
					JsonWriter.State.Error
				},
				new JsonWriter.State[]
				{
					JsonWriter.State.Property,
					JsonWriter.State.Error,
					JsonWriter.State.Property,
					JsonWriter.State.Property,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Error
				},
				new JsonWriter.State[]
				{
					JsonWriter.State.Start,
					JsonWriter.State.Property,
					JsonWriter.State.ObjectStart,
					JsonWriter.State.Object,
					JsonWriter.State.ArrayStart,
					JsonWriter.State.Array,
					JsonWriter.State.Constructor,
					JsonWriter.State.Constructor,
					JsonWriter.State.Error,
					JsonWriter.State.Error
				},
				new JsonWriter.State[]
				{
					JsonWriter.State.Start,
					JsonWriter.State.Property,
					JsonWriter.State.ObjectStart,
					JsonWriter.State.Object,
					JsonWriter.State.ArrayStart,
					JsonWriter.State.Array,
					JsonWriter.State.Constructor,
					JsonWriter.State.Constructor,
					JsonWriter.State.Error,
					JsonWriter.State.Error
				},
				new JsonWriter.State[]
				{
					JsonWriter.State.Start,
					JsonWriter.State.Object,
					JsonWriter.State.Error,
					JsonWriter.State.Error,
					JsonWriter.State.Array,
					JsonWriter.State.Array,
					JsonWriter.State.Constructor,
					JsonWriter.State.Constructor,
					JsonWriter.State.Error,
					JsonWriter.State.Error
				}
			};
			JsonWriter.StateArray = JsonWriter.BuildStateArray();
		}

		internal virtual void OnStringEscapeHandlingChanged()
		{
		}

		protected JsonWriter()
		{
			this._stack = new List<JsonPosition>(4);
			this._currentState = JsonWriter.State.Start;
			this._formatting = Formatting.None;
			this._dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
			this.CloseOutput = true;
		}

		internal void UpdateScopeWithFinishedValue()
		{
			if (this._currentPosition.HasIndex)
			{
				this._currentPosition.Position = this._currentPosition.Position + 1;
			}
		}

		private void Push(JsonContainerType value)
		{
			if (this._currentPosition.Type != JsonContainerType.None)
			{
				this._stack.Add(this._currentPosition);
			}
			this._currentPosition = new JsonPosition(value);
		}

		private JsonContainerType Pop()
		{
			JsonPosition currentPosition = this._currentPosition;
			if (this._stack.Count > 0)
			{
				this._currentPosition = this._stack[this._stack.Count - 1];
				this._stack.RemoveAt(this._stack.Count - 1);
			}
			else
			{
				this._currentPosition = default(JsonPosition);
			}
			return currentPosition.Type;
		}

		private JsonContainerType Peek()
		{
			return this._currentPosition.Type;
		}

		public abstract void Flush();

		public virtual void Close()
		{
			this.AutoCompleteAll();
		}

		public virtual void WriteStartObject()
		{
			this.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
		}

		public virtual void WriteEndObject()
		{
			this.InternalWriteEnd(JsonContainerType.Object);
		}

		public virtual void WriteStartArray()
		{
			this.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
		}

		public virtual void WriteEndArray()
		{
			this.InternalWriteEnd(JsonContainerType.Array);
		}

		public virtual void WriteStartConstructor(string name)
		{
			this.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
		}

		public virtual void WriteEndConstructor()
		{
			this.InternalWriteEnd(JsonContainerType.Constructor);
		}

		public virtual void WritePropertyName(string name)
		{
			this.InternalWritePropertyName(name);
		}

		public virtual void WritePropertyName(string name, bool escape)
		{
			this.WritePropertyName(name);
		}

		public virtual void WriteEnd()
		{
			this.WriteEnd(this.Peek());
		}

		public void WriteToken(JsonReader reader)
		{
			this.WriteToken(reader, true, true);
		}

		public void WriteToken(JsonReader reader, bool writeChildren)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			this.WriteToken(reader, writeChildren, true);
		}

		internal void WriteToken(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate)
		{
			int initialDepth;
			if (reader.TokenType == JsonToken.None)
			{
				initialDepth = -1;
			}
			else if (!JsonWriter.IsStartToken(reader.TokenType))
			{
				initialDepth = reader.Depth + 1;
			}
			else
			{
				initialDepth = reader.Depth;
			}
			this.WriteToken(reader, initialDepth, writeChildren, writeDateConstructorAsDate);
		}

		internal void WriteToken(JsonReader reader, int initialDepth, bool writeChildren, bool writeDateConstructorAsDate)
		{
			while (true)
			{
				switch (reader.TokenType)
				{
				case JsonToken.None:
					goto IL_2AB;
				case JsonToken.StartObject:
					this.WriteStartObject();
					goto IL_2AB;
				case JsonToken.StartArray:
					this.WriteStartArray();
					goto IL_2AB;
				case JsonToken.StartConstructor:
				{
					string a = reader.Value.ToString();
					if (writeDateConstructorAsDate && string.Equals(a, "Date", StringComparison.Ordinal))
					{
						this.WriteConstructorDate(reader);
						goto IL_2AB;
					}
					this.WriteStartConstructor(reader.Value.ToString());
					goto IL_2AB;
				}
				case JsonToken.PropertyName:
					this.WritePropertyName(reader.Value.ToString());
					goto IL_2AB;
				case JsonToken.Comment:
					this.WriteComment((reader.Value != null) ? reader.Value.ToString() : null);
					goto IL_2AB;
				case JsonToken.Raw:
					this.WriteRawValue((reader.Value != null) ? reader.Value.ToString() : null);
					goto IL_2AB;
				case JsonToken.Integer:
					if (reader.Value is BigInteger)
					{
						this.WriteValue((BigInteger)reader.Value);
						goto IL_2AB;
					}
					this.WriteValue(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
					goto IL_2AB;
				case JsonToken.Float:
				{
					object value = reader.Value;
					if (value is decimal)
					{
						this.WriteValue((decimal)value);
						goto IL_2AB;
					}
					if (value is double)
					{
						this.WriteValue((double)value);
						goto IL_2AB;
					}
					if (value is float)
					{
						this.WriteValue((float)value);
						goto IL_2AB;
					}
					this.WriteValue(Convert.ToDouble(value, CultureInfo.InvariantCulture));
					goto IL_2AB;
				}
				case JsonToken.String:
					this.WriteValue(reader.Value.ToString());
					goto IL_2AB;
				case JsonToken.Boolean:
					this.WriteValue(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
					goto IL_2AB;
				case JsonToken.Null:
					this.WriteNull();
					goto IL_2AB;
				case JsonToken.Undefined:
					this.WriteUndefined();
					goto IL_2AB;
				case JsonToken.EndObject:
					this.WriteEndObject();
					goto IL_2AB;
				case JsonToken.EndArray:
					this.WriteEndArray();
					goto IL_2AB;
				case JsonToken.EndConstructor:
					this.WriteEndConstructor();
					goto IL_2AB;
				case JsonToken.Date:
					if (reader.Value is DateTimeOffset)
					{
						this.WriteValue((DateTimeOffset)reader.Value);
						goto IL_2AB;
					}
					this.WriteValue(Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture));
					goto IL_2AB;
				case JsonToken.Bytes:
					this.WriteValue((byte[])reader.Value);
					goto IL_2AB;
				}
				break;
				IL_2AB:
				if (initialDepth - 1 >= reader.Depth - (JsonWriter.IsEndToken(reader.TokenType) ? 1 : 0) || !writeChildren || !reader.Read())
				{
					return;
				}
			}
			throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", reader.TokenType, "Unexpected token type.");
		}

		private void WriteConstructorDate(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
			}
			if (reader.TokenType != JsonToken.Integer)
			{
				throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected Integer, got " + reader.TokenType, null);
			}
			long javaScriptTicks = (long)reader.Value;
			DateTime value = DateTimeUtils.ConvertJavaScriptTicksToDateTime(javaScriptTicks);
			if (!reader.Read())
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
			}
			if (reader.TokenType != JsonToken.EndConstructor)
			{
				throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected EndConstructor, got " + reader.TokenType, null);
			}
			this.WriteValue(value);
		}

		internal static bool IsEndToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.EndObject:
			case JsonToken.EndArray:
			case JsonToken.EndConstructor:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsStartToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.StartObject:
			case JsonToken.StartArray:
			case JsonToken.StartConstructor:
				return true;
			default:
				return false;
			}
		}

		private void WriteEnd(JsonContainerType type)
		{
			switch (type)
			{
			case JsonContainerType.Object:
				this.WriteEndObject();
				return;
			case JsonContainerType.Array:
				this.WriteEndArray();
				return;
			case JsonContainerType.Constructor:
				this.WriteEndConstructor();
				return;
			default:
				throw JsonWriterException.Create(this, "Unexpected type when writing end: " + type, null);
			}
		}

		private void AutoCompleteAll()
		{
			while (this.Top > 0)
			{
				this.WriteEnd();
			}
		}

		private JsonToken GetCloseTokenForType(JsonContainerType type)
		{
			switch (type)
			{
			case JsonContainerType.Object:
				return JsonToken.EndObject;
			case JsonContainerType.Array:
				return JsonToken.EndArray;
			case JsonContainerType.Constructor:
				return JsonToken.EndConstructor;
			default:
				throw JsonWriterException.Create(this, "No close token for type: " + type, null);
			}
		}

		private void AutoCompleteClose(JsonContainerType type)
		{
			int num = 0;
			if (this._currentPosition.Type == type)
			{
				num = 1;
			}
			else
			{
				int num2 = this.Top - 2;
				for (int i = num2; i >= 0; i--)
				{
					int index = num2 - i;
					if (this._stack[index].Type == type)
					{
						num = i + 2;
						break;
					}
				}
			}
			if (num == 0)
			{
				throw JsonWriterException.Create(this, "No token to close.", null);
			}
			for (int j = 0; j < num; j++)
			{
				JsonToken closeTokenForType = this.GetCloseTokenForType(this.Pop());
				if (this._currentState == JsonWriter.State.Property)
				{
					this.WriteNull();
				}
				if (this._formatting == Formatting.Indented && this._currentState != JsonWriter.State.ObjectStart && this._currentState != JsonWriter.State.ArrayStart)
				{
					this.WriteIndent();
				}
				this.WriteEnd(closeTokenForType);
				JsonContainerType jsonContainerType = this.Peek();
				switch (jsonContainerType)
				{
				case JsonContainerType.None:
					this._currentState = JsonWriter.State.Start;
					break;
				case JsonContainerType.Object:
					this._currentState = JsonWriter.State.Object;
					break;
				case JsonContainerType.Array:
					this._currentState = JsonWriter.State.Array;
					break;
				case JsonContainerType.Constructor:
					this._currentState = JsonWriter.State.Array;
					break;
				default:
					throw JsonWriterException.Create(this, "Unknown JsonType: " + jsonContainerType, null);
				}
			}
		}

		protected virtual void WriteEnd(JsonToken token)
		{
		}

		protected virtual void WriteIndent()
		{
		}

		protected virtual void WriteValueDelimiter()
		{
		}

		protected virtual void WriteIndentSpace()
		{
		}

		internal void AutoComplete(JsonToken tokenBeingWritten)
		{
			JsonWriter.State state = JsonWriter.StateArray[(int)tokenBeingWritten][(int)this._currentState];
			if (state == JsonWriter.State.Error)
			{
				throw JsonWriterException.Create(this, "Token {0} in state {1} would result in an invalid JSON object.".FormatWith(CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), this._currentState.ToString()), null);
			}
			if ((this._currentState == JsonWriter.State.Object || this._currentState == JsonWriter.State.Array || this._currentState == JsonWriter.State.Constructor) && tokenBeingWritten != JsonToken.Comment)
			{
				this.WriteValueDelimiter();
			}
			if (this._formatting == Formatting.Indented)
			{
				if (this._currentState == JsonWriter.State.Property)
				{
					this.WriteIndentSpace();
				}
				if (this._currentState == JsonWriter.State.Array || this._currentState == JsonWriter.State.ArrayStart || this._currentState == JsonWriter.State.Constructor || this._currentState == JsonWriter.State.ConstructorStart || (tokenBeingWritten == JsonToken.PropertyName && this._currentState != JsonWriter.State.Start))
				{
					this.WriteIndent();
				}
			}
			this._currentState = state;
		}

		public virtual void WriteNull()
		{
			this.InternalWriteValue(JsonToken.Null);
		}

		public virtual void WriteUndefined()
		{
			this.InternalWriteValue(JsonToken.Undefined);
		}

		public virtual void WriteRaw(string json)
		{
			this.InternalWriteRaw();
		}

		public virtual void WriteRawValue(string json)
		{
			this.UpdateScopeWithFinishedValue();
			this.AutoComplete(JsonToken.Undefined);
			this.WriteRaw(json);
		}

		public virtual void WriteValue(string value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(int value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(uint value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		public virtual void WriteValue(long value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ulong value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		public virtual void WriteValue(float value)
		{
			this.InternalWriteValue(JsonToken.Float);
		}

		public virtual void WriteValue(double value)
		{
			this.InternalWriteValue(JsonToken.Float);
		}

		public virtual void WriteValue(bool value)
		{
			this.InternalWriteValue(JsonToken.Boolean);
		}

		public virtual void WriteValue(short value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ushort value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		public virtual void WriteValue(char value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(byte value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(sbyte value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		public virtual void WriteValue(decimal value)
		{
			this.InternalWriteValue(JsonToken.Float);
		}

		public virtual void WriteValue(DateTime value)
		{
			this.InternalWriteValue(JsonToken.Date);
		}

		public virtual void WriteValue(DateTimeOffset value)
		{
			this.InternalWriteValue(JsonToken.Date);
		}

		public virtual void WriteValue(Guid value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(TimeSpan value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(int? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(uint? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(long? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ulong? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(float? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(double? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(bool? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(short? value)
		{
			short? num = value;
			if (!(num.HasValue ? new int?((int)num.GetValueOrDefault()) : null).HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ushort? value)
		{
			ushort? num = value;
			if (!(num.HasValue ? new int?((int)num.GetValueOrDefault()) : null).HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(char? value)
		{
			char? c = value;
			if (!(c.HasValue ? new int?((int)c.GetValueOrDefault()) : null).HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(byte? value)
		{
			byte? b = value;
			if (!(b.HasValue ? new int?((int)b.GetValueOrDefault()) : null).HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(sbyte? value)
		{
			sbyte? b = value;
			if (!(b.HasValue ? new int?((int)b.GetValueOrDefault()) : null).HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(decimal? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(DateTime? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(DateTimeOffset? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(Guid? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(TimeSpan? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.Value);
		}

		public virtual void WriteValue(byte[] value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.InternalWriteValue(JsonToken.Bytes);
		}

		public virtual void WriteValue(Uri value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			if (value is BigInteger)
			{
				throw JsonWriter.CreateUnsupportedTypeException(this, value);
			}
			JsonWriter.WriteValue(this, ConvertUtils.GetTypeCode(value), value);
		}

		public virtual void WriteComment(string text)
		{
			this.InternalWriteComment();
		}

		public virtual void WriteWhitespace(string ws)
		{
			this.InternalWriteWhitespace(ws);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (this._currentState != JsonWriter.State.Closed)
			{
				this.Close();
			}
		}

		internal static void WriteValue(JsonWriter writer, PrimitiveTypeCode typeCode, object value)
		{
			switch (typeCode)
			{
			case PrimitiveTypeCode.Char:
				writer.WriteValue((char)value);
				return;
			case PrimitiveTypeCode.CharNullable:
				writer.WriteValue((value == null) ? null : new char?((char)value));
				return;
			case PrimitiveTypeCode.Boolean:
				writer.WriteValue((bool)value);
				return;
			case PrimitiveTypeCode.BooleanNullable:
				writer.WriteValue((value == null) ? null : new bool?((bool)value));
				return;
			case PrimitiveTypeCode.SByte:
				writer.WriteValue((sbyte)value);
				return;
			case PrimitiveTypeCode.SByteNullable:
				writer.WriteValue((value == null) ? null : new sbyte?((sbyte)value));
				return;
			case PrimitiveTypeCode.Int16:
				writer.WriteValue((short)value);
				return;
			case PrimitiveTypeCode.Int16Nullable:
				writer.WriteValue((value == null) ? null : new short?((short)value));
				return;
			case PrimitiveTypeCode.UInt16:
				writer.WriteValue((ushort)value);
				return;
			case PrimitiveTypeCode.UInt16Nullable:
				writer.WriteValue((value == null) ? null : new ushort?((ushort)value));
				return;
			case PrimitiveTypeCode.Int32:
				writer.WriteValue((int)value);
				return;
			case PrimitiveTypeCode.Int32Nullable:
				writer.WriteValue((value == null) ? null : new int?((int)value));
				return;
			case PrimitiveTypeCode.Byte:
				writer.WriteValue((byte)value);
				return;
			case PrimitiveTypeCode.ByteNullable:
				writer.WriteValue((value == null) ? null : new byte?((byte)value));
				return;
			case PrimitiveTypeCode.UInt32:
				writer.WriteValue((uint)value);
				return;
			case PrimitiveTypeCode.UInt32Nullable:
				writer.WriteValue((value == null) ? null : new uint?((uint)value));
				return;
			case PrimitiveTypeCode.Int64:
				writer.WriteValue((long)value);
				return;
			case PrimitiveTypeCode.Int64Nullable:
				writer.WriteValue((value == null) ? null : new long?((long)value));
				return;
			case PrimitiveTypeCode.UInt64:
				writer.WriteValue((ulong)value);
				return;
			case PrimitiveTypeCode.UInt64Nullable:
				writer.WriteValue((value == null) ? null : new ulong?((ulong)value));
				return;
			case PrimitiveTypeCode.Single:
				writer.WriteValue((float)value);
				return;
			case PrimitiveTypeCode.SingleNullable:
				writer.WriteValue((value == null) ? null : new float?((float)value));
				return;
			case PrimitiveTypeCode.Double:
				writer.WriteValue((double)value);
				return;
			case PrimitiveTypeCode.DoubleNullable:
				writer.WriteValue((value == null) ? null : new double?((double)value));
				return;
			case PrimitiveTypeCode.DateTime:
				writer.WriteValue((DateTime)value);
				return;
			case PrimitiveTypeCode.DateTimeNullable:
				writer.WriteValue((value == null) ? null : new DateTime?((DateTime)value));
				return;
			case PrimitiveTypeCode.DateTimeOffset:
				writer.WriteValue((DateTimeOffset)value);
				return;
			case PrimitiveTypeCode.DateTimeOffsetNullable:
				writer.WriteValue((value == null) ? null : new DateTimeOffset?((DateTimeOffset)value));
				return;
			case PrimitiveTypeCode.Decimal:
				writer.WriteValue((decimal)value);
				return;
			case PrimitiveTypeCode.DecimalNullable:
				writer.WriteValue((value == null) ? null : new decimal?((decimal)value));
				return;
			case PrimitiveTypeCode.Guid:
				writer.WriteValue((Guid)value);
				return;
			case PrimitiveTypeCode.GuidNullable:
				writer.WriteValue((value == null) ? null : new Guid?((Guid)value));
				return;
			case PrimitiveTypeCode.TimeSpan:
				writer.WriteValue((TimeSpan)value);
				return;
			case PrimitiveTypeCode.TimeSpanNullable:
				writer.WriteValue((value == null) ? null : new TimeSpan?((TimeSpan)value));
				return;
			case PrimitiveTypeCode.BigInteger:
				writer.WriteValue((BigInteger)value);
				return;
			case PrimitiveTypeCode.BigIntegerNullable:
				writer.WriteValue((value == null) ? null : new BigInteger?((BigInteger)value));
				return;
			case PrimitiveTypeCode.Uri:
				writer.WriteValue((Uri)value);
				return;
			case PrimitiveTypeCode.String:
				writer.WriteValue((string)value);
				return;
			case PrimitiveTypeCode.Bytes:
				writer.WriteValue((byte[])value);
				return;
			case PrimitiveTypeCode.DBNull:
				writer.WriteNull();
				return;
			default:
				if (value is IConvertible)
				{
					IConvertible convertible = (IConvertible)value;
					TypeInformation typeInformation = ConvertUtils.GetTypeInformation(convertible);
					PrimitiveTypeCode typeCode2 = (typeInformation.TypeCode == PrimitiveTypeCode.Object) ? PrimitiveTypeCode.String : typeInformation.TypeCode;
					Type conversionType = (typeInformation.TypeCode == PrimitiveTypeCode.Object) ? typeof(string) : typeInformation.Type;
					object value2 = convertible.ToType(conversionType, CultureInfo.InvariantCulture);
					JsonWriter.WriteValue(writer, typeCode2, value2);
					return;
				}
				throw JsonWriter.CreateUnsupportedTypeException(writer, value);
			}
		}

		private static JsonWriterException CreateUnsupportedTypeException(JsonWriter writer, object value)
		{
			return JsonWriterException.Create(writer, "Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()), null);
		}

		protected void SetWriteState(JsonToken token, object value)
		{
			switch (token)
			{
			case JsonToken.StartObject:
				this.InternalWriteStart(token, JsonContainerType.Object);
				return;
			case JsonToken.StartArray:
				this.InternalWriteStart(token, JsonContainerType.Array);
				return;
			case JsonToken.StartConstructor:
				this.InternalWriteStart(token, JsonContainerType.Constructor);
				return;
			case JsonToken.PropertyName:
				if (!(value is string))
				{
					throw new ArgumentException("A name is required when setting property name state.", "value");
				}
				this.InternalWritePropertyName((string)value);
				return;
			case JsonToken.Comment:
				this.InternalWriteComment();
				return;
			case JsonToken.Raw:
				this.InternalWriteRaw();
				return;
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				this.InternalWriteValue(token);
				return;
			case JsonToken.EndObject:
				this.InternalWriteEnd(JsonContainerType.Object);
				return;
			case JsonToken.EndArray:
				this.InternalWriteEnd(JsonContainerType.Array);
				return;
			case JsonToken.EndConstructor:
				this.InternalWriteEnd(JsonContainerType.Constructor);
				return;
			default:
				throw new ArgumentOutOfRangeException("token");
			}
		}

		internal void InternalWriteEnd(JsonContainerType container)
		{
			this.AutoCompleteClose(container);
		}

		internal void InternalWritePropertyName(string name)
		{
			this._currentPosition.PropertyName = name;
			this.AutoComplete(JsonToken.PropertyName);
		}

		internal void InternalWriteRaw()
		{
		}

		internal void InternalWriteStart(JsonToken token, JsonContainerType container)
		{
			this.UpdateScopeWithFinishedValue();
			this.AutoComplete(token);
			this.Push(container);
		}

		internal void InternalWriteValue(JsonToken token)
		{
			this.UpdateScopeWithFinishedValue();
			this.AutoComplete(token);
		}

		internal void InternalWriteWhitespace(string ws)
		{
			if (ws != null && !StringUtils.IsWhiteSpace(ws))
			{
				throw JsonWriterException.Create(this, "Only white space characters should be used.", null);
			}
		}

		internal void InternalWriteComment()
		{
			this.AutoComplete(JsonToken.Comment);
		}
	}
}
