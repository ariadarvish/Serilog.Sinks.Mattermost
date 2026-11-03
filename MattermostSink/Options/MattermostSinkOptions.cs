using Serilog.Events;

namespace Serilog.Sinks.Mattermost.Options
{
    public sealed class MattermostSinkOptions
    {
        public string? WebhookUrl { get; set; }

        public string Username { get; set; } = "Serilog";

        public string? Channel { get; set; }

        public bool UseBot { get; set; } = false;

        public string? MattermostUrl { get; set; }

        public string? BotAccessToken { get; set; }

        public string? BotChannelId { get; set; }

        public string IconEmoji { get; set; } = ":robot_face:";

        public int BatchSize { get; set; } = 20;

        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(5);

        public int QueueLimit { get; set; } = 10000;

        public MattermostFormatterOptions FormatterOptions { get; set; } = new();

        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Error;

        public int RateLimitTokens { get; set; } = 20;

        public TimeSpan RateLimitPeriod { get; set; } = TimeSpan.FromSeconds(10);

        public int MaxMessageLength { get; set; } = 4000;

        public MattermostSinkOptions Validate()
        {
            if (UseBot)
            {
                if (string.IsNullOrWhiteSpace(MattermostUrl))
                {
                    throw new InvalidOperationException("MattermostUrl is required when UseBot is enabled.");
                }

                if (!Uri.TryCreate(MattermostUrl, UriKind.Absolute, out _))
                {
                    throw new InvalidOperationException("MattermostUrl must be a valid absolute URL.");
                }

                if (string.IsNullOrWhiteSpace(BotAccessToken))
                {
                    throw new InvalidOperationException("BotAccessToken is required when UseBot is enabled.");
                }

                if (string.IsNullOrWhiteSpace(BotChannelId))
                {
                    throw new InvalidOperationException("BotChannelId is required when UseBot is enabled.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(WebhookUrl))
                {
                    throw new InvalidOperationException("WebhookUrl is required.");
                }

                if (!Uri.TryCreate(WebhookUrl, UriKind.Absolute, out _))
                {
                    throw new InvalidOperationException("WebhookUrl must be a valid absolute URL.");
                }
            }

            if (BatchSize <= 0)
            {
                throw new InvalidOperationException("BatchSize must be greater than zero.");
            }

            if (QueueLimit <= 0)
            {
                throw new InvalidOperationException("QueueLimit must be greater than zero.");
            }

            if (Period <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("Period must be greater than zero.");
            }

            if (RateLimitTokens <= 0)
            {
                throw new InvalidOperationException("RateLimitTokens must be greater than zero.");
            }

            if (RateLimitPeriod <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("RateLimitPeriod must be greater than zero.");
            }

            if (MaxMessageLength <= 0)
            {
                throw new InvalidOperationException("MaxMessageLength must be greater than zero.");
            }

            return this;
        }
    }
}
