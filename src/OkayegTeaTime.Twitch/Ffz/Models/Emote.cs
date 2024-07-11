using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Ffz.Models;

// ReSharper disable once UseNameofExpressionForPartOfTheString
[DebuggerDisplay("\"{Name}\"")]
public sealed class Emote : IEquatable<Emote>
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    public bool Equals(Emote? other) => ReferenceEquals(this, other) || Id == other?.Id;

    public override bool Equals(object? obj) => obj is Emote other && Equals(other);

    public override int GetHashCode() => Id;

    public static bool operator ==(Emote? left, Emote? right) => Equals(left, right);

    public static bool operator !=(Emote? left, Emote? right) => !(left == right);
}
