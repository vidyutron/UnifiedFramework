using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using System.CodeDom.Compiler;
using UnifiedFramework.UnifiedReports;

namespace UnifiedFrameWork.Controller
{
    public class UCodeEngine
    {
        CodeCompileUnit targetUnit;
        CodeTypeDeclaration targetClass;
        //public static string globalfilePath;
        //private static string outputFileName;
        public UCodeEngine(string nameSpace, List<string> usingCollection, string className, 
            string classAttribute = "", string baseType = "")
        {

            //globalfilePath = filePath;
            CodeAttributeDeclaration classAttributeDeclare;
            targetUnit = new CodeCompileUnit();
            CodeNamespace samples = new CodeNamespace(nameSpace);
            foreach (string singleNameSpace in usingCollection)
                samples.Imports.Add(new CodeNamespaceImport(singleNameSpace));
            targetClass = new CodeTypeDeclaration(className);
            if (!string.IsNullOrEmpty(baseType))
                targetClass.BaseTypes.Add(baseType);
            if (!string.IsNullOrEmpty(classAttribute))
            {
                classAttributeDeclare = new CodeAttributeDeclaration(classAttribute);
                targetClass.CustomAttributes.Add(classAttributeDeclare);
            }
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public;
            samples.Types.Add(targetClass);
            targetUnit.Namespaces.Add(samples);
        }

        internal void UCodeAddMethod(string methodName, Dictionary<string, string> methodAttribute=null, bool tryCatchBlock = false)
        {
            CodeMemberMethod sampleMethod = new CodeMemberMethod();
            sampleMethod.Attributes = MemberAttributes.Public;
            sampleMethod.Name = methodName;
            sampleMethod.ReturnType =
                new CodeTypeReference(typeof(void));
            CodeAttributeDeclarationCollection collection = new CodeAttributeDeclarationCollection();
            if (methodAttribute != null)
            {
                foreach (KeyValuePair<string, string> singleItem in methodAttribute)
                {
                    if (string.IsNullOrEmpty(singleItem.Value))
                        collection.Add(new CodeAttributeDeclaration(singleItem.Key));
                    else
                        collection.Add(new CodeAttributeDeclaration(singleItem.Key,
                            new CodeAttributeArgument(new CodePrimitiveExpression(singleItem.Value))));
                }
                foreach (CodeAttributeDeclaration singleDeclaration in collection)
                    sampleMethod.CustomAttributes.Add(singleDeclaration);
            }
            if (tryCatchBlock)
            {
                CodeTryCatchFinallyStatement testMethodTry = new CodeTryCatchFinallyStatement();
                testMethodTry.TryStatements.Add(new CodeCommentStatement("Please fill in you code here"));
                sampleMethod.Statements.Add(testMethodTry);

                CodeCatchClause testMethodCatch = new CodeCatchClause("ex");
                testMethodCatch.Statements.Add(new CodeCommentStatement("Code to handle the Exception"));
                testMethodTry.CatchClauses.Add(testMethodCatch);

                testMethodTry.FinallyStatements.Add(new CodeCommentStatement("Handle any finally block"));
            }
            targetClass.Members.Add(sampleMethod);
        }

        internal void UCodeAddMethodSnippet(string codeSnippet, string methodName, Dictionary<string, string> methodAttribute = null)
        {
            CodeMemberMethod uCodeSnippetMethod = new CodeMemberMethod();
            uCodeSnippetMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            uCodeSnippetMethod.Name = methodName;
            if (methodAttribute != null)
            {
                CodeAttributeDeclarationCollection collection = new CodeAttributeDeclarationCollection();
                foreach (KeyValuePair<string, string> singleItem in methodAttribute)
                {
                    if (string.IsNullOrEmpty(singleItem.Value))
                        collection.Add(new CodeAttributeDeclaration(singleItem.Key));
                    else
                        collection.Add(new CodeAttributeDeclaration(singleItem.Key,
                            new CodeAttributeArgument(new CodePrimitiveExpression(singleItem.Value))));
                }
                foreach (CodeAttributeDeclaration singleDeclaration in collection)
                    uCodeSnippetMethod.CustomAttributes.Add(singleDeclaration);
            }
            CodeSnippetStatement uCodeSnippet = new CodeSnippetStatement();
            uCodeSnippet.Value = codeSnippet;
            CodeRegionDirective startRegion = new CodeRegionDirective(CodeRegionMode.End, "");
            startRegion.RegionText = "Code Injection";
            startRegion.RegionMode = CodeRegionMode.Start;
            uCodeSnippet.StartDirectives.Add(startRegion);
            uCodeSnippet.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            uCodeSnippetMethod.Statements.Add(uCodeSnippet);
            targetClass.Members.Add(uCodeSnippetMethod);
        }

