using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class DecoratedServiceBuilderGenericWrapper<T> : IServiceBuilder<T> where T : notnull
{
	private readonly IServiceBuilder _builder;

	public DecoratedServiceBuilderGenericWrapper(IServiceBuilder builder)
	{
		_builder = builder;
	}

	public ILifetimeSelector As<TImpl>() where TImpl : class, T
	{
		return _builder.As(typeof(TImpl));
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

	public IServiceConfiguration As(T instance)
	{
		return _builder.As(instance);
	}

	public IServiceBuilder<T> Use<TDecorator>() where TDecorator : class, T
	{
		return new DecoratedServiceBuilderGenericWrapper<T>(_builder.Use(typeof(TDecorator)));
	}
}