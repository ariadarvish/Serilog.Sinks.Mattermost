
using Serilog.Sinks.Mattermost.Payload;

namespace Serilog.Sinks.Mattermost.Transport
{
    internal interface IMattermostClient
    {
        Task SendAsync(MattermostPayload payload, CancellationToken cancellationToken = default);
    }
}
