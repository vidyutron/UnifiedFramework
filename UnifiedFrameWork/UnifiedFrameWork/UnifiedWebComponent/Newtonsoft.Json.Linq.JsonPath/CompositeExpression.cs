using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class CompositeExpression : QueryExpression
	{
		public List<QueryExpression> Expressions
		{
			get;
			set;
		}

		public CompositeExpression()
		{
			this.Expressions = new List<QueryExpression>();
		}

		public override bool IsMatch(JToken t)
		{
			bool result;
			switch (base.Operator)
			{
			case QueryOperator.And:
				foreach (QueryExpression current in this.Expressions)
				{
					if (!current.IsMatch(t))
					{
						result = false;
						return result;
					}
				}
				return true;
			case QueryOperator.Or:
				foreach (QueryExpression current2 in this.Expressions)
				{
					if (current2.IsMatch(t))
					{
						result = true;
						return result;
					}
				}
				return false;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}
	}
}
