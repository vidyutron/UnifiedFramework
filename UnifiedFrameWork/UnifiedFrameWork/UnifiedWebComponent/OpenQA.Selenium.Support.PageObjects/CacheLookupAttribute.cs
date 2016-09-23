using System;

namespace OpenQA.Selenium.Support.PageObjects
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class CacheLookupAttribute : Attribute
	{
	}
}
