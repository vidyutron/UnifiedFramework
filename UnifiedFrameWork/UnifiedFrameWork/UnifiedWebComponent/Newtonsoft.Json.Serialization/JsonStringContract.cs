using System;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonStringContract : JsonPrimitiveContract
	{
		public JsonStringContract(Type underlyingType) : base(underlyingType)
		{
			this.ContractType = JsonContractType.String;
		}
	}
}
