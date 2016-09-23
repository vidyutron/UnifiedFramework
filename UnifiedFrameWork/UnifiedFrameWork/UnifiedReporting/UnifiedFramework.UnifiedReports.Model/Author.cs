using System;

namespace UnifiedFramework.UnifiedReports.Model
{
	internal class Author : TestAttribute
	{
		public Author(string Name) : base(Name.Trim())
		{
		}
	}
}
