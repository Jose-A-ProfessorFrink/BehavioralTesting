
namespace SimpleOrderingSystem;

public static class Program
{

    /// <summary>
    /// Application entry point
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        NewHostBuilder(args).Build().Run();
    }

    /// <summary>
    /// Create host builder.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHostBuilder NewHostBuilder(string[] args) =>
        new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder.UseShutdownTimeout(TimeSpan.FromSeconds(20));
                webHostBuilder.UseKestrel();
                webHostBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging((context, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    builder.AddDebug();
                    builder.AddEventSourceLogger();
            })
            .ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                /*
                 * When run in Kubernetes, these files will be populated from configmaps and secrets
                 * specific to the environment. Each file overrides some portion of the default
                 * appsettings.json, which is what is used when running the project locally.
                 */
                configurationBuilder.AddJsonFile("appsettings.json", false, false);
                configurationBuilder.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, false);
            });
}














/*

    public static void Main(string[] args)
    {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.MapGet("/weatherforecast", () =>
    {
        var forecast =  Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateTime.Now.AddDays(index),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

    app.Run();

    record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
}
*/