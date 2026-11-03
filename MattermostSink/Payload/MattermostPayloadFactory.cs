using Serilog.Sinks.Mattermost.Options;

namespace Serilog.Sinks.Mattermost.Payload
{
    internal sealed class MattermostPayloadFactory(
        MattermostSinkOptions options)
        : IMattermostPayloadFactory
    {
        public IEnumerable<MattermostPayload> Create(MattermostMessage message)
        {
            if (message == null)
            {
                yield break;
            }

            int maxLength = options.MaxMessageLength > 0 ? options.MaxMessageLength : 4000;

            // If message has text that exceeds maxLength, split into multiple payloads
            if (!string.IsNullOrEmpty(message.Text))
            {
                if (message.Text.Length > maxLength)
                {
                    var chunks = SplitText(message.Text, maxLength);
                    foreach (var chunk in chunks)
                    {
                        yield return new MattermostPayload
                        {
                            Text = chunk,
                            Username = options.Username,
                            Channel = options.Channel,
                            IconEmoji = options.IconEmoji
                        };
                    }
                }
                else
                {
                    yield return new MattermostPayload
                    {
                        Text = message.Text,
                        Username = options.Username,
                        Channel = options.Channel,
                        IconEmoji = options.IconEmoji
                    };
                }
            }

            // If message has attachments, ensure attachment texts do not exceed maxLength
            if (message.Attachments is { Count: > 0 })
            {
                var sanitizedAttachments = new List<MattermostAttachment>(message.Attachments.Count);
                foreach (var attachment in message.Attachments)
                {
                    sanitizedAttachments.Add(new MattermostAttachment
                    {
                        Color = attachment.Color,
                        Title = attachment.Title,
                        Text = TruncateText(attachment.Text, maxLength),
                        Footer = attachment.Footer,
                        Timestamp = attachment.Timestamp
                    });
                }

                yield return new MattermostPayload
                {
                    Attachments = sanitizedAttachments,
                    Username = options.Username,
                    Channel = options.Channel,
                    IconEmoji = options.IconEmoji
                };
            }
        }

        private static List<string> SplitText(string text, int maxChunkLength)
        {
            var result = new List<string>();
            int index = 0;
            while (index < text.Length)
            {
                int length = Math.Min(maxChunkLength, text.Length - index);
                result.Add(text.Substring(index, length));
                index += length;
            }
            return result;
        }

        private static string? TruncateText(string? text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            {
                return text;
            }

            const string suffix = "\n... [truncated]";
            int take = Math.Max(0, maxLength - suffix.Length);
            return text.Substring(0, take) + suffix;
        }
    }
}
