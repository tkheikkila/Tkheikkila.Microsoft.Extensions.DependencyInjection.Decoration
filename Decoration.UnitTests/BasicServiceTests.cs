using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Helpers;
using Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Mocks;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests;

public sealed class BasicServiceTests : IDisposable
{
	public ServiceCollection Services { get; }

	public static IEnumerable<object?[]> CasesToTest => TestCases.Create().AsTheoryParameters();

	public static IEnumerable<object?[]> RegistrationCasesToTest
		=> TestCases.Create()
			.AsTheoryParameters(
				setup => setup.Match(
					byType: () => Test(descriptor => descriptor.ImplementationType == typeof(ServiceMock)),
					byFactory: _ => Test(service => service.ImplementationFactory != null),
					byInstance: instance => Test(service => service.ImplementationInstance == instance)
				)
			);

	private static Func<ServiceDescriptor, bool> Test(Func<ServiceDescriptor, bool> test)
		=> test;

	public BasicServiceTests()
	{
		Services = new ServiceCollection();
	}

	[Theory]
	[MemberData(nameof(RegistrationCasesToTest))]
	public void AddingService_RegistersService(Setup setup, Func<ServiceDescriptor, bool> test)
	{
		// Arrange
		
		// Act
		setup.Apply(Services);

		// Assert
		Services.Should().Contain(service => test(service) && service.ServiceType == typeof(IService) && service.Lifetime == setup.Lifetime);
	}

	[Theory]
	[MemberData(nameof(CasesToTest))]
	public void ServiceProvider_CanBeBuilt(Setup setup)
	{
		// Arrange
		setup.Apply(Services);

		// Act
		var buildingServiceProvider = Services.Invoking(services => services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }));

		// Assert
		buildingServiceProvider.Should().NotThrow();
	}

	[Theory]
	[MemberData(nameof(CasesToTest))]
	public void Service_CanBeResolved(Setup setup)
	{
		// Arrange
		setup.Apply(Services);
		var provider = Services.BuildServiceProvider();

		// Act
		var resolvingService = provider.Invoking(p => p.GetRequiredService<IService>());

		// Assert
		var resolvedService = resolvingService.Should().NotThrow();

		resolvedService.Which.Should().NotBeNull();
	}

	[Theory]
	[MemberData(nameof(CasesToTest))]
	public void InvokingService_DoesNotThrow(Setup setup)
	{
		// Arrange
		setup.Apply(Services);
		var provider = Services.BuildServiceProvider();
		var service = provider.GetRequiredService<IService>();

		// Act
		var invokingService = service.Invoking(s => s.Call(new List<string> { "Start" }));

		// Assert
		invokingService.Should().NotThrow();
	}

	[Theory]
	[MemberData(nameof(CasesToTest))]
	public void InvokingService_DoesNotHaveDecorators(Setup setup)
	{
		// Arrange
		setup.Apply(Services);
		var provider = Services.BuildServiceProvider();
		var service = provider.GetRequiredService<IService>();

		// Act
		var result = service.Call(new List<string> { "Start" });

		// Assert
		result.Should()
			.SatisfyRespectively(
				s => s.Should().Be("Start"),
				s => s.Should().Be(ServiceMock.Service)
			);
	}


	public void Dispose()
	{
		Services.Clear();
	}
}