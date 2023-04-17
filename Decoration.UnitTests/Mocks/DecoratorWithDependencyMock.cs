namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Mocks;

public class DecoratorWithDependencyMock : IService
{
	private readonly IServiceProvider _provider;
	private readonly IService _service;

	public const string BeforeDecoratedService = "DecoratorWithDependency-Pre";
	public const string AfterDecoratedService = "DecoratorWithDependency-Post";

	public DecoratorWithDependencyMock(IServiceProvider provider, IService service)
	{
		_provider = provider;
		_service = service;
	}

	public List<string> Call(List<string> services)
	{
		services.Add(BeforeDecoratedService);
		services = _service.Call(services);
		services.Add(AfterDecoratedService);
		return services;
	}
}
