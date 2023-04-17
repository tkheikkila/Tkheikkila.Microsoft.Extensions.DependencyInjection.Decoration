namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Mocks;

public class GenericDecoratorMock<T> : IGenericService<T>
{
	private readonly IGenericService<T> _service;
	public const string BeforeDecoratedService = "GenericDecorator-Pre";
	public const string AfterDecoratedService = "GenericDecorator-Post";

	public GenericDecoratorMock(IGenericService<T> service)
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
