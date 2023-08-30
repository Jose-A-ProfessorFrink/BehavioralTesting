using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SimpleOrderingSystem.Tests.Common;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Replaces all registered services with a singleton.
    /// </summary>
    /// <typeparam name="TService">Service type.</typeparam>
    /// <param name="services"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public static IServiceCollection ReplaceAllWithSingleton<TService>(this IServiceCollection services, TService service)
        where TService : class
    {
        services.RemoveAll<TService>();
        services.AddSingleton(service);
        return services;
    }
}