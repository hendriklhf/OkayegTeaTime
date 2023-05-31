using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Api.JsonConverters;

namespace OkayegTeaTime.Twitch.Api.Helix.Models;

[DebuggerDisplay("Username = \"{Username}\" Title = \"{Title}\"")]
public sealed class Stream : CachedModel, IEquatable<Stream>
{
    [JsonPropertyName("id")]
    [JsonConverter(typeof(Int64StringJsonConverter))]
    public required long Id { get; init; }

    [JsonPropertyName("user_id")]
    [JsonConverter(typeof(Int64StringJsonConverter))]
    public required long UserId { get; init; }

    [JsonPropertyName("user_login")]
    public required string Username { get; init; }

    [JsonPropertyName("user_name")]
    public required string DisplayName { get; init; }

    [JsonPropertyName("game_id")]
    [JsonConverter(typeof(Int64StringJsonConverter))]
    public required long GameId { get; init; }

    [JsonPropertyName("game_name")]
    public required string GameName { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("tags")]
    public required string[] Tags { get; init; }

    [JsonPropertyName("viewer_count")]
    public required int ViewerCount { get; init; }

    [JsonPropertyName("started_at")]
    public required DateTime StartedAt { get; init; }

    [JsonPropertyName("language")]
    public required string Language { get; init; }

    [JsonPropertyName("thumbnail_url")]
    public required string ThumbnailUrl { get; init; }

    [JsonPropertyName("is_mature")]
    public required bool IsMature { get; init; }

    public bool Equals(Stream? other)
    {
        return ReferenceEquals(this, other) || Id == other?.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Stream other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Stream? left, Stream? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Stream? left, Stream? right)
    {
        return !(left == right);
    }
}
