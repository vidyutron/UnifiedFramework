using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using UnifiedFrameWork.Controller;
using System.Collections.Generic;
using UnifiedFrameWork.UCodeGenerator;
using UnifiedFrameWork.UnifiedComponents;
using System.Linq;

namespace UnifiedFramework.Implement
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Initiates the Unified Code Engine, Should be first line.
            UCodeGen.Initiate();

            //Namespace under which c# files will be generated
            string nameSpace = "ImplementUnifiedFramework";

            //Primary config file of the project( probably will be inherited by other implmentators, hence is good candidate to act as base class) 
            string baseType = "Config";

            //Determine the current projects root file, dynamically. Can be replaced with absolute path but not recommended.
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            //In this Section we will create Config File(which handles test intialisation, cleanup, assembly cleanups, Logginh, report generation etc.)
            //Invoke Unified Code Engine:Dynamic code creation fragment, using Namespace and Config file Name defined above.
            UCodeGenController codeEngine = new UCodeGenController(nameSpace, baseType);
            //Invoke Unified Code Config Generator:writing generated code to IOStream(here its c# file), using file path and classnme
            //Eg. here we intend to create file named as Config, so baseType is passed as second parameter.
            codeEngine.UCodeConfigGen(filePath, baseType);

            //In this section we will create a single file inside a folder.
            //Invoke Unified Code Engine, using Namespace, folder name, and baseclass 
            codeEngine = new UCodeGenController(nameSpace, "SampleFileOne", baseType);
            //Invoke Unified Code Generator, apart from creation of config file. this way should be followed to create c# files
            //Here we need to passs file path, folder name, class name.
            codeEngine.UCodeBuilder(filePath, "FolderOne", "SampleFileOne");


            ////In this Section we will be creating two file under single folder
            codeEngine = new UCodeGenController(nameSpace, "SampleFileTwo", baseType);
            codeEngine.UCodeBuilder(filePath, "FolderTwo", "SampleFileTwo");
            codeEngine = new UCodeGenController(nameSpace, "SampleFileThree", baseType);
            codeEngine.UCodeBuilder(filePath, "FolderTwo", "SampleFileThree");


            //In this section we will create Primary test method class with, by reading exisitng test case list(in excel format)
            //You must supply either ur Actual/Dummy Test Case excel sheet, or may skip this section completely.
            //To read from excel sheet, we will be using Unified's internal ExcelReading Component 
            //To read excel sheet seamlessly it depends on user inputs so that it can adapt to the excel sheet and extract suitable informations.
            //Input Required from user--
            //1. filePath- Test case Excel sheet's file path
            //2. TestCaseFileName- Excel Workbook's file name.
            //3. SheetName- Excel Sheet's name.
            //4. breakString[index], where index= (actual column number -1) - Supply index pointing to columns which has relaevant information matching to left side string
            //eg. "TestCategory",breakString[3]- implies that 4th colum has Test Category or any other Relevant information.

            UComponentController component = new UComponentController();
            List<string> uCodeExcelRead;
            uCodeExcelRead = component.ExcelReader(filePath, "UFTestExcelSheet", "Sheet1");
            List<Dictionary<string, string>> uCodeMethodAttribute = new List<Dictionary<string, string>>();
            codeEngine = new UCodeGenController(nameSpace, "BVT_TestMethod", baseType, "TestClass");
            for (int i = 0; i < uCodeExcelRead.Count; i++)
            {
                if (i != 0)
                {
                    List<string> breakString = uCodeExcelRead[i].Split(new string[] { "-!-" }, StringSplitOptions.None).ToList();
                    Dictionary<string, string> uCodeTempDictionary = new Dictionary<string, string>() {
                        { "TestMethod", "" }, { "TestCategory", breakString[1] }, { "Description", breakString[2] } };
                    codeEngine.UCodeAddTestMethod("Verify_Test_Case_" + breakString[0], uCodeTempDictionary);
                }
            }
            codeEngine.UCodeBuilder(filePath, "TestMethod", "BVT_TestMethod");

            //Finalises the Unified Code Generator, should be the last step.
            UCodeGen.Finalise();
        }

        [TestMethod]
        public void TestMethod2()
        {
            //Give fully formed URL(Web Page address) of which you intend to extract elements
            //example-http://www.vishalsridhar.com/unifiedframework instead of vishalsridhar.com/unifiedframework 
            var url = "http://www.convert-jpg-to-pdf.net/";

            //pass on the Url defined in previous step to Extract the elements.
            //By default it will create UFHtmlExtractor Folder in the project file path and relevant json file inside that(based on the URL passed)
            //** UnifiedFramework will automatically add the newly created file to the solution, so that you could refer within IDE itself.
            UnifiedHtmlExtractor.ExtractElements(url);
        }

        [TestMethod]
        public void TestMethod3()
        {

        }
      }
}
