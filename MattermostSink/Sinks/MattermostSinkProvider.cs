using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog.Core;
using Serilog.Sinks.Mattermost.Formatting;
using Serilog.Sinks.Mattermost.Options;
using Serilog.Sinks.Mattermost.Payload;
using Serilog.Sinks.Mattermost.Transport;
using Serilog.Sinks.PeriodicBatching;


namespace Serilog.Sinks.Mattermost.Sinks
{
    internal sealed class MattermostSinkProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public MattermostSinkProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public ILogEventSink Create()
        {
            var client =_serviceProvider.GetRequiredService<IMattermostClient>();
            var formatter =_serviceProvider.GetRequiredService<IMattermostFormatter>();
            var options =_serviceProvider.GetRequiredService<IOptions<MattermostSinkOptions>>().Value;
            var payloadFactory =_serviceProvider.GetRequiredService<IMattermostPayloadFactory>();

            options.Validate();

            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = options.BatchSize,
                Period = options.Period,
                QueueLimit = options.QueueLimit
            };

            var batchedSink = new MattermostBatchingSink(
                client,
                options,
                formatter,
                payloadFactory);

            return new PeriodicBatchingSink(
                batchedSink,
                batchingOptions);
        }
    }
}
