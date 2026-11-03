using Moq;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Mattermost.Formatting;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using Serilog.Sinks.Mattermost.Sinks;
using Serilog.Sinks.Mattermost.Transport;



public class MattermostBatchingSinkTests
{

    [Fact]
    public async Task Should_Send_Batched_Messages()
    {
        var client = new Mock<IMattermostClient>();

        var formatter = new Mock<IMattermostFormatter>();

        var factory = new Mock<IMattermostPayloadFactory>();


        formatter.Setup(x => x.FormatBatch(It.IsAny<IEnumerable<LogEvent>>()))
            .Returns(new MattermostMessage
            {
                Text = "hello"
            });


        factory.Setup(x => x.Create(It.IsAny<MattermostMessage>()))
            .Returns(new[]
                {
                    new MattermostPayload()
                });


        var sink = new MattermostBatchingSink(
                client.Object,
                new MattermostSinkOptions(),
                formatter.Object,
                factory.Object);
        var parser = new MessageTemplateParser();

        await sink.EmitBatchAsync(
            new[]
            {
                new LogEvent(
                    DateTimeOffset.Now,
                    LogEventLevel.Information,
                    null,
                    parser.Parse("test"),
                    Array.Empty<LogEventProperty>())
            });


        client.Verify(
            x => x.SendAsync(
                It.IsAny<MattermostPayload>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}