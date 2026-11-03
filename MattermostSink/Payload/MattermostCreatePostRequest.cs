using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Serilog.Sinks.Mattermost.Payload
{
    internal sealed class MattermostCreatePostRequest
    {
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("props")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MattermostPostProps? Props { get; set; }
    }

    internal sealed class MattermostPostProps
    {
        [JsonPropertyName("attachments")]
        public IList<MattermostAttachment>? Attachments { get; set; }
    }
}
