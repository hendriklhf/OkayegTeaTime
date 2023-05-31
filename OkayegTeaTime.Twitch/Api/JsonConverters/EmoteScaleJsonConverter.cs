using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Api.Helix.Models;

namespace OkayegTeaTime.Twitch.Api.JsonConverters;

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
        if ((value & EmoteScales.One) == EmoteScales.One)
        {
            writer.WriteStringValue(Emote._scaleValues[EmoteScales.One]);
        }

        if ((value & EmoteScales.Two) == EmoteScales.Two)
        {
            writer.WriteStringValue(Emote._scaleValues[EmoteScales.Two]);
        }

        if ((value & EmoteScales.Three) == EmoteScales.Three)
        {
            writer.WriteStringValue(Emote._scaleValues[EmoteScales.Three]);
        }

        writer.WriteEndArray();
    }
}
