using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

internal sealed class DecoratedServiceBuilder : IServiceBuilder
{
	private readonly Type _serviceType;
	private readonly List<Type> _decorators;

	public DecoratedServiceBuilder(Type serviceType, Type firstDecorator)
	{
		_serviceType = serviceType;
		_decorators = new List<Type>
		{
			firstDecorator
		};
	}

	private DecoratedServiceBuilder(DecoratedServiceBuilder original, Type nextDecorator)
	{
		_serviceType = original._serviceType;
		_decorators = new List<Type>(original._decorators)
		{
			nextDecorator
		};
	}

	public ILifetimeSelector As(Type implementingType)
	{
		ArgumentNullException.ThrowIfNull(implementingType);

		if (implementingType.IsGenericTypeDefinition)
		{
			// Generic type definitions would require a service factory delegate ServiceFactory where the type array is the requested type arguments.
			// However, since Microsoft.Extensions.DependencyInjection does not provide that, we're forced to report this to the user
			// This issue could be remedied by:
			// a) using a different container library
			// b) the maintainers modify the factory so that the current factory type (ServiceFactory factory) is replaced with ServiceFactory or similar.
			//
			// I'm not interested in completely rewriting this code to accomodate other containers and honestly I don't see the maintainers breaking the current contract in fear of
			// backwards compatibility issues so this will stay as-is until further notice (forever, likely).
			// 
			throw new NotSupportedException("Microsoft.Extensions.DependencyInjection does not provide a way for resolving implementations that are generic type definitions.");
		}

		if (!implementingType.ImplementsService(_serviceType))
		{
			throw new ArgumentException($"Service {implementingType} does not implement {_serviceType}.", nameof(implementingType));
		}

		return Decorate(Service.FromType(_serviceType, implementingType));
	}

	public ILifetimeSelector As(ServiceFactory factory)
	{
		ArgumentNullException.ThrowIfNull(factory);
		
		return Decorate(Service.FromFactory(_serviceType, factory));
	}

	public IServiceConfiguration As(object instance)
	{
		ArgumentNullException.ThrowIfNull(instance);

		if (!instance.GetType().ImplementsService(_serviceType))
		{
			throw new ArgumentException($"Service instance of type {instance.GetType()} does not implement {_serviceType}.", nameof(instance));
		}

		return Decorate(Service.FromInstance(_serviceType, instance)).WithLifetime(ServiceLifetime.Singleton);
	}

	public IServiceBuilder Use(Type decoratorType)
	{
		ArgumentNullException.ThrowIfNull(decoratorType);
		if (!decoratorType.ImplementsService(_serviceType))
		{
			throw new ArgumentException($"Decorator {decoratorType} does not implement {_serviceType}.", nameof(decoratorType));
		}

		return new DecoratedServiceBuilder(this, decoratorType);
	}

	private DecoratedServiceLifetimeSelector Decorate(Service service)
	{
		// Decorators need to be applied in reverse order in order for
		// them to be invoked in the order of registration
		for (var i = _decorators.Count - 1; i >= 0; i--)
		{
			service = service.Decorate(_decorators[i]);
		}

		return new DecoratedServiceLifetimeSelector(service);
	}
}
