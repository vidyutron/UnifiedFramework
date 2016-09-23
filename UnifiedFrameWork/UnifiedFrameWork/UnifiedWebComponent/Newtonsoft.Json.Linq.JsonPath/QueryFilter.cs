using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class QueryFilter : PathFilter
	{
		public QueryExpression Expression
		{
			get;
			set;
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken current2 in current)
			{
				foreach (JToken current3 in ((IEnumerable<JToken>)current2))
				{
					if (this.Expression.IsMatch(current3))
					{
						yield return current3;
					}
				}
			}
			yield break;
		}
	}
}
