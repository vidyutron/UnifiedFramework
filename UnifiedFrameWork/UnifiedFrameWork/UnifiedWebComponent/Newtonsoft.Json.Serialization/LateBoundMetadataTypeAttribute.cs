using Newtonsoft.Json.Utilities;
using System;
using System.Reflection;

namespace Newtonsoft.Json.Serialization
{
	internal class LateBoundMetadataTypeAttribute : IMetadataTypeAttribute
	{
		private static PropertyInfo _metadataClassTypeProperty;

		private readonly object _attribute;

		public Type MetadataClassType
		{
			get
			{
				if (LateBoundMetadataTypeAttribute._metadataClassTypeProperty == null)
				{
					LateBoundMetadataTypeAttribute._metadataClassTypeProperty = this._attribute.GetType().GetProperty("MetadataClassType");
				}
				return (Type)ReflectionUtils.GetMemberValue(LateBoundMetadataTypeAttribute._metadataClassTypeProperty, this._attribute);
			}
		}

		public LateBoundMetadataTypeAttribute(object attribute)
		{
			this._attribute = attribute;
		}
	}
}
