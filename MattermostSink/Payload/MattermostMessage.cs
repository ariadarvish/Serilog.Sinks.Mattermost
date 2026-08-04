

namespace Serilog.Sinks.Mattermost.Payload
{
    internal sealed class MattermostMessage
    {
        public string? Text { get; set; }
        public List<MattermostAttachment> Attachments { get; } = new();
    }
}
