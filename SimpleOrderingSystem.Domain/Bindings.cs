using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleOrderingSystem.Domain.Models;
using SimpleOrderingSystem.Domain.Providers;
using SimpleOrderingSystem.Domain.Services;

namespace SimpleOrderingSystem.Domain;

public static class Bindings
{
    public static IServiceCollection RegisterDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<ICustomerService,CustomerService>()
            .AddTransient<IMovieService,MovieService>()
            .AddTransient<IOrderService,OrderService>();

        services
            .AddTransient<IGuidProvider,GuidProvider>()
            .AddTransient<IDateTimeProvider,DateTimeProvider>();

        return services;
    }
}