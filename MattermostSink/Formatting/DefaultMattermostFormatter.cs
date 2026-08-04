
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using System.Text;

namespace Serilog.Sinks.Mattermost.Formatting
{
    internal sealed class DefaultMattermostFormatter : IMattermostFormatter
    {
        private readonly MattermostSinkOptions _options;
        private readonly MessageTemplateTextFormatter _formatter;
        public DefaultMattermostFormatter(MattermostSinkOptions options)
        {
            _options = options;
            _formatter = new MessageTemplateTextFormatter(options.FormatterOptions.OutputTemplate, null);
        }

        public MattermostMessage FormatBatch(IEnumerable<LogEvent> events)
        {
            var message = new MattermostMessage();

            foreach (var logEvent in events)
            {
                var body = BuildBody(logEvent);
                if (_options.FormatterOptions.UseAttachments)
                {
                    message.Attachments.Add(new MattermostAttachment
                    {
                        Color = GetColor(logEvent.Level),
                        Title = $"{GetEmoji(logEvent.Level)} {logEvent.Level}",
                        Text = body,
                        Footer = null,
                        Timestamp = _options.FormatterOptions.IncludeTimestamp ? logEvent.Timestamp.ToUnixTimeSeconds() : null
                    });
                }
                else
                {
                    message.Text = body;
                }
            }

            return message;
        }

       
        private string GetColor(LogEventLevel level)
        {
            return level switch
            {
                LogEventLevel.Verbose => _options.FormatterOptions.LevelColors.Verbose,
                LogEventLevel.Debug => _options.FormatterOptions.LevelColors.Debug,
                LogEventLevel.Information => _options.FormatterOptions.LevelColors.Information,
                LogEventLevel.Warning => _options.FormatterOptions.LevelColors.Warning,
                LogEventLevel.Error => _options.FormatterOptions.LevelColors.Error,
                LogEventLevel.Fatal => _options.FormatterOptions.LevelColors.Fatal,
                _ => "#2196F3"
            };
        }
        private string GetEmoji(LogEventLevel level)
        {
            var emojis = _options.FormatterOptions.Emojis;
            return level switch
            {
                LogEventLevel.Verbose => emojis.Verbose,
                LogEventLevel.Debug => emojis.Debug,
                LogEventLevel.Information => emojis.Information,
                LogEventLevel.Warning => emojis.Warning,
                LogEventLevel.Error => emojis.Error,
                LogEventLevel.Fatal => emojis.Fatal,
                _ => string.Empty
            };
        }

        private string BuildBody(LogEvent logEvent)
        {
            var builder = new StringBuilder();

            if (_options.FormatterOptions.IncludeTimestamp)
            {
                builder.AppendLine($"**Time**: {logEvent.Timestamp:yyyy-MM-dd HH:mm:ss}");
                builder.AppendLine();
            }

            using var writer = new StringWriter();
            _formatter.Format(logEvent, writer);
            builder.Append(writer);

            if (_options.FormatterOptions.IncludeProperties && logEvent.Properties.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("### Properties");

                foreach (var property in logEvent.Properties)
                {
                    builder.Append("- ");
                    builder.Append(property.Key);
                    builder.Append(": ");
                    builder.AppendLine(property.Value.ToString());
                }
            }

            if (_options.FormatterOptions.IncludeException && logEvent.Exception != null)
            {
                builder.AppendLine();
                builder.AppendLine("### Exception");
                builder.AppendLine("```");
                builder.AppendLine(logEvent.Exception.ToString());
                builder.AppendLine("```");
            }

            return builder.ToString();
        }

    }
}
