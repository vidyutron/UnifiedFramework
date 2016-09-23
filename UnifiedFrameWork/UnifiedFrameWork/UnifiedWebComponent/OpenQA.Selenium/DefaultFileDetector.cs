using System;
using System.Collections.Generic;
using System.Text;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Represents the default file detector for determining whether a file
    /// must be uploaded to a remote server.
    /// </summary>
    public class DefaultFileDetector : IFileDetector
    {
        /// <summary>
        /// Returns a value indicating whether a specified key sequence represents
        /// a file name and path.
        /// </summary>
        /// <param name="keySequence">The sequence to test for file existence.</param>
        /// <returns>This method always returns <see langword="false"/> in this implementation.</returns>
        public bool IsFile(string keySequence)
        {
            return false;
        }
    }
}