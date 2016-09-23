using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium.Interactions.Internal;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Interface representing basic touch screen operations.
    /// </summary>
    public interface ITouchScreen
    {
        /// <summary>
        /// Allows the execution of single tap on the screen, analogous to click using a Mouse.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen,
        /// usually an <see cref="IWebElement"/>.</param>
        void SingleTap(ICoordinates where);

        /// <summary>
        /// Allows the execution of the gesture 'down' on the screen. It is typically the first of a
        /// sequence of touch gestures.
        /// </summary>
        /// <param name="locationX">The x coordinate relative to the view port.</param>
        /// <param name="locationY">The y coordinate relative to the view port.</param>
        void Down(int locationX, int locationY);

        /// <summary>
        /// Allows the execution of the gesture 'up' on the screen. It is typically the last of a
        /// sequence of touch gestures.
        /// </summary>
        /// <param name="locationX">The x coordinate relative to the view port.</param>
        /// <param name="locationY">The y coordinate relative to the view port.</param>
        void Up(int locationX, int locationY);

        /// <summary>
        /// Allows the execution of the gesture 'move' on the screen.
        /// </summary>
        /// <param name="locationX">The x coordinate relative to the view port.</param>
        /// <param name="locationY">The y coordinate relative to the view port.</param>
        void Move(int locationX, int locationY);

        /// <summary>
        /// Creates a scroll gesture that starts on a particular screen location.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen
        /// where the scroll starts, usually an <see cref="IWebElement"/>.</param>
        /// <param name="offsetX">The x coordinate relative to the view port.</param>
        /// <param name="offsetY">The y coordinate relative to the view port.</param>
        void Scroll(ICoordinates where, int offsetX, int offsetY);

        /// <summary>
        /// Creates a scroll gesture for a particular x and y offset.
        /// </summary>
        /// <param name="offsetX">The horizontal offset relative to the view port.</param>
        /// <param name="offsetY">The vertical offset relative to the view port.</param>
        void Scroll(int offsetX, int offsetY);

        /// <summary>
        /// Allows the execution of double tap on the screen, analogous to click using a Mouse.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen,
        /// usually an <see cref="IWebElement"/>.</param>
        void DoubleTap(ICoordinates where);

        /// <summary>
        /// Allows the execution of a long press gesture on the screen.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen,
        /// usually an <see cref="IWebElement"/>.</param>
        void LongPress(ICoordinates where);

        /// <summary>
        /// Creates a flick gesture for the current view.
        /// </summary>
        /// <param name="speedX">The horizontal speed in pixels per second.</param>
        /// <param name="speedY">The vertical speed in pixels per second.</param>
        void Flick(int speedX, int speedY);

        /// <summary>
        /// Creates a flick gesture for the current view starting at a specific location.
        /// </summary>
        /// <param name="where">The <see cref="ICoordinates"/> object representing the location on the screen
        /// where the scroll starts, usually an <see cref="IWebElement"/>.</param>
        /// <param name="offsetX">The x offset relative to the viewport.</param>
        /// <param name="offsetY">The y offset relative to the viewport.</param>
        /// <param name="speed">The speed in pixels per second.</param>
        void Flick(ICoordinates where, int offsetX, int offsetY, int speed);
    }
}