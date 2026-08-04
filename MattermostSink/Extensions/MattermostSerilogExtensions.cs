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
            return sinkConfiguration.Sink(
                MattermostSinkFactory.Create(options),
                options.MinimumLevel);
        }

        public static LoggerConfiguration Mattermost(
            this LoggerSinkConfiguration sinkConfiguration,
            string webhookUrl,
            string? username = null,
            string? channel = null,
            string? iconEmoji = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information,
            int batchSize = 10,
            int queueLimit = 1000,
            string period = "00:00:05",
            MattermostFormatterOptions? formatterOptions = null
            )
        {
            var options = new MattermostSinkOptions
            {
                WebhookUrl = webhookUrl,
                Username = username,
                Channel = channel,
                IconEmoji = iconEmoji,
                MinimumLevel = restrictedToMinimumLevel,
                BatchSize = batchSize,
                QueueLimit = queueLimit,
                Period = TimeSpan.Parse(period),
                FormatterOptions = formatterOptions,
            };

            return sinkConfiguration.Sink(
                MattermostSinkFactory.Create(options),
                restrictedToMinimumLevel);
        }
    }
}
