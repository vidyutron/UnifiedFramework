using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace OpenQA.Selenium.Chrome
{
	public class ChromeOptions
	{
		private const string ArgumentsChromeOption = "args";

		private const string BinaryChromeOption = "binary";

		private const string ExtensionsChromeOption = "extensions";

		private const string LocalStateChromeOption = "localState";

		private const string PreferencesChromeOption = "prefs";

		private const string DetachChromeOption = "detach";

		private const string DebuggerAddressChromeOption = "debuggerAddress";

		private const string ExcludeSwitchesChromeOption = "excludeSwitches";

		private const string MinidumpPathChromeOption = "minidumpPath";

		private const string MobileEmulationChromeOption = "mobileEmulation";

		private const string PerformanceLoggingPreferencesChromeOption = "perfLoggingPrefs";

		private const string WindowTypesChromeOption = "windowTypes";

		public static readonly string Capability = "chromeOptions";

		private bool leaveBrowserRunning;

		private string binaryLocation;

		private string debuggerAddress;

		private string minidumpPath;

		private List<string> arguments = new List<string>();

		private List<string> extensionFiles = new List<string>();

		private List<string> encodedExtensions = new List<string>();

		private List<string> excludedSwitches = new List<string>();

		private List<string> windowTypes = new List<string>();

		private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();

		private Dictionary<string, object> additionalChromeOptions = new Dictionary<string, object>();

		private Dictionary<string, object> userProfilePreferences;

		private Dictionary<string, object> localStatePreferences;

		private string mobileEmulationDeviceName;

		private ChromeMobileEmulationDeviceSettings mobileEmulationDeviceSettings;

		private ChromePerformanceLoggingPreferences perfLoggingPreferences;

		private Proxy proxy;

		public string BinaryLocation
		{
			get
			{
				return this.binaryLocation;
			}
			set
			{
				this.binaryLocation = value;
			}
		}

		public bool LeaveBrowserRunning
		{
			get
			{
				return this.leaveBrowserRunning;
			}
			set
			{
				this.leaveBrowserRunning = value;
			}
		}

		public Proxy Proxy
		{
			get
			{
				return this.proxy;
			}
			set
			{
				this.proxy = value;
			}
		}

		public ReadOnlyCollection<string> Arguments
		{
			get
			{
				return this.arguments.AsReadOnly();
			}
		}

		public ReadOnlyCollection<string> Extensions
		{
			get
			{
				List<string> list = new List<string>(this.encodedExtensions);
				foreach (string current in this.extensionFiles)
				{
					byte[] inArray = File.ReadAllBytes(current);
					string item = Convert.ToBase64String(inArray);
					list.Add(item);
				}
				return list.AsReadOnly();
			}
		}

		public string DebuggerAddress
		{
			get
			{
				return this.debuggerAddress;
			}
			set
			{
				this.debuggerAddress = value;
			}
		}

		public string MinidumpPath
		{
			get
			{
				return this.minidumpPath;
			}
			set
			{
				this.minidumpPath = value;
			}
		}

		public ChromePerformanceLoggingPreferences PerformanceLoggingPreferences
		{
			get
			{
				return this.perfLoggingPreferences;
			}
			set
			{
				this.perfLoggingPreferences = value;
			}
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

		public void AddArguments(params string[] argumentsToAdd)
		{
			this.AddArguments(new List<string>(argumentsToAdd));
		}

		public void AddArguments(IEnumerable<string> argumentsToAdd)
		{
			if (argumentsToAdd == null)
			{
				throw new ArgumentNullException("argumentsToAdd", "argumentsToAdd must not be null");
			}
			this.arguments.AddRange(argumentsToAdd);
		}

		public void AddExcludedArgument(string argument)
		{
			if (string.IsNullOrEmpty(argument))
			{
				throw new ArgumentException("argument must not be null or empty", "argument");
			}
			this.AddExcludedArguments(new string[]
			{
				argument
			});
		}

		public void AddExcludedArguments(params string[] argumentsToExclude)
		{
			this.AddExcludedArguments(new List<string>(argumentsToExclude));
		}

		public void AddExcludedArguments(IEnumerable<string> argumentsToExclude)
		{
			if (argumentsToExclude == null)
			{
				throw new ArgumentNullException("argumentsToExclude", "argumentsToExclude must not be null");
			}
			this.excludedSwitches.AddRange(argumentsToExclude);
		}

		public void AddExtension(string pathToExtension)
		{
			if (string.IsNullOrEmpty(pathToExtension))
			{
				throw new ArgumentException("pathToExtension must not be null or empty", "pathToExtension");
			}
			this.AddExtensions(new string[]
			{
				pathToExtension
			});
		}

		public void AddExtensions(params string[] extensions)
		{
			this.AddExtensions(new List<string>(extensions));
		}

		public void AddExtensions(IEnumerable<string> extensions)
		{
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions", "extensions must not be null");
			}
			foreach (string current in extensions)
			{
				if (!File.Exists(current))
				{
					throw new FileNotFoundException("No extension found at the specified path", current);
				}
				this.extensionFiles.Add(current);
			}
		}

		public void AddEncodedExtension(string extension)
		{
			if (string.IsNullOrEmpty(extension))
			{
				throw new ArgumentException("extension must not be null or empty", "extension");
			}
			this.AddExtensions(new string[]
			{
				extension
			});
		}

		public void AddEncodedExtensions(params string[] extensions)
		{
			this.AddEncodedExtensions(new List<string>(extensions));
		}

		public void AddEncodedExtensions(IEnumerable<string> extensions)
		{
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions", "extensions must not be null");
			}
			foreach (string current in extensions)
			{
				try
				{
					Convert.FromBase64String(current);
				}
				catch (FormatException innerException)
				{
					throw new WebDriverException("Could not properly decode the base64 string", innerException);
				}
				this.encodedExtensions.Add(current);
			}
		}

		public void AddUserProfilePreference(string preferenceName, object preferenceValue)
		{
			if (this.userProfilePreferences == null)
			{
				this.userProfilePreferences = new Dictionary<string, object>();
			}
			this.userProfilePreferences[preferenceName] = preferenceValue;
		}

		public void AddLocalStatePreference(string preferenceName, object preferenceValue)
		{
			if (this.localStatePreferences == null)
			{
				this.localStatePreferences = new Dictionary<string, object>();
			}
			this.localStatePreferences[preferenceName] = preferenceValue;
		}

		public void EnableMobileEmulation(string deviceName)
		{
			this.mobileEmulationDeviceSettings = null;
			this.mobileEmulationDeviceName = deviceName;
		}

		public void EnableMobileEmulation(ChromeMobileEmulationDeviceSettings deviceSettings)
		{
			this.mobileEmulationDeviceName = null;
			if (deviceSettings != null && string.IsNullOrEmpty(deviceSettings.UserAgent))
			{
				throw new ArgumentException("Device settings must include a user agent string.", "deviceSettings");
			}
			this.mobileEmulationDeviceSettings = deviceSettings;
		}

		[Obsolete("Use the EnableMobileEmulation method instead. This method was released in error, and will be removed in a future release.")]
		public void EnableMobileDeviceEmulation(ChromeMobileEmulationDeviceSettings deviceSettings)
		{
			this.EnableMobileEmulation(deviceSettings);
		}

		public void AddWindowType(string windowType)
		{
			if (string.IsNullOrEmpty(windowType))
			{
				throw new ArgumentException("windowType must not be null or empty", "windowType");
			}
			this.AddWindowTypes(new string[]
			{
				windowType
			});
		}

		public void AddWindowTypes(params string[] windowTypesToAdd)
		{
			this.AddWindowTypes(new List<string>(windowTypesToAdd));
		}

		public void AddWindowTypes(IEnumerable<string> windowTypesToAdd)
		{
			if (windowTypesToAdd == null)
			{
				throw new ArgumentNullException("windowTypesToAdd", "windowTypesToAdd must not be null");
			}
			this.windowTypes.AddRange(windowTypesToAdd);
		}

		public void AddAdditionalCapability(string capabilityName, object capabilityValue)
		{
			this.AddAdditionalCapability(capabilityName, capabilityValue, false);
		}

		public void AddAdditionalCapability(string capabilityName, object capabilityValue, bool isGlobalCapability)
		{
			if (capabilityName == ChromeOptions.Capability || capabilityName == CapabilityType.Proxy || capabilityName == "args" || capabilityName == "binary" || capabilityName == "extensions" || capabilityName == "localState" || capabilityName == "prefs" || capabilityName == "detach" || capabilityName == "debuggerAddress" || capabilityName == "extensions" || capabilityName == "excludeSwitches" || capabilityName == "minidumpPath" || capabilityName == "mobileEmulation" || capabilityName == "perfLoggingPrefs" || capabilityName == "windowTypes")
			{
				string message = string.Format(CultureInfo.InvariantCulture, "There is already an option for the {0} capability. Please use that instead.", new object[]
				{
					capabilityName
				});
				throw new ArgumentException(message, "capabilityName");
			}
			if (string.IsNullOrEmpty(capabilityName))
			{
				throw new ArgumentException("Capability name may not be null an empty string.", "capabilityName");
			}
			if (isGlobalCapability)
			{
				this.additionalCapabilities[capabilityName] = capabilityValue;
				return;
			}
			this.additionalChromeOptions[capabilityName] = capabilityValue;
		}

		public ICapabilities ToCapabilities()
		{
			Dictionary<string, object> capabilityValue = this.BuildChromeOptionsDictionary();
			DesiredCapabilities desiredCapabilities = DesiredCapabilities.Chrome();
			desiredCapabilities.SetCapability(ChromeOptions.Capability, capabilityValue);
			if (this.proxy != null)
			{
				desiredCapabilities.SetCapability(CapabilityType.Proxy, this.proxy);
			}
			foreach (KeyValuePair<string, object> current in this.additionalCapabilities)
			{
				desiredCapabilities.SetCapability(current.Key, current.Value);
			}
			return desiredCapabilities;
		}

		private Dictionary<string, object> BuildChromeOptionsDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (this.Arguments.Count > 0)
			{
				dictionary["args"] = this.Arguments;
			}
			if (!string.IsNullOrEmpty(this.binaryLocation))
			{
				dictionary["binary"] = this.binaryLocation;
			}
			ReadOnlyCollection<string> extensions = this.Extensions;
			if (extensions.Count > 0)
			{
				dictionary["extensions"] = extensions;
			}
			if (this.localStatePreferences != null && this.localStatePreferences.Count > 0)
			{
				dictionary["localState"] = this.localStatePreferences;
			}
			if (this.userProfilePreferences != null && this.userProfilePreferences.Count > 0)
			{
				dictionary["prefs"] = this.userProfilePreferences;
			}
			if (this.leaveBrowserRunning)
			{
				dictionary["detach"] = this.leaveBrowserRunning;
			}
			if (!string.IsNullOrEmpty(this.debuggerAddress))
			{
				dictionary["debuggerAddress"] = this.debuggerAddress;
			}
			if (this.excludedSwitches.Count > 0)
			{
				dictionary["excludeSwitches"] = this.excludedSwitches;
			}
			if (!string.IsNullOrEmpty(this.minidumpPath))
			{
				dictionary["minidumpPath"] = this.minidumpPath;
			}
			if (!string.IsNullOrEmpty(this.mobileEmulationDeviceName) || this.mobileEmulationDeviceSettings != null)
			{
				dictionary["mobileEmulation"] = this.GenerateMobileEmulationSettingsDictionary();
			}
			if (this.perfLoggingPreferences != null)
			{
				dictionary["perfLoggingPrefs"] = this.GeneratePerformanceLoggingPreferencesDictionary();
			}
			if (this.windowTypes.Count > 0)
			{
				dictionary["windowTypes"] = this.windowTypes;
			}
			foreach (KeyValuePair<string, object> current in this.additionalChromeOptions)
			{
				dictionary.Add(current.Key, current.Value);
			}
			return dictionary;
		}

		private Dictionary<string, object> GeneratePerformanceLoggingPreferencesDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["enableNetwork"] = this.perfLoggingPreferences.IsCollectingNetworkEvents;
			dictionary["enablePage"] = this.perfLoggingPreferences.IsCollectingPageEvents;
			dictionary["enableTimeline"] = this.perfLoggingPreferences.IsCollectingTimelineEvents;
			string tracingCategories = this.perfLoggingPreferences.TracingCategories;
			if (!string.IsNullOrEmpty(tracingCategories))
			{
				dictionary["tracingCategories"] = tracingCategories;
			}
			dictionary["bufferUsageReportingInterval"] = Convert.ToInt64(this.perfLoggingPreferences.BufferUsageReportingInterval.TotalMilliseconds);
			return dictionary;
		}

		private Dictionary<string, object> GenerateMobileEmulationSettingsDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (!string.IsNullOrEmpty(this.mobileEmulationDeviceName))
			{
				dictionary["deviceName"] = this.mobileEmulationDeviceName;
			}
			else if (this.mobileEmulationDeviceSettings != null)
			{
				dictionary["userAgent"] = this.mobileEmulationDeviceSettings.UserAgent;
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2["width"] = this.mobileEmulationDeviceSettings.Width;
				dictionary2["height"] = this.mobileEmulationDeviceSettings.Height;
				dictionary2["pixelRatio"] = this.mobileEmulationDeviceSettings.PixelRatio;
				if (!this.mobileEmulationDeviceSettings.EnableTouchEvents)
				{
					dictionary2["touch"] = this.mobileEmulationDeviceSettings.EnableTouchEvents;
				}
			}
			return dictionary;
		}
	}
}
