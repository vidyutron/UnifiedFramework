using System;
using System.Reflection;

namespace OpenQA.Selenium.Support.PageObjects
{
	public interface IPageObjectMemberDecorator
	{
		object Decorate(MemberInfo member, IElementLocator locator);
	}
}
