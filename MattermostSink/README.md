# Serilog.Sinks.Mattermost

A structured **Serilog sink for Mattermost incoming webhooks and bot accounts**.

`Serilog.Sinks.Mattermost` delivers structured application logs to Mattermost with:

* Rich Mattermost attachments
* Custom log level colors & emojis
* Timezone support (configurable or UTC/Local)
* Clean exception stack traces
* Structured properties formatting
* Periodic batching with backpressure controls
* Token bucket rate limiting
* Incoming webhook and Bot token authentication
* Message chunking/truncation for Mattermost post character limits
* Seamless Microsoft Dependency Injection integration

---

## Features

- ✅ **Webhook & Bot Mode**: Supports both standard incoming webhooks and bot access tokens (`/api/v4/posts`).
- ✅ **Asynchronous Batched Delivery**: Groups logs and sends them periodically without blocking application execution.
- ✅ **Rate Limiting**: Integrated token bucket rate limiter to respect Mattermost server policies.
- ✅ **Customizable Formatting**: Configure attachments, output templates, emojis, colors, and timestamps.
- ✅ **Extensible Architecture**: Replace formatters (`IMattermostFormatter`), payload factories (`IMattermostPayloadFactory`), or HTTP transports (`IMattermostClient`).
- ✅ **.NET 8, .NET 9 & .NET 10 Support**: Multi-targeted for current, LTS, and preview .NET runtimes.

---

## Installation

Install via NuGet:

```bash
dotnet add package Serilog.Sinks.Mattermost
```

---

## Configuration

### Option 1: Incoming Webhook (`appsettings.json`)

```json
{
  "Mattermost": {
    "WebhookUrl": "https://mattermost.example.com/hooks/xxxxxxxxxxxx",
    "Username": "My Application",
    "Channel": "application-logs",
    "IconEmoji": ":robot_face:",
    "MinimumLevel": "Warning",
    "BatchSize": 20,
    "Period": "00:00:05",
    "QueueLimit": 10000,
    "RateLimitTokens": 20,
    "RateLimitPeriod": "00:00:10",
    "FormatterOptions": {
      "UseAttachments": true,
      "IncludeLevel": true,
      "IncludeTimestamp": true,
      "IncludeProperties": true,
      "IncludeException": true,
      "IncludeEmoji": true,
      "OutputTemplate": "{Message:lj}"
    }
  }
}
```

### Option 2: Bot Account (`appsettings.json`)

```json
{
  "Mattermost": {
    "UseBot": true,
    "MattermostUrl": "https://mattermost.example.com",
    "BotAccessToken": "your-bot-access-token",
    "BotChannelId": "your-target-channel-id",
    "Username": "LoggingBot",
    "MinimumLevel": "Error"
  }
}
```

---

## Usage

### 1. Fluent Code Configuration

```csharp
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Mattermost.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Mattermost(options =>
    {
        options.WebhookUrl = "https://mattermost.example.com/hooks/xxxxxxxxxxxx";
        options.Username = "App Logger";
        options.MinimumLevel = LogEventLevel.Warning;
        options.BatchSize = 10;
        options.Period = TimeSpan.FromSeconds(5);
    })
    .CreateLogger();

Log.Information("Application started");
Log.Error(new InvalidOperationException("Database unavailable"), "Application failure");

Log.CloseAndFlush();
```

### 2. Dependency Injection Integration

```csharp
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Mattermost.DependencyInjection;
using Serilog.Sinks.Mattermost.Extensions;

var builder = Host.CreateApplicationBuilder(args);

// Register Mattermost logging services & bind options from configuration
builder.Services.AddMattermostLogging(builder.Configuration);

var host = builder.Build();

// Configure Serilog with registered DI services
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Mattermost(host.Services)
    .CreateLogger();

Log.Information("Service running with Mattermost sink!");

await host.RunAsync();
Log.CloseAndFlush();
```

---

## Advanced Options

### Formatting Options

| Option | Type | Default | Description |
|---|---|---|---|
| `UseAttachments` | `bool` | `true` | Render logs as Mattermost attachments. When `false`, posts markdown messages separated by dividers. |
| `IncludeTimestamp` | `bool` | `true` | Prefix log messages with a timestamp. |
| `TimeZone` | `TimeZoneInfo?` | `Utc` | Optional timezone for timestamp formatting. |
| `IncludeLevel` | `bool` | `true` | Display the log event level (e.g., Information, Error). |
| `IncludeEmoji` | `bool` | `true` | Display level-specific emoji. |
| `IncludeProperties` | `bool` | `true` | Render structured log event properties as a list. |
| `IncludeException` | `bool` | `true` | Render exception stack traces in a code block. |
| `OutputTemplate` | `string` | `"{Message:lj}"` | Template used by Serilog message text formatter. |

### Batching & Resilience

| Option | Type | Default | Description |
|---|---|---|---|
| `BatchSize` | `int` | `20` | Maximum number of events grouped per batch. |
| `Period` | `TimeSpan` | `5 seconds` | Interval between batch dispatches. |
| `QueueLimit` | `int` | `10000` | Maximum queued log events during backpressure. |
| `RateLimitTokens` | `int` | `20` | Token limit for outgoing HTTP requests. |
| `RateLimitPeriod` | `TimeSpan` | `10 seconds` | Token replenishment period. |
| `MaxMessageLength` | `int` | `4000` | Maximum characters per post (matches Mattermost default). Automatically splits or truncates larger messages. |

---

## Customization & Extensibility

You can replace any part of the sink pipeline:

```csharp
// Custom Formatter
builder.Services.AddSingleton<IMattermostFormatter, MyCustomFormatter>();

// Custom HTTP Client / Transport
builder.Services.AddSingleton<IMattermostClient, MyCustomMattermostClient>();

// Custom Payload Factory
builder.Services.AddSingleton<IMattermostPayloadFactory, MyCustomPayloadFactory>();
```

---

## License

MIT License. See [LICENSE](LICENSE) for details.
