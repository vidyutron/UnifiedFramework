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


    public class SampleFileOne : Config
    {
        internal static void ValidateScenarioOne()
        {
            driver.Navigate().GoToUrl("http://www.convert-jpg-to-pdf.net/");
            UnifiedWebControl.ClickIdElement(ConvertToPdf.Id[16].value);

            var fileToUpload = winiumDriver.FindElementByName("Choose File to Upload");
            fileToUpload.FindElement(By.ClassName("Edit")).SendKeys(@"C:\Users\v-dwarya\Pictures\anniversary.jpg");
            fileToUpload.FindElement(By.Name("Open")).Click();
        }
    }
}
