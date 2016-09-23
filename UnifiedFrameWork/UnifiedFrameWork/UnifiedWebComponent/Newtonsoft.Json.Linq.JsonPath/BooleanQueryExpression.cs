using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class BooleanQueryExpression : QueryExpression
	{
		public List<PathFilter> Path
		{
			get;
			set;
		}

		public JValue Value
		{
			get;
			set;
		}

		public override bool IsMatch(JToken t)
		{
			IEnumerable<JToken> enumerable = JPath.Evaluate(this.Path, t, false);
			foreach (JToken current in enumerable)
			{
				JValue jValue = current as JValue;
				switch (base.Operator)
				{
				case QueryOperator.Equals:
					if (jValue != null && jValue.Equals(this.Value))
					{
						bool result = true;
						return result;
					}
					break;
				case QueryOperator.NotEquals:
					if (jValue != null && !jValue.Equals(this.Value))
					{
						bool result = true;
						return result;
					}
					break;
				case QueryOperator.Exists:
				{
					bool result = true;
					return result;
				}
				case QueryOperator.LessThan:
					if (jValue != null && jValue.CompareTo(this.Value) < 0)
					{
						bool result = true;
						return result;
					}
					break;
				case QueryOperator.LessThanOrEquals:
					if (jValue != null && jValue.CompareTo(this.Value) <= 0)
					{
						bool result = true;
						return result;
					}
					break;
				case QueryOperator.GreaterThan:
					if (jValue != null && jValue.CompareTo(this.Value) > 0)
					{
						bool result = true;
						return result;
					}
					break;
				case QueryOperator.GreaterThanOrEquals:
					if (jValue != null && jValue.CompareTo(this.Value) >= 0)
					{
						bool result = true;
						return result;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			return false;
		}
	}
}
