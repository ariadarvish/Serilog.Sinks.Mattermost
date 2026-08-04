

namespace Serilog.Sinks.Mattermost.Options
{
    public sealed class EmojiOptions
    {
        public string Verbose { get; set; } = "🔍";
        public string Debug { get; set; } = "🐞";
        public string Information { get; set; } = "ℹ️";
        public string Warning { get; set; } = "⚠️";
        public string Error { get; set; } = "❌";
        public string Fatal { get; set; } = "🔥";
    }
}
