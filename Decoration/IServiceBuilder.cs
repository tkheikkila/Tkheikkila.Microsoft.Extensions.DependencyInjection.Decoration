using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public interface IServiceBuilder
{
	ILifetimeSelector As(Type implementingType);
	ILifetimeSelector As(ServiceFactory factory);
	IServiceConfiguration As(object instance);

	IServiceBuilder Use(Type decoratorType);
}

public interface IServiceBuilder<in T> : IServiceBuilder
	where T : notnull
{
	ILifetimeSelector As<TImpl>() where TImpl : class, T;

	IServiceConfiguration As(T instance);

	IServiceBuilder<T> Use<TDecorator>() where TDecorator : class, T;
}
