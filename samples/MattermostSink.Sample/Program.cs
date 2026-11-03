using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.Mattermost.DependencyInjection;
using Serilog.Sinks.Mattermost.Extensions;
using Serilog.Sinks.Mattermost.Options;

// Enable Serilog SelfLog to diagnose any sink issues
SelfLog.Enable(Console.Error);

var builder = Host.CreateApplicationBuilder(args);

// Option A: Configure via Dependency Injection
builder.Services.AddMattermostLogging(builder.Configuration);

// Read options from configuration
var mattermostOptions = new MattermostSinkOptions();
builder.Configuration.GetSection("Mattermost").Bind(mattermostOptions);

// Option B: Configure Serilog directly
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.Mattermost(mattermostOptions)
    .CreateLogger();

var host = builder.Build();

try
{
    Log.Information("Sample application started successfully!");
    Log.Warning("This is a sample warning message sent to Mattermost.");

    try
    {
        throw new InvalidOperationException("Sample exception for testing error logs.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred in the sample application.");
    }

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
