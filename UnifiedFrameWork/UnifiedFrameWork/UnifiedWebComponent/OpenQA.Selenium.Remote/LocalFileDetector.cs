using System;
using System.IO;

namespace OpenQA.Selenium.Remote
{
	public class LocalFileDetector : IFileDetector
	{
		public bool IsFile(string keySequence)
		{
			return File.Exists(keySequence);
		}
	}
}
