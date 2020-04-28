using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace HttpClientLogging
{
    internal class ResponseLog
    {
        public HttpStatusCode StatusCode { get; set; }

        public string? ReasonPhrase { get; set; }

        [JsonConverter(typeof(AsIsConverter))]
        public string? Body { get; set; }

        public IDictionary Headers { get; set; } = new Dictionary<object, object>();
    }
}
