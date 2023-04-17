namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Mocks;

public class DecoratorMock : IService
{
	private readonly IService _service;

	public const string BeforeDecoratedService = "Decorator-Pre";
	public const string AfterDecoratedService = "Decorator-Post";

	public DecoratorMock(IService service)
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