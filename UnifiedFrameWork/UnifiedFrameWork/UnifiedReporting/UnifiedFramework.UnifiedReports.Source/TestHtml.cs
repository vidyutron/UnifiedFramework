using System;

namespace UnifiedFramework.UnifiedReports.Source
{
	internal class TestHtml
	{
		public static string GetSource(int ColumnCount)
		{
			string str = "";
			if (ColumnCount == 4)
			{
				str = "<th>StepName</th>";
			}
			return "<div class='test-section'><div class='col s12'><div class='test card-panel displayed <!--%%TESTSTATUS%%-->'><div class='test-head'><div class='right test-info'><span alt='Test started time' title='Test started time' class='test-started-time label'><!--%%TESTSTARTTIME%%--></span><span alt='Test ended time' title='Test ended time' class='test-ended-time label'><!--%%TESTENDTIME%%--></span><span alt='Time taken to finish' title='Time taken to finish' class='test-time-taken label'><!--%%TESTTIMETAKEN%%--></span><span class='test-status label <!--%%TESTSTATUS%%-->'><!--%%TESTSTATUS%%--></span></div><div class='test-name'><!--%%TESTNAME%%--><!--%%TESTWARNINGS%%--></div><div class='test-desc' <!--%%DESCVIS%%-->><span><!--%%TESTDESCRIPTION%%--></span></div></div><div class='test-attributes'><div class='categories'><!--%%TESTCATEGORY%%--></div></div><div class='test-body'><table class='bordered table-results'><thead><tr><th>Timestamp</th><th>Status</th>" + str + "<th>Details</th></tr></thead><tbody><!--%%STEP%%--></tbody></table><ul class='collapsible' data-collapsible='accordion'><!--%%NODELIST%%--></ul></div></div></div></div>";
		}

		public static string GetNodeSource(int cols)
		{
			string str = "";
			if (cols == 4)
			{
				str = "<th>StepName</th>";
			}
			return "<li class='<!--%%NODELEVEL%%-->'><div class='collapsible-header test-node <!--%%NODESTATUS%%-->'><div class='right test-info'><span alt='Test started time' title='Test started time' class='test-started-time label'><!--%%NODESTARTTIME%%--></span><span alt='Test ended time' title='Test ended time' class='test-ended-time label'><!--%%NODEENDTIME%%--></span><span alt='Time taken to finish' title='Time taken to finish' class='test-time-taken label'><!--%%NODETIMETAKEN%%--></span><span class='test-status label <!--%%NODESTATUS%%-->'><!--%%NODESTATUS%%--></span></div><div class='test-node-name'><!--%%NODENAME%%--></div></div><div class='collapsible-body'><div class='test-body'><table class='bordered table-results'><thead><tr><th>Timestamp</th><th>Status</th>" + str + "<th>Details</th></tr></thead><tbody><!--%%NODESTEP%%--></tbody></table></div></div></li>";
		}

		public static string GetSourceQuickView()
		{
			return "<tr><td><span class='quick-view-test'><!--%%TESTNAME%%--></span><!--%%TESTWARNINGS%%--></td><td><!--%%CURRENTTESTPASSEDCOUNT%%--></td><td><!--%%CURRENTTESTFAILEDCOUNT%%--></td><td><!--%%CURRENTTESTFATALCOUNT%%--></td><td><!--%%CURRENTTESTERRORCOUNT%%--></td><td><!--%%CURRENTTESTWARNINGCOUNT%%--></td><td><!--%%CURRENTTESTINFOCOUNT%%--></td><td><!--%%CURRENTTESTSKIPPEDCOUNT%%--></td><td><!--%%CURRENTTESTUNKNOWNCOUNT%%--></td><td><div class='status <!--%%CURRENTTESTRUNSTATUS%%--> label'><!--%%CURRENTTESTRUNSTATUSU%%--></div></td></tr>";
		}

		public static string GetCategorySource()
		{
			return "<span class='category'><!--%%CATEGORY%%--></span>";
		}

		public static string GetWarningSource(string Warning)
		{
			string result;
			if (Warning == "")
			{
				result = "";
			}
			else
			{
				result = string.Concat(new string[]
				{
					"<span class='test-warning tooltipped' data-tooltip='",
					Warning,
					"' data-position='top'><i class='fa fa-info' alt='",
					Warning,
					"' title='",
					Warning,
					"'></i></span>"
				});
			}
			return result;
		}
	}
}
