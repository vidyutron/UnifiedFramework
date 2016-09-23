using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class ArrayMultipleIndexFilter : PathFilter
	{
		public List<int> Indexes
		{
			get;
			set;
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken current2 in current)
			{
				foreach (int current3 in this.Indexes)
				{
					JToken tokenIndex = PathFilter.GetTokenIndex(current2, errorWhenNoMatch, current3);
					if (tokenIndex != null)
					{
						yield return tokenIndex;
					}
				}
			}
			yield break;
		}
	}
}
