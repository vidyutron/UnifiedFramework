using System;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonLinqContract : JsonContract
	{
		public JsonLinqContract(Type underlyingType) : base(underlyingType)
		{
			this.ContractType = JsonContractType.Linq;
		}
	}
}
