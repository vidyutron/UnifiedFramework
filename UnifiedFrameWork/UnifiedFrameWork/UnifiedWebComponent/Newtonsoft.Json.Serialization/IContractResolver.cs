using System;

namespace Newtonsoft.Json.Serialization
{
	internal interface IContractResolver
	{
		JsonContract ResolveContract(Type type);
	}
}
