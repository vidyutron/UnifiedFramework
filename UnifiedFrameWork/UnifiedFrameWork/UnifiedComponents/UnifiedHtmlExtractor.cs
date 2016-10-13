using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnifiedFrameWork.Controller;

namespace UnifiedFrameWork.UnifiedComponents
{
    public class UnifiedHtmlExtractor
    {
        public static void ExtractElements(string urlToExract)
        {
            var url = ValidateUrl(urlToExract);
            if (string.IsNullOrWhiteSpace(urlToExract) && url != null)
            {

                Console.WriteLine("Empty or Invalid Url has been supplied!");
            }
            else
            {   
                var html = new HtmlDocument();
                html.LoadHtml(new WebClient().DownloadString(url.UrlString));
                var root = html.DocumentNode;
                var nodes = root.Descendants();
                var extractId = root.Descendants().Select(n => n.GetAttributeValue("id", ""));
                var listID = extractId!=null&&extractId.Count()>0?extractId.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct():null;
                List<KeyValue> test1 = new List<KeyValue>();
                var extractClass =root.Descendants().Select(n => n.GetAttributeValue("class", ""));
                var list2 = extractClass != null && extractClass.Count() > 0 ? extractClass.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct():null;
                var listClass = new List<string>();
                foreach (var item in list2) { listClass.AddRange(item.Split(new char[0])); }
                listClass = listClass.Distinct().ToList();
                List<KeyValue> test2 = new List<KeyValue>();
                var anchorElement = html.DocumentNode.SelectNodes("//a")!=null? html.DocumentNode.SelectNodes("//a").ToList():null;
                var anchorText = anchorElement!=null? anchorElement.Select(n => n.InnerText).Distinct():null;
                var anchorXpath = anchorElement != null ? anchorElement.Select(n => n.XPath).Distinct():null;
                var btnElement = html.DocumentNode.SelectNodes("//button") != null ? html.DocumentNode.SelectNodes("//button").ToList() : null;
                var buttonXpath = btnElement!=null?btnElement.Select(n => n.XPath):null;
                var json = JsonConvert.SerializeObject(new
                {
                    Id = ObjectifyDomElement(listID),
                    Class = ObjectifyDomElement(listClass),
                    AnchorText = ObjectifyDomElement(anchorText),
                    AnchorXpath = ObjectifyDomElement(anchorXpath),
                    ButtonXpath = ObjectifyDomElement(buttonXpath)
                }, Newtonsoft.Json.Formatting.Indented);
                CreateJsonFile(json, url.JsonFileName);            
                //string strtestone = jsonInverse.Id[1].value;
            }

        }

        public static HtmlDomObject GetHtmlElements(string CompleteFilePath)
        {
            using (StreamReader file = File.OpenText(CompleteFilePath.Contains(".json")?CompleteFilePath:CompleteFilePath+".json"))
            {
                var jsonInverse = JsonConvert.DeserializeObject<HtmlDomObject>(file.ReadToEnd());
                return jsonInverse;
            }
                
        }

        internal static List<KeyValue> ObjectifyDomElement(IEnumerable<string> listToObjectify)
        {
            List<KeyValue> returnObject = new List<KeyValue>();
            if (listToObjectify != null)
            {
                for (int i = 0; i < listToObjectify.Count(); i++)
                {
                    returnObject.Add(
                       new KeyValue()
                       {
                           Key = i,
                           Value = listToObjectify.ElementAt(i),
                           Description=string.Empty

                       }
                      );
                }
                return returnObject;
            }
            return null;
            
        }

        internal static UExtractorUrl ValidateUrl(string validateUri)
        {
            try
            {
                var uri = new Uri(validateUri);
                string jsonFileName = uri.Authority.Replace(".","").Replace("www","").Replace("com","");
                if (uri.Segments.Length > 2) { jsonFileName = jsonFileName + "_" + uri.Segments[1].Replace("/", "") + "_" + uri.Segments[2].Replace("/", "");}
                return new UExtractorUrl {
                    UrlString = uri.ToString(),
                    JsonFileName=jsonFileName+"_"+DateTime.Now.ToString("yyyyMMddHHmmss")
                };
            }
            catch (UriFormatException)
            {
                return null;
            }
           
        }
        internal static void CreateJsonFile(string jsonToWrite, string jsonFileName)
        {
            try
            {
                var jsonFile = Path.Combine(DirectoryHandler.DirectoryCreation(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName,
                    "UFHtmlExtractor")), jsonFileName + ".json");
                File.WriteAllText(jsonFile, jsonToWrite, Encoding.UTF8);
                var projFilePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                var p1 = new Microsoft.Build.Evaluation.Project(Path.Combine(projFilePath,
                    Path.GetFileName(projFilePath.TrimEnd(Path.DirectorySeparatorChar)) + ".csproj"));
                    p1.AddItem("None", Path.Combine("UFHtmlExtractor",jsonFileName+".json"));
                    p1.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Creation of Json File, Failed!");
            }
        }      
    }
 
}
