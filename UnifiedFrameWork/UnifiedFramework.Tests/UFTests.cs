using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System.Threading;
using Winium.Desktop.Driver;
using System.IO;
using UnifiedFrameWork.ControllerLayer;
using System.Collections.Generic;
using UnifiedFrameWork.UCodeGenerator;
using UnifiedFrameWork.UComponents;

namespace UnifiedFramework.Tests
{
    [TestClass]
    public class UFTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dc = new DesiredCapabilities();
            dc.SetCapability("app", @"C:/windows/system32/notepad.exe");
            var driver = new RemoteWebDriver(new Uri("http://localhost:9999"), dc);
            var windowNotepad = driver.FindElementByClassName("Notepad");
            windowNotepad.FindElement(By.ClassName("Edit")).SendKeys("Type in Notepad through Automation");
            windowNotepad.FindElement(By.Name("File")).Click();
            windowNotepad.FindElement(By.Name("Save As...")).Click();
        }

        [TestMethod]
        public void testmethod()
        {
            UCodeGen.Initiate();
            string nameSpace = "UnifiedFramework.Tests";
            string baseType = "Configurations";
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            List<string> newFilePaths = new List<string>();
            UCodeGenController codeEngine = new UCodeGenController(nameSpace, baseType);
            codeEngine.UCodeConfigGen(filePath, "Config");

            codeEngine = new UCodeGenController(nameSpace, "GenericOne", baseType);
            codeEngine.UCodeBuilder(filePath, "Common", "Generic");

            codeEngine = new UCodeGenController(nameSpace, "Repository");
            List<string> regionList = new List<string>() { "BusinesOne", "BusinessTwo" };
            codeEngine.UCodeRepository(regionList, 4);
            codeEngine.UCodeBuilder(filePath, "Common", "Repository");

            codeEngine = new UCodeGenController(nameSpace, "BusinessOne", baseType);
            codeEngine.UCodeBuilder(filePath, "Business", "BusinessOne");

            codeEngine = new UCodeGenController(nameSpace, "BusinessTwo", baseType);
            codeEngine.UCodeBuilder(filePath, "Business", "BusinessTwo");


            UCodeGen.Finalise();
        }

        [TestMethod]
        public void testMethodTwo()
        {
            ////Initiates the Unified Code Engine, Should be first line.
            //UCodeGen.Initiate();

            ////Namespace under which c# files will be generated
            //string nameSpace = "MyProjectNamespace";

            ////Primary config file of the project( probably will be inherited by other implmentators, hence is good candidate to act as base class) 
            //string baseType = "Config";

            ////Determine the current projects root file, dynamically. Can be replaced with absolute path but not recommended.
            //string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            ////In this Section we will create Config File(which handles test intialisation, cleanup, assembly cleanups, Logginh, report generation etc.)
            ////Invoke Unified Code Engine:Dynamic code creation fragment, using Namespace and Config file Name defined above.
            //UCodeGenController codeEngine = new UCodeGenController(nameSpace, baseType);
            ////Invoke Unified Code Config Generator:writing generated code to IOStream(here its c# file), using file path and classnme
            ////Eg. here we intend to create file named as Config, so baseType is passed as second parameter.
            //codeEngine.UCodeConfigGen(filePath, baseType);

            ////In this section we will create a single file inside a folder.
            ////Invoke Unified Code Engine, using Namespace, folder name, and baseclass 
            //codeEngine = new UCodeGenController(nameSpace, "SampleFileOne", baseType);
            ////Invoke Unified Code Generator, apart from creation of config file. this way should be followed to create c# files
            ////Here we need to passs file path, folder name, class name.
            //codeEngine.UCodeBuilder(filePath, "FolderOne", "SampleFileOne");


            ////In this Section we will be creating two file under single folder
            //codeEngine = new UCodeGenController(nameSpace, "SampleFileTwo", baseType);
            //codeEngine.UCodeBuilder(filePath, "FolderTwo", "SampleFileTwo");
            //codeEngine = new UCodeGenController(nameSpace, "SampleFileThree", baseType);
            //codeEngine.UCodeBuilder(filePath, "FolderTwo", "SampleFileThree");

            //UCodeGen.Finalise();

            //Give fully formed URL(Web Page address) of which you intend to extract elements
            //example-http://www.vishalsridhar.com/unifiedframework instead of vishalsridhar.com/unifiedframework 
            var url = "http://www.mikesdotnetting.com/article/273/using-the-htmlagilitypack-to-parse-html-in-asp-net";

            //pass on the Url defined in previous step to Extract the elements.
            //By default it will create UFHtmlExtractor Folder in the project file path and relevant json file inside that(based on the URL passed)
            //** UnifiedFramework will automatically add the newly created file to the solution, so that you could refer within IDE itself.
            UnifiedHtmlExtractor.ExtractElements(url);
        }


    }
}
