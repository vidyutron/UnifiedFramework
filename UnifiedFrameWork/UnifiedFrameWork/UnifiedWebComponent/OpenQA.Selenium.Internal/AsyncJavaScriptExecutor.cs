using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;

namespace OpenQA.Selenium.Internal
{
	public class AsyncJavaScriptExecutor
	{
		private const string AsyncScriptTemplate = "document.__$webdriverPageId = '{0}';\r\nvar timeoutId = window.setTimeout(function() {{\r\n  window.setTimeout(function() {{\r\n    document.__$webdriverAsyncTimeout = 1;\r\n  }}, 0);\r\n}}, {1});\r\ndocument.__$webdriverAsyncTimeout = 0;\r\nvar callback = function(value) {{\r\n  document.__$webdriverAsyncTimeout = 0;\r\n  document.__$webdriverAsyncScriptResult = value;\r\n  window.clearTimeout(timeoutId);\r\n}};\r\nvar argsArray = Array.prototype.slice.call(arguments);\r\nargsArray.push(callback);\r\nif (document.__$webdriverAsyncScriptResult !== undefined) {{\r\n  delete document.__$webdriverAsyncScriptResult;\r\n}}\r\n(function() {{\r\n{2}\r\n}}).apply(null, argsArray);";

		private const string PollingScriptTemplate = "var pendingId = '{0}';\r\nif (document.__$webdriverPageId != '{1}') {{\r\n  return [pendingId, -1];\r\n}} else if ('__$webdriverAsyncScriptResult' in document) {{\r\n  var value = document.__$webdriverAsyncScriptResult;\r\n  delete document.__$webdriverAsyncScriptResult;\r\n  return value;\r\n}} else {{\r\n  return [pendingId, document.__$webdriverAsyncTimeout];\r\n}}\r\n";

		private IJavaScriptExecutor executor;

		private TimeSpan timeout = TimeSpan.FromMilliseconds(0.0);

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		public AsyncJavaScriptExecutor(IJavaScriptExecutor executor)
		{
			this.executor = executor;
		}

		public object ExecuteScript(string script, object[] args)
		{
			string text = Guid.NewGuid().ToString();
			string script2 = string.Format(CultureInfo.InvariantCulture, "document.__$webdriverPageId = '{0}';\r\nvar timeoutId = window.setTimeout(function() {{\r\n  window.setTimeout(function() {{\r\n    document.__$webdriverAsyncTimeout = 1;\r\n  }}, 0);\r\n}}, {1});\r\ndocument.__$webdriverAsyncTimeout = 0;\r\nvar callback = function(value) {{\r\n  document.__$webdriverAsyncTimeout = 0;\r\n  document.__$webdriverAsyncScriptResult = value;\r\n  window.clearTimeout(timeoutId);\r\n}};\r\nvar argsArray = Array.prototype.slice.call(arguments);\r\nargsArray.push(callback);\r\nif (document.__$webdriverAsyncScriptResult !== undefined) {{\r\n  delete document.__$webdriverAsyncScriptResult;\r\n}}\r\n(function() {{\r\n{2}\r\n}}).apply(null, argsArray);", new object[]
			{
				text,
				this.timeout.TotalMilliseconds,
				script
			});
			string text2 = Guid.NewGuid().ToString();
			string script3 = string.Format(CultureInfo.InvariantCulture, "var pendingId = '{0}';\r\nif (document.__$webdriverPageId != '{1}') {{\r\n  return [pendingId, -1];\r\n}} else if ('__$webdriverAsyncScriptResult' in document) {{\r\n  var value = document.__$webdriverAsyncScriptResult;\r\n  delete document.__$webdriverAsyncScriptResult;\r\n  return value;\r\n}} else {{\r\n  return [pendingId, document.__$webdriverAsyncTimeout];\r\n}}\r\n", new object[]
			{
				text2,
				text
			});
			DateTime now = DateTime.Now;
			this.executor.ExecuteScript(script2, args);
			TimeSpan timeSpan;
			while (true)
			{
				object obj = this.executor.ExecuteScript(script3, new object[0]);
				ReadOnlyCollection<object> readOnlyCollection = obj as ReadOnlyCollection<object>;
				if (readOnlyCollection == null || readOnlyCollection.Count != 2 || !(text2 == readOnlyCollection[0].ToString()))
				{
					return obj;
				}
				long num = (long)readOnlyCollection[1];
				if (num < 0L)
				{
					break;
				}
				timeSpan = DateTime.Now - now;
				if (num > 0L)
				{
					goto Block_5;
				}
				Thread.Sleep(100);
			}
			throw new WebDriverException("Detected a new page load while waiting for async script result.\nScript: " + script);
			Block_5:
			throw new WebDriverTimeoutException(string.Concat(new object[]
			{
				"Timed out waiting for async script callback.\nElapsed time: ",
				timeSpan.Milliseconds,
				"milliseconds\nScript: ",
				script
			}));
		}
	}
}
