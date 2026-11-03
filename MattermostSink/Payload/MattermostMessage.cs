namespace Serilog.Sinks.Mattermost.Payload
{
    public sealed class MattermostMessage
    {
        public string? Text { get; set; }
        public List<MattermostAttachment> Attachments { get; set; } = new();
    }
}
