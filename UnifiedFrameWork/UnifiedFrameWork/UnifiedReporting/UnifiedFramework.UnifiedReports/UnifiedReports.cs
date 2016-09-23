using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports
{
	public class UnifiedReports
	{
		private List<UnifiedTest> testList;

		private ReportConfig config;

		private ReportInstance reportInstance;

		private SystemInfo systemInfo;

		public UnifiedReports(string FilePath, bool ReplaceExisting, DisplayOrder DisplayOrder = DisplayOrder.OldestFirst)
		{
			this.reportInstance = new ReportInstance();
			this.reportInstance.Initialize(FilePath, ReplaceExisting, DisplayOrder);
			this.systemInfo = new SystemInfo();
		}

		public UnifiedTest StartTest(string TestName, string Description = "")
		{
			if (this.testList == null)
			{
				this.testList = new List<UnifiedTest>();
			}
			UnifiedTest UnifiedTest = new UnifiedTest(TestName, Description);
			this.testList.Add(UnifiedTest);
			return UnifiedTest;
		}

		public void EndTest(UnifiedTest Test)
		{
			Test.GetTest().HasEnded = true;
			this.reportInstance.AddTest(Test.GetTest());
		}

		public ReportConfig Config()
		{
			if (this.config == null)
			{
				if (this.reportInstance == null)
				{
					throw new Exception("Cannot apply config before UnifiedReports is initialized");
				}
				this.config = new ReportConfig(this.reportInstance);
			}
			return this.config;
		}

		public UnifiedReports AddSystemInfo(Dictionary<string, string> SystemInfo)
		{
			this.systemInfo.SetInfo(SystemInfo);
			return this;
		}

		public UnifiedReports AddSystemInfo(string Param, string Value)
		{
			this.systemInfo.SetInfo(Param, Value);
			return this;
		}

		public void Flush()
		{
			this.removeChildTests();
			this.reportInstance.WriteAllResources(this.testList, this.systemInfo);
			if (this.testList != null)
			{
				this.systemInfo.Clear();
			}
		}

		public void Close()
		{
			this.removeChildTests();
			this.Flush();
			this.reportInstance.Terminate(this.testList);
			if (this.testList != null)
			{
				this.testList.Clear();
			}
		}

		private void removeChildTests()
		{
			if (this.testList != null)
			{
				for (int i = this.testList.Count - 1; i > -1; i--)
				{
					if (this.testList[i].GetTest().IsChildNode)
					{
						this.testList.RemoveAt(i);
					}
				}
			}
		}
	}
}
