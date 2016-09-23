using UnifiedFramework.UnifiedReports.Model;
using UnifiedFramework.UnifiedReports.Source;
using System;

namespace UnifiedFramework.UnifiedReports
{
	internal class TestBuilder
	{
		public static string GetSource(Test test)
		{
			string result;
			if (test.IsChildNode)
			{
				result = string.Empty;
			}
			else
			{
				string text = TestHtml.GetSource(3);
				string source = StepHtml.GetSource(2);
				if (test.Logs.Count > 0 && test.Logs[0].StepName != "")
				{
					text = TestHtml.GetSource(4);
					source = StepHtml.GetSource(-1);
				}
				if (string.IsNullOrEmpty(test.Description))
				{
					text = text.Replace(UnifiedReportFlag.GetPlaceHolder("descVis"), "style='display:none;'");
				}
				string[] flags = new string[]
				{
					UnifiedReportFlag.GetPlaceHolder("testName"),
					UnifiedReportFlag.GetPlaceHolder("testStatus"),
					UnifiedReportFlag.GetPlaceHolder("testStartTime"),
					UnifiedReportFlag.GetPlaceHolder("testEndTime"),
					UnifiedReportFlag.GetPlaceHolder("testTimeTaken"),
					UnifiedReportFlag.GetPlaceHolder("testDescription"),
					UnifiedReportFlag.GetPlaceHolder("descVis"),
					UnifiedReportFlag.GetPlaceHolder("category"),
					UnifiedReportFlag.GetPlaceHolder("testWarnings")
				};
				string[] values = new string[]
				{
					test.Name,
					test.Status.ToString().ToLower(),
					test.StartedTime.ToString(),
					test.EndedTime.ToString(),
					string.Concat(new object[]
					{
						(test.EndedTime - test.StartedTime).Minutes,
						"m ",
						(test.EndedTime - test.StartedTime).Seconds,
						"s"
					}),
					test.Description,
					"",
					"",
					TestHtml.GetWarningSource(test.InternalWarning)
				};
				text = SourceBuilder.Build(text, flags, values);
				foreach (TestAttribute current in test.CategoryList)
				{
					text = text.Replace(UnifiedReportFlag.GetPlaceHolder("testCategory"), TestHtml.GetCategorySource() + UnifiedReportFlag.GetPlaceHolder("testCategory")).Replace(UnifiedReportFlag.GetPlaceHolder("category"), current.GetName());
				}
				string source2 = StepHtml.GetSource(2);
				string[] flags2 = new string[]
				{
					UnifiedReportFlag.GetPlaceHolder("step"),
					UnifiedReportFlag.GetPlaceHolder("timeStamp"),
					UnifiedReportFlag.GetPlaceHolder("stepStatusU"),
					UnifiedReportFlag.GetPlaceHolder("stepStatus"),
					UnifiedReportFlag.GetPlaceHolder("statusIcon"),
					UnifiedReportFlag.GetPlaceHolder("stepName"),
					UnifiedReportFlag.GetPlaceHolder("details")
				};
				if (test.Logs.Count > 0)
				{
					if (!string.IsNullOrEmpty(test.Logs[0].StepName))
					{
						source2 = StepHtml.GetSource(3);
					}
					for (int i = 0; i < test.Logs.Count; i++)
					{
						string[] values2 = new string[]
						{
							source2 + UnifiedReportFlag.GetPlaceHolder("step"),
							test.Logs[i].Timestamp.ToShortTimeString(),
							test.Logs[i].LogStatus.ToString().ToUpper(),
							test.Logs[i].LogStatus.ToString().ToLower(),
							Icon.GetIcon(test.Logs[i].LogStatus),
							test.Logs[i].StepName,
							test.Logs[i].Details
						};
						text = SourceBuilder.Build(text, flags2, values2);
					}
				}
				text = text.Replace(UnifiedReportFlag.GetPlaceHolder("step"), "");
				text = TestBuilder.AddChildTests(test, text, 1);
				result = text;
			}
			return result;
		}

