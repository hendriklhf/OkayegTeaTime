using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Api.Ffz.Models;

public readonly struct Room : IEquatable<Room>
{
    [JsonPropertyName("twitch_id")]
    public required long TwitchId { get; init; }

    [JsonPropertyName("id")]
    public required string TwitchUsername { get; init; }

    public static Room Empty => new()
    {
        TwitchId = 0,
        TwitchUsername = string.Empty
    };

    public bool Equals(Room other)
    {
        return TwitchId == other.TwitchId && TwitchUsername == other.TwitchUsername;
    }

    public override bool Equals(object? obj)
    {
        return obj is Room other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TwitchId, TwitchUsername);
    }

    public static bool operator ==(Room left, Room right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Room left, Room right)
    {
        return !(left == right);
    }
}
