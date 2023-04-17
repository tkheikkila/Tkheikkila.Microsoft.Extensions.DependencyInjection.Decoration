using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class DecoratedServiceLifetimeSelector : ILifetimeSelector
{
	private readonly Service _service;

	public DecoratedServiceLifetimeSelector(Service service)
	{
		_service = service;
	}

	public IServiceConfiguration WithLifetime(ServiceLifetime lifetime)
	{
		return new DecoratedService(_service, lifetime);
	}
}