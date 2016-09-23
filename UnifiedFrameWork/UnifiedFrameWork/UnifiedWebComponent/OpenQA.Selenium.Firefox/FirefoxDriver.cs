using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenQA.Selenium.Firefox
{
	public class FirefoxDriver : RemoteWebDriver
	{
		public static readonly string ProfileCapabilityName = "firefox_profile";

		public static readonly string BinaryCapabilityName = "firefox_binary";

		public static readonly int DefaultPort = 7055;

		public static readonly bool DefaultEnableNativeEvents = Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows);

		public static readonly bool AcceptUntrustedCertificates = true;

		public static readonly bool AssumeUntrustedCertificateIssuer = true;

		private FirefoxBinary binary;

		private FirefoxProfile profile;

		public override IFileDetector FileDetector
		{
			get
			{
				return base.FileDetector;
			}
			set
			{
			}
		}

		public bool IsMarionette
		{
			get
			{
				return base.IsSpecificationCompliant;
			}
		}

		protected FirefoxBinary Binary
		{
			get
			{
				return this.binary;
			}
		}

		protected FirefoxProfile Profile
		{
			get
			{
				return this.profile;
			}
		}

		public FirefoxDriver() : this(new FirefoxBinary(), null)
		{
		}

		public FirefoxDriver(FirefoxProfile profile) : this(new FirefoxBinary(), profile)
		{
		}

		public FirefoxDriver(ICapabilities capabilities) : this(FirefoxDriver.ExtractBinary(capabilities), FirefoxDriver.ExtractProfile(capabilities), capabilities, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public FirefoxDriver(FirefoxBinary binary, FirefoxProfile profile) : this(binary, profile, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public FirefoxDriver(FirefoxBinary binary, FirefoxProfile profile, TimeSpan commandTimeout) : this(binary, profile, DesiredCapabilities.Firefox(), commandTimeout)
		{
		}

		public FirefoxDriver(FirefoxOptions options) : this(FirefoxDriverService.CreateDefaultService(), options, RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public FirefoxDriver(FirefoxDriverService service) : this(service, new FirefoxOptions(), RemoteWebDriver.DefaultCommandTimeout)
		{
		}

		public FirefoxDriver(FirefoxDriverService service, FirefoxOptions options, TimeSpan commandTimeout) : base(new DriverServiceCommandExecutor(service, commandTimeout), FirefoxDriver.ConvertOptionsToCapabilities(options))
		{
		}

		private FirefoxDriver(FirefoxBinary binary, FirefoxProfile profile, ICapabilities capabilities, TimeSpan commandTimeout) : base(FirefoxDriver.CreateExtensionConnection(binary, profile, commandTimeout), FirefoxDriver.RemoveUnneededCapabilities(capabilities))
		{
			this.binary = binary;
			this.profile = profile;
		}

		protected virtual void PrepareEnvironment()
		{
		}

		protected override RemoteWebElement CreateElement(string elementId)
		{
			return new FirefoxWebElement(this, elementId);
		}

		private static FirefoxBinary ExtractBinary(ICapabilities capabilities)
		{
			if (capabilities.GetCapability(FirefoxDriver.BinaryCapabilityName) != null)
			{
				string pathToFirefoxBinary = capabilities.GetCapability(FirefoxDriver.BinaryCapabilityName).ToString();
				return new FirefoxBinary(pathToFirefoxBinary);
			}
			return new FirefoxBinary();
		}

		private static FirefoxProfile ExtractProfile(ICapabilities capabilities)
		{
			FirefoxProfile firefoxProfile = new FirefoxProfile();
			if (capabilities.GetCapability(FirefoxDriver.ProfileCapabilityName) != null)
			{
				object capability = capabilities.GetCapability(FirefoxDriver.ProfileCapabilityName);
				FirefoxProfile firefoxProfile2 = capability as FirefoxProfile;
				string text = capability as string;
				if (firefoxProfile2 != null)
				{
					firefoxProfile = firefoxProfile2;
				}
				else if (text != null)
				{
					try
					{
						firefoxProfile = FirefoxProfile.FromBase64String(text);
					}
					catch (IOException innerException)
					{
						throw new WebDriverException("Unable to create profile from specified string", innerException);
					}
				}
			}
			if (capabilities.GetCapability(CapabilityType.Proxy) != null)
			{
				Proxy proxyPreferences = null;
				object capability2 = capabilities.GetCapability(CapabilityType.Proxy);
				Proxy proxy = capability2 as Proxy;
				Dictionary<string, object> dictionary = capability2 as Dictionary<string, object>;
				if (proxy != null)
				{
					proxyPreferences = proxy;
				}
				else if (dictionary != null)
				{
					proxyPreferences = new Proxy(dictionary);
				}
				firefoxProfile.SetProxyPreferences(proxyPreferences);
			}
			if (capabilities.GetCapability(CapabilityType.AcceptSslCertificates) != null)
			{
				bool acceptUntrustedCertificates = (bool)capabilities.GetCapability(CapabilityType.AcceptSslCertificates);
				firefoxProfile.AcceptUntrustedCertificates = acceptUntrustedCertificates;
			}
			return firefoxProfile;
		}

		private static ICommandExecutor CreateExtensionConnection(FirefoxBinary binary, FirefoxProfile profile, TimeSpan commandTimeout)
		{
			FirefoxProfile firefoxProfile = profile;
			string environmentVariable = Environment.GetEnvironmentVariable("webdriver.firefox.profile");
			if (firefoxProfile == null && environmentVariable != null)
			{
				firefoxProfile = new FirefoxProfileManager().GetProfile(environmentVariable);
			}
			else if (firefoxProfile == null)
			{
				firefoxProfile = new FirefoxProfile();
			}
			return new FirefoxDriverCommandExecutor(binary, firefoxProfile, "localhost", commandTimeout);
		}

		private static ICapabilities RemoveUnneededCapabilities(ICapabilities capabilities)
		{
			DesiredCapabilities desiredCapabilities = capabilities as DesiredCapabilities;
			desiredCapabilities.CapabilitiesDictionary.Remove(FirefoxDriver.ProfileCapabilityName);
			desiredCapabilities.CapabilitiesDictionary.Remove(FirefoxDriver.BinaryCapabilityName);
			return desiredCapabilities;
		}

		private static ICapabilities ConvertOptionsToCapabilities(FirefoxOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options", "options must not be null");
			}
			return options.ToCapabilities();
		}
	}
}
