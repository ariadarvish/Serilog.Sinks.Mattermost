using Serilog.Events;
using Serilog.Sinks.Mattermost.Payload;

namespace Serilog.Sinks.Mattermost.Options
{
    public sealed class MattermostSinkOptions
    {
        public string WebhookUrl { get; set; } = "";

        public string Username { get; set; } = "Serilog";

        public string? Channel { get; set; }

        public string IconEmoji { get; set; } = ":robot_face:";

        public int BatchSize { get; set; } = 20;

        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(5);

        public int QueueLimit { get; set; } = 10000;

        internal MattermostFormatterOptions FormatterOptions { get; set; } = new();

        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Error;

        public int RateLimitTokens { get; set; } = 20;

        public TimeSpan RateLimitPeriod { get; set; } = TimeSpan.FromSeconds(10);


        public MattermostSinkOptions Validate()
        {
            if (string.IsNullOrWhiteSpace(WebhookUrl))
                throw new InvalidOperationException("WebhookUrl is required.");

            if (!Uri.TryCreate(WebhookUrl, UriKind.Absolute, out _))
            {
                throw new InvalidOperationException("WebhookUrl is invalid.");
            }

            return this;
        }
    }
}
