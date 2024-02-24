using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
    private readonly ConfigurableWebApplicationFactoryOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurableWebApplicationFactory{TEntryPoint}"/> class.
    /// </summary>
    /// <param name="configuration"></param>
    public ConfigurableWebApplicationFactory(ConfigurableWebApplicationFactoryOptions options)
    {
        _options = options;
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
        // setup command line arguments
        _options.CommandLineArguments?.Invoke()?.ToList().ForEach(a => builder.UseSetting(a.Key, a.Value.ToString()));

        builder.ConfigureTestServices(sc =>
        {
            foreach (var testServiceConfiguration in _testServiceConfigurations)
            {
                testServiceConfiguration(sc);
            }
        });

        if (this._options.TestConfiguration is not null)
        {
            // apply our test configuration
            builder.ConfigureAppConfiguration(this._options.TestConfiguration);
        }

        // remove the hosted services as specified by our options
        //builder.RemoveAllHostedServices(this._options.HostedServiceFilter);
    }
}
