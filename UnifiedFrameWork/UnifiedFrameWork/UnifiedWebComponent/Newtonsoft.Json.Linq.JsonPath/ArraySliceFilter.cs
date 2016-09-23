using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class ArraySliceFilter : PathFilter
	{
		public int? Start
		{
			get;
			set;
		}

		public int? End
		{
			get;
			set;
		}

		public int? Step
		{
			get;
			set;
		}

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			if (this.Step == 0)
			{
				throw new JsonException("Step cannot be zero.");
			}
			foreach (JToken current2 in current)
			{
				JArray jArray = current2 as JArray;
				if (jArray != null)
				{
					int num = this.Step ?? 1;
					int num2 = this.Start ?? ((num > 0) ? 0 : (jArray.Count - 1));
					int num3 = this.End ?? ((num > 0) ? jArray.Count : -1);
					if (this.Start < 0)
					{
						num2 = jArray.Count + num2;
					}
					if (this.End < 0)
					{
						num3 = jArray.Count + num3;
					}
					num2 = Math.Max(num2, (num > 0) ? 0 : -2147483648);
					num2 = Math.Min(num2, (num > 0) ? jArray.Count : (jArray.Count - 1));
					num3 = Math.Max(num3, -1);
					num3 = Math.Min(num3, jArray.Count);
					bool positiveStep = num > 0;
					if (this.IsValid(num2, num3, positiveStep))
					{
						int num4 = num2;
						while (this.IsValid(num4, num3, positiveStep))
						{
							yield return jArray[num4];
							num4 += num;
						}
					}
					else if (errorWhenNoMatch)
					{
						throw new JsonException("Array slice of {0} to {1} returned no results.".FormatWith(CultureInfo.InvariantCulture, this.Start.HasValue ? this.Start.Value.ToString(CultureInfo.InvariantCulture) : "*", this.End.HasValue ? this.End.Value.ToString(CultureInfo.InvariantCulture) : "*"));
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Array slice is not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, current2.GetType().Name));
				}
			}
			yield break;
		}

		private bool IsValid(int index, int stopIndex, bool positiveStep)
		{
			if (positiveStep)
			{
				return index < stopIndex;
			}
			return index > stopIndex;
		}
	}
}
