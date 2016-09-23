using Newtonsoft.Json.Utilities;
using System;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonProperty
	{
		internal Required? _required;

		internal bool _hasExplicitDefaultValue;

		internal object _defaultValue;

		private string _propertyName;

		private bool _skipPropertyNameEscape;

		internal JsonContract PropertyContract
		{
			get;
			set;
		}

		public string PropertyName
		{
			get
			{
				return this._propertyName;
			}
			set
			{
				this._propertyName = value;
				this.CalculateSkipPropertyNameEscape();
			}
		}

		public Type DeclaringType
		{
			get;
			set;
		}

		public int? Order
		{
			get;
			set;
		}

		public string UnderlyingName
		{
			get;
			set;
		}

		public IValueProvider ValueProvider
		{
			get;
			set;
		}

		public Type PropertyType
		{
			get;
			set;
		}

		public JsonConverter Converter
		{
			get;
			set;
		}

		public JsonConverter MemberConverter
		{
			get;
			set;
		}

		public bool Ignored
		{
			get;
			set;
		}

		public bool Readable
		{
			get;
			set;
		}

		public bool Writable
		{
			get;
			set;
		}

		public bool HasMemberAttribute
		{
			get;
			set;
		}

		public object DefaultValue
		{
			get
			{
				return this._defaultValue;
			}
			set
			{
				this._hasExplicitDefaultValue = true;
				this._defaultValue = value;
			}
		}

		public Required Required
		{
			get
			{
				Required? required = this._required;
				if (!required.HasValue)
				{
					return Required.Default;
				}
				return required.GetValueOrDefault();
			}
			set
			{
				this._required = new Required?(value);
			}
		}

		public bool? IsReference
		{
			get;
			set;
		}

		public NullValueHandling? NullValueHandling
		{
			get;
			set;
		}

		public DefaultValueHandling? DefaultValueHandling
		{
			get;
			set;
		}

		public ReferenceLoopHandling? ReferenceLoopHandling
		{
			get;
			set;
		}

		public ObjectCreationHandling? ObjectCreationHandling
		{
			get;
			set;
		}

		public TypeNameHandling? TypeNameHandling
		{
			get;
			set;
		}

		public Predicate<object> ShouldSerialize
		{
			get;
			set;
		}

		public Predicate<object> GetIsSpecified
		{
			get;
			set;
		}

		public Action<object, object> SetIsSpecified
		{
			get;
			set;
		}

		public JsonConverter ItemConverter
		{
			get;
			set;
		}

		public bool? ItemIsReference
		{
			get;
			set;
		}

		public TypeNameHandling? ItemTypeNameHandling
		{
			get;
			set;
		}

		public ReferenceLoopHandling? ItemReferenceLoopHandling
		{
			get;
			set;
		}

		private void CalculateSkipPropertyNameEscape()
		{
			if (this._propertyName == null)
			{
				this._skipPropertyNameEscape = false;
				return;
			}
			this._skipPropertyNameEscape = true;
			string propertyName = this._propertyName;
			for (int i = 0; i < propertyName.Length; i++)
			{
				char c = propertyName[i];
				if (!char.IsLetterOrDigit(c) && c != '_' && c != '@')
				{
					this._skipPropertyNameEscape = false;
					return;
				}
			}
		}

		internal object GetResolvedDefaultValue()
		{
			if (!this._hasExplicitDefaultValue && this.PropertyType != null)
			{
				return ReflectionUtils.GetDefaultValue(this.PropertyType);
			}
			return this._defaultValue;
		}

		public override string ToString()
		{
			return this.PropertyName;
		}

		internal void WritePropertyName(JsonWriter writer)
		{
			if (this._skipPropertyNameEscape)
			{
				writer.WritePropertyName(this.PropertyName, false);
				return;
			}
			writer.WritePropertyName(this.PropertyName);
		}
	}
}
