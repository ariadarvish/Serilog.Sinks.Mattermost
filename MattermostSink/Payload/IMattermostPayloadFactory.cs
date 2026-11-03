namespace Serilog.Sinks.Mattermost.Payload
{
    public interface IMattermostPayloadFactory
    {
        IEnumerable<MattermostPayload> Create(MattermostMessage message);
    }
}
