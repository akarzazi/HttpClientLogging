using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace HttpClientLogging
{
    public class LoggingHandler : DelegatingHandler
    {
        private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };

        private readonly ILogger _logger;

        public LogLevel LogLevel { get; private set; }

        public LoggingHandler(ILogger logger, LogLevel logLevel)
        {
            _logger = logger;
            LogLevel = logLevel;
        }

        public LoggingHandler(ILogger<LoggingHandler> logger)
            : this(logger, LogLevel.Debug)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!_logger.IsEnabled(LogLevel))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var body = request.Content == null ? null : await request.Content.ReadAsStringAsync();
            var response = await base.SendAsync(request, cancellationToken);
            var responseContent = response.Content == null ? null : await response.Content.ReadAsStringAsync();

            var log = new Log()
            {
                Request = new RequestLog
                {
                    RequestDateUtc = DateTime.UtcNow,
                    Url = request.RequestUri.AbsoluteUri,
                    Headers = request.Headers.ToDictionary(h => h.Key, h => h.Value),
                    Method = request.Method.Method,
                    Body = body,
                },

                Response = new ResponseLog
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value),
                    Body = responseContent,
                }
            };

            _logger.Log(LogLevel, JsonSerializer.Serialize(log, _jsonSerializerOptions));

            return response;
        }
    }
}
