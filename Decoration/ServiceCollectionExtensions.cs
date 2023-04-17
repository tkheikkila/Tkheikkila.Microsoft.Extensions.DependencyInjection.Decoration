using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDecorated<T>(this IServiceCollection services, Func<IServiceBuilder<T>, IServiceConfiguration> config) where T : notnull
	{
		return config(new ServiceBuilderGenericWrapper<T>()).Configure(services);
	}

	public static IServiceCollection AddDecorated(this IServiceCollection services, Type serviceType, Func<IServiceBuilder, IServiceConfiguration> config)
	{
		return config(new ServiceBuilder(serviceType)).Configure(services);
	}
}