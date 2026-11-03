using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog.Sinks.Mattermost.Formatting;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using Serilog.Sinks.Mattermost.Sinks;
using Serilog.Sinks.Mattermost.Transport;

namespace Serilog.Sinks.Mattermost.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMattermostLogging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.Configure<MattermostSinkOptions>(configuration.GetSection("Mattermost"));
            return services.AddMattermostLoggingServices();
        }

        public static IServiceCollection AddMattermostLogging(
            this IServiceCollection services,
            Action<MattermostSinkOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.Configure(configure);
            return services.AddMattermostLoggingServices();
        }

        private static IServiceCollection AddMattermostLoggingServices(this IServiceCollection services)
        {
            services.AddHttpClient<IMattermostClient, MattermostWebhookClient>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("User-Agent", "Serilog.Sinks.Mattermost");
            });

            services.AddSingleton<IMattermostFormatter>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MattermostSinkOptions>>().Value;
                return new DefaultMattermostFormatter(options);
            });

            services.AddSingleton<IMattermostPayloadFactory>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MattermostSinkOptions>>().Value;
                return new MattermostPayloadFactory(options);
            });

            services.AddSingleton<MattermostSinkProvider>();

            return services;
        }
    }
}
