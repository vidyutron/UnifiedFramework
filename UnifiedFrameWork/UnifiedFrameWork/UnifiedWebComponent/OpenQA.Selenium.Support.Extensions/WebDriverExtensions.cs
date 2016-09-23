using OpenQA.Selenium.Remote;
using System;
using System.Reflection;

namespace OpenQA.Selenium.Support.Extensions
{
	public static class WebDriverExtensions
	{
		public static Screenshot TakeScreenshot(this IWebDriver driver)
		{
			ITakesScreenshot takesScreenshot = driver as ITakesScreenshot;
			if (takesScreenshot != null)
			{
				return takesScreenshot.GetScreenshot();
			}
			IHasCapabilities hasCapabilities = driver as IHasCapabilities;
			if (hasCapabilities == null)
			{
				throw new WebDriverException("Driver does not implement ITakesScreenshot or IHasCapabilities");
			}
			if (!hasCapabilities.Capabilities.HasCapability(CapabilityType.TakesScreenshot) || !(bool)hasCapabilities.Capabilities.GetCapability(CapabilityType.TakesScreenshot))
			{
				throw new WebDriverException("Driver capabilities do not support taking screenshots");
			}
			MethodInfo method = driver.GetType().GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodBase arg_7E_0 = method;
			object[] array = new object[2];
			array[0] = DriverCommand.Screenshot;
			Response response = arg_7E_0.Invoke(driver, array) as Response;
			if (response == null)
			{
				throw new WebDriverException("Unexpected failure getting screenshot; response was not in the proper format.");
			}
			string base64EncodedScreenshot = response.Value.ToString();
			return new Screenshot(base64EncodedScreenshot);
		}

		public static T ExecuteJavaScript<T>(this IWebDriver driver, string script, params object[] args)
		{
			IJavaScriptExecutor javaScriptExecutor = driver as IJavaScriptExecutor;
			if (javaScriptExecutor == null)
			{
				throw new WebDriverException("Driver does not implement IJavaScriptExecutor");
			}
			Type typeFromHandle = typeof(T);
			T result = default(T);
			object obj = javaScriptExecutor.ExecuteScript(script, args);
			if (obj == null)
			{
				if (typeFromHandle.IsValueType && Nullable.GetUnderlyingType(typeFromHandle) == null)
				{
					throw new WebDriverException("Script returned null, but desired type is a value type");
				}
			}
			else
			{
				if (!typeFromHandle.IsInstanceOfType(obj))
				{
					throw new WebDriverException("Script returned a value, but the result could not be cast to the desired type");
				}
				result = (T)((object)obj);
			}
			return result;
		}
	}
}
