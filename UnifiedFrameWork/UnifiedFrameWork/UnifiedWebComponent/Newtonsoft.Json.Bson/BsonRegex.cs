using System;

namespace Newtonsoft.Json.Bson
{
	internal class BsonRegex : BsonToken
	{
		public BsonString Pattern
		{
			get;
			set;
		}

		public BsonString Options
		{
			get;
			set;
		}

		public override BsonType Type
		{
			get
			{
				return BsonType.Regex;
			}
		}

		public BsonRegex(string pattern, string options)
		{
			this.Pattern = new BsonString(pattern, false);
			this.Options = new BsonString(options, false);
		}
	}
}
