using FluentAssertions;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using Serilog.Sinks.Mattermost.Transport;
using System.Net;

namespace MattermostLogging.Tests.Transport
{
    public class MattermostWebhookClientTests
    {
        [Fact]
        public async Task Should_Send_Post_To_WebhookUrl()
        {
            HttpRequestMessage? sentRequest = null;
            var handler = new TestHttpMessageHandler((req, ct) =>
            {
                sentRequest = req;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            var httpClient = new HttpClient(handler);
            var options = new MattermostSinkOptions
            {
                WebhookUrl = "https://mattermost.example.com/hooks/test1234"
            };

            using var client = new MattermostWebhookClient(httpClient, options);
            await client.SendAsync(new MattermostPayload { Text = "Test payload" });

            sentRequest.Should().NotBeNull();
            sentRequest!.RequestUri.Should().Be(new Uri("https://mattermost.example.com/hooks/test1234"));
            sentRequest.Method.Should().Be(HttpMethod.Post);
        }

        [Fact]
        public async Task Should_Send_As_Bot_When_Configured()
        {
            HttpRequestMessage? sentRequest = null;
            var handler = new TestHttpMessageHandler((req, ct) =>
            {
                sentRequest = req;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created));
            });

            var httpClient = new HttpClient(handler);
            var options = new MattermostSinkOptions
            {
                UseBot = true,
                MattermostUrl = "https://mattermost.example.com",
                BotAccessToken = "my-secret-token",
                BotChannelId = "channel-abc"
            };

            using var client = new MattermostWebhookClient(httpClient, options);
            await client.SendAsync(new MattermostPayload { Text = "Bot message" });

            sentRequest.Should().NotBeNull();
            sentRequest!.RequestUri.Should().Be(new Uri("https://mattermost.example.com/api/v4/posts"));
            sentRequest.Headers.Authorization.Should().NotBeNull();
            sentRequest.Headers.Authorization!.Scheme.Should().Be("Bearer");
            sentRequest.Headers.Authorization!.Parameter.Should().Be("my-secret-token");
        }

        private class TestHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

            public TestHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
            {
                _handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _handler(request, cancellationToken);
            }
        }
    }
}
