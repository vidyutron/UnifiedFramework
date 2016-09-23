using OpenQA.Selenium.Firefox.Internal;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace OpenQA.Selenium.Firefox
{
	public class FirefoxBinary : IDisposable
	{
		private const string NoFocusLibraryName = "x_ignore_nofocus.so";

		private Dictionary<string, string> extraEnv = new Dictionary<string, string>();

		private Executable executable;

		private Process process;

		private TimeSpan timeout = TimeSpan.FromSeconds(45.0);

		private bool isDisposed;

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		internal Executable BinaryExecutable
		{
			get
			{
				return this.executable;
			}
		}

		protected static bool IsOnLinux
		{
			get
			{
				return Platform.CurrentPlatform.IsPlatformType(PlatformType.Linux);
			}
		}

		protected Dictionary<string, string> ExtraEnvironmentVariables
		{
			get
			{
				return this.extraEnv;
			}
		}

		public FirefoxBinary() : this(null)
		{
		}

		public FirefoxBinary(string pathToFirefoxBinary)
		{
			this.executable = new Executable(pathToFirefoxBinary);
		}

		[SecurityPermission(SecurityAction.Demand)]
		public void StartProfile(FirefoxProfile profile, params string[] commandLineArguments)
		{
			if (profile == null)
			{
				throw new ArgumentNullException("profile", "profile cannot be null");
			}
			if (commandLineArguments == null)
			{
				commandLineArguments = new string[0];
			}
			string profileDirectory = profile.ProfileDirectory;
			this.SetEnvironmentProperty("XRE_PROFILE_PATH", profileDirectory);
			this.SetEnvironmentProperty("MOZ_NO_REMOTE", "1");
			this.SetEnvironmentProperty("MOZ_CRASHREPORTER_DISABLE", "1");
			this.SetEnvironmentProperty("NO_EM_RESTART", "1");
			if (FirefoxBinary.IsOnLinux && (profile.EnableNativeEvents || profile.AlwaysLoadNoFocusLibrary))
			{
				this.ModifyLinkLibraryPath(profile);
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = commandLineArguments;
			for (int i = 0; i < array.Length; i++)
			{
				string value = array[i];
				stringBuilder.Append(" ").Append(value);
			}
			this.process = new Process();
			this.process.StartInfo.FileName = this.BinaryExecutable.ExecutablePath;
			this.process.StartInfo.Arguments = stringBuilder.ToString();
			this.process.StartInfo.UseShellExecute = false;
			foreach (string current in this.extraEnv.Keys)
			{
				if (this.process.StartInfo.EnvironmentVariables.ContainsKey(current))
				{
					this.process.StartInfo.EnvironmentVariables[current] = this.extraEnv[current];
				}
				else
				{
					this.process.StartInfo.EnvironmentVariables.Add(current, this.extraEnv[current]);
				}
			}
			this.BinaryExecutable.SetLibraryPath(this.process);
			this.StartFirefoxProcess();
			this.CopeWithTheStrangenessOfTheMac();
		}

		public void SetEnvironmentProperty(string propertyName, string value)
		{
			if (string.IsNullOrEmpty(propertyName) || value == null)
			{
				throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "You must set both the property name and value: {0}, {1}", new object[]
				{
					propertyName,
					value
				}));
			}
			if (this.extraEnv.ContainsKey(propertyName))
			{
				this.extraEnv[propertyName] = value;
				return;
			}
			this.extraEnv.Add(propertyName, value);
		}

		[SecurityPermission(SecurityAction.Demand)]
		public void WaitForProcessExit()
		{
			this.process.WaitForExit();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override string ToString()
		{
			return "FirefoxBinary(" + this.executable.ExecutablePath + ")";
		}

		[SecurityPermission(SecurityAction.Demand)]
		protected void StartFirefoxProcess()
		{
			this.process.Start();
		}

		[SecurityPermission(SecurityAction.Demand)]
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing && this.process != null)
				{
					if (!this.process.HasExited)
					{
						Thread.Sleep(1000);
					}
					if (!this.process.HasExited)
					{
						this.process.Kill();
					}
					this.process.Dispose();
					this.process = null;
				}
				this.isDisposed = true;
			}
		}

		private static void Sleep(int timeInMilliseconds)
		{
			try
			{
				Thread.Sleep(timeInMilliseconds);
			}
			catch (ThreadInterruptedException innerException)
			{
				throw new WebDriverException("Thread was interrupted", innerException);
			}
		}

		private static string ExtractAndCheck(FirefoxProfile profile, string noFocusSoName, string libraryPath32Bit, string libraryPath64Bit)
		{
			List<string> list = new List<string>();
			list.Add(libraryPath32Bit);
			list.Add(libraryPath64Bit);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string current in list)
			{
				string text = Path.Combine(profile.ProfileDirectory, current);
				string path = Path.Combine(text, noFocusSoName);
				string resourceId = string.Format(CultureInfo.InvariantCulture, "WebDriver.FirefoxNoFocus.{0}.dll", new object[]
				{
					current
				});
				if (ResourceUtilities.IsValidResourceName(resourceId))
				{
					using (Stream resourceStream = ResourceUtilities.GetResourceStream(noFocusSoName, resourceId))
					{
						Directory.CreateDirectory(text);
						using (FileStream fileStream = File.Create(path))
						{
							byte[] array = new byte[1000];
							for (int i = resourceStream.Read(array, 0, array.Length); i > 0; i = resourceStream.Read(array, 0, array.Length))
							{
								fileStream.Write(array, 0, i);
							}
						}
					}
				}
				if (!File.Exists(path))
				{
					throw new WebDriverException("Could not locate " + current + ": native events will not work.");
				}
				stringBuilder.Append(text).Append(Path.PathSeparator);
			}
			return stringBuilder.ToString();
		}

		private void ModifyLinkLibraryPath(FirefoxProfile profile)
		{
			string environmentVariable = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
			string text = FirefoxBinary.ExtractAndCheck(profile, "x_ignore_nofocus.so", "x86", "x64");
			if (!string.IsNullOrEmpty(environmentVariable))
			{
				text += environmentVariable;
			}
			this.SetEnvironmentProperty("LD_LIBRARY_PATH", text);
			this.SetEnvironmentProperty("LD_PRELOAD", "x_ignore_nofocus.so");
		}

		[SecurityPermission(SecurityAction.Demand)]
		private void CopeWithTheStrangenessOfTheMac()
		{
			if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Mac))
			{
				try
				{
					Thread.Sleep(300);
					if (this.process.ExitCode == 0)
					{
						return;
					}
					Thread.Sleep(10000);
					this.StartFirefoxProcess();
				}
				catch (ThreadStateException)
				{
				}
				try
				{
					FirefoxBinary.Sleep(300);
					if (this.process.ExitCode != 0)
					{
						StringBuilder stringBuilder = new StringBuilder("Unable to start firefox cleanly.\n");
						stringBuilder.Append("Exit value: ").Append(this.process.ExitCode.ToString(CultureInfo.InvariantCulture)).Append("\n");
						stringBuilder.Append("Ran from: ").Append(this.process.StartInfo.FileName).Append("\n");
						throw new WebDriverException(stringBuilder.ToString());
					}
				}
				catch (ThreadStateException)
				{
				}
			}
		}
	}
}
