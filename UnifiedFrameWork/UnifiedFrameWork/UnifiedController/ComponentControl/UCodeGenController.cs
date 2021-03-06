﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedFrameWork.UCodeGenerator;

namespace UnifiedFrameWork.Controller
{
    public class UCodeGenController
    {
        private UCodeEngine codeEngine;
        private string snippetParentFolder= "UnifiedResources";
        static List<string> localUsingCollection = new List<string>() {" System"
            ," System.Collections.Generic"
            ," System.Linq"
            ," System.Text"
            ," System.Threading.Tasks"
            ," OpenQA.Selenium"
            ," OpenQA.Selenium.IE"
            ," OpenQA.Selenium.Support.UI"
            ," Microsoft.VisualStudio.TestTools.UnitTesting"
            ," System.Diagnostics"
            ," System.IO"
            ," System.Configuration"
            ," System.Threading"
            ," UnifiedFramework.UnifiedReports"
            ," UnifiedFrameWork.Controller"
            ," System.Text.RegularExpressions"
            ," Winium.Desktop.Driver"
            ," OpenQA.Selenium.Remote"
            ," UnifiedFrameWork.UnifiedComponents"
        };
        private string nameSpace;
        private string className;

        public object AutomatedAddFiles { get; private set; }

        public UCodeGenController(string nameSpace, List<string> usingCollection, string className,
            string classAttribute = "", string baseType = "")
        {
            this.nameSpace = nameSpace;
            this.className = className;
            if (usingCollection != null)
            {
                if (usingCollection.Count > 0)
                    codeEngine = new UCodeEngine(nameSpace, usingCollection, className, classAttribute, baseType);
            }
            else
                codeEngine = new UCodeEngine(nameSpace, localUsingCollection, className, classAttribute, baseType);
        }

        public UCodeGenController(string nameSpace,string ClassName) : this(nameSpace, localUsingCollection, ClassName,"",""){}
        public UCodeGenController(string nameSpace, string ClassName,string baseType) : this(nameSpace, localUsingCollection, ClassName,"",baseType){}
        public UCodeGenController(string nameSpace, string ClassName, string baseType, string classAttribute) : this(nameSpace, localUsingCollection, ClassName,classAttribute, baseType) { }

        public void UCodeAddMembers(Dictionary<string,string> memberCollection)
        {
            codeEngine.UCodeAddMembers(memberCollection);
        }

        public void UcodeAddCollectionMembers(string collectionType,string collectionName,string dataTypeOne,string dataTypeTwo="")
        {
            codeEngine.UCodeAddCollectionMembers(collectionType, collectionName, dataTypeOne,dataTypeTwo);
        }

        public void UcodeAddCollectionMembers(string collectionName,
            string dataTypeOne, string dataTypeTwo = "")
        {
            UcodeAddCollectionMembers(string.Empty, collectionName, dataTypeOne, dataTypeTwo);
        }

        public void UCodeAddSnippetMethod(string snippet,string methodName,Dictionary<string,string> methodAttribute=null)
        {
            codeEngine.UCodeAddMethodSnippet(snippet, methodName, methodAttribute);
        }

        public void UCodeAddMethod(string methodName,Dictionary<string,string>methodAtttribute=null,bool isTryCatch=false)
        {
            codeEngine.UCodeAddMethod(methodName, methodAtttribute, isTryCatch);
        }
        
       public void UCodeAddTestMethod(string methodName, Dictionary<string, string> methodAttribute,
            string reportCollection = "unifiedLogCollection", string reportInstance = "unifiedReport")
        {
            codeEngine.UCodeAddTestMethod(methodName, methodAttribute, reportCollection, reportInstance);
        }

        public void UcodeAddProperty(string propertyName, string returnVariable, bool get = true, bool set = true)
        {
            codeEngine.UCodeAddProperties(propertyName, returnVariable, get, set);
        }

        public void UCodeAddReportingProperty(string reportInstance, string reportName, string reportTitle,
            string reportHeadline, string reportFilePath = "", string reportMemberName = "unifiedReport")
        {
            codeEngine.UCodeAddReportProperty(reportInstance, reportName, reportTitle, reportHeadline, reportFilePath, reportMemberName);
        }

        public void UCodeRepository(List<string> regionList,int memberCount=0)
        {
            if (regionList == null)
                codeEngine.UCodeRepository(null, memberCount);
            else
                codeEngine.UCodeRepository(regionList, memberCount);
        }
        public void UCodeBuilder(string filePath, string directory, string className)
        {
            codeEngine.UCodeGenerator(filePath, directory, className);
            UCodeGen.FilesToInclude.Add(Path.Combine(directory, className));
            //string projectName = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().Location).Replace(".dll","");
            //AutomateAddFile.IncludeInProject(projectName, Path.Combine(filePath,Directory, className + ".cs"));
        }

        public void UCodeConfigGen(string filePath,string directory)
        {
            string testIntialiseCodeSnippet = File.ReadAllText("..//..//"+ snippetParentFolder + "//testIntializeSnipp.txt");
            string testCleanupCodeSnippet = File.ReadAllText("..//..//" + snippetParentFolder + "//testCleanUpSnipp.txt");
            string testClearBrowserCodeSnippet = File.ReadAllText("..//..//" + snippetParentFolder + "//clrBrowserSnipp.txt");
            Dictionary<string, string> testInitialiseAttributes = new Dictionary<string, string>() { { "TestInitialize", "" }, };
            Dictionary<string, string> testCleanupAttributes = new Dictionary<string, string>() { { "TestCleanup", "" }, };
            string reportInstance = "Instance";
            Dictionary<string, string> memberCollection = new Dictionary<string, string>();
            memberCollection.Add("OpenQA.Selenium.IWebDriver,UnifiedFramework", "driver");
            memberCollection.Add("System.String", "textFile");
            memberCollection.Add("UnifiedFramework.UnifiedReports.UnifiedReports,UnifiedFramework", "unifiedReport");
            memberCollection.Add("UnifiedFramework.UnifiedReports.UnifiedTest,UnifiedFramework", "unifiedTestLog");
            memberCollection.Add("OpenQA.Selenium.Support.UI.WebDriverWait,UnifiedFramework", "wait");
            memberCollection.Add("UnifiedFrameWork.Controller.UComponentController", "unifiedComponent");

            UCodeEngine codeEngine = new UCodeEngine(nameSpace, localUsingCollection, className,"TestClass");
            codeEngine.UCodeAddMembers(memberCollection);
            codeEngine.UCodeAddCollectionMembers("List", "unifiedLogCollection", "UnifiedFramework.UnifiedReports.UnifiedTest,UnifiedFramework");
            codeEngine.UCodeAddMethodSnippet(testIntialiseCodeSnippet, "TestIntialise", testInitialiseAttributes);
            codeEngine.UCodeAddMethodSnippet(testCleanupCodeSnippet, "TestCleanUp", testCleanupAttributes);
            codeEngine.UCodeAddMethodSnippet(testClearBrowserCodeSnippet, "ClearBrowser");
            codeEngine.UCodeAddReportProperty(reportInstance, "UnifiedReports", "UReportsTitle", "UReportsHeadline");
            codeEngine.UCodeGenerator(filePath, directory, className);

            //Include file in project
            UCodeGen.FilesToInclude.Add(Path.Combine(directory, className));
        }

    }
}
