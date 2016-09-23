using Newtonsoft.Json.Utilities;
using System;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonContainerContract : JsonContract
	{
		private JsonContract _itemContract;

		private JsonContract _finalItemContract;

		internal JsonContract ItemContract
		{
			get
			{
				return this._itemContract;
			}
			set
			{
				this._itemContract = value;
				if (this._itemContract != null)
				{
					this._finalItemContract = (this._itemContract.UnderlyingType.IsSealed() ? this._itemContract : null);
					return;
				}
				this._finalItemContract = null;
			}
		}

		internal JsonContract FinalItemContract
		{
			get
			{
				return this._finalItemContract;
			}
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

		public ReferenceLoopHandling? ItemReferenceLoopHandling
		{
			get;
			set;
		}

		public TypeNameHandling? ItemTypeNameHandling
		{
			get;
			set;
		}

		internal JsonContainerContract(Type underlyingType) : base(underlyingType)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(underlyingType);
			if (jsonContainerAttribute != null)
			{
				if (jsonContainerAttribute.ItemConverterType != null)
				{
					this.ItemConverter = JsonConverterAttribute.CreateJsonConverterInstance(jsonContainerAttribute.ItemConverterType);
				}
				this.ItemIsReference = jsonContainerAttribute._itemIsReference;
				this.ItemReferenceLoopHandling = jsonContainerAttribute._itemReferenceLoopHandling;
				this.ItemTypeNameHandling = jsonContainerAttribute._itemTypeNameHandling;
			}
		}
	}
}
