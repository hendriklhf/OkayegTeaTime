using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Helix.Models;

namespace OkayegTeaTime.Twitch.Json.Converters;

public sealed class UserTypeJsonConverter : JsonConverter<UserType>
{
    public override UserType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.ValueSpan.Length == 0)
        {
            return UserType.Normal;
        }

        return reader.ValueSpan[0] switch
        {
            (byte)'a' => UserType.Admin,
            (byte)'g' => UserType.GlobalMod,
            (byte)'s' => UserType.Staff,
            _ => UserType.Normal
        };
    }

    public override void Write(Utf8JsonWriter writer, UserType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            UserType.Normal => string.Empty,
            UserType.Admin => "admin",
            UserType.GlobalMod => "global_mod",
            UserType.Staff => "staff",
            _ => string.Empty
        });
}
