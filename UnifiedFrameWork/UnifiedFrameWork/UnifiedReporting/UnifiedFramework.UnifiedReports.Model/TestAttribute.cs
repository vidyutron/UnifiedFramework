using System;

namespace UnifiedFramework.UnifiedReports.Model
{
	internal abstract class TestAttribute
	{
		protected string name;

		public string GetName()
		{
			return this.name;
		}

		protected TestAttribute(string Name)
		{
			this.name = Name;
		}
	}
}
