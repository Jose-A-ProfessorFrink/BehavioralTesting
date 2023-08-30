using Microsoft.Extensions.DependencyInjection;

namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// Allows simple setup of a custom web application factory using extensions. <see cref="ConfigurableWebApplicationFactoryExtensions"/> <br/>
/// You should not use <see cref="ConfigurableWebApplicationFactory{TEntryPoint}"/> directly. <br/>
/// </summary>
public interface IConfigurableWebApplicationFactory
{
    /// <summary>
    /// Allows configuration of the <see cref="IServiceCollection"/> to enable registering of mocks and overriding dependencies.
    /// </summary>
    /// <param name="action"></param>
    void ConfigureTestServices(Action<IServiceCollection> action);
}
