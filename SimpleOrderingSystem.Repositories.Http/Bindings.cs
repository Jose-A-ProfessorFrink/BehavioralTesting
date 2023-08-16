
using SimpleOrderingSystem.Domain.Repositories;
using SimpleOrderingSystem.Repositories.Http.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleOrderingSystem.Domain.Extensions;

namespace SimpleOrderingSystem.Repositories.Http;

public static class Bindings
{
    public static IServiceCollection RegisterHttpProviders(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IMoviesRepository, MoviesRepository>()
            .AddTransient<IZipCodeRepository,ZipCodeRepository>();
        
        services
            .AddTransient<IMovieProvider,MovieProvider>()
            .AddTransient<IZipCodeProvider,ZipCodeProvider>();

        services
            .AddHttpClient(MovieProvider.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(configuration.GetRequiredValue("OmdbApiUrl"));
            });

        services
            .AddHttpClient(ZipCodeProvider.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(configuration.GetRequiredValue("ZipwiseApiUrl"));
            });

        return services;
    }
}