using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;

namespace OpenQA.Selenium.IE
{
    /// <summary>
    /// Provides a way to access Internet Explorer to run your tests by creating a InternetExplorerDriver instance
    /// </summary>
    /// <remarks>
    /// When the WebDriver object has been instantiated the browser will load. The test can then navigate to the URL under test and
    /// start your test.
    /// </remarks>
    /// <example>
    /// <code>
    /// [TestFixture]
    /// public class Testing
    /// {
    ///     private IWebDriver driver;
    ///     <para></para>
    ///     [SetUp]
    ///     public void SetUp()
    ///     {
    ///         driver = new InternetExplorerDriver();
    ///     }
    ///     <para></para>
    ///     [Test]
    ///     public void TestGoogle()
    ///     {
    ///         driver.Navigate().GoToUrl("http://www.google.co.uk");
    ///         /*
    ///         *   Rest of the test
    ///         */
    ///     }
    ///     <para></para>
    ///     [TearDown]
    ///     public void TearDown()
    ///     {
    ///         driver.Quit();
    ///         driver.Dispose();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class InternetExplorerDriver : RemoteWebDriver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriver"/> class.
        /// </summary>
        public InternetExplorerDriver()
            : this(new InternetExplorerOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriver"/> class with the desired
        /// options.
        /// </summary>
        /// <param name="options">The <see cref="InternetExplorerOptions"/> used to initialize the driver.</param>
        public InternetExplorerDriver(InternetExplorerOptions options)
            : this(InternetExplorerDriverService.CreateDefaultService(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriver"/> class using the specified driver service.
        /// </summary>
        /// <param name="service">The <see cref="InternetExplorerDriverService"/> used to initialize the driver.</param>
        public InternetExplorerDriver(InternetExplorerDriverService service)
            : this(service, new InternetExplorerOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriver"/> class using the specified path
        /// to the directory containing IEDriverServer.exe.
        /// </summary>
        /// <param name="internetExplorerDriverServerDirectory">The full path to the directory containing IEDriverServer.exe.</param>
        public InternetExplorerDriver(string internetExplorerDriverServerDirectory)
            : this(internetExplorerDriverServerDirectory, new InternetExplorerOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriver"/> class using the specified path
        /// to the directory containing IEDriverServer.exe and options.
        /// </summary>
        /// <param name="internetExplorerDriverServerDirectory">The full path to the directory containing IEDriverServer.exe.</param>
        /// <param name="options">The <see cref="InternetExplorerOptions"/> used to initialize the driver.</param>
        public InternetExplorerDriver(string internetExplorerDriverServerDirectory, InternetExplorerOptions options)
            : this(internetExplorerDriverServerDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriver"/> class using the specified path
        /// to the directory containing IEDriverServer.exe, options, and command timeout.
        /// </summary>
        /// <param name="internetExplorerDriverServerDirectory">The full path to the directory containing IEDriverServer.exe.</param>
        /// <param name="options">The <see cref="InternetExplorerOptions"/> used to initialize the driver.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public InternetExplorerDriver(string internetExplorerDriverServerDirectory, InternetExplorerOptions options, TimeSpan commandTimeout)
            : this(InternetExplorerDriverService.CreateDefaultService(internetExplorerDriverServerDirectory), options, commandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriver"/> class using the specified
        /// <see cref="InternetExplorerDriverService"/> and options.
        /// </summary>
        /// <param name="service">The <see cref="DriverService"/> to use.</param>
        /// <param name="options">The <see cref="InternetExplorerOptions"/> used to initialize the driver.</param>
        public InternetExplorerDriver(InternetExplorerDriverService service, InternetExplorerOptions options)
            : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriver"/> class using the specified
        /// <see cref="DriverService"/>, <see cref="InternetExplorerOptions"/>, and command timeout.
        /// </summary>
        /// <param name="service">The <see cref="InternetExplorerDriverService"/> to use.</param>
        /// <param name="options">The <see cref="InternetExplorerOptions"/> used to initialize the driver.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public InternetExplorerDriver(InternetExplorerDriverService service, InternetExplorerOptions options, TimeSpan commandTimeout)
            : base(new DriverServiceCommandExecutor(service, commandTimeout), ConvertOptionsToCapabilities(options))
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="IFileDetector"/> responsible for detecting
        /// sequences of keystrokes representing file paths and names.
        /// </summary>
        /// <remarks>The IE driver does not allow a file detector to be set,
        /// as the server component of the IE driver (IEDriverServer.exe) only
        /// allows uploads from the local computer environment. Attempting to set
        /// this property has no effect, but does not throw an exception. If you
        /// are attempting to run the IE driver remotely, use <see cref="RemoteWebDriver"/>
        /// in conjunction with a standalone WebDriver server.</remarks>
        public override IFileDetector FileDetector
        {
            get { return base.FileDetector; }
            set { }
        }

        private static ICapabilities ConvertOptionsToCapabilities(InternetExplorerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options", "options must not be null");
            }

            return options.ToCapabilities();
        }
    }
}