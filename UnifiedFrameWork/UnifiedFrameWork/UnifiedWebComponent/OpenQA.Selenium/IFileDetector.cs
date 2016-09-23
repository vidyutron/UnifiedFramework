using System;
using System.Collections.Generic;
using System.Text;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Defines an object responsible for detecting sequences of keystrokes
    /// representing file paths and names.
    /// </summary>
    public interface IFileDetector
    {
        /// <summary>
        /// Returns a value indicating whether a specified key sequence represents
        /// a file name and path.
        /// </summary>
        /// <param name="keySequence">The sequence to test for file existence.</param>
        /// <returns><see langword="true"/> if the key sequence represents a file; otherwise, <see langword="false"/>.</returns>
        bool IsFile(string keySequence);
    }
}