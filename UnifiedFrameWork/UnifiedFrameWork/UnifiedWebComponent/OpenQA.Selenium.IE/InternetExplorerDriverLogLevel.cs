using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.IE
{
    /// <summary>
    /// Represents the valid values of logging levels available with the IEDriverServer.exe.
    /// </summary>
    public enum InternetExplorerDriverLogLevel
    {
        /// <summary>
        /// Represents the Trace value, the most detailed logging level available.
        /// </summary>
        Trace,

        /// <summary>
        /// Represents the Debug value
        /// </summary>
        Debug,

        /// <summary>
        /// Represents the Info value
        /// </summary>
        Info,

        /// <summary>
        /// Represents the Warn value
        /// </summary>
        Warn,

        /// <summary>
        /// Represents the Error value
        /// </summary>
        Error,

        /// <summary>
        /// Represents the Fatal value, the least detailed logging level available.
        /// </summary>
        Fatal
    }
}