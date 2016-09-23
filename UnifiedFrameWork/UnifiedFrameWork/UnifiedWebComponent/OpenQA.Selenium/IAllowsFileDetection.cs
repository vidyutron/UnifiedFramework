using System;
using System.Collections.Generic;
using System.Text;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Interface determining whether the driver implementation allows detection of files
    /// when sending keystrokes to a file upload element.
    /// </summary>
    public interface IAllowsFileDetection
    {
        /// <summary>
        /// Gets or sets the <see cref="IFileDetector"/> responsible for detecting
        /// sequences of keystrokes representing file paths and names.
        /// </summary>
        IFileDetector FileDetector { get; set; }
    }
}