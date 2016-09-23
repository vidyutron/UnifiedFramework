using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class ArrayIndexFilter : PathFilter
	{
		public int? Index
		{
			get;
			set;
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken current2 in current)
			{
				if (this.Index.HasValue)
				{
					JToken tokenIndex = PathFilter.GetTokenIndex(current2, errorWhenNoMatch, this.Index.Value);
					if (tokenIndex != null)
					{
						yield return tokenIndex;
					}
				}
				else if (current2 is JArray || current2 is JConstructor)
				{
					foreach (JToken current3 in ((IEnumerable<JToken>)current2))
					{
						yield return current3;
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, current2.GetType().Name));
				}
			}
			yield break;
		}
	}
}
