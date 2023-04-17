using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class ServiceBuilder : IServiceBuilder
{
	private readonly Type _serviceType;

	public ServiceBuilder(Type serviceType)
	{
		_serviceType = serviceType;
	}

	public ILifetimeSelector As(Type implementingType)
	{
		return new TypedLifetimeSelector(_serviceType, implementingType);
	}

	public ILifetimeSelector As(ServiceFactory factory)
	{
		return new FactoryLifetimeSelector(_serviceType, factory);
	}

	public IServiceConfiguration As(object instance)
	{
		return new BasicService(ServiceDescriptor.Singleton(_serviceType, instance));
	}

	public IServiceBuilder Use(Type decoratorType)
	{
		return new DecoratedServiceBuilder(_serviceType, decoratorType);
	}
}
