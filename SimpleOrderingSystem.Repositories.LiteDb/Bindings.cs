
using SimpleOrderingSystem.Domain.Repositories;
using SimpleOrderingSystem.Repositories.LiteDB.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleOrderingSystem.Domain.Extensions;
using LiteDB;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Repositories.LiteDB;

public static class Bindings
{
    public static IServiceCollection RegisterLiteDbProviders(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<ICustomersRepository, CustomersRepository>()
            .AddTransient<IOrdersRepository, OrdersRepository>();
        

        // Register this provider as a singleton. It will do some initialization work
        // that should only happen once. It is CRITICAL that this be defined as a lambda
        // because that defers the execution of this code until the last responsible moment. 
        // If we replace the registration for this interface in the container before we ask for
        // the first instance, this makes sure we can get a mock without ever running this code.
        // Making sure code doesn't run in a test context that we would otherwise
        // find problematic is a required and fundamental part of our testing strategy.
        services
            .AddSingleton<ILiteDbProvider>(serviceProvider => 
            {

                var connectionString = serviceProvider.GetRequiredService<IOptions<SimpleOrderingSystemOptions>>().Value.LiteDbConnectionString!;

                var provider = new LiteDbProvider(connectionString);

                provider.Initialize();

                return provider;
            });

        return services;
    }
}