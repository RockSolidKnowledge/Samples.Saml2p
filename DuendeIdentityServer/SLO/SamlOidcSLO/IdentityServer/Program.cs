using IdentityServer;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration))
        .ConfigureLogging((_, logging) => 
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.ClearProviders();
            logging.AddSimpleConsole(options => options.IncludeScopes = true);
            if (OperatingSystem.IsWindows())
            {
                logging.AddEventLog();
            }
        });
    WebApplication app = builder
        .ConfigureServices()
        .ConfigurePipeline();
    
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    await Log.CloseAndFlushAsync();
}