using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnifiedFrameWork.UnifiedComponents;
using UnifiedFrameWork.Controller;
using System.IO;
using UnifiedFrameWork.UCodeGenerator;

namespace UnifiedFramework.Tests
{
    [TestClass]
    public class UnifiedTests
    {
        [TestMethod]
        public void TestMethod1()
        {

            //Give fully formed URL(Web Page address) of which you intend to extract elements
            //example-http://www.vishalsridhar.com/unifiedframework instead of vishalsridhar.com/unifiedframework 
            var url = "http://www.mikesdotnetting.com/article/273/using-the-htmlagilitypack-to-parse-html-in-asp-net";

            //pass on the Url defined in previous step to Extract the elements.
            //By default it will create UFHtmlExtractor Folder in the project file path and relevant json file inside that(based on the URL passed)
            //** UnifiedFramework will automatically add the newly created file to the solution, so that you could refer within IDE itself.
            UnifiedHtmlExtractor.ExtractElements(url);
        }

        [TestMethod]
        public void TestMethod2()
        {
            //Initiates the Unified Code Engine, Should be first line.
            UCodeGen.Initiate();

            //Namespace under which c# files will be generated
            string nameSpace = "UnifiedFramework.Tests";

            //Primary config file of the project( probably will be inherited by other implmentators, 
            //hence is good candidate to act as base class) 
            string baseType = "Config";

            //Determine the current projects root file, dynamically. Can be replaced with absolute path but not recommended.
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            UCodeGenController codeEngine = new UCodeGenController(nameSpace, baseType);
            codeEngine.UCodeConfigGen(filePath, baseType,"ChromeDriver");

            //Finalises the Unified Code Generator, should be the last step.
            UCodeGen.Finalise();

        }
     }
}
