// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public interface IServiceConfiguration
{
	internal IServiceCollection Configure(IServiceCollection services);
}