using System;

namespace OpenQA.Selenium.Remote
{
	public sealed class W3CWireProtocolCommandInfoRepository : CommandInfoRepository
	{
		public override int SpecificationLevel
		{
			get
			{
				return 1;
			}
		}

		public W3CWireProtocolCommandInfoRepository()
		{
			this.InitializeCommandDictionary();
		}

		protected override void InitializeCommandDictionary()
		{
			base.TryAddCommand(DriverCommand.DefineDriverMapping, new CommandInfo("POST", "/config/drivers"));
			base.TryAddCommand(DriverCommand.Status, new CommandInfo("GET", "/status"));
			base.TryAddCommand(DriverCommand.NewSession, new CommandInfo("POST", "/session"));
			base.TryAddCommand(DriverCommand.Quit, new CommandInfo("DELETE", "/session/{sessionId}"));
			base.TryAddCommand(DriverCommand.Get, new CommandInfo("POST", "/session/{sessionId}/url"));
			base.TryAddCommand(DriverCommand.GetCurrentUrl, new CommandInfo("GET", "/session/{sessionId}/url"));
			base.TryAddCommand(DriverCommand.GoBack, new CommandInfo("POST", "/session/{sessionId}/back"));
			base.TryAddCommand(DriverCommand.GoForward, new CommandInfo("POST", "/session/{sessionId}/forward"));
			base.TryAddCommand(DriverCommand.Refresh, new CommandInfo("POST", "/session/{sessionId}/refresh"));
			base.TryAddCommand(DriverCommand.GetTitle, new CommandInfo("GET", "/session/{sessionId}/title"));
			base.TryAddCommand(DriverCommand.GetCurrentWindowHandle, new CommandInfo("GET", "/session/{sessionId}/window"));
			base.TryAddCommand(DriverCommand.Close, new CommandInfo("DELETE", "/session/{sessionId}/window"));
			base.TryAddCommand(DriverCommand.SwitchToWindow, new CommandInfo("POST", "/session/{sessionId}/window"));
			base.TryAddCommand(DriverCommand.GetWindowHandles, new CommandInfo("GET", "/session/{sessionId}/window/handles"));
			base.TryAddCommand(DriverCommand.FullScreenWindow, new CommandInfo("POST", "/session/{sessionId}/window/fullscreen"));
			base.TryAddCommand(DriverCommand.MaximizeWindow, new CommandInfo("POST", "/session/{sessionId}/window/maximize"));
			base.TryAddCommand(DriverCommand.GetWindowSize, new CommandInfo("GET", "/session/{sessionId}/window/size"));
			base.TryAddCommand(DriverCommand.SetWindowSize, new CommandInfo("POST", "/session/{sessionId}/window/size"));
			base.TryAddCommand(DriverCommand.SwitchToFrame, new CommandInfo("POST", "/session/{sessionId}/frame"));
			base.TryAddCommand(DriverCommand.SwitchToParentFrame, new CommandInfo("POST", "/session/{sessionId}/frame/parent"));
			base.TryAddCommand(DriverCommand.FindElement, new CommandInfo("POST", "/session/{sessionId}/element"));
			base.TryAddCommand(DriverCommand.FindElements, new CommandInfo("POST", "/session/{sessionId}/elements"));
			base.TryAddCommand(DriverCommand.GetActiveElement, new CommandInfo("POST", "/session/{sessionId}/element/active"));
			base.TryAddCommand(DriverCommand.IsElementDisplayed, new CommandInfo("GET", "/session/{sessionId}/element/{id}/displayed"));
			base.TryAddCommand(DriverCommand.IsElementSelected, new CommandInfo("GET", "/session/{sessionId}/element/{id}/selected"));
			base.TryAddCommand(DriverCommand.GetElementAttribute, new CommandInfo("GET", "/session/{sessionId}/element/{id}/attribute/{name}"));
			base.TryAddCommand(DriverCommand.GetElementProperty, new CommandInfo("GET", "/session/{sessionId}/element/{id}/property/{name}"));
			base.TryAddCommand(DriverCommand.GetElementValueOfCssProperty, new CommandInfo("GET", "/session/{sessionId}/element/{id}/css/{name}"));
			base.TryAddCommand(DriverCommand.GetElementText, new CommandInfo("GET", "/session/{sessionId}/element/{id}/text"));
			base.TryAddCommand(DriverCommand.GetElementTagName, new CommandInfo("GET", "/session/{sessionId}/element/{id}/name"));
			base.TryAddCommand(DriverCommand.GetElementRect, new CommandInfo("GET", "/session/{sessionId}/element/{id}/rect"));
			base.TryAddCommand(DriverCommand.IsElementEnabled, new CommandInfo("GET", "/session/{sessionId}/element/{id}/enabled"));
			base.TryAddCommand(DriverCommand.ExecuteScript, new CommandInfo("POST", "/session/{sessionId}/execute/sync"));
			base.TryAddCommand(DriverCommand.ExecuteAsyncScript, new CommandInfo("POST", "/session/{sessionId}/execute/async"));
			base.TryAddCommand(DriverCommand.GetAllCookies, new CommandInfo("GET", "/session/{sessionId}/cookie"));
			base.TryAddCommand(DriverCommand.AddCookie, new CommandInfo("POST", "/session/{sessionId}/cookie"));
			base.TryAddCommand(DriverCommand.DeleteAllCookies, new CommandInfo("DELETE", "/session/{sessionId}/cookie"));
			base.TryAddCommand(DriverCommand.GetCookie, new CommandInfo("POST", "/session/{sessionId}/cookie/{name}"));
			base.TryAddCommand(DriverCommand.DeleteCookie, new CommandInfo("DELETE", "/session/{sessionId}/cookie/{name}"));
			base.TryAddCommand(DriverCommand.SetTimeout, new CommandInfo("POST", "/session/{sessionId}/timeouts"));
			base.TryAddCommand(DriverCommand.Actions, new CommandInfo("POST", "/session/{sessionId}/actions"));
			base.TryAddCommand(DriverCommand.ClickElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/click"));
			base.TryAddCommand(DriverCommand.TapElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/tap"));
			base.TryAddCommand(DriverCommand.ClearElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/clear"));
			base.TryAddCommand(DriverCommand.SendKeysToElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/value"));
			base.TryAddCommand(DriverCommand.DismissAlert, new CommandInfo("POST", "/session/{sessionId}/alert/dismiss"));
			base.TryAddCommand(DriverCommand.AcceptAlert, new CommandInfo("POST", "/session/{sessionId}/alert/accept"));
			base.TryAddCommand(DriverCommand.GetAlertText, new CommandInfo("GET", "/session/{sessionId}/alert/text"));
			base.TryAddCommand(DriverCommand.SetAlertValue, new CommandInfo("POST", "/session/{sessionId}/alert/text"));
			base.TryAddCommand(DriverCommand.SetAlertCredentials, new CommandInfo("POST", "/session/{sessionId}/alert/credentials"));
			base.TryAddCommand(DriverCommand.Screenshot, new CommandInfo("GET", "/session/{sessionId}/screenshot"));
			base.TryAddCommand(DriverCommand.ElementScreenshot, new CommandInfo("GET", "/session/{sessionId}/screenshot/{id}"));
			base.TryAddCommand(DriverCommand.FindChildElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/element"));
			base.TryAddCommand(DriverCommand.FindChildElements, new CommandInfo("POST", "/session/{sessionId}/element/{id}/elements"));
			base.TryAddCommand(DriverCommand.GetSessionList, new CommandInfo("GET", "/sessions"));
			base.TryAddCommand(DriverCommand.GetSessionCapabilities, new CommandInfo("GET", "/session/{sessionId}"));
			base.TryAddCommand(DriverCommand.GetPageSource, new CommandInfo("GET", "/session/{sessionId}/source"));
			base.TryAddCommand(DriverCommand.DescribeElement, new CommandInfo("GET", "/session/{sessionId}/element/{id}"));
			base.TryAddCommand(DriverCommand.SubmitElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/submit"));
			base.TryAddCommand(DriverCommand.GetElementLocation, new CommandInfo("GET", "/session/{sessionId}/element/{id}/location"));
			base.TryAddCommand(DriverCommand.GetElementLocationOnceScrolledIntoView, new CommandInfo("GET", "/session/{sessionId}/element/{id}/location_in_view"));
			base.TryAddCommand(DriverCommand.ElementEquals, new CommandInfo("GET", "/session/{sessionId}/element/{id}/equals/{other}"));
			base.TryAddCommand(DriverCommand.GetWindowPosition, new CommandInfo("GET", "/session/{sessionId}/window/{windowHandle}/position"));
			base.TryAddCommand(DriverCommand.SetWindowPosition, new CommandInfo("POST", "/session/{sessionId}/window/{windowHandle}/position"));
			base.TryAddCommand(DriverCommand.GetOrientation, new CommandInfo("GET", "/session/{sessionId}/orientation"));
			base.TryAddCommand(DriverCommand.SetOrientation, new CommandInfo("POST", "/session/{sessionId}/orientation"));
			base.TryAddCommand(DriverCommand.ImplicitlyWait, new CommandInfo("POST", "/session/{sessionId}/timeouts/implicit_wait"));
			base.TryAddCommand(DriverCommand.SetAsyncScriptTimeout, new CommandInfo("POST", "/session/{sessionId}/timeouts/async_script"));
			base.TryAddCommand(DriverCommand.MouseClick, new CommandInfo("POST", "/session/{sessionId}/click"));
			base.TryAddCommand(DriverCommand.MouseDoubleClick, new CommandInfo("POST", "/session/{sessionId}/doubleclick"));
			base.TryAddCommand(DriverCommand.MouseDown, new CommandInfo("POST", "/session/{sessionId}/buttondown"));
			base.TryAddCommand(DriverCommand.MouseUp, new CommandInfo("POST", "/session/{sessionId}/buttonup"));
			base.TryAddCommand(DriverCommand.MouseMoveTo, new CommandInfo("POST", "/session/{sessionId}/moveto"));
			base.TryAddCommand(DriverCommand.SendKeysToActiveElement, new CommandInfo("POST", "/session/{sessionId}/keys"));
			base.TryAddCommand(DriverCommand.TouchSingleTap, new CommandInfo("POST", "/session/{sessionId}/touch/click"));
			base.TryAddCommand(DriverCommand.TouchPress, new CommandInfo("POST", "/session/{sessionId}/touch/down"));
			base.TryAddCommand(DriverCommand.TouchRelease, new CommandInfo("POST", "/session/{sessionId}/touch/up"));
			base.TryAddCommand(DriverCommand.TouchMove, new CommandInfo("POST", "/session/{sessionId}/touch/move"));
			base.TryAddCommand(DriverCommand.TouchScroll, new CommandInfo("POST", "/session/{sessionId}/touch/scroll"));
			base.TryAddCommand(DriverCommand.TouchDoubleTap, new CommandInfo("POST", "/session/{sessionId}/touch/doubleclick"));
			base.TryAddCommand(DriverCommand.TouchLongPress, new CommandInfo("POST", "/session/{sessionId}/touch/longclick"));
			base.TryAddCommand(DriverCommand.TouchFlick, new CommandInfo("POST", "/session/{sessionId}/touch/flick"));
			base.TryAddCommand(DriverCommand.UploadFile, new CommandInfo("POST", "/session/{sessionId}/file"));
			base.TryAddCommand(DriverCommand.GetLocation, new CommandInfo("GET", "/session/{sessionId}/location"));
			base.TryAddCommand(DriverCommand.SetLocation, new CommandInfo("POST", "/session/{sessionId}/location"));
			base.TryAddCommand(DriverCommand.GetAppCache, new CommandInfo("GET", "/session/{sessionId}/application_cache"));
			base.TryAddCommand(DriverCommand.GetAppCacheStatus, new CommandInfo("GET", "/session/{sessionId}/application_cache/status"));
			base.TryAddCommand(DriverCommand.ClearAppCache, new CommandInfo("DELETE", "/session/{sessionId}/application_cache/clear"));
			base.TryAddCommand(DriverCommand.GetLocalStorageKeys, new CommandInfo("GET", "/session/{sessionId}/local_storage"));
			base.TryAddCommand(DriverCommand.SetLocalStorageItem, new CommandInfo("POST", "/session/{sessionId}/local_storage"));
			base.TryAddCommand(DriverCommand.ClearLocalStorage, new CommandInfo("DELETE", "/session/{sessionId}/local_storage"));
			base.TryAddCommand(DriverCommand.GetLocalStorageItem, new CommandInfo("GET", "/session/{sessionId}/local_storage/key/{key}"));
			base.TryAddCommand(DriverCommand.RemoveLocalStorageItem, new CommandInfo("DELETE", "/session/{sessionId}/local_storage/key/{key}"));
			base.TryAddCommand(DriverCommand.GetLocalStorageSize, new CommandInfo("GET", "/session/{sessionId}/local_storage/size"));
			base.TryAddCommand(DriverCommand.GetSessionStorageKeys, new CommandInfo("GET", "/session/{sessionId}/session_storage"));
			base.TryAddCommand(DriverCommand.SetSessionStorageItem, new CommandInfo("POST", "/session/{sessionId}/session_storage"));
			base.TryAddCommand(DriverCommand.ClearSessionStorage, new CommandInfo("DELETE", "/session/{sessionId}/session_storage"));
			base.TryAddCommand(DriverCommand.GetSessionStorageItem, new CommandInfo("GET", "/session/{sessionId}/session_storage/key/{key}"));
			base.TryAddCommand(DriverCommand.RemoveSessionStorageItem, new CommandInfo("DELETE", "/session/{sessionId}/session_storage/key/{key}"));
			base.TryAddCommand(DriverCommand.GetSessionStorageSize, new CommandInfo("GET", "/session/{sessionId}/session_storage/size"));
		}
	}
}
