using Serilog.Core;
using Serilog.Sinks.Mattermost.Formatting;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using Serilog.Sinks.Mattermost.Transport;
using Serilog.Sinks.PeriodicBatching;


namespace Serilog.Sinks.Mattermost.Sinks
{
    internal class MattermostSinkFactory
    {
        public static ILogEventSink Create(MattermostSinkOptions options)
        {
            options.Validate();

            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "Serilog.Sinks.Mattermost");

            var client = new MattermostWebhookClient(httpClient,options);
            var formatter = new DefaultMattermostFormatter(options);
            var payloadFactory = new MattermostPayloadFactory(options);
            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = options.BatchSize,
                Period = options.Period,
                QueueLimit = options.QueueLimit
            };

            return new PeriodicBatchingSink(
                new MattermostBatchingSink(
                    client,
                    options,
                    formatter,
                    payloadFactory),
                batchingOptions);
        }
    }
}
