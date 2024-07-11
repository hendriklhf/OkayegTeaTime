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

    public override string ToString() => Name;

    public bool Equals(Emote? other) => ReferenceEquals(this, other) || Id == other?.Id && Name == other.Name;

    public override bool Equals(object? obj) => obj is Emote other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Id, Name);

    public static bool operator ==(Emote? left, Emote? right) => Equals(left, right);

    public static bool operator !=(Emote? left, Emote? right) => !(left == right);
}
