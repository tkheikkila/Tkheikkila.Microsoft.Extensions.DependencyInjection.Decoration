using Microsoft.Extensions.DependencyInjection;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Helpers;

public abstract partial class Setup
{
	public ServiceLifetime Lifetime { get; }

	private protected Setup(ServiceLifetime lifetime)
	{
		Lifetime = lifetime;
	}

	public abstract void Apply(IServiceCollection services);

	public abstract TResult Match<TResult>(
		Func<TResult> byType,
		Func<ServiceFactory, TResult> byFactory,
		Func<object, TResult> byInstance
	);
}