using Microsoft.Extensions.Configuration;
using SimpleOrderingSystem;

namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// The web application factory for this solution.
/// </summary>
public sealed class WebApplicationFactory : ConfigurableWebApplicationFactory<Startup>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebApplicationFactory"/> class.
    /// Removes all hosted services except the one specified. If not specified, removes all.
    /// </summary>
    /// <param name="removeHostedServicesExcept"></param>
    /// <param name="testConfiguration"></param>
    public WebApplicationFactory(Action<IConfigurationBuilder>? testConfiguration = null)
        : base(testConfiguration)
    {

    }

    /// <summary>
    /// Creates a web application factory
    /// </summary>
    /// <param name="testConfiguration"></param>
    /// <returns></returns>
    public static WebApplicationFactory Create(
        Action<IConfigurationBuilder>? testConfiguration = null)
    {
        return new(testConfiguration);
    }

}
