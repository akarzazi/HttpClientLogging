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