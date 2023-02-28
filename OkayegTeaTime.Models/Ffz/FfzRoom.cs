#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class FfzRoom
{
    [JsonPropertyName("_id")]
    public int Id { get; set; }

    [JsonPropertyName("twitch_id")]
    public long TwitchUserId { get; set; }

    [JsonPropertyName("youtube_id")]
    public int? YoutubeUserId { get; set; }

    [JsonPropertyName("id")]
    public string Username { get; set; }

    [JsonPropertyName("is_group")]
    public bool IsGroup { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("set")]
    public int SetId { get; set; }

    [JsonPropertyName("moderator_badge")]
    public string ModeratorBadgeUrl { get; set; }

    [JsonPropertyName("vip_badge")]
    public FfzUrls VipBadgeUrl { get; set; }

    [JsonPropertyName("mod_urls")]
    public FfzUrls ModeratorBadgeUrls { get; set; }

    [JsonPropertyName("user_badges")]
    public FfzBotBadges BotBadges { get; set; }

    [JsonPropertyName("user_badge_ids")]
    public FfzBotBadgeIds BotBadgeIds { get; set; }

    [JsonPropertyName("css")]
    public object Css { get; set; }
}
