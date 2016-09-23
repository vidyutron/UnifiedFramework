using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace UnifiedFrameWork.Controller
{
    public class UnifiedWebControl: UnifiedWebControlConfig
    {

        public static IWebElement IsElementPresent(By by)
        {
            try
            {
                return innerDriver != null ? innerDriver.FindElement(by) : null;
            }
            catch (NoSuchElementException ex) { throw ex; }
            
        }

        public static void ClickIdElement(string element)
        {
            try
            {
                innerWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(element))).Click();
            }
            catch (WebDriverTimeoutException ex) { throw ex;/*Log Action*/ }
            catch (NoSuchElementException ex) { throw ex;/*Log Action*/ }
        }
        public static void ClickClassElement(string element)
        {
            try
            {
                innerWait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName(element))).Click();
            }
            catch (WebDriverTimeoutException ex) { throw ex;/*Log Action*/ }
            catch (NoSuchElementException ex) { throw ex;/*Log Action*/ }
        }
        public static void ClickLinkElement(string element)
        {
            try
            {
                innerWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(element))).Click();
            }
            catch (WebDriverTimeoutException ex) { throw ex;/*Log Action*/ }
            catch (NoSuchElementException ex) { throw ex;/*Log Action*/ }
        }
        /// <summary>
        /// Clicking the UI element, which cannot be identified by id,class,link text
        /// </summary>
        /// <param name="by">Clicking the UI element, which cannot be identified by id,class,link text</param>
        public static void ClickElement(By by)
        {
            try
            {
                innerWait.Until(ExpectedConditions.ElementToBeClickable(by)).Click();
            }
            catch (WebDriverTimeoutException ex) { throw ex;/*Log Action*/ }
            catch (NoSuchElementException ex) { throw ex;/*Log Action*/ }
        }

        public static void SendKeysIdElement(string element, string text)
        {
            try
            {
                innerWait.Until(ExpectedConditions.ElementIsVisible(By.Id(element)))
                    .Clear();
                innerWait.Until(ExpectedConditions.ElementIsVisible(By.Id(element)))
                    .SendKeys(text);

            }
            catch (WebDriverTimeoutException ex) { throw ex;/*Log Action*/ }
            catch (NoSuchElementException ex) { throw ex;/*Log Action*/ }
        }
        public static void SendKeysClassElement(string element, string text)
        {
            try
            {
                innerWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(element)))
                    .SendKeys(text);
            }
            catch (WebDriverTimeoutException ex) { throw ex;/*Log Action*/ }
            catch (NoSuchElementException ex) { throw ex;/*Log Action*/ }
        }
        public static void SendKeysElement(By by, string text)
        {
            try
            {
                innerWait.Until(ExpectedConditions.ElementIsVisible(by)).SendKeys(text);
            }
            catch (WebDriverTimeoutException ex) { throw ex;/*Log Action*/ }
            catch (NoSuchElementException ex) { throw ex;/*Log Action*/ }
        }

        public static void SelectElement(By by, string valueElement)
        {
            try
            {
                SelectElement selectElement = new SelectElement(innerWait.Until(ExpectedConditions.
                    ElementIsVisible(by)));
                selectElement.SelectByValue(valueElement);
            }
            catch (WebDriverTimeoutException ex) { throw ex; }
            catch (NoSuchElementException ex) { throw ex; }
        }

        public static string GetTextElement(By by)
        {
            string textValue = string.Empty;
            try
            {
                textValue = innerWait.Until(ExpectedConditions.
                    ElementExists(by)).Text.ToLower();
            }
            catch (WebDriverTimeoutException) { return textValue; }
            catch (NoSuchElementException) { return textValue; }
            return textValue;
        }

        public static void WaitUntillElementIsInvisible(By by)
        {
            //bool returnStatus = false;
            try
            {
                innerWait.Until(ExpectedConditions.InvisibilityOfElementLocated(by));
                //returnStatus = true;
            }
            catch (WebDriverTimeoutException ex) { throw ex; /*return false;Log Action*/ }
            catch (NoSuchElementException ex) { throw ex;/*return false;Log Action*/ }
            //return returnStatus;
        }

        public static IWebElement WaitUntillElementIsVisible(By by)
        {
            //bool returnStatus = false;
            try
            {
               return innerWait.Until(ExpectedConditions.ElementIsVisible(by));
                //returnStatus = true;
            }
            catch (WebDriverTimeoutException ex) { throw ex; /*return false;Log Action*/ }
            catch (NoSuchElementException ex) { throw ex;/*return false;Log Action*/ }
            //return returnStatus;
        }

        public static IList<IWebElement> FindMultipleElements(By by)
        {
            IList<IWebElement> listElements = new List<IWebElement>();
            try
            {
                innerWait.Until(ExpectedConditions.ElementExists(by));
                listElements = innerDriver.FindElements(by);
            }
            catch (WebDriverTimeoutException ex) { throw ex; }
            catch (NoSuchElementException ex) { throw ex; }
            return listElements;
        }

        public static void DoubleClickElement(IWebDriver driver, By by){
            try{
                if(driver!=null)
                new Actions(driver).DoubleClick(innerWait.Until(ExpectedConditions.ElementToBeClickable(by))).Build().Perform();
            }
            catch(WebDriverTimeoutException ex){throw ex;}
            catch(NoSuchElementException ex){throw ex;}
        }

        public static bool CheckElementExistence(IWebElement uIElement)
        {
            try { if(uIElement.Displayed) return true;
                else return false;}
            catch (NoSuchElementException ex) { Console.WriteLine(ex.Message); return false; }
        }
        public static bool CheckElementExistence(By by)
        {
            try
            {
                innerDriver.FindElement(by); return true;
            }
            catch (NoSuchElementException ex) { Console.WriteLine(ex.Message); return false; }
        }

        public static void UIElementScroll(string scrollValue)
        {
            try {
                OpenQA.Selenium.IJavaScriptExecutor js = (OpenQA.Selenium.IJavaScriptExecutor)innerDriver;
                //Amount by which the element is to be scrolled.
                js.ExecuteScript("scroll(0," + scrollValue + ")");
            }
            catch (WebDriverTimeoutException ex) { throw ex; }
            catch (NoSuchElementException ex) { throw ex; }
        }

        public static void HoverAction(By by)
        {
            try {
                IWebElement webElement = innerDriver.FindElement(by);
                new Actions(innerDriver).MoveToElement(webElement).Build().Perform();
            }
            catch (WebDriverTimeoutException ex) { throw ex; }
            catch (NoSuchElementException ex) { throw ex; }
        }

    }
}
