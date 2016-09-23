using System;

namespace Newtonsoft.Json.Converters
{
	internal interface IEntityKeyMember
	{
		string Key
		{
			get;
			set;
		}

		object Value
		{
			get;
			set;
		}
	}
}
