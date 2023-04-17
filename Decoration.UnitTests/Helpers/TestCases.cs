using Microsoft.Extensions.DependencyInjection;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Mocks;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Helpers;

public static class TestCases
{
	public static IEnumerable<object?[]> AsTheoryParameters(this IEnumerable<Setup> setups, params Func<Setup, object?>[] additionalParameters)
		=> setups.Select(
            setup => additionalParameters.Select(parameter => parameter(setup))
	            .Prepend(setup)
	            .ToArray()
		);

	public static IEnumerable<Setup> Create(ServiceConfig<IService>? config = null)
	{
		return Create<IService, ServiceMock>(new ServiceMock(), _ => new ServiceMock(), config);
    }

	public static IEnumerable<Setup> Create(Type serviceType, Type implementationType, ServiceConfig? config = null)
	{
		config ??= NoConfig;

		foreach (var lifetime in Enum.GetValues<ServiceLifetime>())
		{
			yield return Setup.ByType(serviceType, implementationType, config, lifetime);
		}
	}

	public static IEnumerable<Setup> Create<T, TImpl>(T instance, ServiceFactory factory, ServiceConfig<T>? config = null) where T : notnull where TImpl : class, T
	{
		config ??= NoConfig;

		yield return Setup.ByInstance(instance, config);

		foreach (var lifetime in Enum.GetValues<ServiceLifetime>())
		{
			yield return Setup.ByType(typeof(T), typeof(TImpl), builder => config(new ServiceBuilderGenericWrapper<T>(builder)), lifetime);
			yield return Setup.ByType<T, TImpl>(config, lifetime);
			yield return Setup.ByFactory(factory, config, lifetime);
		}
	}

	private static IServiceBuilder<T> NoConfig<T>(IServiceBuilder<T> builder) where T : notnull
		=> builder;

	private static IServiceBuilder NoConfig(IServiceBuilder builder)
		=> builder;

	private class ServiceBuilderGenericWrapper<T> : IServiceBuilder<T> where T : notnull
	{
		private readonly IServiceBuilder _builder;

		public ServiceBuilderGenericWrapper(IServiceBuilder builder)
		{
			_builder = builder;
		}

		public ILifetimeSelector As(Type implementingType)
		{
			return _builder.As(implementingType);
		}

		public ILifetimeSelector As(ServiceFactory factory)
		{
			return _builder.As(factory);
		}

		public IServiceConfiguration As(object instance)
		{
			return _builder.As(instance);
		}

		public IServiceBuilder Use(Type decoratorType)
		{
			return _builder.Use(decoratorType);
		}

		public ILifetimeSelector As<TImpl>() where TImpl : class, T
		{
			return _builder.As(typeof(TImpl));
		}

		public IServiceConfiguration As(T instance)
		{
			return _builder.As(instance);
		}

		public IServiceBuilder<T> Use<TDecorator>() where TDecorator : class, T
		{
			return new ServiceBuilderGenericWrapper<T>(_builder.Use(typeof(TDecorator)));
		}
	}
}
