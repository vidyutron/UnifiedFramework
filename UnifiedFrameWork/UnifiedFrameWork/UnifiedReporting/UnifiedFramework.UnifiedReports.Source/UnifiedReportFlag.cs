using System;

namespace UnifiedFramework.UnifiedReports.Source
{
	internal class UnifiedReportFlag
	{
		public static string GetPlaceHolder(string flag)
		{
			return "<!--%%" + flag.ToUpper() + "%%-->";
		}
	}
}
