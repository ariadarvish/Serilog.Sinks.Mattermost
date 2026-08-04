
using Microsoft.Extensions.Options;
using Serilog.Debugging;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using System.Net.Http.Json;
using System.Threading.RateLimiting;

namespace Serilog.Sinks.Mattermost.Transport
{
    internal class MattermostWebhookClient : IMattermostClient
    {
        private readonly HttpClient _httpClient;
        private readonly MattermostSinkOptions _options;
        private readonly TokenBucketRateLimiter _limiter;
        public MattermostWebhookClient(HttpClient httpClient, MattermostSinkOptions options)
        {
            _httpClient = httpClient;
            _options = options;
            _limiter = new TokenBucketRateLimiter(
               new TokenBucketRateLimiterOptions
               {
                   TokenLimit = _options.RateLimitTokens,
                   TokensPerPeriod = _options.RateLimitTokens,
                   ReplenishmentPeriod = _options.RateLimitPeriod,
                   QueueLimit = 100
               });
        }

        public async Task SendAsync(MattermostPayload payload, CancellationToken cancellationToken = default)
        {

            using var lease = await _limiter.AcquireAsync(1);
            if (!lease.IsAcquired)
            {
                SelfLog.WriteLine("Mattermost rate limit reached. Dropping message.");
                return;
            }

            using var content = JsonContent.Create(payload);
            var response = await _httpClient.PostAsync(_options.WebhookUrl, content, cancellationToken);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine(ex.ToString());
            }

        }
    }
}
