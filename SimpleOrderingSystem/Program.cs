
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
                configurationBuilder.AddJsonFile("appsettings.json", false, false);

                //contains your specific keys. file is optional
                configurationBuilder.AddJsonFile($@"c:\temp\appsettings.json", true, false);
            });
}