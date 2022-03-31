using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;

public class SevenTvUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("login")]
    public string Username { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("role")]
    public SevenTvRole Role { get; set; }

    [JsonPropertyName("emote_aliases")]
    public List<string> EmoteAliases { get; set; }

    [JsonPropertyName("emotes")]
    public List<SevenTvEmote> Emotes { get; set; }

    [JsonPropertyName("owned_emotes")]
    public List<SevenTvEmote> OwnedEmotes { get; set; }

    [JsonPropertyName("emote_ids")]
    public List<string> EmoteIds { get; set; }

    [JsonPropertyName("editors")]
    public List<SevenTvMinimalUser> Editors { get; set; }

    [JsonPropertyName("editor_in")]
    public List<SevenTvMinimalUser> EditorIn { get; set; }

    [JsonPropertyName("follower_count")]
    public int FollowerCount { get; set; }

    [JsonPropertyName("broadcast")]
    public SevenTvBroadcast Broadcast { get; set; }

    [JsonPropertyName("twitch_id")]
    public string UserId { get; set; }

    [JsonPropertyName("broadcaster_type")]
    public string BroadcasterType { get; set; }

    [JsonPropertyName("profile_image_url")]
    public string ProfilePictureUrl { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("emote_slots")]
    public int EmoteSlotCount { get; set; }

    [JsonPropertyName("audit_entries")]
    public List<SevenTvAuditEntry> AuditEntries { get; set; }

    [JsonPropertyName("banned")]
    public bool IsBanned { get; set; }

    [JsonPropertyName("youtube_id")]
    public string YoutubeId { get; set; }
}
