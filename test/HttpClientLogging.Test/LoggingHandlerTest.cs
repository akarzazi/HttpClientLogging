using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using HttpClientLogging.Test.TestUtils;

using Microsoft.Extensions.Logging;

using Xunit;

namespace HttpClientLogging.Test
{
    public class LoggingHandlerTest
    {
        [Fact]
        public void GET_Ok_With_Headers()
        {
            var logSpy = new StringLogger();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com");
            httpRequestMessage.Headers.Add("Header1", "HeaderValue1");
            httpRequestMessage.Headers.Add("Header2", "HeaderValue2");

            var handler = new LoggingHandler(logSpy, LogLevel.Debug)
            {
                InnerHandler = new TestHandler((r, c) =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    return Task.FromResult(response);
                })
            };

            var client = new HttpClient(handler);
            var result = client.SendAsync(httpRequestMessage).Result;

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var logs = logSpy.Logs[LogLevel.Debug];
            Assert.Contains("http://test.com", logs);
            Assert.Contains("GET", logs);
            Assert.Contains("OK", logs);
            Assert.Contains("200", logs);
            Assert.Contains("Header1", logs);
            Assert.Contains("HeaderValue1", logs);
            Assert.Contains("Header2", logs);
            Assert.Contains("HeaderValue2", logs);
        }

        [Fact]
        public void GET_Ok_With_Body()
        {
            var logSpy = new StringLogger();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            var handler = new LoggingHandler(logSpy, LogLevel.Debug)
            {
                InnerHandler = new TestHandler((r, c) =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent("Sample Content Body");
                    return Task.FromResult(response);
                })
            };

            var client = new HttpClient(handler);
            var result = client.SendAsync(httpRequestMessage).Result;

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Sample Content Body", result.Content.ReadAsStringAsync().Result);

            var logs = logSpy.Logs[LogLevel.Debug];
            Assert.Contains("http://test.com", logs);
            Assert.Contains("GET", logs);
            Assert.Contains("OK", logs);
            Assert.Contains("200", logs);
            Assert.Contains("Sample Content Body", logs);
        }

        [Fact]
        public void GET_Response_404()
        {
            var logSpy = new StringLogger();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            var handler = new LoggingHandler(logSpy, LogLevel.Debug)
            {
                InnerHandler = new TestHandler((r, c) =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return Task.FromResult(response);
                })
            };

            var client = new HttpClient(handler);
            var result = client.SendAsync(httpRequestMessage).Result;

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            var logs = logSpy.Logs[LogLevel.Debug];
            Assert.Contains("http://test.com", logs);
            Assert.Contains("404", logs);
        }

        [Fact]
        public void GET_Response_500()
        {
            var logSpy = new StringLogger();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            var handler = new LoggingHandler(logSpy, LogLevel.Debug)
            {
                InnerHandler = new TestHandler((r, c) =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    return Task.FromResult(response);
                })
            };

            var client = new HttpClient(handler);
            var result = client.SendAsync(httpRequestMessage).Result;

            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

            var logs = logSpy.Logs[LogLevel.Debug];
            Assert.Contains("http://test.com", logs);
            Assert.Contains("500", logs);
        }

        [Fact]
        public void Post_Ok_With_Body()
        {
            var logSpy = new StringLogger();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://test.com");
            httpRequestMessage.Content = new StringContent("Sample Payload");

            var handler = new LoggingHandler(logSpy, LogLevel.Debug)
            {
                InnerHandler = new TestHandler((r, c) =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent("Sample Content Body");
                    return Task.FromResult(response);
                })
            };

            var client = new HttpClient(handler);
            var result = client.SendAsync(httpRequestMessage).Result;

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Sample Content Body", result.Content.ReadAsStringAsync().Result);

            var logs = logSpy.Logs[LogLevel.Debug];
            Assert.Contains("http://test.com", logs);
            Assert.Contains("POST", logs);
            Assert.Contains("Sample Payload", logs);
            Assert.Contains("OK", logs);
            Assert.Contains("200", logs);
            Assert.Contains("Sample Content Body", logs);
        }

        [Fact]
        public void Log_With_Info_Level()
        {
            var logSpy = new StringLogger();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            var handler = new LoggingHandler(logSpy, LogLevel.Information)
            {
                InnerHandler = new TestHandler((r, c) =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    return Task.FromResult(response);
                })
            };

            var client = new HttpClient(handler);
            var result = client.SendAsync(httpRequestMessage).Result;

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var logs = logSpy.Logs[LogLevel.Information];
            Assert.Contains("http://test.com", logs);
            Assert.Contains("GET", logs);
            Assert.Contains("OK", logs);
            Assert.Contains("200", logs);
        }

        [Fact]
        public void Disabled_Logger_Nothing_Logged()
        {
            var logSpy = new StringLogger();
            logSpy.Enabled = false;

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com");
            var handler = new LoggingHandler(logSpy, LogLevel.Information)
            {
                InnerHandler = new TestHandler((r, c) =>
                {
                    var response = new HttpResponseMessage();
                    return Task.FromResult(response);
                })
            };

            var client = new HttpClient(handler);
            var result = client.SendAsync(httpRequestMessage).Result;

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Empty(logSpy.Logs);
        }
    }
}
