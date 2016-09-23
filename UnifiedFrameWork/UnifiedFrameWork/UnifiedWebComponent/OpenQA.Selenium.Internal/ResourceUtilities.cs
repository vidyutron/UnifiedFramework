using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace OpenQA.Selenium.Internal
{
	public static class ResourceUtilities
	{
		public static Stream GetResourceStream(string fileName, string resourceId)
		{
			string text = Path.Combine(FileUtilities.GetCurrentDirectory(), Path.GetFileName(fileName));
			Stream stream;
			if (File.Exists(text))
			{
				stream = new FileStream(text, FileMode.Open, FileAccess.Read);
			}
			else if (File.Exists(fileName))
			{
				stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			}
			else
			{
				if (string.IsNullOrEmpty(resourceId))
				{
					throw new WebDriverException("The file specified does not exist, and you have specified no internal resource ID");
				}
				Assembly callingAssembly = Assembly.GetCallingAssembly();
				stream = callingAssembly.GetManifestResourceStream(resourceId);
			}
			if (stream == null)
			{
				throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "Cannot find a file named '{0}' or an embedded resource with the id '{1}'.", new object[]
				{
					text,
					resourceId
				}));
			}
			return stream;
		}

		public static bool IsValidResourceName(string resourceId)
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			List<string> list = new List<string>(callingAssembly.GetManifestResourceNames());
			return list.Contains(resourceId);
		}
	}
}
