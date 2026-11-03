namespace Serilog.Sinks.Mattermost.Options
{
    public sealed class MattermostFormatterOptions
    {
        public bool IncludeProperties { get; set; } = true;

        public bool IncludeException { get; set; } = true;

        public bool IncludeTimestamp { get; set; } = true;

        public bool IncludeLevel { get; set; } = true;

        public bool IncludeEmoji { get; set; } = true;

        public string OutputTemplate { get; set; } = "{Message:lj}";

        public TimeZoneInfo? TimeZone { get; set; }

        public EmojiOptions Emojis { get; set; } = new();

        public LevelColorOptions LevelColors { get; set; } = new();

        public bool UseAttachments { get; set; } = true;
    }
}
