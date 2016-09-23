using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.IE;

namespace UnifiedFrameWork.Controller
{
    public class UnifiedWebControlConfig
    {
        internal static IWebDriver innerDriver;
        internal static WebDriverWait innerWait;

        //Arguments description:-
        //    1. driverServerPath: provide the server executable for the webdriver
        //    2. protectedMode: Set [IntroduceInstabilityByIgnoringProtectedModeSettings] to True or False
        //    3. requireWindowFocus: Set [RequireWindowFocus] to true or False.
        //    4. browserAttachTimeout: Provide [BrowserAttachTimeout] time period.
        //    5. ensureCleanSession: Set [EnsureCleanSession] to true or False.
        public static IWebDriver IWebDriverConfig(string ieDriverServerPath,
            bool protectedMode = true, bool requireWindowFocus = true,
            int browserAttachTimeout = 60, bool ensureCleanSession = false)
        {
            innerDriver = new InternetExplorerDriver(ieDriverServerPath, new InternetExplorerOptions());
            innerDriver.Manage().Window.Maximize();
            innerDriver.Manage().Cookies.DeleteAllCookies();
            //This is needed due to environment configurations
            var options = new InternetExplorerOptions()
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = protectedMode,
                RequireWindowFocus = requireWindowFocus,
                UnexpectedAlertBehavior = InternetExplorerUnexpectedAlertBehavior.Accept,
                BrowserAttachTimeout = TimeSpan.FromSeconds(browserAttachTimeout),
                EnsureCleanSession = ensureCleanSession
            };

            return innerDriver;
        }

        public static WebDriverWait ExplicitWaitConfig(IWebDriver driver, int timeToWait)
        {
            return innerWait= driver != null ? new WebDriverWait(driver, TimeSpan.FromSeconds(timeToWait)) : null;
        }
    }
}
