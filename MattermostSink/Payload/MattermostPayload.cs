using System.Text.Json.Serialization;

namespace Serilog.Sinks.Mattermost.Payload
{
    public class MattermostPayload
    {
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        [JsonPropertyName("username")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Username { get; set; }

        [JsonPropertyName("icon_emoji")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IconEmoji { get; set; }

        [JsonPropertyName("channel")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Channel { get; set; }

        [JsonPropertyName("attachments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IList<MattermostAttachment>? Attachments { get; set; }
    }
}
