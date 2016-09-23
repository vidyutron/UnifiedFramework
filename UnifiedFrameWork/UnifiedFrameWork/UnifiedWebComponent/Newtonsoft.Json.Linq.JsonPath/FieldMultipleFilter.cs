using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class FieldMultipleFilter : PathFilter
	{
		public List<string> Names
		{
			get;
			set;
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken current2 in current)
			{
				JObject jObject = current2 as JObject;
				if (jObject != null)
				{
					foreach (string current3 in this.Names)
					{
						JToken jToken = jObject[current3];
						if (jToken != null)
						{
							yield return jToken;
						}
						if (errorWhenNoMatch)
						{
							throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, current3));
						}
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Properties {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", (from n in this.Names
					select "'" + n + "'").ToArray<string>()), current2.GetType().Name));
				}
			}
			yield break;
		}
	}
}
