using UnifiedFramework.UnifiedReports.Source;
using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports
{
	internal class SourceBuilder
	{
		public static string BuildRegex(string Source, string[] Flags, string[] Values)
		{
			for (int i = 0; i < Flags.Length; i++)
			{
				string text = Flags[i] + ".*" + Flags[i];
				string nthMatch = RegexMatcher.GetNthMatch(Source, text, 0);
				if (nthMatch == null)
				{
					Source = Source.Replace(Flags[i], Values[i]);
				}
				else
				{
					Source = Source.Replace(nthMatch, text.Replace(".*", Values[i]));
				}
			}
			return Source;
		}

		public static string Build(string source, string[] flags, string[] values)
		{
			for (int i = 0; i < flags.Length; i++)
			{
				source = source.Replace(flags[i], values[i]);
			}
			return source;
		}

		public static string GetSource(Dictionary<string, string> Info)
		{
			string text = string.Empty;
			foreach (KeyValuePair<string, string> current in Info)
			{
				text += SystemInfoHtml.GetColumn();
				text = text.Replace(UnifiedReportFlag.GetPlaceHolder("systemInfoParam"), current.Key).Replace(UnifiedReportFlag.GetPlaceHolder("systemInfoValue"), current.Value);
			}
			return text;
		}
	}
}
