using UnifiedFramework.UnifiedReports.Source;
using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports
{
	internal class CategorySourceBuilder
	{
		public static string buildOptions(List<string> Categories)
		{
			string text = string.Empty;
			Categories.Sort();
			foreach (var current in Categories)
			{
				text += SourceBuilder.Build(CategoryHtml.GetOptionSource(), new string[]
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
