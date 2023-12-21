using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Helix.Models;

namespace OkayegTeaTime.Twitch.Json.Converters;

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
        if ((value & EmoteImageFormats.Static) != 0)
        {
            writer.WriteStringValue(Emote.s_imageFormatValues[EmoteImageFormats.Static]);
        }

        if ((value & EmoteImageFormats.Animated) != 0)
        {
            writer.WriteStringValue(Emote.s_imageFormatValues[EmoteImageFormats.Animated]);
        }

        writer.WriteEndArray();
    }
}
