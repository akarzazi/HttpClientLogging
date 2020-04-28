using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClientLogging
{
    internal class Log
    {
        public RequestLog? Request { get; set; }

        public ResponseLog? Response { get; set; }
    }
}
