using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Models;

public sealed class RedditPost : CachedModel
{
    [JsonPropertyName("subreddit")]
    public required string SubReddit { get; init; }

    [JsonPropertyName("selftext")]
    public required string SelfText { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("over_18")]
    public required bool IsNsfw { get; init; }

    [JsonPropertyName("spoiler")]
    public required bool IsSpoiler { get; init; }

    [JsonPropertyName("url")]
    public required string Url { get; init; }

    [JsonPropertyName("stickied")]
    public required bool Pinned { get; init; }

    [JsonPropertyName("score")]
    public required int Score { get; init; }
}
