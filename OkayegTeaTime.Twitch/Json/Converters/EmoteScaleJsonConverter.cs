using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Helix.Models;

namespace OkayegTeaTime.Twitch.Json.Converters;

public sealed class EmoteScaleJsonConverter : JsonConverter<EmoteScales>
{
    public override EmoteScales Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        EmoteScales result = 0;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                continue;
            }

            result |= reader.ValueSpan[0] switch
            {
                (byte)'1' => EmoteScales.One,
                (byte)'2' => EmoteScales.Two,
                (byte)'3' => EmoteScales.Three,
                _ => 0
            };
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, EmoteScales value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        if ((value & EmoteScales.One) != 0)
        {
            writer.WriteStringValue(Emote.s_scaleValues[EmoteScales.One]);
        }

        if ((value & EmoteScales.Two) != 0)
        {
            writer.WriteStringValue(Emote.s_scaleValues[EmoteScales.Two]);
        }

        if ((value & EmoteScales.Three) != 0)
        {
            writer.WriteStringValue(Emote.s_scaleValues[EmoteScales.Three]);
        }

        writer.WriteEndArray();
    }
}
