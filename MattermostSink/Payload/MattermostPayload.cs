namespace Serilog.Sinks.Mattermost.Payload
{
    internal class MattermostPayload
    {
        public string? Text { get; set; }
        public string? Username { get; set; }
        public string? IconEmoji { get; set; }
        public string? Channel { get; set; }
        public IList<MattermostAttachment>? Attachments { get; set; }
    }
}
