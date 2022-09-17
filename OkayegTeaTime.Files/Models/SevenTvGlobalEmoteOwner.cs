#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public sealed class SevenTvGlobalEmoteOwner
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("login")]
    public string Username { get; set; }

    [JsonPropertyName("twitch_id")]
    public string TwitchId { get; set; }
}
