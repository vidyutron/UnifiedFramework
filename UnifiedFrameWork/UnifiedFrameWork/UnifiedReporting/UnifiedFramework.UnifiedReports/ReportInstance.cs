using UnifiedFramework.UnifiedReports.Model;
using UnifiedFramework.UnifiedReports.Source;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnifiedFramework.UnifiedReports
{
	public class ReportInstance
	{
		private bool terminated = false;

		private AttributeList categoryList;

		private DisplayOrder displayOrder;

		private int infoWrite = 0;

		private string filePath;

		private MediaList mediaList;

		private string quickSummarySource = "";

		private RunInfo runInfo;

		private string unifiedReportSource = null;

		private string testSource = "";

		private object sourcelock = new object();

		internal string Source
		{
			get
			{
				return this.unifiedReportSource;
			}
		}

		internal void AddTest(Test Test)
		{
			Test.EndedTime = DateTime.Now;
			if (Test.ScreenCapture.Count > 0)
			{
				this.mediaList.ScreenCapture.AddRange(Test.ScreenCapture);
			}
			if (Test.Screencast.Count > 0)
			{
				this.mediaList.Screencast.AddRange(Test.Screencast);
			}
			Test.PrepareFinalize();
			this.AddTest(TestBuilder.GetSource(Test));
			this.AddQuickTestSummary(TestBuilder.GetQuickSummary(Test));
			this.AddCategories(Test);
			this.UpdateCategoryView(Test);
		}

		private void AddTest(string TestSource)
		{
			if (this.displayOrder == DisplayOrder.OldestFirst)
			{
				this.testSource += TestSource;
			}
			else
			{
				this.testSource = TestSource + this.testSource;
			}
		}

		private void AddQuickTestSummary(string Summary)
		{
			if (this.displayOrder == DisplayOrder.OldestFirst)
			{
				this.quickSummarySource += Summary;
			}
			else
			{
				this.quickSummarySource = Summary + this.quickSummarySource;
			}
		}

		private void AddCategories(Test test)
		{
			foreach (TestAttribute current in test.CategoryList)
			{
				if (!this.categoryList.Contains((Category)current))
				{
					this.categoryList.Categories.Add(current);
				}
			}
		}

		internal void Initialize(string FilePath, bool ReplaceExisting, DisplayOrder DisplayOrder)
		{
			this.displayOrder = DisplayOrder;
			this.filePath = FilePath;
			if (!File.Exists(FilePath))
			{
				ReplaceExisting = true;
			}
			if (this.unifiedReportSource == null)
			{
				if (ReplaceExisting)
				{
					lock (this.sourcelock)
					{
						this.unifiedReportSource = UnifiedFramework.UnifiedReports.Source.UnifiedReportHtmlTemplate.GetSource();
					}
				}
				else
				{
					lock (this.sourcelock)
					{
						this.unifiedReportSource = File.ReadAllText(FilePath);
					}
				}
				this.runInfo = new RunInfo();
				this.runInfo.StartedTime = DateTime.Now;
				this.categoryList = new AttributeList();
				this.mediaList = new MediaList();
			}
		}

		internal void WriteAllResources(List<UnifiedTest> TestList, SystemInfo SystemInfo)
		{
			if (this.terminated)
			{
				throw new IOException("Stream closed");
			}
			if (SystemInfo != null)
			{
				this.UpdateSystemInfo(SystemInfo.GetInfo());
			}
			if (!(this.testSource == ""))
			{
				this.runInfo.EndedTime = DateTime.Now;
				this.UpdateCategoryList();
				this.UpdateSuiteExecutionTime();
				this.UpdateMediaList();
				if (this.displayOrder == DisplayOrder.OldestFirst)
				{
					lock (this.sourcelock)
					{
						this.unifiedReportSource = SourceBuilder.Build(this.unifiedReportSource, new string[]
						{
							UnifiedReportFlag.GetPlaceHolder("test"),
							UnifiedReportFlag.GetPlaceHolder("quickTestSummary")
						}, new string[]
						{
							this.testSource + UnifiedReportFlag.GetPlaceHolder("test"),
							this.quickSummarySource + UnifiedReportFlag.GetPlaceHolder("quickTestSummary")
						});
					}
				}
				else
				{
					lock (this.sourcelock)
					{
						this.unifiedReportSource = SourceBuilder.Build(this.unifiedReportSource, new string[]
						{
							UnifiedReportFlag.GetPlaceHolder("test"),
							UnifiedReportFlag.GetPlaceHolder("quickTestSummary")
						}, new string[]
						{
							UnifiedReportFlag.GetPlaceHolder("test") + this.testSource,
							UnifiedReportFlag.GetPlaceHolder("quickTestSummary") + this.quickSummarySource
						});
					}
				}
				using (StreamWriter streamWriter = new StreamWriter(this.filePath))
				{
					TextWriter.Synchronized(streamWriter).WriteLine(this.unifiedReportSource);
				}
                this.testSource = string.Empty;
                this.quickSummarySource = string.Empty;
			}
		}

		internal void Terminate(List<UnifiedTest> TestList)
		{
			if (TestList != null)
			{
				foreach (UnifiedTest current in TestList)
				{
					if (!current.GetTest().HasEnded)
					{
						current.GetTest().InternalWarning = "Test did not end safely because endTest() was not called. There may be errors.";
						this.AddTest(current.GetTest());
					}
				}
			}
			this.WriteAllResources(null, null);
			this.unifiedReportSource = string.Empty;
			this.categoryList = null;
			this.runInfo = null;
			this.terminated = true;
		}

		private void UpdateCategoryList()
		{
			string text = string.Empty;
			for (int i = this.categoryList.Categories.Count - 1; i > -1; i--)
			{
				string item = this.categoryList.GetItem(i);
				string placeHolder = UnifiedReportFlag.GetPlaceHolder("categoryAdded" + item);
				if (this.unifiedReportSource.IndexOf(placeHolder) == -1 && text.IndexOf(placeHolder) == -1)
				{
					text += UnifiedReportFlag.GetPlaceHolder("categoryAdded" + item);
				}
				else
				{
					this.categoryList.Categories.RemoveAt(i);
				}
			}
			string text2 = CategoryOptionBuilder.Build(this.categoryList.Categories);
			if (!string.IsNullOrEmpty(text2))
			{
				lock (this.sourcelock)
				{
					this.unifiedReportSource = SourceBuilder.Build(this.unifiedReportSource, new string[]
					{
						UnifiedReportFlag.GetPlaceHolder("categoryListOptions"),
						UnifiedReportFlag.GetPlaceHolder("categoryAdded")
					}, new string[]
					{
						text2 + UnifiedReportFlag.GetPlaceHolder("categoryListOptions"),
						text + UnifiedReportFlag.GetPlaceHolder("categoryAdded")
					});
				}
			}
		}

		private void UpdateCategoryView(Test test)
		{
			string text = string.Empty;
			string[] flags = new string[]
			{
				UnifiedReportFlag.GetPlaceHolder("categoryViewName"),
				UnifiedReportFlag.GetPlaceHolder("categoryViewTestDetails")
			};
			string[] flags2 = new string[]
			{
				UnifiedReportFlag.GetPlaceHolder("categoryViewTestRunTime"),
				UnifiedReportFlag.GetPlaceHolder("categoryViewTestName"),
				UnifiedReportFlag.GetPlaceHolder("categoryViewTestStatus")
			};
			string[] values = new string[]
			{
				test.StartedTime.ToString(),
				test.Name,
				test.Status.ToString().ToLower()
			};
			foreach (TestAttribute current in test.CategoryList)
			{
				string placeHolder = UnifiedReportFlag.GetPlaceHolder("categoryViewTestDetails" + current.GetName());
				if (!this.unifiedReportSource.Contains(placeHolder))
				{
					string[] values2 = new string[]
					{
						current.GetName(),
						placeHolder
					};
					text += SourceBuilder.Build(CategoryHtml.GetCategoryViewSource(), flags, values2);
					string str = SourceBuilder.Build(CategoryHtml.GetCategoryViewTestSource(), flags2, values);
					text = SourceBuilder.Build(text, new string[]
					{
						placeHolder
					}, new string[]
					{
						str + placeHolder
					});
				}
				else
				{
					string str = SourceBuilder.Build(CategoryHtml.GetCategoryViewTestSource(), flags2, values);
					lock (this.sourcelock)
					{
						this.unifiedReportSource = SourceBuilder.Build(this.unifiedReportSource, new string[]
						{
							placeHolder
						}, new string[]
						{
							str + placeHolder
						});
					}
				}
			}
			lock (this.sourcelock)
			{
				this.unifiedReportSource = SourceBuilder.Build(this.unifiedReportSource, new string[]
				{
					UnifiedReportFlag.GetPlaceHolder("extentCategoryDetails")
				}, new string[]
				{
					text + UnifiedReportFlag.GetPlaceHolder("extentCategoryDetails")
				});
			}
		}

		private void UpdateSuiteExecutionTime()
		{
			string[] flags = new string[]
			{
				UnifiedReportFlag.GetPlaceHolder("suiteStartTime"),
				UnifiedReportFlag.GetPlaceHolder("suiteEndTime")
			};
			string[] values = new string[]
			{
				this.runInfo.StartedTime.ToString(),
				this.runInfo.EndedTime.ToString()
			};
			lock (this.sourcelock)
			{
				this.unifiedReportSource = SourceBuilder.BuildRegex(this.unifiedReportSource, flags, values);
			}
		}

		private void UpdateSystemInfo(Dictionary<string, string> SystemInfo)
		{
			if (this.unifiedReportSource.IndexOf(UnifiedReportFlag.GetPlaceHolder("systemInfoApplied")) <= 0)
			{
				if (SystemInfo.Count > 0)
				{
					string str = SourceBuilder.GetSource(SystemInfo) + UnifiedReportFlag.GetPlaceHolder("systemInfoApplied");
					string[] flags = new string[]
					{
						UnifiedReportFlag.GetPlaceHolder("systemInfoView")
					};
					string[] values = new string[]
					{
						str + UnifiedReportFlag.GetPlaceHolder("systemInfoView")
					};
					lock (this.sourcelock)
					{
						this.unifiedReportSource = SourceBuilder.BuildRegex(this.unifiedReportSource, flags, values);
					}
				}
			}
		}

		private void UpdateMediaList()
		{
			string source = MediaViewBuilder.GetSource<ScreenCapture>(this.mediaList.ScreenCapture, "img");
			string[] flags = new string[]
			{
				UnifiedReportFlag.GetPlaceHolder("imagesView")
			};
			string[] array = new string[]
			{
				source + UnifiedReportFlag.GetPlaceHolder("imagesView")
			};
			if (this.infoWrite < 1 || array[0].IndexOf("No media") < 0)
			{
				lock (this.sourcelock)
				{
					this.unifiedReportSource = SourceBuilder.BuildRegex(this.unifiedReportSource, flags, array);
					if (this.mediaList.ScreenCapture.Count > 0)
					{
						try
						{
							string nthMatch = RegexMatcher.GetNthMatch(this.unifiedReportSource, UnifiedReportFlag.GetPlaceHolder("objectViewNullImg") + ".*" + UnifiedReportFlag.GetPlaceHolder("objectViewNullImg"), 0);
							this.unifiedReportSource = this.unifiedReportSource.Replace(nthMatch, "");
						}
						catch
						{
						}
					}
					this.mediaList.ScreenCapture.Clear();
				}
			}
			source = MediaViewBuilder.GetSource<Screencast>(this.mediaList.Screencast, "vid");
			flags = new string[]
			{
				UnifiedReportFlag.GetPlaceHolder("videosView")
			};
			array = new string[]
			{
				source + UnifiedReportFlag.GetPlaceHolder("videosView")
			};
			if (this.infoWrite < 1 || array[0].IndexOf("No media") < 0)
			{
				lock (this.sourcelock)
				{
					this.unifiedReportSource = SourceBuilder.BuildRegex(this.unifiedReportSource, flags, array);
					if (this.mediaList.Screencast.Count > 0)
					{
						try
						{
							string nthMatch = RegexMatcher.GetNthMatch(this.unifiedReportSource, UnifiedReportFlag.GetPlaceHolder("objectViewNullVid") + ".*" + UnifiedReportFlag.GetPlaceHolder("objectViewNullVid"), 0);
							this.unifiedReportSource = this.unifiedReportSource.Replace(nthMatch, "");
						}
						catch
						{
						}
					}
					this.mediaList.Screencast.Clear();
				}
			}
			this.infoWrite++;
		}

		internal void UpdateSource(string NewSource)
		{
			lock (this.sourcelock)
			{
				this.unifiedReportSource = NewSource;
			}
		}
	}
}
