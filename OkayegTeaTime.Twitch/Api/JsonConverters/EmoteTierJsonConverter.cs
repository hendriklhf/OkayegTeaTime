using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Api.Helix.Models;

namespace OkayegTeaTime.Twitch.Api.JsonConverters;

public sealed class EmoteTierJsonConverter : JsonConverter<EmoteTier>
{
    public override EmoteTier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.ValueSpan.Length == 0)
        {
            return EmoteTier.None;
        }

        return reader.ValueSpan[0] switch
        {
            (byte)'1' => EmoteTier.One,
            (byte)'2' => EmoteTier.Two,
            (byte)'3' => EmoteTier.Three,
            _ => EmoteTier.None
        };
    }

    public override void Write(Utf8JsonWriter writer, EmoteTier value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            EmoteTier.One => "1000",
            EmoteTier.Two => "2000",
            EmoteTier.Three => "3000",
            _ => string.Empty
        });
    }
}
