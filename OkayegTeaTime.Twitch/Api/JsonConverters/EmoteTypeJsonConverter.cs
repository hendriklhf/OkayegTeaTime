using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Api.Helix.Models;

namespace OkayegTeaTime.Twitch.Api.JsonConverters;

public sealed class EmoteTypeJsonConverter : JsonConverter<EmoteType>
{
    public override EmoteType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ValueSpan[0] switch
        {
            (byte)'b' => EmoteType.BitsTier,
            (byte)'f' => EmoteType.Follower,
            (byte)'s' => EmoteType.Subscription,
            _ => throw new InvalidOperationException($"Emote deserialization failed. Unknown {nameof(EmoteType)} in API response.")
        };
    }

    public override void Write(Utf8JsonWriter writer, EmoteType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            EmoteType.BitsTier => "bitstier",
            EmoteType.Follower => "follower",
            EmoteType.Subscription => "subscriptions",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Emote serialization failed. Unknown {nameof(EmoteType)}.")
        });
    }
}
