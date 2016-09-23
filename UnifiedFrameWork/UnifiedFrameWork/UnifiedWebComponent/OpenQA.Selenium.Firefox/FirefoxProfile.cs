using Newtonsoft.Json;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace OpenQA.Selenium.Firefox
{
	public class FirefoxProfile
	{
		private const string ExtensionFileName = "webdriver.xpi";

		private const string ExtensionResourceId = "WebDriver.FirefoxExt.zip";

		private const string UserPreferencesFileName = "user.js";

		private const string WebDriverPortPreferenceName = "webdriver_firefox_port";

		private const string EnableNativeEventsPreferenceName = "webdriver_enable_native_events";

		private const string AcceptUntrustedCertificatesPreferenceName = "webdriver_accept_untrusted_certs";

		private const string AssumeUntrustedCertificateIssuerPreferenceName = "webdriver_assume_untrusted_issuer";

		private int profilePort;

		private string profileDir;

		private string sourceProfileDir;

		private bool enableNativeEvents;

		private bool loadNoFocusLibrary;

		private bool acceptUntrustedCerts;

		private bool assumeUntrustedIssuer;

		private bool deleteSource;

		private bool deleteOnClean = true;

		private Preferences profilePreferences;

		private Dictionary<string, FirefoxExtension> extensions = new Dictionary<string, FirefoxExtension>();

		public int Port
		{
			get
			{
				return this.profilePort;
			}
			set
			{
				this.profilePort = value;
			}
		}

		public string ProfileDirectory
		{
			get
			{
				return this.profileDir;
			}
		}

		public bool DeleteAfterUse
		{
			get
			{
				return this.deleteOnClean;
			}
			set
			{
				this.deleteOnClean = value;
			}
		}

		public bool EnableNativeEvents
		{
			get
			{
				return this.enableNativeEvents;
			}
			set
			{
				this.enableNativeEvents = value;
			}
		}

		public bool AlwaysLoadNoFocusLibrary
		{
			get
			{
				return this.loadNoFocusLibrary;
			}
			set
			{
				this.loadNoFocusLibrary = value;
			}
		}

		public bool AcceptUntrustedCertificates
		{
			get
			{
				return this.acceptUntrustedCerts;
			}
			set
			{
				this.acceptUntrustedCerts = value;
			}
		}

		public bool AssumeUntrustedCertificateIssuer
		{
			get
			{
				return this.assumeUntrustedIssuer;
			}
			set
			{
				this.assumeUntrustedIssuer = value;
			}
		}

		public FirefoxProfile() : this(null)
		{
		}

		public FirefoxProfile(string profileDirectory) : this(profileDirectory, false)
		{
		}

		public FirefoxProfile(string profileDirectory, bool deleteSourceOnClean)
		{
			this.sourceProfileDir = profileDirectory;
			this.profilePort = FirefoxDriver.DefaultPort;
			this.enableNativeEvents = FirefoxDriver.DefaultEnableNativeEvents;
			this.acceptUntrustedCerts = FirefoxDriver.AcceptUntrustedCertificates;
			this.assumeUntrustedIssuer = FirefoxDriver.AssumeUntrustedCertificateIssuer;
			this.deleteSource = deleteSourceOnClean;
			this.ReadDefaultPreferences();
			this.profilePreferences.AppendPreferences(this.ReadExistingPreferences());
			this.AddWebDriverExtension();
		}

		public static FirefoxProfile FromBase64String(string base64)
		{
			string text = FileUtilities.GenerateRandomTempDirectoryName("webdriver.{0}.duplicated");
			byte[] buffer = Convert.FromBase64String(base64);
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (ZipStorer zipStorer = ZipStorer.Open(memoryStream, FileAccess.Read))
				{
					List<ZipStorer.ZipFileEntry> list = zipStorer.ReadCentralDirectory();
					foreach (ZipStorer.ZipFileEntry current in list)
					{
						string path = current.FilenameInZip.Replace('/', Path.DirectorySeparatorChar);
						string destinationFileName = Path.Combine(text, path);
						zipStorer.ExtractFile(current, destinationFileName);
					}
				}
			}
			return new FirefoxProfile(text, true);
		}

		public void AddExtension(string extensionToInstall)
		{
			this.extensions.Add(Path.GetFileNameWithoutExtension(extensionToInstall), new FirefoxExtension(extensionToInstall));
		}

		public void SetPreference(string name, string value)
		{
			this.profilePreferences.SetPreference(name, value);
		}

		public void SetPreference(string name, int value)
		{
			this.profilePreferences.SetPreference(name, value);
		}

		public void SetPreference(string name, bool value)
		{
			this.profilePreferences.SetPreference(name, value);
		}

		public void SetProxyPreferences(Proxy proxy)
		{
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy", "proxy must not be null");
			}
			if (proxy.Kind == ProxyKind.Unspecified)
			{
				return;
			}
			this.SetPreference("network.proxy.type", (int)proxy.Kind);
			switch (proxy.Kind)
			{
			case ProxyKind.Manual:
				this.SetPreference("network.proxy.no_proxies_on", string.Empty);
				this.SetManualProxyPreference("ftp", proxy.FtpProxy);
				this.SetManualProxyPreference("http", proxy.HttpProxy);
				this.SetManualProxyPreference("ssl", proxy.SslProxy);
				this.SetManualProxyPreference("socks", proxy.SocksProxy);
				if (proxy.NoProxy != null)
				{
					this.SetPreference("network.proxy.no_proxies_on", proxy.NoProxy);
					return;
				}
				break;
			case ProxyKind.ProxyAutoConfigure:
				this.SetPreference("network.proxy.autoconfig_url", proxy.ProxyAutoConfigUrl);
				break;
			default:
				return;
			}
		}

		public void WriteToDisk()
		{
			this.profileDir = FirefoxProfile.GenerateProfileDirectoryName();
			if (!string.IsNullOrEmpty(this.sourceProfileDir))
			{
				FileUtilities.CopyDirectory(this.sourceProfileDir, this.profileDir);
			}
			else
			{
				Directory.CreateDirectory(this.profileDir);
			}
			this.InstallExtensions();
			this.DeleteLockFiles();
			this.DeleteExtensionsCache();
			this.UpdateUserPreferences();
		}

		public void Clean()
		{
			if (this.deleteOnClean && !string.IsNullOrEmpty(this.profileDir) && Directory.Exists(this.profileDir))
			{
				FileUtilities.DeleteDirectory(this.profileDir);
			}
			if (this.deleteSource && !string.IsNullOrEmpty(this.sourceProfileDir) && Directory.Exists(this.sourceProfileDir))
			{
				FileUtilities.DeleteDirectory(this.sourceProfileDir);
			}
		}

		public string ToBase64String()
		{
			string result = string.Empty;
			this.WriteToDisk();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipStorer zipStorer = ZipStorer.Create(memoryStream, string.Empty))
				{
					string[] files = Directory.GetFiles(this.profileDir, "*.*", SearchOption.AllDirectories);
					string[] array = files;
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i];
						string fileNameInZip = text.Substring(this.profileDir.Length).Replace(Path.DirectorySeparatorChar, '/');
						zipStorer.AddFile(ZipStorer.CompressionMethod.Deflate, text, fileNameInZip, string.Empty);
					}
				}
				result = Convert.ToBase64String(memoryStream.ToArray());
				this.Clean();
			}
			return result;
		}

		internal void AddWebDriverExtension()
		{
			if (!this.extensions.ContainsKey("webdriver"))
			{
				this.extensions.Add("webdriver", new FirefoxExtension("webdriver.xpi", "WebDriver.FirefoxExt.zip"));
			}
		}

		private static string GenerateProfileDirectoryName()
		{
			return FileUtilities.GenerateRandomTempDirectoryName("anonymous.{0}.webdriver-profile");
		}

		private void DeleteLockFiles()
		{
			File.Delete(Path.Combine(this.profileDir, ".parentlock"));
			File.Delete(Path.Combine(this.profileDir, "parent.lock"));
		}

		private void InstallExtensions()
		{
			foreach (string current in this.extensions.Keys)
			{
				this.extensions[current].Install(this.profileDir);
			}
		}

		private void DeleteExtensionsCache()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(this.profileDir, "extensions"));
			string path = Path.Combine(directoryInfo.Parent.FullName, "extensions.cache");
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		private void UpdateUserPreferences()
		{
			if (this.profilePort == 0)
			{
				throw new WebDriverException("You must set the port to listen on before updating user preferences file");
			}
			string text = Path.Combine(this.profileDir, "user.js");
			if (File.Exists(text))
			{
				try
				{
					File.Delete(text);
				}
				catch (Exception innerException)
				{
					throw new WebDriverException("Cannot delete existing user preferences", innerException);
				}
			}
			this.profilePreferences.SetPreference("webdriver_firefox_port", this.profilePort);
			this.profilePreferences.SetPreference("webdriver_enable_native_events", this.enableNativeEvents);
			this.profilePreferences.SetPreference("webdriver_accept_untrusted_certs", this.acceptUntrustedCerts);
			this.profilePreferences.SetPreference("webdriver_assume_untrusted_issuer", this.assumeUntrustedIssuer);
			string preference = this.profilePreferences.GetPreference("browser.startup.homepage");
			if (!string.IsNullOrEmpty(preference))
			{
				this.profilePreferences.SetPreference("startup.homepage_welcome_url", string.Empty);
				if (preference != "about:blank")
				{
					this.profilePreferences.SetPreference("browser.startup.page", 1);
				}
			}
			this.profilePreferences.WriteToFile(text);
		}

		private void ReadDefaultPreferences()
		{
			using (Stream resourceStream = ResourceUtilities.GetResourceStream("webdriver.json", "WebDriver.FirefoxPreferences"))
			{
				using (StreamReader streamReader = new StreamReader(resourceStream))
				{
					string value = streamReader.ReadToEnd();
					Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(value, new JsonConverter[]
					{
						new ResponseValueJsonConverter()
					});
					Dictionary<string, object> defaultImmutablePreferences = dictionary["frozen"] as Dictionary<string, object>;
					Dictionary<string, object> defaultPreferences = dictionary["mutable"] as Dictionary<string, object>;
					this.profilePreferences = new Preferences(defaultImmutablePreferences, defaultPreferences);
				}
			}
		}

		private Dictionary<string, string> ReadExistingPreferences()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			try
			{
				if (!string.IsNullOrEmpty(this.sourceProfileDir))
				{
					string path = Path.Combine(this.sourceProfileDir, "user.js");
					if (File.Exists(path))
					{
						string[] array = File.ReadAllLines(path);
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							string text = array2[i];
							if (text.StartsWith("user_pref(\"", StringComparison.OrdinalIgnoreCase))
							{
								string text2 = text.Substring("user_pref(\"".Length);
								text2 = text2.Substring(0, text2.Length - ");".Length);
								string[] array3 = text.Split(new string[]
								{
									","
								}, StringSplitOptions.None);
								array3[0] = array3[0].Substring(0, array3[0].Length - 1);
								dictionary.Add(array3[0].Trim(), array3[1].Trim());
							}
						}
					}
				}
			}
			catch (IOException innerException)
			{
				throw new WebDriverException(string.Empty, innerException);
			}
			return dictionary;
		}

		private void SetManualProxyPreference(string key, string settingString)
		{
			if (settingString == null)
			{
				return;
			}
			string[] array = settingString.Split(new char[]
			{
				':'
			});
			this.SetPreference("network.proxy." + key, array[0]);
			if (array.Length > 1)
			{
				this.SetPreference("network.proxy." + key + "_port", int.Parse(array[1], CultureInfo.InvariantCulture));
			}
		}
	}
}
