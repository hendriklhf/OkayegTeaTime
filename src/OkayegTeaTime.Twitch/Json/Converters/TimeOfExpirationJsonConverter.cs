using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using HLE.Numerics;
using OkayegTeaTime.Twitch.Helix.Models;

namespace OkayegTeaTime.Twitch.Json.Converters;

public sealed class TimeOfExpirationJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        int expiresInSeconds = NumberHelpers.ParsePositiveNumber<int>(reader.ValueSpan);
        TimeSpan expiresIn = TimeSpan.FromMilliseconds(expiresInSeconds);
#pragma warning disable S6354
        return DateTime.UtcNow + expiresIn;
#pragma warning restore S6354
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => throw new NotSupportedException($"The property {nameof(AccessToken)}.{nameof(AccessToken.TimeOfExpiration)} is not available for serialization.");
}
