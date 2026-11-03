using System.Text.Json.Serialization;

namespace Serilog.Sinks.Mattermost.Payload
{
    public sealed class MattermostAttachment
    {
        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; set; }

        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        [JsonPropertyName("footer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Footer { get; set; }

        [JsonPropertyName("ts")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? Timestamp { get; set; }
    }
}
