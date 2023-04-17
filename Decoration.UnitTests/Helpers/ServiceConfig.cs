using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.UnitTests.Helpers;

public delegate IServiceBuilder<T> ServiceConfig<T>(IServiceBuilder<T> builder) where T : notnull;
public delegate IServiceBuilder ServiceConfig(IServiceBuilder builder);