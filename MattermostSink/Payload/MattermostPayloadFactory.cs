

using Serilog.Sinks.Mattermost.Options;

namespace Serilog.Sinks.Mattermost.Payload; 

internal sealed class MattermostPayloadFactory(
    MattermostSinkOptions options)
    : IMattermostPayloadFactory
{

    public IEnumerable<MattermostPayload> Create(MattermostMessage message)
    {
        if (message == null)
            yield break;

        yield return new MattermostPayload
        {
            Text = message.Text,
            Attachments = message.Attachments,
            Username = options.Username,
            Channel = options.Channel,
            IconEmoji = options.IconEmoji
        };
    }
}
