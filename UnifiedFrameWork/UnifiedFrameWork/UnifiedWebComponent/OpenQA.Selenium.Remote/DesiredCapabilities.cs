using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
	public class DesiredCapabilities : ICapabilities
	{
		private readonly Dictionary<string, object> capabilities = new Dictionary<string, object>();

		public string BrowserName
		{
			get
			{
				string result = string.Empty;
				object capability = this.GetCapability(CapabilityType.BrowserName);
				if (capability != null)
				{
					result = capability.ToString();
				}
				return result;
			}
		}

		public Platform Platform
		{
			get
			{
				return (this.GetCapability(CapabilityType.Platform) as Platform) ?? new Platform(PlatformType.Any);
			}
			set
			{
				this.SetCapability(CapabilityType.Platform, value);
			}
		}

		public string Version
		{
			get
			{
				string result = string.Empty;
				object capability = this.GetCapability(CapabilityType.Version);
				if (capability != null)
				{
					result = capability.ToString();
				}
				return result;
			}
		}

		public bool IsJavaScriptEnabled
		{
			get
			{
				bool result = false;
				object capability = this.GetCapability(CapabilityType.IsJavaScriptEnabled);
				if (capability != null)
				{
					result = (bool)capability;
				}
				return result;
			}
			set
			{
				this.SetCapability(CapabilityType.IsJavaScriptEnabled, value);
			}
		}

		internal Dictionary<string, object> CapabilitiesDictionary
		{
			get
			{
				return this.capabilities;
			}
		}

		public DesiredCapabilities(string browser, string version, Platform platform)
		{
			this.SetCapability(CapabilityType.BrowserName, browser);
			this.SetCapability(CapabilityType.Version, version);
			this.SetCapability(CapabilityType.Platform, platform);
		}

		public DesiredCapabilities()
		{
		}

		public DesiredCapabilities(Dictionary<string, object> rawMap)
		{
			if (rawMap != null)
			{
				foreach (string current in rawMap.Keys)
				{
					if (current == CapabilityType.Platform)
					{
						object obj = rawMap[CapabilityType.Platform];
						string text = obj as string;
						Platform platform = obj as Platform;
						if (text != null)
						{
							this.SetCapability(CapabilityType.Platform, Platform.FromString(text));
						}
						else if (platform != null)
						{
							this.SetCapability(CapabilityType.Platform, platform);
						}
					}
					else
					{
						this.SetCapability(current, rawMap[current]);
					}
				}
			}
		}

		public static DesiredCapabilities Firefox()
		{
			return new DesiredCapabilities("firefox", string.Empty, new Platform(PlatformType.Any));
		}

		public static DesiredCapabilities PhantomJS()
		{
			return new DesiredCapabilities("phantomjs", string.Empty, new Platform(PlatformType.Any));
		}

		public static DesiredCapabilities InternetExplorer()
		{
			return new DesiredCapabilities("internet explorer", string.Empty, new Platform(PlatformType.Windows));
		}

		public static DesiredCapabilities Edge()
		{
			return new DesiredCapabilities("MicrosoftEdge", string.Empty, new Platform(PlatformType.Windows));
		}

		public static DesiredCapabilities HtmlUnit()
		{
			return new DesiredCapabilities("htmlunit", string.Empty, new Platform(PlatformType.Any));
		}

		public static DesiredCapabilities HtmlUnitWithJavaScript()
		{
			return new DesiredCapabilities("htmlunit", string.Empty, new Platform(PlatformType.Any))
			{
				IsJavaScriptEnabled = true
			};
		}

		public static DesiredCapabilities IPhone()
		{
			return new DesiredCapabilities("iPhone", string.Empty, new Platform(PlatformType.Mac));
		}

		public static DesiredCapabilities IPad()
		{
			return new DesiredCapabilities("iPad", string.Empty, new Platform(PlatformType.Mac));
		}

		public static DesiredCapabilities Chrome()
		{
			return new DesiredCapabilities("chrome", string.Empty, new Platform(PlatformType.Any))
			{
				IsJavaScriptEnabled = true
			};
		}

		public static DesiredCapabilities Android()
		{
			return new DesiredCapabilities("android", string.Empty, new Platform(PlatformType.Android));
		}

		public static DesiredCapabilities Opera()
		{
			return new DesiredCapabilities("opera", string.Empty, new Platform(PlatformType.Any));
		}

		public static DesiredCapabilities Safari()
		{
			return new DesiredCapabilities("safari", string.Empty, new Platform(PlatformType.Mac));
		}

		public bool HasCapability(string capability)
		{
			return this.capabilities.ContainsKey(capability);
		}

		public object GetCapability(string capability)
		{
			object obj = null;
			if (this.capabilities.ContainsKey(capability))
			{
				obj = this.capabilities[capability];
				string text = obj as string;
				if (capability == CapabilityType.Platform && text != null)
				{
					obj = Platform.FromString(obj.ToString());
				}
			}
			return obj;
		}

		public void SetCapability(string capability, object capabilityValue)
		{
			Platform platform = capabilityValue as Platform;
			if (platform != null)
			{
				this.capabilities[capability] = platform.ProtocolPlatformType;
				return;
			}
			this.capabilities[capability] = capabilityValue;
		}

		public Dictionary<string, object> ToDictionary()
		{
			return this.capabilities;
		}

		public override int GetHashCode()
		{
			int num = (this.BrowserName != null) ? this.BrowserName.GetHashCode() : 0;
			num = 31 * num + ((this.Version != null) ? this.Version.GetHashCode() : 0);
			num = 31 * num + ((this.Platform != null) ? this.Platform.GetHashCode() : 0);
			return 31 * num + (this.IsJavaScriptEnabled ? 1 : 0);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Capabilities [BrowserName={0}, IsJavaScriptEnabled={1}, Platform={2}, Version={3}]", new object[]
			{
				this.BrowserName,
				this.IsJavaScriptEnabled,
				this.Platform.PlatformType.ToString(),
				this.Version
			});
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			DesiredCapabilities desiredCapabilities = obj as DesiredCapabilities;
			return desiredCapabilities != null && this.IsJavaScriptEnabled == desiredCapabilities.IsJavaScriptEnabled && !((this.BrowserName != null) ? (this.BrowserName != desiredCapabilities.BrowserName) : (desiredCapabilities.BrowserName != null)) && this.Platform.IsPlatformType(desiredCapabilities.Platform.PlatformType) && !((this.Version != null) ? (this.Version != desiredCapabilities.Version) : (desiredCapabilities.Version != null));
		}
	}
}
