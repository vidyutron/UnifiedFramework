using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json
{
	internal abstract class JsonReader : IDisposable
	{
		protected internal enum State
		{
			Start,
			Complete,
			Property,
			ObjectStart,
			Object,
			ArrayStart,
			Array,
			Closed,
			PostValue,
			ConstructorStart,
			Constructor,
			Error,
			Finished
		}

		private JsonToken _tokenType;

		private object _value;

		internal char _quoteChar;

		internal JsonReader.State _currentState;

		internal ReadType _readType;

		private JsonPosition _currentPosition;

		private CultureInfo _culture;

		private DateTimeZoneHandling _dateTimeZoneHandling;

		private int? _maxDepth;

		private bool _hasExceededMaxDepth;

		internal DateParseHandling _dateParseHandling;

		internal FloatParseHandling _floatParseHandling;

		private readonly List<JsonPosition> _stack;

		protected JsonReader.State CurrentState
		{
			get
			{
				return this._currentState;
			}
		}

		public bool CloseInput
		{
			get;
			set;
		}

		public bool SupportMultipleContent
		{
			get;
			set;
		}

		public virtual char QuoteChar
		{
			get
			{
				return this._quoteChar;
			}
			protected internal set
			{
				this._quoteChar = value;
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

		public DateParseHandling DateParseHandling
		{
			get
			{
				return this._dateParseHandling;
			}
			set
			{
				this._dateParseHandling = value;
			}
		}

		public FloatParseHandling FloatParseHandling
		{
			get
			{
				return this._floatParseHandling;
			}
			set
			{
				this._floatParseHandling = value;
			}
		}

		public int? MaxDepth
		{
			get
			{
				return this._maxDepth;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("Value must be positive.", "value");
				}
				this._maxDepth = value;
			}
		}

		public virtual JsonToken TokenType
		{
			get
			{
				return this._tokenType;
			}
		}

		public virtual object Value
		{
			get
			{
				return this._value;
			}
		}

		public virtual Type ValueType
		{
			get
			{
				if (this._value == null)
				{
					return null;
				}
				return this._value.GetType();
			}
		}

		public virtual int Depth
		{
			get
			{
				int count = this._stack.Count;
				if (JsonReader.IsStartToken(this.TokenType) || this._currentPosition.Type == JsonContainerType.None)
				{
					return count;
				}
				return count + 1;
			}
		}

		public virtual string Path
		{
			get
			{
				if (this._currentPosition.Type == JsonContainerType.None)
				{
					return string.Empty;
				}
				bool flag = this._currentState != JsonReader.State.ArrayStart && this._currentState != JsonReader.State.ConstructorStart && this._currentState != JsonReader.State.ObjectStart;
				IEnumerable<JsonPosition> positions = (!flag) ? this._stack : this._stack.Concat(new JsonPosition[]
				{
					this._currentPosition
				});
				return JsonPosition.BuildPath(positions);
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

		internal JsonPosition GetPosition(int depth)
		{
			if (depth < this._stack.Count)
			{
				return this._stack[depth];
			}
			return this._currentPosition;
		}

		protected JsonReader()
		{
			this._currentState = JsonReader.State.Start;
			this._stack = new List<JsonPosition>(4);
			this._dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
			this._dateParseHandling = DateParseHandling.DateTime;
			this._floatParseHandling = FloatParseHandling.Double;
			this.CloseInput = true;
		}

		private void Push(JsonContainerType value)
		{
			this.UpdateScopeWithFinishedValue();
			if (this._currentPosition.Type == JsonContainerType.None)
			{
				this._currentPosition = new JsonPosition(value);
				return;
			}
			this._stack.Add(this._currentPosition);
			this._currentPosition = new JsonPosition(value);
			if (this._maxDepth.HasValue && this.Depth + 1 > this._maxDepth && !this._hasExceededMaxDepth)
			{
				this._hasExceededMaxDepth = true;
				throw JsonReaderException.Create(this, "The reader's MaxDepth of {0} has been exceeded.".FormatWith(CultureInfo.InvariantCulture, this._maxDepth));
			}
		}

		private JsonContainerType Pop()
		{
			JsonPosition currentPosition;
			if (this._stack.Count > 0)
			{
				currentPosition = this._currentPosition;
				this._currentPosition = this._stack[this._stack.Count - 1];
				this._stack.RemoveAt(this._stack.Count - 1);
			}
			else
			{
				currentPosition = this._currentPosition;
				this._currentPosition = default(JsonPosition);
			}
			if (this._maxDepth.HasValue && this.Depth <= this._maxDepth)
			{
				this._hasExceededMaxDepth = false;
			}
			return currentPosition.Type;
		}

		private JsonContainerType Peek()
		{
			return this._currentPosition.Type;
		}

		public abstract bool Read();

		public abstract int? ReadAsInt32();

		public abstract string ReadAsString();

		public abstract byte[] ReadAsBytes();

		public abstract decimal? ReadAsDecimal();

		public abstract DateTime? ReadAsDateTime();

		public abstract DateTimeOffset? ReadAsDateTimeOffset();

		internal virtual bool ReadInternal()
		{
			throw new NotImplementedException();
		}

		internal DateTimeOffset? ReadAsDateTimeOffsetInternal()
		{
			this._readType = ReadType.ReadAsDateTimeOffset;
			while (this.ReadInternal())
			{
				JsonToken tokenType = this.TokenType;
				if (tokenType != JsonToken.Comment)
				{
					if (tokenType == JsonToken.Date)
					{
						if (this.Value is DateTime)
						{
							this.SetToken(JsonToken.Date, new DateTimeOffset((DateTime)this.Value));
						}
						return new DateTimeOffset?((DateTimeOffset)this.Value);
					}
					if (tokenType == JsonToken.Null)
					{
						return null;
					}
					if (tokenType == JsonToken.String)
					{
						string text = (string)this.Value;
						if (string.IsNullOrEmpty(text))
						{
							this.SetToken(JsonToken.Null);
							return null;
						}
						DateTimeOffset dateTimeOffset;
						if (DateTimeOffset.TryParse(text, this.Culture, DateTimeStyles.RoundtripKind, out dateTimeOffset))
						{
							this.SetToken(JsonToken.Date, dateTimeOffset);
							return new DateTimeOffset?(dateTimeOffset);
						}
						throw JsonReaderException.Create(this, "Could not convert string to DateTimeOffset: {0}.".FormatWith(CultureInfo.InvariantCulture, this.Value));
					}
					else
					{
						if (tokenType == JsonToken.EndArray)
						{
							return null;
						}
						throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, tokenType));
					}
				}
			}
			this.SetToken(JsonToken.None);
			return null;
		}

		internal byte[] ReadAsBytesInternal()
		{
			this._readType = ReadType.ReadAsBytes;
			while (this.ReadInternal())
			{
				JsonToken tokenType = this.TokenType;
				if (tokenType != JsonToken.Comment)
				{
					if (this.IsWrappedInTypeObject())
					{
						byte[] array = this.ReadAsBytes();
						this.ReadInternal();
						this.SetToken(JsonToken.Bytes, array);
						return array;
					}
					if (tokenType == JsonToken.String)
					{
						string text = (string)this.Value;
						byte[] array2 = (text.Length == 0) ? new byte[0] : Convert.FromBase64String(text);
						this.SetToken(JsonToken.Bytes, array2);
						return array2;
					}
					if (tokenType == JsonToken.Null)
					{
						return null;
					}
					if (tokenType == JsonToken.Bytes)
					{
						return (byte[])this.Value;
					}
					if (tokenType == JsonToken.StartArray)
					{
						List<byte> list = new List<byte>();
						while (this.ReadInternal())
						{
							tokenType = this.TokenType;
							JsonToken jsonToken = tokenType;
							switch (jsonToken)
							{
							case JsonToken.Comment:
								continue;
							case JsonToken.Raw:
								break;
							case JsonToken.Integer:
								list.Add(Convert.ToByte(this.Value, CultureInfo.InvariantCulture));
								continue;
							default:
								if (jsonToken == JsonToken.EndArray)
								{
									byte[] array3 = list.ToArray();
									this.SetToken(JsonToken.Bytes, array3);
									return array3;
								}
								break;
							}
							throw JsonReaderException.Create(this, "Unexpected token when reading bytes: {0}.".FormatWith(CultureInfo.InvariantCulture, tokenType));
						}
						throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
					}
					if (tokenType == JsonToken.EndArray)
					{
						return null;
					}
					throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, tokenType));
				}
			}
			this.SetToken(JsonToken.None);
			return null;
		}

		internal decimal? ReadAsDecimalInternal()
		{
			this._readType = ReadType.ReadAsDecimal;
			while (this.ReadInternal())
			{
				JsonToken tokenType = this.TokenType;
				if (tokenType != JsonToken.Comment)
				{
					if (tokenType == JsonToken.Integer || tokenType == JsonToken.Float)
					{
						if (!(this.Value is decimal))
						{
							this.SetToken(JsonToken.Float, Convert.ToDecimal(this.Value, CultureInfo.InvariantCulture));
						}
						return new decimal?((decimal)this.Value);
					}
					if (tokenType == JsonToken.Null)
					{
						return null;
					}
					if (tokenType == JsonToken.String)
					{
						string text = (string)this.Value;
						if (string.IsNullOrEmpty(text))
						{
							this.SetToken(JsonToken.Null);
							return null;
						}
						decimal num;
						if (decimal.TryParse(text, NumberStyles.Number, this.Culture, out num))
						{
							this.SetToken(JsonToken.Float, num);
							return new decimal?(num);
						}
						throw JsonReaderException.Create(this, "Could not convert string to decimal: {0}.".FormatWith(CultureInfo.InvariantCulture, this.Value));
					}
					else
					{
						if (tokenType == JsonToken.EndArray)
						{
							return null;
						}
						throw JsonReaderException.Create(this, "Error reading decimal. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, tokenType));
					}
				}
			}
			this.SetToken(JsonToken.None);
			return null;
		}

		internal int? ReadAsInt32Internal()
		{
			this._readType = ReadType.ReadAsInt32;
			while (this.ReadInternal())
			{
				JsonToken tokenType = this.TokenType;
				if (tokenType != JsonToken.Comment)
				{
					if (tokenType == JsonToken.Integer || tokenType == JsonToken.Float)
					{
						if (!(this.Value is int))
						{
							this.SetToken(JsonToken.Integer, Convert.ToInt32(this.Value, CultureInfo.InvariantCulture));
						}
						return new int?((int)this.Value);
					}
					if (tokenType == JsonToken.Null)
					{
						return null;
					}
					if (tokenType == JsonToken.String)
					{
						string text = (string)this.Value;
						if (string.IsNullOrEmpty(text))
						{
							this.SetToken(JsonToken.Null);
							return null;
						}
						int num;
						if (int.TryParse(text, NumberStyles.Integer, this.Culture, out num))
						{
							this.SetToken(JsonToken.Integer, num);
							return new int?(num);
						}
						throw JsonReaderException.Create(this, "Could not convert string to integer: {0}.".FormatWith(CultureInfo.InvariantCulture, this.Value));
					}
					else
					{
						if (tokenType == JsonToken.EndArray)
						{
							return null;
						}
						throw JsonReaderException.Create(this, "Error reading integer. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
					}
				}
			}
			this.SetToken(JsonToken.None);
			return null;
		}

		internal string ReadAsStringInternal()
		{
			this._readType = ReadType.ReadAsString;
			while (this.ReadInternal())
			{
				JsonToken tokenType = this.TokenType;
				if (tokenType != JsonToken.Comment)
				{
					if (tokenType == JsonToken.String)
					{
						return (string)this.Value;
					}
					if (tokenType == JsonToken.Null)
					{
						return null;
					}
					if (JsonReader.IsPrimitiveToken(tokenType) && this.Value != null)
					{
						string text;
						if (this.Value is IFormattable)
						{
							text = ((IFormattable)this.Value).ToString(null, this.Culture);
						}
						else
						{
							text = this.Value.ToString();
						}
						this.SetToken(JsonToken.String, text);
						return text;
					}
					if (tokenType == JsonToken.EndArray)
					{
						return null;
					}
					throw JsonReaderException.Create(this, "Error reading string. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, tokenType));
				}
			}
			this.SetToken(JsonToken.None);
			return null;
		}

		internal DateTime? ReadAsDateTimeInternal()
		{
			this._readType = ReadType.ReadAsDateTime;
			while (this.ReadInternal())
			{
				if (this.TokenType != JsonToken.Comment)
				{
					if (this.TokenType == JsonToken.Date)
					{
						return new DateTime?((DateTime)this.Value);
					}
					if (this.TokenType == JsonToken.Null)
					{
						return null;
					}
					if (this.TokenType == JsonToken.String)
					{
						string text = (string)this.Value;
						if (string.IsNullOrEmpty(text))
						{
							this.SetToken(JsonToken.Null);
							return null;
						}
						DateTime dateTime;
						if (DateTime.TryParse(text, this.Culture, DateTimeStyles.RoundtripKind, out dateTime))
						{
							dateTime = DateTimeUtils.EnsureDateTime(dateTime, this.DateTimeZoneHandling);
							this.SetToken(JsonToken.Date, dateTime);
							return new DateTime?(dateTime);
						}
						throw JsonReaderException.Create(this, "Could not convert string to DateTime: {0}.".FormatWith(CultureInfo.InvariantCulture, this.Value));
					}
					else
					{
						if (this.TokenType == JsonToken.EndArray)
						{
							return null;
						}
						throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
					}
				}
			}
			this.SetToken(JsonToken.None);
			return null;
		}

		private bool IsWrappedInTypeObject()
		{
			this._readType = ReadType.Read;
			if (this.TokenType != JsonToken.StartObject)
			{
				return false;
			}
			if (!this.ReadInternal())
			{
				throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
			}
			if (this.Value.ToString() == "$type")
			{
				this.ReadInternal();
				if (this.Value != null && this.Value.ToString().StartsWith("System.Byte[]"))
				{
					this.ReadInternal();
					if (this.Value.ToString() == "$value")
					{
						return true;
					}
				}
			}
			throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, JsonToken.StartObject));
		}

		public void Skip()
		{
			if (this.TokenType == JsonToken.PropertyName)
			{
				this.Read();
			}
			if (JsonReader.IsStartToken(this.TokenType))
			{
				int depth = this.Depth;
				while (this.Read() && depth < this.Depth)
				{
				}
			}
		}

		protected void SetToken(JsonToken newToken)
		{
			this.SetToken(newToken, null);
		}

		protected void SetToken(JsonToken newToken, object value)
		{
			this._tokenType = newToken;
			this._value = value;
			switch (newToken)
			{
			case JsonToken.StartObject:
				this._currentState = JsonReader.State.ObjectStart;
				this.Push(JsonContainerType.Object);
				return;
			case JsonToken.StartArray:
				this._currentState = JsonReader.State.ArrayStart;
				this.Push(JsonContainerType.Array);
				return;
			case JsonToken.StartConstructor:
				this._currentState = JsonReader.State.ConstructorStart;
				this.Push(JsonContainerType.Constructor);
				return;
			case JsonToken.PropertyName:
				this._currentState = JsonReader.State.Property;
				this._currentPosition.PropertyName = (string)value;
				return;
			case JsonToken.Comment:
				break;
			case JsonToken.Raw:
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				if (this.Peek() != JsonContainerType.None)
				{
					this._currentState = JsonReader.State.PostValue;
				}
				else
				{
					this.SetFinished();
				}
				this.UpdateScopeWithFinishedValue();
				break;
			case JsonToken.EndObject:
				this.ValidateEnd(JsonToken.EndObject);
				return;
			case JsonToken.EndArray:
				this.ValidateEnd(JsonToken.EndArray);
				return;
			case JsonToken.EndConstructor:
				this.ValidateEnd(JsonToken.EndConstructor);
				return;
			default:
				return;
			}
		}

		private void UpdateScopeWithFinishedValue()
		{
			if (this._currentPosition.HasIndex)
			{
				this._currentPosition.Position = this._currentPosition.Position + 1;
			}
		}

		private void ValidateEnd(JsonToken endToken)
		{
			JsonContainerType jsonContainerType = this.Pop();
			if (this.GetTypeForCloseToken(endToken) != jsonContainerType)
			{
				throw JsonReaderException.Create(this, "JsonToken {0} is not valid for closing JsonType {1}.".FormatWith(CultureInfo.InvariantCulture, endToken, jsonContainerType));
			}
			if (this.Peek() != JsonContainerType.None)
			{
				this._currentState = JsonReader.State.PostValue;
				return;
			}
			this.SetFinished();
		}

		protected void SetStateBasedOnCurrent()
		{
			JsonContainerType jsonContainerType = this.Peek();
			switch (jsonContainerType)
			{
			case JsonContainerType.None:
				this.SetFinished();
				return;
			case JsonContainerType.Object:
				this._currentState = JsonReader.State.Object;
				return;
			case JsonContainerType.Array:
				this._currentState = JsonReader.State.Array;
				return;
			case JsonContainerType.Constructor:
				this._currentState = JsonReader.State.Constructor;
				return;
			default:
				throw JsonReaderException.Create(this, "While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith(CultureInfo.InvariantCulture, jsonContainerType));
			}
		}

		private void SetFinished()
		{
			if (this.SupportMultipleContent)
			{
				this._currentState = JsonReader.State.Start;
				return;
			}
			this._currentState = JsonReader.State.Finished;
		}

		internal static bool IsPrimitiveToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				return true;
			}
			return false;
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

		private JsonContainerType GetTypeForCloseToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.EndObject:
				return JsonContainerType.Object;
			case JsonToken.EndArray:
				return JsonContainerType.Array;
			case JsonToken.EndConstructor:
				return JsonContainerType.Constructor;
			default:
				throw JsonReaderException.Create(this, "Not a valid close JsonToken: {0}".FormatWith(CultureInfo.InvariantCulture, token));
			}
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._currentState != JsonReader.State.Closed && disposing)
			{
				this.Close();
			}
		}

		public virtual void Close()
		{
			this._currentState = JsonReader.State.Closed;
			this._tokenType = JsonToken.None;
			this._value = null;
		}
	}
}
