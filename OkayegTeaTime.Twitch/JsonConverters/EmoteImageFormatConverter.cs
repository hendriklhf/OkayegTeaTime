using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Helix.Models;

namespace OkayegTeaTime.Twitch.JsonConverters;

public sealed class EmoteImageFormatConverter : JsonConverter<EmoteImageFormats>
{
    public override EmoteImageFormats Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        EmoteImageFormats result = 0;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                continue;
            }

            result |= reader.ValueSpan[0] switch
            {
                (byte)'s' => EmoteImageFormats.Static,
                (byte)'a' => EmoteImageFormats.Animated,
                _ => 0
            };
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, EmoteImageFormats value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        if ((value & EmoteImageFormats.Static) == EmoteImageFormats.Static)
        {
            writer.WriteStringValue(Emote._imageFormatValues[EmoteImageFormats.Static]);
        }

        if ((value & EmoteImageFormats.Animated) == EmoteImageFormats.Animated)
        {
            writer.WriteStringValue(Emote._imageFormatValues[EmoteImageFormats.Animated]);
        }

        writer.WriteEndArray();
    }
}