        internal void UCodeAddTestMethod(string methodName, Dictionary<string, string> methodAttribute,
            string reportCollection="unifiedLogCollection",string reportInstance="unifiedReport")
        {
            string testCaseDescription = string.Empty;
            CodeMemberMethod sampleMethod = new CodeMemberMethod();
            sampleMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            sampleMethod.Name = methodName;
            sampleMethod.ReturnType =
                new CodeTypeReference(typeof(void));
            CodeAttributeDeclarationCollection collection = new CodeAttributeDeclarationCollection();
            if (methodAttribute != null)
            {
                foreach (KeyValuePair<string, string> singleItem in methodAttribute)
                {
                    if (singleItem.Key.ToLower().ToString().Contains("description"))
                        testCaseDescription = "\"" + singleItem.Value + "\"";
                    if (string.IsNullOrEmpty(singleItem.Value))
                        collection.Add(new CodeAttributeDeclaration(singleItem.Key));
                    else
                        collection.Add(new CodeAttributeDeclaration(singleItem.Key,
                            new CodeAttributeArgument(new CodePrimitiveExpression(singleItem.Value))));
                }
                foreach (CodeAttributeDeclaration singleDeclaration in collection)
                    sampleMethod.CustomAttributes.Add(singleDeclaration);
            }
            sampleMethod.Statements.Add(new CodeSnippetExpression(reportCollection + ".Add("+reportInstance+".StartTest(" + testCaseDescription + "));"));
            CodeTryCatchFinallyStatement testMethodTry = new CodeTryCatchFinallyStatement();
            testMethodTry.TryStatements.Add(new CodeCommentStatement("Please fill in you code here"));
            sampleMethod.Statements.Add(testMethodTry);

            CodeCatchClause testMethodCatch = new CodeCatchClause("ex");
            testMethodCatch.Statements.Add(new CodeCommentStatement("Code to handle the Exception(s)"));
            testMethodTry.CatchClauses.Add(testMethodCatch);

            testMethodTry.FinallyStatements.Add(new CodeCommentStatement("Handle finally block"));

            targetClass.Members.Add(sampleMethod);
        }

        internal void UCodeAddMembers(Dictionary<string, string> memberList)
        {
            foreach (KeyValuePair<string, string> singleitem in memberList)
            {
                if (!string.IsNullOrEmpty(singleitem.Value))
                {
                    CodeMemberField field1 = new CodeMemberField();
                    field1.Name = singleitem.Value;
                    field1.Attributes = MemberAttributes.Static | MemberAttributes.Public;
                    field1.Type = new CodeTypeReference(Type.GetType(singleitem.Key));
                    targetClass.Members.Add(field1);
                }
            }

        }

        internal void UCodeAddCollectionMembers(string refName, string memberName, string param1, string param2 = "")
        {
            CodeTypeReference uCodeType = new CodeTypeReference(
                refName,
                new CodeTypeReference[] {
                    new CodeTypeReference(Type.GetType(param1))});
            CodeMemberField field1 = new CodeMemberField();
            field1.Name = memberName;
            field1.Attributes = MemberAttributes.Static | MemberAttributes.Public;
            field1.Type = uCodeType;

            targetClass.Members.Add(field1);
        }

