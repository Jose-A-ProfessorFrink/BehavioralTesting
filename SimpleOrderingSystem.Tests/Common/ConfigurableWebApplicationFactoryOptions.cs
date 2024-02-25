using Microsoft.Extensions.DependencyInjection;

namespace SimpleOrderingSystem.Tests.Common;

public record ConfigurableWebApplicationFactoryOptions
{
    public static readonly Func<ServiceDescriptor, bool> HostedServiceFilterRemoveAll = (a) => true;
    public static readonly Func<ServiceDescriptor, bool> HostedServiceFilterRemoveNone = (a) => false;

    /// <summary>
    /// A function that returns a set of command line arguments you wish to pass to the startup routine
    /// </summary>
    public Func<Dictionary<string, object>>? CommandLineArguments { get; init; }

    /// <summary>
    /// A filter that specifies which hosted services should be removed from the web application factory container. 
    /// </summary>
    public Func<ServiceDescriptor, bool> HostedServiceFilter { get; init; } = HostedServiceFilterRemoveNone;

    /// <summary>
    /// Creates a set of default options that are useful for behavioral tests.
    /// </summary>
    /// <param name="testConfiguration"></param>
    /// <param name="commandLineArguments"></param>
    /// <returns></returns>
    public static ConfigurableWebApplicationFactoryOptions CreateDefaultBehavioralOptions(
        Dictionary<string, object>? commandLineArguments = null)
    {
        return new()
        {
            CommandLineArguments = () =>
            {
                commandLineArguments = commandLineArguments ?? new Dictionary<string, object>();


                return commandLineArguments;
            },
            HostedServiceFilter = ConfigurableWebApplicationFactoryOptions.HostedServiceFilterRemoveAll
        };
    }
}
