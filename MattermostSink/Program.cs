using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Mattermost.DependencyInjection;
using Serilog.Sinks.Mattermost.Extensions;
using Serilog.Sinks.Mattermost.Options;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddMattermostLogging(builder.Configuration);


var host = builder.Build();


//MattermostConfiguration.UseServiceProvider(host.Services);

Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

//var options = new MattermostSinkOptions();

//builder.Configuration
//    .GetSection("Mattermost")
//    .Bind(options);


//Log.Logger = new LoggerConfiguration()
//                .ReadFrom.Configuration(builder.Configuration)
//                .WriteTo.Mattermost(options)
//                .CreateLogger();

try
{
    Log.Information("Application started");
    Log.Fatal(new InvalidOperationException(), "error occured during running app");

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

