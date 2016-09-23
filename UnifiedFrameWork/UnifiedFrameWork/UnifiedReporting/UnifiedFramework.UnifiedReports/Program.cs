using System;

namespace UnifiedFramework.UnifiedReports
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			UnifiedReports extentReports = new UnifiedReports("C:\\Users\\Anshoo\\Documents\\workspace\\extent2examples\\Extent.NET.html", true, DisplayOrder.OldestFirst);
			UnifiedTest extentTest = extentReports.StartTest("Test 1", "");
			extentTest.Log(LogStatus.Error, "Error");
			extentReports.EndTest(extentTest);
			extentTest = extentReports.StartTest("Nodes", "");
			UnifiedTest extentTest2 = extentReports.StartTest("Child 1", "");
			extentTest2.Log(LogStatus.Pass, "Pass");
			extentTest2.Log(LogStatus.Pass, "Pass");
			UnifiedTest extentTest3 = extentReports.StartTest("Child 2", "");
			extentTest3.Log(LogStatus.Info, "info");
			extentTest3.Log(LogStatus.Info, "info");
			extentTest.AppendChild(extentTest2);
			extentTest.AppendChild(extentTest3);
			extentReports.EndTest(extentTest);
			extentTest = extentReports.StartTest("Nodes", "");
			extentTest2 = extentReports.StartTest("Child 1", "");
			extentTest2.Log(LogStatus.Error, "Error" + extentTest2.AddScreenCapture("1.png"));
			extentTest2.Log(LogStatus.Pass, "Pass");
			extentTest3 = extentReports.StartTest("Child 2", "");
			extentTest3.Log(LogStatus.Info, "info");
			extentTest3.Log(LogStatus.Info, "info");
			extentTest.AppendChild(extentTest2);
			extentTest.AppendChild(extentTest3);
			extentReports.EndTest(extentTest);
			extentTest = extentReports.StartTest("Nodes", "");
			extentTest2 = extentReports.StartTest("Child 1", "");
			extentTest2.Log(LogStatus.Pass, "Pass");
			extentTest2.Log(LogStatus.Pass, "Pass");
			extentTest3 = extentReports.StartTest("Child 2", "");
			extentTest3.Log(LogStatus.Info, "info");
			extentTest3.Log(LogStatus.Error, "error");
			extentTest2.AppendChild(extentTest3);
			extentTest.AppendChild(extentTest2);
			extentReports.EndTest(extentTest);
			extentReports.Flush();
		}
	}
}
