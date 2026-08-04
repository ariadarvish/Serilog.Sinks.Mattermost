

namespace Serilog.Sinks.Mattermost.Payload
{
    internal interface IMattermostPayloadFactory
    {
        IEnumerable<MattermostPayload> Create(MattermostMessage markdown);
    }
}
