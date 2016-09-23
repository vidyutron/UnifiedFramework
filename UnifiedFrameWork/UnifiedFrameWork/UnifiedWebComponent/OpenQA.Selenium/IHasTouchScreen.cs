using System;
using System.Collections.Generic;
using System.Text;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Interface implemented by each driver that allows access to touch screen capabilities.
    /// </summary>
    public interface IHasTouchScreen
    {
        /// <summary>
        /// Gets the device representing the touch screen.
        /// </summary>
        ITouchScreen TouchScreen { get; }
    }
}