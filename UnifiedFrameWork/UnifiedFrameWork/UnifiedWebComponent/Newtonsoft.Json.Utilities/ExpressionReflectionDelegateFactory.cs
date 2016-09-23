using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Newtonsoft.Json.Utilities
{
	internal class ExpressionReflectionDelegateFactory : ReflectionDelegateFactory
	{
		private static readonly ExpressionReflectionDelegateFactory _instance = new ExpressionReflectionDelegateFactory();

		internal static ReflectionDelegateFactory Instance
		{
			get
			{
				return ExpressionReflectionDelegateFactory._instance;
			}
		}

		public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
		{
			ValidationUtils.ArgumentNotNull(method, "method");
			Type typeFromHandle = typeof(object);
			ParameterExpression parameterExpression = Expression.Parameter(typeFromHandle, "target");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object[]), "args");
			ParameterInfo[] parameters = method.GetParameters();
			Expression[] array = new Expression[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				Expression index = Expression.Constant(i);
				Expression expression = Expression.ArrayIndex(parameterExpression2, index);
				expression = this.EnsureCastExpression(expression, parameters[i].ParameterType);
				array[i] = expression;
			}
			Expression expression2;
			if (method.IsConstructor)
			{
				expression2 = Expression.New((ConstructorInfo)method, array);
			}
			else if (method.IsStatic)
			{
				expression2 = Expression.Call((MethodInfo)method, array);
			}
			else
			{
				Expression instance = this.EnsureCastExpression(parameterExpression, method.DeclaringType);
				expression2 = Expression.Call(instance, (MethodInfo)method, array);
			}
			if (method is MethodInfo)
			{
				MethodInfo methodInfo = (MethodInfo)method;
				if (methodInfo.ReturnType != typeof(void))
				{
					expression2 = this.EnsureCastExpression(expression2, typeFromHandle);
				}
				else
				{
					expression2 = Expression.Block(expression2, Expression.Constant(null));
				}
			}
			else
			{
				expression2 = this.EnsureCastExpression(expression2, typeFromHandle);
			}
			LambdaExpression lambdaExpression = Expression.Lambda(typeof(MethodCall<T, object>), expression2, new ParameterExpression[]
			{
				parameterExpression,
				parameterExpression2
			});
			return (MethodCall<T, object>)lambdaExpression.Compile();
		}

		public override Func<T> CreateDefaultConstructor<T>(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (type.IsAbstract())
			{
				return () => (T)((object)Activator.CreateInstance(type));
			}
			Func<T> result;
			try
			{
				Type typeFromHandle = typeof(T);
				Expression expression = Expression.New(type);
				expression = this.EnsureCastExpression(expression, typeFromHandle);
				LambdaExpression lambdaExpression = Expression.Lambda(typeof(Func<T>), expression, new ParameterExpression[0]);
				Func<T> func = (Func<T>)lambdaExpression.Compile();
				result = func;
			}
			catch
			{
				result = (() => (T)((object)Activator.CreateInstance(type)));
			}
			return result;
		}

		public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
		{
			ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
			Type typeFromHandle = typeof(T);
			Type typeFromHandle2 = typeof(object);
			ParameterExpression parameterExpression = Expression.Parameter(typeFromHandle, "instance");
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			Expression expression;
			if (getMethod.IsStatic)
			{
				expression = Expression.MakeMemberAccess(null, propertyInfo);
			}
			else
			{
				Expression expression2 = this.EnsureCastExpression(parameterExpression, propertyInfo.DeclaringType);
				expression = Expression.MakeMemberAccess(expression2, propertyInfo);
			}
			expression = this.EnsureCastExpression(expression, typeFromHandle2);
			LambdaExpression lambdaExpression = Expression.Lambda(typeof(Func<T, object>), expression, new ParameterExpression[]
			{
				parameterExpression
			});
			return (Func<T, object>)lambdaExpression.Compile();
		}

		public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
		{
			ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "source");
			Expression expression;
			if (fieldInfo.IsStatic)
			{
				expression = Expression.Field(null, fieldInfo);
			}
			else
			{
				Expression expression2 = this.EnsureCastExpression(parameterExpression, fieldInfo.DeclaringType);
				expression = Expression.Field(expression2, fieldInfo);
			}
			expression = this.EnsureCastExpression(expression, typeof(object));
			return Expression.Lambda<Func<T, object>>(expression, new ParameterExpression[]
			{
				parameterExpression
			}).Compile();
		}

		public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
		{
			ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
			if (fieldInfo.DeclaringType.IsValueType() || fieldInfo.IsInitOnly)
			{
				return LateBoundReflectionDelegateFactory.Instance.CreateSet<T>(fieldInfo);
			}
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "source");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object), "value");
			Expression expression;
			if (fieldInfo.IsStatic)
			{
				expression = Expression.Field(null, fieldInfo);
			}
			else
			{
				Expression expression2 = this.EnsureCastExpression(parameterExpression, fieldInfo.DeclaringType);
				expression = Expression.Field(expression2, fieldInfo);
			}
			Expression right = this.EnsureCastExpression(parameterExpression2, expression.Type);
			BinaryExpression body = Expression.Assign(expression, right);
			LambdaExpression lambdaExpression = Expression.Lambda(typeof(Action<T, object>), body, new ParameterExpression[]
			{
				parameterExpression,
				parameterExpression2
			});
			return (Action<T, object>)lambdaExpression.Compile();
		}

		public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
		{
			ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
			if (propertyInfo.DeclaringType.IsValueType())
			{
				return LateBoundReflectionDelegateFactory.Instance.CreateSet<T>(propertyInfo);
			}
			Type typeFromHandle = typeof(T);
			Type typeFromHandle2 = typeof(object);
			ParameterExpression parameterExpression = Expression.Parameter(typeFromHandle, "instance");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeFromHandle2, "value");
			Expression expression = this.EnsureCastExpression(parameterExpression2, propertyInfo.PropertyType);
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			Expression body;
			if (setMethod.IsStatic)
			{
				body = Expression.Call(setMethod, expression);
			}
			else
			{
				Expression instance = this.EnsureCastExpression(parameterExpression, propertyInfo.DeclaringType);
				body = Expression.Call(instance, setMethod, new Expression[]
				{
					expression
				});
			}
			LambdaExpression lambdaExpression = Expression.Lambda(typeof(Action<T, object>), body, new ParameterExpression[]
			{
				parameterExpression,
				parameterExpression2
			});
			return (Action<T, object>)lambdaExpression.Compile();
		}

		private Expression EnsureCastExpression(Expression expression, Type targetType)
		{
			Type type = expression.Type;
			if (type == targetType || (!type.IsValueType() && targetType.IsAssignableFrom(type)))
			{
				return expression;
			}
			return Expression.Convert(expression, targetType);
		}
	}
}
