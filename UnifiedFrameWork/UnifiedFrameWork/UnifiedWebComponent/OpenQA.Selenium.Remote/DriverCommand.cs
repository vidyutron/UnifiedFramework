using System;

namespace OpenQA.Selenium.Remote
{
	public static class DriverCommand
	{
		public static readonly string DefineDriverMapping = "defineDriverMapping";

		public static readonly string Status = "status";

		public static readonly string NewSession = "newSession";

		public static readonly string GetSessionList = "getSessionList";

		public static readonly string GetSessionCapabilities = "getSessionCapabilities";

		public static readonly string Close = "close";

		public static readonly string Quit = "quit";

		public static readonly string Get = "get";

		public static readonly string GoBack = "goBack";

		public static readonly string GoForward = "goForward";

		public static readonly string Refresh = "refresh";

		public static readonly string AddCookie = "addCookie";

		public static readonly string GetAllCookies = "getCookies";

		public static readonly string GetCookie = "getCookie";

		public static readonly string DeleteCookie = "deleteCookie";

		public static readonly string DeleteAllCookies = "deleteAllCookies";

		public static readonly string FindElement = "findElement";

		public static readonly string FindElements = "findElements";

		public static readonly string FindChildElement = "findChildElement";

		public static readonly string FindChildElements = "findChildElements";

		public static readonly string DescribeElement = "describeElement";

		public static readonly string ClearElement = "clearElement";

		public static readonly string ClickElement = "clickElement";

		public static readonly string SendKeysToElement = "sendKeysToElement";

		public static readonly string TapElement = "tapElement";

		public static readonly string SubmitElement = "submitElement";

		public static readonly string GetCurrentWindowHandle = "getCurrentWindowHandle";

		public static readonly string GetWindowHandles = "getWindowHandles";

		public static readonly string SwitchToWindow = "switchToWindow";

		public static readonly string SwitchToFrame = "switchToFrame";

		public static readonly string SwitchToParentFrame = "switchToParentFrame";

		public static readonly string GetActiveElement = "getActiveElement";

		public static readonly string GetCurrentUrl = "getCurrentUrl";

		public static readonly string GetPageSource = "getPageSource";

		public static readonly string GetTitle = "getTitle";

		public static readonly string ExecuteScript = "executeScript";

		public static readonly string ExecuteAsyncScript = "executeAsyncScript";

		public static readonly string GetElementText = "getElementText";

		public static readonly string GetElementTagName = "getElementTagName";

		public static readonly string IsElementSelected = "isElementSelected";

		public static readonly string IsElementEnabled = "isElementEnabled";

		public static readonly string IsElementDisplayed = "isElementDisplayed";

		public static readonly string GetElementLocation = "getElementLocation";

		public static readonly string GetElementLocationOnceScrolledIntoView = "getElementLocationOnceScrolledIntoView";

		public static readonly string GetElementSize = "getElementSize";

		public static readonly string GetElementRect = "getElementRect";

		public static readonly string GetElementAttribute = "getElementAttribute";

		public static readonly string GetElementProperty = "getElementProperty";

		public static readonly string GetElementValueOfCssProperty = "getElementValueOfCssProperty";

		public static readonly string ElementEquals = "elementEquals";

		public static readonly string Screenshot = "screenshot";

		public static readonly string ElementScreenshot = "elementScreenshot";

		public static readonly string GetOrientation = "getOrientation";

		public static readonly string SetOrientation = "setOrientation";

		public static readonly string GetWindowSize = "getWindowSize";

		public static readonly string SetWindowSize = "setWindowSize";

		public static readonly string GetWindowPosition = "getWindowPosition";

		public static readonly string SetWindowPosition = "setWindowPosition";

		public static readonly string MaximizeWindow = "maximizeWindow";

		public static readonly string FullScreenWindow = "fullScreenWindow";

		public static readonly string DismissAlert = "dismissAlert";

		public static readonly string AcceptAlert = "acceptAlert";

		public static readonly string GetAlertText = "getAlertText";

		public static readonly string SetAlertValue = "setAlertValue";

		public static readonly string SetAlertCredentials = "setAlertCredentials";

		public static readonly string ImplicitlyWait = "implicitlyWait";

		public static readonly string SetAsyncScriptTimeout = "setScriptTimeout";

		public static readonly string SetTimeout = "setTimeout";

		public static readonly string Actions = "actions";

		public static readonly string MouseClick = "mouseClick";

		public static readonly string MouseDoubleClick = "mouseDoubleClick";

		public static readonly string MouseDown = "mouseDown";

		public static readonly string MouseUp = "mouseUp";

		public static readonly string MouseMoveTo = "mouseMoveTo";

		public static readonly string SendKeysToActiveElement = "sendKeysToActiveElement";

		public static readonly string UploadFile = "uploadFile";

		public static readonly string TouchSingleTap = "touchSingleTap";

		public static readonly string TouchPress = "touchDown";

		public static readonly string TouchRelease = "touchUp";

		public static readonly string TouchMove = "touchMove";

		public static readonly string TouchScroll = "touchScroll";

		public static readonly string TouchDoubleTap = "touchDoubleTap";

		public static readonly string TouchLongPress = "touchLongPress";

		public static readonly string TouchFlick = "touchFlick";

		public static readonly string GetLocation = "getLocation";

		public static readonly string SetLocation = "setLocation";

		public static readonly string GetAppCache = "getAppCache";

		public static readonly string GetAppCacheStatus = "getStatus";

		public static readonly string ClearAppCache = "clearAppCache";

		public static readonly string GetLocalStorageItem = "getLocalStorageItem";

		public static readonly string GetLocalStorageKeys = "getLocalStorageKeys";

		public static readonly string SetLocalStorageItem = "setLocalStorageItem";

		public static readonly string RemoveLocalStorageItem = "removeLocalStorageItem";

		public static readonly string ClearLocalStorage = "clearLocalStorage";

		public static readonly string GetLocalStorageSize = "getLocalStorageSize";

		public static readonly string GetSessionStorageItem = "getSessionStorageItem";

		public static readonly string GetSessionStorageKeys = "getSessionStorageKeys";

		public static readonly string SetSessionStorageItem = "setSessionStorageItem";

		public static readonly string RemoveSessionStorageItem = "removeSessionStorageItem";

		public static readonly string ClearSessionStorage = "clearSessionStorage";

		public static readonly string GetSessionStorageSize = "getSessionStorageSize";
	}
}
