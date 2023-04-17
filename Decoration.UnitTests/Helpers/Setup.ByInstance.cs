using Microsoft.Extensions.DependencyInjection;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Helpers;

public abstract partial class Setup
{
	public static Setup ByInstance<T>(
		T instance,
		ServiceConfig<T> config
	) where T : notnull
	{
		return new SetupByInstance<T>(instance, config);
	}

	private sealed class SetupByInstance<T> : Setup where T : notnull
	{
		private readonly T _instance;
		private readonly ServiceConfig<T> _config;

		public SetupByInstance(T instance, ServiceConfig<T> config) : base(ServiceLifetime.Singleton)
		{
			_instance = instance;
			_config = config;
		}

		public override void Apply(IServiceCollection services)
		{
			services.AddDecorated<T>(
				service => _config(service).As(_instance)
			);
		}

		public override TResult Match<TResult>(Func<TResult> byType, Func<ServiceFactory, TResult> byFactory, Func<object, TResult> byInstance)
		{
			return byInstance(_instance);
		}

		public override string ToString()
		{
			return $"ByInstance ({Lifetime})";
		}
	}
}
