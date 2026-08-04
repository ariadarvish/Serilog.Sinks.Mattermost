

namespace Serilog.Sinks.Mattermost.Options
{
    public sealed class MattermostFormatterOptions
    {
        public bool IncludeProperties { get; init; } = true;

        public bool IncludeException { get; init; } = true;

        public bool IncludeTimestamp { get; init; } = true;

        public bool IncludeLevel { get; init; } = true;

        public bool IncludeEmoji { get; init; } = true;
        public string OutputTemplate { get; set; } =
            "{Timestamp:yyyy-MM-dd HH:mm:ss}\n" +
            "[{Level}] {Message}{NewLine}{Exception}";

        public EmojiOptions Emojis { get; set; } = new();
        public LevelColorOptions LevelColors { get; set; } = new();
        public bool UseAttachments { get; set; } = true;
    }
}
