using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HttpClientLogging
{
    internal class RequestLog
    {
        public DateTime RequestDateUtc { get; set; }

        public string? Url { get; set; }

        public IDictionary Headers { get; set; } = new Dictionary<object, object>();

        public string? Method { get; set; }

        [JsonConverter(typeof(AsIsConverter))]
        public string? Body { get; set; }
    }
}
