using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	internal abstract class JsonContract
	{
		internal bool IsNullable;

		internal bool IsConvertable;

		internal bool IsSealed;

		internal bool IsEnum;

		internal Type NonNullableUnderlyingType;

		internal ReadType InternalReadType;

		internal JsonContractType ContractType;

		internal bool IsReadOnlyOrFixedSize;

		internal bool IsInstantiable;

		private List<SerializationCallback> _onDeserializedCallbacks;

		private IList<SerializationCallback> _onDeserializingCallbacks;

		private IList<SerializationCallback> _onSerializedCallbacks;

		private IList<SerializationCallback> _onSerializingCallbacks;

		private IList<SerializationErrorCallback> _onErrorCallbacks;

		public Type UnderlyingType
		{
			get;
			private set;
		}

		public Type CreatedType
		{
			get;
			set;
		}

		public bool? IsReference
		{
			get;
			set;
		}

		public JsonConverter Converter
		{
			get;
			set;
		}

		internal JsonConverter InternalConverter
		{
			get;
			set;
		}

		public IList<SerializationCallback> OnDeserializedCallbacks
		{
			get
			{
				if (this._onDeserializedCallbacks == null)
				{
					this._onDeserializedCallbacks = new List<SerializationCallback>();
				}
				return this._onDeserializedCallbacks;
			}
		}

		public IList<SerializationCallback> OnDeserializingCallbacks
		{
			get
			{
				if (this._onDeserializingCallbacks == null)
				{
					this._onDeserializingCallbacks = new List<SerializationCallback>();
				}
				return this._onDeserializingCallbacks;
			}
		}

		public IList<SerializationCallback> OnSerializedCallbacks
		{
			get
			{
				if (this._onSerializedCallbacks == null)
				{
					this._onSerializedCallbacks = new List<SerializationCallback>();
				}
				return this._onSerializedCallbacks;
			}
		}

		public IList<SerializationCallback> OnSerializingCallbacks
		{
			get
			{
				if (this._onSerializingCallbacks == null)
				{
					this._onSerializingCallbacks = new List<SerializationCallback>();
				}
				return this._onSerializingCallbacks;
			}
		}

		public IList<SerializationErrorCallback> OnErrorCallbacks
		{
			get
			{
				if (this._onErrorCallbacks == null)
				{
					this._onErrorCallbacks = new List<SerializationErrorCallback>();
				}
				return this._onErrorCallbacks;
			}
		}

		[Obsolete("This property is obsolete and has been replaced by the OnDeserializedCallbacks collection.")]
		public MethodInfo OnDeserialized
		{
			get
			{
				if (this.OnDeserializedCallbacks.Count <= 0)
				{
					return null;
				}
				return this.OnDeserializedCallbacks[0].Method();
			}
			set
			{
				this.OnDeserializedCallbacks.Clear();
				this.OnDeserializedCallbacks.Add(JsonContract.CreateSerializationCallback(value));
			}
		}

		[Obsolete("This property is obsolete and has been replaced by the OnDeserializingCallbacks collection.")]
		public MethodInfo OnDeserializing
		{
			get
			{
				if (this.OnDeserializingCallbacks.Count <= 0)
				{
					return null;
				}
				return this.OnDeserializingCallbacks[0].Method();
			}
			set
			{
				this.OnDeserializingCallbacks.Clear();
				this.OnDeserializingCallbacks.Add(JsonContract.CreateSerializationCallback(value));
			}
		}

		[Obsolete("This property is obsolete and has been replaced by the OnSerializedCallbacks collection.")]
		public MethodInfo OnSerialized
		{
			get
			{
				if (this.OnSerializedCallbacks.Count <= 0)
				{
					return null;
				}
				return this.OnSerializedCallbacks[0].Method();
			}
			set
			{
				this.OnSerializedCallbacks.Clear();
				this.OnSerializedCallbacks.Add(JsonContract.CreateSerializationCallback(value));
			}
		}

		[Obsolete("This property is obsolete and has been replaced by the OnSerializingCallbacks collection.")]
		public MethodInfo OnSerializing
		{
			get
			{
				if (this.OnSerializingCallbacks.Count <= 0)
				{
					return null;
				}
				return this.OnSerializingCallbacks[0].Method();
			}
			set
			{
				this.OnSerializingCallbacks.Clear();
				this.OnSerializingCallbacks.Add(JsonContract.CreateSerializationCallback(value));
			}
		}

		[Obsolete("This property is obsolete and has been replaced by the OnErrorCallbacks collection.")]
		public MethodInfo OnError
		{
			get
			{
				if (this.OnErrorCallbacks.Count <= 0)
				{
					return null;
				}
				return this.OnErrorCallbacks[0].Method();
			}
			set
			{
				this.OnErrorCallbacks.Clear();
				this.OnErrorCallbacks.Add(JsonContract.CreateSerializationErrorCallback(value));
			}
		}

		public Func<object> DefaultCreator
		{
			get;
			set;
		}

		public bool DefaultCreatorNonPublic
		{
			get;
			set;
		}

		internal JsonContract(Type underlyingType)
		{
			ValidationUtils.ArgumentNotNull(underlyingType, "underlyingType");
			this.UnderlyingType = underlyingType;
			this.IsSealed = underlyingType.IsSealed();
			this.IsInstantiable = (!underlyingType.IsInterface() && !underlyingType.IsAbstract());
			this.IsNullable = ReflectionUtils.IsNullable(underlyingType);
			this.NonNullableUnderlyingType = ((this.IsNullable && ReflectionUtils.IsNullableType(underlyingType)) ? Nullable.GetUnderlyingType(underlyingType) : underlyingType);
			this.CreatedType = this.NonNullableUnderlyingType;
			this.IsConvertable = ConvertUtils.IsConvertible(this.NonNullableUnderlyingType);
			this.IsEnum = this.NonNullableUnderlyingType.IsEnum();
			if (this.NonNullableUnderlyingType == typeof(byte[]))
			{
				this.InternalReadType = ReadType.ReadAsBytes;
				return;
			}
			if (this.NonNullableUnderlyingType == typeof(int))
			{
				this.InternalReadType = ReadType.ReadAsInt32;
				return;
			}
			if (this.NonNullableUnderlyingType == typeof(decimal))
			{
				this.InternalReadType = ReadType.ReadAsDecimal;
				return;
			}
			if (this.NonNullableUnderlyingType == typeof(string))
			{
				this.InternalReadType = ReadType.ReadAsString;
				return;
			}
			if (this.NonNullableUnderlyingType == typeof(DateTime))
			{
				this.InternalReadType = ReadType.ReadAsDateTime;
				return;
			}
			if (this.NonNullableUnderlyingType == typeof(DateTimeOffset))
			{
				this.InternalReadType = ReadType.ReadAsDateTimeOffset;
				return;
			}
			this.InternalReadType = ReadType.Read;
		}

		internal void InvokeOnSerializing(object o, StreamingContext context)
		{
			if (this._onSerializingCallbacks != null)
			{
				foreach (SerializationCallback current in this._onSerializingCallbacks)
				{
					current(o, context);
				}
			}
		}

		internal void InvokeOnSerialized(object o, StreamingContext context)
		{
			if (this._onSerializedCallbacks != null)
			{
				foreach (SerializationCallback current in this._onSerializedCallbacks)
				{
					current(o, context);
				}
			}
		}

		internal void InvokeOnDeserializing(object o, StreamingContext context)
		{
			if (this._onDeserializingCallbacks != null)
			{
				foreach (SerializationCallback current in this._onDeserializingCallbacks)
				{
					current(o, context);
				}
			}
		}

		internal void InvokeOnDeserialized(object o, StreamingContext context)
		{
			if (this._onDeserializedCallbacks != null)
			{
				foreach (SerializationCallback current in this._onDeserializedCallbacks)
				{
					current(o, context);
				}
			}
		}

		internal void InvokeOnError(object o, StreamingContext context, ErrorContext errorContext)
		{
			if (this._onErrorCallbacks != null)
			{
				foreach (SerializationErrorCallback current in this._onErrorCallbacks)
				{
					current(o, context, errorContext);
				}
			}
		}

		internal static SerializationCallback CreateSerializationCallback(MethodInfo callbackMethodInfo)
		{
			return delegate(object o, StreamingContext context)
			{
				callbackMethodInfo.Invoke(o, new object[]
				{
					context
				});
			};
		}

		internal static SerializationErrorCallback CreateSerializationErrorCallback(MethodInfo callbackMethodInfo)
		{
			return delegate(object o, StreamingContext context, ErrorContext econtext)
			{
				callbackMethodInfo.Invoke(o, new object[]
				{
					context,
					econtext
				});
			};
		}
	}
}
