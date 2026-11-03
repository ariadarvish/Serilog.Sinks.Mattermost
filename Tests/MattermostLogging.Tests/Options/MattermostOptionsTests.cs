using FluentAssertions;
using Serilog.Sinks.Mattermost.Options;

namespace MattermostLogging.Tests.Options
{
    public class MattermostOptionsTests
    {
        [Fact]
        public void Should_Fail_When_Webhook_Is_Missing()
        {
            var options = new MattermostSinkOptions();

            Action action = () => options.Validate();

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("*WebhookUrl is required*");
        }

        [Fact]
        public void Should_Accept_Valid_Webhook()
        {
            var options = new MattermostSinkOptions
            {
                WebhookUrl = "https://mattermost.example.com/hooks/test"
            };

            Action action = () => options.Validate();

            action.Should().NotThrow();
        }

        [Fact]
        public void Should_Fail_When_Webhook_Is_Invalid_Uri()
        {
            var options = new MattermostSinkOptions
            {
                WebhookUrl = "invalid-url"
            };

            Action action = () => options.Validate();

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("*valid absolute URL*");
        }

        [Fact]
        public void Should_Fail_When_Bot_Options_Are_Missing()
        {
            var options = new MattermostSinkOptions
            {
                UseBot = true
            };

            Action action = () => options.Validate();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Accept_Valid_Bot_Options()
        {
            var options = new MattermostSinkOptions
            {
                UseBot = true,
                MattermostUrl = "https://mattermost.example.com",
                BotAccessToken = "test-token",
                BotChannelId = "test-channel"
            };

            Action action = () => options.Validate();

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_BatchSize_Is_Invalid(int batchSize)
        {
            var options = new MattermostSinkOptions
            {
                WebhookUrl = "https://mattermost.example.com/hooks/test",
                BatchSize = batchSize
            };

            Action action = () => options.Validate();

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("*BatchSize*");
        }
    }
}