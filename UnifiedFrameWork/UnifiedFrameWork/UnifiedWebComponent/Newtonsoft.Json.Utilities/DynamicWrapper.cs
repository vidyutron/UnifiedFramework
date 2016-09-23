using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;

namespace Newtonsoft.Json.Utilities
{
	internal static class DynamicWrapper
	{
		private static readonly object _lock = new object();

		private static readonly WrapperDictionary _wrapperDictionary = new WrapperDictionary();

		private static ModuleBuilder _moduleBuilder;

		private static ModuleBuilder ModuleBuilder
		{
			get
			{
				DynamicWrapper.Init();
				return DynamicWrapper._moduleBuilder;
			}
		}

		private static void Init()
		{
			if (DynamicWrapper._moduleBuilder == null)
			{
				lock (DynamicWrapper._lock)
				{
					if (DynamicWrapper._moduleBuilder == null)
					{
						AssemblyName assemblyName = new AssemblyName("Newtonsoft.Json.Dynamic");
						assemblyName.KeyPair = new StrongNameKeyPair(DynamicWrapper.GetStrongKey());
						AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
						DynamicWrapper._moduleBuilder = assemblyBuilder.DefineDynamicModule("Newtonsoft.Json.DynamicModule", false);
					}
				}
			}
		}

		private static byte[] GetStrongKey()
		{
			byte[] result;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Newtonsoft.Json.Dynamic.snk"))
			{
				if (manifestResourceStream == null)
				{
					throw new MissingManifestResourceException("Should have Newtonsoft.Json.Dynamic.snk as an embedded resource.");
				}
				int num = (int)manifestResourceStream.Length;
				byte[] array = new byte[num];
				manifestResourceStream.Read(array, 0, num);
				result = array;
			}
			return result;
		}

		public static Type GetWrapper(Type interfaceType, Type realObjectType)
		{
			Type type = DynamicWrapper._wrapperDictionary.GetType(interfaceType, realObjectType);
			if (type == null)
			{
				lock (DynamicWrapper._lock)
				{
					type = DynamicWrapper._wrapperDictionary.GetType(interfaceType, realObjectType);
					if (type == null)
					{
						type = DynamicWrapper.GenerateWrapperType(interfaceType, realObjectType);
						DynamicWrapper._wrapperDictionary.SetType(interfaceType, realObjectType, type);
					}
				}
			}
			return type;
		}

		public static object GetUnderlyingObject(object wrapper)
		{
			DynamicWrapperBase dynamicWrapperBase = wrapper as DynamicWrapperBase;
			if (dynamicWrapperBase == null)
			{
				throw new ArgumentException("Object is not a wrapper.", "wrapper");
			}
			return dynamicWrapperBase.UnderlyingObject;
		}

		private static Type GenerateWrapperType(Type interfaceType, Type underlyingType)
		{
			TypeBuilder typeBuilder = DynamicWrapper.ModuleBuilder.DefineType("{0}_{1}_Wrapper".FormatWith(CultureInfo.InvariantCulture, interfaceType.Name, underlyingType.Name), TypeAttributes.Sealed, typeof(DynamicWrapperBase), new Type[]
			{
				interfaceType
			});
			WrapperMethodBuilder wrapperMethodBuilder = new WrapperMethodBuilder(underlyingType, typeBuilder);
			foreach (MethodInfo current in interfaceType.GetAllMethods())
			{
				wrapperMethodBuilder.Generate(current);
			}
			return typeBuilder.CreateType();
		}

		public static T CreateWrapper<T>(object realObject) where T : class
		{
			Type wrapper = DynamicWrapper.GetWrapper(typeof(T), realObject.GetType());
			DynamicWrapperBase dynamicWrapperBase = (DynamicWrapperBase)Activator.CreateInstance(wrapper);
			dynamicWrapperBase.UnderlyingObject = realObject;
			return dynamicWrapperBase as T;
		}
	}
}
