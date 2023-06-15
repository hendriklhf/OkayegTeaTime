using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using HLE.Numerics;

namespace OkayegTeaTime.Twitch.JsonConverters;

internal sealed class Int64StringJsonConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return NumberHelper.ParsePositiveNumber<long>(reader.ValueSpan);
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        Span<char> charBuffer = stackalloc char[20];
        value.TryFormat(charBuffer, out int charsWritten);
        writer.WriteStringValue(charBuffer[..charsWritten]);
    }
}
