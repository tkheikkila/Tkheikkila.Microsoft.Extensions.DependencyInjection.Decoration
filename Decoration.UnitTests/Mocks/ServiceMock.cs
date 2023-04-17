namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Mocks;

public interface IService
{
	List<string> Call(List<string> services);
}

public class ServiceMock : IService
{
	public const string Service = "Service";

	public List<string> Call(List<string> services)
	{
		services.Add(Service);
		return services;
	}
}

public class ServiceWithDependencyMock : IService
{
	private readonly IServiceProvider _provider;
	public const string Service = "ServiceWithDependency";

	public ServiceWithDependencyMock(IServiceProvider provider)
	{
		_provider = provider;
	}

	public List<string> Call(List<string> services)
	{
		services.Add(Service);
		return services;
	}
}