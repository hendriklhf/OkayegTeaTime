using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Ffz.Models.Responses;

public readonly struct GlobalSet : IEquatable<GlobalSet>
{
    [JsonPropertyName("emoticons")]
    public required ImmutableArray<Emote> Emotes { get; init; } = [];

    public GlobalSet()
    {
    }

    public bool Equals(GlobalSet other) => Emotes.Equals(other.Emotes);

    public override bool Equals(object? obj) => obj is GlobalSet other && Equals(other);

    public override int GetHashCode() => Emotes.GetHashCode();

    public static bool operator ==(GlobalSet left, GlobalSet right) => left.Equals(right);

    public static bool operator !=(GlobalSet left, GlobalSet right) => !left.Equals(right);
}
