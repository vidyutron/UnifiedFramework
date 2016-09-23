using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace OpenQA.Selenium.Internal
{
	internal static class FileUtilities
	{
		public static bool CopyDirectory(string sourceDirectory, string destinationDirectory)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirectory);
			DirectoryInfo directoryInfo2 = new DirectoryInfo(destinationDirectory);
			if (directoryInfo.Exists)
			{
				if (!directoryInfo2.Exists)
				{
					directoryInfo2.Create();
				}
				FileInfo[] files = directoryInfo.GetFiles();
				for (int i = 0; i < files.Length; i++)
				{
					FileInfo fileInfo = files[i];
					fileInfo.CopyTo(Path.Combine(directoryInfo2.FullName, fileInfo.Name));
				}
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				for (int j = 0; j < directories.Length; j++)
				{
					DirectoryInfo directoryInfo3 = directories[j];
					if (!FileUtilities.CopyDirectory(directoryInfo3.FullName, Path.Combine(directoryInfo2.FullName, directoryInfo3.Name)))
					{
					}
				}
			}
			return true;
		}

		public static void DeleteDirectory(string directoryToDelete)
		{
			int num = 0;
			while (Directory.Exists(directoryToDelete) && num < 10)
			{
				try
				{
					Directory.Delete(directoryToDelete, true);
				}
				catch (IOException)
				{
					Thread.Sleep(500);
				}
				catch (UnauthorizedAccessException)
				{
					Thread.Sleep(500);
				}
				finally
				{
					num++;
				}
			}
			if (Directory.Exists(directoryToDelete))
			{
				Console.WriteLine("Unable to delete directory '{0}'", directoryToDelete);
			}
		}

		public static string FindFile(string fileName)
		{
			string currentDirectory = FileUtilities.GetCurrentDirectory();
			if (File.Exists(Path.Combine(currentDirectory, fileName)))
			{
				return currentDirectory;
			}
			string environmentVariable = Environment.GetEnvironmentVariable("PATH");
			if (!string.IsNullOrEmpty(environmentVariable))
			{
				string text = Environment.ExpandEnvironmentVariables(environmentVariable);
				string[] array = text.Split(new char[]
				{
					Path.PathSeparator
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i];
					if (text2.IndexOfAny(Path.GetInvalidPathChars()) < 0 && File.Exists(Path.Combine(text2, fileName)))
					{
						return text2;
					}
				}
			}
			return string.Empty;
		}

		public static string GetCurrentDirectory()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string directoryName = Path.GetDirectoryName(executingAssembly.Location);
			if (AppDomain.CurrentDomain.ShadowCopyFiles)
			{
				Uri uri = new Uri(executingAssembly.CodeBase);
				directoryName = Path.GetDirectoryName(uri.LocalPath);
			}
			return directoryName;
		}

		public static string GenerateRandomTempDirectoryName(string directoryPattern)
		{
			string path = string.Format(CultureInfo.InvariantCulture, directoryPattern, new object[]
			{
				Guid.NewGuid().ToString("N")
			});
			return Path.Combine(Path.GetTempPath(), path);
		}
	}
}
