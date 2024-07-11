using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Bttv.Models.Responses;

public readonly struct GetUserResponse : IEquatable<GetUserResponse>
{
    [JsonPropertyName("channelEmotes")]
    public required ImmutableArray<Emote> ChannelEmotes { get; init; } = [];

    [JsonPropertyName("sharedEmotes")]
    public required ImmutableArray<Emote> SharedEmotes { get; init; } = [];

    public GetUserResponse()
    {
    }

    public bool Equals(GetUserResponse other) => ChannelEmotes.Equals(other.ChannelEmotes) && SharedEmotes.Equals(other.SharedEmotes);

    public override bool Equals(object? obj) => obj is GetUserResponse other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(ChannelEmotes, SharedEmotes);

    public static bool operator ==(GetUserResponse left, GetUserResponse right) => left.Equals(right);

    public static bool operator !=(GetUserResponse left, GetUserResponse right) => !left.Equals(right);
}
