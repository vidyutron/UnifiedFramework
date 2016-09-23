using System;

namespace UnifiedFramework.UnifiedReports.Model
{
	internal class Category : TestAttribute
	{
		public Category(string Name) : base(Name.Trim())
		{
		}
	}
}
