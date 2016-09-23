using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnifiedFrameWork.Controller
{
    class XmlReadWrite
    {
        internal void XmlWriter(string filePath, string fileName, string subRootNode/*,List<CustomClass> customClass*/)
        {
            string completeFilepath =Path.Combine(filePath, fileName);
            using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(completeFilepath))
            {
                writer.WriteStartDocument();
                //Assuming fileName is same as the Root Node name
                writer.WriteStartElement(fileName);

                #region ForeachLoop
                //foreach (CustomClass customClass in customClass)
                //{
                //    writer.WriteStartElement(subRootNode);

                //    writer.WriteElementString("ID", customClass.Id.ToString());
                //    writer.WriteElementString("FirstName", customClass.FirstName);
                //    writer.WriteElementString("LastName", customClass.LastName);
                //    //Add the writer.WriteElementString based on the number of member variable in CustomClass

                //    writer.WriteEndElement();
                //}
                #endregion

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        internal void XmlReader(string filepath, string fileName, List<string> nodesToCheck, string customNode)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Path.Combine(filepath,fileName));
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/" + fileName + "/" + customNode);
            List<string> nodeDataOne = new List<string>();
            List<string> nodeDataTwo = new List<string>();
            //lists to store the xml node data, add according to the nodes requied to be read
            int iterator = 0;
            while (iterator < nodesToCheck.Count)
            {
                int innerInc = 0;
                foreach (XmlNode node in nodeList)
                {
                    nodeDataOne.Add(node.SelectSingleNode(nodesToCheck[innerInc]).InnerText);
                    nodeDataTwo.Add(node.SelectSingleNode(nodesToCheck[++innerInc]).InnerText);
                    //Add according to the variables defined above, ++innerInc will reamain same expect first.
                }
                iterator++;
            }
        }

    }
}
