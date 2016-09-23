using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports.Model
{
	internal class AttributeList
	{
		public List<TestAttribute> Categories;

		public int Count
		{
			get
			{
				return this.Categories.Count;
			}
		}

		public string GetItem(int Index)
		{
            return this.Categories.Count > Index ? this.Categories[Index].GetName() : null;
		}

		public bool Contains(Category c)
		{
			return this.Categories.Contains(c);
		}

		public AttributeList()
		{
			this.Categories = new List<TestAttribute>();
		}
	}
}
