using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class ExpressionFactoryLifetimeSelector : ILifetimeSelector
{
	private readonly Type _serviceType;
	private readonly Expression<ServiceFactory> _factory;

    public ExpressionFactoryLifetimeSelector(Type serviceType, Expression<ServiceFactory> factory)
    {
	    _serviceType = serviceType;
	    _factory = factory;
    }

    public IServiceConfiguration WithLifetime(ServiceLifetime lifetime)
    {
        var factory = _factory.Compile();

        return new BasicService(
            ServiceDescriptor.Describe(
                _serviceType,
                provider => factory(provider, Array.Empty<Type>()),
                lifetime
            )
        );
    }
}