using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class DecoratedService : IServiceConfiguration
{
    private readonly Service _service;
    private readonly ServiceLifetime _lifetime;

    public DecoratedService(Service service, ServiceLifetime lifetime)
    {
        _service = service;
        _lifetime = lifetime;
    }

    public IServiceCollection Configure(IServiceCollection services)
    {
        var service = _service.CreateOptimizedVersion();
        var factory = service.FactoryExpression.Compile();
        services.Add(
            ServiceDescriptor.Describe(
                service.ServiceType,
                provider => factory(provider),
                _lifetime
            )
        );

        return services;
    }
}