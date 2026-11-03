using Microsoft.Extensions.Options;
using Serilog.Debugging;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.RateLimiting;

namespace Serilog.Sinks.Mattermost.Transport
{
    public class MattermostWebhookClient : IMattermostClient
    {
        private readonly HttpClient _httpClient;
        private readonly MattermostSinkOptions _options;
        private readonly TokenBucketRateLimiter _limiter;
        private readonly bool _disposeHttpClient;
        private bool _disposed;

        public MattermostWebhookClient(
            HttpClient httpClient,
            IOptions<MattermostSinkOptions> options)
            : this(httpClient, options?.Value ?? throw new ArgumentNullException(nameof(options)), disposeHttpClient: false)
        {
        }

        public MattermostWebhookClient(
            HttpClient httpClient,
            MattermostSinkOptions options,
            bool disposeHttpClient = false)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _disposeHttpClient = disposeHttpClient;

            _limiter = new TokenBucketRateLimiter(
                new TokenBucketRateLimiterOptions
                {
                    TokenLimit = _options.RateLimitTokens > 0 ? _options.RateLimitTokens : 20,
                    TokensPerPeriod = _options.RateLimitTokens > 0 ? _options.RateLimitTokens : 20,
                    ReplenishmentPeriod = _options.RateLimitPeriod > TimeSpan.Zero ? _options.RateLimitPeriod : TimeSpan.FromSeconds(10),
                    QueueLimit = 100
                });
        }

        public async Task SendAsync(MattermostPayload payload, CancellationToken cancellationToken = default)
        {
            if (_disposed)
            {
                return;
            }

            using var lease = await _limiter.AcquireAsync(1, cancellationToken).ConfigureAwait(false);
            if (!lease.IsAcquired)
            {
                SelfLog.WriteLine("Mattermost rate limit reached. Dropping message.");
                return;
            }

            try
            {
                if (_options.UseBot)
                {
                    await SendAsBotAsync(payload, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await SendViaWebhookAsync(payload, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine($"Failed to send Mattermost message: {ex}");
            }
        }

        private async Task SendViaWebhookAsync(MattermostPayload payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.WebhookUrl))
            {
                SelfLog.WriteLine("Mattermost WebhookUrl is not configured.");
                return;
            }

            using var content = JsonContent.Create(payload);
            using var response = await _httpClient.PostAsync(_options.WebhookUrl, content, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                SelfLog.WriteLine($"Mattermost webhook returned {(int)response.StatusCode} {response.ReasonPhrase}: {errorBody}");
            }
        }

        private async Task SendAsBotAsync(MattermostPayload payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.MattermostUrl))
            {
                SelfLog.WriteLine("MattermostUrl is not configured for bot mode.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_options.BotAccessToken))
            {
                SelfLog.WriteLine("BotAccessToken is not configured for bot mode.");
                return;
            }

            var channelId = !string.IsNullOrWhiteSpace(payload.Channel) ? payload.Channel : _options.BotChannelId;
            if (string.IsNullOrWhiteSpace(channelId))
            {
                SelfLog.WriteLine("Mattermost BotChannelId is not configured.");
                return;
            }

            var requestPayload = new MattermostCreatePostRequest
            {
                ChannelId = channelId,
                Message = payload.Text ?? string.Empty,
                Props = payload.Attachments is { Count: > 0 }
                    ? new MattermostPostProps
                    {
                        Attachments = payload.Attachments
                    }
                    : null
            };

            var url = $"{_options.MattermostUrl.TrimEnd('/')}/api/v4/posts";
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(requestPayload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.BotAccessToken);

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                SelfLog.WriteLine($"Mattermost API returned {(int)response.StatusCode} {response.ReasonPhrase}: {errorBody}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _limiter.Dispose();
                    if (_disposeHttpClient)
                    {
                        _httpClient.Dispose();
                    }
                }

                _disposed = true;
            }
        }
    }
}
