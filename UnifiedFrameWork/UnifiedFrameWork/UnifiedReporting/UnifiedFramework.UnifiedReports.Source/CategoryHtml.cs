using System;

namespace UnifiedFramework.UnifiedReports.Source
{
	internal class CategoryHtml
	{
		public static string GetOptionSource()
		{
			return "<option value='<!--%%TESTCATEGORYU%%-->'><!--%%TESTCATEGORY%%--></option>";
		}

		public static string GetCategoryViewSource()
		{
			return "<div class='col s12 m12 l12'><div class='card-panel category-view'><div class='category-header test-attributes'><span class='category'><!--%%CATEGORYVIEWNAME%%--></span><div class='category-status right'><span class='cat-pass'>PASS: </span><span class='cat-fail'>FAIL: </span><span class='cat-other'>OTHER: </span></div></div><table class='bordered'><tr><th>Run Date</th><th>Test Name</th><th>Status</th></tr><!--%%CATEGORYVIEWTESTDETAILS%%--></table></div></div>";
		}

		public static string GetCategoryViewTestSource()
		{
			return "<tr><td><!--%%CATEGORYVIEWTESTRUNTIME%%--></td><td><span class='category-link'><!--%%CATEGORYVIEWTESTNAME%%--></span></td><td><div class='label <!--%%CATEGORYVIEWTESTSTATUS%%-->'><!--%%CATEGORYVIEWTESTSTATUS%%--></div></td></tr>";
		}
	}
}
