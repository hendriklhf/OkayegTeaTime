using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Api.JsonConverters;

namespace OkayegTeaTime.Twitch.Api.Helix.Models;

[DebuggerDisplay("Username = \"{Username}\" Id = {Id}")]
public sealed class User : CachedModel, IEquatable<User>
{
    [JsonPropertyName("id")]
    [JsonConverter(typeof(Int64StringJsonConverter))]
    public required long Id { get; init; }

    [JsonPropertyName("login")]
    public required string Username { get; init; }

    [JsonPropertyName("display_name")]
    public required string DisplayName { get; init; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(UserTypeJsonConverter))]
    public required UserType UserType { get; init; }

    [JsonPropertyName("broadcaster_type")]
    [JsonConverter(typeof(BroadcasterTypeJsonConverter))]
    public required BroadcasterType BroadcasterType { get; init; }

    [JsonPropertyName("description")]
    public required string ChannelDescription { get; init; }

    [JsonPropertyName("profile_image_url")]
    public required string? ProfileImageUrl { get; init; }

    [JsonPropertyName("offline_image_url")]
    public required string? OfflineImageUrl { get; init; }

    [JsonPropertyName("view_count")]
    public required long ViewCount { get; init; }

    [JsonPropertyName("created_at")]
    public required DateTime TimeOfCreation { get; init; }

    public override bool Equals(object? obj)
    {
        return obj is User other && Equals(other);
    }

    public bool Equals(User? other)
    {
        return ReferenceEquals(this, other) || Id == other?.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(User? left, User? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(User? left, User? right)
    {
        return !(left == right);
    }
}
