//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImplementUnifiedFramework
{
    using  System;
    using  System.Collections.Generic;
    using  System.Linq;
    using  System.Text;
    using  System.Threading.Tasks;
    using  OpenQA.Selenium;
    using  OpenQA.Selenium.IE;
    using  OpenQA.Selenium.Support.UI;
    using  Microsoft.VisualStudio.TestTools.UnitTesting;
    using  System.Diagnostics;
    using  System.IO;
    using  System.Configuration;
    using  System.Threading;
    using  UnifiedFramework.UnifiedReports;
    using  UnifiedFrameWork.Controller;
    using  System.Text.RegularExpressions;
    using  Winium.Desktop.Driver;
    using  OpenQA.Selenium.Remote;
    using  UnifiedFrameWork.UnifiedComponents;
    
    
    [TestClass()]
    public class Config
    {
        
        public static OpenQA.Selenium.IWebDriver driver;
        
        public static string textFile;
        
        public static UnifiedFramework.UnifiedReports.UnifiedReports unifiedReport;
        
        public static UnifiedFramework.UnifiedReports.UnifiedTest unifiedTestLog;
        
        public static OpenQA.Selenium.Support.UI.WebDriverWait wait;
        
        public static UnifiedFrameWork.Controller.UComponentController unifiedComponent;
        
        public static List<UnifiedFramework.UnifiedReports.UnifiedTest> unifiedLogCollection;
        
        public virtual UnifiedFramework.UnifiedReports.UnifiedReports Instance
        {
            get
            {
                if (unifiedReport==null)
                {
                    string reportFilePath = UnifiedFrameWork.Controller.DirectoryHandler.DirectoryCreation("UnifiedReport");
                    unifiedReport= new UnifiedReports(Path.Combine(reportFilePath,"UnifiedReports"+".html"), true);
                    unifiedReport.Config().DocumentTitle("UReportsTitle").ReportName("UnifiedReports").ReportHeadline("UReportsHeadline");
                }
                return unifiedReport;
            }
        }
        
        [TestInitialize()]
        public void TestIntialise()
        {
            #region Code Injection
            ClearBrowser();
            string ieServerFilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "UnifiedTools", "IEWebDriver");
            driver = UnifiedWebControlConfig.IWebDriverConfig(ieServerFilePath);
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();
            wait = UnifiedWebControlConfig.ExplicitWaitConfig(driver, 60);
            unifiedReport = Instance;
            unifiedLogCollection = new List<UnifiedTest>();
            WiniumDriver.WiniumInitiate();
            #endregion
        }
        
        [TestCleanup()]
        public void TestCleanUp()
        {
            #region Code Injection
	    for (int i = 0; i < unifiedLogCollection.Count; i++)
            {
                if (unifiedLogCollection[i] != null)
                {
                    unifiedReport.EndTest(unifiedLogCollection[i]);
                    unifiedReport.Flush();
                    unifiedLogCollection[i] = null;
                }
            }
            unifiedLogCollection.Clear();
            //WiniumDriver.WiniumFinalise();
            driver.Dispose();
            ClearBrowser();
            #endregion
        }
        
        public void ClearBrowser()
        {
            #region Code Injection
            Console.Write("Parent Class");
            Process p = new Process();
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = "cmd.exe";
            ps.Arguments = "/C RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 4351";
            p.StartInfo = ps;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit(30000);
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in Process.GetProcesses())
            {
                if (theprocess.ProcessName == "iexplore")
                {
                    theprocess.Kill();
                }
            }
            #endregion
        }
    }
}
