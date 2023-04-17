using Microsoft.Extensions.DependencyInjection;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Helpers;

public abstract partial class Setup
{
	public static Setup ByFactory<T>(ServiceFactory factory, ServiceConfig<T> config, ServiceLifetime lifetime) where T : notnull
	{
		return new SetupByFactory<T>(factory, config, lifetime);
	}

	private sealed class SetupByFactory<T> : Setup where T : notnull
	{
		private readonly ServiceFactory _factory;
		private readonly ServiceConfig<T> _config;

		public SetupByFactory(ServiceFactory factory, ServiceConfig<T> config, ServiceLifetime lifetime) : base(lifetime)
		{
			_factory = factory;
			_config = config;
		}

		public override void Apply(IServiceCollection services)
		{
			services.AddDecorated<T>(
				service => _config(service).As(_factory).WithLifetime(Lifetime)
			);
		}

		public override TResult Match<TResult>(Func<TResult> byType, Func<ServiceFactory, TResult> byFactory, Func<object, TResult> byInstance)
		{
			return byFactory(_factory);
		}

		public override string ToString()
		{
			return $"ByFactory ({Lifetime})";
		}
	}
}