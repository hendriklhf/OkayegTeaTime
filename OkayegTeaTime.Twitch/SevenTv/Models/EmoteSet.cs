using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.SevenTv.Models;

public readonly struct EmoteSet : IEquatable<EmoteSet>
{
    [JsonPropertyName("emotes")]
    public required ImmutableArray<Emote> Emotes { get; init; } = [];

    public static EmoteSet Empty => new()
    {
        Emotes = []
    };

    public EmoteSet()
    {
    }

    public bool Equals(EmoteSet other) => Emotes.Equals(other.Emotes);

    public override bool Equals(object? obj) => obj is EmoteSet other && Equals(other);

    public override int GetHashCode() => Emotes.GetHashCode();

    public static bool operator ==(EmoteSet left, EmoteSet right) => left.Equals(right);

    public static bool operator !=(EmoteSet left, EmoteSet right) => !left.Equals(right);
}
