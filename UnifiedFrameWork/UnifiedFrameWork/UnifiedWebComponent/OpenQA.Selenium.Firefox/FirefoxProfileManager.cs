using OpenQA.Selenium.Firefox.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace OpenQA.Selenium.Firefox
{
	public class FirefoxProfileManager
	{
		private Dictionary<string, string> profiles = new Dictionary<string, string>();

		public ReadOnlyCollection<string> ExistingProfiles
		{
			get
			{
				List<string> list = new List<string>(this.profiles.Keys);
				return list.AsReadOnly();
			}
		}

		public FirefoxProfileManager()
		{
			string applicationDataDirectory = FirefoxProfileManager.GetApplicationDataDirectory();
			this.ReadProfiles(applicationDataDirectory);
		}

		public FirefoxProfile GetProfile(string profileName)
		{
			FirefoxProfile firefoxProfile = null;
			if (!string.IsNullOrEmpty(profileName) && this.profiles.ContainsKey(profileName))
			{
				firefoxProfile = new FirefoxProfile(this.profiles[profileName]);
				if (firefoxProfile.Port == 0)
				{
					firefoxProfile.Port = FirefoxDriver.DefaultPort;
				}
			}
			return firefoxProfile;
		}

		private static string GetApplicationDataDirectory()
		{
			string path = string.Empty;
			switch (Environment.OSVersion.Platform)
			{
			case PlatformID.Unix:
				path = Path.Combine(".mozilla", "firefox");
				goto IL_6E;
			case PlatformID.MacOSX:
				path = Path.Combine("Library", Path.Combine("Application Support", "Firefox"));
				goto IL_6E;
			}
			path = Path.Combine("Mozilla", "Firefox");
			IL_6E:
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path);
		}

		private void ReadProfiles(string appDataDirectory)
		{
			string text = Path.Combine(appDataDirectory, "profiles.ini");
			if (File.Exists(text))
			{
				IniFileReader iniFileReader = new IniFileReader(text);
				ReadOnlyCollection<string> sectionNames = iniFileReader.SectionNames;
				foreach (string current in sectionNames)
				{
					if (current.StartsWith("profile", StringComparison.OrdinalIgnoreCase))
					{
						string value = iniFileReader.GetValue(current, "name");
						bool flag = iniFileReader.GetValue(current, "isrelative") == "1";
						string value2 = iniFileReader.GetValue(current, "path");
						string value3 = string.Empty;
						if (flag)
						{
							value3 = Path.Combine(appDataDirectory, value2);
						}
						else
						{
							value3 = value2;
						}
						this.profiles.Add(value, value3);
					}
				}
			}
		}
	}
}
