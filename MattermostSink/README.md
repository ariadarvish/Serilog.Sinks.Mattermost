# Serilog.Sinks.Mattermost

A structured **Serilog sink for Mattermost incoming webhooks**.

`Serilog.Sinks.Mattermost` sends application logs to Mattermost using webhook messages with:

* Rich Mattermost attachments
* Log level colors
* Level emojis
* Timestamp support
* Exception formatting
* Structured properties
* Configurable batching
* Minimum log level filtering
* Dependency injection support
* Custom formatter and client extensibility

---

## Features

✅ Serilog integration
✅ Mattermost incoming webhook support
✅ Asynchronous batched delivery
✅ Configurable batch size and interval
✅ Level-based attachment colors
✅ Custom emojis per log level
✅ Exception details
✅ Structured log properties
✅ Configurable output templates
✅ Dependency injection friendly
✅ Custom transport support
✅ Custom formatting support

---

# Installation

Install the package from NuGet:

```bash
dotnet add package Serilog.Sinks.Mattermost
```

---

# Configuration

## appsettings.json

Add a Mattermost section:

```json
{
  "Mattermost": {
    "WebhookUrl": "https://mattermost.example.com/hooks/xxxxxxxxxxxx",

    "Username": "My Application",
    "Channel": "application-logs",
    "IconEmoji": ":robot_face:",

    "MinimumLevel": "Warning",

    "BatchSize": 10,
    "QueueLimit": 1000,
    "Period": "00:00:05",

    "FormatterOptions": {
      "UseAttachments": true,

      "OutputTemplate": "{Message:lj}{NewLine}{Exception}",

      "IncludeLevel": true,
      "IncludeTimestamp": true,
      "IncludeProperties": true,
      "IncludeException": true,
      "IncludeEmoji": true,

      "Emojis": {
        "Verbose": "🔍",
        "Debug": "🐞",
        "Information": "ℹ️",
        "Warning": "⚠️",
        "Error": "❌",
        "Fatal": "🔥"
      },

      "LevelColors": {
        "Verbose": "#9E9E9E",
        "Debug": "#607D8B",
        "Information": "#2196F3",
        "Warning": "#FF9800",
        "Error": "#F44336",
        "Fatal": "#9C27B0"
      }
    }
  }
}
```

---

# Basic Usage

## Console Application

Example:

```csharp
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMattermostLogging(
    builder.Configuration);


var host = builder.Build();


Log.Logger =
    new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.Mattermost(host.Services)
        .CreateLogger();


Log.Information("Application started");

Log.Warning(
    "Disk space is getting low");


Log.Error(
    new InvalidOperationException("Database unavailable"),
    "Application failed");


Log.CloseAndFlush();
```

---

# Dependency Injection Setup

Register Mattermost services:

```csharp
builder.Services.AddMattermostLogging(
    builder.Configuration);
```

This registers:

* `IMattermostClient`
* `IMattermostFormatter`
* `IMattermostPayloadFactory`
* required options

---

# Output Example

Mattermost messages are sent using attachments.

Example:

```
🔥 Fatal

Time: 2026-08-02 15:17:26

error occurred during running app

Exception:

System.InvalidOperationException:
Operation is not valid due to the current state of the object.
```

The attachment color represents the log level:

| Level       | Color     |
| ----------- | --------- |
| Verbose     | Gray      |
| Debug       | Blue Gray |
| Information | Blue      |
| Warning     | Orange    |
| Error       | Red       |
| Fatal       | Purple    |

---

# Batching

The sink uses Serilog periodic batching internally.

Configure batching:

```json
{
  "Mattermost": {
    "BatchSize": 20,
    "QueueLimit": 500,
    "Period": "00:00:10"
  }
}
```

Meaning:

* `BatchSize`
  Maximum number of events sent in one batch.

* `QueueLimit`
  Maximum queued log events before dropping.

* `Period`
  Maximum wait time before sending a batch.

---

# Minimum Log Level

The minimum level can be configured:

```json
{
  "Mattermost": {
    "MinimumLevel": "Error"
  }
}
```

Available values:

```
Verbose
Debug
Information
Warning
Error
Fatal
```

Example:

```json
"MinimumLevel": "Warning"
```

will send:

✅ Warning
✅ Error
✅ Fatal

and ignore:

❌ Information
❌ Debug
❌ Verbose

---

# Custom Formatter

You can provide your own formatter.

Implement:

```csharp
public interface IMattermostFormatter
{
    MattermostMessage FormatBatch(
        IEnumerable<LogEvent> events);
}
```

Register your formatter:

```csharp
services.AddSingleton<IMattermostFormatter,
    MyCustomFormatter>();
```

Your formatter can customize:

* Titles
* Colors
* Markdown
* Attachments
* Exception rendering

---

# Custom Mattermost Client

The default implementation sends messages through HTTP webhooks.

You can replace it:

```csharp
services.AddSingleton<IMattermostClient,
    MyMattermostClient>();
```

Implement:

```csharp
public interface IMattermostClient
{
    Task SendAsync(
        MattermostPayload payload,
        CancellationToken cancellationToken = default);
}
```

Possible use cases:

* Proxy support
* Custom authentication
* Different Mattermost endpoints
* Additional monitoring

---

# Structured Properties

Properties are included automatically when enabled:

```json
{
  "FormatterOptions": {
    "IncludeProperties": true
  }
}
```

Example:

```csharp
Log.Information("User {UserId} logged in", userId);
```

Mattermost:

```
Properties

UserId: 12345
```

---

# Exception Handling

Enable exception output:

```json
{
  "FormatterOptions": {
    "IncludeException": true
  }
}
```

Example:

```csharp
try
{
    RunApplication();
}
catch(Exception ex)
{
    Log.Error(
        ex,
        "Application crashed");
}
```

Mattermost will display:

```
Exception

System.Exception:
Application crashed
...
```

---

# Configuration Without appsettings.json

You can also configure programmatically:

```csharp
var options =
    new MattermostSinkOptions
    {
        WebhookUrl =
            "https://mattermost/hooks/example",

        MinimumLevel =
            LogEventLevel.Warning
    };


Log.Logger =
    new LoggerConfiguration()
        .WriteTo.Mattermost(
            options)
        .CreateLogger();
```

---

# Design Overview

The package follows this pipeline:

```
Serilog
   |
   v
Mattermost Sink
   |
   v
Periodic Batch Processor
   |
   v
Formatter
   |
   v
Payload Factory
   |
   v
Mattermost Client
   |
   v
Mattermost Webhook
```

---

# Performance Considerations

The sink is designed for production use:

* Logs are sent asynchronously.
* Application threads are not blocked by HTTP calls.
* Events are grouped into batches.
* HTTP connections are reused through `HttpClient`.
* Large log bursts are controlled through queue limits.

---

# Requirements

* .NET 9+
* Serilog
* Mattermost Incoming Webhook

---

# License

This project is licensed under the MIT License.

---

# Contributing

Contributions are welcome.

Before submitting changes:

1. Add tests.
2. Keep public API changes backward compatible.
3. Update documentation.
4. Follow existing coding style.

---

# Roadmap

Planned improvements:

* Health checks
* Metrics support

---
