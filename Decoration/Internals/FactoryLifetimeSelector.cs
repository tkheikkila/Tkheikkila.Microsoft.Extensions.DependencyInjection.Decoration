using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class FactoryLifetimeSelector : ILifetimeSelector
{
	private readonly Type _serviceType;
	private readonly ServiceFactory _factory;

    public FactoryLifetimeSelector(Type serviceType, ServiceFactory factory)
    {
	    _serviceType = serviceType;
	    _factory = factory;
    }

    public IServiceConfiguration WithLifetime(ServiceLifetime lifetime)
    {
        return new BasicService(
            ServiceDescriptor.Describe(
                _serviceType,
                provider => _factory(provider),
                lifetime
            )
        );
    }
}