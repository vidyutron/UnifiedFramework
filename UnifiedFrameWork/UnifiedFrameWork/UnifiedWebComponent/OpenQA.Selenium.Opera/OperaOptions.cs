using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace OpenQA.Selenium.Opera
{
	public class OperaOptions
	{
		private const string ArgumentsOperaOption = "args";

		private const string BinaryOperaOption = "binary";

		private const string ExtensionsOperaOption = "extensions";

		private const string LocalStateOperaOption = "localState";

		private const string PreferencesOperaOption = "prefs";

		private const string DetachOperaOption = "detach";

		private const string DebuggerAddressOperaOption = "debuggerAddress";

		private const string ExcludeSwitchesOperaOption = "excludeSwitches";

		private const string MinidumpPathOperaOption = "minidumpPath";

		public static readonly string Capability = "operaOptions";

		private bool leaveBrowserRunning;

		private string binaryLocation;

		private string debuggerAddress;

		private string minidumpPath;

		private List<string> arguments = new List<string>();

		private List<string> extensionFiles = new List<string>();

		private List<string> encodedExtensions = new List<string>();

		private List<string> excludedSwitches = new List<string>();

		private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();

		private Dictionary<string, object> additionalOperaOptions = new Dictionary<string, object>();

		private Dictionary<string, object> userProfilePreferences;

		private Dictionary<string, object> localStatePreferences;

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

		public void AddAdditionalCapability(string capabilityName, object capabilityValue)
		{
			this.AddAdditionalCapability(capabilityName, capabilityValue, false);
		}

		public void AddAdditionalCapability(string capabilityName, object capabilityValue, bool isGlobalCapability)
		{
			if (capabilityName == OperaOptions.Capability || capabilityName == CapabilityType.Proxy || capabilityName == "args" || capabilityName == "binary" || capabilityName == "extensions" || capabilityName == "localState" || capabilityName == "prefs" || capabilityName == "detach" || capabilityName == "debuggerAddress" || capabilityName == "extensions" || capabilityName == "excludeSwitches" || capabilityName == "minidumpPath")
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
			this.additionalOperaOptions[capabilityName] = capabilityValue;
		}

		public ICapabilities ToCapabilities()
		{
			Dictionary<string, object> capabilityValue = this.BuildOperaOptionsDictionary();
			DesiredCapabilities desiredCapabilities = DesiredCapabilities.Opera();
			desiredCapabilities.SetCapability(OperaOptions.Capability, capabilityValue);
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

		private Dictionary<string, object> BuildOperaOptionsDictionary()
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
			foreach (KeyValuePair<string, object> current in this.additionalOperaOptions)
			{
				dictionary.Add(current.Key, current.Value);
			}
			return dictionary;
		}
	}
}
