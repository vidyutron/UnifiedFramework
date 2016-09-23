using System;

namespace Newtonsoft.Json.Schema
{
	[Flags]
	internal enum JsonSchemaType
	{
		None = 0,
		String = 1,
		Float = 2,
		Integer = 4,
		Boolean = 8,
		Object = 16,
		Array = 32,
		Null = 64,
		Any = 127
	}
}
