using Serilog.Sinks.Mattermost.Payload;

namespace Serilog.Sinks.Mattermost.Transport
{
    public interface IMattermostClient : IDisposable
    {
        Task SendAsync(MattermostPayload payload, CancellationToken cancellationToken = default);
    }
}
