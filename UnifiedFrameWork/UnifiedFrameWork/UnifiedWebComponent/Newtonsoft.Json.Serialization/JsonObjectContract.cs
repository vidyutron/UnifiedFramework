using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonObjectContract : JsonContainerContract
	{
		private bool? _hasRequiredOrDefaultValueProperties;

		public MemberSerialization MemberSerialization
		{
			get;
			set;
		}

		public Required? ItemRequired
		{
			get;
			set;
		}

		public JsonPropertyCollection Properties
		{
			get;
			private set;
		}

		public JsonPropertyCollection ConstructorParameters
		{
			get;
			private set;
		}

		public ConstructorInfo OverrideConstructor
		{
			get;
			set;
		}

		public ConstructorInfo ParametrizedConstructor
		{
			get;
			set;
		}

		public ExtensionDataSetter ExtensionDataSetter
		{
			get;
			set;
		}

		public ExtensionDataGetter ExtensionDataGetter
		{
			get;
			set;
		}

		internal bool HasRequiredOrDefaultValueProperties
		{
			get
			{
				if (!this._hasRequiredOrDefaultValueProperties.HasValue)
				{
					this._hasRequiredOrDefaultValueProperties = new bool?(false);
					if (this.ItemRequired.GetValueOrDefault(Required.Default) != Required.Default)
					{
						this._hasRequiredOrDefaultValueProperties = new bool?(true);
					}
					else
					{
						foreach (JsonProperty current in this.Properties)
						{
							if (current.Required != Required.Default || ((current.DefaultValueHandling & DefaultValueHandling.Populate) == DefaultValueHandling.Populate && current.Writable))
							{
								this._hasRequiredOrDefaultValueProperties = new bool?(true);
								break;
							}
						}
					}
				}
				return this._hasRequiredOrDefaultValueProperties.Value;
			}
		}

		public JsonObjectContract(Type underlyingType) : base(underlyingType)
		{
			this.ContractType = JsonContractType.Object;
			this.Properties = new JsonPropertyCollection(base.UnderlyingType);
			this.ConstructorParameters = new JsonPropertyCollection(base.UnderlyingType);
		}

		[SecuritySafeCritical]
		internal object GetUninitializedObject()
		{
			if (!JsonTypeReflector.FullyTrusted)
			{
				throw new JsonException("Insufficient permissions. Creating an uninitialized '{0}' type requires full trust.".FormatWith(CultureInfo.InvariantCulture, this.NonNullableUnderlyingType));
			}
			return FormatterServices.GetUninitializedObject(this.NonNullableUnderlyingType);
		}
	}
}
