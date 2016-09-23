using System;

namespace OpenQA.Selenium.Remote
{
	public static class CapabilityType
	{
		public static readonly string BrowserName = "browserName";

		public static readonly string Platform = "platform";

		public static readonly string Version = "version";

		public static readonly string IsJavaScriptEnabled = "javascriptEnabled";

		public static readonly string TakesScreenshot = "takesScreenshot";

		public static readonly string HandlesAlerts = "handlesAlerts";

		public static readonly string SupportsFindingByCss = "cssSelectorsEnabled";

		public static readonly string Proxy = "proxy";

		public static readonly string Rotatable = "rotatable";

		public static readonly string AcceptSslCertificates = "acceptSslCerts";

		public static readonly string HasNativeEvents = "nativeEvents";

		public static readonly string UnexpectedAlertBehavior = "unexpectedAlertBehaviour";

		public static readonly string PageLoadStrategy = "pageLoadStrategy";

		public static readonly string SupportsLocationContext = "locationContextEnabled";

		public static readonly string SupportsApplicationCache = "applicationCacheEnabled";

		public static readonly string SupportsWebStorage = "webStorageEnabled";
	}
}
