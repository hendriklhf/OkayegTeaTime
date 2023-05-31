using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Api.Bttv.Models;

// ReSharper disable once UseNameofExpressionForPartOfTheString
[DebuggerDisplay("\"{Name}\"")]
public sealed class Emote : CachedModel, IEquatable<Emote>
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("code")]
    public required string Name { get; init; }

    public bool Equals(Emote? other)
    {
        return ReferenceEquals(this, other) || Id == other?.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Emote other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
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
