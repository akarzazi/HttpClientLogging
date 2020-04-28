using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HttpClientLogging
{
    internal class AsIsConverter : JsonConverter<string>
    {
        private NoopEncoder _noopEncoder = new NoopEncoder();

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(JsonEncodedText.Encode(value, _noopEncoder));
        }

        private class NoopEncoder : JavaScriptEncoder
        {
            public override int MaxOutputCharactersPerInputCharacter => throw new NotImplementedException();

            public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
            {
                return 0;
            }

            public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
            {
                numberOfCharactersWritten = 0;
                return false;
            }

            public override bool WillEncode(int unicodeScalar)
            {
                return false;
            }
        }
    }
}
