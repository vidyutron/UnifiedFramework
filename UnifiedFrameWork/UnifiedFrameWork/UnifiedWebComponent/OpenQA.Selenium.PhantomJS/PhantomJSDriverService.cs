using Newtonsoft.Json;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace OpenQA.Selenium.PhantomJS
{
	[JsonObject(MemberSerialization.OptIn)]
	public sealed class PhantomJSDriverService : DriverService
	{
		private static readonly string PhantomJSDriverServiceFileName = PhantomJSDriverService.PlatformSpecificDriverServiceFileName;

		private static readonly Uri PhantomJSDownloadUrl = new Uri("http://phantomjs.org/download.html");

		private List<string> additionalArguments = new List<string>();

		private string ghostDriverPath = string.Empty;

		private string logFile = string.Empty;

		private string address = string.Empty;

		private string gridHubUrl = string.Empty;

		[JsonProperty("cookiesFile", NullValueHandling = NullValueHandling.Ignore), CommandLineArgumentName("cookies-file")]
		public string CookiesFile
		{
			get;
			set;
		}

		[JsonProperty("diskCache", DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("disk-cache"), DefaultValue(false)]
		public bool DiskCache
		{
			get;
			set;
		}

		[JsonProperty("ignoreSslErrors", DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("ignore-ssl-errors"), DefaultValue(false)]
		public bool IgnoreSslErrors
		{
			get;
			set;
		}

		[JsonProperty("loadImages", DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("load-images"), DefaultValue(true)]
		public bool LoadImages
		{
			get;
			set;
		}

		[JsonProperty("localStoragePath", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("local-storage-path")]
		public string LocalStoragePath
		{
			get;
			set;
		}

		[JsonProperty("localStorageQuota", DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("local-storage-quota"), DefaultValue(0)]
		public int LocalStorageQuota
		{
			get;
			set;
		}

		[JsonProperty("localToRemoteUrlAccess", DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("local-to-remote-url-access"), DefaultValue(false)]
		public bool LocalToRemoteUrlAccess
		{
			get;
			set;
		}

		[JsonProperty("maxDiskCacheSize", DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("max-disk-cache-size"), DefaultValue(0)]
		public int MaxDiskCacheSize
		{
			get;
			set;
		}

		[JsonProperty("outputEncoding", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("output-encoding"), DefaultValue("utf8")]
		public string OutputEncoding
		{
			get;
			set;
		}

		[JsonProperty("proxy", NullValueHandling = NullValueHandling.Ignore), CommandLineArgumentName("proxy")]
		public string Proxy
		{
			get;
			set;
		}

		[JsonProperty("proxyType", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("proxy-type"), DefaultValue("http")]
		public string ProxyType
		{
			get;
			set;
		}

		[JsonProperty("proxyAuth", NullValueHandling = NullValueHandling.Ignore), CommandLineArgumentName("proxy-auth")]
		public string ProxyAuthentication
		{
			get;
			set;
		}

		[JsonProperty("scriptEncoding", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("script-encoding"), DefaultValue("utf8")]
		public string ScriptEncoding
		{
			get;
			set;
		}

		[JsonProperty("sslProtocol", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("ssl-protocol"), DefaultValue("SSLv3")]
		public string SslProtocol
		{
			get;
			set;
		}

		[JsonProperty("sslCertificatesPath", NullValueHandling = NullValueHandling.Ignore), CommandLineArgumentName("ssl-certificates-path")]
		public string SslCertificatesPath
		{
			get;
			set;
		}

		[JsonProperty("webSecurity", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore), CommandLineArgumentName("web-security"), DefaultValue(true)]
		public bool WebSecurity
		{
			get;
			set;
		}

		[JsonIgnore]
		public string GhostDriverPath
		{
			get
			{
				return this.ghostDriverPath;
			}
			set
			{
				this.ghostDriverPath = value;
			}
		}

		[JsonIgnore]
		public string IPAddress
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		[JsonIgnore]
		public string GridHubUrl
		{
			get
			{
				return this.gridHubUrl;
			}
			set
			{
				this.gridHubUrl = value;
			}
		}

		public string LogFile
		{
			get
			{
				return this.logFile;
			}
			set
			{
				this.logFile = value;
			}
		}

		[JsonIgnore]
		public ReadOnlyCollection<string> AdditionalArguments
		{
			get
			{
				return this.additionalArguments.AsReadOnly();
			}
		}

		[JsonIgnore]
		public string ConfigFile
		{
			get;
			set;
		}

		protected override string CommandLineArguments
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (!string.IsNullOrEmpty(this.ConfigFile))
				{
					stringBuilder.AppendFormat(" --config={0}", this.ConfigFile);
				}
				else
				{
					PropertyInfo[] properties = typeof(PhantomJSDriverService).GetProperties();
					PropertyInfo[] array = properties;
					for (int i = 0; i < array.Length; i++)
					{
						PropertyInfo propertyInfo = array[i];
						if (PhantomJSDriverService.IsSerializableProperty(propertyInfo))
						{
							object value = propertyInfo.GetValue(this, null);
							object propertyDefaultValue = PhantomJSDriverService.GetPropertyDefaultValue(propertyInfo);
							if (value != null && !value.Equals(propertyDefaultValue))
							{
								string propertyCommandLineArgName = PhantomJSDriverService.GetPropertyCommandLineArgName(propertyInfo);
								string propertyCommandLineArgValue = this.GetPropertyCommandLineArgValue(propertyInfo);
								stringBuilder.AppendFormat(" --{0}={1}", propertyCommandLineArgName, propertyCommandLineArgValue);
							}
						}
					}
				}
				if (string.IsNullOrEmpty(this.ghostDriverPath))
				{
					if (string.IsNullOrEmpty(this.address))
					{
						stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --webdriver={0}", new object[]
						{
							base.Port
						});
					}
					else
					{
						stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --webdriver={0}:{1}", new object[]
						{
							this.address,
							base.Port
						});
					}
					if (!string.IsNullOrEmpty(this.logFile))
					{
						stringBuilder.AppendFormat(" --webdriver-logfile=\"{0}\"", this.logFile);
					}
					if (!string.IsNullOrEmpty(this.gridHubUrl))
					{
						stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --webdriver-selenium-grid-hub={0}", new object[]
						{
							this.gridHubUrl
						});
					}
				}
				else
				{
					stringBuilder.AppendFormat(" \"{0}\"", this.ghostDriverPath);
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " --port={0}", new object[]
					{
						base.Port
					});
					if (!string.IsNullOrEmpty(this.logFile))
					{
						stringBuilder.AppendFormat(" --logFile=\"{0}\"", this.logFile);
					}
				}
				if (this.additionalArguments.Count > 0)
				{
					foreach (string current in this.additionalArguments)
					{
						if (!string.IsNullOrEmpty(current.Trim()))
						{
							stringBuilder.AppendFormat(" {0}", current);
						}
					}
				}
				return stringBuilder.ToString();
			}
		}

		private static string PlatformSpecificDriverServiceFileName
		{
			get
			{
				if (!Platform.CurrentPlatform.IsPlatformType(PlatformType.Unix))
				{
					return "PhantomJS.exe";
				}
				return "phantomjs";
			}
		}

		[JsonConstructor]
		private PhantomJSDriverService() : this(FileUtilities.FindFile(PhantomJSDriverService.PhantomJSDriverServiceFileName), PhantomJSDriverService.PhantomJSDriverServiceFileName, PortUtilities.FindFreePort())
		{
		}

		private PhantomJSDriverService(string executablePath, string executableFileName, int port) : base(executablePath, port, executableFileName, PhantomJSDriverService.PhantomJSDownloadUrl)
		{
			this.InitializeProperties();
		}

		public static PhantomJSDriverService CreateDefaultService()
		{
			string driverPath = DriverService.FindDriverServiceExecutable(PhantomJSDriverService.PhantomJSDriverServiceFileName, PhantomJSDriverService.PhantomJSDownloadUrl);
			return PhantomJSDriverService.CreateDefaultService(driverPath);
		}

		public static PhantomJSDriverService CreateDefaultService(string driverPath)
		{
			return PhantomJSDriverService.CreateDefaultService(driverPath, PhantomJSDriverService.PhantomJSDriverServiceFileName);
		}

		public static PhantomJSDriverService CreateDefaultService(string driverPath, string driverExecutableFileName)
		{
			return new PhantomJSDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
		}

		public void AddArgument(string argument)
		{
			if (string.IsNullOrEmpty(argument))
			{
				throw new ArgumentException("argument must not be null or empty", "argument");
			}
			this.AddArguments(new string[]
			{
				argument
			});
		}

		public void AddArguments(params string[] arguments)
		{
			this.AddArguments(new List<string>(arguments));
		}

		public void AddArguments(IEnumerable<string> arguments)
		{
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments", "arguments must not be null");
			}
			this.additionalArguments.AddRange(arguments);
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		private static object GetPropertyDefaultValue(PropertyInfo info)
		{
			object[] customAttributes = info.GetCustomAttributes(typeof(DefaultValueAttribute), false);
			if (customAttributes.Length > 0)
			{
				DefaultValueAttribute defaultValueAttribute = customAttributes[0] as DefaultValueAttribute;
				if (defaultValueAttribute != null)
				{
					return defaultValueAttribute.Value;
				}
			}
			else if (info.PropertyType.IsValueType)
			{
				return Activator.CreateInstance(info.PropertyType);
			}
			return null;
		}

		private static string GetPropertyCommandLineArgName(PropertyInfo info)
		{
			object[] customAttributes = info.GetCustomAttributes(typeof(CommandLineArgumentNameAttribute), false);
			if (customAttributes.Length > 0)
			{
				CommandLineArgumentNameAttribute commandLineArgumentNameAttribute = customAttributes[0] as CommandLineArgumentNameAttribute;
				if (commandLineArgumentNameAttribute != null)
				{
					return commandLineArgumentNameAttribute.Name;
				}
			}
			return null;
		}

		private static bool IsSerializableProperty(PropertyInfo info)
		{
			object[] customAttributes = info.GetCustomAttributes(typeof(JsonPropertyAttribute), true);
			return customAttributes.Length > 0;
		}

		private string GetPropertyCommandLineArgValue(PropertyInfo info)
		{
			object value = info.GetValue(this, null);
			if (info.PropertyType == typeof(string))
			{
				string text = value.ToString();
				if (text.Contains(" "))
				{
					return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[]
					{
						text
					});
				}
				return text;
			}
			else
			{
				if (!(info.PropertyType == typeof(bool)))
				{
					return value.ToString();
				}
				if (!(bool)value)
				{
					return "false";
				}
				return "true";
			}
		}

		private void InitializeProperties()
		{
			PropertyInfo[] properties = typeof(PhantomJSDriverService).GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (PhantomJSDriverService.IsSerializableProperty(propertyInfo))
				{
					object propertyDefaultValue = PhantomJSDriverService.GetPropertyDefaultValue(propertyInfo);
					if (propertyDefaultValue != null)
					{
						propertyInfo.SetValue(this, propertyDefaultValue, null);
					}
				}
			}
		}
	}
}
