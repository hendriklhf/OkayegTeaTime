using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Api.JsonConverters;

namespace OkayegTeaTime.Twitch.Api.Helix.Models;

// ReSharper disable once UseNameofExpressionForPartOfTheString
[DebuggerDisplay("\"{Name}\"")]
public sealed class ChannelEmote : Emote, IEquatable<ChannelEmote>
{
    [JsonPropertyName("emote_type")]
    [JsonConverter(typeof(EmoteTypeJsonConverter))]
    public required EmoteType Type { get; init; }

    [JsonPropertyName("emote_set_id")]
    public required string SetId { get; init; }

    [JsonPropertyName("tier")]
    [JsonConverter(typeof(EmoteTierJsonConverter))]
    public required EmoteTier Tier { get; init; }

    [Pure]
    public bool Equals(ChannelEmote? other)
    {
        return ReferenceEquals(this, other) || Id == other?.Id;
    }

    [Pure]
    public override bool Equals(object? obj)
    {
        return obj is ChannelEmote other && Equals(other);
    }

    [Pure]
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(ChannelEmote? left, ChannelEmote? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ChannelEmote? left, ChannelEmote? right)
    {
        return !(left == right);
    }
}
