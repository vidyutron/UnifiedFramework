using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.IE
{
    /// <summary>
    /// Represents the valid values for driver engine available with the IEDriverServer.exe.
    /// </summary>
    public enum InternetExplorerDriverEngine
    {
        /// <summary>
        /// Represents the Legacy value, forcing the driver to use only the open-source
        /// driver engine implementation.
        /// </summary>
        Legacy,

        /// <summary>
        /// Represents the AutoDetect value, instructing the driver to use the vendor-provided
        /// driver engine implementation, if available, falling back to the open-source
        /// implementation, if it is not available.
        /// </summary>
        AutoDetect,

        /// <summary>
        /// Represents the Vendor value, instructing the driver to use the vendor-provided
        /// driver engine implementation, and throwing an exception if it is not available.
        /// </summary>
        Vendor
    }
}