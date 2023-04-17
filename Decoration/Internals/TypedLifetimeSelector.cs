using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class TypedLifetimeSelector : ILifetimeSelector
{
	private readonly Type _serviceType;
	private readonly Type _implementingType;

	public TypedLifetimeSelector(Type serviceType, Type implementingType)
	{
		_serviceType = serviceType;
		_implementingType = implementingType;
	}

    public IServiceConfiguration WithLifetime(ServiceLifetime lifetime)
    {
        return new BasicService(
            ServiceDescriptor.Describe(
                _serviceType,
                _implementingType,
                lifetime
            )
        );
    }
}