using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Mattermost.Formatting;
using Serilog.Sinks.Mattermost.Options;

namespace MattermostLogging.Tests.Formatting
{
    public class DefaultMattermostFormatterTests
    {
        private readonly MattermostSinkOptions _options;

        public DefaultMattermostFormatterTests()
        {
            _options = new MattermostSinkOptions
            {
                FormatterOptions = new MattermostFormatterOptions
                {
                    IncludeTimestamp = true,
                    IncludeProperties = true,
                    IncludeException = true,
                    IncludeEmoji = true
                }
            };
        }

        [Fact]
        public void Should_Format_Error_Log_With_Attachment()
        {
            var formatter = new DefaultMattermostFormatter(_options);

            var parser = new MessageTemplateParser();
            var log = new LogEvent(
                DateTimeOffset.UtcNow,
                LogEventLevel.Error,
                null,
                parser.Parse("Test error"),
                Array.Empty<LogEventProperty>());

            var result = formatter.FormatBatch(new[] { log });
            result.Attachments.Should().ContainSingle();
            var attachment = result.Attachments.First();

            attachment.Title.Should().Contain("Error");
            attachment.Color.Should().NotBeNullOrEmpty();
            attachment.Text.Should().Contain("Test error");
        }

        [Fact]
        public void Should_Include_Exception()
        {
            var formatter = new DefaultMattermostFormatter(_options);

            var exception = new InvalidOperationException("failed operation");
            var parser = new MessageTemplateParser();

            var log = new LogEvent(
                DateTimeOffset.UtcNow,
                LogEventLevel.Fatal,
                exception,
                parser.Parse("Crash"),
                Array.Empty<LogEventProperty>());

            var result = formatter.FormatBatch(new[] { log });

            result.Attachments[0]
                .Text
                .Should()
                .Contain("failed operation");
        }

        [Fact]
        public void Should_Preserve_All_Events_When_Attachments_Disabled()
        {
            var options = new MattermostSinkOptions
            {
                FormatterOptions = new MattermostFormatterOptions
                {
                    UseAttachments = false
                }
            };

            var formatter = new DefaultMattermostFormatter(options);
            var parser = new MessageTemplateParser();

            var log1 = new LogEvent(
                DateTimeOffset.UtcNow,
                LogEventLevel.Information,
                null,
                parser.Parse("First event"),
                Array.Empty<LogEventProperty>());

            var log2 = new LogEvent(
                DateTimeOffset.UtcNow,
                LogEventLevel.Warning,
                null,
                parser.Parse("Second event"),
                Array.Empty<LogEventProperty>());

            var result = formatter.FormatBatch(new[] { log1, log2 });

            result.Attachments.Should().BeEmpty();
            result.Text.Should().Contain("First event");
            result.Text.Should().Contain("Second event");
        }

        [Fact]
        public void Should_Use_Configured_TimeZone()
        {
            var utcPlus330 = TimeZoneInfo.CreateCustomTimeZone("CustomZone", TimeSpan.FromMinutes(210), "Custom Zone", "Custom Zone");
            var options = new MattermostSinkOptions
            {
                FormatterOptions = new MattermostFormatterOptions
                {
                    IncludeTimestamp = true,
                    TimeZone = utcPlus330
                }
            };

            var formatter = new DefaultMattermostFormatter(options);
            var parser = new MessageTemplateParser();
            var timestamp = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

            var log = new LogEvent(
                timestamp,
                LogEventLevel.Information,
                null,
                parser.Parse("Time test"),
                Array.Empty<LogEventProperty>());

            var result = formatter.FormatBatch(new[] { log });

            // 12:00 UTC + 3:30 = 15:30
            result.Attachments[0].Text.Should().Contain("2026-01-01 15:30:00");
        }
    }
}
