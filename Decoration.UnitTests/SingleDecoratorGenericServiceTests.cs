using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Helpers;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Mocks;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests;

public sealed class SingleDecoratorGenericServiceTests
{
	[Fact]
	public void AddingService_ThrowsNotSupportedException()
	{ 
		// Arrange
		var services = new ServiceCollection();
		var setup = Setup.ByType(typeof(IGenericService<>), typeof(GenericServiceMock<>), service => service.Use(typeof(GenericDecoratorMock<>)), ServiceLifetime.Transient);
		
		// Act
		var registeringServices = setup.Invoking(s => s.Apply(services));

		// Assert
		registeringServices.Should().Throw<NotSupportedException>();
	}
}
