using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class ScanFilter : PathFilter
	{
		public string Name
		{
			get;
			set;
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken current2 in current)
			{
				if (this.Name == null)
				{
					yield return current2;
				}
				JToken jToken = current2;
				JToken jToken2 = current2;
				while (true)
				{
					if (jToken2 != null)
					{
						jToken = jToken2.First;
					}
					else
					{
						while (jToken != null && jToken != current2 && jToken == jToken.Parent.Last)
						{
							jToken = jToken.Parent;
						}
						if (jToken == null || jToken == current2)
						{
							break;
						}
						jToken = jToken.Next;
					}
					JProperty jProperty = jToken as JProperty;
					if (jProperty != null)
					{
						if (jProperty.Name == this.Name)
						{
							yield return jProperty.Value;
						}
					}
					else if (this.Name == null)
					{
						yield return jToken;
					}
					jToken2 = (jToken as JContainer);
				}
			}
			yield break;
		}
	}
}
