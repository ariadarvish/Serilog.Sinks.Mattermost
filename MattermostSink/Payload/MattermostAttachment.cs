

using System.Text.Json.Serialization;

namespace Serilog.Sinks.Mattermost.Payload
{
    internal sealed class MattermostAttachment
    {
        public string? Color { get; set; }

        public string? Title { get; set; }

        public string? Text { get; set; }

        public string? Footer { get; set; }

        [JsonPropertyName("ts")]
        public long? Timestamp { get; set; }
    }
}
