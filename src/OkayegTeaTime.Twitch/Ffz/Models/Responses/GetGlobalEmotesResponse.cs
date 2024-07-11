using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Ffz.Models.Responses;

public readonly struct GetGlobalEmotesResponse : IEquatable<GetGlobalEmotesResponse>
{
    [JsonPropertyName("sets")]
    public required Sets Sets { get; init; }

    public bool Equals(GetGlobalEmotesResponse other) => Sets.Equals(other.Sets);

    public override bool Equals(object? obj) => obj is GetGlobalEmotesResponse other && Equals(other);

    public override int GetHashCode() => Sets.GetHashCode();

    public static bool operator ==(GetGlobalEmotesResponse left, GetGlobalEmotesResponse right) => left.Equals(right);

    public static bool operator !=(GetGlobalEmotesResponse left, GetGlobalEmotesResponse right) => !left.Equals(right);
}
