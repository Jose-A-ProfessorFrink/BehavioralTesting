using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// A configurable web application factory.
/// <see cref="ConfigurableWebApplicationFactoryExtensions"/>.
/// </summary>
/// <typeparam name="TEntryPoint">Generic entrypoint.</typeparam>
public class ConfigurableWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IConfigurableWebApplicationFactory
    where TEntryPoint : class
{
    private readonly List<Action<IServiceCollection>> _testServiceConfigurations = new();
    private readonly Action<IConfigurationBuilder>? _testConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurableWebApplicationFactory{TEntryPoint}"/> class.
    /// </summary>
    /// <param name="configuration"></param>
    public ConfigurableWebApplicationFactory(Action<IConfigurationBuilder>? configuration = null)
    {
        _testConfiguration = configuration;
    }

    /// <summary>
    /// Adds an <see cref="Action"/>(<see cref="IServiceCollection"/>) method to the test configuration.
    /// </summary>
    /// <param name="action"></param>
    public void ConfigureTestServices(Action<IServiceCollection> action)
    {
        _testServiceConfigurations.Add(action);
    }

    /// <summary>
    /// Overrides the base <see cref="WebApplicationFactory"/> virtual method to allow for adjustment of configuration option conditions.
    /// </summary>
    /// <param name="builder"></param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(sc =>
        {
            foreach (var testServiceConfiguration in _testServiceConfigurations)
            {
                testServiceConfiguration(sc);
            }
        });

        if (_testConfiguration != null)
        {
            builder.ConfigureAppConfiguration(_testConfiguration);
        }
    }
}
