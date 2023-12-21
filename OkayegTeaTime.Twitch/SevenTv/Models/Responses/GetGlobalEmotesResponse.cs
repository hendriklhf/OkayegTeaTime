using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.SevenTv.Models.Responses;

public readonly struct GetGlobalEmotesResponse : IEquatable<GetGlobalEmotesResponse>
{
    [JsonPropertyName("emotes")]
    public required ImmutableArray<Emote> Emotes { get; init; }

    public bool Equals(GetGlobalEmotesResponse other) => Emotes.Equals(other.Emotes);

    public override bool Equals(object? obj) => obj is GetGlobalEmotesResponse other && Equals(other);

    public override int GetHashCode() => Emotes.GetHashCode();

    public static bool operator ==(GetGlobalEmotesResponse left, GetGlobalEmotesResponse right) => left.Equals(right);

    public static bool operator !=(GetGlobalEmotesResponse left, GetGlobalEmotesResponse right) => !left.Equals(right);
}
