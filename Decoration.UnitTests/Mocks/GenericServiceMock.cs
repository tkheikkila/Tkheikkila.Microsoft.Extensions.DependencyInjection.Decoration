namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Mocks;

public interface IGenericService<T>
{
	List<string> Call(List<string> services);
}

public class GenericServiceMock<T> : IGenericService<T>
{
	public const string Service = "GenericService";

	public List<string> Call(List<string> services)
	{
		services.Add(Service);
		return services;
	}
}

public class GenericServiceWithDependencyMock<T> : IGenericService<T>
{
	public const string Service = "GenericServiceWithDependency";

	public List<string> Call(List<string> services)
	{
		services.Add(Service);
		return services;
	}
}
