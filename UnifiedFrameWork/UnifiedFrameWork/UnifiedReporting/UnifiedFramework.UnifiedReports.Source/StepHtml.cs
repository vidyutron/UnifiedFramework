using System;

namespace UnifiedFramework.UnifiedReports.Source
{
	internal class StepHtml
	{
		public static string GetSource(int ColSpan)
		{
			string result;
			if (ColSpan == 2)
			{
				result = "<tr><td><!--%%TIMESTAMP%%--></td><td title='<!--%%STEPSTATUSU%%-->' class='status <!--%%STEPSTATUS%%-->'><i class='fa fa-<!--%%STATUSICON%%-->'></i></td><td class='step-details' colspan='2'><!--%%DETAILS%%--></td></tr>";
			}
			else
			{
				result = "<tr><td><!--%%TIMESTAMP%%--></td><td title='<!--%%STEPSTATUSU%%-->' class='status <!--%%STEPSTATUS%%-->'><i class='fa fa-<!--%%STATUSICON%%-->'></i></td><td class='step-name'><!--%%STEPNAME%%--></td><td class='step-details'><!--%%DETAILS%%--></td></tr>";
			}
			return result;
		}
	}
}
