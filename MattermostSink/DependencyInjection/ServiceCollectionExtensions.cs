using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Serilog.Sinks.Mattermost.Formatting;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using Serilog.Sinks.Mattermost.Sinks;
using Serilog.Sinks.Mattermost.Transport;
using System.Net;


namespace Serilog.Sinks.Mattermost.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMattermostLogging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MattermostSinkOptions>(configuration.GetSection("Mattermost"));
            services.AddHttpClient<IMattermostClient, MattermostWebhookClient>(
                 client =>
                 {
                     client.Timeout = TimeSpan.FromSeconds(10);
                     client.DefaultRequestHeaders.Add("User-Agent","Serilog.MattermostSink");
                 })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(response =>
                response.StatusCode == HttpStatusCode.TooManyRequests || (int)response.StatusCode >= 500);
            });

            services.AddSingleton<IMattermostFormatter>(sp =>
            {
                var options =sp.GetRequiredService<IOptions<MattermostSinkOptions>>().Value;
                return new DefaultMattermostFormatter(options);
            });

            services.AddSingleton<IMattermostPayloadFactory>(sp =>
            {
                var options =sp.GetRequiredService<IOptions<MattermostSinkOptions>>().Value;
                return new MattermostPayloadFactory(options);
            });

            services.AddSingleton<MattermostSinkProvider>();

            return services;
        }
    }
}
