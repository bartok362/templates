# Logging

## Introduction
The purpose of these projects is to show the setup and usage of `Microsoft.Extensions.Logging` in combination with the Microsoft's default Logger implementation as well as with Serilog.

## TLDR Usage of ILogger

By applying the correct usage of the `ILogger` abstraction, the concrete logging framework can be easily switched out.
Check belows examples and comments.

```
// Do NOT use string interpolation / string.Format() to construct the message
// Use properties in the message template using {} and let the logger input the data (this preserves the message template)
// Additional objects not used in the message are NOT added as properties in application insights
// the @ instructs Serilog to deconstruct the object as json

logger.LogDebug("Accessed at {timestampUtc} by {@user}", DateTime.UtcNow, DataProvider.User);
```
```
// Serilog honors scopes just as the default logger
// it is important that the scope is of type Dictionary<string, object>, otherwise the properties are not correctly added in application insights
// the '@' is important to instruct the logger to deconstruct the object, otherwise ToString() is used
// Since this notation is not so handy, we can add an extension method as shown in LoggerExtensions
// The message should be helpful even without the properties, because not all sinks/output targets can display those properly

using (logger.BeginScope(new Dictionary<string, object> { ["@data"] = data }))
{
    logger.LogInformation("Computed something with id {id}", data.Id);
}
```

## Microsoft.Extensions.Logging
Microsofts default logging provider is automatically registered when using the default `HostBuilder` in Program.cs.
It can be further configured using the `appsettings.json` configuration file. See StructuredLogging.Microsoft.
```
  "Logging": {
    "Console": {
      "IncludeScopes": true
    },
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Debug",
        "Microsoft": "Warning"
      }
    }
  }
```

## Serilog
In ASP.NET Core projects, Serilog can be added by including the the package `Serilog.AspNetCore`.  
Additionally for Application Insights `Serilog.Sinks.ApplicationInsights` is required.  
Serilog is configured at the `HostBuilder` in Program.cs.

```
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
        .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Destructure.With()
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            // properties can be added to the message template by adding '{Properties}' to the outputTemplate
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.ApplicationInsights(TelemetryConverter.Traces)
        );
```

Serilog uses its own configuration settings in `appsettings.json`, see StructuredLogging.Serilog.
Microsoft's default configuration is not used.

```
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
```