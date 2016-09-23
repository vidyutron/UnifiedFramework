using System;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	internal class JsonExtensionDataAttribute : Attribute
	{
		public bool WriteData
		{
			get;
			set;
		}

		public bool ReadData
		{
			get;
			set;
		}

		public JsonExtensionDataAttribute()
		{
			this.WriteData = true;
			this.ReadData = true;
		}
	}
}
