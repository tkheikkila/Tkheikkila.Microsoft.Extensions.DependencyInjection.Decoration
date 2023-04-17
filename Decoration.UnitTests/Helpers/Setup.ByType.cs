using Microsoft.Extensions.DependencyInjection;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Helpers;

public abstract partial class Setup
{
	public static Setup ByType<T, TImpl>(ServiceConfig<T> config, ServiceLifetime lifetime) where T : notnull where TImpl : class, T
	{
		return new SetupByType<T, TImpl>(config, lifetime);
	}

	public static Setup ByType(
		Type serviceType,
		Type implementationType,
		ServiceConfig config,
		ServiceLifetime lifetime
	)
	{
		return new SetupByType(serviceType, implementationType, config, lifetime);
	}

	private sealed class SetupByType : Setup
	{
		private readonly Type _serviceType;
		private readonly Type _implementingType;
		private readonly ServiceConfig _config;

		internal SetupByType(
			Type serviceType,
			Type implementingType,
			ServiceConfig config,
			ServiceLifetime lifetime
		) : base(lifetime)
		{
			_serviceType = serviceType;
			_implementingType = implementingType;
			_config = config;
		}

		public override void Apply(IServiceCollection services)
		{
			services.AddDecorated(
				_serviceType,
				options => _config(options).As(_implementingType).WithLifetime(Lifetime)
			);
		}

		public override TResult Match<TResult>(Func<TResult> byType, Func<ServiceFactory, TResult> byFactory, Func<object, TResult> byInstance)
		{
			return byType();
		}
	}

	private sealed class SetupByType<T, TImpl> : Setup where T : notnull where TImpl : class, T
	{
		private readonly ServiceConfig<T> _config;

		public SetupByType(ServiceConfig<T> config, ServiceLifetime lifetime) : base(lifetime)
		{
			_config = config;
		}

		public override void Apply(IServiceCollection services)
		{
			services.AddDecorated<T>(
				service => _config(service).As<TImpl>().WithLifetime(Lifetime)
			);
		}

		public override TResult Match<TResult>(Func<TResult> byType, Func<ServiceFactory, TResult> byFactory, Func<object, TResult> byInstance)
		{
			return byType();
		}

		public override string ToString()
		{
			return $"ByType ({Lifetime})";
		}
	}
}
