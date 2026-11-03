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

            if (_options.FormatterOptions.UseAttachments)
            {
                foreach (var logEvent in events)
                {
                    var body = BuildBody(logEvent);
                    var titlePrefix = _options.FormatterOptions.IncludeEmoji ? GetEmoji(logEvent.Level) : string.Empty;
                    var titleLevel = _options.FormatterOptions.IncludeLevel ? logEvent.Level.ToString() : string.Empty;
                    var title = $"{titlePrefix} {titleLevel}".Trim();

                    message.Attachments.Add(new MattermostAttachment
                    {
                        Color = GetColor(logEvent.Level),
                        Title = string.IsNullOrEmpty(title) ? null : title,
                        Text = body,
                        Footer = null,
                        Timestamp = null
                    });
                }
            }
            else
            {
                var combinedText = new StringBuilder();
                foreach (var logEvent in events)
                {
                    var body = BuildBody(logEvent);
                    if (combinedText.Length > 0)
                    {
                        combinedText.AppendLine();
                        combinedText.AppendLine("---");
                        combinedText.AppendLine();
                    }

                    var titlePrefix = _options.FormatterOptions.IncludeEmoji ? GetEmoji(logEvent.Level) : string.Empty;
                    var titleLevel = _options.FormatterOptions.IncludeLevel ? logEvent.Level.ToString() : string.Empty;
                    var header = $"{titlePrefix} {titleLevel}".Trim();

                    if (!string.IsNullOrEmpty(header))
                    {
                        combinedText.AppendLine($"### {header}");
                    }

                    combinedText.Append(body);
                }

                message.Text = combinedText.ToString();
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
                var timeZone = _options.FormatterOptions.TimeZone ?? TimeZoneInfo.Utc;
                var displayTime = TimeZoneInfo.ConvertTime(logEvent.Timestamp, timeZone);
                builder.AppendLine($"**Time**: {displayTime:yyyy-MM-dd HH:mm:ss}");
                builder.AppendLine();
            }

            using (var writer = new StringWriter())
            {
                _formatter.Format(logEvent, writer);
                builder.Append(writer.ToString().TrimEnd());
            }

            if (_options.FormatterOptions.IncludeProperties && logEvent.Properties.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine("### Properties");

                foreach (var property in logEvent.Properties)
                {
                    builder.Append("- ");
                    builder.Append(property.Key);
                    builder.Append(": ");
                    using var propWriter = new StringWriter();
                    property.Value.Render(propWriter);
                    builder.AppendLine(propWriter.ToString());
                }
            }

            if (_options.FormatterOptions.IncludeException && logEvent.Exception != null)
            {
                builder.AppendLine();
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
