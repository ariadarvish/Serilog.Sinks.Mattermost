using Serilog.Events;
using Serilog.Sinks.Mattermost.Formatting;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using Serilog.Sinks.Mattermost.Transport;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Mattermost.Sinks
{
    internal sealed class MattermostBatchingSink : IBatchedLogEventSink, IDisposable
    {
        private readonly IMattermostClient _client;
        private readonly IMattermostFormatter _formatter;
        private readonly IMattermostPayloadFactory _payloadFactory;
        private bool _disposed;

        public MattermostBatchingSink(
            IMattermostClient client,
            MattermostSinkOptions options,
            IMattermostFormatter formatter,
            IMattermostPayloadFactory payloadFactory)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _payloadFactory = payloadFactory ?? throw new ArgumentNullException(nameof(payloadFactory));
        }

        public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            if (batch == null)
            {
                return;
            }

            if (batch is IReadOnlyCollection<LogEvent> collection && collection.Count == 0)
            {
                return;
            }

            if (!batch.Any())
            {
                return;
            }

            var message = _formatter.FormatBatch(batch);
            var payloads = _payloadFactory.Create(message);

            foreach (var payload in payloads)
            {
                await _client.SendAsync(payload).ConfigureAwait(false);
            }
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _client.Dispose();
                _disposed = true;
            }
        }
    }
}
