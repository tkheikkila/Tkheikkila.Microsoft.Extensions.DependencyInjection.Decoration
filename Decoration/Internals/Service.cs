using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal class Service
{
	private static readonly MethodInfo _makeGenericTypeMethod = typeof(Type).GetMethod(nameof(Type.MakeGenericType))!;
	public Type ServiceType { get; }
	public Expression<ServiceFactory> FactoryExpression { get; }

	public static Service FromType(Type serviceType, Type implementingType)
	{
		var provider = Expression.Parameter(typeof(IServiceProvider), name: "provider");
		var genericArguments = Expression.Parameter(typeof(Type[]), name: "genericArguments");

		var constructor = implementingType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SingleOrDefault();
		if (constructor is null)
		{
			// Use implicit default constructor
			 var factory = Expression.Lambda<ServiceFactory>(
				Expression.New(implementingType),
				provider, genericArguments
			);

			 return new Service(serviceType, factory);
		}
		else
		{

			var constructorArgs = constructor.GetParameters()
				.Select(parameter => parameter.ParameterType.CreateServiceFactoryExpression(provider, genericArguments));

			var factory = Expression.Lambda<ServiceFactory>(
				Expression.New(
					constructor,
					constructorArgs
				),
				provider, genericArguments
			);

			return new Service(serviceType, factory);
		}
	}

	public static Service FromFactory(Type serviceType, ServiceFactory serviceFactory)
	{
		var provider = Expression.Parameter(typeof(IServiceProvider), name: "provider");
		var genericArguments = Expression.Parameter(typeof(Type[]), name: "genericArguments");

		var factory = Expression.Lambda<ServiceFactory>(
			Expression.Call(
				serviceFactory.Target is null
					? null
					: Expression.Constant(serviceFactory.Target),
				serviceFactory.Method,
				provider, genericArguments
			),
			provider, genericArguments
		);

		return new Service(serviceType, factory);
	}

	public static Service FromInstance(Type serviceType, object instance)
	{
		var provider = Expression.Parameter(typeof(IServiceProvider), name: "provider");
		var genericArguments = Expression.Parameter(typeof(Type[]), name: "genericArguments");

		var factory = Expression.Lambda<ServiceFactory>(
			Expression.Constant(instance),
			provider, genericArguments
		);

		return new Service(serviceType, factory);
	}

	private Service(Type serviceType, Expression<ServiceFactory> factoryExpression)
	{
		ServiceType = serviceType;
		FactoryExpression = factoryExpression;
	}

	public Service Decorate(Type decoratorType)
	{
		var constructor = FindDecoratableConstructorOrThrow(decoratorType);

		var factory = CreateDecoratedFactory(constructor);

		return new Service(
			ServiceType,
			factory
		);
	}

	public Service CreateOptimizedVersion()
	{
		var factory = FactoryExpression.Body;

		while (factory.CanReduce)
		{
			factory = factory.ReduceAndCheck();
		}

		return new Service(
			ServiceType,
			Expression.Lambda<ServiceFactory>(
				factory,
				FactoryExpression.Parameters
			)
		);
	}

	private Expression<ServiceFactory> CreateDecoratedFactory(ConstructorInfo constructor)
	{
		var constructorArguments = constructor.GetParameters()
			.Select(
				parameter => ParameterIsService(parameter)
					? InjectService()
					: InjectDependency(FactoryExpression.Parameters[0], parameter.ParameterType)
			)
			.ToArray();

		return Expression.Lambda<ServiceFactory>(
			Expression.New(
				constructor,
				constructorArguments
			),
			FactoryExpression.Parameters
		);
	}

	private Expression InjectService()
	{
		return Expression.Convert(FactoryExpression.Body, ServiceType);
	}

	private static Expression MakeGenericTypeExpression(Type type, Expression genericArgumentExpression)
	{
		return Expression.Call(
			Expression.Constant(type),
			_makeGenericTypeMethod,
			genericArgumentExpression
		);
	}

	private static Expression InjectDependency(ParameterExpression providerExpression, Type dependencyType)
	{
		return
			// (IDependency)someObject
			Expression.Convert(
				// provider.GetService(injectedServiceType.MakeGenericType(genericArguments))
				Expression.Call(
					instance: providerExpression,
					methodName: nameof(IServiceProvider.GetService),
					typeArguments: null,
					arguments: Expression.Constant(dependencyType)
				),
				dependencyType
			);
	}

	private ConstructorInfo FindDecoratableConstructorOrThrow(Type decoratorType, [CallerArgumentExpression(nameof(decoratorType))] string? paramName = null)
	{
		var constructors = decoratorType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(c => c.GetParameters().Any(ParameterIsService))
			.ToList();

		if (constructors.Count == 0)
		{
			throw new ArgumentException($"{decoratorType} does not have a constructor with any {ServiceType} as parameters", paramName);
		}

		if (constructors.Count > 1)
		{
			throw new ArgumentException($"{decoratorType} has more than one ({constructors.Count}) constructors with a {ServiceType} as parameters", paramName);
		}

		var constructor = constructors[0];

		if (constructor.GetParameters().Count(ParameterIsService) > 1)
		{
			throw new ArgumentException($"{decoratorType} constructor has multiple parameters of type {ServiceType}", paramName);
		}

		return constructor;
	}

	private bool ParameterIsService(ParameterInfo parameter)
	{
		return parameter.ParameterType.IsAssignableTo(ServiceType);
	}
}
