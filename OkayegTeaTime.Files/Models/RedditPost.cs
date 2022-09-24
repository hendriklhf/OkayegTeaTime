using System;
using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Models;

public sealed class RedditPost
{
    [JsonPropertyName("subreddit")]
    public string SubReddit { get; set; }

    [JsonPropertyName("selftext")]
    public string SelfText { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("over_18")]
    public bool IsNsfw { get; set; }

    [JsonPropertyName("spoiler")]
    public bool IsSpoiler { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("stickied")]
    public bool Pinned { get; set; }

    [JsonPropertyName("score")]
    public int Score { get; set; }

    public DateTime TimeOfRequest { get; } = DateTime.UtcNow;
}