		private static string AddChildTests(Test test, string testSource, int nodeLevel)
		{
			string[] flags = new string[]
			{
				UnifiedReportFlag.GetPlaceHolder("nodeList"),
				UnifiedReportFlag.GetPlaceHolder("nodeName"),
				UnifiedReportFlag.GetPlaceHolder("nodeStartTime"),
				UnifiedReportFlag.GetPlaceHolder("nodeEndTime"),
				UnifiedReportFlag.GetPlaceHolder("nodeTimeTaken"),
				UnifiedReportFlag.GetPlaceHolder("nodeLevel")
			};
			string[] flags2 = new string[]
			{
				UnifiedReportFlag.GetPlaceHolder("nodeStep"),
				UnifiedReportFlag.GetPlaceHolder("timeStamp"),
				UnifiedReportFlag.GetPlaceHolder("stepStatusU"),
				UnifiedReportFlag.GetPlaceHolder("stepStatus"),
				UnifiedReportFlag.GetPlaceHolder("statusIcon"),
				UnifiedReportFlag.GetPlaceHolder("stepName"),
				UnifiedReportFlag.GetPlaceHolder("details")
			};
			foreach (Test current in test.NodeList)
			{
				string nodeSource = TestHtml.GetNodeSource(3);
				if (current.Logs.Count > 0 && current.Logs[0].StepName != "")
				{
					nodeSource = TestHtml.GetNodeSource(4);
				}
				string[] values = new string[]
				{
					nodeSource + UnifiedReportFlag.GetPlaceHolder("nodeList"),
					current.Name,
					current.StartedTime.ToString(),
					current.EndedTime.ToString(),
					string.Concat(new object[]
					{
						(current.EndedTime - current.StartedTime).Minutes,
						"m ",
						(current.EndedTime - current.StartedTime).Seconds,
						"s"
					}),
					"node-" + nodeLevel + "x"
				};
				testSource = SourceBuilder.Build(testSource, flags, values);
				if (current.Logs.Count > 0)
				{
					testSource = testSource.Replace(UnifiedReportFlag.GetPlaceHolder("nodeStatus"), current.Status.ToString().ToLower());
					string source = StepHtml.GetSource(2);
					if (current.Logs[0].StepName != "")
					{
						source = StepHtml.GetSource(3);
					}
					for (int i = 0; i < current.Logs.Count; i++)
					{
						string[] values2 = new string[]
						{
							source + UnifiedReportFlag.GetPlaceHolder("nodeStep"),
							current.Logs[i].Timestamp.ToShortTimeString(),
							current.Logs[i].LogStatus.ToString().ToUpper(),
							current.Logs[i].LogStatus.ToString().ToLower(),
							Icon.GetIcon(current.Logs[i].LogStatus),
							current.Logs[i].StepName,
							current.Logs[i].Details
						};
						testSource = SourceBuilder.Build(testSource, flags2, values2);
					}
				}
				testSource = SourceBuilder.Build(testSource, new string[]
				{
					UnifiedReportFlag.GetPlaceHolder("step"),
					UnifiedReportFlag.GetPlaceHolder("nodeStep")
				}, new string[]{string.Empty,string.Empty}
                );
				if (current.HasChildNodes)
				{
					testSource = TestBuilder.AddChildTests(current, testSource, ++nodeLevel);
					nodeLevel--;
				}
			}
			return testSource;
		}

		public static string GetQuickSummary(Test test)
		{
			string result;
			if (test.IsChildNode)
			{
				result = string.Empty;
			}
			else
			{
				string text = TestHtml.GetSourceQuickView();
				LogCounts logCounts = new LogCounts().GetLogCounts(test);
				string[] flags = new string[]
				{
					UnifiedReportFlag.GetPlaceHolder("testName"),
					UnifiedReportFlag.GetPlaceHolder("testWarnings"),
					UnifiedReportFlag.GetPlaceHolder("currentTestPassedCount"),
					UnifiedReportFlag.GetPlaceHolder("currentTestFailedCount"),
					UnifiedReportFlag.GetPlaceHolder("currentTestFatalCount"),
					UnifiedReportFlag.GetPlaceHolder("currentTestErrorCount"),
					UnifiedReportFlag.GetPlaceHolder("currentTestWarningCount"),
					UnifiedReportFlag.GetPlaceHolder("currentTestInfoCount"),
					UnifiedReportFlag.GetPlaceHolder("currentTestSkippedCount"),
					UnifiedReportFlag.GetPlaceHolder("currentTestUnknownCount"),
					UnifiedReportFlag.GetPlaceHolder("currentTestRunStatus"),
					UnifiedReportFlag.GetPlaceHolder("currentTestRunStatusU")
				};
				string[] values = new string[]
				{
					test.Name,
					TestHtml.GetWarningSource(test.InternalWarning),
					logCounts.Pass.ToString(),
					logCounts.Fail.ToString(),
					logCounts.Fatal.ToString(),
					logCounts.Error.ToString(),
					logCounts.Warning.ToString(),
					logCounts.Info.ToString(),
					logCounts.Skip.ToString(),
					logCounts.Unknown.ToString(),
					test.Status.ToString().ToLower(),
					test.Status.ToString().ToUpper()
				};
				text = SourceBuilder.Build(text, flags, values);
				result = text;
			}
			return result;
		}
	}
}
