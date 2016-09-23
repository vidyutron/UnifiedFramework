using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports.Source
{
	internal class Icon
	{
		private static Dictionary<LogStatus, string> icon = new Dictionary<LogStatus, string>();

		public static void Override(LogStatus LogStatus, string Icon)
		{
            UnifiedFramework.UnifiedReports.Source.Icon.icon.Add(LogStatus, Icon);
		}

		public static string GetIcon(LogStatus LogStatus)
		{
			string result;
			if (Icon.icon.ContainsKey(LogStatus))
			{
				result = Icon.icon[LogStatus];
			}
			else
			{
				string text = LogStatus.ToString().ToUpper();
				switch (text)
				{
				case "FAIL":
					result = "times-circle-o";
					return result;
				case "ERROR":
					result = "exclamation-circle";
					return result;
				case "FATAL":
					result = "exclamation-circle";
					return result;
				case "PASS":
					result = "check-circle-o";
					return result;
				case "INFO":
					result = "info-circle";
					return result;
				case "WARNING":
					result = "warning";
					return result;
				case "SKIP":
					result = "chevron-circle-right";
					return result;
				}
				result = "question";
			}
			return result;
		}
	}
}
