
using Serilog.Events;
using Serilog.Sinks.Mattermost.Payload;

namespace Serilog.Sinks.Mattermost.Formatting
{
    internal interface IMattermostFormatter
    {
        MattermostMessage FormatBatch(IEnumerable<LogEvent> logEvents);
    }
}
