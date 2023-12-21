using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Helix.Models.Responses;

public readonly struct GetResponse<T> : IEquatable<GetResponse<T>>
{
    [JsonPropertyName("data")]
    public required ImmutableArray<T> Items { get; init; } = [];

    public GetResponse()
    {
    }

    public bool Equals(GetResponse<T> other) => Items == other.Items;

    public override bool Equals(object? obj) => obj is GetResponse<T> other && Equals(other);

    public override int GetHashCode() => Items.GetHashCode();

    public static bool operator ==(GetResponse<T> left, GetResponse<T> right) => left.Equals(right);

    public static bool operator !=(GetResponse<T> left, GetResponse<T> right) => !(left == right);
}
