using FluentAssertions;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;

namespace MattermostLogging.Tests.Payload
{
    public class MattermostPayloadFactoryTests
    {
        [Fact]
        public void Should_Create_Payload_With_Metadata()
        {
            var options = new MattermostSinkOptions
            {
                Username = "TestApp",
                Channel = "logs",
                IconEmoji = ":robot:"
            };

            var factory = new MattermostPayloadFactory(options);

            var message = new MattermostMessage
            {
                Text = "hello"
            };

            var result = factory.Create(message).ToList();

            result.Should().ContainSingle();
            result[0].Username.Should().Be("TestApp");
            result[0].Channel.Should().Be("logs");
            result[0].IconEmoji.Should().Be(":robot:");
            result[0].Text.Should().Be("hello");
        }

        [Fact]
        public void Should_Split_Large_Message()
        {
            var options = new MattermostSinkOptions
            {
                MaxMessageLength = 4000
            };

            var factory = new MattermostPayloadFactory(options);

            var message = new MattermostMessage
            {
                Text = new string('x', 5000)
            };

            var result = factory.Create(message).ToList();

            // 5000 chars should be split into 2 chunks (4000 + 1000)
            result.Should().HaveCount(2);
            result[0].Text.Should().HaveLength(4000);
            result[1].Text.Should().HaveLength(1000);
        }

        [Fact]
        public void Should_Truncate_Oversized_Attachment_Text()
        {
            var options = new MattermostSinkOptions
            {
                MaxMessageLength = 100
            };

            var factory = new MattermostPayloadFactory(options);

            var message = new MattermostMessage
            {
                Attachments = new List<MattermostAttachment>
                {
                    new MattermostAttachment
                    {
                        Title = "Test Attachment",
                        Text = new string('a', 200)
                    }
                }
            };

            var result = factory.Create(message).ToList();

            result.Should().ContainSingle();
            result[0].Attachments.Should().ContainSingle();
            result[0].Attachments![0].Text.Should().EndWith("... [truncated]");
            result[0].Attachments![0].Text!.Length.Should().BeLessThanOrEqualTo(100);
        }
    }
}