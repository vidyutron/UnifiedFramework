using System;

namespace UnifiedFramework.UnifiedReports
{
	internal abstract class LogSettings
	{
		protected static string LogTimeFormat = "HH:mm:ss";

		protected static string LogDateFormat = "yyyy-MM-dd";

		protected static string LogDateTimeFormat = LogSettings.LogDateFormat + " " + LogSettings.LogTimeFormat;
	}
}
