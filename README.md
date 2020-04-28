# HttpClientLogging
Provides LoggingHandler as an implemetation of HttpMessageHandler that logs requests and responses into an ILogger instance.

![.NET Core](https://github.com/akarzazi/HttpClientLogging/workflows/.NET%20Core/badge.svg)

# Nuget Package
.Net Standard 2.1

https://www.nuget.org/packages/HttpClientLogging/

# Why

The main motivation behind this package is to provide an easy way to inspect HttpClient traffic.
It is especially helpful in the development stage.

# Usage

## Register with DI
A sample registration with a named HttpClient

```csharp
var services = new ServiceCollection();
 // register the handler
services.AddTransient<LoggingHandler>();

// affect the handler to the client
services.AddHttpClient("MyClient").AddHttpMessageHandler<LoggingHandler>();
```

That's all

## Customize log level and logger instance

By default, ```LoggingHandler``` logs with level ```LogLevel.Debug```.

It is possible to change this behavior using the constructor.

```csharp
services.AddTransient<LoggingHandler>(srv => new LoggingHandler(ILogger, LogLevel.Information));
```
Where ```ILogger``` is an instance of ```Microsoft.Extensions.Logging.ILogger```

# Demo sample

A sample demo project is also available in the sources at :
https://github.com/akarzazi/HttpClientLogging/tree/master/demo


```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;

using HttpClientLogging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demo
{
    public class Program
    {
        async static Task Main(string[] args)
        {
            ServiceProvider serviceProvider = BuildServices();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // named http client
            var namedClient = httpClientFactory.CreateClient("MyGithubClient");
            await namedClient.GetAsync("https://api.github.com/zen");
        }

        private static ServiceProvider BuildServices()
        {
            var services = new ServiceCollection();

            // 1 - customize injected instance
            services.AddTransient<LoggingHandler>(srv => new LoggingHandler(new ConsoleLogger(), LogLevel.Information));

            // 2 - Or use DI with defaults
            // services.AddTransient<LoggingHandler>();

            services.AddHttpClient("MyGithubClient", c =>
            {
                c.DefaultRequestHeaders.Add("User-Agent", "Anonymous-Guy");
            })
            .AddHttpMessageHandler<LoggingHandler>();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        public class ConsoleLogger : ILogger
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Console.WriteLine(formatter(state, exception));
            }

            public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
            public bool IsEnabled(LogLevel logLevel) => true;
        }
    }
}

```

Output

```
{
  "Request": {
    "RequestDateUtc": "2020-04-28T14:12:39.3076773Z",
    "Url": "https://api.github.com/zen",
    "Headers": {
      "User-Agent": [
        "Debug-Sample"
      ]
    },
    "Method": "GET",
    "Body": null
  },
  "Response": {
    "StatusCode": 200,
    "ReasonPhrase": "OK",
    "Body": "Design for failure.",
    "Headers": {
      "Date": [
        "Tue, 28 Apr 2020 14:12:40 GMT"
      ],
      ...
```