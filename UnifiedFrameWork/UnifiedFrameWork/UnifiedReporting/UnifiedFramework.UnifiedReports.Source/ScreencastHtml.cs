using System;

namespace UnifiedFramework.UnifiedReports.Source
{
	internal class ScreencastHtml
	{
		public static string GetSource(string ScreencastPath)
		{
			return "<video id='video' width='50%' controls><source src='file:///" + ScreencastPath + "'>Your browser does not support the video tag.</video>";
		}
	}
}