        internal void UCodeAddProperties(string propertyName, string returnVariable, bool get = true, bool set = true)
        {
            CodeMemberProperty uCodeProperty = new CodeMemberProperty();
            uCodeProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            uCodeProperty.HasGet = get;
            uCodeProperty.HasSet = set;
            uCodeProperty.Comments.Add(new CodeCommentStatement("Please fill in your code"));
            uCodeProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), returnVariable)));
            targetClass.Members.Add(uCodeProperty);
        }

        internal void UCodeAddReportProperty(string reportInstance,string reportName, string reportTitle, 
            string reportHeadline, string reportFilePath = "",string reportMemberName="unifiedReport")
        {
            CodeMemberProperty uCodeProperty = new CodeMemberProperty();
            uCodeProperty.Attributes = MemberAttributes.Public;
            reportFilePath = string.IsNullOrEmpty(reportFilePath) ? Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName : reportFilePath;
            uCodeProperty.Name = reportInstance;
            uCodeProperty.Type = new CodeTypeReference(Type.GetType("UnifiedFramework.UnifiedReports.UnifiedReports, UnifiedFramework"));
            uCodeProperty.HasGet = true;
            uCodeProperty.HasSet = false;
            string localReportFilepath = "\"" + reportFilePath + "\"";
            string localReportName = "\"" + reportName + "\"";
            string localReportTitle = "\"" + reportTitle + "\"";
            string localReportHeadLine="\""+reportHeadline+"\"";
            var ucodeMethodOne = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(
                Type.GetType("UnifiedFrameWork.Controller.DirectoryHandler,UnifiedFrameWork")), "DirectoryCreation", new CodePrimitiveExpression("UnifiedReport"));
            CodeExpression uCodeOjectCreate = new CodeObjectCreateExpression(new CodeTypeReference("UnifiedReports"),
                new CodePrimitiveExpression(Path.Combine(reportFilePath, reportName + ".html")), new CodePrimitiveExpression(true));
            CodeExpression thisExpr = new CodeThisReferenceExpression();
            uCodeProperty.GetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("unifiedReport==null"), new CodeStatement[] {
                new CodeVariableDeclarationStatement(typeof(string),"reportFilePath",ucodeMethodOne),
                new CodeExpressionStatement( new CodeSnippetExpression(reportMemberName+"= new UnifiedReports(Path.Combine(reportFilePath,"+localReportName+"+"+"\".html\"), true)")),
                new CodeExpressionStatement(new CodeSnippetExpression(reportMemberName+".Config().DocumentTitle("+localReportTitle+").ReportName("+localReportName+").ReportHeadline("+localReportHeadLine+")"))}));
            uCodeProperty.GetStatements.Add(new CodeSnippetExpression("return "+ reportMemberName));
            //mpBriefcaseName.GetStatements.Add(new CodeSnippetExpression("return m_BriefcaseName"));
            targetClass.Members.Add(uCodeProperty);

        }

        internal void UCodeRepository(List<string> regionList=null,int memberCount=0)
        {
            memberCount =memberCount== 0 ? 3 : memberCount;
            int regionCount = (regionList != null) ? regionList.Count:3;
            for(int i = 0; i < regionCount; i++)
            {
                targetClass.Members.Add(new CodeSnippetTypeMember("# region Region"+i));
                for(int k = 0; k < memberCount; k++)
                {
                    CodeMemberField field1 = new CodeMemberField();
                    field1.Name = "keyWord"+i+k;
                    field1.Attributes = MemberAttributes.Static | MemberAttributes.Public;
                    field1.Type = new CodeTypeReference(typeof(System.String));
                    field1.InitExpression = new CodeSnippetExpression("\""+"keyWord"+i+k+"\"");
                    targetClass.Members.Add(field1);
                }
                targetClass.Members.Add(new CodeSnippetTypeMember("#endregion"));
            }
        }

        public void UCodeGenerator(string filePath,string directory, string className)
        {
            string CompletePath=DirectoryHandler.DirectoryCreation(Path.Combine(filePath, directory), "custom");
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            Random random = new Random();
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            className = File.Exists(Path.Combine(CompletePath, className + ".cs")) ? className + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") : className;
            using (StreamWriter sourceWriter = new StreamWriter(Path.Combine(CompletePath, className + ".cs")))
            {
                provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
            }
        }
    }
}