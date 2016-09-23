using System;

namespace UnifiedFramework.UnifiedReports.Source
{
	internal class ImageHtml
	{
		public static string GetSource(string ImgPath)
		{
			return "<img class='report-img materialboxed' src='file:///" + ImgPath + "' />";
		}
	}
}
