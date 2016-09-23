using UnifiedFramework.UnifiedReports.Model;
using UnifiedFramework.UnifiedReports.Source;
using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports
{
	internal class MediaViewBuilder
	{
		public static string GetSource<T>(List<T> MediaList, string type)
		{
			string text = string.Empty;
			string result;
			if (MediaList == null || MediaList.Count == 0)
			{
				text = ObjectEmbedHtml.GetFullWidth().Replace(UnifiedReportFlag.GetPlaceHolder("objectViewValue"), "No media was embed for the tests in this report.").Replace(UnifiedReportFlag.GetPlaceHolder("objectViewNull"), UnifiedReportFlag.GetPlaceHolder("objectViewNull" + type));
				result = text;
			}
			else
			{
				foreach (object obj in MediaList)
				{
					text += ObjectEmbedHtml.GetColumn();
					if (obj is ScreenCapture)
					{
						text = text.Replace(UnifiedReportFlag.GetPlaceHolder("objectViewParam"), ((ScreenCapture)obj).TestName).Replace(UnifiedReportFlag.GetPlaceHolder("objectViewValue"), ((ScreenCapture)obj).Source);
					}
					if (obj is Screencast)
					{
						text = text.Replace(UnifiedReportFlag.GetPlaceHolder("objectViewParam"), ((Screencast)obj).TestName).Replace(UnifiedReportFlag.GetPlaceHolder("objectViewValue"), ((Screencast)obj).Source);
					}
				}
				result = text;
			}
			return result;
		}
	}
}
