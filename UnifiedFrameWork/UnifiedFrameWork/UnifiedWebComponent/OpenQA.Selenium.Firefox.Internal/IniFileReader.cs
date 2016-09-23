using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace OpenQA.Selenium.Firefox.Internal
{
	internal class IniFileReader
	{
		private Dictionary<string, Dictionary<string, string>> iniFileStore = new Dictionary<string, Dictionary<string, string>>();

		public ReadOnlyCollection<string> SectionNames
		{
			get
			{
				List<string> list = new List<string>(this.iniFileStore.Keys);
				return new ReadOnlyCollection<string>(list);
			}
		}

		public IniFileReader(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName", "File name must not be null or empty");
			}
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException("INI file not found", fileName);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string text = string.Empty;
			string[] array = File.ReadAllLines(fileName);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text2 = array2[i];
				if (!string.IsNullOrEmpty(text2.Trim()) && !text2.StartsWith(";", StringComparison.OrdinalIgnoreCase))
				{
					if (text2.StartsWith("[", StringComparison.OrdinalIgnoreCase) && text2.EndsWith("]", StringComparison.OrdinalIgnoreCase))
					{
						if (!string.IsNullOrEmpty(text))
						{
							this.iniFileStore.Add(text, dictionary);
						}
						text = text2.Substring(1, text2.Length - 2).ToUpperInvariant();
						dictionary = new Dictionary<string, string>();
					}
					else
					{
						string[] array3 = text2.Split(new char[]
						{
							'='
						}, 2);
						string key = array3[0].ToUpperInvariant();
						string value = string.Empty;
						if (array3.Length > 1)
						{
							value = array3[1];
						}
						dictionary.Add(key, value);
					}
				}
			}
			this.iniFileStore.Add(text, dictionary);
		}

		public string GetValue(string sectionName, string valueName)
		{
			if (string.IsNullOrEmpty(sectionName))
			{
				throw new ArgumentNullException("sectionName", "Section name cannot be null or empty");
			}
			string key = sectionName.ToUpperInvariant();
			if (string.IsNullOrEmpty(valueName))
			{
				throw new ArgumentNullException("valueName", "Value name cannot be null or empty");
			}
			string key2 = valueName.ToUpperInvariant();
			if (!this.iniFileStore.ContainsKey(key))
			{
				throw new ArgumentException("Section does not exist: " + sectionName, "sectionName");
			}
			Dictionary<string, string> dictionary = this.iniFileStore[key];
			if (!dictionary.ContainsKey(key2))
			{
				throw new ArgumentException("Value does not exist: " + valueName, "valueName");
			}
			return dictionary[key2];
		}
	}
}
