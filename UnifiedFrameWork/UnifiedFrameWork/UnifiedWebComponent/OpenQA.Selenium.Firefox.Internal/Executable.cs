using Microsoft.Win32;
using OpenQA.Selenium.Internal;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace OpenQA.Selenium.Firefox.Internal
{
	internal class Executable
	{
		private readonly string binaryInDefaultLocationForPlatform;

		private string binaryLocation;

		public string ExecutablePath
		{
			get
			{
				return this.binaryLocation;
			}
		}

		public Executable(string userSpecifiedBinaryPath)
		{
			if (!string.IsNullOrEmpty(userSpecifiedBinaryPath))
			{
				if (File.Exists(userSpecifiedBinaryPath))
				{
					this.binaryLocation = userSpecifiedBinaryPath;
					return;
				}
				throw new WebDriverException("Specified firefox binary location does not exist or is not a real file: " + userSpecifiedBinaryPath);
			}
			else
			{
				this.binaryInDefaultLocationForPlatform = Executable.LocateFirefoxBinaryFromPlatform();
				if (this.binaryInDefaultLocationForPlatform != null && File.Exists(this.binaryInDefaultLocationForPlatform))
				{
					this.binaryLocation = this.binaryInDefaultLocationForPlatform;
					return;
				}
				throw new WebDriverException("Cannot find Firefox binary in PATH or default install locations. Make sure Firefox is installed. OS appears to be: " + Platform.CurrentPlatform.ToString());
			}
		}

		[SecurityPermission(SecurityAction.Demand)]
		public void SetLibraryPath(Process builder)
		{
			string libraryPathPropertyName = Executable.GetLibraryPathPropertyName();
			StringBuilder stringBuilder = new StringBuilder();
			string environmentVariable = Executable.GetEnvironmentVariable(libraryPathPropertyName, null);
			if (environmentVariable != null)
			{
				stringBuilder.Append(environmentVariable).Append(Path.PathSeparator);
			}
			if (builder.StartInfo.EnvironmentVariables.ContainsKey(libraryPathPropertyName))
			{
				stringBuilder.Append(builder.StartInfo.EnvironmentVariables[libraryPathPropertyName]).Append(Path.PathSeparator);
			}
			string fullPath = Path.GetFullPath(this.binaryLocation);
			if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Mac) && Platform.CurrentPlatform.MinorVersion > 5)
			{
				stringBuilder.Append(Path.PathSeparator);
			}
			else
			{
				stringBuilder.Insert(0, Path.PathSeparator).Insert(0, fullPath);
			}
			if (builder.StartInfo.EnvironmentVariables.ContainsKey(libraryPathPropertyName))
			{
				builder.StartInfo.EnvironmentVariables[libraryPathPropertyName] = stringBuilder.ToString();
				return;
			}
			builder.StartInfo.EnvironmentVariables.Add(libraryPathPropertyName, stringBuilder.ToString());
		}

		[SecurityPermission(SecurityAction.Demand)]
		private static string LocateFirefoxBinaryFromPlatform()
		{
			string text = string.Empty;
			if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
			{
				string name = "SOFTWARE\\Mozilla\\Mozilla Firefox";
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
				if (registryKey == null)
				{
					registryKey = Registry.CurrentUser.OpenSubKey(name);
				}
				if (registryKey != null)
				{
					text = Executable.GetExecutablePathUsingRegistry(registryKey);
				}
				else
				{
					string[] defaultInstallLocations = new string[]
					{
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Mozilla Firefox"),
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + " (x86)", "Mozilla Firefox")
					};
					text = Executable.GetExecutablePathUsingDefaultInstallLocations(defaultInstallLocations, "Firefox.exe");
				}
			}
			else
			{
				string[] defaultInstallLocations2 = new string[]
				{
					"/Applications/Firefox.app/Contents/MacOS",
					string.Format(CultureInfo.InvariantCulture, "/Users/{0}/Applications/Firefox.app/Contents/MacOS", new object[]
					{
						Environment.UserName
					})
				};
				text = Executable.GetExecutablePathUsingDefaultInstallLocations(defaultInstallLocations2, "firefox-bin");
				if (string.IsNullOrEmpty(text))
				{
					using (Process process = new Process())
					{
						process.StartInfo.FileName = "which";
						process.StartInfo.Arguments = "firefox";
						process.StartInfo.CreateNoWindow = true;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.UseShellExecute = false;
						process.Start();
						process.WaitForExit();
						text = process.StandardOutput.ReadToEnd().Trim();
					}
				}
			}
			if (text != null && File.Exists(text))
			{
				return text;
			}
			return Executable.FindBinary(new string[]
			{
				"firefox3",
				"firefox"
			});
		}

		private static string GetExecutablePathUsingRegistry(RegistryKey mozillaKey)
		{
			string text = (string)mozillaKey.GetValue("CurrentVersion");
			if (string.IsNullOrEmpty(text))
			{
				throw new WebDriverException("Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox correctly");
			}
			RegistryKey registryKey = mozillaKey.OpenSubKey(string.Format(CultureInfo.InvariantCulture, "{0}\\Main", new object[]
			{
				text
			}));
			if (registryKey == null)
			{
				throw new WebDriverException("Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox correctly");
			}
			string text2 = (string)registryKey.GetValue("PathToExe");
			if (!File.Exists(text2))
			{
				throw new WebDriverException("FireFox executable listed in the registry does not exist, please make sure you have installed FireFox correctly");
			}
			return text2;
		}

		private static string GetExecutablePathUsingDefaultInstallLocations(string[] defaultInstallLocations, string exeName)
		{
			for (int i = 0; i < defaultInstallLocations.Length; i++)
			{
				string path = defaultInstallLocations[i];
				string text = Path.Combine(path, exeName);
				if (File.Exists(text))
				{
					return text;
				}
			}
			return null;
		}

		private static string GetEnvironmentVariable(string name, string defaultValue)
		{
			string text = Environment.GetEnvironmentVariable(name);
			if (string.IsNullOrEmpty(text))
			{
				text = defaultValue;
			}
			return text;
		}

		private static string GetLibraryPathPropertyName()
		{
			string result = "LD_LIBRARY_PATH";
			if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
			{
				result = "PATH";
			}
			else if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Mac))
			{
				result = "DYLD_LIBRARY_PATH";
			}
			return result;
		}

		private static string FindBinary(string[] binaryNames)
		{
			for (int i = 0; i < binaryNames.Length; i++)
			{
				string text = binaryNames[i];
				string text2 = text;
				if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
				{
					text2 += ".exe";
				}
				string text3 = FileUtilities.FindFile(text2);
				if (!string.IsNullOrEmpty(text3))
				{
					return Path.Combine(text3, text2);
				}
			}
			return null;
		}
	}
}
