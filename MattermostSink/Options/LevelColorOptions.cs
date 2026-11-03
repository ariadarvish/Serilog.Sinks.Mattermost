

namespace Serilog.Sinks.Mattermost.Options
{
    public sealed class LevelColorOptions
    {
        public string Verbose { get; set; } = "#9E9E9E";
        public string Debug { get; set; } = "#607D8B";
        public string Information { get; set; } = "#2196F3";
        public string Warning { get; set; } = "#FF9800";
        public string Error { get; set; } = "#F44336";
        public string Fatal { get; set; } = "#9C27B0";
    }
}
