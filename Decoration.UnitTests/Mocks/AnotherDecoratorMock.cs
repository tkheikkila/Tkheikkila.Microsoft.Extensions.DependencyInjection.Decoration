namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Mocks;

public class AnotherDecoratorMock : IService
{
	private readonly IService _service;

	public const string BeforeDecoratedService = "AnotherDecorator-Pre";
	public const string AfterDecoratedService = "AnotherDecorator-Post";

	public AnotherDecoratorMock(IService service)
	{
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
