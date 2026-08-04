
using Serilog.Events;
using Serilog.Sinks.Mattermost.Formatting;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using Serilog.Sinks.Mattermost.Transport;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Mattermost.Sinks
{
    internal sealed class MattermostBatchingSink: IBatchedLogEventSink
    {
        private readonly IMattermostClient _client;
        private readonly IMattermostFormatter _formatter;
        private readonly IMattermostPayloadFactory _payloadFactory;

        public MattermostBatchingSink(
            IMattermostClient client,
            MattermostSinkOptions options,
            IMattermostFormatter formatter,
            IMattermostPayloadFactory payloadFactory) 
        {
            _client = client;
            _formatter = formatter;
            _payloadFactory = payloadFactory;
        }

        public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            if(batch.ToList().Count == 0) return;

            var markdown = _formatter.FormatBatch(batch);
            var payloads = _payloadFactory.Create(markdown);

            foreach (var payload in payloads)
            {
                await _client.SendAsync(payload).ConfigureAwait(false);
            }
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}
