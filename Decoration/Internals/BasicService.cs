using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class BasicService : IServiceConfiguration
{
    private readonly ServiceDescriptor _descriptor;

    public BasicService(ServiceDescriptor descriptor)
    {
        _descriptor = descriptor;
    }

    public IServiceCollection Configure(IServiceCollection services)
    {
        services.Add(_descriptor);

        return services;
    }
}