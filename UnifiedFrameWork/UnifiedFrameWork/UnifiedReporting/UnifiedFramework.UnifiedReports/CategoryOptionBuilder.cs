using UnifiedFramework.UnifiedReports.Model;
using UnifiedFramework.UnifiedReports.Source;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnifiedFramework.UnifiedReports
{
	internal class CategoryOptionBuilder
	{
		internal static string Build(List<TestAttribute> Categories)
		{
            string text = string.Empty;
			List<string> list = new List<string>();
			Categories.ToList<TestAttribute>().ForEach(delegate(TestAttribute c)
			{
				list.Add(c.GetName());
			});
			list.Sort();
			foreach (var current in list)
			{
				text += SourceBuilder.Build(CategoryFilterHtml.GetOptionSource(), new string[]
				{
					UnifiedReportFlag.GetPlaceHolder("testCategory"),
					UnifiedReportFlag.GetPlaceHolder("testCategoryU")
				}, new string[]
				{
					current,
					current.ToLower().Replace(" ", "")
				});
			}
			return text;
		}
	}
}
