using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnifiedFrameWork.UnifiedComponents;

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
    }
}
