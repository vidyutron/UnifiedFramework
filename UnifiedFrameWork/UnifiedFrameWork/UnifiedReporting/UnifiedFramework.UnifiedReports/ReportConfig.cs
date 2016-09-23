using UnifiedFramework.UnifiedReports.Source;
using System;

namespace UnifiedFramework.UnifiedReports
{
	public class ReportConfig
	{
		private ReportInstance report;

		public ReportConfig InsertJs(string Script)
		{
			Script = "<script type='text/javascript'>" + Script + "</script>";
			this.report.UpdateSource(this.report.Source.Replace(UnifiedReportFlag.GetPlaceHolder("customscript"), Script + UnifiedReportFlag.GetPlaceHolder("customscript")));
			return this;
		}

		public ReportConfig InsertStyles(string Styles)
		{
			Styles = "<style type='text/css'>" + Styles + "</style>";
			this.report.UpdateSource(this.report.Source.Replace(UnifiedReportFlag.GetPlaceHolder("customcss"), Styles + UnifiedReportFlag.GetPlaceHolder("customcss")));
			return this;
		}

		public ReportConfig AddStylesheet(string StylesheetPath)
		{
			string str = "<link href='file:///" + StylesheetPath + "' rel='stylesheet' type='text/css' />";
			if (StylesheetPath.StartsWith(".") || StylesheetPath.StartsWith("/"))
			{
				str = "<link href='" + StylesheetPath + "' rel='stylesheet' type='text/css' />";
			}
			this.report.UpdateSource(this.report.Source.Replace(UnifiedReportFlag.GetPlaceHolder("customcss"), str + UnifiedReportFlag.GetPlaceHolder("customcss")));
			return this;
		}

		public ReportConfig ReportHeadline(string Headline)
		{
			int num = 70;
			if (Headline.Length >= num)
			{
				Headline = Headline.Substring(0, num - 1);
			}
			string text = UnifiedReportFlag.GetPlaceHolder("headline") + ".*" + UnifiedReportFlag.GetPlaceHolder("headline");
			Headline = text.Replace(".*", Headline);
			this.report.UpdateSource(this.report.Source.Replace(RegexMatcher.GetNthMatch(this.report.Source, text, 0), Headline));
			return this;
		}

        public ReportConfig ReportRightSideText(string Headline)
        {
            int num = 30;
            if (Headline.Length >= num)
            {
                Headline = Headline.Substring(0, num - 1);
            }
            string text = UnifiedReportFlag.GetPlaceHolder("rightsidetext") + ".*" + UnifiedReportFlag.GetPlaceHolder("rightsidetext");
            Headline = text.Replace(".*", Headline);
            this.report.UpdateSource(this.report.Source.Replace(RegexMatcher.GetNthMatch(this.report.Source, text, 0), Headline));
            return this;
        }

        public ReportConfig ReportName(string Name)
		{
			int num = 20;
			if (Name.Length >= num)
			{
				Name = Name.Substring(0, num - 1);
			}
			string text = UnifiedReportFlag.GetPlaceHolder("logo") + ".*" + UnifiedReportFlag.GetPlaceHolder("logo");
			Name = text.Replace(".*", Name);
			this.report.UpdateSource(this.report.Source.Replace(RegexMatcher.GetNthMatch(this.report.Source, text, 0), Name));
			return this;
		}

		public ReportConfig DocumentTitle(string Title)
		{
			string text = "<title>.*</title>";
			this.report.UpdateSource(this.report.Source.Replace(RegexMatcher.GetNthMatch(this.report.Source, text, 0), text.Replace(".*", Title)));
			return this;
		}

		internal ReportConfig(ReportInstance report)
		{
			this.report = report;
		}
	}
}
