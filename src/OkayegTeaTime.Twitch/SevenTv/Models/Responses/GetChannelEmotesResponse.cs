using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.SevenTv.Models.Responses;

public readonly struct GetChannelEmotesResponse : IEquatable<GetChannelEmotesResponse>
{
    [JsonPropertyName("emote_set")]
    public required EmoteSet EmoteSet { get; init; } = EmoteSet.Empty;

    public GetChannelEmotesResponse()
    {
    }

    public bool Equals(GetChannelEmotesResponse other) => EmoteSet.Equals(other.EmoteSet);

    public override bool Equals(object? obj) => obj is GetChannelEmotesResponse other && Equals(other);

    public override int GetHashCode() => EmoteSet.GetHashCode();

    public static bool operator ==(GetChannelEmotesResponse left, GetChannelEmotesResponse right) => left.Equals(right);

    public static bool operator !=(GetChannelEmotesResponse left, GetChannelEmotesResponse right) => !left.Equals(right);
}
