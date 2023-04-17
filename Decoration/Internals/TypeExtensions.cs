using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

public static class TypeExtensions
{
	public static bool ImplementsService(this Type type, Type serviceType)
	{
		if (type.IsAssignableTo(serviceType))
		{
			return true;
		}

		if (!serviceType.IsGenericTypeDefinition)
		{
			// Service type is not generic and not directly assignable
			// => type cannot implement it
			return false;
		}

		if (serviceType.IsInterface)
		{
			return type.HasServiceInterface(serviceType);
		}

		if (serviceType.IsClass)
		{
			return type.HasServiceBaseType(serviceType);
		}

		return false;
	}

	private static bool HasServiceInterface(this Type type, Type genericServiceTypeDefinition)
	{
		return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericServiceTypeDefinition);
	}

	private static bool HasServiceBaseType(this Type type, Type genericServiceTypeDefinition)
	{
		return type.GetBaseTypes().Any(b => b.IsGenericType && b.GetGenericTypeDefinition() == genericServiceTypeDefinition);
	}

	private static IEnumerable<Type> GetBaseTypes(this Type type)
	{
		var baseType = type.BaseType;

		while (baseType is not null)
		{
			yield return baseType;
			baseType = baseType.BaseType;
		}
	}
}
