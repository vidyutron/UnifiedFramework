using System;
using System.Globalization;
using System.Reflection;

namespace OpenQA.Selenium.Support.PageObjects
{
	internal static class ByFactory
	{
		public static By From(FindsByAttribute attribute)
		{
			How how = attribute.How;
			string @using = attribute.Using;
			switch (how)
			{
			case How.Id:
				return By.Id(@using);
			case How.Name:
				return By.Name(@using);
			case How.TagName:
				return By.TagName(@using);
			case How.ClassName:
				return By.ClassName(@using);
			case How.CssSelector:
				return By.CssSelector(@using);
			case How.LinkText:
				return By.LinkText(@using);
			case How.PartialLinkText:
				return By.PartialLinkText(@using);
			case How.XPath:
				return By.XPath(@using);
			case How.Custom:
			{
				if (attribute.CustomFinderType == null)
				{
					throw new ArgumentException("Cannot use How.Custom without supplying a custom finder type");
				}
				if (!attribute.CustomFinderType.IsSubclassOf(typeof(By)))
				{
					throw new ArgumentException("Custom finder type must be a descendent of the By class");
				}
				ConstructorInfo constructor = attribute.CustomFinderType.GetConstructor(new Type[]
				{
					typeof(string)
				});
				if (constructor == null)
				{
					throw new ArgumentException("Custom finder type must expose a public constructor with a string argument");
				}
				return constructor.Invoke(new object[]
				{
					@using
				}) as By;
			}
			default:
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Did not know how to construct How from how {0}, using {1}", new object[]
				{
					how,
					@using
				}));
			}
		}
	}
}
