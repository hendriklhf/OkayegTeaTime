#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class Room
{
    [JsonPropertyName("_id")]
    public int Id { get; set; }

    [JsonPropertyName("twitch_id")]
    public long TwitchUserId { get; set; }

    [JsonPropertyName("id")]
    public string Username { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("set")]
    public int SetId { get; set; }
}
