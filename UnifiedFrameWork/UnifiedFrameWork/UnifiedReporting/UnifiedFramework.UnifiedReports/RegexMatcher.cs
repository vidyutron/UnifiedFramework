using System;
using System.Text.RegularExpressions;

namespace UnifiedFramework.UnifiedReports
{
	internal class RegexMatcher
	{
		public static string GetNthMatch(string Text, string Pattern, int MatchNumber)
		{
			GroupCollection match = RegexMatcher.GetMatch(Text, Pattern);
            return match != null ? match[MatchNumber].ToString() : null;
        }

		public static GroupCollection GetMatch(string Text, string Pattern)
		{
			Match match = Regex.Match(Text, Pattern);
            return match.Success ? match.Groups : null;
		}
	}
}
