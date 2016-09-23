using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace OpenQA.Selenium.Firefox
{
	public class FirefoxExtension
	{
		private const string EmNamespaceUri = "http://www.mozilla.org/2004/em-rdf#";

		private string extensionFileName;

		private string extensionResourceId;

		public FirefoxExtension(string fileName) : this(fileName, string.Empty)
		{
		}

		internal FirefoxExtension(string fileName, string resourceId)
		{
			this.extensionFileName = fileName;
			this.extensionResourceId = resourceId;
		}

		public void Install(string profileDirectory)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(profileDirectory);
			string text = Path.Combine(Path.GetTempPath(), directoryInfo.Name + ".staging");
			string text2 = Path.Combine(text, Path.GetFileName(this.extensionFileName));
			if (Directory.Exists(text2))
			{
				Directory.Delete(text2, true);
			}
			Directory.CreateDirectory(text2);
			Stream resourceStream = ResourceUtilities.GetResourceStream(this.extensionFileName, this.extensionResourceId);
			using (ZipStorer zipStorer = ZipStorer.Open(resourceStream, FileAccess.Read))
			{
				List<ZipStorer.ZipFileEntry> list = zipStorer.ReadCentralDirectory();
				foreach (ZipStorer.ZipFileEntry current in list)
				{
					string path = current.FilenameInZip.Replace('/', Path.DirectorySeparatorChar);
					string destinationFileName = Path.Combine(text2, path);
					zipStorer.ExtractFile(current, destinationFileName);
				}
			}
			string path2 = FirefoxExtension.ReadIdFromInstallRdf(text2);
			string text3 = Path.Combine(Path.Combine(profileDirectory, "extensions"), path2);
			if (Directory.Exists(text3))
			{
				Directory.Delete(text3, true);
			}
			Directory.CreateDirectory(text3);
			FileUtilities.CopyDirectory(text2, text3);
			FileUtilities.DeleteDirectory(text);
		}

		private static string ReadIdFromInstallRdf(string root)
		{
			string text = null;
			try
			{
				string text2 = Path.Combine(root, "install.rdf");
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(text2);
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("em", "http://www.mozilla.org/2004/em-rdf#");
				xmlNamespaceManager.AddNamespace("RDF", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
				XmlNode xmlNode = xmlDocument.SelectSingleNode("//em:id", xmlNamespaceManager);
				if (xmlNode == null)
				{
					XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//RDF:Description", xmlNamespaceManager);
					XmlAttribute xmlAttribute = xmlNode2.Attributes["id", "http://www.mozilla.org/2004/em-rdf#"];
					if (xmlAttribute == null)
					{
						throw new WebDriverException("Cannot locate node containing extension id: " + text2);
					}
					text = xmlAttribute.Value;
				}
				else
				{
					text = xmlNode.InnerText;
				}
				if (string.IsNullOrEmpty(text))
				{
					throw new FileNotFoundException("Cannot install extension with ID: " + text);
				}
			}
			catch (Exception innerException)
			{
				throw new WebDriverException("Error installing extension", innerException);
			}
			return text;
		}
	}
}
