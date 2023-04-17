using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class ServiceBuilderGenericWrapper<T> : IServiceBuilder<T>
	where T : notnull
{
	private readonly IServiceBuilder _builder;

	public ServiceBuilderGenericWrapper()
	{
		_builder = new ServiceBuilder(typeof(T));
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
		return As((object)instance);
	}

	public IServiceBuilder<T> Use<TDecorator>() where TDecorator : class, T
	{
		return new DecoratedServiceBuilderGenericWrapper<T>(_builder.Use(typeof(TDecorator)));
	}
}