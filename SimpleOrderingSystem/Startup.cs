using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

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
        /*
        services
            .RegisterDomain(_configuration)
            .RegisterMongoProviders(_configuration)
            .RegisterZipCodeProvider(_configuration)
            .RegisterOrderEventsProvider(_configuration);
            */
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
