using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Sinks;

namespace Serilog.Sinks.Mattermost.Extensions
{
    public static class MattermostSerilogExtensions
    {
        public static LoggerConfiguration Mattermost(
            this LoggerSinkConfiguration sinkConfiguration,
            Action<MattermostSinkOptions> configure)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new MattermostSinkOptions();
            configure(options);

            return sinkConfiguration.Sink(
                MattermostSinkFactory.Create(options),
                options.MinimumLevel);
        }

        public static LoggerConfiguration Mattermost(
            this LoggerSinkConfiguration sinkConfiguration,
            MattermostSinkOptions options)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return sinkConfiguration.Sink(
                MattermostSinkFactory.Create(options),
                options.MinimumLevel);
        }

        public static LoggerConfiguration Mattermost(
            this LoggerSinkConfiguration sinkConfiguration,
            IServiceProvider serviceProvider)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var provider = serviceProvider.GetRequiredService<MattermostSinkProvider>();
            var options = serviceProvider.GetRequiredService<IOptions<MattermostSinkOptions>>().Value;

            return sinkConfiguration.Sink(
                provider.Create(),
                options.MinimumLevel);
        }

        public static LoggerConfiguration Mattermost(
            this LoggerSinkConfiguration sinkConfiguration,
            string? webhookUrl = null,
            string? username = null,
            string? channel = null,
            string? iconEmoji = null,
            bool useBot = false,
            string? mattermostUrl = null,
            string? botAccessToken = null,
            string? botChannelId = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
            int batchSize = 20,
            int queueLimit = 10000,
            TimeSpan? period = null,
            int rateLimitTokens = 20,
            TimeSpan? rateLimitPeriod = null,
            int maxMessageLength = 4000,
            MattermostFormatterOptions? formatterOptions = null)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            var options = new MattermostSinkOptions
            {
                WebhookUrl = webhookUrl,
                Username = username ?? "Serilog",
                Channel = channel,
                IconEmoji = iconEmoji ?? ":robot_face:",
                UseBot = useBot,
                MattermostUrl = mattermostUrl,
                BotAccessToken = botAccessToken,
                BotChannelId = botChannelId,
                MinimumLevel = restrictedToMinimumLevel,
                BatchSize = batchSize,
                QueueLimit = queueLimit,
                Period = period ?? TimeSpan.FromSeconds(5),
                RateLimitTokens = rateLimitTokens,
                RateLimitPeriod = rateLimitPeriod ?? TimeSpan.FromSeconds(10),
                MaxMessageLength = maxMessageLength,
                FormatterOptions = formatterOptions ?? new MattermostFormatterOptions()
            };

            return sinkConfiguration.Sink(
                MattermostSinkFactory.Create(options),
                restrictedToMinimumLevel);
        }
    }
}
