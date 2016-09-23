using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.Utilities
{
	internal static class TypeExtensions
	{
		public static MethodInfo Method(this Delegate d)
		{
			return d.Method;
		}

		public static MemberTypes MemberType(this MemberInfo memberInfo)
		{
			return memberInfo.MemberType;
		}

		public static bool ContainsGenericParameters(this Type type)
		{
			return type.ContainsGenericParameters;
		}

		public static bool IsInterface(this Type type)
		{
			return type.IsInterface;
		}

		public static bool IsGenericType(this Type type)
		{
			return type.IsGenericType;
		}

		public static bool IsGenericTypeDefinition(this Type type)
		{
			return type.IsGenericTypeDefinition;
		}

		public static Type BaseType(this Type type)
		{
			return type.BaseType;
		}

		public static Assembly Assembly(this Type type)
		{
			return type.Assembly;
		}

		public static bool IsEnum(this Type type)
		{
			return type.IsEnum;
		}

		public static bool IsClass(this Type type)
		{
			return type.IsClass;
		}

		public static bool IsSealed(this Type type)
		{
			return type.IsSealed;
		}

		public static bool IsAbstract(this Type type)
		{
			return type.IsAbstract;
		}

		public static bool IsVisible(this Type type)
		{
			return type.IsVisible;
		}

		public static bool IsValueType(this Type type)
		{
			return type.IsValueType;
		}

		public static bool AssignableToTypeName(this Type type, string fullTypeName, out Type match)
		{
			Type type2 = type;
			while (type2 != null)
			{
				if (string.Equals(type2.FullName, fullTypeName, StringComparison.Ordinal))
				{
					match = type2;
					return true;
				}
				type2 = type2.BaseType();
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				Type type3 = interfaces[i];
				if (string.Equals(type3.Name, fullTypeName, StringComparison.Ordinal))
				{
					match = type;
					return true;
				}
			}
			match = null;
			return false;
		}

		public static bool AssignableToTypeName(this Type type, string fullTypeName)
		{
			Type type2;
			return type.AssignableToTypeName(fullTypeName, out type2);
		}

		public static MethodInfo GetGenericMethod(this Type type, string name, params Type[] parameterTypes)
		{
			IEnumerable<MethodInfo> enumerable = from method in type.GetMethods()
			where method.Name == name
			select method;
			foreach (MethodInfo current in enumerable)
			{
				if (current.HasParameters(parameterTypes))
				{
					return current;
				}
			}
			return null;
		}

		public static bool HasParameters(this MethodInfo method, params Type[] parameterTypes)
		{
			Type[] array = (from parameter in method.GetParameters()
			select parameter.ParameterType).ToArray<Type>();
			if (array.Length != parameterTypes.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ToString() != parameterTypes[i].ToString())
				{
					return false;
				}
			}
			return true;
		}

		public static IEnumerable<Type> GetAllInterfaces(this Type target)
		{
			try
			{
				Type[] interfaces = target.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					Type type = interfaces[i];
					yield return type;
					try
					{
						Type[] interfaces2 = type.GetInterfaces();
						for (int j = 0; j < interfaces2.Length; j++)
						{
							Type type2 = interfaces2[j];
							yield return type2;
						}
					}
					finally
					{
					}
				}
			}
			finally
			{
			}
			yield break;
		}

		public static IEnumerable<MethodInfo> GetAllMethods(this Type target)
		{
			List<Type> list = target.GetAllInterfaces().ToList<Type>();
			list.Add(target);
			return from type in list
			from method in type.GetMethods()
			select method;
		}
	}
}
