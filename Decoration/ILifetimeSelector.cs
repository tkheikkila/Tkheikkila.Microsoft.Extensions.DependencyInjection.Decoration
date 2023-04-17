// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public interface ILifetimeSelector
{
	IServiceConfiguration AsTransient() => WithLifetime(ServiceLifetime.Transient);
	IServiceConfiguration AsScoped() => WithLifetime(ServiceLifetime.Scoped);
	IServiceConfiguration AsSingleton() => WithLifetime(ServiceLifetime.Singleton);

	IServiceConfiguration WithLifetime(ServiceLifetime lifetime);
}