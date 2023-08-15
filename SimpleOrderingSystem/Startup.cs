using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using SimpleOrderingSystem.Repositories;
using SimpleOrderingSystem.Services;
using SimpleOrderingSystem.Providers;
using SimpleOrderingSystem.Extensions;

namespace SimpleOrderingSystem;


public class Startup
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.Filters.Add<ExceptionFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.Converters.Add(new ValidationProblemDetailsConverter());
                options.SerializerSettings.Converters.Add(new ProblemDetailsConverter());

                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });

        RegisterDependencies(services);
    }

    /// <summary>
    /// Register all modules
    /// </summary>
    public void RegisterDependencies(IServiceCollection services)
    {     
        services
            .AddTransient<ICustomersRepository, CustomersRepository>()
            .AddTransient<IMoviesRepository, MoviesRepository>()
            .AddTransient<IOrdersRepository, OrdersRepository>()
            .AddTransient<IZipCodeRepository,ZipCodeRepository>();
        
        services
            .AddTransient<ICustomerService,CustomerService>()
            .AddTransient<IMovieService,MovieService>()
            .AddTransient<IOrderService,OrderService>();

        services
            .AddTransient<IMovieProvider,MovieProvider>()
            .AddTransient<IZipCodeProvider,ZipCodeProvider>();

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
                var connectionString = _configuration.GetValue<string>("LiteDbConnectionString");

                var provider = new LiteDbProvider(connectionString);

                provider.Initialize();

                return provider;
            });

        services
            .AddHttpClient(MovieProvider.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(_configuration.GetRequiredValue("OmdbApiUrl"));
            });

        services
            .AddHttpClient(ZipCodeProvider.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(_configuration.GetRequiredValue("ZipwiseApiUrl"));
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        app.UseEndpoints(e => e.MapControllers());
    }
}
