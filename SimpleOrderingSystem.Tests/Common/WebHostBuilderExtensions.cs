using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SimpleOrderingSystem.Tests.Common;


/// <summary>
/// Helper class that extends <see cref="IWebHostBuilder"/>.
/// </summary>
public static class WebHostBuilderExtensions
{
    /// <summary>
    /// The running of background hosted services is counter-productive to what we want to achieve in most test scenarios.
    /// </summary>
    /// <param name="webHostBuilder"></param>
    /// <param name="removeFilterFunction"></param>
    /// <returns></returns>
    public static IWebHostBuilder RemoveAllHostedServices(this IWebHostBuilder webHostBuilder, Func<ServiceDescriptor, bool> removeFilterFunction)
    {
        webHostBuilder.ConfigureTestServices(serviceCollection =>
        {
            serviceCollection
                .Where(serviceDescriptor => typeof(IHostedService).IsAssignableFrom(serviceDescriptor.ImplementationType) && removeFilterFunction(serviceDescriptor))
                .ToList()
                .ForEach(serviceDescriptor => serviceCollection.Remove(serviceDescriptor));
        });

        return webHostBuilder;
    }
}