using System;
using System.Collections.Generic;
using System.Text;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Represents rotation of the browser view for orientation-sensitive devices.
    /// When using this with a real device, the device should not be moved so that
    /// the built-in sensors do not interfere.
    /// </summary>
    public interface IRotatable
    {
        /// <summary>
        /// Gets or sets the screen orientation of the browser on the device.
        /// </summary>
        ScreenOrientation Orientation { get; set; }
    }
}