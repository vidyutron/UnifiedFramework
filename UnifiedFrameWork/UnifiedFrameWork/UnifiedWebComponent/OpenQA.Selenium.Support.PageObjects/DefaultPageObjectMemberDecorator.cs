using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace OpenQA.Selenium.Support.PageObjects
{
	public class DefaultPageObjectMemberDecorator : IPageObjectMemberDecorator
	{
		private static List<Type> interfacesToBeProxied;

		private static Type interfaceProxyType;

		private static List<Type> InterfacesToBeProxied
		{
			get
			{
				if (DefaultPageObjectMemberDecorator.interfacesToBeProxied == null)
				{
					DefaultPageObjectMemberDecorator.interfacesToBeProxied = new List<Type>();
					DefaultPageObjectMemberDecorator.interfacesToBeProxied.Add(typeof(IWebElement));
					DefaultPageObjectMemberDecorator.interfacesToBeProxied.Add(typeof(ILocatable));
					DefaultPageObjectMemberDecorator.interfacesToBeProxied.Add(typeof(IWrapsElement));
				}
				return DefaultPageObjectMemberDecorator.interfacesToBeProxied;
			}
		}

		private static Type InterfaceProxyType
		{
			get
			{
				if (DefaultPageObjectMemberDecorator.interfaceProxyType == null)
				{
					DefaultPageObjectMemberDecorator.interfaceProxyType = DefaultPageObjectMemberDecorator.CreateTypeForASingleElement();
				}
				return DefaultPageObjectMemberDecorator.interfaceProxyType;
			}
		}

		public object Decorate(MemberInfo member, IElementLocator locator)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			PropertyInfo propertyInfo = member as PropertyInfo;
			Type memberType = null;
			if (fieldInfo != null)
			{
				memberType = fieldInfo.FieldType;
			}
			bool flag = false;
			if (propertyInfo != null)
			{
				flag = propertyInfo.CanWrite;
				memberType = propertyInfo.PropertyType;
			}
			if (fieldInfo == null & (propertyInfo == null || !flag))
			{
				return null;
			}
			IList<By> list = DefaultPageObjectMemberDecorator.CreateLocatorList(member);
			if (list.Count > 0)
			{
				bool cache = DefaultPageObjectMemberDecorator.ShouldCacheLookup(member);
				return DefaultPageObjectMemberDecorator.CreateProxyObject(memberType, locator, list, cache);
			}
			return null;
		}

		protected static bool ShouldCacheLookup(MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member", "memeber cannot be null");
			}
			Type typeFromHandle = typeof(CacheLookupAttribute);
			return member.GetCustomAttributes(typeFromHandle, true).Length != 0 || member.DeclaringType.GetCustomAttributes(typeFromHandle, true).Length != 0;
		}

		protected static ReadOnlyCollection<By> CreateLocatorList(MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member", "memeber cannot be null");
			}
			Attribute[] customAttributes = Attribute.GetCustomAttributes(member, typeof(FindsBySequenceAttribute), true);
			bool flag = customAttributes.Length > 0;
			Attribute[] customAttributes2 = Attribute.GetCustomAttributes(member, typeof(FindsByAllAttribute), true);
			bool flag2 = customAttributes2.Length > 0;
			if (flag && flag2)
			{
				throw new ArgumentException("Cannot specify FindsBySequence and FindsByAll on the same member");
			}
			List<By> list = new List<By>();
			Attribute[] customAttributes3 = Attribute.GetCustomAttributes(member, typeof(FindsByAttribute), true);
			if (customAttributes3.Length > 0)
			{
				Array.Sort<Attribute>(customAttributes3);
				Attribute[] array = customAttributes3;
				for (int i = 0; i < array.Length; i++)
				{
					Attribute attribute = array[i];
					FindsByAttribute findsByAttribute = (FindsByAttribute)attribute;
					if (findsByAttribute.Using == null)
					{
						findsByAttribute.Using = member.Name;
					}
					list.Add(findsByAttribute.Finder);
				}
				if (flag)
				{
					ByChained item = new ByChained(list.ToArray());
					list.Clear();
					list.Add(item);
				}
				if (flag2)
				{
					ByAll item2 = new ByAll(list.ToArray());
					list.Clear();
					list.Add(item2);
				}
			}
			return list.AsReadOnly();
		}

		private static object CreateProxyObject(Type memberType, IElementLocator locator, IEnumerable<By> bys, bool cache)
		{
			object result = null;
			if (memberType == typeof(IList<IWebElement>))
			{
				using (List<Type>.Enumerator enumerator = DefaultPageObjectMemberDecorator.InterfacesToBeProxied.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Type current = enumerator.Current;
						Type type = typeof(IList<>).MakeGenericType(new Type[]
						{
							current
						});
						if (type.Equals(memberType))
						{
							result = WebElementListProxy.CreateProxy(memberType, locator, bys, cache);
							break;
						}
					}
					return result;
				}
			}
			if (!(memberType == typeof(IWebElement)))
			{
				throw new ArgumentException("Type of member '" + memberType.Name + "' is not IWebElement or IList<IWebElement>");
			}
			result = WebElementProxy.CreateProxy(DefaultPageObjectMemberDecorator.InterfaceProxyType, locator, bys, cache);
			return result;
		}

		private static Type CreateTypeForASingleElement()
		{
			AssemblyName assemblyName = new AssemblyName(Guid.NewGuid().ToString());
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
			TypeBuilder typeBuilder = moduleBuilder.DefineType(typeof(IWebElement).FullName, TypeAttributes.Public | TypeAttributes.ClassSemanticsMask | TypeAttributes.Abstract);
			foreach (Type current in DefaultPageObjectMemberDecorator.InterfacesToBeProxied)
			{
				typeBuilder.AddInterfaceImplementation(current);
			}
			return typeBuilder.CreateType();
		}
	}
}
