using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.SevenTv.Models;

[DebuggerDisplay("\"{ToString()}\"")]
public sealed class Emote : IEquatable<Emote>
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    public override string ToString()
    {
        return Name;
    }

    public bool Equals(Emote? other)
    {
        return ReferenceEquals(this, other) || Id == other?.Id && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is Emote other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }

    public static bool operator ==(Emote? left, Emote? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Emote? left, Emote? right)
    {
        return !(left == right);
    }
}
